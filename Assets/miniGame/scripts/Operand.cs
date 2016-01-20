using UnityEngine;
using System.Collections;

public class Operand : Symbol {
	bool beingDragged;
	
	void Awake() {
		base.initialize(); //this calls initialization stuff inside the Symbol class
	}

	void OnMouseDrag() {
		beingDragged = true;
		group.onDragOperand();
	}
	void OnMouseUp() {
		if (beingDragged) {
			group.equation.align(null); //causes everything to snap into place
			//TODO: modify align() to include smooth movement and to avoid setting the x position of the object being dragged
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