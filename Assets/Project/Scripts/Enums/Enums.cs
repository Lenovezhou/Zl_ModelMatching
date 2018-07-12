using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums 
{

    static public T ParseEnums<T>(string type)
    {
        return (T)Enum.Parse(typeof(T), type);
    }

	//是否允许鼠标左键控制模型
	public enum LeftMouseAlive{TurnOn,TurnOff}

	//左键旋转/移动
	public enum LeftMouseButtonControl{Roate,Move,AddMatchingGizmo, AddAvoidGizmo}

	//"正视图","左视图","底视图","右视图","后视图","顶视图"
	public enum ViewMode{Forword,Left,Down,Right,Back,Up,ResetView,ChangeTargetPos}

	//点的类型
	public enum PointMode{Normal,UserImport}
    
    //显示当前/全部
    public enum PointControll { Choise, All,ShutDownAll }

    //Camera类型
    public enum CameraMode { NormalCam,SmallerCam}

    //Camera有没有TargetRenderTexture
    public enum CameraTarget { WithTarget,WithoutTarget}

    //UI点选文件路径后发送给modelmanager加载本地
    public enum ModelPath { ModelPath,Result}

    //视图控制窗口的toggle
    public enum ToggleToModel { Patient, Normalpice, Unnormal, Preservemodel }

    //UI调整位移，旋转
    public enum ControllTransform { Translate,Rotate,Scaler,ScalerX,ScalerY,ScalerZ}

    //点的操作
    public enum MatchigPointGizmoControll { Highlight, PointerDown, SaveMatchingpoint, Cancle, AutoNext ,AutoNextToggle,LoadUserData}

    //避空位操作
    public enum AvoidvacancyControll { Edit,Remove,Inactive,InactiveAll,Active,ActiveAll,Add,SaveAvoid,Reset}

    //取消当前操作重新开始选择病例
    public enum MainProcess { MainProcess_RePlay,MainProcess_SaveALL,MainProcess_loadedUserPointDone,MainProcess_LoadUserDataDone}


    //webrequest类型
    public enum WebRequestType { Post,Put,Get}


    //使用材质
    public enum MatarialsUse { NormalPoint_hight,NormalPoint_origin,UserImportpoint_hight,UserImportpoint_origin,UserimportModel,NormalModel,Indicator_Effect,Indicator_origin}

}
