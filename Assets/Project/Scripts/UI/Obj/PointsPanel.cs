using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsPanel : MonoBehaviour {

	public GameObject selfitem;
	public GameObject ScrollView;

	void Start()
	{
		ScrollView = transform.Find ("Scroll View").gameObject;
		selfitem = ScrollView.transform.Find ("Viewport/Content/Item").gameObject;

	}

	public void Refresh(Dictionary<int,Vector3> points)
	{
		if (points == null || points.Count == 0) 
		{
			Debug.Log ("检查传入点的集合数据");
			ScrollView.SetActive (false);
			return;
		}

        foreach (KeyValuePair<int,Vector3> item in points)
        {
            ScrollView.SetActive(true);
            GameObject go = Instantiate(selfitem);
            go.transform.SetParent(selfitem.transform.parent);
            go.transform.Find("Text").GetComponent<Text>().text = (item.Key + 1).ToString();
            go.SetActive(true);
        }

	}
}
