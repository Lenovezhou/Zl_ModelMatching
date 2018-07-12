using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOpenPanel : MonoBehaviour {


    private Dictionary<Button, GameObject> map = new Dictionary<Button, GameObject>();
    protected Dictionary<Button, List<GameObject>> listmap = new Dictionary<Button, List<GameObject>>();
    protected Button lastkey;

    protected List<GameObject> spawnall = new List<GameObject>();

    protected void RegestToMap(Button b,GameObject go)
    {
        map.Add(b,go);
    }

    /// <summary>
    /// 生成预设物体
    /// </summary>
    /// <param name="prefab">预设物体</param>
    /// <returns></returns>
    protected GameObject SpawnChildren(GameObject prefab)
    {
        GameObject go = Instantiate(prefab);
        go.SetActive(true);
        go.transform.SetParent(prefab.transform.parent);
        spawnall.Add(go);
        return go;
    }



    /// <summary>
    /// 根据点击的button选择对应的panel
    /// </summary>
    /// <param name="bu">点击button</param>
    protected virtual void ChoisePanel(Button bu)
    {
        if (null != lastkey)
        {
            if (map.ContainsKey(lastkey))
            {
                lastkey.GetComponent<Image>().color = Color.white;
                IPanelItem ipi = map[lastkey].gameObject.GetComponent<IPanelItem>();
                if (null != ipi)
                {
                    ipi.OnLeveThisPage();
                }
                map[lastkey].gameObject.SetActive(false);
            }
            if (listmap.ContainsKey(lastkey))
            {
                for (int i = 0; i < listmap[lastkey].Count; i++)
                {
                    listmap[lastkey][i].SetActive(false);
                }
            }
        }
        if (map.ContainsKey(bu))
        {
            bu.GetComponent<Image>().color = Color.green;
            map[bu].gameObject.SetActive(true);
            IPanelItem ipi = map[bu].gameObject.GetComponent<IPanelItem>();
            if (null != ipi)
            {
                ipi.OnEnterThisPage();
            }

        }
        if (listmap.ContainsKey(bu))
        {
            for (int i = 0; i < listmap[bu].Count; i++)
            {
                listmap[bu][i].SetActive(true);
            }
        }
        lastkey = bu;
        ChoiseEnd(bu);
    }

    protected virtual void ChoiseEnd(Button bu) { }
}
