using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BandagePanelControl : ButtonOpenPanel {

	void Start () {

        Dropdown bandage = transform.Find("OtherPanel/RollingstripDropdown").GetComponent<Dropdown>();
        bandage.InitDropDown(new List<string>() { "2" });
	}
	
	
}
