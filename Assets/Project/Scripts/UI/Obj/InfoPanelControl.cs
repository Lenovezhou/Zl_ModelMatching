using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelControl : MonoBehaviour {

    public Text title;
    public Text position;
    public Text protector_shape;
    public Text injury_position;
    public Text description;
    public Text note;


    private void Init()
    {
        title = transform.Find("titleText").GetComponent<Text>();
        position = transform.Find("positionText").GetComponent<Text>();
        protector_shape = transform.Find("protector_shapeText").GetComponent<Text>();
        injury_position = transform.Find("injury_positionText").GetComponent<Text>();
        description = transform.Find("descriptionText").GetComponent<Text>();
        note = transform.Find("noteText").GetComponent<Text>();

        RefreshInfo(PlayerDataCenter.Currentillnessdata);
    }



    public void RefreshInfo(PlayerDataCenter.IllNessData data)
    {
        if (!title)
        {
            Init();
        }
        //名字
        title.text = data.title;
        //受伤部位
        injury_position.text = data.injury_position;
        //方向
        int direction = (int)data.position;
        position.text = Tool.Illposition[direction];
        //护具外形
        int shape = (int)data.protector_shape;
        protector_shape.text = Tool.protector_shape[shape];
        //病情简述
        description.text = data.description;
        //备注
        note.text = data.note;
    }

}
