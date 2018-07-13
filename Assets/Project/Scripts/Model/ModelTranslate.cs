using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class ModelTranslate : StateMachine {
    private float mouseoffsetx = 0;
    private float mouseoffsety = 0;
    [SerializeField]
    private Camera selfcamera;


    State ido = new State();
    State fixrotate = new State();
    State fixmove = new State();

    public void Init (Dictionary<Enums.MatarialsUse,Material> materialmap)
	{
        selfcamera = Camera.main;

        fixmove.OnUpdate = MoveUpdate;
        fixrotate.OnUpdate = RotateUpdata;

        state = ido;

        RevertoLastSave();

        MeshRenderer[] childmeshrenders = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < childmeshrenders.Length; i++)
        {
            childmeshrenders[i].material = materialmap[Enums.MatarialsUse.UserimportModel];
            childmeshrenders[i].gameObject.AddComponent<MeshCollider>();
            childmeshrenders[i].tag = "Player";
        }
        gameObject.tag = "Player";

        MSGCenter.Register (Enums.LeftMouseButtonControl.Move.ToString(), LeftMouseControlMode);
		MSGCenter.Register (Enums.LeftMouseButtonControl.Roate.ToString(), LeftMouseControlMode);
		MSGCenter.Register (Enums.LeftMouseAlive.TurnOff.ToString (), LeftMouseAlive);
		MSGCenter.Register (Enums.LeftMouseAlive.TurnOn.ToString (), LeftMouseAlive);

        MSGCenter.Register(Enums.ControllTransform.Rotate.ToString(), RotateCallback);
        MSGCenter.Register(Enums.ControllTransform.Translate.ToString(), TranslateCallback);

        MSGCenter.Register(Enums.MainProcess.MainProcess_RePlay.ToString(), Replay);
        
        //MSGCenter.Register(Enums.MainProcess.MainProcess_LoadUserDataDone.ToString(), RevertoLastSave);

    }


    private void Update()
    {
        OnUpdateState(Time.deltaTime);
    }


    public void RevertoLastSave()
    {
        Vector3 localpos = Tool.ImprotUserPos;
        Vector3 localeuler = Vector3.zero;
        PlayerDataCenter.LoclaUserData lud = PlayerDataCenter.CurrentLocaluserdata;
            localpos.x = lud.usermodellocalposX;
            localpos.y = lud.usermodellocalposY;
            localpos.z = lud.usermodellocalposZ;
            localeuler.x = lud.usermodellocaleulerangleX;
            localeuler.y = lud.usermodellocaleulerangleY;
            localeuler.z = lud.usermodellocaleulerangleZ;
        PlayerDataCenter.UpdataUserData(PlayerDataCenter.LoclaUserData.DataKey.Postion, localpos);
        PlayerDataCenter.UpdataUserData(PlayerDataCenter.LoclaUserData.DataKey.Eulerangel, localeuler);
        transform.localPosition = localpos;
        transform.localEulerAngles = localeuler;

        transform.localScale = Vector3.one * Tool.UserImportScaler;
    }

    //是否允许左键移动或旋转
    void LeftMouseAlive(string message)
	{
		Enums.LeftMouseAlive alive = (Enums.LeftMouseAlive)Enum.Parse(typeof(Enums.LeftMouseAlive), message);
        switch (alive)
        {
            case Enums.LeftMouseAlive.TurnOn:
                break;
            case Enums.LeftMouseAlive.TurnOff:
                state = ido;
                break;
            default:
                break;
        }
    }

	//UI点击当前是移动模式，还是旋转模式
	void LeftMouseControlMode(string message)
	{
		Enums.LeftMouseButtonControl Cm = (Enums.LeftMouseButtonControl)Enum.Parse(typeof(Enums.LeftMouseButtonControl), message);
        switch (Cm)
        {
            case Enums.LeftMouseButtonControl.Roate:
                state = fixrotate;
                break;
            case Enums.LeftMouseButtonControl.Move:
                state = fixmove;
                break;
            default:
                break;
        }
    }

    public ModelTranslate ResetCenterEnter(int id)
    {
        Destroy(transform.GetComponent<ModelTranslate>());
        GameObject tempparent = new GameObject("UserImprotparent" + id);
        Transform parent = transform;
        Vector3 postion = parent.position;
        Quaternion rotation = parent.rotation;
        Vector3 scale = parent.localScale;
        parent.position = Vector3.zero;
        parent.rotation = Quaternion.Euler(Vector3.zero);
        //parent.localScale = Vector3.one;


        Vector3 center = Vector3.zero;
        Renderer[] renders = parent.GetComponentsInChildren<MeshRenderer>();
        foreach (Renderer child in renders)
        {
            center += child.bounds.center;
        }
        center /= parent.GetComponentsInChildren<MeshRenderer>().Length;
        tempparent.transform.position = center;
        ModelTranslate realmt = tempparent.AddComponent<ModelTranslate>();
        //tempparent.transform.position = transform.position;
        tempparent.transform.SetParent(transform.parent);
        transform.SetParent(tempparent.transform);

        return realmt;
    }

    #region 注册事件回调

    float lastvalue;
    void RotateCallback(string _endvalue)
    {
        string endvalue = _endvalue.Split('*')[1];
        float end = float.Parse(endvalue);
        float rotatedir = end - lastvalue;
        transform.Rotate( selfcamera.transform.forward, rotatedir);
        lastvalue = end;
        Vector3 euler = transform.localEulerAngles;
        PlayerDataCenter.UpdataUserData(PlayerDataCenter.LoclaUserData.DataKey.Eulerangel, euler);
    }

    void TranslateCallback(string _direction)
    {
        string direction = _direction.Split('*')[1];
        Enums.ViewMode vm = (Enums.ViewMode)Enum.Parse(typeof(Enums.ViewMode), direction);
        Vector3 dir = Vector3.zero;
        switch (vm)
        {
            case Enums.ViewMode.Forword:
                break;
            case Enums.ViewMode.Left:
                dir = -selfcamera.transform.right;
                break;
            case Enums.ViewMode.Down:
                dir = -selfcamera.transform.up;
                break;
            case Enums.ViewMode.Right:
                dir = selfcamera.transform.right;
                break;
            case Enums.ViewMode.Up:
                dir = selfcamera.transform.up;
                break;
            default:
                break;
        }
        transform.localPosition += dir * 0.02f;
        Vector3 v = transform.localPosition;
        PlayerDataCenter.UpdataUserData(PlayerDataCenter.LoclaUserData.DataKey.Postion, v);
    }


    #endregion


    #region 状态机方法
    void MoveUpdate(float obj)
    {
        if (CanLeftMouseRotate())
        {
            mouseoffsetx = Input.GetAxis("Mouse X") * 0.01f;
            mouseoffsety = Input.GetAxis("Mouse Y") * 0.01f;
            Vector3 offset = mouseoffsetx * selfcamera.transform.right + mouseoffsety * selfcamera.transform.up;
            transform.localPosition += offset;
            Vector3 v = transform.localPosition;
            PlayerDataCenter.UpdataUserData(PlayerDataCenter.LoclaUserData.DataKey.Postion, v);
        }
    }
    void RotateUpdata(float obj)
    {
        if (CanLeftMouseRotate())
        {
            mouseoffsetx = Input.GetAxis("Mouse X") * 4f;
            mouseoffsety = Input.GetAxis("Mouse Y") * 0.04f;

            transform.Rotate(Vector3.Cross(selfcamera.transform.right, selfcamera.transform.up), -mouseoffsetx, Space.World);
            Vector3 euler = transform.localEulerAngles;
            PlayerDataCenter.UpdataUserData(PlayerDataCenter.LoclaUserData.DataKey.Eulerangel, euler);

        }
    }


    bool CanLeftMouseRotate()
    {
        return Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject();
    }


    #endregion


    private void Replay(string obj)
    {
        //保存用户设置到本地
        int id = PlayerDataCenter.Currentillnessdata.ID;
        PlayerDataCenter.CurrentLocaluserdata.isfill = true;
        string data = JsonHelper.ParseObjectToJson(PlayerDataCenter.CurrentLocaluserdata);
        Tool.UpdateLocalJsonFiles(id, data);
    }



    static Vector3 Test(Transform target)
    {
        Transform parent = target;
        Vector3 postion = parent.position;
        Quaternion rotation = parent.rotation;
        Vector3 scale = parent.localScale;
        parent.position = Vector3.zero;
        parent.rotation = Quaternion.Euler(Vector3.zero);
        parent.localScale = Vector3.one;

        Collider[] colliders = parent.GetComponentsInChildren<Collider>();
        foreach (Collider child in colliders)
        {
            DestroyImmediate(child);
        }
        Vector3 center = Vector3.zero;
        Renderer[] renders = parent.GetComponentsInChildren<Renderer>();
        foreach (Renderer child in renders)
        {
            center += child.bounds.center;
        }
        center /= parent.GetComponentsInChildren<Renderer>().Length;
        Bounds bounds = new Bounds(center, Vector3.zero);
        foreach (Renderer child in renders)
        {
            bounds.Encapsulate(child.bounds);
        }
        BoxCollider boxCollider = parent.gameObject.AddComponent<BoxCollider>();
        boxCollider.center = bounds.center - parent.position;
        boxCollider.size = bounds.size;

        parent.position = postion;
       // parent.rotation = rotation;
       // parent.localScale = scale;

        return boxCollider.size;

    }

   

}
