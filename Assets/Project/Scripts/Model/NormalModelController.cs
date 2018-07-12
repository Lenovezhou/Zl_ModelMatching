
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalModelController : MonoBehaviour {


    Dictionary<int, List<Transform>> pointmap = new Dictionary<int, List<Transform>>();


    NormalPointer lasthighlight;

    float selforiginsclaer;

    public Dictionary<int, List<Transform>> Init (Dictionary<Enums.MatarialsUse,Material> dic)
    {
        MSGCenter.Register(Enums.PointControll.All.ToString(), ShowGroupPoints);
        MSGCenter.Register(Enums.PointControll.Choise.ToString(), ShowGroupPoints);
        MSGCenter.Register(Enums.PointControll.ShutDownAll.ToString(), ShowGroupPoints);
        MSGCenter.Register(Enums.MatchigPointGizmoControll.Highlight.ToString(), HighLightPoint);

        MSGCenter.Register(Enums.ControllTransform.Scaler.ToString(), ScalerCallback);
        MSGCenter.Register(Enums.ControllTransform.ScalerX.ToString(), ScalerCallback);
        MSGCenter.Register(Enums.ControllTransform.ScalerY.ToString(), ScalerCallback);
        MSGCenter.Register(Enums.ControllTransform.ScalerZ.ToString(), ScalerCallback);

        //MSGCenter.Register(Enums.MainProcess.MainProcess_LoadUserDataDone.ToString(), RevertoLastSave);

        Material origin_normal = Instantiate(Resources.Load<Material>("Materials/origin_normal"));
        Material highlight_normal = Instantiate(Resources.Load<Material>("Materials/highlight_normal"));
        Material origin_userimprot = Instantiate(Resources.Load<Material>("Materials/origin_userimprot"));
        Material highlight_userimport = Instantiate(Resources.Load<Material>("Materials/highlight_userimport"));

        LoadNormalPoints(transform.position, dic);

        transform.localScale = Vector3.one * Tool.UserImportScaler;

        //RevertoLastSave();

        selforiginsclaer = transform.localScale.x;

        return pointmap;
    }


    /// <summary>
    /// 从本地数据中恢复上次保存的坐标及旋转
    /// </summary>
    public void RevertoLastSave()
    {
        Vector3 localscaler = Vector3.one * 0.01f;
        PlayerDataCenter.LoclaUserData lud = PlayerDataCenter.CurrentLocaluserdata;
        if (lud.isfill)
        {
            localscaler.x = lud.normalmodelscalerX;
            localscaler.y = lud.normalmodelscalerY;
            localscaler.z = lud.normalmodelscalerZ;
        }
       
        
        transform.localScale = localscaler;
        selforiginsclaer = transform.localScale.x;
        PlayerDataCenter.UpdataUserData(PlayerDataCenter.LoclaUserData.DataKey.Scale, localscaler);

    }

    void LoadNormalPoints(Vector3 pos, Dictionary<Enums.MatarialsUse,Material> materialdic)
    {
        Dictionary<int, Dictionary<int, Vector3>> dic = PointHelper.GetInstance().normaldataformlocaljson;/*Tool.ParseXML( Enums.PointMode.Normal);*/
        Dictionary<int, Dictionary<int, Transform>> map = new Dictionary<int, Dictionary<int, Transform>>();
        GameObject go = new GameObject("PointsParent");
        go.transform.position = pos;
        go.transform.SetParent(transform);
        foreach (KeyValuePair<int, Dictionary<int, Vector3>> item in dic)
        {
            Dictionary<int, Vector3> templist = item.Value;
            Dictionary<int, Transform> tempdic = new Dictionary<int, Transform>();
            List<Transform> tempgos = new List<Transform>();

            foreach (KeyValuePair<int, Vector3> it in item.Value)
            {
                GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //g.transform.localScale = Vector3.one * 3;
                g.transform.SetParent(go.transform);
                g.transform.localPosition = it.Value;
                g.SetActive(false);
                int group = item.Key;
                int index = it.Key;
                float s = 0.3f / 0.1f;
                Vector3 finalscaler = Vector3.one * s;

                g.AddComponent<NormalPointer>().Init(materialdic, group, index, Enums.PointMode.Normal, finalscaler);
                Destroy(g.GetComponent<Collider>());
                tempgos.Add(g.transform);
                tempdic.Add(index, g.transform);
            }
            pointmap.Add(item.Key, tempgos);
            map.Add(item.Key,tempdic);
        }

        PointHelper.GetInstance().normalmodelInSceneMap = map;

    }


    void ShowGroupPoints(string message)
    {
        string type = message.Split('*')[0];
        Enums.PointControll pcontroll = (Enums.PointControll)Enum.Parse(typeof(Enums.PointControll), type);
        switch (pcontroll)
        {
            case Enums.PointControll.All:
                foreach (KeyValuePair<int,List<Transform>> item in pointmap)
                {
                    ControlAllPointGizmo(true, item.Value);

                }
                break;
            case Enums.PointControll.ShutDownAll:
                foreach (KeyValuePair<int, List<Transform>> item in pointmap)
                {
                    ControlAllPointGizmo(false, item.Value);
                }
                break;
            case Enums.PointControll.Choise:
                string choise = message.Split('*')[1];
                int index = int.Parse(choise);
                foreach (KeyValuePair<int, List<Transform>> item in pointmap)
                {
                    bool active = false;
                    if (item.Key == index)
                    {
                        active = true;
                    }
                    ControlAllPointGizmo(active, item.Value);
                }
                break;
            default:
                break;
        }

    }


    /// <summary>
    /// 控制所有gizmopointer(标准，用户输入，指示器)
    /// </summary>
    /// <param name="active"></param>
    /// <param name="trans"></param>
    void ControlAllPointGizmo(bool active,List<Transform> trans)
    {
        List<Transform> tempgos = trans;
        for (int i = 0; i < tempgos.Count; i++)
        {
            GameObject go = tempgos[i].gameObject;
            //go.SetActive(active);
            NormalPointer np = go.GetComponent<NormalPointer>();
            if (np)
            {
                np.ShowOrDisabelme(active);
            }
        }
    }


    void HighLightPoint(string message)
    {
        if (lasthighlight)
        {
            lasthighlight.SetMaterialOrigin();
        }

        NormalPointer p = PointHelper.GetInstance().CheckChoisePointer();
        p.SetMaterialHighlight();
        lasthighlight = p;
    }



    void ScalerCallback(string message)
    {
        string type = message.Split('*')[0];
        string strvalue = message.Split('*')[1];
        float value = float.Parse(strvalue);

        Enums.ControllTransform ct = (Enums.ControllTransform)Enum.Parse(typeof(Enums.ControllTransform), type);
        Vector3 scaler = transform.localScale;
        switch (ct)
        {
            case Enums.ControllTransform.Scaler:
                scaler = new Vector3(value, value, value) * selforiginsclaer;
                break;
            case Enums.ControllTransform.ScalerX:
                scaler.x = value * selforiginsclaer;
                break;
            case Enums.ControllTransform.ScalerY:
                scaler.y = value * selforiginsclaer;
                break;
            case Enums.ControllTransform.ScalerZ:
                scaler.z = value * selforiginsclaer;
                break;
            default:
                break;
        }
        transform.localScale = scaler;
        PlayerDataCenter.UpdataUserData(PlayerDataCenter.LoclaUserData.DataKey.Scale, scaler);
    }


}
