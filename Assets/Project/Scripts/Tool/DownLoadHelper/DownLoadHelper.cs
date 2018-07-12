using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownLoadHelper
{
    static private DownLoadHelper instance;

    static public DownLoadHelper Instance
    {
        get
        {
            if (null == instance)
            {
                instance = new DownLoadHelper();
            }
            return instance;
        }

        set
        {
            instance = value;
        }
    }

    /// <summary>
    /// 是否正在创建
    /// </summary>
    bool iscreating = false;

    Func<bool> call;
    //检查场景内是否已经加载好片体和护具模型
    public bool Canexport
    {
        get { return !iscreating && null != call && call(); }
    }

    public void CreateDownloadeModel(STL loader, string path, Transform parent,Transform target,Func<bool> call)
    {
        this.call = call;
        while (iscreating)
        {

        }

        if (path.IsStl())
        {
            try
            {
                iscreating = true;
                loader.CreateInstance(path, (result) =>
                {
                    //MSGCenter.Execute(Enums.ModelPath.Result.ToString(), result);
                },
                (go) =>
                {
                    ReleseDownloadeModel(parent);
                    go.transform.localScale = Vector3.one * Tool.UserImportScaler * 1.1f;
                    parent.transform.position = target.position;
                    parent.rotation = target.rotation;
                    go.transform.SetParent(parent.transform);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    iscreating = false;
                }
               );
            }
            catch (System.Exception e)
            {
                MSGCenter.Execute(Enums.ModelPath.Result.ToString(), e.ToString());
                throw;
            }

        }

    }
    void ReleseDownloadeModel(Transform parent)
    {
        if (parent.transform.childCount > 0)
        {
            for (int i = 0; i < parent.transform.childCount; i++)
            {
               GameObject.Destroy(parent.transform.GetChild(i).gameObject);
            }
        }
    }


    public bool CanExportModel()
    {
        return iscreating;
    }
}
