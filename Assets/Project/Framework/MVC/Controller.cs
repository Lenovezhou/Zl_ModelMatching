using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public abstract class Controller
{
    public abstract void Execute (object obj = null);


    public Model GetModel<T>() where T:Model
    {
        return MVC.GetModel<T>();
    }

    public View GetView<T>() where T : View
    {
        return MVC.GetView<T>();
    }

    public void RegisterModel(Model m) 
    {
        MVC.RegisterModel(m);
    }

    public void RegisterView(View v) 
    {
        MVC.RegisterView(v);
    }

    public void SendEvent(string eventname, object obj = null) 
    {
        MVC.SendEvent(eventname, obj);
    }
}
