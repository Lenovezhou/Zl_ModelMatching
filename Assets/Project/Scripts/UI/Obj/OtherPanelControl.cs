
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherPanelControl : ButtonOpenPanel {

    void Start()
    {

        Dropdown offsetdroupdown = transform.Find("OffsetDropdown").GetComponent<Dropdown>();
        offsetdroupdown.InitDropDown(new List<string>() { "0" });

        Dropdown deepthdorpdown = transform.Find("DeepthDropdown").GetComponent<Dropdown>();
        deepthdorpdown.InitDropDown(new List<string>() { "5mm" });
    }
}
