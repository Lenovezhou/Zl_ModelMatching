using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ResetRotationController:MonoBehaviour
{
    //手动赋值，"Project/Render"
    [SerializeField]
    private RenderTexture rendertexture;
    public void Init()
    {
        MSGCenter.Execute(Enums.CameraMode.NormalCam.ToString());
        GetComponentInChildren<RawImage>().texture = rendertexture;
    }

}
