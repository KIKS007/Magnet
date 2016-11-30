using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonComponent : MonoBehaviour, IPointerClickHandler, ISubmitHandler
{
	[Header ("Start Mode")]
	public bool startModeButton = false;
	public string whichMode = "";

	[Header ("End Mode")]
	public bool endModeButton = false;
	public int whichModeContent = 0;

	[HideInInspector]
	public int buttonIndex;
	[HideInInspector]
	public MenuComponent menuComponentParent;

	private Button button;

	// Use this for initialization
	void Awake () 
	{
		button = GetComponent<Button> ();
		menuComponentParent = transform.parent.GetComponent<MenuComponent> ();
	}

	public void OnPointerClick( PointerEventData data )
	{
		if(button.interactable)
		{
			menuComponentParent.Submit (buttonIndex);

			if(startModeButton)
				MenuManager.Instance.MenuLoadMode (whichMode);
		}
	}

	public void OnSubmit( BaseEventData data )
	{
		if(button.interactable)
		{
			menuComponentParent.Submit (buttonIndex);

			if(startModeButton)
				MenuManager.Instance.MenuLoadMode (whichMode);
		}
	}
}
