using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Text;

public class UIFirstPage : TTUIPage 
{
	
	public UIFirstPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
	{
		uiPath = "UIPrefab/UIFirstPage";
	}
    private int hereillnesscount = 0;
    private GameObject illnessprefabe;
    Dictionary<int,GameObject> allhereilldatas = new Dictionary<int, GameObject>();
    public override void Awake(GameObject go)
    {
        ShowPage<UINotice>(Tool.ConnectingStr);

        //添加新病例
        transform.Find("Panel (1)/Button").GetComponent<Button>().onClick.AddListener(
            () => 
            {
                ShowPage<CreateIllnessPopup>();
            });
        illnessprefabe = transform.Find("Panel (1)/Scroll View/Viewport/Content/Item").gameObject;

        PlayerDataCenter.callback = CallbackRefresh;
    }


    void SpawnIllData(PlayerDataCenter.IllNessData da)
    {
        PlayerDataCenter.IllNessData _d = da;
        GameObject go = null;
        if (!allhereilldatas.ContainsKey(_d.ID))
        {
            go = GameObject.Instantiate(illnessprefabe);
            allhereilldatas.Add(_d.ID, go);
        }
        go = allhereilldatas[_d.ID];
        go.GetComponent<IllnessButton>().Init(illnessprefabe, _d);

    }

    public void CallbackRefresh(List<PlayerDataCenter.IllNessData> illarray)
    {
        if (null == illarray)
        {
            return;
        }
        //根据id从大到小,冒泡排序
        int length = illarray.Count;
        PlayerDataCenter.IllNessData temp = null;
        for (int i = length; i > 0 ; i--)
        {
            for (int j = 0; j < i - 1; j++)
            {
                if (illarray[j].ID < illarray[j + 1].ID)
                {
                    temp = illarray[j];
                    illarray[j] = illarray[j + 1];
                    illarray[j + 1] = temp;
                }
            }
        }


        for (int i = 0; i < illarray.Count; i++)
        {
            SpawnIllData(illarray[i]);
        }

    }

    public override void Refresh()
    {
        //Get请求服务器  获取数据刷新
        string url = Tool.illnessdatasimplepath;
        MyWebRequest.Instance.Get(url, (success, str) =>
        {
            if (success)
            {
                //json解析
                IllDatalistRoot cd = JsonHelper.ParseJsonToNeed<IllDatalistRoot>(str);
                //根据还原的数据进行本地还原,刷新到UI上
                PlayerDataCenter.RevertToNormal(cd);
                
              //  ClosePage<UINotice>();
            }
            else
            {
                ShowPage<UINotice>(str);
            }
        });

    }

}
