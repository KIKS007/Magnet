using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MenuButtonComponent))]
[CanEditMultipleObjects]
public class MenuButtonComponentEditor : Editor
{
	SerializedProperty menuButtonType;

	SerializedProperty whichMode;

	SerializedProperty showOnSelect;
	SerializedProperty showOnSubmit;
	SerializedProperty hideOnDeselect;
	SerializedProperty secondaryContentList;

	// Use this for initialization
	void OnEnable () 
	{
		menuButtonType = serializedObject.FindProperty ("menuButtonType");

		whichMode = serializedObject.FindProperty ("whichMode");

		showOnSelect = serializedObject.FindProperty ("showOnSelect");
		showOnSubmit = serializedObject.FindProperty ("showOnSubmit");
		hideOnDeselect = serializedObject.FindProperty ("hideOnDeselect");
		secondaryContentList = serializedObject.FindProperty ("secondaryContentList");

	}
	
	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (menuButtonType, true);

		EditorGUILayout.Space ();

		if(menuButtonType.enumValueIndex == (int)MenuButtonType.StartMode)
		{
			EditorGUILayout.PropertyField (whichMode, true);
		}

		EditorGUILayout.PropertyField (showOnSelect, true);
		EditorGUILayout.PropertyField (showOnSubmit, true);
		EditorGUILayout.PropertyField (hideOnDeselect, true);
		EditorGUILayout.PropertyField (secondaryContentList, true);

		serializedObject.ApplyModifiedProperties ();
	}
}