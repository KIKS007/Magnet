using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MenuButtonComponent))]
[CanEditMultipleObjects]
public class MenuButtonComponentEditor : Editor
{
	SerializedProperty menuButtonType;

	SerializedProperty whichMode;


	// Use this for initialization
	void OnEnable () 
	{
		menuButtonType = serializedObject.FindProperty ("menuButtonType");

		whichMode = serializedObject.FindProperty ("whichMode");
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

		serializedObject.ApplyModifiedProperties ();
	}
}
