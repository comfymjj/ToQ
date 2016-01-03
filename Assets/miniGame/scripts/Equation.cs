using UnityEngine;
using System.Collections;

public class Equation : MonoBehaviour {
	private float _eqWidth; //combined width + spacing of all symbols in equation
	private float _screenWidth; //width of the screen in worldspace
	private Symbol head; //the head of our list of symbols

	void Start() {
		_eqWidth = 0f;
		_screenWidth = (Camera.main.orthographicSize*2)*(Screen.width/(float)Screen.height); //screen width in world units
		Symbol[] children = this.GetComponentsInChildren<Symbol>();
		foreach (Symbol item in children) {
			Debug.Log(item);
		}
		head = children[0]; //will be head of a linked list of symbols
		_eqWidth += head.width;
		for (int i = 1; i < children.Length-1; i++) { //for all children except first & last
			children[i].next = children[i+1]; //tie these two together
			children[i+1].prev = children[i];
			_eqWidth += children[i].width + children[i].padding*2; //used to center the equation on the screen
			Debug.Log("width: " + children[i].width+", padding: "+ children[i].padding);
		}
		_eqWidth += children[children.Length-1].width;
		this.align(); //properly space all the symbols in a row
	}

	void Update() {
        Debug.DrawLine(head.transform.position, head.transform.position + Vector3.right*_eqWidth);
    }

	public void align() {
		head.x = (_screenWidth - _eqWidth)/2;
		for (Symbol symbol = head.next; symbol != null; symbol = symbol.next) { //for all symbols except head
            Debug.Log("aligned");
			symbol.align();
		}
	}
}
