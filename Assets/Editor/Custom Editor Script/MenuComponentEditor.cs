using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MenuComponent))]
[CanEditMultipleObjects]
public class MenuComponentEditor : Editor 
{
	SerializedProperty menuComponentType;

	SerializedProperty viewportContent;

	SerializedProperty aboveMenuScript;

	SerializedProperty otherMenuList;
	SerializedProperty otherButtonsList;

	SerializedProperty underMenuList;
	SerializedProperty underButtonsList;

	SerializedProperty button;

	SerializedProperty content;

	SerializedProperty selectable;

	SerializedProperty previousSelected;

	SerializedProperty secondaryContentList;

	SerializedProperty endModeContentList;

	SerializedProperty underButtonsPositionsList;

	void OnEnable ()
	{
		menuComponentType = serializedObject.FindProperty ("menuComponentType");

		viewportContent = serializedObject.FindProperty ("viewportContent");

		aboveMenuScript = serializedObject.FindProperty ("aboveMenuScript");

		otherMenuList = serializedObject.FindProperty ("otherMenuList");
		otherButtonsList = serializedObject.FindProperty ("otherButtonsList");

		underMenuList = serializedObject.FindProperty ("underMenuList");
		underButtonsList = serializedObject.FindProperty ("underButtonsList");

		button = serializedObject.FindProperty ("button");

		content = serializedObject.FindProperty ("content");

		selectable = serializedObject.FindProperty ("selectable");

		previousSelected = serializedObject.FindProperty ("previousSelected");

		secondaryContentList = serializedObject.FindProperty ("secondaryContentList");

		endModeContentList = serializedObject.FindProperty ("endModeContentList");

		underButtonsPositionsList = serializedObject.FindProperty ("underButtonsPositionsList");
	}
	
	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (menuComponentType, true);

		EditorGUILayout.PropertyField (viewportContent, true);

		if(menuComponentType.enumValueIndex != (int)MenuComponentType.EndModeMenu)
		{
			if(menuComponentType.enumValueIndex != (int)MenuComponentType.MainMenu)
			{
				EditorGUILayout.PropertyField (aboveMenuScript, true);
				
				EditorGUILayout.PropertyField (button, true);
				
				EditorGUILayout.PropertyField (otherMenuList, true);
				EditorGUILayout.PropertyField (otherButtonsList, true);		
			}
			
			if(menuComponentType.enumValueIndex == (int)MenuComponentType.ButtonsListMenu || menuComponentType.enumValueIndex == (int)MenuComponentType.MainMenu)
			{
				EditorGUILayout.PropertyField (underMenuList, true);
				EditorGUILayout.PropertyField (underButtonsList, true);
			}
			
			if(menuComponentType.enumValueIndex == (int)MenuComponentType.ContentMenu || viewportContent.boolValue == true)
			{
				EditorGUILayout.PropertyField (content, true);			
			}
			
		}

		EditorGUILayout.Space ();			

		EditorGUILayout.PropertyField (selectable, true);
		EditorGUILayout.PropertyField (previousSelected, true);


		EditorGUILayout.Space ();
	
		EditorGUILayout.PropertyField (secondaryContentList, true);

		EditorGUILayout.Space ();
		
		if(menuComponentType.enumValueIndex == (int)MenuComponentType.EndModeMenu)
		{
			EditorGUILayout.PropertyField (endModeContentList, true);			
		}

		EditorGUILayout.PropertyField (underButtonsPositionsList, true);

		serializedObject.ApplyModifiedProperties ();
	}
}
