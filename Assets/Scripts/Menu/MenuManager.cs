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
		List<Player> playersListTemp = new List<Player> (ReInput.players.GetPlayers ());

		for(int i = 0; i < playersListTemp.Count; i++)
		{
			if (playersListTemp [i].GetButtonDown ("UI Cancel") && !tweening)
				currentMenu.Cancel ();
		}
	}

	//Main Menu
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

		currentMenu = mainMenu.GetComponent<MenuComponent> ();

		yield return null;			
	}



	//Sumbit Methods
	public void ShowUnderButtons (List<RectTransform> otherButtonsList, int submitButton, List<RectTransform> underButtonsList, MenuComponent whichMenu)
	{
		StartCoroutine (ShowUnderButtonsCoroutine (otherButtonsList, submitButton, underButtonsList, whichMenu));
	}

	IEnumerator ShowUnderButtonsCoroutine (List<RectTransform> otherButtonsList, int submitButton, List<RectTransform> underButtonsList, MenuComponent whichMenu)
	{
		yield return HideOtherButtons (otherButtonsList, submitButton).WaitForCompletion();			

		//Show Under Menu Buttons
		for (int i = 0; i < underButtonsList.Count; i++)
			underButtonsList [i].anchoredPosition = new Vector2 (offScreenX, buttonsYPositions [i]);

		int underDelay = 0;

		for(int i = underButtonsList.Count - 1; i >= 0; i--)
		{
			underButtonsList [i].DOAnchorPosX (onScreenX, durationSubmit).SetDelay (buttonsDelay [underDelay]).SetEase (easeMenu);
			underDelay++;
		}

		//Select First Under Menu Button
		underButtonsList [0].gameObject.GetComponent<Button> ().Select ();

		currentMenu = whichMenu;
	}

	public void ShowContent (List<RectTransform> otherButtonsList, int submitButton, RectTransform content, MenuComponent whichMenu, Selectable selectable = null)
	{
		StartCoroutine (ShowContentCoroutine (otherButtonsList, submitButton, content, whichMenu, selectable));
	}

	IEnumerator ShowContentCoroutine (List<RectTransform> otherButtonsList, int submitButton, RectTransform content, MenuComponent whichMenu, Selectable selectable = null)
	{
		yield return HideOtherButtons (otherButtonsList, submitButton).WaitForCompletion();			

		content.anchoredPosition = new Vector2 (offScreenX, 0);

		content.DOAnchorPosX (onScreenX, durationContent).SetEase (easeMenu);

		if(selectable != null)
			selectable.Select ();

		currentMenu = whichMenu;
	}

	Tween HideOtherButtons (List<RectTransform> otherButtonsList, int submitButton)
	{
		//Hide Other Buttons and Get Header Button At Top
		int aboveDelay = 0;

		for(int i = otherButtonsList.Count - 1; i >= 0; i--)
		{
			if(i != submitButton)
			{
				otherButtonsList [i].DOAnchorPosX (offScreenX, durationSubmit).SetDelay (buttonsDelay [aboveDelay]).SetEase (easeMenu);
				aboveDelay++;
			}
		}

		return otherButtonsList [submitButton].DOAnchorPos (new Vector2(onScreenX, headerButtonsYPosition), durationSubmit).SetDelay (buttonsDelay [aboveDelay]).SetEase (easeMenu);
	}



	//Cancel Methods
	public void HideUnderButtons (List<RectTransform> otherButtonsList, MenuComponent aboveMenu, RectTransform menuButton)
	{
		StartCoroutine (HiderUnderButtonsCoroutine (otherButtonsList, aboveMenu, menuButton));
	}

	IEnumerator HiderUnderButtonsCoroutine (List<RectTransform> otherButtonsList, MenuComponent aboveMenu, RectTransform menuButton)
	{
		int delay = 0;
		Tween tween = null;

		for(int i = otherButtonsList.Count - 1; i >= 0; i--)
		{
			tween = otherButtonsList [i].DOAnchorPosX (offScreenX, durationSubmit).SetDelay (buttonsDelay [delay]).SetEase (easeMenu);
			delay++;
		}

		yield return tween.WaitForCompletion();

		ShowAboveButtons (aboveMenu, menuButton);
	}

	public void HideContent (RectTransform content, MenuComponent aboveMenu, RectTransform menuButton)
	{
		StartCoroutine (HideContentCoroutine (content, aboveMenu, menuButton));
	}

	IEnumerator HideContentCoroutine (RectTransform content, MenuComponent aboveMenu, RectTransform menuButton)
	{
		Tween tween = content.DOAnchorPosX (offScreenX, durationContent).SetEase (easeMenu);

		yield return tween.WaitForCompletion();
	
		ShowAboveButtons (aboveMenu, menuButton);
	}
		
	void ShowAboveButtons (MenuComponent aboveMenu, RectTransform menuButton)
	{
		int cancelButton = 0;

		for (int i = 0; i < aboveMenu.underButtonsList.Count; i++)
			if (aboveMenu.underButtonsList [i] == menuButton)
				cancelButton = i;

		int delay = 0;

		aboveMenu.underButtonsList [cancelButton].DOAnchorPos (new Vector2(onScreenX, buttonsYPositions[cancelButton]), durationCancel).SetDelay (buttonsDelay [delay]).SetEase (easeMenu);
		delay++;

		for(int i = aboveMenu.underButtonsList.Count - 1; i >= 0; i--)
		{
			if(i != cancelButton)
			{
				aboveMenu.underButtonsList [i].DOAnchorPosX (onScreenX, durationCancel).SetDelay (buttonsDelay [delay]).SetEase (easeMenu);
				delay++;
			}
		}

		aboveMenu.button.DOAnchorPos (new Vector2(onScreenX, headerButtonsYPosition), durationCancel).SetDelay (buttonsDelay [delay]).SetEase (easeMenu);

		aboveMenu.underButtonsList [0].gameObject.GetComponent<Button> ().Select ();

		currentMenu = aboveMenu;
	}

	void Tweening ()
	{
		tweening = true;
	}

	void NotTweening ()
	{
		tweening = false;
	}
}
