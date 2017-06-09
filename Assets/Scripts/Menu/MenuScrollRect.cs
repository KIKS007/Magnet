using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Rewired;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;
using Sirenix.OdinInspector;
using System.Linq;

public class MenuScrollRect : SerializedMonoBehaviour
{
	[Header ("Settings")]
	public float centerDuration = 0.25f;
	public Ease centerEase = Ease.OutQuad;
	public float heightFactor = 1.2f;

	[Header ("Elements")]
	public Dictionary<int, RectTransform> elements = new Dictionary<int, RectTransform>();

	[Header ("Infos")]
	public float verticalPosition;

	private ScrollRect scrollRect;

	void Start ()
	{
		scrollRect = GetComponent<ScrollRect> ();

		scrollRect.onValueChanged.AddListener ((arg)=> verticalPosition = arg.y);
	}

	public void CenterButton (RectTransform button)
	{
		if (MenuManager.Instance.mouseControl)
			return;
		
		float movement = 1f - (float)elements.FirstOrDefault (x => x.Value == button).Key / (float)(elements.Count - 1);

		scrollRect.DOVerticalNormalizedPos (movement, centerDuration).SetEase (centerEase);
	}
}
