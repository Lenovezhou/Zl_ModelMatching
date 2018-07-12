using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UITopBar : TTUIPage {

    public UITopBar() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None)
    {
        uiPath = "UIPrefab/Topbar";
    }

    public override void Awake(GameObject go)
    {
        this.gameObject.transform.Find("btn_back").GetComponent<Button>().onClick.AddListener(() =>
        {
            TTUIPage.ClosePage();
        });
        this.gameObject.transform.Find("btn_notice").GetComponent<Button>().onClick.AddListener(() =>
        {
            ShowPage<UINotice>();
        });
    }

    //继承后，没有重写虚方法，调用的是父类的方法，如果有重写则是调用自己重写后的方法
    //   public override void Refresh(){ Debug.Log("step,this"); }

}
