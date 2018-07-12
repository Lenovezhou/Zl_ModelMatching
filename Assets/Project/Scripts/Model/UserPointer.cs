using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPointer : MonoBehaviour {
    private Enums.PointMode pm;
    private Material originmaterial;
    private Material highlightmaterial;
    private Material origin_userimprotpointermaterial;
    private Material highlight_userimprotpointermaterial;
    private Material origin_normal;
    private Material highlight_normal;

    private MeshRenderer selfmeshrender;
    [SerializeField]
    private int group;
    [SerializeField]
    private int index;

    private Action<Vector3, int, int> callback;


    ///记录自己被保存时的位置
    private Vector3 savedposition;

    public void SavedPosition()
    {
        savedposition = transform.localPosition;
        callback(transform.localPosition, group, index);
    }

    public void ResetLastSavedPosition()
    {
        transform.localPosition = savedposition;
    }

    public void TempChangePos(Vector3 pos)
    {
        transform.localPosition = pos;
    }

    public void Init(Material userorigin, Material userhigh, int group, int index, Vector3 localsclaer, Action<Vector3, int, int> callback)
    {
        originmaterial = userorigin;
        highlightmaterial = userhigh;

        this.group = group;
        this.index = index;

        transform.localScale = localsclaer;

        selfmeshrender = gameObject.GetComponent<MeshRenderer>();
        selfmeshrender.material = originmaterial;
        this.callback = callback;
    }



    public void SetMaterialOrigin()
    {
        selfmeshrender.material = originmaterial;
    }
    public void SetMaterialHighlight()
    {
        MSGCenter.Execute(Enums.ViewMode.ChangeTargetPos.ToString(), transform.position.ToString());
        selfmeshrender.material = highlightmaterial;
    }
    public void OnMouseDown()
    {
        SetMaterialHighlight();
        string type = Enums.MatchigPointGizmoControll.PointerDown.ToString();
        string message = MSGCenter.FormatMessage(group.ToString(), index.ToString(), transform.position.ToString());
        MSGCenter.Execute(type, message);
    }

}
