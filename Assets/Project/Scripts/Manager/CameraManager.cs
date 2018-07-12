using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    //手动赋值，"Project/RenderTexture"
    [SerializeField]
    private RenderTexture rendertexture;

    private Camera selfcam;
	void Start () 
	{
        MSGCenter.Register(Enums.CameraMode.NormalCam.ToString(), RefreshCameraRect);
        MSGCenter.Register(Enums.CameraMode.SmallerCam.ToString(), RefreshCameraRect);
        MSGCenter.Register(Enums.CameraTarget.WithTarget.ToString(), RefreshCameraTarget);
        MSGCenter.Register(Enums.CameraTarget.WithoutTarget.ToString(), RefreshCameraTarget);

        selfcam = GetComponentInChildren<Camera>();
        Transform light = GameObject.FindObjectOfType<Light> ().transform;
		light.SetParent (transform);
    }

   

    public void RefreshCameraRect(string message)
    {
        Enums.CameraMode cameramode = (Enums.CameraMode)Enum.Parse(typeof(Enums.CameraMode), message);
        Rect r = new Rect(0,0,0,0);
        switch (cameramode)
        {
            case Enums.CameraMode.NormalCam:
                r = Tool.ThiredpanelNormalcam;
                break;
            case Enums.CameraMode.SmallerCam:
                r = Tool.ThirdpanelSmallercam;
                selfcam.targetTexture = null;
                break;
            default:
                break;
        }
        selfcam.rect = r;
    }

    public void RefreshCameraTarget(string message)
    {
        Enums.CameraTarget cameramode = (Enums.CameraTarget)Enum.Parse(typeof(Enums.CameraTarget), message);
        switch (cameramode)
        {
            case Enums.CameraTarget.WithTarget:
                selfcam.targetTexture = rendertexture;
                break;
            case Enums.CameraTarget.WithoutTarget:
                selfcam.targetTexture = null;
                break;
            default:
                break;
        }
    }

}
