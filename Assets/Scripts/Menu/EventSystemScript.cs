using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class EventSystemScript : MonoBehaviour 
{
	public static Action<GameObject> OnNewSelectedGameObject;

	public GameObject buttonCurrentlySelected = null;

	private EventSystem eventSystem;

	// Use this for initialization
	void Start () 
	{
		eventSystem = GetComponent<EventSystem> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(buttonCurrentlySelected != eventSystem.currentSelectedGameObject)
		{
			buttonCurrentlySelected = eventSystem.currentSelectedGameObject;

			if (OnNewSelectedGameObject != null)
				OnNewSelectedGameObject (buttonCurrentlySelected);
		}
	}
}
