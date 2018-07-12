using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;



public class CoordinateSystemItem : MonoBehaviour,IPointerDownHandler
{
	Action<CoordinateSystemItem> call;
	public void Init(Action<CoordinateSystemItem> call)
	{

		this.call = call;
	}

	public void OnPointerDown(PointerEventData eventdata)
	{
		call (this);
	}
}
