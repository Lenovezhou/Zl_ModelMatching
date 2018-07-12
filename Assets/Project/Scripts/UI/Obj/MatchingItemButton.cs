using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchingItemButton : MonoBehaviour {

    Button selfbutton;
    Text selfstatetext;
    [SerializeField]
    int group = -1;
    [SerializeField]
    int index = -1;
    string laststate;
    Color nomatchingcolor = Color.red;
    Color matchingcolor = Color.yellow;
    Color matchingDoneColor = Color.green;
    string nomatchingstr = "未匹配";
    string matchingstr = "编辑中";
    string matchingdonestr = "匹配完成";


    Action<MatchingItemButton> callback;

    bool hassaved = false;

    private void Start()
    {
        if ( null == selfstatetext)
        {
            CheckChildAssigne();
        }
    }
    void CheckChildAssigne ()
    {
        selfbutton = GetComponentInChildren<Button>();
        selfstatetext = transform.Find("StateText").GetComponentInChildren<Text>();
        selfstatetext.text = nomatchingstr;
        selfstatetext.color = nomatchingcolor;
        laststate = selfstatetext.text;
        selfbutton.onClick.AddListener(OnEditHit);
	}


    public void OnEditHit()
    {
        if (null == selfstatetext)
        {
            CheckChildAssigne();
        }
        callback(this);
        DotweenColor(transform.GetComponent<Image>(), new Color(1, 1, 1, 0.4f), Color.red);
        selfstatetext.text = matchingstr;
        selfstatetext.color = matchingcolor;
        selfbutton.GetComponent<Image>().color = Color.black;
        MSGCenter.Execute(Enums.LeftMouseButtonControl.AddMatchingGizmo.ToString(), "True*" + group + "*" + index);
        MSGCenter.Execute(Enums.MatchigPointGizmoControll.Highlight.ToString());
    }


    public void SaveMatchingpoint()
    {
        if (null == selfstatetext)
        {
            CheckChildAssigne();
        }
        hassaved = true;
        DotweenColor(transform.GetComponent<Image>(), new Color(1, 1, 1, 0.4f), Color.red);
        selfstatetext.text = matchingdonestr;
        selfstatetext.color = matchingDoneColor;
        selfbutton.GetComponent<Image>().color = Color.white;
    }

    public void Cancle()
    {
        hassaved = false;
        if (selfstatetext)
        {
            selfstatetext.text = nomatchingstr;
            selfstatetext.color = nomatchingcolor;
            selfbutton.GetComponent<Image>().color = Color.white;
        }
    }

    
    public void CheckMeNeedCancle()
    {
        if (hassaved)
        {
            SaveMatchingpoint();
        }
        else
        {
            Cancle();
        }
    }


    public void Init(int _group,int _index,Action<MatchingItemButton> callback)
    {
        this.group = _group;
        this.index = _index;
        this.callback = callback;
    }


    void DotweenColor(Image _image, Color normal, Color worming)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(_image.DOColor(worming, 0.5f).SetEase(Ease.Linear).SetLoops(2));

        seq.OnComplete(() => { _image.color = normal; });
    }

  
}
