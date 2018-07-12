using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetNoticeTools : ButtonOpenPanel {

    void Start ()
    {
       Button Rotate = transform.Find("RotateButton").GetComponent<Button>();
       Button translate = transform.Find("TranslateButton").GetComponent<Button>();
       Button scaler = transform.Find("ScalerButton").GetComponent<Button>();

        Rotate.onClick.AddListener(() => 
        {
            ChoisePanel(Rotate);
            MSGCenter.Execute(Enums.LeftMouseAlive.TurnOn.ToString());
            MSGCenter.Execute(Enums.LeftMouseButtonControl.Roate.ToString());
        });
        translate.onClick.AddListener(() =>
        {
            ChoisePanel(translate);
            MSGCenter.Execute(Enums.LeftMouseAlive.TurnOn.ToString());
            MSGCenter.Execute(Enums.LeftMouseButtonControl.Move.ToString());
        });
        scaler.onClick.AddListener(() => 
        {
            MSGCenter.Execute(Enums.LeftMouseAlive.TurnOff.ToString());
            ChoisePanel(scaler);
        });

        Transform RotatePanel = transform.Find("RotatePanel");
        Transform TranslatePanel = transform.Find("TranslatePanel");
        Transform ScalerPanel = transform.Find("ScalerPanel");


        RegestToMap(Rotate,RotatePanel.gameObject);
        RegestToMap(translate, TranslatePanel.gameObject);
        RegestToMap(scaler, ScalerPanel.gameObject);

        Slider S_rotate = RotatePanel.GetComponentInChildren<Slider>();
        InputField I_rotate = RotatePanel.GetComponentInChildren<InputField>();
        Slider S_XYZscaler = ScalerPanel.transform.Find("XYZPanel").GetComponentInChildren<Slider>();
        InputField I_XYZscaler = ScalerPanel.transform.Find("XYZPanel").GetComponentInChildren<InputField>();

        Slider S_Xscaler = ScalerPanel.transform.Find("XPanel").GetComponentInChildren<Slider>();
        InputField I_Xscaler = ScalerPanel.transform.Find("XPanel").GetComponentInChildren<InputField>();

        Slider S_Yscaler = ScalerPanel.transform.Find("YPanel").GetComponentInChildren<Slider>();
        InputField I_Yscaler = ScalerPanel.transform.Find("YPanel").GetComponentInChildren<InputField>();

        Slider S_Zscaler = ScalerPanel.transform.Find("ZPanel").GetComponentInChildren<Slider>();
        InputField I_Zscaler = ScalerPanel.transform.Find("ZPanel").GetComponentInChildren<InputField>();


        Button B_up = TranslatePanel.Find("UpButton").GetComponent<Button>();
        Button B_down = TranslatePanel.Find("DownButton").GetComponent<Button>();
        Button B_mid = TranslatePanel.Find("MidButton").GetComponent<Button>();
        Button B_right = TranslatePanel.Find("RightButton").GetComponent<Button>();
        Button B_left = TranslatePanel.Find("LeftButton").GetComponent<Button>();

        S_rotate.onValueChanged.AddListener((endvalue)=> 
        {
            OnRotateSliderChange(endvalue);
            I_rotate.text = endvalue.ToString();
        });
        I_rotate.onValueChanged.AddListener((str)=> 
        {
            try
            {
                float a = float.Parse(str);
                S_rotate.value =a;
            }
            catch (System.Exception)
            {
                I_rotate.text = S_rotate.value.ToString();
                throw;
            }
        });


        S_XYZscaler.onValueChanged.AddListener((endvalue) =>
        {
            OnScalerSliderChange(Enums.ControllTransform.Scaler, endvalue);
            I_XYZscaler.text = endvalue.ToString();
        });
        I_XYZscaler.onValueChanged.AddListener((str) =>
        {
            try
            {
                float a = float.Parse(str);
                S_XYZscaler.value = a;
            }
            catch (System.Exception)
            {
                I_XYZscaler.text = S_XYZscaler.value.ToString();
                throw;
            }
        });

        S_Xscaler.onValueChanged.AddListener((endvalue)=> 
        {
            OnScalerSliderChange(Enums.ControllTransform.ScalerX,endvalue);
            I_Xscaler.text = endvalue.ToString();
        });
        I_Xscaler.onValueChanged.AddListener((str) => 
        {
            try
            {
                float a = float.Parse(str);
                S_Xscaler.value = a;
            }
            catch (System.Exception)
            {
                I_Xscaler.text = S_Xscaler.value.ToString();
                throw;
            }
        });

        S_Yscaler.onValueChanged.AddListener((endvalue) =>
        {
            OnScalerSliderChange(Enums.ControllTransform.ScalerY,endvalue);
            I_Yscaler.text = endvalue.ToString();
        });
        I_Yscaler.onValueChanged.AddListener((str) =>
        {
            try
            {
                float a = float.Parse(str);
                S_Yscaler.value = a;
            }
            catch (System.Exception)
            {
                I_Yscaler.text = S_Yscaler.value.ToString();
                throw;
            }
        });

        S_Zscaler.onValueChanged.AddListener((endvalue) =>
        {
            OnScalerSliderChange(Enums.ControllTransform.ScalerZ, endvalue);
            I_Zscaler.text = endvalue.ToString();
        });
        I_Zscaler.onValueChanged.AddListener((str) =>
        {
            try
            {
                float a = float.Parse(str);
                S_Zscaler.value = a;
            }
            catch (System.Exception)
            {
                I_Zscaler.text = S_Zscaler.value.ToString();
                throw;
            }
        });


        B_up.onClick.AddListener(() => { OnTranslateChange(Enums.ViewMode.Up); });
        B_down.onClick.AddListener(() => { OnTranslateChange(Enums.ViewMode.Down); });
        B_mid.onClick.AddListener(() => { OnTranslateChange(Enums.ViewMode.Forword); });
        B_right.onClick.AddListener(() => { OnTranslateChange(Enums.ViewMode.Right); });
        B_left.onClick.AddListener(() => { OnTranslateChange(Enums.ViewMode.Left); });


        Rotate.onClick.Invoke();
    }

    ///缩放
    void OnRotateSliderChange(float endvalue)
    {
        MSGCenter.Execute(Enums.ControllTransform.Rotate.ToString(), endvalue.ToString());
    }

    ///XYZ及等比缩放
    void OnScalerSliderChange(Enums.ControllTransform ct,float endvalue)
    {
        MSGCenter.Execute(ct.ToString(), endvalue.ToString());
    }

    /// 平移
    void OnTranslateChange(Enums.ViewMode vm)
    {
        MSGCenter.Execute(Enums.ControllTransform.Translate.ToString(), vm.ToString());
    }
}
