 using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;

public enum MenuButtonType {Basic, StartMode, Static};

public class MenuButtonComponent : MonoBehaviour, IPointerClickHandler, ISubmitHandler, ISelectHandler, IDeselectHandler
{
	public MenuButtonType menuButtonType = MenuButtonType.Basic;

	[ShowIfAttribute ("StartMode")]
	[Header ("Start Mode")]
	public WhichMode whichMode;

	bool StartMode ()
	{
		return menuButtonType != MenuButtonType.Basic;
	}

	[ShowIfAttribute ("StaticButton")]
	[Header ("Show Menu")]
	public MenuComponent whichMenu;

	bool StaticButton ()
	{
		return menuButtonType == MenuButtonType.Static;
	}

	[Header ("Secondary Content")]
	public List<SecondaryContent> secondaryContentList;

	[HideInInspector]
	public int buttonIndex;
	[HideInInspector]
	public MenuComponent menuComponentParent;

	private Button button;
	private EventSystem eventSyst;

	private bool hasBeenSubmit = false;

	// Use this for initialization
	void Awake () 
	{
		button = GetComponent<Button> ();
		eventSyst = GameObject.FindGameObjectWithTag ("EventSystem").GetComponent<EventSystem> ();
	}

	void OnEnable ()
	{
		Setup ();
	}

	void Setup ()
	{
		if (menuComponentParent == null)
			menuComponentParent = transform.GetComponentInParent<MenuComponent> ();

		hasBeenSubmit = false;

		//Setup Secondary Content
		for(int i = 0; i < secondaryContentList.Count; i++)
		{
			secondaryContentList [i].content.anchoredPosition = secondaryContentList [i].offScreenPos;
			secondaryContentList [i].content.gameObject.SetActive (false);
		}
	}

	public void OnPointerClick( PointerEventData data )
	{
		if (MenuManager.Instance.isTweening)
			return;

		if (!button.interactable)
			return;

		hasBeenSubmit = true;
		
		if(menuButtonType != MenuButtonType.Static)
			menuComponentParent.Submit (buttonIndex);
		else
			menuComponentParent.Submit (whichMenu);
		
		menuComponentParent.aboveMenuScript.previousSelected = gameObject;
		
		ShowSecondaryContent ();
		
		if(menuButtonType != MenuButtonType.Basic)
			MenuManager.Instance.MenuLoadMode (whichMode);
		
		hasBeenSubmit = false;
	}

	public void OnSubmit( BaseEventData data )
	{
		if (MenuManager.Instance.isTweening)
			return;

		if (!button.interactable)
			return;

		hasBeenSubmit = true;
		
		eventSyst.SetSelectedGameObject (null);
		
		ShowSecondaryContent ();
		
		menuComponentParent.Submit (buttonIndex);
		
		if(menuComponentParent.menuComponentType == MenuComponentType.BasicMenu)
			menuComponentParent.aboveMenuScript.previousSelected = gameObject;
		
		if(menuButtonType != MenuButtonType.Basic)
			MenuManager.Instance.MenuLoadMode (whichMode);
		
		hasBeenSubmit = false;
	}

	public void OnSelect (BaseEventData eventData)
	{
		if (MenuManager.Instance.isTweening)
			return;

		if (!button.interactable)
			return;
		
		if(menuComponentParent.menuComponentType == MenuComponentType.BasicMenu)
			menuComponentParent.aboveMenuScript.previousSelected = gameObject;

		ShowSecondaryContent ();
	}

	public void OnDeselect (BaseEventData eventData)
	{
		HideSecondaryContent ();
	}

	void ShowSecondaryContent ()
	{
		if (secondaryContentList.Count == 0)
			return;
		
		for(int i = 0; i < secondaryContentList.Count; i++)
		{
			if(secondaryContentList [i].content.anchoredPosition != secondaryContentList [i].onScreenPos)
				secondaryContentList [i].content.anchoredPosition = secondaryContentList [i].offScreenPos;

			secondaryContentList [i].content.gameObject.SetActive (true);

			secondaryContentList [i].content.DOAnchorPos (secondaryContentList [i].onScreenPos, MenuManager.Instance.animationDuration).SetDelay (secondaryContentList [i].delay).SetEase (MenuManager.Instance.easeMenu).SetId ("MenuButton");
		}	
	}

	void HideSecondaryContent ()
	{
		if (secondaryContentList.Count == 0 || hasBeenSubmit)
			return;

		for(int i = 0; i < secondaryContentList.Count; i++)
		{
			StartCoroutine (DisableContent (MenuManager.Instance.animationDuration * 0.6f, secondaryContentList [i].content.gameObject));

			secondaryContentList [i].content.DOAnchorPos (secondaryContentList [i].offScreenPos, MenuManager.Instance.animationDuration * 0.6f).SetDelay (secondaryContentList [i].delay).SetEase (MenuManager.Instance.easeMenu).SetId ("MenuButton");
		}

		hasBeenSubmit = false;
	}

	IEnumerator DisableContent (float delayDuration, GameObject target)
	{
		yield return new WaitForSecondsRealtime (delayDuration);

		if(eventSyst.currentSelectedGameObject != gameObject)
			target.gameObject.SetActive (false);
	}
}
