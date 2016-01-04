using UnityEngine;
using System.Collections;
using System;

public class Group : Nomial { //every group contains an operator followed by and operand
	[HideInInspector]
	public Equation equation; //the equation this group belongs to
	[HideInInspector]
	public Operator operation; //the operator contained by this group
	[HideInInspector]
	public Operand operand; //the operand contained by this group
	[HideInInspector]
	public Group prev; //group before this one
	[HideInInspector]
	public Group next; //group after this one

	public override float x {
		get { return transform.position.x; }
		set { transform.position = new Vector3(value, transform.position.y, transform.position.z); }
	}

	public override float width {
		get {
			Debug.Log("operation.width: "+operation.width+", padding: "+operation.padding+", operand.width: "+operand.width);
			return operation.width + operation.padding*2 + operand.width; }
	}

	public override float midPoint { //for easy access
		get { return transform.position.x + this.width/2f; }
	}

	//called before first frame of game loop
	void Awake() {
		operation = transform.GetChild(0).GetComponent<Operator>();
		operation.group = this;
		operand = transform.GetChild(1).GetComponent<Operand>();
		operand.group = this;
	}

	public void align() { //position this group to the right of prev
		Debug.Log("BEFORE: this.x: "+this.x+", operation.x: "+operation.x+", operand.x: "+operand.x+", padding: "+operation.padding);
		this.x = prev.x + prev.width; //position this group to the right of the preceding group
		operation.x = this.x + operation.padding; //position so that this.x is to the right of operation.x
		operand.x = operation.x + operation.width + operation.padding; //position so that operand.x is to the right of operation.x
		Debug.Log("AFTER: this.x: "+this.x+", operation.x: "+operation.x+", operand.x: "+operand.x+", padding: "+operation.padding);
	}

	public void setRight(Group group) { //put 'group' to the right of this group
		this.next = group;
		if (group != null) {
			group.prev = this;
			if (group == equation.head) {
				equation.head = this; //this precedes the head, therefore it must be the new head
			}
		}
	}
	public void setLeft(Group group) { //put 'group' to the left of this group
		this.prev = group;
		if (group != null) {
			group.next = this;
		}
	}
	public void insertAfter(Group group) { //insert this group in between two others
		this.next = group.next; //attach preceding group
		this.prev = group;
		if (group.next != null) { //attach following group
			group.next.prev = this;
			group.next = this;
		}

	}
	public void remove() {
		if (this.prev != null) { //connect surrounding groups
			this.prev.next = this.next;
		}
		if (this.next != null) {
			this.next.prev = this.prev;
		}
		if (this == equation.head) { //we have removed the head, therefor this.next is the new head
			equation.head = this.next;
		}
		this.next = null; //reset this group's connections
		this.prev = null;
	}

	public void onDragOperand() { //called by operand when onMouseDrag() is triggered
		this.x = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, 0)).x - (operand.midPoint - this.x); //center this object at the mouse's x coordinate
		if (this.operand.midPoint < equation.head.midPoint) { //if this symbol is positioned before the first element
			this.remove(); //remove from current spot
			this.setRight(equation.head); //make this the new head
		} else {
			foreach (Group group in equation.getChildren()) {
				if (group.next == this) { //if this symbol is where it should be
					continue;
				} else if (this.operand.midPoint > group.midPoint && (group.next == null || this.operand.midPoint < group.next.midPoint)) { //if this.x is in between two symbols, or after the last symbol
					this.remove(); //remove from current spot
					this.insertAfter(group);
					equation.align();
					break;
				}
			}
		}
	}
}