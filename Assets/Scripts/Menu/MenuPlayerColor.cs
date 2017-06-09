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
	void Setup ()
	{
		if(GetComponent<Text> () != null)
			GetComponent<Text> ().color = GlobalVariables.Instance.playersColors [(int)player];
		
		else if(GetComponent<Image> () != null)
			GetComponent<Image> ().color = GlobalVariables.Instance.playersColors [(int)player];
	}
}
