using UnityEngine;
using System.Collections.Generic;

public class Equation : MonoBehaviour {
	public LinkedList<Operand> children; //list of all children
	private float _eqWidth; //combined width + spacing of all symbols in equation
	private float _screenWidth;

	void Start() {
		_eqWidth = 0f;
		_screenWidth = (Camera.main.orthographicSize*2)*(Screen.width/(float)Screen.height); //screen width in world units
		children = new LinkedList<Operand>(this.GetComponentsInChildren<Operand>()); //create LinkedList of all children
		for (LinkedListNode<Operand> current = children.First; current != null; current = current.Next) { //for each node in children list
			current.Value.equation = this; //give it a reference to this equation
			current.Value.node = current; //set its node
			_eqWidth += current.Value.sortWidth; //add up widths of all children
		}
		this.align();
	}

	void Update() {
		Debug.DrawLine(children.First.Value.transform.position, children.First.Value.transform.position + Vector3.right*_eqWidth, Color.red);
		Debug.DrawLine(children.First.Value.transform.position - Vector3.down, children.First.Value.transform.position + Vector3.right*_screenWidth - Vector3.down, Color.blue);
	}

	//properly space all the symbols in a row
	public void align() {
		LinkedListNode<Operand> head = children.First; //for easy access
		if (head.Value.beingDragged) { //if head is being dragged
			head.Next.Value.align(-_eqWidth/2f+head.Value.width); //align start
		} else {
			head.Value.align(-_eqWidth/2f); //position head so that rest of equation is centered on screen
		}
	}

	public void deleteNode(LinkedListNode<Operand> node) {
		if (node == children.First) { //node is head
			node.Next.Value.operation.gameObject.SetActive(false); //hide operator of the next in line
		}
		node.List.Remove(node); //remove from the list
		//change this to stack enemies
		Destroy(node.Value.GetComponent<Operand>()); //script
		Destroy(node.Value.transform.GetChild(0).gameObject); //destroy both children
		Destroy(node.Value.transform.GetChild(1).gameObject);
		_eqWidth = 0; //prepare _eqWidth for update
		foreach(Operand child in children) {
			_eqWidth += child.sortWidth;
		}
		this.align();
	}
}