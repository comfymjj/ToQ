using UnityEngine;
using System.Collections.Generic;

public class Equation : MonoBehaviour {
	//return a list of all children
	public IEnumerable<Group> getChildren() {
		for (Group group = head; group != null; group = group.next) {
			yield return group;
		}
	}
	public Group head; //the head of our list of groups
	private float _eqWidth; //combined width + spacing of all symbols in equation
	private float _screenWidth; //width of the screen in worldspace

	void Start() {
		_eqWidth = 0f;
		_screenWidth = (Camera.main.orthographicSize*2)*(Screen.width/(float)Screen.height); //screen width in world units
		Group[] children = this.GetComponentsInChildren<Group>();
		head = children[0]; 
		for (int i = 0; i < children.Length-1; i++) { //for all children except last
			children[i].equation = this; //tell the symbol which equation it belongs to
			children[i].setNext(children[i+1]); //tie children[i] and children[i+1] together
			_eqWidth += children[i].width; //_eqWidth is used later to center the equation on the screen
		}
		children[children.Length-1].equation = this; //add last symbol since it was excluded from the above loop
		_eqWidth += children[children.Length-1].width;
		this.align(null); //properly space all the symbols in a row
	}

	void Update() {
        Debug.DrawLine(head.transform.position, head.transform.position + Vector3.right*_eqWidth, Color.red);
		Debug.DrawLine(head.transform.position - Vector3.down, head.transform.position + Vector3.right*_screenWidth - Vector3.down, Color.blue);
	}

	//ignore is given space in the alignment but it's x position is not set
	public void align(Group ignore) {
		string childs = "";
		foreach (Group child in getChildren()) {
			childs += child.ToString() + ", ";
		}
		Debug.Log(childs);
		head.x = -_eqWidth/2f; //position head so that rest of equation is centered on screen
		for (Group group = head.next; group != null; group = group.next) { //for all symbols except head
			if (group != ignore) {
				group.align(); //line up each symbol to the right of the previous one
			}
		}
	}
}
