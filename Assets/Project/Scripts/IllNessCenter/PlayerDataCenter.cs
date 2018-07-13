using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataCenter : MonoBehaviour {
    [Serializable]
    public class IllNessData
    {
        public enum InjuryPosition { Arm, Lowerleg, Shoulder, Ankle }
        public enum Direction { RIGHT, LEFT }
        public enum ProtectorShape { Short,Long }



        public Direction position;
        public ProtectorShape protector_shape;

        public int ID;

        public string Modelpath;

        public string JsonPath;

        public string title;

        public string description;

        public string note;

        public string illcreatetime;

        public string injury_position;

        public string matching_point;

        public string LocalUserModelPath;
    }

    [Serializable]
    #region 本地修改(偏好设置)
    public class LoclaUserData
    {
        public enum DataKey { Postion,Eulerangel,Scale}
        //本地坐标，需双精度浮点数
        public float usermodellocalposX;
        public float usermodellocalposY;
        public float usermodellocalposZ;

        public float usermodellocaleulerangleX;
        public float usermodellocaleulerangleY;
        public float usermodellocaleulerangleZ;

        public float normalmodelscalerX = Tool.NormalScaler;
        public float normalmodelscalerY = Tool.NormalScaler;
        public float normalmodelscalerZ = Tool.NormalScaler;
        //是否填充
        public bool isfill;
    }
    #endregion




    #region 根据病例详情返回标准md5
    static private Dictionary<string, Dictionary<IllNessData.Direction, string>> md5map;

    static public string ModelMd5Map(string pos, IllNessData.Direction dirction)
    {
        if (md5map == null)
        {
            md5map = new Dictionary<string, Dictionary<IllNessData.Direction, string>>();
            md5map.Add(Tool.InjuryPosition[0], new Dictionary<IllNessData.Direction, string>());
            md5map.Add(Tool.InjuryPosition[1], new Dictionary<IllNessData.Direction, string>());
            md5map.Add(Tool.InjuryPosition[2], new Dictionary<IllNessData.Direction, string>());
            md5map.Add(Tool.InjuryPosition[3], new Dictionary<IllNessData.Direction, string>());

            md5map[Tool.InjuryPosition[0]].Add(IllNessData.Direction.LEFT, Tool.lefthand);
            md5map[Tool.InjuryPosition[0]].Add(IllNessData.Direction.RIGHT, Tool.righthand);
            md5map[Tool.InjuryPosition[1]].Add(IllNessData.Direction.LEFT, Tool.leftshoulder);
            md5map[Tool.InjuryPosition[1]].Add(IllNessData.Direction.RIGHT, Tool.rightshoulder);
            md5map[Tool.InjuryPosition[2]].Add(IllNessData.Direction.LEFT, Tool.leftfoot);
            md5map[Tool.InjuryPosition[2]].Add(IllNessData.Direction.RIGHT, Tool.rightfoot);
            md5map[Tool.InjuryPosition[3]].Add(IllNessData.Direction.LEFT, Tool.leftknee);
            md5map[Tool.InjuryPosition[3]].Add(IllNessData.Direction.RIGHT, Tool.rightknee);
        }

        return md5map[pos][dirction];
    }

    #endregion



    //当前病例
    private static IllNessData currentillnessdata;

    //当前偏好设置
    private static LoclaUserData currentlocaluserdata;

    static  private bool isnewmatchingdata = false;

    //花纹选择
    static public string Decorativepattern = "1";
    //md5
    static public string md5;
    //患者模型在服务器的地址
    static public string stlInServerpath;
    //标准片体和护具在服务器的地址
    static public string PieceProtectorURL;


    static public Action<IllNessData> OnIllNessDataChange;

    static public List<IllNessData> illnesslist = new List<IllNessData>();

    static public void AddIllness(IllNessData data)
    {
        illnesslist.Add(data);

        if (null != callback)
        {
            callback(illnesslist);
        }

    }


    static public Action<List<IllNessData>> callback;

    public static bool Isnewmatchingdata
    {
        get
        {
            return isnewmatchingdata;
        }

        set
        {
            isnewmatchingdata = value;
        }
    }


    /// <summary>
    /// 获取，修改当前病例接口
    /// </summary>
    public static IllNessData Currentillnessdata
    {
        get
        {
            return currentillnessdata;
        }

        set
        {
            if (null != OnIllNessDataChange)
            {
                OnIllNessDataChange.Invoke(value);
            }
            currentillnessdata = value;
        }
    }

    /// <summary>
    /// 获取本地偏好设置接口
    /// </summary>
    public static LoclaUserData CurrentLocaluserdata
    {
        get
        {
            if (null == currentlocaluserdata)
            {
                currentlocaluserdata = new LoclaUserData();
            }
            return currentlocaluserdata;
        }

        set
        {
            currentlocaluserdata = value;
        }
    }

    static public void RevertToNormal(IllDatalistRoot checkIllDatalist)
    {
        for (int i = 0; i < checkIllDatalist.data.Count; i++)
        {
            IllNessData ind = new IllNessData();

            DataItem di = checkIllDatalist.data[i];
            ind.ID = di.case_id;
            ind.title = di.title;
            try
            {
                ind.protector_shape = (IllNessData.ProtectorShape)Enum.Parse(typeof(IllNessData.ProtectorShape), di.protector_shape);
            }
            catch (Exception)
            {
                ind.protector_shape = IllNessData.ProtectorShape.Long;
            }

            ind.position = (IllNessData.Direction)Enum.Parse(typeof(IllNessData.Direction), di.position.ToUpper());

            ind.injury_position = di.injury_position;

            ind.note = di.note;
            ind.description = di.description;
            ind.illcreatetime = di.created_at;
            illnesslist.Add(ind);
        }
        

        if (null != callback)
        {
            callback(illnesslist);
        }
    }

    /// <summary>
    /// 点击新的病例，重新载入病例和用户设置
    /// </summary>
    /// <param name="data"></param>
    static public void NewSceneCame(IllNessData data)
    {
        currentillnessdata = data;
        md5 = ModelMd5Map(data.injury_position, data.position);
        PointHelper.GetInstance().userdataformweb.Clear();
        string path = Tool.LocalJsonSavePath + data.ID + ".json";
        string localuserjson = Tool.ReadLocalJson(path);


        currentillnessdata.LocalUserModelPath = Tool.LocalModelonSavePath + data.ID + ".stl";// mm.pdata.LocalUserModelPath;//

        if (!string.IsNullOrEmpty(localuserjson))
        {
            currentlocaluserdata = JsonHelper.ParseJsonToNeed<LoclaUserData>(localuserjson);
        }
        else
        {
            currentlocaluserdata = new LoclaUserData();
        }
        MSGCenter.Execute(Enums.MainProcess.MainProcess_LoadUserDataDone.ToString());
    }


    static public void LoadLocalUserData()
    {
        string path = Tool.LocalJsonSavePath + currentillnessdata.ID + ".json";
        string localuserjson = Tool.ReadLocalJson(path);

        if (!string.IsNullOrEmpty(localuserjson))
        {
            currentlocaluserdata = JsonHelper.ParseJsonToNeed<LoclaUserData>(localuserjson);
        }
        else
        {
            currentlocaluserdata = new LoclaUserData();
        }
    }

    /// <summary>
    /// 更新用户设置
    /// </summary>
    static public void UpdataUserData(LoclaUserData.DataKey key,Vector3 v)
    {
        switch (key)
        {
            case LoclaUserData.DataKey.Postion:
                currentlocaluserdata.usermodellocalposX = v.x;
                currentlocaluserdata.usermodellocalposY = v.y;
                currentlocaluserdata.usermodellocalposZ = v.z;
                break;
            case LoclaUserData.DataKey.Eulerangel:
                currentlocaluserdata.usermodellocaleulerangleX = v.x;
                currentlocaluserdata.usermodellocaleulerangleY = v.y;
                currentlocaluserdata.usermodellocaleulerangleZ = v.z;
                break;
            case LoclaUserData.DataKey.Scale:
                currentlocaluserdata.normalmodelscalerX = v.x;
                currentlocaluserdata.normalmodelscalerY = v.y;
                currentlocaluserdata.normalmodelscalerZ = v.z;
                break;
            default:
                break;
        }
    }

}
