using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;

//helper classes
public enum Speaker { leftCharacter, rightCharacter };
[System.Serializable]
public struct Line {
	public Speaker speaker;
	public Sprite sprite;
	public string name;
	public string dialogue;
}

//main class
public class Lines : MonoBehaviour {
	public TextAsset txt; //a file, e.g. dialogue.txt
	public Line[] lines; //array of all lines shown one-by-one in textbox
	public SpriteRenderer leftCharacter;
	public SpriteRenderer rightCharacter;
	public Color fadeIn = Color.black;
	public Color fadeOut = Color.black;

	private string[] words; //array of lines broken up by word
	private Text text; //actual text object in box
	private int lineIndex; //the current line we're on
	private int wordIndex; //the current word we're on

	private enum Fade { In, Out, None }
	private Fade fading;
	private RectTransform textBox; //position of our textBox
	private RectTransform nameBox; //position of the box in the textbox that displays names
	private Text speakerName; //text showing name of current speaker
	private RawImage fader; //used for fading in/out of black

	void Start() {
		//get game objects from the hierarchy
		textBox = GameObject.Find("canvas/textBox").GetComponent<RectTransform>();
		text = GameObject.Find("canvas/textBox/textHolder/text").GetComponent<Text>();
		nameBox = GameObject.Find("canvas/textBox/nameBox").GetComponent<RectTransform>();
		speakerName = GameObject.Find("canvas/textBox/nameBox/name").GetComponent<Text>();
		fader = GameObject.Find("canvas/black").GetComponent<RawImage>();
		speakerName.text = "";
		text.text = "";

		lineIndex = -1;

		//start with fadeIn colour
		fading = Fade.In;
		fader.color = fadeIn;
	}

	//runs 60 times a second
	void Update() {
		//fades take away control until done
		if (fading != Fade.None) {
			fade();
			return;
		}

		//go to next line on mouse click
		if (Input.GetMouseButtonDown(0) || Input.anyKeyDown) {
			nextLine();
		}
		Debug.Log(fader.color.a);
		if (wordIndex < words.Length) { //print more words, unless all words are onscreen
			text.text += words[wordIndex]+" ";
			wordIndex++;
		}
	}

	private void fade() {
		switch (fading) {
			case Fade.In:
				fadeIn.a -= Time.deltaTime;
				fader.color = fadeIn;
				if (fadeIn.a <= 0) {
					nextLine();
					fading = Fade.None;
				}
				break;
			case Fade.Out:
				fadeOut.a += Time.deltaTime;
				fader.color = fadeIn;
				if (fadeOut.a >= 1) {
					fading = Fade.None;
					//TODO: go to next scene!
				}
				break;
		}
	}

	private void nextLine() {
		//go to the next line
		lineIndex++;
		//prep variables for new line
		wordIndex = 0;
		text.text = "";
		if (lineIndex < lines.Length) {
			words = lines[lineIndex].dialogue.Split(' '); //break up this line by word and store it in words

			float offset = 0.3f; // a 0 to 1 value for how far nameBox.x is from the textbox's center 
			speakerName.text = lines[lineIndex].name; //set name displayed onscreen

			//set current speaker
			if (lines[lineIndex].speaker == Speaker.leftCharacter) {
				nameBox.anchoredPosition = new Vector2(-textBox.rect.width*offset, nameBox.anchoredPosition.y);
				leftCharacter.color = Color.white;
				leftCharacter.sprite = lines[lineIndex].sprite; //update character sprites
				rightCharacter.color = new Color(.75f, .75f, .75f);
			} else if (lines[lineIndex].speaker == Speaker.rightCharacter) {
				nameBox.anchoredPosition = new Vector2(textBox.rect.width*offset, nameBox.anchoredPosition.y);
				rightCharacter.color = Color.white;
				rightCharacter.sprite = lines[lineIndex].sprite; //update character sprites
				leftCharacter.color = new Color(.75f, .75f, .75f);
			}
		} else { //we have finished all lines
			words = new string[]{""};
			fading = Fade.Out;
		}
	}

}