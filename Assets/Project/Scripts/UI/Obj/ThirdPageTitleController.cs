using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThirdPageTitleController : ButtonOpenPanel
{
    #region UI变量说明
    //导出dorpdown
    Dropdown ExportDropdown;
    Button 
        Matchingpoint,              //匹配点
        Avoidvacancy,               //避空位
        DecorativepatternButton,    //花纹
        BandageButton,              //扎带
        OtherButton,                //其它
        showmainUIbutton,           //弹出主控制面板的button
        userguide;                  //使用说明

    Button ResetRotationbutton;     //重设角度

    //tool顶端的text显示
    Text toolpaneltitle;            //顶端title显示

    Transform 
        MatchingpointPanel,         //匹配点面板
        AvoidvacancyPanel,          //避空位面板
        DecorativepatternPanel,     //花纹面板
        BandagePanel,               //扎带面板
        OtherPanel,                 //其它面板
        MainUIPanel,                //弹出主控制面板
        ResetNoticePanel,           //重设角度面板
        UserguidePanel;             //使用说明面板

    MatchingpointPanelControl matchingpointpanelcontrol;
    InfoPanelControl infopanelcontrol;
    Button CommitButton;


    //花纹UI预设
    GameObject DecorativepatternItem;
    #endregion
    enum ExportManu { All, Protectiveclothing, Body}

    ExportManu exprotmanu = ExportManu.All;

    public void Start ()
    {
        MouseButtonController.Instance.mousebuttondownhit += go => 
        {
            if (go != null)
            {
                if (Tool.ContainsChild(MainUIPanel.gameObject, go.name) || go.name == "Blocker")
                {
                    return;
                }
            }
            MainUIPanel.gameObject.SetActive(false);
        };
        //信息面板
        infopanelcontrol = transform.Find("InfoPanel").gameObject.AddComponent<InfoPanelControl>();

        MainUIPanel = transform.Find("Panel");
        //三UI显示面板
        showmainUIbutton = transform.Find("Button").GetComponent<Button>();
        showmainUIbutton.onClick.AddListener(() => 
        {
            //ChoisePanel(showmainUIbutton);
            //ExportDropdown.Show();
            MainUIPanel.gameObject.SetActive(!MainUIPanel.gameObject.activeSelf);
        });


        //重设角度
        ResetRotationbutton = transform.Find("ResetRotationBut").GetComponent<Button>();
        ResetRotationbutton.onClick.AddListener(() =>
        {
            //选择匹配点界面，强制关闭
            ChoisePanel(ResetRotationbutton);
        });
        //导出保存
        transform.Find("Panel/SaveButton").GetComponent<Button>().onClick.AddListener(Exprot);
        //取消
        transform.Find("Panel/CamcelButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            ChoisePanel(userguide);

            MSGCenter.Execute(Enums.MainProcess.MainProcess_RePlay.ToString());
            MainUIPanel.gameObject.SetActive(!MainUIPanel.gameObject.activeSelf);
            TTUIPage.ShowPage<UIFirstPage>();
        });
        //导出
        ExportDropdown = transform.Find("Panel/ExportDropdown").GetComponent<Dropdown>();
        AssignDropdown(ExportDropdown);
        ExportDropdown.onValueChanged.AddListener(ExportDropdownChange);

        
        //匹配点
        Matchingpoint = transform.Find("MatchingpointButton").GetComponent<Button>();
        //避空位
        Avoidvacancy = transform.Find("AvoidvacancyButton").GetComponent<Button>();
        //参数设置
        //Parametersetting = transform.Find("ParametersettingButton").GetComponent<Button>();
        //花纹
        DecorativepatternButton = transform.Find("DecorativepatternButton").GetComponent<Button>();
        //扎带
        BandageButton = transform.Find("BandageButton").GetComponent<Button>();
        //其它
        OtherButton = transform.Find("OtherButton").GetComponent<Button>();
        //使用说明
        userguide = transform.Find("UserGuide").GetComponent<Button>();


        Transform tools = transform.parent.Find("Tools");

        toolpaneltitle = tools.Find("titletext").GetComponent<Text>();

        MatchingpointPanel = tools.transform.Find("MatchingpointPanel");
        AvoidvacancyPanel = tools.transform.Find("AvoidvacancyPanel");

        DecorativepatternPanel = tools.transform.Find("DecorativepatternPanel");
        BandagePanel = tools.transform.Find("BandagePanel");
        OtherPanel = tools.transform.Find("OtherPanel");
        ResetNoticePanel = tools.Find("ResetNotice");
        ResetNoticePanel.gameObject.AddComponent<ResetNoticeTools>();
        UserguidePanel = tools.Find("UserGuidePanel");

        //花纹UI预设
        DecorativepatternItem = DecorativepatternPanel.transform.Find("Scroll View/Viewport/Content/DecorativepatternItem").gameObject;


        matchingpointpanelcontrol = MatchingpointPanel.gameObject.AddComponent<MatchingpointPanelControl>();
        AvoidvacancyPanel.gameObject.AddComponent<AvoidvacancyPanelControl>();
        BandagePanel.gameObject.AddComponent<BandagePanelControl>();
        OtherPanel.gameObject.AddComponent<OtherPanelControl>();

        RegestToMap(Matchingpoint, MatchingpointPanel.gameObject);
        RegestToMap(Avoidvacancy, AvoidvacancyPanel.gameObject);
        RegestToMap(DecorativepatternButton, DecorativepatternPanel.gameObject);
        RegestToMap(BandageButton, BandagePanel.gameObject);
        RegestToMap(OtherButton, OtherPanel.gameObject);
        RegestToMap(showmainUIbutton, MainUIPanel.gameObject);
        RegestToMap(ResetRotationbutton, ResetNoticePanel.gameObject);
        RegestToMap(userguide, UserguidePanel.gameObject);



        //匹配点
        Matchingpoint.onClick.AddListener(() =>
        {
            ChoisePanel(Matchingpoint);
            ActiveAddGizmo(true);
        });

        //避空位
        Avoidvacancy.onClick.AddListener(() =>
        {
            ChoisePanel(Avoidvacancy);
            ActiveAddGizmo(false);
        });

        //花纹
        DecorativepatternButton.onClick.AddListener(() => 
        {
            ChoisePanel(DecorativepatternButton);
        });


        //扎带
        BandageButton.onClick.AddListener(() => 
        {
            ChoisePanel(BandageButton);
        });

        //其它
        OtherButton.onClick.AddListener(() => 
        {
            ChoisePanel(OtherButton);
        });
        //使用说明
        userguide.onClick.AddListener(() => 
        {
            ChoisePanel(userguide);
        });

        CommitButton = tools.transform.Find("CommitButton").GetComponent<Button>();

        //提交
        CommitButton.onClick.AddListener(() =>
        {
            //①序列化当前数据
            PointHelper.GetInstance().SaveAll();

            //②将现在的所有数据提交到web


        });

        AssigneDecorativepatterns();
        ChoisePanel(userguide);
    }


    void ActiveAddGizmo(bool active)
    {
        string act = "False";
        if (active)
        {
            act = "True";
        }
        int group = PointHelper.GetInstance().currentgroup;
        int index = PointHelper.GetInstance().currentindex;
        string message = MSGCenter.FormatMessage(act, group.ToString(), index.ToString());
        MSGCenter.Execute(Enums.LeftMouseButtonControl.AddMatchingGizmo.ToString(), message);
    }


    void AssignDropdown(Dropdown dropdown)
    {
        List<string> namelist = new List<string>() { "全部导出", "导出护具","导出片体" };
        dropdown.InitDropDown(namelist);
    }


    void ExportDropdownChange(int index)
    {
        exprotmanu = (ExportManu)index;
    }

    List<GameObject> decorativelis = new List<GameObject>();
    void AssigneDecorativepatterns()
    {
        Texture[] Decorativepatterns = Resources.LoadAll<Texture>(Tool.LocalDecorativepatternPath);

        for (int i = 0; i < Decorativepatterns.Length; i++)
        {
            GameObject go = SpawnChildren(DecorativepatternItem);
            decorativelis.Add(go);
            go.GetComponentInChildren<RawImage>().texture = Decorativepatterns[i];
            go.GetComponentInChildren<Text>().text = Decorativepatterns[i].name;
            go.name = (i + 1).ToString();

            if (string.Equals(PlayerDataCenter.Decorativepattern,go.name))
            {
                go.GetComponent<Image>().color = Color.green;
            }


            go.GetComponent<Button>().onClick.AddListener(() => 
            {
                for (int j = 0; j < decorativelis.Count; j++)
                {
                    if (decorativelis[j] != go)
                    {
                        decorativelis[j].GetComponent<Image>().color = Color.gray;
                    }
                    else {
                        decorativelis[j].GetComponent<Image>().color = Color.green;
                    }
                }
                PlayerDataCenter.Decorativepattern = go.name;
            });
        }
    }


    /// <summary>
    /// 点击按钮选择panel之后，父类回调
    /// </summary>
    /// <param name="bu"></param>
    protected override void ChoiseEnd(Button bu)
    {
        toolpaneltitle.text = bu.GetComponentInChildren<Text>().text;
        //只有点击重调角度才可以打开左键控制
        if (bu == ResetRotationbutton)
        {
            MSGCenter.Execute(Enums.LeftMouseAlive.TurnOn.ToString());
        }
        else
        {
            MSGCenter.Execute(Enums.LeftMouseAlive.TurnOff.ToString());
        }
    }


    void Exprot()
    {
        //判断是否提交mathingpoint并且展示出模型
        bool canexport = DownLoadHelper.Instance.Canexport;
        if (canexport)
        {
            string savepath = Tool.SaveFileDisplay();
            string nowtime = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string fileformat = ".stl";
            string bodyfilefullpath = savepath + "/" + nowtime + "body.stl";
            string Protectiveclothingfilefullpath = savepath + "/" + nowtime + "Protectiveclothing.stl";
            int id = PlayerDataCenter.Currentillnessdata.ID;
            switch (exprotmanu)
            {
                case ExportManu.All:
                    Tool.CopyFile(Tool.SaveDownLoadFromWebPath + id + Tool.downloadbodykey + fileformat, bodyfilefullpath);
                    Tool.CopyFile(Tool.SaveDownLoadFromWebPath + id + Tool.Protectiveclothingkey + fileformat, Protectiveclothingfilefullpath);
                    break;
                case ExportManu.Protectiveclothing:
                    Tool.CopyFile(Tool.SaveDownLoadFromWebPath + id + Tool.Protectiveclothingkey + fileformat, Protectiveclothingfilefullpath);
                    break;
                case ExportManu.Body:
                    Tool.CopyFile(Tool.SaveDownLoadFromWebPath + id + Tool.downloadbodykey + fileformat, bodyfilefullpath);
                    break;
                default:
                    break;
            }
            string messagestr = MSGCenter.FormatMessage("保存完成,请去 \r\n" + savepath + " 查看", "4");
            TTUIPage.ShowPage<UINotice>(messagestr);
        }
        else
        {
            TTUIPage.ShowPage<UINotice>("未正常下载,稍后重试");
        }

    }

    public void EnterThiredPageRefresh(PlayerDataCenter.IllNessData data)
    {
        if (!infopanelcontrol)
        {
            infopanelcontrol = transform.Find("InfoPanel").gameObject.AddComponent<InfoPanelControl>();
        }
        infopanelcontrol.RefreshInfo(data);
        if (userguide)
        {
            ChoisePanel(userguide);
        }
    }

}
