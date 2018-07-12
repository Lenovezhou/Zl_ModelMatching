using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchingpointPanelControl : ButtonOpenPanel,IPanelItem
{
    int dropdownindex = 0;
    int choiseindex = 0;
    GameObject Groupbuttonitem;
    GameObject Pointbuttonitem;
    GameObject toolpanel;
    Dictionary<int, Dictionary<int,Vector3>> pointmap = new Dictionary<int, Dictionary<int, Vector3>>();
    Dictionary<int, List<MatchingItemButton>> buttonmap = new Dictionary<int, List<MatchingItemButton>>();

    Dictionary<int, Button> buttonindex = new Dictionary<int, Button>();


    //编辑点时保存和取消按钮
    Button savepoint, canceledit;
    //编辑点时自动下一点的toggle
    Toggle autonexe;
    //上次选择的组button
    Button lastchoiselistbutton;

    //分组显示or全部显示dropdown
    Dropdown displaymodeDd;

    private MatchingItemButton lastchoiseitem;


    private void Assigne()
    {
        displaymodeDd = transform.Find("DisplaymodeDropdown").GetComponent<Dropdown>();
        Groupbuttonitem = transform.Find("ListScrollView/Viewport/Content/ListItem").gameObject;
        Pointbuttonitem = transform.Find("PointsScrollView/Viewport/Content/DecorativepatternItem").gameObject;

        toolpanel = transform.Find("Panel (1)").gameObject;
        savepoint = toolpanel.transform.Find("Button").GetComponent<Button>();
        canceledit = toolpanel.transform.Find("Button (1)").GetComponent<Button>();
        autonexe = transform.Find("Toggle").GetComponent<Toggle>();
    }

    public void Start ()
    {
        Assigne();
        savepoint.onClick.AddListener(() =>
        {
            if (PointHelper.GetInstance().CheckChoisePointer().OwnedPointer())
            {
                MSGCenter.Execute(Enums.MatchigPointGizmoControll.SaveMatchingpoint.ToString());
            }
        });
        canceledit.onClick.AddListener(() =>
        {
            transform.Find("Panel (1)").gameObject.SetActive(false);
            MSGCenter.Execute(Enums.MatchigPointGizmoControll.Cancle.ToString());
        });
        autonexe.onValueChanged.AddListener((iSon) =>
        {
            MSGCenter.Execute(Enums.MatchigPointGizmoControll.AutoNextToggle.ToString(), iSon.ToString());
        });

        //RefreshViewByData();

        MSGCenter.Register(Enums.MatchigPointGizmoControll.PointerDown.ToString(), PointerGizmoCallBack);
        MSGCenter.Register(Enums.MatchigPointGizmoControll.SaveMatchingpoint.ToString(),SaveToRefresh);
        MSGCenter.Register(Enums.MatchigPointGizmoControll.Cancle.ToString(), SaveToRefresh);
        MSGCenter.Register(Enums.MatchigPointGizmoControll.AutoNext.ToString(),AutoNext);

        MSGCenter.Register(Enums.MainProcess.MainProcess_RePlay.ToString(), ReplayRestDisplay);
       // MSGCenter.Register(Enums.MainProcess.MainProcess_loadedUserPointDone.ToString(), LoadUserPintdone);
        AssignDropdown(displaymodeDd);
    }

    private void LoadUserPintdone(string obj)
    {
        AssignDropdown(displaymodeDd);
    }

    void AssignDropdown(Dropdown Dd)
    {
        Dd.onValueChanged.RemoveAllListeners();
        Dd.InitDropDown(new List<string>() { "分组显示", "全部显示" });
        Dd.onValueChanged.AddListener(DropdownCallBack);
        MSGCenter.Execute(Enums.PointControll.Choise.ToString(), choiseindex.ToString());
    }
    void DropdownCallBack(int index)
    {
        dropdownindex = index;
        Enums.PointControll pcontroll = (Enums.PointControll)index;
        MSGCenter.Execute(pcontroll.ToString(),choiseindex.ToString());
    }

    void AutoNext(string message)
    {
        int group = PointHelper.GetInstance().currentgroup;
        int index = PointHelper.GetInstance().currentindex;

        MatchingItemButton b = SearchHelper.GetInstance().SerchChoise(buttonmap, group, index);
        b.OnEditHit();
    }

    /// <summary>
    /// 根据normalmodledata数据刷新两列button
    /// </summary>
    public void RefreshViewByNormalModleData()
    {
        Clear();
        pointmap = PointHelper.GetInstance().normaldataformlocaljson;//PointHelper.GetInstance().normalmodelInSceneMap;
        if (null == pointmap || pointmap.Count == 0)
        {
            Debug.LogError("pointmap 为空！！！");
            return;
        }
        foreach (KeyValuePair<int, Dictionary<int, Vector3>> item in pointmap)
        {
            int group = item.Key;
            GameObject but = SpawnChildren(Groupbuttonitem);
            Button b = but.GetComponent<Button>();
            string groupstr = Tool.NumberToChar(group + 1).ToUpper();
            b.GetComponentInChildren<Text>().text = groupstr;
            b.onClick.AddListener(() => 
            {
                ResetLastAndSetThis(b);
                if (dropdownindex == 0)
                {
                    MSGCenter.Execute(Enums.PointControll.Choise.ToString(),group.ToString());
                }
                ChoisePanel(b);
                choiseindex = group;
            });
            if (group == choiseindex)
            {
                ChoisePanel(b);
                lastchoiselistbutton = b;
            }
            Dictionary<int,Vector3> templist = item.Value;
            List<GameObject> tempgos = new List<GameObject>();
            List<MatchingItemButton> tempbuts = new List<MatchingItemButton>();

            foreach (KeyValuePair<int,Vector3> it in templist)
            {
                GameObject g =SpawnChildren(Pointbuttonitem);
                g.SetActive(false);
                int tempgroup = group;
                int tempindex = it.Key;
                g.GetComponentInChildren<Text>().text = groupstr +" "+ (tempindex + 1).ToString();

                MatchingItemButton Matchingitem = g.GetComponent<MatchingItemButton>();
                Matchingitem.Init(tempgroup, tempindex, SetLastMatchingItem);
                tempgos.Add(g);
                tempbuts.Add(Matchingitem);
            }
            listmap.Add(b, tempgos);
            buttonmap.Add(group, tempbuts);
            buttonindex.Add(group, b);
        }

        RefreshViewByUserModelData();


    }
    public void RefreshViewByUserModelData()
    {
        Dictionary<int, Dictionary<int, Vector3>> savedmap = PointHelper.GetInstance().userdataformweb;

        foreach (KeyValuePair<int, Dictionary<int, Vector3>> item in savedmap)
        {
            foreach (KeyValuePair<int, Vector3> it in item.Value)
            {
                MatchingItemButton mib = SearchHelper.GetInstance().SerchChoise(buttonmap, item.Key, it.Key);
                if (mib)
                {
                    mib.SaveMatchingpoint();
                }
            }
        }

    }


    /// <summary>
    /// 清空所有spawn的物体
    /// </summary>
    public void Clear()
    {
        listmap.Clear();
        buttonmap.Clear();
        buttonindex.Clear();
        for (int i = 0; i < spawnall.Count; i++)
        {
            Destroy(spawnall[i]);
        }
    }



    void PointerGizmoCallBack(string message)
    {
        Dictionary<int, string> map = MSGCenter.UnFormatMessage(message);
        string type = map[0];
        string strGroupindex = map[1];
        string strindex = map[2];

        int Groupindex = int.Parse(strGroupindex);
        int index = int.Parse(strindex);

        buttonindex[Groupindex].onClick.Invoke();
        ResetLastAndSetThis(buttonindex[Groupindex]);
        buttonmap[Groupindex][index].OnEditHit();
    }

    void ResetLastAndSetThis(Button currentlistbutton)
    {
        if (lastchoiselistbutton)
        {
            lastchoiselistbutton.GetComponent<Image>().color = Color.white;
        }
        currentlistbutton.GetComponent<Image>().color = Color.green;
        lastchoiselistbutton = currentlistbutton;
    }


    void SetLastMatchingItem(MatchingItemButton item)
    {
        if (lastchoiseitem)
        {
            lastchoiseitem.CheckMeNeedCancle();
        }
        toolpanel.SetActive(true);
        this.lastchoiseitem = item;
    }

    //当前点，点击保存后回调刷新
    void SaveToRefresh(string message)
    {
        transform.Find("Panel (1)").gameObject.SetActive(false);
        Enums.MatchigPointGizmoControll controll = (Enums.MatchigPointGizmoControll)Enum.Parse(typeof(Enums.MatchigPointGizmoControll), message);
        int group = PointHelper.GetInstance().currentgroup;
        int index = PointHelper.GetInstance().currentindex;
        if (buttonmap == null || buttonmap.Count == 0)
        {
            RefreshViewByNormalModleData();
        }
        MatchingItemButton it = SearchHelper.GetInstance().SerchChoise(buttonmap, group, index);
        if (!it)
        {
            return;
        }
        switch (controll)
        {
            case Enums.MatchigPointGizmoControll.SaveMatchingpoint:
                it.SaveMatchingpoint();
                break;
            case Enums.MatchigPointGizmoControll.Cancle:
                it.Cancle();
                break;
            default:
                break;
        }
    }

    void ReplayRestDisplay(string str)
    {
        if (buttonindex == null || buttonindex.Count == 0)
        {
            return;
        }
        buttonindex[0].onClick.Invoke();

        foreach (KeyValuePair<int,List<MatchingItemButton>> item in buttonmap)
        {
            List<MatchingItemButton> list = item.Value;
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Cancle();
            }
        }
    }

    /// <summary>
    /// 进入该页面的刷新
    /// </summary>
    public void OnEnterThisPage()
    {
        Assigne();
        RefreshViewByNormalModleData();
        transform.Find("Panel (1)").gameObject.SetActive(false);
        MSGCenter.Execute(Enums.LeftMouseButtonControl.AddMatchingGizmo.ToString(), "True");
        //TODO--->>>根据之前选择的[分组显示or全部显示]再次显示出对应点
        if (displaymodeDd)
        {
            displaymodeDd.onValueChanged.Invoke(dropdownindex);
        }
        if (lastchoiselistbutton)
        {
            lastchoiselistbutton.onClick.Invoke();
        }
    }

    /// <summary>
    /// 离开该页面的刷新
    /// </summary>
    public void OnLeveThisPage()
    {
        Clear();
        transform.Find("Panel (1)").gameObject.SetActive(false);
        MSGCenter.Execute(Enums.LeftMouseButtonControl.AddMatchingGizmo.ToString(), "False");
        //TODO--->>>隐藏所有点
        MSGCenter.Execute(Enums.PointControll.ShutDownAll.ToString());
        //摄像机的中心点恢复
        MSGCenter.Execute(Enums.ViewMode.ChangeTargetPos.ToString(), Vector3.zero.ToString());
    }

}
