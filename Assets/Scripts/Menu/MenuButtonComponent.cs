using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum MenuButtonType {Basic, StartMode};

public class MenuButtonComponent : MonoBehaviour, IPointerClickHandler, ISubmitHandler
{
	public MenuButtonType menuButtonType = MenuButtonType.Basic;

	[Header ("Start Mode")]
	public string whichMode = "";

	[HideInInspector]
	public int buttonIndex;
	[HideInInspector]
	public MenuComponent menuComponentParent;

	private Button button;
	private EventSystem eventSyst;

	// Use this for initialization
	void Awake () 
	{
		button = GetComponent<Button> ();
		menuComponentParent = transform.parent.GetComponent<MenuComponent> ();
		eventSyst = GameObject.FindGameObjectWithTag ("EventSystem").GetComponent<EventSystem> ();
	}

	public void OnPointerClick( PointerEventData data )
	{
		if(button.interactable)
		{
			menuComponentParent.Submit (buttonIndex);

			if(menuButtonType == MenuButtonType.StartMode)
				MenuManager.Instance.MenuLoadMode (whichMode);
		}
	}

	public void OnSubmit( BaseEventData data )
	{
		if(button.interactable)
		{
			eventSyst.SetSelectedGameObject (null);

			menuComponentParent.Submit (buttonIndex);

			if(menuButtonType == MenuButtonType.StartMode)
				MenuManager.Instance.MenuLoadMode (whichMode);

		}
	}
}
