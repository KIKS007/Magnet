using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonComponent : EventTrigger 
{
	public int buttonIndex;
	public MenuComponent menuComponentParent;

	private Button button;

	// Use this for initialization
	void Awake () 
	{
		button = GetComponent<Button> ();
		menuComponentParent = transform.parent.GetComponent<MenuComponent> ();
	}

	public override void OnPointerClick( PointerEventData data )
	{
		if(button.interactable)
			menuComponentParent.Submit (buttonIndex);
	}

	public override void OnSubmit( BaseEventData data )
	{
		if(button.interactable)
			menuComponentParent.Submit (buttonIndex);
	}
}
