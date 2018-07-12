using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThiredPageViewController : MonoBehaviour {
    Button up, left, forward, right, back, down, resetview;
	void Start ()
    {
        up = transform.Find("UpButton").GetComponent<Button>();
        left = transform.Find("LeftButton").GetComponent<Button>();
        forward = transform.Find("ForwardButton").GetComponent<Button>();
        right = transform.Find("RightButton").GetComponent<Button>();
        back = transform.Find("BackButton").GetComponent<Button>();
        down = transform.Find("DownButton").GetComponent<Button>();

        resetview = transform.Find("ResetViewButton").GetComponent<Button>();

        up.onClick.AddListener(()=> { OnViewChange((int)Enums.ViewMode.Up); });
        left.onClick.AddListener(() => { OnViewChange((int)Enums.ViewMode.Left); });
        forward.onClick.AddListener(() => { OnViewChange((int)Enums.ViewMode.Forword); });
        right.onClick.AddListener(() => { OnViewChange((int)Enums.ViewMode.Right); });
        back.onClick.AddListener(() => { OnViewChange((int)Enums.ViewMode.Back); });
        down.onClick.AddListener(() => { OnViewChange((int)Enums.ViewMode.Down); });
        resetview.onClick.AddListener(() => { OnViewChange((int)Enums.ViewMode.ResetView); });


    }
    void OnViewChange(int index)
    {
        Enums.ViewMode messagetype = (Enums.ViewMode)index;
        MSGCenter.Execute(messagetype.ToString());
    }


}
