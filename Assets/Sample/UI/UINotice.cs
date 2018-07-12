using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UINotice : TTUIPage
{
    public UINotice() : base(UIType.PopUp, UIMode.DoNothing, UICollider.Normal)
    {
        uiPath = "UIPrefab/Notice";
    }

    Text noticetext;
    Button confimbut;

    public override void Awake(GameObject go)
    {
        confimbut = this.gameObject.transform.Find("content/btn_confim").GetComponent<Button>();
        confimbut.onClick.AddListener(() =>
        {
            Hide();
        });

        noticetext = this.transform.Find("content/Text").GetComponent<Text>();
    }

    public override void Refresh()
    {
        string message = data as string;
        if (string.IsNullOrEmpty(message))
        {
            return;
        }
        Dictionary<int,string> map = MSGCenter.UnFormatMessage(message);
        int duration = 2;
        if (map.ContainsKey(0))
        {
            noticetext.text = map[0];
        }
        if (map.ContainsKey(1))
        {
            duration = int.Parse(map[1]);
        }
        TimerManager.Instance.Create(Hide, null, 0, duration);
    }

   

}
