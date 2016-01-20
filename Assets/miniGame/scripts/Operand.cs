using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public class Operand : Symbol { //every operand contains an operator
	//VARIABLES
	[HideInInspector]
	public bool beingDragged;
	[HideInInspector]
	public Equation equation; //mosty used for its .align() function
	[HideInInspector]
	public LinkedListNode<Operand> node; //the node to which this belongs
	[HideInInspector]
	public Operator operation; //the operator contained by this group
	[HideInInspector]
	public int coefficient;
	[HideInInspector]
	public string variable;

	//GETTERS AND SETTERS
	public float sortX { //left edge of operator
		get { return operation.x; }
		set { transform.position = new Vector3(value + operation.width + operation.padding*2 + this.width/2f, transform.position.y, transform.position.z); }
	}
	public float sortWidth { //width of operand+operator+spacing
		get {
			return operation.width + operation.padding*2 + this.width;
		}
	}

	//FUNCTIONS
	void Awake() { //called before first frame of game loop
		base.initialize(); //this calls initialization stuff inside the Symbol class
		string value = transform.GetChild(0).GetComponent<TextMesh>().text; //text value, e.g. 7a
		Match result = new Regex(@"(\d+)([a-zA-Z]+)").Match(value); //matches a digit followed by a string
		coefficient = Int32.Parse(result.Groups[1].Value); //int part
		variable = result.Groups[2].Value; //letter part
		operation = transform.GetChild(1).GetComponent<Operator>(); //get operator
	}

	public void align(float newX) { //position this group to the right of prev
		if (!beingDragged) { //don't reposition this if the player is dragging it around
			operation.x = this.x - operation.width - operation.padding * 2; //properly space operator to the left of operand
			sortX = newX; //position group to the right of the preceding group
		}
		if (node.Next != null) { //recursive call to the next operand
			node.Next.Value.align(this.x + this.width); //tell next group where to position itself
		}
	}

	void OnMouseDrag() { //when user drags this operand
		beingDragged = true;
		LinkedList<Operand> list = node.List; //for easy access
		this.x = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, 0)).x - (this.midPoint - this.x); //center this object at the mouse's x coordinate
		Debug.Log("head.midpoint: " + list.First.Value.midPoint + ", this.midpoint: " + this.midPoint + ", next.midPoint: " + list.First.Next.Value.midPoint);
		if (this.node == list.First) { //if this object is the head
			if (this.midPoint > node.Next.Value.midPoint) { //and it is to the right of the next operand
				Debug.Log("remove head");
				this.operation.gameObject.SetActive(true); //show its operator
				node.Next.Value.operation.gameObject.SetActive(false); //hide the new head's value
				list.RemoveFirst(); //remove this node
				list.AddAfter(list.First, node); //make it the next node
				equation.align();
			}
		} else { //this.node != list.First
			if (this.midPoint < list.First.Value.midPoint) { //if this is to the right of the first element
				Debug.Log("new head");
				list.First.Value.operation.gameObject.SetActive(true); //show old head's operator
				this.operation.gameObject.SetActive(false); //hide this operator
				list.Remove(node); //remove from current spot
				list.AddFirst(node); //make this the new head
				equation.align();
			} else {
				foreach (Operand operand in list) {
					if (operand.node.Next == this.node || operand == this) { //if this symbol is where it should be
						continue;
					} else if (this.midPoint > operand.midPoint && (operand.node.Next == null || midPoint < operand.node.Next.Value.midPoint)) { //if this.x is in between two symbols, or after the last symbol
						Debug.Log("new spot");
						list.Remove(node); //remove from current spot
						list.AddAfter(operand.node, node);
						equation.align();
						break;
					}
				}
			}
		}
	}
	void OnMouseUp() {
		if (beingDragged) {
			beingDragged = false;
			equation.align(); //causes everything to snap into place
		}
	}

	public void addToPrevious() {
		if (!node.Previous.Value.variable.Equals(this.variable)) { //if they are different nomials
			Debug.Log("Incompatible Nomials!"); //throw an error
		}
		int prevCoefficient = node.Previous.Value.coefficient;
		node.List.Remove(node.Previous); //remove from the list
		Destroy(node.Previous.Value.gameObject); //destroy
		coefficient = prevCoefficient + this.coefficient; //add this.value and prevValue
		TextMesh textMesh = transform.GetChild(0).GetComponent<TextMesh>(); //get our text mesh
		textMesh.text = coefficient.ToString() + variable; //update value onscreen
		equation.align(); //realign equation to account for gap
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (beingDragged) {
			//Destroy (gameObject);
		}
	}
}