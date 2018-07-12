using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThiredPageModeldisplayController : MonoBehaviour {


    Button resetactive;

    Toggle Patient, Normalpice, Unnormal, Preservemodel;

    Transform MainUIpanel;

    void Start () {

       
        resetactive = transform.Find("MainUIPanel/ResetActiveButton").GetComponent<Button>();


        resetactive.onClick.AddListener(() =>
        {
            Patient.isOn = true;
            Normalpice.isOn = true;
            Unnormal.isOn = false;
            Preservemodel.isOn = false;
        });

        Patient = transform.Find("MainUIPanel/PatientToggle").GetComponent<Toggle>();
        Normalpice = transform.Find("MainUIPanel/NormalpiceToggle").GetComponent<Toggle>();
        Unnormal = transform.Find("MainUIPanel/UnNoramlToggle").GetComponent<Toggle>();
        Preservemodel = transform.Find("MainUIPanel/PreserveModelToggle").GetComponent<Toggle>();
        Patient.onValueChanged.AddListener((ison) => { OnActiveModelChange(Enums.ToggleToModel.Patient.ToString(), ison); });
        Normalpice.onValueChanged.AddListener((ison) => { OnActiveModelChange(Enums.ToggleToModel.Normalpice.ToString(), ison); });
        Unnormal.onValueChanged.AddListener((ison) => { OnActiveModelChange(Enums.ToggleToModel.Unnormal.ToString(), ison); });
        Preservemodel.onValueChanged.AddListener((ison) => { OnActiveModelChange(Enums.ToggleToModel.Preservemodel.ToString(), ison); });


        Button mainUIbutton = transform.Find("Button").GetComponent<Button>();
        MainUIpanel = transform.Find("MainUIPanel");
        mainUIbutton.onClick.AddListener(() =>
        {
            MainUIpanel.gameObject.SetActive(true);

            //修改toggle显示
            //mainUIToggle.image.sprite = iSon ? onStateNormalSprite : offStateNormalSprite;
        });


        MouseButtonController.Instance.mousebuttondownhit += go =>
        {

            if (go != mainUIbutton)
            {
                for (int i = 0; i < MainUIpanel.childCount; i++)
                {
                    if (go != null && go.name == MainUIpanel.GetChild(i).name)
                    {
                        return;
                    }
                }
            }

            MainUIpanel.gameObject.SetActive(false);
        };

    }



    void OnActiveModelChange(string name, bool iSon)
    {
        MSGCenter.Execute(name, iSon.ToString());
    }


}
