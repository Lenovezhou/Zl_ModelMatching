using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public abstract class View: MonoBehaviour
{
    public List<string> AttantionEvent = new List<string>();
    public abstract string Name { get; }


    public abstract void HandleEvent(string eventname, object obj = null);


    protected Model GetModle<T>() where T : Model 
    {
        return MVC.GetModel<T>();
    }


    protected void SendEvent(string eventname, object obj = null) 
    {
        MVC.SendEvent(eventname, obj);
    }
}
