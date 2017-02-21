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


	SerializedProperty overrideHeaderPos;
	SerializedProperty menuHeaderY;
	SerializedProperty menuFirstButtonY;
	SerializedProperty buttonFirstButtonY;

	SerializedProperty overrideButtonsDisplay;
	SerializedProperty showButtonsOnSubmit;
	SerializedProperty hideButtonsOnCancel;
	SerializedProperty hideButtonsOnUnderSubmit;

	SerializedProperty overrideContentDisplay;
	SerializedProperty showContentOnSubmit;
	SerializedProperty hideContentOnCancel;
	SerializedProperty hideContentOnUnderSubmit;

	void OnEnable ()
	{
		menuComponentType = serializedObject.FindProperty ("menuComponentType");

		contentDisplay = serializedObject.FindProperty ("contentDisplay");

		secondaryContents = serializedObject.FindProperty ("secondaryContents");

		selectable = serializedObject.FindProperty ("selectable");

		endModeContents = serializedObject.FindProperty ("endModeContents");


		overrideHeaderPos = serializedObject.FindProperty ("overrideHeaderPos");
		menuHeaderY = serializedObject.FindProperty ("menuHeaderY");
		menuFirstButtonY = serializedObject.FindProperty ("menuFirstButtonY");
		buttonFirstButtonY = serializedObject.FindProperty ("buttonFirstButtonY");


		overrideMenuPos = serializedObject.FindProperty ("overrideMenuPos");
		menuOffScreenX = serializedObject.FindProperty ("menuOffScreenX");
		menuOnScreenX = serializedObject.FindProperty ("menuOnScreenX");

		overrideButtonPos = serializedObject.FindProperty ("overrideButtonPos");
		buttonOffScreenX = serializedObject.FindProperty ("buttonOffScreenX");
		buttonOnScreenX = serializedObject.FindProperty ("buttonOnScreenX");

		overrideContentPos = serializedObject.FindProperty ("overrideContentPos");
		offScreenContent = serializedObject.FindProperty ("offScreenContent");
		onScreenContent = serializedObject.FindProperty ("onScreenContent");


		overrideButtonsDisplay = serializedObject.FindProperty ("overrideButtonsDisplay");
		showButtonsOnSubmit = serializedObject.FindProperty ("showButtonsOnSubmit");
		hideButtonsOnCancel = serializedObject.FindProperty ("hideButtonsOnCancel");
		hideButtonsOnUnderSubmit = serializedObject.FindProperty ("hideButtonsOnUnderSubmit");

		overrideContentDisplay = serializedObject.FindProperty ("overrideContentDisplay");
		showContentOnSubmit = serializedObject.FindProperty ("showContentOnSubmit");
		hideContentOnCancel = serializedObject.FindProperty ("hideContentOnCancel");
		hideContentOnUnderSubmit = serializedObject.FindProperty ("hideContentOnUnderSubmit");
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

		EditorGUILayout.PropertyField (overrideHeaderPos, true);

		if(overrideHeaderPos.boolValue == true)
		{
			EditorGUI.indentLevel = 1;
			EditorGUILayout.PropertyField (menuHeaderY, true);
			EditorGUILayout.PropertyField (menuFirstButtonY, true);
			EditorGUILayout.PropertyField (buttonFirstButtonY, true);
			EditorGUI.indentLevel = 0;
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (overrideMenuPos, true);

		if(overrideMenuPos.boolValue == true)
		{
			EditorGUI.indentLevel = 1;
			EditorGUILayout.PropertyField (menuOffScreenX, true);
			EditorGUILayout.PropertyField (menuOnScreenX, true);
			EditorGUI.indentLevel = 0;
		}

		EditorGUILayout.Space ();
		
		EditorGUILayout.PropertyField (overrideButtonPos, true);

		if(overrideButtonPos.boolValue == true)
		{
			EditorGUI.indentLevel = 1;
			EditorGUILayout.PropertyField (buttonOffScreenX, true);
			EditorGUILayout.PropertyField (buttonOnScreenX, true);
			EditorGUI.indentLevel = 0;
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (overrideContentPos, true);

		if(overrideContentPos.boolValue == true)
		{
			EditorGUI.indentLevel = 1;
			EditorGUILayout.PropertyField (offScreenContent, true);
			EditorGUILayout.PropertyField (onScreenContent, true);
			EditorGUI.indentLevel = 0;
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (overrideButtonsDisplay, true);

		if(overrideButtonsDisplay.boolValue == true)
		{
			EditorGUI.indentLevel = 1;
			EditorGUILayout.PropertyField (showButtonsOnSubmit, true);
			EditorGUILayout.PropertyField (hideButtonsOnCancel, true);
			EditorGUILayout.PropertyField (hideButtonsOnUnderSubmit, true);
			EditorGUI.indentLevel = 0;
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (overrideContentDisplay, true);

		if(overrideContentDisplay.boolValue == true)
		{
			EditorGUI.indentLevel = 1;
			EditorGUILayout.PropertyField (showContentOnSubmit, true);
			EditorGUILayout.PropertyField (hideContentOnCancel, true);
			EditorGUILayout.PropertyField (hideContentOnUnderSubmit, true);
			EditorGUI.indentLevel = 0;
		}
			
		serializedObject.ApplyModifiedProperties ();
	}
}
