
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
public class MyWebRequest : Singleton<MyWebRequest> {

    //// Use this for initialization
    //void Start () {
    //       string Url = "http://demo.bio3d.com/api/test";
    //       string Url1 = "http://ers.momic.me/login";
    //       StartCoroutine(Test(Url));
    //   }

    //   IEnumerator Test(string url)
    //   {
    //       // http://demo.bio3d.com/api/mold

    //       UnityWebRequest Request = new UnityWebRequest(url);
    //       Request.method = UnityWebRequest.kHttpVerbGET;
    //       // Request.SetRequestHeader("matching_point", "{}");
    //       // Request.SetRequestHeader("type", "stl");

    //       // Request.SetRequestHeader("type", "stl");


    //       UnityWebRequestAsyncOperation Result = Request.SendWebRequest();
    //       while (Result.webRequest.downloadHandler == null || !Result.webRequest.downloadHandler.isDone)
    //       {
    //           yield return new WaitForSeconds(0.2f);
    //       }

    //       string data = Result.webRequest.downloadHandler.text;
    //       Debug.Log(data);

    //   }

    //   IEnumerator GetText(string url)
    //   {
    //       using (UnityWebRequest www = UnityWebRequest.Get(url))
    //       {
    //           yield return www.Send();

    //           if (www.isNetworkError || www.isHttpError)
    //           {
    //               Debug.Log(www.error);
    //           }
    //           else
    //           {
    //               // Show results as text
    //               Debug.Log(www.downloadHandler.text);

    //               // Or retrieve results as binary data
    //               byte[] results = www.downloadHandler.data;
    //           }
    //       }
    //   }






    //public void Send(Action<string> call)
    //{
    //    StartCoroutine(SendToPHP(Url, (str) => { Debug.Log(str); }));

    //}

    public void Get(string url,Action<bool,string> call)
    {
        string path = url;
        StartCoroutine(GetToPHP(path, call));

    }

    public void Put(string url,string message,Action<bool,string> call)
    {
        string path = url;
        StartCoroutine(PUTToPHP(path,message, call));
    }

    public void Post(string Url, string poststr, Action<bool, string> call)
    {
        StartCoroutine(PostToPHP(Url, poststr, call));
    }

    public void PostStl(string url, PlayerDataCenter.IllNessData Pd, Action<bool, string> call)
    {
        StartCoroutine(PostStlToPHP(url, Pd, call));
    }
    #region 上传文件
    IEnumerator PostStlToPHP(string url, PlayerDataCenter.IllNessData pd, Action<bool, string> call)
    {
        byte[] bytes = Tool.AuthGetFileData(pd.Modelpath);

        WWWForm form = new WWWForm();

        form.AddField("title", pd.title);
        form.AddField("injury_position", pd.injury_position.ToString());
        //int intposition = pd.position == PlayerDataCenter.IllNessData.Direction.Left ? 0 : 1;
        form.AddField("position", pd.position.ToString().ToLower());
        form.AddField("description", pd.description.ToString());
        form.AddField("note", pd.note);
        form.AddField("protector_shape", pd.protector_shape.ToString());

        form.AddBinaryData("stl", bytes, "3.stl");

        UnityWebRequest StlRequest = UnityWebRequest.Post(url, form);
        // StlRequest.SetRequestHeader("Content-Type", "application/json");
        StlRequest.SetRequestHeader("X-Requested-With", "XMLHttpRequest");

        yield return StlRequest.SendWebRequest();
        string result = StlRequest.downloadHandler.text;

        if (StlRequest.isNetworkError || StlRequest.isHttpError)
        {
            call(false, StlRequest.error);
        }
        else
        {
            if (StlRequest.responseCode == 200)
            {
                call(true, result);
            }
            else
            {
                call(false, result);
            }
        }
    }
    #endregion


    public void DownloadFileFromWed(string url,string downloadfile,string filename,Action<bool, string> Finish = null)
    {
        StartCoroutine(DownloadAndSave(url, downloadfile, filename, Finish));
    }



    private IEnumerator PostToPHP(string url, string postData, Action<bool,string> callback)
    {
        using (UnityWebRequest postrequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
        {
            byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(postData);
            postrequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(postBytes);
            postrequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            postrequest.method = UnityWebRequest.kHttpVerbPOST;
            postrequest.SetRequestHeader("Content-Type", "application/json");
            postrequest.SetRequestHeader("X-Requested-With", "XMLHttpRequest");

            yield return postrequest.Send();
            if (postrequest.isNetworkError)
            {
                if (null != callback)
                {
                    callback(false,postrequest.error);
                }
            }
            else
            {
                // Show results as text    
                if (postrequest.responseCode == 200)
                {
                    if (null != callback)
                    {
                        //string s = Encoding.UTF8.GetString(postrequest.downloadHandler.text)
                        callback(true, postrequest.downloadHandler.text);
                    }
                }
                else {
                    callback(false, postrequest.responseCode.ToString());
                }
            }
        }
    }


    private IEnumerator PUTToPHP(string url, string postData, Action<bool, string> callback)
    {
        using (UnityWebRequest putrequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPUT))
        {
            byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(postData);
            putrequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(postBytes);
            putrequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            putrequest.SetRequestHeader("Content-Type", "application/json");
            putrequest.SetRequestHeader("X-Requested-With", "XMLHttpRequest");

            yield return putrequest.Send();
            if (putrequest.isNetworkError)
            {
                if (null != callback)
                {
                    callback(false, putrequest.error);
                }
            }
            else
            {
                // Show results as text    
                if (putrequest.responseCode == 200)
                {
                    if (null != callback)
                    {
                        //string s = Encoding.UTF8.GetString(postrequest.downloadHandler.text)
                        callback(true, putrequest.downloadHandler.text);
                    }
                }
                else
                {
                    callback(false, putrequest.responseCode.ToString());
                }
            }
        }
    }
    private IEnumerator GetToPHP(string url, Action<bool,string> callback)
    {
        using (UnityWebRequest getrequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET))
        {
            //byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(postData);
            //postrequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(postBytes);
            getrequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            getrequest.SetRequestHeader("Content-Type", "application/json");
            getrequest.SetRequestHeader("X-Requested-With", "XMLHttpRequest");

            yield return getrequest.Send();
            if (getrequest.isNetworkError)
            {
                if (null != callback)
                {
                    callback(false,getrequest.error);
                }
            }
            else
            {
                // Show results as text    
                if (getrequest.responseCode == 200)
                {
                    if (null != callback)
                    {
                        //string s = Encoding.UTF8.GetString(postrequest.downloadHandler.text)
                        callback(true,getrequest.downloadHandler.text);
                    }
                }
                else
                {
                    callback(false, getrequest.responseCode.ToString());
                }
            }
        }
    }





    #region 下载资源并保存到本地
    /// <summary>  
    /// 下载并保存资源到本地  
    /// </summary>  
    /// <param name="url"></param>  
    /// <param name="name"></param>  
    /// <returns></returns>  
    private static IEnumerator DownloadAndSave(string url,string downloadfile, string name, Action<bool, string> Finish = null)
    {
        url = Uri.EscapeUriString(url);
        string Loading = string.Empty;
        bool b = false;
        WWW www = new WWW(url);
        if (www.error != null)
        {
            print("error:" + www.error);
        }
        while (!www.isDone)
        {
            Loading = (((int)(www.progress * 100)) % 100) + "%";
            if (Finish != null)
            {
                Finish(b, Loading);
            }
            yield return 1;
        }
        if (www.isDone)
        {
            Loading = "100%";
            byte[] bytes = www.bytes;
            //b = SaveAssets(Application.persistentDataPath, name, bytes);
            b = SaveAssets(downloadfile, name, bytes);

            if (Finish != null)
            {
                Finish(b, Loading);
            }
        }
    }




    /// <summary>  
    /// 保存资源到本地  
    /// </summary>  
    /// <param name="path"></param>  
    /// <param name="name"></param>  
    /// <param name="info"></param>  
    /// <param name="length"></param>  
    private static bool SaveAssets(string path, string name, byte[] bytes)
    {
        Stream sw;
        FileInfo t = new FileInfo(path + "//" + name);
        if (t.Exists)
        {
            File.Delete(t.FullName);
        }
        try
        {
            sw = t.Create();
            sw.Write(bytes, 0, bytes.Length);
            sw.Close();
            sw.Dispose();
            return true;
        }
        catch
        {
            return false;
        }
    }
    #endregion



    #region 上传文件
    IEnumerator getTexture2d()
    {
        byte[] bytes = AuthGetFileData(Tool.LocalModelonSavePath + "3.stl");

        WWWForm form = new WWWForm();
        form.AddField("id", 1);
        form.AddField("data", "{'data': 'this is a json!'}");
        form.AddBinaryData("stl", bytes, "3.stl");

        UnityWebRequest www = UnityWebRequest.Post("http://demo.bio3d.com/api/file", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) { Debug.Log(www.error); }
        else
        {
            Debug.Log("Form upload complete!");
            Debug.Log(www);
        }
    }

    public byte[] AuthGetFileData(string fileUrl)
    {
        FileStream fs = new FileStream(fileUrl, FileMode.Open, FileAccess.Read);
        byte[] buffur = new byte[fs.Length];

        fs.Read(buffur, 0, buffur.Length);
        fs.Close();
        return buffur;
    }

    #endregion

}
