using UnityEngine;
using System.Collections;

public abstract class Symbol : MonoBehaviour {
	protected SpriteRenderer sprite; //reference to our SpriteRenderer
	protected virtual void initialize() { //need to call base.initialize inside Awake() function of all subclasses
		sprite = GetComponent<SpriteRenderer>();
	}

	public virtual float x { //for easy access
		get { return sprite.bounds.min.x; } //x position of left edge of sprite
		set { transform.position = new Vector3(value + sprite.bounds.extents.x, transform.position.y, transform.position.z); }
	}

	public virtual float width { //width of sprite
		get { return sprite.bounds.size.x; }
	}

	public virtual float midPoint {
		get { return transform.position.x; }
	}
}
