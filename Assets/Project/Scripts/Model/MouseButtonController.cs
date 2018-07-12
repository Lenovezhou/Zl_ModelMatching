using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseButtonController : Singleton<MouseButtonController>
{
    public Action<GameObject> mousebuttondownhit;

	void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject hitgo = null;
            hitgo = EventSystem.current.currentSelectedGameObject;
            if (null != mousebuttondownhit)
            {
                mousebuttondownhit(hitgo);
            }
        }
	}
}
