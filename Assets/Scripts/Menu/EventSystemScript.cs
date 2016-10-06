using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class EventSystemScript : MonoBehaviour 
{
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
		buttonCurrentlySelected = eventSystem.currentSelectedGameObject;
	}
}
