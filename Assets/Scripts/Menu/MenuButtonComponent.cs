using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MenuButtonComponent : EventTrigger 
{
	public int whichChildButton;
	public MenuComponent menuComponentParent;

	// Use this for initialization
	void Awake () 
	{
		menuComponentParent = transform.parent.GetComponent<MenuComponent> ();

		for (int i = 0; i < menuComponentParent.transform.childCount; i++)
			if (menuComponentParent.transform.GetChild (i) == transform)
				whichChildButton = i + 1;
	}

	public override void OnPointerClick( PointerEventData data )
	{
		menuComponentParent.Submit (whichChildButton);
	}

	public override void OnSubmit( BaseEventData data )
	{
		menuComponentParent.Submit (whichChildButton);
	}
}
