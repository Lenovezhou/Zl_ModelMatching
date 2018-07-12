using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UISecendPage : TTUIPage {

    public UISecendPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
    {
		uiPath = "UIPrefab/UISecendPage";
    }

    public override void Awake(GameObject go)
    {
		AssignDropDown();
        this.transform.Find("btn_skill").GetComponent<Button>().onClick.AddListener(() =>
        {
            ShowPage<UISkillPage>();
        });

        this.transform.Find("btn_battle").GetComponent<Button>().onClick.AddListener(() =>
        {
            ShowPage<UIBattle>();
        });
		this.transform.Find ("CommitButton").GetComponent<Button> ().onClick.AddListener (() => 
        {
            MSGCenter.Execute(Enums.LeftMouseAlive.TurnOff.ToString());
           // ShowPage<UIThirdPage>();
		});
    }

    public override void Refresh()
    {
        MSGCenter.Execute(Enums.CameraMode.NormalCam.ToString());
        MSGCenter.Execute(Enums.CameraTarget.WithoutTarget.ToString());
        MSGCenter.Execute(Enums.LeftMouseAlive.TurnOn.ToString());

        //TODO ---->>> 加载用户选择模型及数据
        PlayerDataCenter.IllNessData ind = data as PlayerDataCenter.IllNessData;
        if (null != ind)
        {
            //①根据服务器存储路径加载本地模型
            string Modelpath = ind.Modelpath;//"D:/Test";

            //②如果服务器没有存储则根据ID查找本地存储的模型
            if (string .IsNullOrEmpty(Modelpath))
            {
                Modelpath = Tool.LocalModelonSavePath + ind.ID.ToString()+ ".obj";
            }

            //③最终确认是否包含该路径如果没有则加载默认模型
            if (!Tool.CheckFileExist(Modelpath))
            {
                Modelpath = Tool.ModleDefaultPath;
            }
            Debug.Log(Modelpath);
            //MSGCenter.Execute(Enums.ModelPath.ModelPath.ToString(), Modelpath);
        }
    }

    void AssignDropDown()
	{
		List<string> Translate_namelist = new List<string> (){ "旋转","移动"};
		Dropdown TranslateDropdown = transform.Find ("TranslateDropdown").GetComponent<Dropdown> ();
        TranslateDropdown.InitDropDown(Translate_namelist);
		TranslateDropdown.onValueChanged.AddListener (OnTranslateChange);


		List<string> View_namelist = new List<string> (){ "正视图","左视图","底视图","右视图","后视图","顶视图"};
		Dropdown ViewDropdown = transform.Find ("ViewDropdown").GetComponent<Dropdown> ();
        ViewDropdown.InitDropDown(View_namelist);
		ViewDropdown.onValueChanged.AddListener (OnViewChange);

	}


	void OnTranslateChange(int index)
	{
		Enums.LeftMouseButtonControl  messagetype = (Enums.LeftMouseButtonControl )index;
		MSGCenter.Execute(messagetype.ToString());
	}


	void OnViewChange(int index)
	{
		Enums.ViewMode  messagetype = (Enums.ViewMode )index;
		MSGCenter.Execute(messagetype.ToString());
	}

    //继承后，没有重写虚方法，调用的是父类的方法，如果有重写则是调用自己重写后的方法
  //  public override void Refresh() { Debug.Log("step,that"); }
    
}
