using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIThirdPage : TTUIPage
{

	public UIThirdPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
	{
		uiPath = "UIPrefab/UIThirdPage";
	}

    ThirdPageTitleController thirdpagetitlecontroller;

    public override void Awake(GameObject go)
    {
        transform.Find("Modeldisplayconcealment").gameObject.AddComponent<ThiredPageModeldisplayController>();
        transform.Find("ViewPanel").gameObject.AddComponent<ThiredPageViewController>();
        thirdpagetitlecontroller = transform.Find("Title").gameObject.AddComponent<ThirdPageTitleController>();
    }

    public override void Refresh()
    {
        //TODO ---->>> 加载用户选择模型及数据
        PlayerDataCenter.IllNessData ind = data as PlayerDataCenter.IllNessData;
        thirdpagetitlecontroller.EnterThiredPageRefresh(ind);

        ShowPage<UINotice>(Tool.ConnectingStr);
        //①加载matchingpoint在thirdpage
        string url = Tool.requestdetailillnessdatapath + PlayerDataCenter.Currentillnessdata.ID;

        MyWebRequest.Instance.Get(url, (success, str) =>
         {
             if (success)
             {
                 //将获取到的matchingpoint存储到Map内
                 PointHelper.GetInstance().userdataformweb = JsonHelper.ParseJsonToMatchingpointroot(str);
                 //标准点
                 PointHelper.GetInstance().normaldataformlocaljson = JsonHelper.LoadJsonFromFile();
                 //避空位
                 AvoidVacancyHelper.avoidmap = JsonHelper.ParseJsonToAvoid(str);
                 //获取上次用户设置
                 PlayerDataCenter.CurrentLocaluserdata = JsonHelper.ParseJsonToLocalUserData(str);
                 //获取花纹
                 PlayerDataCenter.Decorativepattern = JsonHelper.ParseJsonToDecorativepattern(str);
                 //当前stl模型在服务器的地址
                 PlayerDataCenter.stlInServerpath = JsonHelper.ParseJsonToNeed(str, Tool.datakey, Tool.stlInServerpath);

                 string message = MSGCenter.FormatMessage(PlayerDataCenter.stlInServerpath, PlayerDataCenter.md5);
                 MSGCenter.Execute(Enums.ModelPath.ModelPath.ToString(), message);

                 string PieceProtectorURL = JsonHelper.ParseJsonToNeed(str,Tool.datakey,Tool.PieceProtectorURLkey);

                 PointHelper.GetInstance().DownloadSave(PieceProtectorURL);

                 //执行加载用户保存点并自动刷新第三页UI列表
                 MSGCenter.Execute(Enums.MatchigPointGizmoControll.LoadUserData.ToString());
             }
             else
             {
                 ShowPage<UINotice>(str);
             }
         });


    }

  


}
