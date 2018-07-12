using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NormalPointer: MonoBehaviour
{
    private Enums.PointMode pm;
    private Material originmaterial;
    private Material highlightmaterial;
    private Material origin_userimprotpointermaterial;
    private Material highlight_userimprotpointermaterial;
    private Material origin_normal;
    private Material highlight_normal;
    //指示器使用材质
    private Dictionary<Enums.MatarialsUse, Material> materialmap = new Dictionary<Enums.MatarialsUse, Material>();

    private MeshRenderer selfmeshrender;
    [SerializeField]
    private int group;
    [SerializeField]
    private int index;
    private bool hasOwnedUserPointer = false;

    private bool hasOwnedUserPointerSave = false;

    [SerializeField]
    private GameObject owendpointerobj;
    private MeshRenderer owendpointerrender;
    private UserPointer owendchildpointer;

    //private Color origincolor, highlightcolor;

    private GameObject InstructionsGizmo;

    

    public void Init(Dictionary<Enums.MatarialsUse,Material> dic,int group,int index , Enums.PointMode pm,Vector3 localsclaer)
    {
        if (null !=  dic)
        {
            origin_normal = dic[Enums.MatarialsUse.NormalPoint_origin];
            highlight_normal = dic[Enums.MatarialsUse.NormalPoint_hight];
            origin_userimprotpointermaterial = dic[Enums.MatarialsUse.UserImportpoint_origin];
            highlight_userimprotpointermaterial = dic[Enums.MatarialsUse.UserImportpoint_hight];
            materialmap = dic;
        }

        this.group = group;
        this.index = index;
        this.pm = pm;

        switch (pm)
        {
            case Enums.PointMode.Normal:
                originmaterial = origin_normal;
                highlightmaterial = highlight_normal;
                break;
            case Enums.PointMode.UserImport:
                originmaterial = origin_userimprotpointermaterial;
                highlightmaterial = highlight_userimprotpointermaterial;
                break;
            default:
                break;
        }
        transform.localScale = localsclaer;

        selfmeshrender = gameObject.GetComponent<MeshRenderer>();
        selfmeshrender.material = originmaterial;
    }

    public void CreateOwnedUserPointer(Dictionary<Enums.MatarialsUse,Material> dic,Vector3 hitinfopoint,Transform parent,bool active = true,Action<Vector3,int,int> callback = null)
    {
        materialmap = dic;
        if (Canspawn(Enums.PointMode.Normal))
        {
            owendpointerobj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            owendpointerobj.GetComponent<MeshRenderer>().material.color = Color.yellow;

            owendpointerobj.transform.SetParent(parent.transform);
            owendpointerobj.transform.localPosition = hitinfopoint;

            owendpointerobj.gameObject.SetActive(active);

            CreateInstructionsGizmo(gameObject, owendpointerobj);

            owendpointerrender = owendpointerobj.GetComponent<MeshRenderer>();
            UserPointer userpointer = owendpointerobj.AddComponent<UserPointer>();
            Vector3 sclaer = parent.localScale;
            float s = 0.02f / Tool.UserImportScaler;
            Vector3 finalscaler = Vector3.one * s;
            userpointer.Init(origin_userimprotpointermaterial,highlight_userimprotpointermaterial, group, index, finalscaler, callback);
            owendchildpointer = userpointer;
            hasOwnedUserPointer = true;
        }
        else if (hasOwnedUserPointer)
        {
            owendchildpointer.TempChangePos(hitinfopoint);
            CreateInstructionsGizmo(gameObject, owendpointerobj);
        }
    }

    /// <summary>
    /// 创建标准点到用户输入点的关联
    /// </summary>
    /// <param name="self"></param>
    /// <param name="target"></param>
    private void CreateInstructionsGizmo(GameObject self,GameObject target)
    {
        if (!InstructionsGizmo)
        {
            GameObject obj = Resources.Load(Tool.InstructionsGizmoLocalPath) as GameObject;
            InstructionsGizmo = Instantiate(obj);
        }
        InstructionsGizmo.transform.SetParent(null);
        InstructionsGizmo.GetComponentInChildren<MeshRenderer>().material = materialmap[Enums.MatarialsUse.Indicator_Effect];
        InstructionsGizmo.transform.position = self.transform.position;
        InstructionsGizmo.transform.up = target.transform.position - self.transform.position;
        Vector3 endscaler = new Vector3(InstructionsGizmo.transform.localScale.x, Vector3.Distance(self.transform.position, target.transform.position)/2, InstructionsGizmo.transform.localScale.z);
        InstructionsGizmo.transform.localScale = endscaler;
        InstructionsGizmo.transform.SetParent(transform);
    }

    bool Canspawn(Enums.PointMode pm) { return !hasOwnedUserPointer && this.pm == pm; }
    public bool IsMe(int group,int index)
    {
        if (group == this.group && index == this.index)
        {
            return true;
        }
        return false;
    }

    public bool OwnedPointer()
    {
        if (owendpointerobj != null)
        {
            return owendpointerobj.GetComponent<UserPointer>() != null;
        }
        return false;
    }


    /// <summary>
    /// 显示或隐藏自己及指示器和用户输入点
    /// </summary>
    /// <param name="active">显示或隐藏</param>
    public void ShowOrDisabelme(bool active)
    {
        gameObject.SetActive(active);
        if (InstructionsGizmo)
        {
            InstructionsGizmo.SetActive(active);
        }
        if (owendpointerobj)
        {
            owendpointerobj.SetActive(active);
        }
    }

    ///删除存储的对应点并重置状态
    public void ReleaseOwned()
    {
        if (hasOwnedUserPointerSave)
        {
            RePlay();
        }
    }

    /// <summary>
    /// 下一编辑命令到来时检查自己的状态，若未存储则删除
    /// </summary>
    public void NextEditCheckThis()
    {
        if (!hasOwnedUserPointerSave)
        {
            RePlay();
        }
        else {
            owendchildpointer.ResetLastSavedPosition();
            CreateInstructionsGizmo(gameObject, owendpointerobj);
        }
    }

    /// <summary>
    /// 重选病例，清空状态
    /// </summary>
    public void RePlay()
    {
        if (owendpointerobj)
        {
            Destroy(owendpointerobj);
            Destroy(InstructionsGizmo);
            owendpointerrender = null;
            owendpointerobj = null;
            hasOwnedUserPointer = false;
            hasOwnedUserPointerSave = false;
        }
    }

    /// <summary>
    /// 外界调用，保存写入map
    /// </summary>
    public void SaveOwned()
    {
        owendchildpointer.SavedPosition();
        hasOwnedUserPointerSave = true;
    }

    public void SetMaterialOrigin()
    {
        selfmeshrender.material = originmaterial;
        if (owendchildpointer)
        {
            InstructionsGizmo.GetComponentInChildren<MeshRenderer>().material = materialmap[Enums.MatarialsUse.Indicator_Effect];
            owendchildpointer.SetMaterialOrigin();
        }
    }
    public void SetMaterialHighlight()
    {
        MSGCenter.Execute(Enums.ViewMode.ChangeTargetPos.ToString(), transform.position.ToString());
        selfmeshrender.material = highlightmaterial;
        if (owendchildpointer)
        {
            InstructionsGizmo.GetComponentInChildren<MeshRenderer>().material = materialmap[Enums.MatarialsUse.Indicator_origin];
            owendchildpointer.SetMaterialHighlight();
        }
    }

    void DotweenColor(Material _material, Color normal, Color worming)
    {        
        Sequence seq = DOTween.Sequence();
        seq.Append(_material.DOColor(worming, 0.5f).SetEase(Ease.Linear).SetLoops(5));

        seq.OnComplete(() => { _material.color = normal; });
    }


    public void OnMouseDown()
    {
        SetMaterialHighlight();
        string type = Enums.MatchigPointGizmoControll.PointerDown.ToString();
        string message = MSGCenter.FormatMessage(group.ToString(), index.ToString(), transform.position.ToString());
        MSGCenter.Execute(type, message);
    }
}
