using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MenuComponent))]
[CanEditMultipleObjects]
public class MenuComponentEditor : Editor 
{
	SerializedProperty menuComponentType;

	SerializedProperty contentDisplay;

	SerializedProperty secondaryContents;

	SerializedProperty selectable;

	SerializedProperty endModeContents;

	SerializedProperty overrideMenuPos;
	SerializedProperty menuOffScreenX;
	SerializedProperty menuOnScreenX;

	SerializedProperty overrideButtonPos;
	SerializedProperty buttonOffScreenX;
	SerializedProperty buttonOnScreenX;

	SerializedProperty overrideContentPos;
	SerializedProperty offScreenContent;
	SerializedProperty onScreenContent;

	void OnEnable ()
	{
		menuComponentType = serializedObject.FindProperty ("menuComponentType");

		contentDisplay = serializedObject.FindProperty ("contentDisplay");

		secondaryContents = serializedObject.FindProperty ("secondaryContents");

		selectable = serializedObject.FindProperty ("selectable");

		endModeContents = serializedObject.FindProperty ("endModeContents");


		overrideMenuPos = serializedObject.FindProperty ("overrideMenuPos");
		menuOffScreenX = serializedObject.FindProperty ("menuOffScreenX");
		menuOnScreenX = serializedObject.FindProperty ("menuOnScreenX");

		overrideButtonPos = serializedObject.FindProperty ("overrideButtonPos");
		buttonOffScreenX = serializedObject.FindProperty ("buttonOffScreenX");
		buttonOnScreenX = serializedObject.FindProperty ("buttonOnScreenX");

		overrideContentPos = serializedObject.FindProperty ("overrideContentPos");
		offScreenContent = serializedObject.FindProperty ("offScreenContent");
		onScreenContent = serializedObject.FindProperty ("onScreenContent");

	}
	
	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (menuComponentType, true);

		EditorGUILayout.PropertyField (contentDisplay, true);

		EditorGUILayout.PropertyField (secondaryContents, true);

		EditorGUILayout.PropertyField (selectable, true);

		if(menuComponentType.enumValueIndex == (int)MenuComponentType.EndModeMenu)
			EditorGUILayout.PropertyField (endModeContents, true);			

		EditorGUILayout.PropertyField (overrideMenuPos, true);

		if(overrideMenuPos.boolValue == true)
		{
			EditorGUILayout.PropertyField (menuOffScreenX, true);
			EditorGUILayout.PropertyField (menuOnScreenX, true);
		}

			EditorGUILayout.Space ();
		
		EditorGUILayout.PropertyField (overrideButtonPos, true);

		if(overrideButtonPos.boolValue == true)
		{
			EditorGUILayout.PropertyField (buttonOffScreenX, true);
			EditorGUILayout.PropertyField (buttonOnScreenX, true);
		}

			EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (overrideContentPos, true);

		if(overrideContentPos.boolValue == true)
		{
			EditorGUILayout.PropertyField (offScreenContent, true);
			EditorGUILayout.PropertyField (onScreenContent, true);
		}
			
		serializedObject.ApplyModifiedProperties ();
	}
}
