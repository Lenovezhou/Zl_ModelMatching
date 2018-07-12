using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PointHelper
{
    static private PointHelper instance;
    static public PointHelper GetInstance()
    {
        if (null == instance)
        {
            instance = new PointHelper();
        }
        return instance;
    }
    private PointHelper()
    {
        MSGCenter.Register(Enums.MainProcess.MainProcess_RePlay.ToString(), RePlay);
        MSGCenter.Register(Enums.LeftMouseButtonControl.AddMatchingGizmo.ToString(), AliveAddGizmoCallBack);
    }


    //避空位字典
    Dictionary<int, GameObject> avoidvacancymap = new Dictionary<int, GameObject>();


    //当前正在编辑的避空位序号
    public int currenteditvacancyindex;


    //点集合
    Dictionary<Enums.PointMode, Dictionary<int, Dictionary<int,Vector3>>> map = new Dictionary<Enums.PointMode, Dictionary<int, Dictionary<int, Vector3>>>();



    public Dictionary<int, Dictionary<int, Transform>> normalmodelInSceneMap = new Dictionary<int, Dictionary<int, Transform>>();

    //从服务器过来的数据
    public Dictionary<int, Dictionary<int, Vector3>> userdataformweb = new Dictionary<int, Dictionary<int, Vector3>>();

    //本地标准数据
    public Dictionary<int, Dictionary<int, Vector3>> normaldataformlocaljson = new Dictionary<int, Dictionary<int, Vector3>>();

    static public string usermodlepath = "";

    static public Vector3 usermodelpos;

    //当前正在编辑的点和组
    public int currentgroup, currentindex;


    public int AddPoint(int _group, int _index, Vector3 pos)
    {
        int group = _group;
        int index = _index;
        if (!userdataformweb.ContainsKey(group))
        {
            userdataformweb.Add(group, new Dictionary<int, Vector3>());
        }

        if (!userdataformweb[group].ContainsKey(index))
        {
            userdataformweb[group].Add(index, pos);
        }

        userdataformweb[group][index] = (pos);

        string debuglog = string.Format("成功写入:  group {0} index {1} pos {2} ",  group, index, pos);
        Debug.Log(debuglog);

        return userdataformweb[group].Count;
    }


    public int AddAvoidvacancy(int index,GameObject gizmo)
    {
        return 1;
    }

    public void SaveAll()
    {
        //如果匹配没有完成，提醒用户，不能上传
        List<int> notcheckinglist = CheckAllMatchDone();
        if (notcheckinglist.Count > 0)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < notcheckinglist.Count; i++)
            {
                stringBuilder.Append( " "+Tool.NumberToChar(notcheckinglist[i] + 1) + " ");
            }

            string _strs = MSGCenter.FormatMessage("第<color=red>"+stringBuilder.ToString()+"</color>组未匹配", "3");

            TTUIPage.ShowPage<UINotice>(_strs);
            return;
        }
        int currentillnessdataid = PlayerDataCenter.Currentillnessdata.ID;

        string url = Tool.refreshillnessdatasimplepath + currentillnessdataid;

        MyWebRequest.Instance.Get(Tool.requestdetailillnessdatapath + currentillnessdataid, (success, str) =>
        {
        bool hasvalue = false;
        if (success)
        {
            hasvalue = JsonHelper.ChackMatchingHasValue(str);
            string _url = "";
            string _message = "";
            if (hasvalue)
            {
                int pointid = JsonHelper.GetPointID(str);
                //更新旧的
                _url = Tool.refreshmathcingpointpath + pointid;
                _message = JsonHelper.MergMatchingpointChangeNeed(PlayerDataCenter.Currentillnessdata);
                MyWebRequest.Instance.Put(_url, _message,(suc, ss)=>{
                    TTUIPage.ShowPage<UINotice>( Tool.DownloadDir+ss);
                    if (suc)
                    {
                        //解析路径
                        string downloadurlsjson = JsonHelper.ParseJsonToNeed(str, Tool.datakey, Tool.stlInServerpath);
                        DownloadSave(downloadurlsjson);
                        TTUIPage.ClosePage<UINotice>();
                    }
                });
            }
            else {
                //添加新的
                _url = Tool.addmatchingpointspath;
                MyWebRequest.Instance.Post(_url, JsonHelper.MergMatchingpointChangeNeed(PlayerDataCenter.Currentillnessdata), (_suc,_str) => 
                {
                    TTUIPage.ShowPage<UINotice>(Tool.DownloadDir + _str);
                    if (_suc)
                    {
                        //解析路径
                        string downloadurlsjson = JsonHelper.ParseJsonToNeed(str, str, Tool.datakey, Tool.stlInServerpath);
                        DownloadSave(downloadurlsjson);
                        TTUIPage.ClosePage<UINotice>();
                    }
                });
            }

            }
        });
    }


    public void DownloadSave(string json)
    {
        Dictionary<string, string> pathmap = JsonHelper.ParseDownloadPath(json, Tool.downloadbodykey, Tool.Protectiveclothingkey);
        if (pathmap == null || pathmap .Count == 0)
        {
            return;
        }
        //TODO--->>> 根据返回路径下载对应模型存储到本地返回给modelmanager
        string bodyurl = pathmap[Tool.downloadbodykey];
        string Protectiveclothingurl = pathmap[Tool.Protectiveclothingkey];
        //两个路径
        string bodyurlsavefilename = PlayerDataCenter.Currentillnessdata.ID + Tool.downloadbodykey+".stl";
        string Protectiveclothingfilename = PlayerDataCenter.Currentillnessdata.ID + Tool.Protectiveclothingkey + ".stl";
        MyWebRequest.Instance.DownloadFileFromWed(bodyurl, Tool.SaveDownLoadFromWebPath, bodyurlsavefilename, (success, str) =>
        {
            TTUIPage.ShowPage<UINotice>(Tool.DownloadDir + str);
            if (success)
            {
                TTUIPage.ClosePage<UINotice>();
                //存在本地的模型路径
                string _messagestr = MSGCenter.FormatMessage(Tool.downloadbodykey, Tool.SaveDownLoadFromWebPath + bodyurlsavefilename);
                MSGCenter.Execute(Enums.MainProcess.MainProcess_SaveALL.ToString(), _messagestr);
            }
        });
        MyWebRequest.Instance.DownloadFileFromWed(Protectiveclothingurl, Tool.SaveDownLoadFromWebPath, Protectiveclothingfilename, (success, str) =>
        {
            if (success)
            {
                //存在本地的模型路径
                string _messagestr = MSGCenter.FormatMessage(Tool.Protectiveclothingkey, Tool.SaveDownLoadFromWebPath + Protectiveclothingfilename);
                MSGCenter.Execute(Enums.MainProcess.MainProcess_SaveALL.ToString(), _messagestr);
            }
        });
    }

    public NormalPointer CheckChoisePointer()
    {
        NormalPointer p = null;
        foreach (KeyValuePair<int, Dictionary<int, Transform>> item in normalmodelInSceneMap)
        {
            int group = item.Key;
            List<NormalPointer> temppointers = new List<NormalPointer>();
            for (int i = 0; i < item.Value.Count; i++)
            {
                temppointers.Add(item.Value[i].GetComponent<NormalPointer>());
            }
            for (int i = 0; i < temppointers.Count; i++)
            {
                int index = i;
                if (temppointers[i].IsMe(currentgroup, currentindex))
                {
                    NormalPointer tempp = temppointers[i];
                    p = tempp;
                    return p;
                }
            }
        }
        return p;
    }

    List<int> CheckAllMatchDone()
    {
        List<int> nomatchinglist = new List<int>();
        foreach (KeyValuePair<int, Dictionary<int, Transform>> item in normalmodelInSceneMap)
        {
            foreach (KeyValuePair<int,Transform> it in item.Value)
            {
                NormalPointer np = it.Value.GetComponent<NormalPointer>();
                if (!np.OwnedPointer())
                {
                    if (!nomatchinglist.Contains(item.Key))
                    {
                        nomatchinglist.Add(item.Key);
                    }
                    break;
                }
            }
        }
        return nomatchinglist;
    }
    void AliveAddGizmoCallBack(string message)
    {
        Dictionary<int, string> messagemap = MSGCenter.UnFormatMessage(message);
        if (!messagemap.ContainsKey(2))
        {
            return;
        }
        string type = messagemap[1];
        string strGroupindex = messagemap[2];
        string strindex = messagemap[3];

        currentgroup = int.Parse(strGroupindex);
        currentindex = int.Parse(strindex);
    }

    public int AutoAddIndex()
    {
        int lenght = normalmodelInSceneMap[currentgroup].Count;
        if (currentindex < lenght - 1)
        {
            currentindex++;
            MSGCenter.Execute(Enums.MatchigPointGizmoControll.AutoNext.ToString());
        }
        return currentindex;
    }

    public bool IsCurrentGroupMax()
    {
        return currentindex == normalmodelInSceneMap[currentgroup].Count - 1;
    }

    private void RePlay(string message)
    {
        avoidvacancymap.Clear();
        map.Clear();
        currenteditvacancyindex = -1;

        foreach (KeyValuePair<int, Dictionary<int, Transform>> item in normalmodelInSceneMap)
        {
            foreach (KeyValuePair<int,Transform> it in item.Value)
            {
                NormalPointer p = it.Value.GetComponent<NormalPointer>();
                p.RePlay();
            }
        }
    }

}
