using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;

public class MenuButtonAnimationsAndSounds : EventTrigger
{
	[Header ("On Selected Feedback")]
	public bool selected;
	public bool scaleChangement = true;

	private Text text;

	private RectTransform buttonRect;
	
	private static float scaleOnSelected = 1.1f;
	private static float scaleOnDuration = 0.5f;

	private EventSystem eventSys;

	private bool mainButton;

	private bool pointerDown = false;

	void Awake	 ()
	{
		eventSys = GameObject.FindGameObjectWithTag ("EventSystem").GetComponent<EventSystem> ();
		buttonRect = GetComponent<RectTransform> ();
		text = transform.GetChild (0).GetComponent<Text> ();

		if (tag == "MainButton")
		{
			mainButton = true;
			text.color = GlobalVariables.Instance.mainButtonIdleColorText;
		}
		else
		{
			mainButton = false;
			text.color = GlobalVariables.Instance.secondaryButtonIdleColorText;
		}

		if(buttonRect.GetComponent<Button>().interactable == false)
			text.color = mainButton ? GlobalVariables.Instance.mainButtonHighlightedColorText : GlobalVariables.Instance.secondaryButtonIdleColorText;
	}

	void Update ()
	{
		if(buttonRect.GetComponent<Button>().interactable == true && !pointerDown)
		{
			if(eventSys.currentSelectedGameObject == gameObject)
				text.color = mainButton ? GlobalVariables.Instance.mainButtonHighlightedColorText : GlobalVariables.Instance.secondaryButtonHighlightedColorText;

			else
				text.color = mainButton ? GlobalVariables.Instance.mainButtonIdleColorText : GlobalVariables.Instance.secondaryButtonIdleColorText;
		}

		if(buttonRect.GetComponent<Button>().interactable == false)
			text.color = mainButton ? GlobalVariables.Instance.mainButtonIdleColorText : GlobalVariables.Instance.secondaryButtonIdleColorText;

		if(eventSys.currentSelectedGameObject != gameObject && buttonRect.localScale != Vector3.one)
			buttonRect.DOScale(1, scaleOnDuration);
	}

	public void OnSelect () 
	{
		selected = true;
			
		if(scaleChangement)
			buttonRect.DOScale(scaleOnSelected, scaleOnDuration);
	}

	public void OnDeselect () 
	{
		selected = false;
		
		if(scaleChangement)
			buttonRect.DOScale(1, scaleOnDuration);

	}

	//OnSelect Methods
	public override void OnPointerClick( PointerEventData data )
	{
		if(buttonRect.GetComponent<Button>().interactable == true)
		{
			OnSelect ();

			text.color = mainButton ? GlobalVariables.Instance.mainButtonClickedColorText : GlobalVariables.Instance.secondaryClickedColorText;
		}
	}

	public override void OnSelect( BaseEventData data )
	{
		OnSelect ();

		GameSoundsManager.Instance.MenuNavigation ();
	}


	public override void OnPointerEnter(PointerEventData data )
	{
		eventSys.SetSelectedGameObject (null);
		eventSys.SetSelectedGameObject (gameObject);

		selected = true;

		if(scaleChangement)
			buttonRect.DOScale(scaleOnSelected, scaleOnDuration);
		
		GameSoundsManager.Instance.MenuNavigation ();
	}

	public override void OnPointerExit( PointerEventData data )
	{
		pointerDown = false;

		eventSys.SetSelectedGameObject (null);

		OnDeselect ();
	}



	public override void OnDeselect( BaseEventData data )
	{
		OnDeselect ();
	}

	public override void OnSubmit( BaseEventData data )
	{
		if(buttonRect.GetComponent<Button>().interactable == true)
		{
			OnDeselect ();

			GameSoundsManager.Instance.MenuSubmit ();

			text.color = mainButton ? GlobalVariables.Instance.mainButtonClickedColorText : GlobalVariables.Instance.secondaryClickedColorText;
		}
	}

	public override void OnPointerDown (PointerEventData eventData)
	{
		if(buttonRect.GetComponent<Button>().interactable == true)
		{
			pointerDown = true;

			text.color = mainButton ? GlobalVariables.Instance.mainButtonClickedColorText : GlobalVariables.Instance.secondaryClickedColorText;
		}

	}

	public override void OnPointerUp (PointerEventData eventData)
	{
		if(buttonRect.GetComponent<Button>().interactable == true)
		pointerDown = false;
	}
}
