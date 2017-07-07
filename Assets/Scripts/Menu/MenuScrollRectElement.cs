using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuScrollRectElement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IScrollHandler
{
	private ScrollRect scrollRect;

	// Use this for initialization
	void Start () 
	{
		scrollRect = transform.GetComponentInParent<ScrollRect> ();
	}

	public void OnDrag (PointerEventData eventData)
	{
		scrollRect.OnDrag (eventData);
	}

	public void OnBeginDrag (PointerEventData eventData)
	{
		scrollRect.OnBeginDrag (eventData);
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		scrollRect.OnEndDrag (eventData);
	}

	public void OnScroll (PointerEventData eventData)
	{
		scrollRect.OnScroll (eventData);
	}
}
