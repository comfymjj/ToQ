using UnityEngine;
using System.Collections;

public class Operator : Symbol {
	public override float x { //x position of left edge of sprite
		get {
			if (isActiveAndEnabled) { //true if operator isn't hidden on screen
				return sprite.bounds.min.x;
			} else { //the above statement returns null if hidden
				Debug.Log("op.x: " + transform.position.x);
				return transform.position.x; //so instead we'll just return the regular transform position
			}
		}
		set { transform.position = new Vector3(value + sprite.bounds.extents.x, transform.position.y, transform.position.z); }
	}

	public override float width { //width of sprite
		get {
			if (isActiveAndEnabled) {
				return sprite.bounds.size.x;
			} else {
				return 0;
			}
		}
	}

	private float _padding; //operators have padding on either side
	public float padding {
		get {
			if (isActiveAndEnabled) {
				return _padding;
			} else {
				return 0;
			}
		}
		set { _padding = value; }
	}
}
