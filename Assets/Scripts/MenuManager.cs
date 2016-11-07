using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DarkTonic.MasterAudio;
using Rewired;
using GameAnalyticsSDK;
using Rewired.UI.ControlMapper;
using System;

public class MenuManager : Singleton <MenuManager> 
{

	public bool test = false;

	[Header ("Ease")]
	public Ease easeMenu;

	[Header ("Animations Duration")]
	public float durationSubmit;
	public float durationCancel;
	public float durationContent;

	[Header ("Animations Delay")]
	public float[] delaySubmit;
	public float[] delayCancel;

	[Header ("Positions")]
	public float offScreenX = -1600;
	public float onScreenX = -650;
	public float screenCenterY = -200;
	public float gapBetweenButtons = 131;
	public float topYpositionButton = 540;
	public float[] yPositions = new float[9];

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(test)
		{
			test = false;
		}
	}

	Tween ButtonsSubmit (RectTransform[] buttonsList, int submitButton, Action OnCompleteAction = null)
	{
		int whichDelay = 0;

		for(int i = buttonsList.Length - 1; i >= 0; i--)
		{

			if(i != submitButton)
			{
				buttonsList [i].DOAnchorPosX (offScreenX, durationSubmit).SetDelay (delaySubmit [whichDelay]).SetEase (easeMenu);
				whichDelay++;
			}
		}

		if(OnCompleteAction == null)
			return buttonsList [submitButton].DOAnchorPos (new Vector2(onScreenX, topYpositionButton), durationSubmit).SetDelay (delaySubmit [whichDelay]).SetEase (easeMenu);
		else
			return buttonsList [submitButton].DOAnchorPos (new Vector2(onScreenX, topYpositionButton), durationSubmit).SetDelay (delaySubmit [whichDelay]).SetEase (easeMenu).OnComplete (()=> OnCompleteAction());
	}

	/*Tween ButtonsCancel (RectTransform[] buttonsList, int cancelButton, Action OnCompleteAction = null)
	{
		int whichDelay = 0;

		buttonsList [cancelButton].DOAnchorPos (new Vector2(onScreenX, yPositions[cancelButton]), durationCancel).SetDelay (delayCancel [whichDelay]).SetEase (easeMenu);

		for(int i = buttonsList.Length - 1; i >= 0; i--)
		{

			if(i != cancelButton)
			{
				buttonsList [i].DOAnchorPosX (offScreenX, durationCancel).SetDelay (delayCancel [whichDelay]).SetEase (easeMenu);
				whichDelay++;
			}
		}

		return buttonsList [cancelButton].DOAnchorPos (new Vector2(onScreenX, topYpositionButton), durationCancel).SetDelay (delayCancel [whichDelay]).SetEase (easeMenu);
	}*/
}
