﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;

public class MenuButtonAnimationsAndSounds : MonoBehaviour, IPointerClickHandler, ISelectHandler, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler, ISubmitHandler, IPointerDownHandler, IPointerUpHandler
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

	private Button buttonComponent;

	void Awake	 ()
	{
		eventSys = GameObject.FindGameObjectWithTag ("EventSystem").GetComponent<EventSystem> ();
		buttonRect = GetComponent<RectTransform> ();
		buttonComponent = GetComponent<Button> ();
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

		if(buttonComponent.interactable == false)
			text.color = mainButton ? GlobalVariables.Instance.mainButtonHighlightedColorText : GlobalVariables.Instance.secondaryButtonIdleColorText;
	}

	void Update ()
	{
		if(buttonComponent.interactable == true)
		{
			if(!pointerDown)
			{
				if(eventSys.currentSelectedGameObject == gameObject)
					text.color = mainButton ? GlobalVariables.Instance.mainButtonHighlightedColorText : GlobalVariables.Instance.secondaryButtonHighlightedColorText;

				else
					text.color = mainButton ? GlobalVariables.Instance.mainButtonIdleColorText : GlobalVariables.Instance.secondaryButtonIdleColorText;
			}

			if(eventSys.currentSelectedGameObject != gameObject && buttonRect.localScale != Vector3.one && !DOTween.IsTweening ("ResetScale" + GetInstanceID ()))
				buttonRect.DOScale(1, scaleOnDuration).SetId ("ResetScale" + GetInstanceID ());
		}

		else
			text.color = mainButton ? GlobalVariables.Instance.mainButtonIdleColorText : GlobalVariables.Instance.secondaryButtonIdleColorText;
	}

	public void OnSelect () 
	{
		selected = true;
			
		if(scaleChangement && !DOTween.IsTweening ("Select" + GetInstanceID ()))
			buttonRect.DOScale(scaleOnSelected, scaleOnDuration).SetId ("Select" + GetInstanceID ());

	}

	public void OnDeselect () 
	{
		selected = false;

		if(scaleChangement && !DOTween.IsTweening ("Deselect" + GetInstanceID ()))
			buttonRect.DOScale(1, scaleOnDuration).SetId ("Deselect" + GetInstanceID ());

	}



	//OnSelect Methods
	public void OnPointerClick( PointerEventData data )
	{
		if(buttonComponent.interactable == true)
		{
			VibrationManager.Instance.Vibrate (1, FeedbackType.ButtonClick);
			
			OnSelect ();

			text.color = mainButton ? GlobalVariables.Instance.mainButtonClickedColorText : GlobalVariables.Instance.secondaryClickedColorText;
		}
	}

	public void OnSelect( BaseEventData data )
	{
		OnSelect ();

		SoundsManager.Instance.MenuNavigation ();
	}


	public void OnPointerEnter(PointerEventData data )
	{
		if(buttonComponent.interactable == true)
		{
			eventSys.SetSelectedGameObject (null);
			eventSys.SetSelectedGameObject (gameObject);
			
			selected = true;			
		}

		if(scaleChangement && !DOTween.IsTweening ("Select" + GetInstanceID ()))
			buttonRect.DOScale(scaleOnSelected, scaleOnDuration).SetId ("Select" + GetInstanceID ());
		
		SoundsManager.Instance.MenuNavigation ();
	}

	public void OnPointerExit( PointerEventData data )
	{
		pointerDown = false;

		//eventSys.SetSelectedGameObject (null);

		OnDeselect ();
	}



	public void OnDeselect( BaseEventData data )
	{
		OnDeselect ();
	}

	public void OnSubmit( BaseEventData data )
	{
		if(buttonComponent.interactable == true)
		{
			VibrationManager.Instance.Vibrate (1, FeedbackType.ButtonClick);
			
			OnDeselect ();

			SoundsManager.Instance.MenuSubmit ();

			text.color = mainButton ? GlobalVariables.Instance.mainButtonClickedColorText : GlobalVariables.Instance.secondaryClickedColorText;
		}
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		if(buttonComponent.interactable == true)
		{
			pointerDown = true;

			text.color = mainButton ? GlobalVariables.Instance.mainButtonClickedColorText : GlobalVariables.Instance.secondaryClickedColorText;
		}

	}

	public void OnPointerUp (PointerEventData eventData)
	{
		if(buttonComponent.interactable == true)
			pointerDown = false;
	}
}
