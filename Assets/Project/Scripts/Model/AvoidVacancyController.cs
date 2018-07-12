using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidVacancyController : MonoBehaviour {



    private int id;


    public void Init(int id)
    {
        this.id = id;

    }



    public bool Isme(int id)
    {
        return this.id == id;
    }



    public void Change(Vector3 pos,Vector3 scaler)
    {
        transform.position = pos;
        transform.localScale = scaler;
    }

}
