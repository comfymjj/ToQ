using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Text.RegularExpressions;

[CustomEditor(typeof(Lines))]
public class LinesEditor : Editor {
	private Lines script;
	//better than direct assignment b/c it allows undoing & redoing
	private ReorderableList reorderableList;
	private SerializedProperty leftCharacter;
	private SerializedProperty rightCharacter;
	private float width1; //width of first item in reorderablelist

	[MenuItem("GameObject/Create Visual Novel Setup")]
	public static void CreateVisualNovel() {
		PrefabUtility.InstantiatePrefab(Resources.Load("List of Lines"));
		PrefabUtility.InstantiatePrefab(Resources.Load("background"));
		PrefabUtility.InstantiatePrefab(Resources.Load("leftCharacter"));
		PrefabUtility.InstantiatePrefab(Resources.Load("rightCharacter"));
		((GameObject)PrefabUtility.InstantiatePrefab(Resources.Load("canvas"))).GetComponent<Canvas>().worldCamera = Camera.main;
	}

	void OnEnable() {
		script = (Lines)target;

		leftCharacter = serializedObject.FindProperty("leftCharacter");
		rightCharacter = serializedObject.FindProperty("rightCharacter");
	}

	public override void OnInspectorGUI() {
		serializedObject.Update();

		//the .txt file we're regexing the lines from
		SerializedProperty txt = serializedObject.FindProperty("txt");
		EditorGUILayout.PropertyField(txt);

		if (GUI.changed) {
			string text = ((TextAsset)txt.objectReferenceValue).text;

			string pattern =
				@"\n(?<name>[^\n:]*): *" + //match beginning, e.g. "Prince:"
				@"(?<dialogue>[^:]*)" + //match middle, i.e. dialogue
				@"\n[^\n:]*:"; //match end, i.e. beginning of next line

			MatchCollection matches = Regex.Matches(text, pattern); //get all regex matches

			Debug.Log("GUI changed");

			if (script.lines.Length != matches.Count) {
				script.lines = new Line[matches.Count];
			}

			for (int i = 0; i < matches.Count; i++) {
				script.lines[i].name = matches[i].Groups["name"].ToString();
				script.lines[i].dialogue = matches[i].Groups["dialogue"].ToString();
			}

			serializedObject.Update();
		}

		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.PropertyField(leftCharacter, GUIContent.none);
			EditorGUILayout.PropertyField(rightCharacter, GUIContent.none);
		}
		EditorGUILayout.EndHorizontal();

		SerializedProperty fadeIn = serializedObject.FindProperty("fadeIn");
		EditorGUILayout.PropertyField(fadeIn);

		SerializedProperty lines = serializedObject.FindProperty("lines");

		Debug.Log(lines.arraySize);

		ArrayGUI(lines);

		SerializedProperty fadeOut = serializedObject.FindProperty("fadeOut");
		EditorGUILayout.PropertyField(fadeOut);

		//wrap text in reorderablelist 
		//EditorStyles.textField.wordWrap = true;
		//run drawElementCallback
		//reorderableList.DoLayoutList();

		serializedObject.ApplyModifiedProperties();
	}

	void ArrayGUI(SerializedProperty _property) {
		int size = _property.arraySize;

		for (int i = 0; i<size; i++) {
			var element = _property.GetArrayElementAtIndex(i);

			EditorGUILayout.BeginHorizontal();
			{
				//sprite
				SerializedProperty spriteProp = element.FindPropertyRelative("sprite");
				Sprite sprite = (Sprite)spriteProp.objectReferenceValue;

				float height = 80; //chosen arbitrarily
				float width; //width of the sprite thumbnail
				if (sprite == null) {
					width = height;
				} else {
					width = sprite.rect.width * (height / sprite.rect.height);
					if (width > height) {
						width = height;
					}
				}

				Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(width), GUILayout.Height(height));
				spriteProp.objectReferenceValue = (Sprite)EditorGUI.ObjectField(rect, sprite, typeof(Sprite), false);

				EditorGUILayout.BeginVertical();
				{
					//speaker
					EditorGUILayout.PropertyField(element.FindPropertyRelative("speaker"), GUIContent.none);

					//text
					string name = element.FindPropertyRelative("name").stringValue;
					string dialogue = element.FindPropertyRelative("dialogue").stringValue;
					EditorStyles.label.wordWrap = true;
					EditorGUILayout.LabelField(name+": "+dialogue);
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndHorizontal();
		}
	}





















	private void stuff() {
		script = (Lines)target; //the script this class is targeting
		
		//reorderableList = new ReorderableList(script.lines, typeof(string), true, true, true, true);
		reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("lines"), true, true, true, true); //a list of all lines
		width1 = 90; //width of item 1, for easy reference
		reorderableList.elementHeight = 40; //this is also the width of item2
		leftCharacter = serializedObject.FindProperty("leftCharacter");
		rightCharacter = serializedObject.FindProperty("rightCharacter");

		//Draws one element of the list
		reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
			EditorGUI.BeginChangeCheck();

			SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
			
			//edge offset
			rect.x += 2;

			//speaker enum
			float yCenter = (rect.height - EditorGUIUtility.singleLineHeight)/2; //for Y-Centering the enum
			EditorGUI.PropertyField(
				new Rect(rect.x, rect.y + yCenter, width1, rect.height),
				element.FindPropertyRelative("speaker"), GUIContent.none);

			//area of next item, + spacing
			rect.x += width1 + 2;
			rect.width -= width1 + 2;

			//sprite
			SerializedProperty spriteProp = element.FindPropertyRelative("sprite");
			Sprite sprite = (Sprite)spriteProp.objectReferenceValue;
			float width2; //width of the sprite thumbnail
			if (sprite == null) {
				width2 = rect.height;
			} else {
				width2 = sprite.rect.width * (rect.height / sprite.rect.height);
				if (width2 > rect.height) {
					width2 = rect.height;
				}
			}
			float xCenter = (rect.height - width2)/2; //used to center the sprite
			spriteProp.objectReferenceValue = (Sprite)EditorGUI.ObjectField(new Rect(rect.x + xCenter, rect.y, width2, rect.height),
				 sprite, typeof(Sprite), false);

			//draw next in line
			rect.x += rect.height + 2;
			rect.width -= rect.height + 2;
			rect.y++; //spacing between textboxes
			rect.height--;

			//text
			EditorGUI.PropertyField(rect, element.FindPropertyRelative("text"), GUIContent.none);
			
			if (EditorGUI.EndChangeCheck()) {
				EditorUtility.SetDirty(target);
			}
			
		};
		
		
		//Now add listeners for the ReorderableList (it's kind of outdated so it doesn't update through onInpectorGUI)

		// Draws the header of the list
		reorderableList.drawHeaderCallback = (Rect rect) => {
			rect.x += 14; //shift everything to the right
			GUI.Label(new Rect(rect.x, rect.y, width1, rect.height), "Speaker:");
			rect.x += width1 + 2;
			rect.width -= width1 + 2;
			GUI.Label(new Rect(rect.x, rect.y, reorderableList.elementHeight, rect.height), "Sprite:");
			rect.x += reorderableList.elementHeight + 2;
			rect.width -= reorderableList.elementHeight + 2;
			GUI.Label(rect, "Lines:");
		};

		reorderableList.onAddCallback = (ReorderableList list) => {
			/*
			script.lines.Add("");
			script.sprites.Add(null);
			*/

			var index = list.serializedProperty.arraySize;
			list.serializedProperty.arraySize++;
			list.index = index;
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			element.FindPropertyRelative("speaker").enumValueIndex = 0;
			element.FindPropertyRelative("sprite").objectReferenceValue = null;
			element.FindPropertyRelative("text").stringValue = "";

			EditorUtility.SetDirty(target);
		};
		reorderableList.onRemoveCallback = (ReorderableList list) => {
			/*
			script.lines.RemoveAt(list.index);
			script.sprites.RemoveAt(list.index);
			*/
			ReorderableList.defaultBehaviours.DoRemoveButton(list);
			EditorUtility.SetDirty(target);
		};

		// This could be used aswell, but I only advise this your class inherrits from UnityEngine.Object or has a CustomPropertyDrawer
		// Since you'll find your item using: serializedObject.FindProperty("list").GetArrayElementAtIndex(index).objectReferenceValue
		// which is a UnityEngine.Object
		// reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("list"), true, true, true, true);
	}

	

}