using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class Model
{
    public abstract string Name { get; }
    public void SendEvent(string eventname, object obj = null) 
    {
        MVC.SendEvent(eventname, obj);
    }

}
