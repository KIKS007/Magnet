using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameText : MonoBehaviour 
{
	private Text text;

	// Use this for initialization
	void Start () 
	{
		text = GetComponent<Text> ();

		MenuManager.Instance.OnFarPosition += ChangeText;
	}

	void ChangeText ()
	{
		text.text = "PRESS BACK BUTTON";	
	}

	void OnDestroy ()
	{
		if(!GlobalVariables.applicationIsQuitting)
			MenuManager.Instance.OnFarPosition -= ChangeText;
	}
}
