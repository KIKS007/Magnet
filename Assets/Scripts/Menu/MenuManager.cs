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

public enum MenuComponentType {ContentMenu, ButtonsListMenu, MainMenu};

public class MenuManager : Singleton <MenuManager> 
{
	public bool tweening;

	public GameObject mainMenu;
	public MenuComponent currentMenu;

	private MenuComponent mainMenuScript;

	[Header ("Ease")]
	public Ease easeMenu = Ease.OutQuad;

	[Header ("Animations Duration")]
	public float durationSubmit = 0.15f;
	public float durationCancel = 0.15f;
	public float durationContent = 0.15f;

	[Header ("Buttons Delay")]
	public float[] buttonsDelay = {0, 0.05f, 0.1f, 0.15f, 0.2f, 0.25f};

	[Header ("Positions")]
	public float offScreenX = -1600;
	public float onScreenX = -650;

	[Header ("Buttons Positions")]
	public float headerButtonsYPosition = 540;
	public float gapAfterHeaderButton;
	public float gapBetweenButtons = 131;
	public float[] buttonsYPositions = new float[9];

	// Use this for initialization
	void Start () 
	{
		mainMenuScript = mainMenu.GetComponent<MenuComponent> ();

		SetupButtonsPositions ();

		MainMenu (mainMenuScript.underButtonsList);
	}

	void SetupButtonsPositions ()
	{
		buttonsYPositions [0] = headerButtonsYPosition - gapAfterHeaderButton;

		for (int i = 1; i < buttonsYPositions.Length; i++)
			buttonsYPositions [i] = buttonsYPositions [0] - (gapBetweenButtons * i);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void MainMenu (List<RectTransform> underButtonsList)
	{
		StartCoroutine (MainMenuCoroutine (underButtonsList));
	}

	IEnumerator MainMenuCoroutine (List<RectTransform> underButtonsList)
	{
		for (int i = 0; i < underButtonsList.Count; i++)
			underButtonsList [i].anchoredPosition = new Vector2 (offScreenX, buttonsYPositions [i]);

		int underDelay = 0;

		for(int i = underButtonsList.Count - 1; i >= 0; i--)
		{
			underButtonsList [i].DOAnchorPosX (onScreenX, durationSubmit).SetDelay (buttonsDelay [underDelay]).SetEase (easeMenu);
			underDelay++;
		}

		underButtonsList [0].GetComponent<Button> ().Select ();

		yield return null;			
	}

	public void ButtonsListSubmit (List<RectTransform> underButtonsList, List<RectTransform> aboveButtonsList = null, int submitButton = -1)
	{
		StartCoroutine (ButtonsListSubmitCoroutine (underButtonsList, aboveButtonsList, submitButton));
	}

	IEnumerator ButtonsListSubmitCoroutine (List<RectTransform> underButtonsList, List<RectTransform> aboveButtonsList = null, int submitButton = -1)
	{
		Debug.Log (aboveButtonsList.Count);

		if(aboveButtonsList != null)
		{
			int aboveDelay = 0;
			
			for(int i = aboveButtonsList.Count - 1; i >= 0; i--)
			{
				if(i != submitButton)
				{
					aboveButtonsList [i].DOAnchorPosX (offScreenX, durationSubmit).SetDelay (buttonsDelay [aboveDelay]).SetEase (easeMenu);
					aboveDelay++;
				}
			}
			
			Tween tween = aboveButtonsList [submitButton].DOAnchorPos (new Vector2(onScreenX, headerButtonsYPosition), durationSubmit).SetDelay (buttonsDelay [aboveDelay]).SetEase (easeMenu);
			
			yield return tween.WaitForCompletion();			
		}

		for (int i = 0; i < underButtonsList.Count; i++)
			underButtonsList [i].anchoredPosition = new Vector2 (offScreenX, buttonsYPositions [i]);

		int underDelay = 0;

		for(int i = underButtonsList.Count - 1; i >= 0; i--)
		{
			underButtonsList [i].DOAnchorPosX (onScreenX, durationSubmit).SetDelay (buttonsDelay [underDelay]).SetEase (easeMenu);
			underDelay++;
		}

		underButtonsList [0].GetComponent<Button> ().Select ();

		yield return null;			
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

	void Tweening ()
	{
		tweening = true;
	}

	void NotTweening ()
	{
		tweening = false;
	}
}
