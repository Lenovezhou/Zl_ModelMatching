using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyToggle : Toggle {

    Image[] childimages;
    protected override void Start()
    {
        base.Start();
        childimages = transform.GetComponentsInChildren<Image>();

        onValueChanged.AddListener(MyToggleExtention);
        childimages[1].gameObject.SetActive(false);

    }

    void MyToggleExtention(bool iSon)
    {
        childimages[0].gameObject.SetActive(iSon);
        childimages[1].gameObject.SetActive(!iSon);
    }
}
