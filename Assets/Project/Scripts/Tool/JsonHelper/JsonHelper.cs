using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class JsonHelper  {

    #region Json操作

    /// <summary>
    /// 发送给服务器存储matchingpoints -->point 序号从1开始
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    static JObject MergMatchingPointsJson(Dictionary<int, Dictionary<int, Vector3>> points)
    {
        string key = "matching_point";
        JObject j = new JObject();
        JObject groupjobject = new JObject();
        foreach (KeyValuePair<int, Dictionary<int, Vector3>> item in points)
        {
            JObject pointsjobject = new JObject();
            string groupstr =Tool.NumberToChar(item.Key + 1);
            foreach (KeyValuePair<int, Vector3> it in item.Value)
            {
                string indexstr = (it.Key + 1).ToString();
                var jx = new JProperty("x", it.Value.x);
                var jy = new JProperty("y", it.Value.y);
                var jz = new JProperty("z", it.Value.z);

                JProperty pointproper = new JProperty(indexstr, new JObject(jx, jy, jz));
                pointsjobject.Add(pointproper);
            }
            JProperty groupproper = new JProperty(groupstr, new JObject(pointsjobject));
            groupjobject.Add(groupproper);

        }
        JProperty jend = new JProperty(key, groupjobject);
        j.Add(jend);
        Debug.Log(j.ToString());
        // ParseJsontoDic(j.ToString());
        return j;
    }

    /// <summary>
    /// 解析时本地normal和服务器userimport时point序号都是从1开始
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    static public Dictionary<int, Dictionary<int, Vector3>> ParseMatchingpointsJosn(string json,string _key)
    {
        Dictionary<int, Dictionary<int, Vector3>> map = new Dictionary<int, Dictionary<int, Vector3>>();
        string datapointkey = Tool.datapointkey;
        //string poitmatchingpointkey = Tool.pointmatchingpoints;
        string key = _key;

        JObject msgData = (JObject)JsonConvert.DeserializeObject(json);
        string channelStr = msgData.Property(key) != null ? msgData[key].ToString() : "error_noKey";
        if (string.Equals(channelStr,"[]"))
        {
            return map;
        }
        //LogView.setViewText ("GameGlobal.cs,setChannelInfo(),channelStr=="+channelStr);
        JObject itemData = (JObject)JsonConvert.DeserializeObject(channelStr);

        int group = -1;

        foreach (KeyValuePair<string, JToken> item in itemData)
        {
            Dictionary<int, Vector3> dic = new Dictionary<int, Vector3>();
            //ABCD...I
            key = item.Key;
            group = Tool.CharToNumber(key)- 65;
            string itemchannelStr = itemData.Property(key) != null ? itemData[key].ToString() : "error_noKey";
            JObject itdata = (JObject)JsonConvert.DeserializeObject(itemchannelStr);

            int index = -1;
            foreach (KeyValuePair<string, JToken> it in itdata)
            {
                //0234...7
                key = it.Key;
                index = int.Parse(key);
                string posindex = itdata.Property(key) != null ? itdata[key].ToString() : "error_noKey";
                JObject posdata = (JObject)JsonConvert.DeserializeObject(posindex);
                Vector3 pos = Vector3.zero;
                //xyz
                foreach (KeyValuePair<string, JToken> p in posdata)
                {
                    key = p.Key;
                    switch (key)
                    {
                        case "x":
                            pos.x = float.Parse(p.Value.ToString());
                            break;
                        case "y":
                            pos.y = float.Parse(p.Value.ToString());
                            break;
                        case "z":
                            pos.z = float.Parse(p.Value.ToString());
                            break;
                        default:
                            break;
                    }
                }
                dic.Add(index - 1, pos);
            }
            map.Add(group, dic);
        }
       // MergMatchingPointsJson(map);
        return map;
    }

    #endregion

    #region 写入本地
    public static void UpdateLocalJsonFiles(string  matchingdata,int id)
    {
        string message = matchingdata;
        string FileName = Tool.LocalJsonSavePath + id + ".json";

        if (File.Exists(FileName))
        {
            File.Delete(FileName);
        }
        using (FileStream fileStream = File.Create(FileName))
        {
            byte[] bytes = new UTF8Encoding(true).GetBytes(message);
            fileStream.Write(bytes, 0, bytes.Length);
        }
    }
    #endregion


    #region 读取本地Json文件
    static public Dictionary<int, Dictionary<int, Vector3>> LoadJsonFromFile()
    {
        string Localjson = Tool.ReadLocalJson(Tool.LocalNormalpointsJsonPath+ PlayerDataCenter.md5 + Tool.jsondir);
        if (!string.IsNullOrEmpty(Localjson))
        {
            return ParseMatchingpointsJosn(Localjson, Tool.matchingpointskey);
        }
        else
        {
            return null;
        }

    }
    #endregion


    /// <summary>
    /// 新增病例 时，服务器所需json
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    static public string MergAddIllnessdataJson(PlayerDataCenter.IllNessData data)
    {
        JObject requestobject = new JObject();
        JProperty title = new JProperty("title", data.title);
        JProperty injury_position = new JProperty("injury_position", data.injury_position.ToString());
        JProperty position = new JProperty("position", data.position);
        JProperty description = new JProperty("description", data.description.ToString());
        JProperty note = new JProperty("note", data.note);
        JProperty protector_shape = new JProperty("protector_shape", data.protector_shape.ToString());

        requestobject.Add(title);
        requestobject.Add(injury_position);
        requestobject.Add(position);
        requestobject.Add(description);
        requestobject.Add(note);
        requestobject.Add(protector_shape);

        return requestobject.ToString();
    }


    /// <summary>
    ///  序列化matchingpoint所需的所有数据 发送那个给服务器
    /// </summary>
    /// <returns></returns>
    static public string MergMatchingpointNeed()
    {

        JObject jo = MergMatchingPointsJson(PointHelper.GetInstance().userdataformweb);
        jo["type"] = "obj";
        jo["case_id"] =  PlayerDataCenter.Currentillnessdata.ID;

        return jo.ToString();
    }





    static public string MergMatchingpointChangeNeed(PlayerDataCenter.IllNessData pd)
    {
        JObject requestobject = new JObject();
        JProperty md5 = new JProperty("md5", PlayerDataCenter.md5);
        JProperty decorativepattern = new JProperty("pattern", "花纹" +PlayerDataCenter.Decorativepattern);
        JProperty caseid = new JProperty("case_id", pd.ID);

        Dictionary<int, Dictionary<int, Vector3>> points = PointHelper.GetInstance().userdataformweb;

        //匹配点
        string key = Tool.matchingpointskey;
        JObject groupjobject = new JObject();
        foreach (KeyValuePair<int, Dictionary<int, Vector3>> item in points)
        {
            JObject pointsjobject = new JObject();
            string groupstr = Tool.NumberToChar(item.Key + 1);
            foreach (KeyValuePair<int, Vector3> it in item.Value)
            {
                string indexstr = (it.Key + 1).ToString();
                var jx = new JProperty("x", it.Value.x);
                var jy = new JProperty("y", it.Value.y);
                var jz = new JProperty("z", it.Value.z);

                JProperty pointproper = new JProperty(indexstr, new JObject(jx, jy, jz));
                pointsjobject.Add(pointproper);
            }
            JProperty groupproper = new JProperty(groupstr, new JObject(pointsjobject));
            groupjobject.Add(groupproper);

        }
        JProperty jend = new JProperty(key, groupjobject);

        //用户设置 + 避空位
        JObject userdata = new JObject();
        JProperty localuserdata = new JProperty(Tool.Localuserdatakey, ParseObjectToJson(PlayerDataCenter.CurrentLocaluserdata));

        Dictionary<int, Dictionary<int, Vector3>> avoidmap = AvoidVacancyHelper.avoidmap;
        //JObject avoidjobj = new JObject();
        if (null != avoidmap)
        {
            string avoidkey = Tool.Avoidkey;
            JObject avoidindex = new JObject();
            foreach (KeyValuePair<int, Dictionary<int, Vector3>> item in avoidmap)
            {
                JObject pointsjobject = new JObject();
                string groupstr = Tool.NumberToChar(item.Key + 1);
                foreach (KeyValuePair<int, Vector3> it in item.Value)
                {
                    string indexstr = (it.Key + 1).ToString();
                    var jx = new JProperty("x", it.Value.x);
                    var jy = new JProperty("y", it.Value.y);
                    var jz = new JProperty("z", it.Value.z);

                    JProperty pointproper = new JProperty(indexstr, new JObject(jx, jy, jz));
                    pointsjobject.Add(pointproper);
                }
                JProperty groupproper = new JProperty(groupstr, new JObject(pointsjobject));
                avoidindex.Add(groupproper);

            }
            JProperty avoidjend = new JProperty(avoidkey, avoidindex);
            //avoidjobj.Add(avoidjend);
            userdata.Add(new JProperty(avoidjend));

        }
        userdata.Add(new JProperty(localuserdata));
        JProperty moldinfo = new JProperty(Tool.alllocaldatakey, userdata);



        requestobject.Add(jend);
        requestobject.Add(moldinfo);
        requestobject.Add(md5);
        requestobject.Add(caseid);
        requestobject.Add(decorativepattern);

        return requestobject.ToString();
    }



    static public int GetPointID(string json)
    {
        int id = -1;
        var v = JObject.Parse(json);


        string pointsstr = v["data"]["points"].ToString();

        if (string.IsNullOrEmpty(pointsstr))
        {
            return id;
        }
        else
        {
            string idstr = v["data"]["points"]["point_id"].ToString();
            if (!string.IsNullOrEmpty(idstr))
            {
                return int.Parse(idstr);
            }
        }

        return id;
    }



    static public string ParseJsonToNeed(string json, params string[] key)
    {
        JObject o = JObject.Parse(json);


        JToken result = o;
        for (int i = 0; i < key.Length; i++)
        {
            if (null == result)
            {
                return "";
            }
            else {
                result = result[key[i]];
                if (result == null || string.IsNullOrEmpty(result.ToString()))
                {
                    return "";
                }
            }
        }
        if (null == result)
        {
            return "";
        }
        else
        {
            return (result.ToString());
        }

    }



    /// <summary>
    /// 病例详情中解析出需要的字段
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    static public Dictionary<int, Dictionary<int, Vector3>> ParseJsonToMatchingpointroot(string json)
    {
        Dictionary<int, Dictionary<int, Vector3>> map = new Dictionary<int, Dictionary<int, Vector3>>();


        var v = JObject.Parse(json);
        if (!string.IsNullOrEmpty(v[Tool.datakey].ToString()))
        {
            string _str = v[Tool.datakey][Tool.datapointkey].ToString();
            if (!string.IsNullOrEmpty(_str))
            {
                string matchingstr = v[Tool.datakey][Tool.datapointkey][Tool.matchingpointskey].ToString();
                JProperty jp = new JProperty(Tool.matchingpointskey, matchingstr);
                JObject jo = new JObject(jp);
                map = ParseMatchingpointsJosn(jo.ToString(), Tool.matchingpointskey);
            }
        }
        return map;
    }

    /// <summary>
    /// 解析json为避空位
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    static public Dictionary<int, Dictionary<int, Vector3>> ParseJsonToAvoid(string json)
    {
        Dictionary<int, Dictionary<int, Vector3>> map = new Dictionary<int, Dictionary<int, Vector3>>();


        var v = JObject.Parse(json);
        if (!string.IsNullOrEmpty(v[Tool.datakey].ToString()))
        {
            string _str = v[Tool.datakey][Tool.datapointkey].ToString();
            if (!string.IsNullOrEmpty(_str))
            {
                string matchingstr = v[Tool.datakey][Tool.datapointkey][Tool.alllocaldatakey][Tool.Avoidkey].ToString();
                JProperty jp = new JProperty(Tool.Avoidkey, matchingstr);
                JObject jo = new JObject(jp);
                map = ParseMatchingpointsJosn(jo.ToString(), Tool.Avoidkey);
            }
        }
        return map;

    }

    /// <summary>
    /// 解析json到LocalUserData
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    static public PlayerDataCenter.LoclaUserData ParseJsonToLocalUserData(string json)
    {
        PlayerDataCenter.LoclaUserData userdata = null;
        var v = JObject.Parse(json);
        if (!string.IsNullOrEmpty(v[Tool.datakey].ToString()))
        {
            string _str = v[Tool.datakey][Tool.datapointkey].ToString();
            if (!string.IsNullOrEmpty(_str))
            {
                string userdatastr = v[Tool.datakey][Tool.datapointkey][Tool.alllocaldatakey][Tool.Localuserdatakey].ToString();
                if (!string.IsNullOrEmpty(userdatastr))
                {
                    userdata = ParseJsonToNeed<PlayerDataCenter.LoclaUserData>(userdatastr);
                }
            }
        }
        return userdata;
    }

    /// <summary>
    /// 解析json到花纹
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    static public string ParseJsonToDecorativepattern(string json)
    {
        string result = "";
        var v = JObject.Parse(json);
        if (!string.IsNullOrEmpty(v[Tool.datakey].ToString()))
        {
            string _str = v[Tool.datakey][Tool.datapointkey].ToString();
            if (!string.IsNullOrEmpty(_str))
            {
                result = v[Tool.datakey][Tool.datapointkey][Tool.patternkey].ToString();
                 
            }
        }
        if (!string.IsNullOrEmpty(result))
        {
            result = result.Substring(result.Length - 1);
        }
        else {
            result = Tool.defultpattern;
        }
        return result;

    }


    /// <summary>
    /// 检查当前json的matchingpoint是否为null
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    static public bool ChackMatchingHasValue(string json)
    {
        var v = JObject.Parse(json);


        string  pointsstr = v["data"]["points"].ToString();

        if (string.IsNullOrEmpty(pointsstr))
        {
            return false;
        }
        else {
            string matchingpointstr = v["data"]["points"]["matching_point"].ToString();
            if(string.IsNullOrEmpty(matchingpointstr))
            {
                return false;
            }
        }
        return true;
        //if (array.Count > 0)
        //{
        //    string vid = array[0]["mold_id"].ToString();
        //    IllNessCenter.currentmold_id = int.Parse(vid);
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}

    }


    /// <summary>
    /// 新建病例时，服务器返回id，记录到本地
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    static public int ParseNewIllDataID(string key,string json)
    {
        JObject o = JObject.Parse(json);

        return int.Parse(o[key].ToString());
        
    }



    static public Dictionary<string,string> ParseDownloadPath(string json, params string[] keys)
    {
        Dictionary<string, string> map = new Dictionary<string, string>();


        if (string.IsNullOrEmpty(json))
        {
            return map;
        }

        JObject o = JObject.Parse(json);
        for (int i = 0; i < keys.Length; i++)
        {
            map.Add(keys[i], o[keys[i]].ToString());
        }

        return map;
    }


    /// <summary>
    /// 泛型解析出想要的数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    static public T ParseJsonToNeed<T>(string json)
    {
        T t = JsonConvert.DeserializeObject<T>(json);
        return t;

    }


    static public string ParseObjectToJson<T>(T t)
    {
        return JsonConvert.SerializeObject(t);
    }
}

#region 病例所需
public class DataItem
{
    public int case_id { get; set; }
    public string title { get; set; }
    public string injury_position { get; set; }
    public string position { get; set; }
    public string description { get; set; }
    public string note { get; set; }
    public string created_at { get; set; }
    public string updated_at { get; set; }
    public string protector_shape { get; set; }
}

public class IllDatalistRoot
{
    public string success { get; set; }
    public List<DataItem> data { get; set; }
}
#endregion

#region 用户保存的点所需

public class Pivot
{
    public int case_id { get; set; }
    public int mold_id { get; set; }
}

public class Medical_molds
{
    public int mold_id { get; set; }
    public string matching_point { get; set; }
    public string obj_path { get; set; }
    public string stl_path { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public Pivot pivot { get; set; }
}

public class MatchingInfo
{
    public int case_id { get; set; }
    public string title { get; set; }
    public string injury_position { get; set; }
    public int direction { get; set; }
    public string description { get; set; }
    public string note { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public string protector_shape { get; set; }
    public List<Medical_molds> medical_molds { get; set; }
}

public class DataOFMatchinRoot
{
    public bool success { get; set; }
    public MatchingInfo data { get; set; }
}


#endregion


#region 获取模型路径所需
public class RequestModelPath
{
    public string md5;
    public string decorativepattern;
    public string direction;

}

#endregion

