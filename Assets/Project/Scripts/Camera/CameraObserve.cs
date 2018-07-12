using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraObserve : MonoBehaviour {
	private Transform hitchild;
	private Quaternion finalquaternion = Quaternion.identity;
	private bool isquaternionlerp = false;
	public Camera cam;
	public float speed = 10;



	void Start()
	{

		for (int i = 0; i < transform.childCount; i++)
		{
			CoordinateSystemItem cs = transform.GetChild (i).gameObject.AddComponent<CoordinateSystemItem> ();
			cs.Init (Callback);
		}
	}


	void Callback(CoordinateSystemItem cooritem)
	{
		hitchild = cooritem.transform;
	}

	void Update() 
	{
//		Vector3 fwd = cam.transform.forward; fwd.Normalize();
//		if (Input.GetMouseButton(0)) 
//		{ 
//			Vector3 vaxis = Vector3.Cross(fwd, Vector3.right); 
//			transform.Rotate(vaxis, -Input.GetAxis("Mouse X") * speed, Space.World);
//			Vector3 haxis = Vector3.Cross(fwd, Vector3.up);
//			transform.Rotate(haxis, -Input.GetAxis("Mouse Y") * speed, Space.World); 
//		} 

		if (Input.GetMouseButtonDown(0)) 
		{
			GetTheCklickPoint ();
			isquaternionlerp = true;
		}

		if (isquaternionlerp && transform.rotation != finalquaternion) 
		{
			transform.rotation = Quaternion.Lerp (transform.rotation, finalquaternion, 0.25f);			
		}
		else
		{
			isquaternionlerp = false;
		}


	} 

	void GetTheCklickPoint()
	{

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hitinfo;
		if (Physics.Raycast(ray,out hitinfo))
		{
			hitchild = hitinfo.transform;
			finalquaternion = CalculateRotation (hitinfo.transform.position);
		}
	}

	Quaternion CalculateRotation(Vector3 hitpoint)
	{
		Vector3 v1 = hitpoint - transform.position;
		Vector3 v2 = Camera.main.transform.position - transform.position;
		Vector3 cross = Vector3.Cross (v1, v2);

		float angle = Vector3.Angle (v1, v2);

		Quaternion qua = Quaternion.AngleAxis (angle, cross);

		return qua * transform.rotation ;
	}

}
