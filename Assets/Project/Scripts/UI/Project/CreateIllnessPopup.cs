using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateIllnessPopup : TTUIPage {


    public CreateIllnessPopup() : base(UIType.PopUp, UIMode.DoNothing, UICollider.Normal)
    {
        uiPath = "UIPrefab/CreateIllnessPopup";
    }

    int currentillnessID = 0;

    int illnessindex = 0;

    Text wormingdisplay;
    string filepath;

    Dropdown injury_positionDropdown, protector_shapeDropdown, directionDropdown;
    InputField illdatatitle, illdatadescription, illdatanote;

    Button Commitbutton;
    List<string> xmlpaths = new List<string>();

    [SerializeField]
    PlayerDataCenter.IllNessData currentilldata;

    public override void Awake(GameObject go)
    {
        MSGCenter.Register(Enums.ModelPath.Result.ToString(), LoadModelResult);
        wormingdisplay = transform.Find("Panel/Text").GetComponent<Text>();
        transform.Find("Panel/ShowFileButton").GetComponent<Button>().onClick.AddListener(OpenFileDisplay);


        #region FindChild

        Dropdown caseofillness = transform.Find("Panel/caseofillnessDropdown").GetComponent<Dropdown>();
        List<string> a = new List<string>();
        List<string> DropdownName = new List<string>();
        string path = Application.dataPath + "/LocalSave";
        Tool.GetAllFiles(path, a);

        for (int i = 0; i < a.Count; i++)
        {
            if (!a[i].Contains("meta"))
            {
                xmlpaths.Add(a[i]);
                string[] sss = a[i].Split('\\');

                DropdownName.Add(sss[sss.Length - 1]);
            }
        }
        caseofillness.InitDropDown(DropdownName);

        caseofillness.onValueChanged.AddListener((index) =>
        {
            illnessindex = index;
        });

        transform.Find("Panel/IllnessButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            string xmlpath = xmlpaths[illnessindex];

            Hide();
        });


        injury_positionDropdown = transform.Find("Panel/Dropdown").GetComponent<Dropdown>();
        protector_shapeDropdown = transform.Find("Panel/Dropdown (2)").GetComponent<Dropdown>();
        directionDropdown = transform.Find("Panel/Dropdown (1)").GetComponent<Dropdown>();

        injury_positionDropdown.InitDropDown(Tool.InjuryPosition);
        protector_shapeDropdown.InitDropDown(Tool.protector_shape);
        directionDropdown.InitDropDown(Tool.Illposition);

        //点击确定发送请求到服务器
        Commitbutton = transform.Find("Panel/CommitButton").GetComponent<Button>();

        //病例名称
        illdatatitle = transform.Find("Panel/InputField").GetComponent<InputField>();

        //病情描述
        illdatadescription = transform.Find("Panel/InputField (1)").GetComponent<InputField>();
        
        //备注
        illdatanote = transform.Find("Panel/InputField (2)").GetComponent<InputField>();

        #endregion


        #region 初始化UI事件
        //取消
        transform.Find("Panel/CancleButton").GetComponent<Button>().onClick.AddListener(Hide);

        //部位
        injury_positionDropdown.onValueChanged.AddListener((index) =>
        {
            currentilldata.injury_position = Tool.InjuryPosition[index];
        });
        //外形
        protector_shapeDropdown.onValueChanged.AddListener((index) =>
        {
            currentilldata.protector_shape = (PlayerDataCenter.IllNessData.ProtectorShape)index;
        });
        //方向
        directionDropdown.onValueChanged.AddListener((index) =>
        {
            currentilldata.position = (PlayerDataCenter.IllNessData.Direction)index;
        });

        //病例名称
        illdatatitle.onEndEdit.AddListener((str) =>
        {
            currentilldata.title = str;
        });

        //病情描述
        illdatadescription.onEndEdit.AddListener((str) =>
        {
            currentilldata.description = str;
        });

        //备注
        illdatanote.onEndEdit.AddListener((str) =>
        {
            currentilldata.note = str;
        });
        #endregion

    }



    void OpenFileDisplay()
    {
        filepath = "";
        filepath = Tool.OpenFileDisplay();
        if (string.IsNullOrEmpty(filepath))
        {
            string result = "请选择路径!!!";
            LoadModelResult(result);
        }
        else
        {
            PointHelper.usermodlepath = filepath;
            currentilldata.Modelpath = filepath;
        }
    }

    void LoadModelResult(string result)
    {
        string str = "请正确上传患者模型";
        if (result.Split('*').Length > 1)
        {
            str = result.Split('*')[1];
        }
        Sequence seq = DOTween.Sequence();
        seq.Append(this.wormingdisplay.DOColor(Color.red, 0.5f).SetEase(Ease.Linear).SetLoops(3));
        //seq.Append(this.transform.DOMove(vecPosition, time).SetEase(Ease.Linear));
        seq.OnComplete(delegate
        {
            wormingdisplay.color = Color.black;
        });
        wormingdisplay.text = str;
    }


    bool CheckDataAllright()
    {
        if (string.IsNullOrEmpty(currentilldata.Modelpath)||string.IsNullOrEmpty(currentilldata.title))
        {
            return false;
        }
        else {
            return true;
        }
    }



    void AddIllNess()
    {
        if (CheckDataAllright())
        {
            currentilldata.JsonPath = xmlpaths[illnessindex];
            //TODO ----->>>>>>>>SendRequest To PHP
            string Url = Tool.addillnessdatasimplepath;
            MyWebRequest.Instance.PostStl(Url,currentilldata, (success,str) =>
            {
                if (success)
                {
                    currentilldata.ID = JsonHelper.ParseNewIllDataID("data",str);
                    currentilldata.illcreatetime = DateTime.Now.ToString();

                    PlayerDataCenter.AddIllness(currentilldata);
                    ShowPage<UIFirstPage>();
                    Hide();
                }
                else {
                    ShowPage<UINotice>(Tool.FaleToConnect);
                }
            });
        }
        else
        {
            string result = "请选择路径!!!";
            LoadModelResult(result);
        }

    }

    public override void Refresh()
    {
        PlayerDataCenter.IllNessData da = data as PlayerDataCenter.IllNessData;
        RefreshDisplay(da);
       
    }


    void RefreshDisplay(PlayerDataCenter.IllNessData da)
    {
        currentilldata = da != null ? da : new PlayerDataCenter.IllNessData() {
            ID = -1,
            injury_position = "脚",
            protector_shape =  PlayerDataCenter.IllNessData.ProtectorShape.Short,
            position = PlayerDataCenter.IllNessData.Direction.RIGHT,
            title ="",
            description = "",
            note = ""
        };

        currentillnessID = currentilldata.ID;
        injury_positionDropdown.value = Tool.InjuryPosition.IndexOf(currentilldata.injury_position);
        protector_shapeDropdown.value = (int)currentilldata.protector_shape;
        directionDropdown.value = (int)currentilldata.position;
        illdatatitle.text = currentilldata.title;
        illdatadescription.text = currentilldata.description;
        illdatanote.text = currentilldata.note;


        //点击确定发送请求到服务器
        Commitbutton.onClick.RemoveAllListeners();
        if (null == da)
        {
            Commitbutton.onClick.AddListener(AddIllNess);
        }
        else
        {
            Commitbutton.onClick.AddListener(() =>
            {
                string url = Tool.refreshillnessdatasimplepath + currentillnessID.ToString();

                string json = JsonHelper.MergAddIllnessdataJson(currentilldata);
                MyWebRequest.Instance.Put(url, json, (success, str) =>
                {
                    if (success)
                    {
                        ShowPage<UIFirstPage>();
                        Hide();
                    }
                    else
                    {
                        ShowPage<UINotice>(str);
                    }

                });
            });
        }

    }
}


