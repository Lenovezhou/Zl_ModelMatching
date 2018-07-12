using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class MSGCenter 
{

	static private MSGCenter instance;
	static public MSGCenter GetInstance()
	{
		if (null == instance)
		{
			instance = new MSGCenter ();
		}
		return instance;
	}

	static Dictionary<string,Action<string>> map = new Dictionary<string, Action<string>> ();

    static Dictionary<string, Action<bool, string>> webrequestmap = new Dictionary<string, Action<bool, string>>();

	static public void Register(string type,Action<string> callback)
	{
		if (map.ContainsKey (type))
		{
			map [type] += callback;
		}
		else
		{
			map.Add (type, callback);
		}
	}
    static public void Register(string requesttype, Action<bool, string> callback)
    {
        webrequestmap.Add(requesttype, callback);
    }


	static public void Execute(string type,string arg = "")
	{
		if (map.ContainsKey (type)) 
		{
            if (string.IsNullOrEmpty(arg))
            {
                map[type](type);
            }
            else {
                map[type](type + "*"+arg);
            }
        } 
		else 
		{		
			Debug.LogError ( "事件类型："+type+"未注册");
		}

	}


    static public string FormatMessage(params string[] _params)
    {
        string splitstr = Tool.FormatMessageStr;
        int length = _params.Length;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            if (i == length - 1)
            {
                splitstr = "";
            }
            sb.Append(_params[i] + splitstr);
        }
        return sb.ToString();
    }


    static public Vector3 ParseToVector(string str)
    {
        string pos = str;
        string[] _s = pos.Split('(');
        string[] ss = _s[1].Split(')');
        string[] sss = ss[0].Split(',');
        float x = float.Parse(sss[0]);
        float y = float.Parse(sss[1]);
        float z = float.Parse(sss[2]);
        Vector3 v = new Vector3(x, y, z);
        return v;
    }


    static public Dictionary<int,string> UnFormatMessage(string message)
    {
        Dictionary<int, string> messagemap = new Dictionary<int, string>();
        char unsplitstr = Tool.UnFormatMessageChar;
        string[] strs = message.Split(unsplitstr);

        for (int i = 0; i < strs.Length; i++)
        {
            messagemap.Add(i, strs[i]);
        }

        return messagemap;
    }

}
