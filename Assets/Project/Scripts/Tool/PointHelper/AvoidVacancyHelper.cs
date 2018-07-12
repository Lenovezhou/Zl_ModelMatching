using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidVacancyHelper  {

    static private AvoidVacancyHelper instance;
    static public AvoidVacancyHelper GetInstance()
    {
        if (null == instance)
        {
            MSGCenter.Register(Enums.MainProcess.MainProcess_RePlay.ToString(), ReleseDownloadeModel);
            instance = new AvoidVacancyHelper();
        }
        return instance;
    }

    private static void ReleseDownloadeModel(string obj)
    {
        avoidmap.Clear();
    }

    Dictionary<int, GameObject> avoidvacancymap = new Dictionary<int, GameObject>();
    static public Dictionary<int, Dictionary<int, Vector3>> avoidmap = new Dictionary<int, Dictionary<int, Vector3>>();

    //当前正在编辑的避空位序号
    public int  currenteditvacancyindex;





}
