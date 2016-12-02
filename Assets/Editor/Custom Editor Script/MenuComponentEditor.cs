using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MenuComponent))]
[CanEditMultipleObjects]
public class MenuComponentEditor : Editor 
{
	SerializedProperty menuComponentType;

	SerializedProperty aboveMenuScript;

	SerializedProperty otherMenuList;
	SerializedProperty otherButtonsList;

	SerializedProperty underMenuList;
	SerializedProperty underButtonsList;

	SerializedProperty button;

	SerializedProperty content;

	SerializedProperty selectable;

	SerializedProperty secondaryContentList;

	SerializedProperty endModeContentList;

	bool selectableEnabled = true;
	bool secondaryContentEnabled = true;

	void OnEnable ()
	{
		menuComponentType = serializedObject.FindProperty ("menuComponentType");

		aboveMenuScript = serializedObject.FindProperty ("aboveMenuScript");

		otherMenuList = serializedObject.FindProperty ("otherMenuList");
		otherButtonsList = serializedObject.FindProperty ("otherButtonsList");

		underMenuList = serializedObject.FindProperty ("underMenuList");
		underButtonsList = serializedObject.FindProperty ("underButtonsList");

		button = serializedObject.FindProperty ("button");

		content = serializedObject.FindProperty ("content");

		selectable = serializedObject.FindProperty ("selectable");

		secondaryContentList = serializedObject.FindProperty ("secondaryContentList");

		endModeContentList = serializedObject.FindProperty ("endModeContentList");
	}
	
	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (menuComponentType, true);

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
			
			if(menuComponentType.enumValueIndex == (int)MenuComponentType.ContentMenu)
			{
				EditorGUILayout.PropertyField (content, true);			
			}
			
		}

		EditorGUILayout.Space ();			

		selectableEnabled = EditorGUILayout.Toggle ("Select On Enable", selectableEnabled);

		if(selectableEnabled)
			EditorGUILayout.PropertyField (selectable, true);


		EditorGUILayout.Space ();
	
		secondaryContentEnabled = EditorGUILayout.Toggle ("Secondary Content", secondaryContentEnabled);

		if(secondaryContentEnabled)
			EditorGUILayout.PropertyField (secondaryContentList, true);


		EditorGUILayout.Space ();
		
		if(menuComponentType.enumValueIndex == (int)MenuComponentType.EndModeMenu)
		{
			EditorGUILayout.PropertyField (endModeContentList, true);			
		}


		serializedObject.ApplyModifiedProperties ();
	}
}
