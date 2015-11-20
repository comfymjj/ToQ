using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	void OnMouseDrag()
		
	{
		
		Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		
		point.z = gameObject.transform.position.z;
		
		gameObject.transform.position = point;
		
		Cursor.visible = false;
		
		
	}}