using UnityEngine;
using System.Collections;

public abstract class Symbol : MonoBehaviour {
	public float x { //for easy access
		get { return transform.position.x; }
		set { transform.position = new Vector3(value, transform.position.y, transform.position.z); }
	}
	private SpriteRenderer sprite; //reference to our SpriteRenderer
	public float width { //width of sprite
		get { return sprite.bounds.size.x; }
	}
    [HideInInspector]
    public float padding; //operators have padding on either side
    [HideInInspector]
    public Symbol prev; //symbol before this one
    [HideInInspector]
    public Symbol next; //symbol after this one

	public void Awake() { //need to call base.Start in subclasses
		sprite = GetComponent<SpriteRenderer>();
	}

	public void align() { //positions this symbol to the right of prev
		this.x = prev.x + prev.width/2 + padding + this.width/2;
	}
}
