using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

public class LogoKickSteam : EventTrigger 
{
	[Header ("On Selected Feedback")]
	public bool selected;
	public bool scaleChangement = true;

	private RectTransform otherButton;

	private static float scaleOnSelected = 1.1f;
	private static float scaleOnDuration = 0.5f;

	private EventSystem eventSys;

	void Start ()
	{
		eventSys = GameObject.FindGameObjectWithTag ("EventSystem").GetComponent<EventSystem> ();
		otherButton = GetComponent<RectTransform> ();
	}

	void Update ()
	{
		
	}

	void SelectNewButton ()
	{
		if(!eventSys.alreadySelecting)
			eventSys.SetSelectedGameObject (null);
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

		SelectNewButton ();
	}

	public void OnPointerEnter () 
	{
		eventSys.SetSelectedGameObject (null);
		eventSys.SetSelectedGameObject (gameObject);

		selected = true;

		if(scaleChangement)
			otherButton.DOScale(scaleOnSelected, scaleOnDuration);
	}

	public override void OnPointerEnter( PointerEventData data )
	{
		OnPointerEnter ();
		SoundsManager.Instance.MenuNavigation ();
	}

	public override void OnPointerExit( PointerEventData data )
	{
		OnDeselect ();
	}

	public override void OnPointerClick( PointerEventData data )
	{
		OnSelect ();
	}

	public override void OnSelect( BaseEventData data )
	{
		OnSelect ();
		SoundsManager.Instance.MenuNavigation ();
	}

	public override void OnDeselect( BaseEventData data )
	{
		OnDeselect ();
	}

	public override void OnSubmit( BaseEventData data )
	{
		OnDeselect ();
	}

	public void GetToURL (string url)
	{
		if(url != "")
		Application.OpenURL (url);
	}
}
