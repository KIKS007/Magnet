using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public enum MenuButtonType {Basic, StartMode};

public class MenuButtonComponent : MonoBehaviour, IPointerClickHandler, ISubmitHandler, ISelectHandler, IDeselectHandler
{
	public MenuButtonType menuButtonType = MenuButtonType.Basic;

	[Header ("Start Mode")]
	public WhichMode whichMode;

	[Header ("Secondary Content")]
	public bool showOnSelect = true;
	public bool showOnSubmit = false;
	public bool hideOnDeselect = true;
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
		hasBeenSubmit = false;
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

		if(button.interactable)
		{
			hasBeenSubmit = true;

			menuComponentParent.Submit (buttonIndex);
			menuComponentParent.aboveMenuScript.previousSelected = gameObject;

			if(showOnSubmit)
				ShowSecondaryContent ();

			if(menuButtonType == MenuButtonType.StartMode)
				MenuManager.Instance.MenuLoadMode (whichMode);

			hasBeenSubmit = false;
		}
	}

	public void OnSubmit( BaseEventData data )
	{
		if (MenuManager.Instance.isTweening)
			return;
		
		if(button.interactable)
		{
			hasBeenSubmit = true;

			eventSyst.SetSelectedGameObject (null);

			if(showOnSubmit)
				ShowSecondaryContent ();

			menuComponentParent.Submit (buttonIndex);

			if(menuComponentParent.menuComponentType == MenuComponentType.BasicMenu)
				menuComponentParent.aboveMenuScript.previousSelected = gameObject;

			if(menuButtonType == MenuButtonType.StartMode)
				MenuManager.Instance.MenuLoadMode (whichMode);

			hasBeenSubmit = false;
		}
	}

	public void OnSelect (BaseEventData eventData)
	{
		if (MenuManager.Instance.isTweening)
			return;

		if(menuComponentParent.menuComponentType == MenuComponentType.BasicMenu)
			menuComponentParent.aboveMenuScript.previousSelected = gameObject;

		if(showOnSelect)
			ShowSecondaryContent ();
	}

	public void OnDeselect (BaseEventData eventData)
	{
		if(hideOnDeselect)
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

			secondaryContentList [i].content.DOAnchorPos (secondaryContentList [i].onScreenPos, MenuManager.Instance.durationToShow).SetDelay (secondaryContentList [i].delay).SetEase (MenuManager.Instance.easeMenu).SetId ("MenuButton");
		}	
	}

	void HideSecondaryContent ()
	{
		if (secondaryContentList.Count == 0 || hasBeenSubmit)
			return;

		for(int i = 0; i < secondaryContentList.Count; i++)
		{
			StartCoroutine (DisableContent (MenuManager.Instance.durationToHide * 0.6f, secondaryContentList [i].content.gameObject));

			secondaryContentList [i].content.DOAnchorPos (secondaryContentList [i].offScreenPos, MenuManager.Instance.durationToHide * 0.6f).SetDelay (secondaryContentList [i].delay).SetEase (MenuManager.Instance.easeMenu).SetId ("MenuButton");
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
