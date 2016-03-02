using UnityEngine;
public class Character : MonoBehaviour {
	public Sprite[] sprites;
	private SpriteRenderer spriteRenderer;

	void Start() {
		spriteRenderer = this.GetComponent<SpriteRenderer>();
	}

	public void setSprite(Sprite sprite) {
		spriteRenderer.sprite = sprite;
	}
}