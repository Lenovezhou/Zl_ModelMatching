using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Xml;
using System.IO;
using System.Security.AccessControl;


/*  声明一个新的类（需继承TTUIPage<using TinyTeam.UI;>）调用相同的方法，
 *  在该类的构造方法为uipath赋值，填好枚举值（决定父节点），
 *  重写Awake()方法，为子节点添加事件
 */
public class GameMain : ApplicationBase<GameMain> {
	// Use this for initialization
	void Start () 
	{
        int width = (int)Tool.ScreenSize.x;
        int height = (int)Tool.ScreenSize.y;
        Screen.SetResolution(width, height, false);

        //事件注册时机
        //TTUIPage.ShowPage<UIThirdPage>();
        TTUIPage.ShowPage<UIFirstPage> ();
    }
   
}
