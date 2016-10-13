using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;

public class ButtonOnSelected : EventTrigger
{
	[Header ("On Selected Feedback")]
	public bool selected;
	public bool scaleChangement = true;

	private Text text;

	private RectTransform otherButton;
	
	private static float scaleOnSelected = 1.1f;
	private static float scaleOnDuration = 0.5f;

	private EventSystem eventSys;

	private bool mainButton;

	private bool pointerDown = false;

	void Awake	 ()
	{
		eventSys = GameObject.FindGameObjectWithTag ("EventSystem").GetComponent<EventSystem> ();
		otherButton = GetComponent<RectTransform> ();
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

		if(otherButton.GetComponent<Button>().interactable == false)
			text.color = mainButton ? GlobalVariables.Instance.mainButtonHighlightedColorText : GlobalVariables.Instance.secondaryButtonIdleColorText;
	}

	void Update ()
	{
		if(otherButton.GetComponent<Button>().interactable == true && !pointerDown)
		{
			if(eventSys.currentSelectedGameObject == gameObject)
				text.color = mainButton ? GlobalVariables.Instance.mainButtonHighlightedColorText : GlobalVariables.Instance.secondaryButtonHighlightedColorText;

			else
				text.color = mainButton ? GlobalVariables.Instance.mainButtonIdleColorText : GlobalVariables.Instance.secondaryButtonIdleColorText;
		}

		if(otherButton.GetComponent<Button>().interactable == false)
			text.color = mainButton ? GlobalVariables.Instance.mainButtonHighlightedColorText : GlobalVariables.Instance.secondaryButtonIdleColorText;

		if(eventSys.currentSelectedGameObject != gameObject && otherButton.localScale != Vector3.one)
			otherButton.DOScale(1, scaleOnDuration);
	}

	public void OnSelect () 
	{
		selected = true;
			
		if(scaleChangement)
			otherButton.DOScale(scaleOnSelected, scaleOnDuration);
	}

	public void OnDeselect () 
	{
		selected = false;
		
		if(scaleChangement)
			otherButton.DOScale(1, scaleOnDuration);

	}
	
	public void OnPointerEnter () 
	{
		eventSys.SetSelectedGameObject (null);
		eventSys.SetSelectedGameObject (gameObject);
		
		selected = true;
		
		if(scaleChangement)
			otherButton.DOScale(scaleOnSelected, scaleOnDuration);
	}

	public override void OnPointerEnter(PointerEventData data )
	{
			OnPointerEnter ();
			GameSoundsManager.Instance.MenuNavigation ();
	}

	public override void OnPointerExit( PointerEventData data )
	{
		pointerDown = false;
		
		OnDeselect ();
	}

	public override void OnPointerClick( PointerEventData data )
	{
		if(otherButton.GetComponent<Button>().interactable == true)
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

	public override void OnDeselect( BaseEventData data )
	{
		OnDeselect ();
	}

	public override void OnSubmit( BaseEventData data )
	{
		if(otherButton.GetComponent<Button>().interactable == true)
		{
			OnDeselect ();
			
			text.color = mainButton ? GlobalVariables.Instance.mainButtonClickedColorText : GlobalVariables.Instance.secondaryClickedColorText;
		}
	}

	public override void OnPointerDown (PointerEventData eventData)
	{
		if(otherButton.GetComponent<Button>().interactable == true)
		{
			pointerDown = true;
			
			text.color = mainButton ? GlobalVariables.Instance.mainButtonClickedColorText : GlobalVariables.Instance.secondaryClickedColorText;
		}

	}

	public override void OnPointerUp (PointerEventData eventData)
	{
		if(otherButton.GetComponent<Button>().interactable == true)
		pointerDown = false;
	}
}
