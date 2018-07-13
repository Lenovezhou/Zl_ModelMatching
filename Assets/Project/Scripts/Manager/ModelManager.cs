using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModelManager : MonoBehaviour
{
    //片体
    STL ploader;
    //护具
    STL hloader;

    //生成标准模型后回调设置摄像机target
    Action<Transform> setcameratargetcallback;

    //坐标系
    GameObject coordinatesystem;

    private OBJ OBJLoader;
    private STL STLLoader;
    private int userimporottaltol = 0;
    GameObject Normal;
    GameObject userimport;
    bool canaddgizmo = false;
    bool canaddAvoidvacancy = false;
    bool autoadd = false;

    NormalPointer lastchoisePointer;

    Dictionary<int, List<Transform>> pointermap = new Dictionary<int, List<Transform>>();
    Dictionary<int, GameObject> avoidvacancymap = new Dictionary<int, GameObject>();

    //使用材质字典
    Dictionary<Enums.MatarialsUse, Material> materialmap = new Dictionary<Enums.MatarialsUse, Material>();

    //添加避空位时鼠标点下时的位置，用于计算半径及圆心
    private Vector3 avoidvacancystartpos;
    private int currenteditvacancy;


    public void Init(Action<Transform> call)
	{
        Material origin = Instantiate(Resources.Load<Material>("Materials/origin_normal"));
        Material highlight = Instantiate(Resources.Load<Material>("Materials/highlight_normal"));
        Material highlight_userimport = Instantiate(Resources.Load<Material>("Materials/highlight_userimport"));
        Material origin_userimport = Instantiate(Resources.Load<Material>("Materials/origin_userimprot"));

        Material indicatoreffect = Instantiate(Resources.Load<Material>("Materials/indicatoreffect"));
        Material indicatornormal = Instantiate(Resources.Load<Material>("Materials/indicatornormal"));
        Material Normalmodel = Instantiate(Resources.Load<Material>("Materials/NormalModel"));
        Material UserImportModel = Instantiate(Resources.Load<Material>("Materials/UserImportModel"));



        materialmap.Add(Enums.MatarialsUse.Indicator_Effect, indicatoreffect);
        materialmap.Add(Enums.MatarialsUse.Indicator_origin, indicatornormal);
        materialmap.Add(Enums.MatarialsUse.NormalModel, Normalmodel);
        materialmap.Add(Enums.MatarialsUse.UserimportModel, UserImportModel);
        materialmap.Add(Enums.MatarialsUse.NormalPoint_origin, origin);
        materialmap.Add(Enums.MatarialsUse.NormalPoint_hight, highlight);
        materialmap.Add(Enums.MatarialsUse.UserImportpoint_origin, origin_userimport);
        materialmap.Add(Enums.MatarialsUse.UserImportpoint_hight, highlight_userimport);

        #region RegistEvent
        MSGCenter.Register(Enums.ModelPath.ModelPath.ToString(), LoadUserImprot);
        MSGCenter.Register(Enums.ToggleToModel.Patient.ToString(), ToggleSetActiveModle);
        MSGCenter.Register(Enums.ToggleToModel.Normalpice.ToString(), ToggleSetActiveModle);
        MSGCenter.Register(Enums.ToggleToModel.Preservemodel.ToString(), ToggleSetActiveModle);
        MSGCenter.Register(Enums.ToggleToModel.Unnormal.ToString(), ToggleSetActiveModle);

        MSGCenter.Register(Enums.LeftMouseButtonControl.AddMatchingGizmo.ToString(), AliveAddGizmoCallBack);
        MSGCenter.Register(Enums.LeftMouseButtonControl.AddAvoidGizmo.ToString(), ChangeAddAvoidGizmo);


        MSGCenter.Register(Enums.MatchigPointGizmoControll.SaveMatchingpoint.ToString(), PointerSaveCancleCallback);
        MSGCenter.Register(Enums.MatchigPointGizmoControll.Cancle.ToString(), PointerSaveCancleCallback);

        MSGCenter.Register(Enums.MatchigPointGizmoControll.AutoNextToggle.ToString(), AutoAddToggleCallback);

        MSGCenter.Register(Enums.AvoidvacancyControll.Active.ToString(), AliveAddAvoidvacancy);
        MSGCenter.Register(Enums.AvoidvacancyControll.Add.ToString(), AliveAddAvoidvacancy);
        MSGCenter.Register(Enums.AvoidvacancyControll.Edit.ToString(), AliveAddAvoidvacancy);
        MSGCenter.Register(Enums.AvoidvacancyControll.Inactive.ToString(), AliveAddAvoidvacancy);
        MSGCenter.Register(Enums.AvoidvacancyControll.Remove.ToString(), AliveAddAvoidvacancy);
        MSGCenter.Register(Enums.AvoidvacancyControll.SaveAvoid.ToString(), AliveAddAvoidvacancy);

        MSGCenter.Register(Enums.AvoidvacancyControll.ActiveAll.ToString(), AliveAddAvoidvacancy);
        MSGCenter.Register(Enums.AvoidvacancyControll.InactiveAll.ToString(), AliveAddAvoidvacancy);

        //MSGCenter.Register(Enums.MatchigPointGizmoControll.LoadUserData.ToString(), LoadUserPointer);

        MSGCenter.Register(Enums.MainProcess.MainProcess_SaveALL.ToString(), SaveAllCall);

        MSGCenter.Register(Enums.MainProcess.MainProcess_RePlay.ToString(), ReleseDownloadeModel);
        #endregion

        this.setcameratargetcallback = call;
    }


	void CreatModel(string md5)
	{

        string normalmodelpath = Tool.LocalNormalModelPath + md5 + ".stl";

        if (!Tool.CheckFileExist(normalmodelpath))
        {
            return;
        }

        GameObject normalgo = new GameObject("NormalModels" + userimporottaltol);

        Normal = normalgo;
        GameObject cameratargetgo = new GameObject("CameraTarget");
        cameratargetgo.transform.SetParent(normalgo.transform);
        cameratargetgo.transform.localPosition = Vector3.zero;


        normalgo.AddComponent<STL>().CreateInstance(normalmodelpath, str => { }, instancego =>
        {
            instancego.transform.SetParent(normalgo.transform);
            NormalModelController nmc = normalgo.AddComponent<NormalModelController>();

            pointermap = nmc.Init(materialmap);
            nmc.RevertoLastSave();

            MeshRenderer[] childrenderers = normalgo.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < childrenderers.Length; i++)
            {
                //childrenderers [i].gameObject.AddComponent<MeshCollider> ();
                childrenderers[i].gameObject.tag = "Finish";
                childrenderers[i].material = materialmap[Enums.MatarialsUse.NormalModel];
            }
            normalgo.transform.SetParent(transform);
            //normalgo.transform.position = transform.position + transform.forward * 4;
            CreatCoordinateSystem(normalgo.transform);
            setcameratargetcallback(cameratargetgo.transform);
        });
	}

    /// <summary>
    /// 生成坐标系
    /// </summary>
    /// <returns></returns>
    GameObject CreatCoordinateSystem(Transform targettransfrom)
    {
        if (!coordinatesystem)
        {
            GameObject cs = Resources.Load<GameObject>(Tool.CoordinateSystemLocalPath);
            coordinatesystem = Instantiate(cs);
            coordinatesystem.AddComponent<CoordinateSystemController>().Init(targettransfrom);
        }

        return coordinatesystem;
    }

    GameObject CreateUserOBJ()
    {
        GameObject UserImport = new GameObject("UserImport");
       
        UserImport.AddComponent<ModelTranslate>();
        UserImport.transform.SetParent(transform);
        UserImport.transform.localScale = Vector3.one * 0.01f;
        UserImport.transform.position = transform.position + transform.forward * 4;
        return UserImport;
    }

    void MatchingModelNormal(GameObject import)
    {
        import.name = "UserImport";
        import.transform.SetParent(transform);
        ModelTranslate mt = import.AddComponent<ModelTranslate>();
        mt.Init(materialmap);
        userimporottaltol++;
        //ModelTranslate realmt = mt.ResetCenterEnter(userimporottaltol);
        //realmt.LoadPosFormLastSave(localpos,localrotation);
    }

    void LoadUserImprot(string message)
    {
        SetLastInactive();
        userimport = new GameObject("UserImport" + userimporottaltol);
        OBJLoader = userimport.AddComponent<OBJ>();
        STLLoader = userimport.AddComponent<STL>();
        Dictionary<int, string> map = MSGCenter.UnFormatMessage(message);
        //string[] s = message.Split('*');
        string serverpath = map[1];
        if (map.ContainsKey(2))
        {
            string md5 = map[2];
            CreatModel(md5);
        }
        if (serverpath.IsObj())
        {
            TTUIPage.ShowPage<UINotice>("请使用stl格式的模型");
            //pcpath = "file:///" + pcpath;
            //try
            //{
            //    OBJLoader.Load(pcpath, (result) =>
            //    {
            //        MatchingModelNormal(userimport,v, hasposvalue);
            //        userimport.AddComponent<MeshCollider>();
            //        MSGCenter.Execute(Enums.ModelPath.Result.ToString(), result );
            //    });
            //}
            //catch (System.Exception e)
            //{
            //    MSGCenter.Execute(Enums.ModelPath.Result.ToString(), e.ToString() );
            //    throw;
            //}
        }
        else if (serverpath.IsStl())
        {

            string stlpath = Tool.LocalModelonSavePath + PlayerDataCenter.Currentillnessdata.ID + ".stl";// mm.pdata.LocalUserModelPath;//
            if (!Tool.CheckFileExist(stlpath))
            {
                MyWebRequest.Instance.DownloadFileFromWed(serverpath, Tool.LocalModelonSavePath, PlayerDataCenter.Currentillnessdata.ID + ".stl"
                    , (suc, str) =>
                    {
                        TTUIPage.ShowPage<UINotice>(Tool.DownloadDir + str);
                        if (suc)
                        {
                            TTUIPage.ClosePage<UINotice>();
                            CreatStlInstance(stlpath);
                        }
                    });
            }
            else {
                CreatStlInstance(stlpath);
            }
                       
        }
    }




    void CreatStlInstance(string path)
    {
        try
        {
            STLLoader.CreateInstance(path, (result) =>
            {
                MSGCenter.Execute(Enums.ModelPath.Result.ToString(), result);
            },
            (go) =>
            {
                go.transform.SetParent(userimport.transform);
                MatchingModelNormal(userimport);
                LoadUserPointerAndPieceProtect();
            }
           );
            //Destroy(userimport);
        }
        catch (System.Exception e)
        {
            MSGCenter.Execute(Enums.ModelPath.Result.ToString(), e.ToString());
            throw;
        }

    }


    void ToggleSetActiveModle(string message)
    {
        string[] strs = message.Split('*');
        Enums.ToggleToModel ttm = (Enums.ToggleToModel)Enum.Parse(typeof(Enums.ToggleToModel), strs[0]);
        bool active = bool.Parse(strs[1]);
        switch (ttm)
        {
            case Enums.ToggleToModel.Patient:
                SetLastInactive(active);
                break;
            case Enums.ToggleToModel.Normalpice:
                Normal.SetActive(active);
                break;
            case Enums.ToggleToModel.Unnormal:
                if (ploader)
                {
                    ploader.gameObject.SetActive(active);
                }
                break;
            case Enums.ToggleToModel.Preservemodel:
                if (hloader)
                {
                    hloader.gameObject.SetActive(active);
                }
                break;
            default:
                break;
        }
    }

    void SetLastInactive(bool active = false)
    {
        if (null != OBJLoader)
        {
            OBJLoader.gameObject.SetActive(active);
            //if (OBJLoader.transform.parent != null)
            //    OBJLoader.transform.parent.gameObject.SetActive(active);
        }
        if (null != STLLoader)
        {
            STLLoader.gameObject.SetActive(active);
            //if (STLLoader.transform.parent != null)
            //    STLLoader.transform.parent.gameObject.SetActive(active);
        }
    }

	bool CanAddMatchingpointGizmo()
	{
		return Input.GetMouseButtonDown (0) && !EventSystem.current.IsPointerOverGameObject () && canaddgizmo;
	}

    bool CanAddAvoidvacancy()
    {
        return Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() && canaddAvoidvacancy;
    }

    void ChangeAddAvoidGizmo(string message)
    {
        Dictionary<int,string> messagemap = MSGCenter.UnFormatMessage(message);
        bool b = bool.Parse( messagemap[1]);
        canaddAvoidvacancy = b;
    }
    void AliveAddGizmoCallBack(string message)
    {
        Dictionary<int, string> map = MSGCenter.UnFormatMessage(message);
        if (map.ContainsKey(1))
        {
            string type = map[1];
            bool active = bool.Parse(type);
            this.canaddgizmo = active;
        }
        if (!map.ContainsKey(2))
        {
            return;
        }
        string strGroupindex = map[2];
        string strindex = map[3];


        //下一个编辑命令到来
        if (lastchoisePointer)
        {
            lastchoisePointer.NextEditCheckThis();
        }
    }
    void AliveAddAvoidvacancy(string message)
    {
        Dictionary<int, string> map = MSGCenter.UnFormatMessage(message);
        int index = currenteditvacancy;
        Vector3 pos = Vector3.zero;
        Vector3 scaler = Vector3.zero;
        if (map.ContainsKey(1))
        {
            index = int.Parse(map[1]);
        }
        if (map.ContainsKey(2))
        {
            pos = MSGCenter.ParseToVector(map[2]);
        }
        if (map.ContainsKey(3))
        {
            scaler = MSGCenter.ParseToVector(map[3]);
        }
        GameObject voidva = null;


        if (!avoidvacancymap.ContainsKey(index))
        {
            voidva = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            voidva.transform.SetParent(userimport.transform);
            voidva.transform.localScale = scaler;
            voidva.transform.localPosition = pos;
            voidva.AddComponent<AvoidVacancyController>();
            voidva.GetComponent<MeshRenderer>().material.color = Color.blue;
            Destroy(voidva.GetComponent<Collider>());
            avoidvacancymap[index] = voidva;
        }
        voidva = avoidvacancymap[index];

        Enums.AvoidvacancyControll ac = Enums.ParseEnums<Enums.AvoidvacancyControll>(map[0]);
        switch (ac)
        {
            case Enums.AvoidvacancyControll.Add:
                
                break;
            case Enums.AvoidvacancyControll.Edit:
                canaddAvoidvacancy = true;
                break;
            case Enums.AvoidvacancyControll.Remove:
                Destroy(voidva);
                avoidvacancymap.Remove(index);
                if (AvoidVacancyHelper.avoidmap.ContainsKey(index))
                {
                    AvoidVacancyHelper.avoidmap.Remove(index);
                }
                index--;
                canaddAvoidvacancy = false;
                break;
            case Enums.AvoidvacancyControll.Inactive:
                voidva.SetActive(false);
                canaddAvoidvacancy = false;
                break;
            case Enums.AvoidvacancyControll.Active:
                voidva.SetActive(true);
                canaddAvoidvacancy = true;
                break;
            case Enums.AvoidvacancyControll.SaveAvoid:
                canaddAvoidvacancy = false;
                if (!AvoidVacancyHelper.avoidmap.ContainsKey(index))
                {
                    AvoidVacancyHelper.avoidmap.Add(index, new Dictionary<int, Vector3>());
                    AvoidVacancyHelper.avoidmap[index].Add(0, voidva.transform.localPosition);
                    AvoidVacancyHelper.avoidmap[index].Add(1, voidva.transform.localScale);
                }
                else {
                    AvoidVacancyHelper.avoidmap[index][0] = voidva.transform.localPosition;
                    AvoidVacancyHelper.avoidmap[index][1] = voidva.transform.localScale;
                }

                break;
            case Enums.AvoidvacancyControll.InactiveAll:
                foreach (KeyValuePair<int,GameObject> item in avoidvacancymap)
                {
                    if (avoidvacancymap.ContainsKey(item.Key))
                    {
                        avoidvacancymap[item.Key].SetActive(false);
                    }
                }
                break;
            case Enums.AvoidvacancyControll.ActiveAll:
                foreach (KeyValuePair<int, GameObject> item in avoidvacancymap)
                {
                    if (avoidvacancymap.ContainsKey(item.Key))
                    {
                        avoidvacancymap[item.Key].SetActive(true);
                    }
                }
                break;
            case Enums.AvoidvacancyControll.Reset:
            default:
                break;
        }
        currenteditvacancy = index;
        PointHelper.GetInstance().currenteditvacancyindex = currenteditvacancy;
    }


    private void Update()
    {
        CheckMouseHit();
    }

    Ray ray;
    RaycastHit hitinfo;

    private void CheckMouseHit()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitinfo, 100))
        {
            if (hitinfo.collider != null)
            {
                string _tag = hitinfo.collider.tag;
                if (_tag == Tool.normaltag)
                {
                }
                else if (_tag == Tool.userimporttag)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        avoidvacancystartpos = hitinfo.point;
                    }

                    //添加匹配点
                    if (CanAddMatchingpointGizmo())
                    {
                        AddMatchingPoint();
                    }

                    if (CanAddAvoidvacancy())
                    {
                        EditAvoidvacancyPos(hitinfo.point);
                    }
                }
            }
        }
        
    }

    void AddMatchingPoint()
    {
        NormalPointer p = PointHelper.GetInstance().CheckChoisePointer();
        if (p)
        {
            Vector3 localpos = userimport.transform.InverseTransformPoint(hitinfo.point);
            CreateNormalOwnedPointer(p, localpos, PointHelper.GetInstance().currentgroup, PointHelper.GetInstance().currentindex);
            lastchoisePointer = p;
            //如果自动下一步，才执行这里
            if (autoadd && !PointHelper.GetInstance().IsCurrentGroupMax())
            {
                //保存
                MSGCenter.Execute(Enums.MatchigPointGizmoControll.SaveMatchingpoint.ToString());
                //增加序号
                PointHelper.GetInstance().AutoAddIndex();
            }
            else if (autoadd && PointHelper.GetInstance().IsCurrentGroupMax())
            {
                //保存
                MSGCenter.Execute(Enums.MatchigPointGizmoControll.SaveMatchingpoint.ToString());
            }
        }

    }

    void EditAvoidvacancyPos(Vector3 pos)
    {
        if (avoidvacancymap.ContainsKey(currenteditvacancy))
        {
            GameObject g = avoidvacancymap[currenteditvacancy];
            if (!g.activeSelf)
            {
                g.SetActive(true);
            }
            Vector3 v = (avoidvacancystartpos + (pos - avoidvacancystartpos) / 2);
            float scaler = Vector3.Distance(pos ,avoidvacancystartpos);
            
            float parentscaler = g.transform.parent.localScale.x;
            float endvalue = scaler / parentscaler;
            g.GetComponent<AvoidVacancyController>().Change(v, new Vector3(endvalue, endvalue, endvalue));

            if (AvoidVacancyHelper.avoidmap.ContainsKey(currenteditvacancy))
            {
                AvoidVacancyHelper.avoidmap[currenteditvacancy][0] = v;
                AvoidVacancyHelper.avoidmap[currenteditvacancy][1] = new Vector3(endvalue, endvalue, endvalue);
            }

        }
    }

    private void PointerSaveCancleCallback(string message)
    {
        Enums.MatchigPointGizmoControll gpc = (Enums.MatchigPointGizmoControll)Enum.Parse(typeof(Enums.MatchigPointGizmoControll), message);
        NormalPointer p = PointHelper.GetInstance().CheckChoisePointer();
        if (p)
        {
            switch (gpc)
            {
                case Enums.MatchigPointGizmoControll.SaveMatchingpoint:
                    p.SaveOwned();
                    break;
                case Enums.MatchigPointGizmoControll.Cancle:
                    p.ReleaseOwned();
                    break;
                default:
                    break;
            }
        }
    }


    private void AutoAddToggleCallback(string message)
    {
        string bstr = message.Split('*')[1];
        bool active = bool.Parse(bstr);
        autoadd = active;
    }

    /// <summary>
    /// 从服务器过来的数据刷新
    /// </summary>
    void LoadUserPointerAndPieceProtect()
    {
        Dictionary<int, Dictionary<int, Vector3>> userdata = PointHelper.GetInstance().userdataformweb;// Tool.ParseXML(Enums.PointMode.UserImport);

        if (userdata.Count <= 0)
        {
            MSGCenter.Execute(Enums.MainProcess.MainProcess_loadedUserPointDone.ToString());
            return;
        }
        int group = -1;
        int index = -1;
        foreach (KeyValuePair<int,Dictionary<int,Vector3>> item in userdata)
        {
            group = item.Key;
            foreach (KeyValuePair<int,Vector3> it in item.Value)
            {
                index = it.Key;
                Vector3 localpos = it.Value;

                PointHelper.GetInstance().currentgroup = group;
                PointHelper.GetInstance().currentindex = index;

                NormalPointer p = PointHelper.GetInstance().CheckChoisePointer();
                if (p)
                {
                    CreateNormalOwnedPointer(p, localpos, group, index,false);
                    //MSGCenter.Execute(Enums.MatchigPointGizmoControll.SaveMatchingpoint.ToString());
                }
            }
        }

        PointHelper.GetInstance().DownloadSave(PlayerDataCenter.PieceProtectorURL);

        MSGCenter.Execute(Enums.MainProcess.MainProcess_loadedUserPointDone.ToString());
    }

    void CreateNormalOwnedPointer(NormalPointer p,Vector3 hitinfopoint,int _group,int _index,bool avtive = true)
    {
        p.CreateOwnedUserPointer(materialmap, hitinfopoint,userimport.transform, avtive,(_localpos,_g,_i) =>
        {
            Vector3 localpos = _localpos;
            int group = _g;
            int index = _i;
            PointHelper.GetInstance().AddPoint(group, index, localpos);
        });
    }


    /// <summary>
    /// 点击提交后，下载模型后实例到场景里
    /// </summary>
    /// <param name="message"></param>
    void SaveAllCall(string message)
    {
        if (!ploader)
        {
            ploader = new GameObject().AddComponent<STL>();
        }
        if (!hloader)
        {
            hloader = new GameObject().AddComponent<STL>();
        }

        Dictionary<int, string> map = MSGCenter.UnFormatMessage(message);
        string path = "";
        Transform parent = null;
        string key = "";
        STL loader = null;
        if (map.ContainsKey(1))
        {
            key = map[1];
        }
        if (map.ContainsKey(2))
        {
            switch (key)
            {
                case "a":
                    path = map[2];
                    loader = ploader;
                    parent = ploader.transform;
                    break;
                case "b":
                    path = map[2];
                    loader = hloader;
                    parent = hloader.transform;
                    break;
                default:
                    break;
            }
             DownLoadHelper.Instance.CreateDownloadeModel(loader, path, parent, userimport.transform, HasSceneLoadDownloadModel);
        }


    }


    bool HasSceneLoadDownloadModel()
    {
        return ploader.transform.childCount != 0 && hloader.transform.childCount != 0;
    }

    
    /// <summary>
    /// 释放掉已经下载的片体和护具模型
    /// </summary>
    /// <param name="message"></param>
    void ReleseDownloadeModel(string message)
    {
        if (ploader && ploader.transform.childCount > 0)
        {
            for (int i = 0; i < ploader.transform.childCount; i++)
            {
                Destroy(ploader.transform.GetChild(i).gameObject);
            }
        }
        if (hloader && hloader.transform.childCount > 0)
        {
            for (int i = 0; i < hloader.transform.childCount; i++)
            {
                Destroy(hloader.transform.GetChild(i).gameObject);
            }
        }
        if (Normal)
        {
            Normal.SetActive(false);
            for (int i = 0; i < Normal.transform.childCount; i++)
            {
                Normal.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        foreach (KeyValuePair<int,GameObject> item in avoidvacancymap)
        {
            Destroy(item.Value);
        }
        avoidvacancymap.Clear();
    }
}
