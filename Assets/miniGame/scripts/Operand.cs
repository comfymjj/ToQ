using UnityEngine;
using System.Collections;

public class Operand : Symbol {
	float mouseX;
	float mouseY;
	bool beingDragged;

	void Awake() {
		base.Awake();
		padding = 0;
	}

	// Update is called once per frame
	void Update(){
		mouseX = Input.mousePosition.x;
		mouseY = Input.mousePosition.y;
	}
	void OnMouseDrag() {
		beingDragged = true;
		transform.position = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(mouseX,mouseY,9f)).x, transform.position.y);
	}
	void OnMouseUp() {
		if (beingDragged) {
			//transform.position = new Vector3(transform.position.x, transform.position.y, 0);
		}
		beingDragged = false;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (beingDragged) {
			//Destroy (gameObject);
		}
	}

}