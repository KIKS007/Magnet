using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;

public class ButtonOnSelected : MonoBehaviour
{
	public Image otherButton;

	private static float scaleOnSelected = 1.1f;

	private static float scaleOnDuration = 0.5f;

	public bool selected;

	public bool scaleChangement = true;

	public void OnSelect () 
	{
		selected = true;
		otherButton.color = new Color (3, 0, 255, 255) / 255;

		if(scaleChangement)
			otherButton.gameObject.GetComponent<RectTransform>().DOScale(scaleOnSelected, scaleOnDuration);
	}

	public void OnDeselect () 
	{
		selected = false;
		otherButton.color = new Color (255, 255, 255, 255) / 255;

		if(scaleChangement)
			otherButton.gameObject.GetComponent<RectTransform>().DOScale(1, scaleOnDuration);
	}
	
	public void OnPointerEnter () 
	{
		foreach(ButtonOnSelected buttonSelect in FindObjectsOfType<ButtonOnSelected>())
		{
			buttonSelect.OnDeselect ();
		}
		
		selected = true;
		otherButton.color = new Color (3, 0, 255, 255) / 255;
		
		if(scaleChangement)
			otherButton.gameObject.GetComponent<RectTransform>().DOScale(scaleOnSelected, scaleOnDuration);
	}

	/*
	public void OnSelect (BaseEventData eventData) 
	{
		selected = true;
		otherButton.color = new Color (3, 0, 255, 255) / 255;

		otherButton.gameObject.GetComponent<RectTransform>().DOScale(1.05f, 0.2f);
	}

	public void OnDeselect (BaseEventData eventData) 
	{
		selected = false;
		otherButton.color = new Color (255, 255, 255, 255) / 255;

		otherButton.gameObject.GetComponent<RectTransform>().DOScale(1, 0.2f);
	}

	public void OnSubmit (BaseEventData eventData)
	{

		selected = false;
		otherButton.color = new Color (255, 255, 255, 255) / 255;
		
		otherButton.gameObject.GetComponent<RectTransform>().DOScale(1, 0.2f);
	}



	public void OnPointerEnter (PointerEventData eventData) 
	{
		foreach(ButtonOnSelected buttonSelect in FindObjectsOfType<ButtonOnSelected>())
		{
			buttonSelect.Deselect ();
		}

		selected = true;
		otherButton.color = new Color (3, 0, 255, 255) / 255;
		
		otherButton.gameObject.GetComponent<RectTransform>().DOScale(1.05f, 0.2f);
	}

	public void OnPointerClick (PointerEventData eventData)
	{
		selected = false;
		otherButton.color = new Color (255, 255, 255, 255) / 255;
		
		otherButton.gameObject.GetComponent<RectTransform>().DOScale(1, 0.2f);
	}

	public void OnPointerExit (PointerEventData eventData) 
	{
		selected = false;
		otherButton.color = new Color (255, 255, 255, 255) / 255;
		
		otherButton.gameObject.GetComponent<RectTransform>().DOScale(1, 0.2f);
	}
	*/
}
