using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class MenuPlayerColor : MonoBehaviour 
{
	public PlayerName player;

	// Use this for initialization
	void Start () 
	{
		Setup ();
	}

	[ButtonAttribute()]
	public void Setup ()
	{
		GlobalVariables gv = FindObjectOfType<GlobalVariables> ();

		if(GetComponent<Text> () != null)
			GetComponent<Text> ().color = gv.playersColors [(int)player];
		
		else if(GetComponent<Image> () != null)
			GetComponent<Image> ().color = gv.playersColors [(int)player];
	}
}
