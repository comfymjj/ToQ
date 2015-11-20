using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

//helper classes
public enum Speaker {leftCharacter, rightCharacter};
[System.Serializable]
public struct Line {
	public Speaker speaker;
	public Sprite sprite;
	public string text;
}

//main class
public class Lines : MonoBehaviour {
    public List<Line> lines = new List<Line>(); //array of all lines shown one-by-one in textbox
    public SpriteRenderer leftCharacter;
    public SpriteRenderer rightCharacter;

    private string[] words; //array of lines broken up by word
    private Text text; //actual text object in box
    private int lineIndex; //the current line we're on
    private int wordIndex; //the current word we're on

	private RectTransform textBox; //position of our textBox
	private RectTransform nameBox; //position of the box in the textbox that displays names
	private Text speakerName; //text showing name of current speaker

	void Start () {
		//get game objects from the hierarchy
		textBox = GameObject.Find("canvas/textBox").GetComponent<RectTransform>();
		text = GameObject.Find("canvas/textBox/textHolder/text").GetComponent<Text>();
		nameBox = GameObject.Find("canvas/textBox/nameBox").GetComponent<RectTransform>();
		speakerName = GameObject.Find("canvas/textBox/nameBox/name").GetComponent<Text>();

		//display the first line
		lineIndex = -1;
		nextLine();
	}

	//runs 60 times a second
    void Update() {
        //go to next line one mouse click
        if (Input.GetMouseButtonDown(0) || Input.anyKeyDown) {
			nextLine();
        }
        //print more words, unless all words are onscreen
        if (wordIndex < words.Length) {
			text.text += words[wordIndex]+" ";
			wordIndex++;
		}
	}

	private void nextLine() {
		//go to the next line
		lineIndex++;
		//prep variables for new line
		wordIndex = 0;
		text.text = "";
		words = lines[lineIndex].text.Split(' '); //break up this line by word and store it in words
		//update character sprites
		if (lines[lineIndex].sprite != null) {
			//set the current speaker
			float offset = 0.3f; // a 0 to 1 value for how far nameBox.x is from the textbox's center 
			if (lines[lineIndex].speaker == Speaker.leftCharacter) {
				nameBox.anchoredPosition = new Vector2(-textBox.rect.width*offset, nameBox.anchoredPosition.y);
				speakerName.text = leftCharacter.name;
				leftCharacter.color = Color.white;
				leftCharacter.sprite = lines[lineIndex].sprite;
				rightCharacter.color = new Color(.75f, .75f, .75f);
			} else if (lines[lineIndex].speaker == Speaker.rightCharacter) {
				nameBox.anchoredPosition = new Vector2(textBox.rect.width*offset, nameBox.anchoredPosition.y);
				speakerName.text = rightCharacter.name;
				rightCharacter.color = Color.white;
				rightCharacter.sprite = lines[lineIndex].sprite;
				leftCharacter.color = new Color(.75f, .75f, .75f);
			}
		}
	}

}