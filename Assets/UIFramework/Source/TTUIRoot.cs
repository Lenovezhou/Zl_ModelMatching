
    using UnityEngine;
    using System.Collections;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
using System;

/// <summary>
/// Init The UI Root
/// 
/// UIRoot
/// -Canvas
/// --FixedRoot
/// --NormalRoot
/// --PopupRoot
/// -Camera
/// </summary>
public class TTUIRoot : MonoBehaviour
{
    private static TTUIRoot m_Instance = null;
    public static TTUIRoot Instance
    {
        get
        {
            if(m_Instance == null)
            {
                InitRoot();/*生成新的m_Instance*/
            }
            return m_Instance;
        }
    }

    public Transform root;
    public Transform fixedRoot;
    public Transform normalRoot;
    public Transform popupRoot;
    public Camera uiCamera;

    static void InitRoot()
    {
        GameObject go = new GameObject("UIRoot");
        go.layer = LayerMask.NameToLayer("UI");
        m_Instance = go.AddComponent<TTUIRoot>();
        go.AddComponent<RectTransform>();
        m_Instance.root = go.transform;

        Canvas can = go.AddComponent<Canvas>();
        can.renderMode = RenderMode.ScreenSpaceOverlay;
        can.pixelPerfect = true;

		GameObject cameraobj = Resources.Load ("Model/Main Camera")as GameObject;
		GameObject camObj = Instantiate (cameraobj);
		Camera cam = cameraobj.GetComponent<Camera> ();
        CameraRotateSphere crs = camObj.GetComponent<CameraRotateSphere>();
        CreatModelManger(_trans=> { crs.Init(_trans); });

    CanvasScaler cs = go.AddComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        cs.referenceResolution = new Vector2(1136f, 640f);
        cs.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

        GameObject subRoot = CreateSubCanvasForRoot(go.transform,250);
        subRoot.name = "FixedRoot";
        m_Instance.fixedRoot = subRoot.transform;

        subRoot = CreateSubCanvasForRoot(go.transform,0);
        subRoot.name = "NormalRoot";
        m_Instance.normalRoot = subRoot.transform;

        subRoot = CreateSubCanvasForRoot(go.transform,500);
        subRoot.name = "PopupRoot";
        m_Instance.popupRoot = subRoot.transform;

        //add Event System
        GameObject esObj = GameObject.Find("EventSystem");
        if(esObj != null)
        {
            GameObject.DestroyImmediate(esObj);
        }

        GameObject eventObj = new GameObject("EventSystem");
        eventObj.layer = LayerMask.NameToLayer("UI");
        eventObj.transform.SetParent(go.transform);
        eventObj.AddComponent<EventSystem>();
        if (!Application.isMobilePlatform || Application.isEditor)
        {
            eventObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        else
        {
            eventObj.AddComponent<UnityEngine.EventSystems.TouchInputModule>();
        }

            
    }

    static GameObject CreateSubCanvasForRoot(Transform root,int sort)
    {
        GameObject go = new GameObject("canvas");
        go.transform.parent = root;
        go.layer = LayerMask.NameToLayer("UI");

        Canvas can = go.AddComponent<Canvas>();
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);/*相对父节点左端对齐，相对距离，宽度*/
        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);/*相对父节点上端对齐，相对距离，高度*/
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.transform.localScale = Vector3.one;

        can.overrideSorting = true;
        can.sortingOrder = sort;
        go.AddComponent<GraphicRaycaster>();

        return go;
    }


   static void CreatModelManger(Action<Transform> callbak)
    {
        GameObject modelmanager = new GameObject("ModelManager");
        ModelManager mg = modelmanager.AddComponent<ModelManager>();
        mg.Init(callbak);
    }

    void OnDestroy()
        {
            m_Instance = null;
        }
    
}