using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class MVC  
{
    public static Dictionary<string, Model> Models = new Dictionary<string, Model>();   //名字--模型
    public static Dictionary<string, View> Views = new Dictionary<string, View>();      //名字--视图
    public static Dictionary<string, Type> CommendMap = new Dictionary<string, Type>(); //事件名字--控制器类型


    //注册
    public static void RegisterModel(Model m) 
    {
        Models[m.Name] = m;
    }

    public static void RegisterView(View v)
    {
        Views[v.Name] = v;
    }

    public static void RegisterCommendMap(string eventname,Type controllerType) 
    {
        CommendMap[eventname] = controllerType;
    }

    //查询获取
    public static Model GetModel<T>() where T : Model
    {
        foreach (Model item in Models.Values)
        {
            if (item is T)
                return item;
        }
        return null;
    }

    public static View GetView<T>() where T : View
    {
        foreach (View item in Views.Values)
        {
            if (item is T)
                return item;
        }
        return null;
    }


    //发送消息

    public static void SendEvent(string eventname, object obj = null) 
    {
        //controller先响应
        if (CommendMap.ContainsKey(eventname))
        {
            Type t = CommendMap[eventname];
            Controller c = Activator.CreateInstance(t) as Controller;
            c.Execute(obj);
        }

        //然后是view 视图再响应

        foreach (View item in Views.Values)
        {
            if (item.AttantionEvent.Contains("eventname"))
            {
                item.HandleEvent(eventname, obj);
            }
        }
    }

}
