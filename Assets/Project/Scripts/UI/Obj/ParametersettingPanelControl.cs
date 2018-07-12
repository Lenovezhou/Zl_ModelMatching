using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParametersettingPanelControl : ButtonOpenPanel {

    //花纹UI预设
    GameObject DecorativepatternItem;
    Button DecorativepatternButton, OtherButton;
    void Start ()
    {
        DecorativepatternButton = transform.Find("DecorativepatternButton").GetComponent<Button>();
        OtherButton = transform.Find("OtherButton").GetComponent<Button>();
        DecorativepatternButton.onClick.AddListener(() => { ChoisePanel(DecorativepatternButton); });
        OtherButton.onClick.AddListener(() => { ChoisePanel(OtherButton); });
        AsigneDropdown(transform.Find("OtherPanel/RollingstripDropdown").GetComponent<Dropdown>());
        RegestToMap(DecorativepatternButton, transform.Find("Scroll View").gameObject);
        RegestToMap(OtherButton, transform.Find("OtherPanel").gameObject);


        DecorativepatternButton.onClick.Invoke();
        DecorativepatternItem = transform.Find("Scroll View/Viewport/Content/DecorativepatternItem").gameObject;
        ChoisePanel(DecorativepatternButton);

        AssigneDecorativepatterns();


    }

    void AsigneDropdown(Dropdown dd)
    {
        List<string> ddname = new List<string>() { "两条","三条"};
        dd.InitDropDown(ddname);
        dd.onValueChanged.AddListener(DropdownCallback);
    }

    void DropdownCallback(int index)
    {

    }

    void AssigneDecorativepatterns()
    {
        Texture[] Decorativepatterns = Resources.LoadAll<Texture>(Tool.LocalDecorativepatternPath);

        for (int i = 0; i < Decorativepatterns.Length; i++)
        {
            GameObject go = SpawnChildren(DecorativepatternItem);
            go.GetComponentInChildren<RawImage>().texture = Decorativepatterns[i];
            go.GetComponentInChildren<Text>().text = Decorativepatterns[i].name;
        }
    }
}
