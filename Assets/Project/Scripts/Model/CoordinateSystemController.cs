using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinateSystemController : MonoBehaviour {

    Vector3 originpos;

    public void Init(Transform targettransform)
    {
        MSGCenter.Register(Enums.MainProcess.MainProcess_RePlay.ToString(), Replay);
        MSGCenter.Register(Enums.ViewMode.ChangeTargetPos.ToString(), ChangeTargetPosCall);
        transform.position = targettransform.position;
        transform.forward = targettransform.forward;
        originpos = transform.position;
    }


    private void ChangeTargetPosCall(string message)
    {
        Vector3 v = MSGCenter.ParseToVector(message);
        if (v == Vector3.zero)
        {
            v = originpos;
        }
        transform.position = v;
    }
    void Replay(string message)
    {
        transform.transform.localPosition = originpos;
    }

}
