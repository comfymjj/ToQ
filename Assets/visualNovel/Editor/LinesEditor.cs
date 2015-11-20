using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Lines))]
public class LinesEditor : Editor {
	private Lines script;
	//better than direct assignment b/c it allows undoing & redoing
	private ReorderableList reorderableList;
	private SerializedProperty leftCharacter;
	private SerializedProperty rightCharacter;
	private float width1; //width of first item in reorderablelist

	private void OnEnable() {
		//script = (Lines)target; //the script this class is targeting
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
				 sprite, typeof(Sprite));

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

	public override void OnInspectorGUI() {
		//begin OnInspectorGUI
		serializedObject.Update();
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
				EditorGUILayout.LabelField("Left Character:");
				EditorGUILayout.PropertyField(leftCharacter, GUIContent.none);
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical();
				EditorGUILayout.LabelField("Right Character:");
				EditorGUILayout.PropertyField(rightCharacter, GUIContent.none);
			EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();

		//wrap text in reorderablelist 
		EditorStyles.textField.wordWrap = true;
		//run drawElementCallback
		reorderableList.DoLayoutList();
		//end OnInpectorGUI
		serializedObject.ApplyModifiedProperties();
	}
}