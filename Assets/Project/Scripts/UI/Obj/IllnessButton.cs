using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IllnessButton : MonoBehaviour {

    [SerializeField]
   private PlayerDataCenter.IllNessData selfillnessdata;
    public void Init(GameObject illnessprefabe, PlayerDataCenter.IllNessData data)
    {
        selfillnessdata = data;


        transform.SetParent(illnessprefabe.transform.parent);
        transform.SetAsLastSibling();
        gameObject.SetActive(true);
        transform.localScale = Vector3.one;

        transform.Find("Text (1)").GetComponent<Text>().text = selfillnessdata.note;
        transform.Find("Text (2)").GetComponent<Text>().text = selfillnessdata.illcreatetime;
        AssigneEvents(selfillnessdata);
    }


    void AssigneEvents(PlayerDataCenter.IllNessData _data)
    {
        Text titlename = transform.Find("Text").GetComponent<Text>();
        titlename.text = _data.title;

        //点击显示/隐藏
        transform.Find("Toggle").GetComponent<Toggle>().onValueChanged.AddListener(iSon =>
        {
            titlename.text = iSon ? _data.title : "***";
        });
        //点击编辑

        transform.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
        transform.Find("Button").GetComponent<Button>().onClick.AddListener(() =>
        {
            TTUIPage.ShowPage<CreateIllnessPopup>(_data);
        });
        //点击病例
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() =>
        {
            PlayerDataCenter.NewSceneCame(_data);

            TTUIPage.ShowPage<UIThirdPage>(_data);
        });

    }

}
