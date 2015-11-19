using UnityEngine;
using System.Collections;

public class DragObject : MonoBehaviour {
	float x;
	float y;
	bool beingDragged;

	void Start() {
	}

	// Update is called once per frame
	void Update(){
		x = Input.mousePosition.x;
		y = Input.mousePosition.y;
	}
	void OnMouseDrag() {
		beingDragged = true;
		transform.position = Camera.main.ScreenToWorldPoint(new Vector3(x,y,10.0f));
	}
	void OnMouseUp() {
		beingDragged = false;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (beingDragged) {
			Destroy (gameObject);
		}
	}

}