﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DarkTonic.MasterAudio;
using Rewired;
using GameAnalyticsSDK;
using System;

public class MenuManager : Singleton <MenuManager> 
{
	#region Variables Declaration
	[Header ("Infos")]
	public bool isTweening;
	public bool menuTweening;
	public GameObject mainMenu;
	public MenuComponent currentMenu = null;

	private MenuComponent mainMenuScript;

	[Header ("Ease")]
	public Ease easeMenu = Ease.OutQuad;

	[Header ("Animations Duration")]
	public float durationToShow = 0.15f;
	public float durationToHide = 0.15f;
	public float durationContent = 0.15f;
	
	[Header ("Positions")]
	public float offScreenX = -1600;
	public float onScreenX = -650;

	[Header ("Selectable")]
	public bool selectPreviousElement = true;

	[Header ("Menu Element To Enable")]
	public List<GameObject> elementsToEnable;

	[Header ("MainMenu Buttons Positions")]
	public float mainMenuFirstButtonY = 100;
	
	[Header ("Buttons Positions")]
	public float firstButtonY = 100;
	public float gapBetweenButtons = 131;
	
	[Header ("Header Buttons")]
	public bool hidePreviousHeaderButton;
	public float headerButtonsYPosition = 540;
	public List<RectTransform> headerButtonsList;

	[Header ("Buttons Delay")]
	public float buttonsDelay = 0.05f;

	[Header ("Viewport Buttons Positions")]
	public float viewportYOffscreen;
	public float viewportButtonsDuration = 0.15f;
	public float viewportButtonsDelay = 0.05f;
	public Ease viewportButtonsMovementEase;

	[Header ("Back Buttons")]
	public RectTransform backButtons;
	public Vector2 backButtonsXPos;

	[Header ("Disconnected Gamepads")]
	public bool oneGamepadDisconnected = false;
	public RectTransform[] disconnectedGamepads = new RectTransform[4];
	public Vector2 disconnectedGamepadsYPos;

	[Header ("End Mode Menu")]
	public MenuComponent endModeMenu;
	public Vector2 initialPanelSize;
	public Vector2 modifiedPanelSize;
	public RectTransform[] playerScore = new RectTransform[4];
	public RectTransform panelBackground;
	public float delayBetweenStats = 0.01f ;

	private RectTransform endModecontent;
	private List<SecondaryContent> endModesecondaryContentList;

	private EventSystem eventSyst;
	private GameObject mainCamera;
	private LoadModeManager loadModeScript;
	private MenuCameraMovement cameraMovement;

	private bool startScreen = true;

	#endregion

	#region Setup
	void Start () 
	{
		DOTween.Init();
		DOTween.defaultTimeScaleIndependent = true;

		OnMenuChange += BackButtons;
		GlobalVariables.Instance.OnMenu += ResetGamepadsDisconnected;

		mainMenu.SetActive (true);
		mainMenuScript = mainMenu.GetComponent<MenuComponent> ();
			
		eventSyst = GameObject.FindGameObjectWithTag ("EventSystem").GetComponent<EventSystem> ();
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		cameraMovement = mainCamera.GetComponent<MenuCameraMovement> ();
		loadModeScript = GameObject.FindObjectOfType<LoadModeManager> ();

		backButtons.anchoredPosition = new Vector2(backButtonsXPos.x, backButtons.anchoredPosition.y);

		for (int i = 0; i < elementsToEnable.Count; i++)
			elementsToEnable [i].SetActive (true);
	}
		
	void StartScreen ()
	{
		MasterAudio.PlaySound (SoundsManager.Instance.gameStartSound);

		cameraMovement.StartCoroutine ("StartScreen");

		MainMenu ();

		StartCoroutine (OnMenuChangeEvent (mainMenuScript));
	}
	#endregion

	#region Update
	void Update () 
	{
		if(GlobalVariables.Instance.GameState != GameStateEnum.Playing)
		{
			CheckNothingSelected ();

			GamepadsDisconnected ();

			CheckMenuInput ();
		}

		CheckPauseInput ();

		if(DOTween.IsTweening ("Menu") || menuTweening)
			isTweening = true;
		else
			isTweening = false;
	}

	void CheckMenuInput ()
	{
		if(!isTweening && !DOTween.IsTweening ("MenuCamera"))
		{
			for(int i = 0; i < GlobalVariables.Instance.rewiredPlayers.Length; i++)
			{
				if(startScreen)
				{
					if(GlobalVariables.Instance.rewiredPlayers[i].GetButtonDown("UI Submit") || GlobalVariables.Instance.rewiredPlayers[i].GetButtonDown("UI Start") || Input.GetMouseButtonDown(0))
					{
						StartScreen ();
						startScreen = false;
					}
				}
				else
				{
					if (GlobalVariables.Instance.GameState != GameStateEnum.Playing && GlobalVariables.Instance.GameState != GameStateEnum.EndMode && GlobalVariables.Instance.rewiredPlayers [i].GetButtonDown ("UI Cancel"))
						currentMenu.Cancel ();
				}
			}			
		}
	}

	void CheckPauseInput ()
	{
		if(!isTweening && !DOTween.IsTweening ("MenuCamera"))
		{
			for(int i = 0; i < GlobalVariables.Instance.rewiredPlayers.Length; i++)
			{
				if (GlobalVariables.Instance.GameState == GameStateEnum.Paused && GlobalVariables.Instance.rewiredPlayers [i].GetButtonDown ("UI Start") && !oneGamepadDisconnected)
				{
					currentMenu.HideMenu ();
					PauseResumeGame ();
				}

				if (GlobalVariables.Instance.GameState == GameStateEnum.Playing && GlobalVariables.Instance.rewiredPlayers [i].GetButtonDown ("UI Start"))
					PauseResumeGame ();				
			}			
		}
	}
		
	public void ExitMenu ()
	{
		if(!isTweening)
			currentMenu.Cancel ();
	}

	void CheckNothingSelected ()
	{
		if(eventSyst.currentSelectedGameObject == null && currentMenu != null && !isTweening)
			SelectPreviousElement (currentMenu);
	}

	void GamepadsDisconnected ()
	{
		if (disconnectedGamepads [0].parent.gameObject.activeSelf == false)
			disconnectedGamepads [0].parent.gameObject.SetActive (true);

		if(GlobalVariables.Instance.GameState != GameStateEnum.Menu)
		{
			for(int i = 0; i < 4; i++)
			{
				if (GamepadsManager.Instance.gamepadsUnplugged [i] == true)
					oneGamepadDisconnected = true;
			}

			if(GamepadsManager.Instance.gamepadsUnplugged [0] == false && GamepadsManager.Instance.gamepadsUnplugged [1] == false && GamepadsManager.Instance.gamepadsUnplugged [2] == false && GamepadsManager.Instance.gamepadsUnplugged [3] == false)
				oneGamepadDisconnected = false;
		}		

		if (GlobalVariables.Instance.GameState == GameStateEnum.Paused)
		{
			for(int i = 0; i < 4; i++)
			{
				if(GamepadsManager.Instance.gamepadsUnplugged[i] == true && disconnectedGamepads[i].anchoredPosition.y != disconnectedGamepadsYPos.y && !DOTween.IsTweening("GamepadDisconnected" + i.ToString()))
					disconnectedGamepads[i].DOAnchorPosY (disconnectedGamepadsYPos.y, durationContent).SetEase (easeMenu).SetId("GamepadDisconnected" + i.ToString());
				

				if(GamepadsManager.Instance.gamepadsUnplugged[i] == false && disconnectedGamepads[i].anchoredPosition.y != disconnectedGamepadsYPos.x && !DOTween.IsTweening("GamepadDisconnected" + i.ToString()))
					disconnectedGamepads[i].DOAnchorPosY (disconnectedGamepadsYPos.x, durationContent).SetEase (easeMenu).SetId("GamepadDisconnected" + i.ToString());				
			}
		}
	}

	void ResetGamepadsDisconnected ()
	{
		for(int i = 0; i < 4; i++)
		{
			if(disconnectedGamepads[i].anchoredPosition.y != disconnectedGamepadsYPos.x && !DOTween.IsTweening("GamepadDisconnected" + i.ToString()))
			{
				disconnectedGamepads[i].DOAnchorPosY (disconnectedGamepadsYPos.x, durationContent).SetEase (easeMenu).SetId("GamepadDisconnected" + i.ToString());				

				if (GlobalVariables.Instance.PlayersControllerNumber[i] == i + 1)
					GlobalVariables.Instance.PlayersControllerNumber[i] = -1;
			}

		}
	}
	#endregion

	#region Main Menu		
	void MainMenu ()
	{
		List<RectTransform> underButtonsList = mainMenuScript.underMenusButtons;

		for (int i = 0; i < underButtonsList.Count; i++)
			underButtonsList [i].anchoredPosition = new Vector2 (offScreenX, MainMenuButtonsYPos (i));

		int underDelay = 0;

		for(int i = underButtonsList.Count - 1; i >= 0; i--)
		{
			Enable (underButtonsList [i]);
			SetInteractable (underButtonsList [i], durationToShow + ButtonsDelay (underDelay));

			underButtonsList [i].DOAnchorPosX (onScreenX, durationToShow).SetDelay (ButtonsDelay (underDelay)).SetEase (easeMenu).SetId ("Menu");
			underDelay++;
		}

		if(!startScreen)
			cameraMovement.StartCoroutine ("ShowLogo");

		SetInteractable (underButtonsList [0], ButtonsDelay (underDelay) * 0.5f);
		DOVirtual.DelayedCall (ButtonsDelay (underDelay), ()=> SelectPreviousElement (mainMenuScript)); 

		currentMenu = mainMenuScript;
	}

	void MainMenu (RectTransform menuButton)
	{
		List<RectTransform> underButtonsList = mainMenuScript.underMenusButtons;

		int cancelButton = 0;

		for (int i = 0; i < underButtonsList.Count; i++)
			if (underButtonsList [i] == menuButton)
				cancelButton = i;		

		int delay = 0;

		SetInteractable (underButtonsList [cancelButton], durationToShow);
		underButtonsList [cancelButton].DOAnchorPos (new Vector2(onScreenX, MainMenuButtonsYPos(cancelButton)), durationToShow).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");

		headerButtonsList.RemoveAt (headerButtonsList.Count - 1);

		delay++;

		for(int i = underButtonsList.Count - 1; i >= 0; i--)
		{
			if(i != cancelButton)
			{
				Enable (underButtonsList [i]);
				SetInteractable (underButtonsList [i], durationToShow + ButtonsDelay (delay));

				underButtonsList [i].DOAnchorPosX (onScreenX, durationToShow).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
				delay++;
			}
		}

		if(!startScreen)
			cameraMovement.StartCoroutine ("ShowLogo");

		SetInteractable (underButtonsList [0], ButtonsDelay (delay) * 0.5f);
		DOVirtual.DelayedCall (ButtonsDelay (delay), ()=> SelectPreviousElement (mainMenuScript)); 

		currentMenu = mainMenuScript;
	}

	public void HideMainMenu ()
	{
		List<RectTransform> underButtonsList = mainMenuScript.underMenusButtons;

		int underDelay = 0;

		for(int i = underButtonsList.Count - 1; i >= 0; i--)
		{
			Disable (underButtonsList [i], durationToHide + ButtonsDelay (underDelay));
			SetNonInteractable (underButtonsList [i]);

			underButtonsList [i].DOAnchorPosX (offScreenX, durationToHide).SetDelay (ButtonsDelay (underDelay)).SetEase (easeMenu).SetId ("Menu");
			underDelay++;
		}

		cameraMovement.StartCoroutine ("HideLogo");

		currentMenu = null;
	}
	#endregion		

	#region Submit Methods
	public void ShowUnderButtons (List<RectTransform> otherButtonsList, int submitButton, List<RectTransform> underButtonsList, MenuComponent whichMenu, List<SecondaryContent> secondaryContentList = null)
	{
		StartCoroutine (ShowUnderButtonsCoroutine (otherButtonsList, submitButton, underButtonsList, whichMenu, secondaryContentList));
	}

	IEnumerator ShowUnderButtonsCoroutine (List<RectTransform> otherButtonsList, int submitButton, List<RectTransform> underButtonsList, MenuComponent whichMenu, List<SecondaryContent> secondaryContentList = null)
	{
		PlaySubmitSound ();

		yield return HideOtherButtons (otherButtonsList, submitButton, whichMenu).WaitForCompletion();			

		//Show Under Menu Buttons
		for (int i = 0; i < underButtonsList.Count; i++)
			underButtonsList [i].anchoredPosition = new Vector2 (offScreenX, ButtonsYPos (i) + GapAfterHeaderButton ());

		int underDelay = 0;

		for(int i = underButtonsList.Count - 1; i >= 0; i--)
		{
			Enable (underButtonsList [i]);
			SetInteractable (underButtonsList [i], durationToShow + ButtonsDelay (underDelay));

			underButtonsList [i].DOAnchorPosX (onScreenX, durationToShow).SetDelay (ButtonsDelay (underDelay)).SetEase (easeMenu).SetId ("Menu");
			underDelay++;
		}

		//Secondary Content
		if(secondaryContentList != null)
		{
			for(int i = 0; i < secondaryContentList.Count; i++)
			{
				if(secondaryContentList [i].content.anchoredPosition != secondaryContentList [i].onScreenPos)
					secondaryContentList [i].content.anchoredPosition = secondaryContentList [i].offScreenPos;
				
				Enable (secondaryContentList [i].content);
				secondaryContentList [i].content.DOAnchorPos (secondaryContentList [i].onScreenPos, durationToShow).SetDelay (secondaryContentList [i].delay).SetEase (easeMenu).SetId ("Menu");
			}			
		}

		//Select First Under Menu Button
		SetInteractable (underButtonsList [0], ButtonsDelay (underDelay) * 0.5f);
		DOVirtual.DelayedCall (ButtonsDelay (underDelay), ()=> SelectPreviousElement (whichMenu)); 

		currentMenu = whichMenu;
	}

	public void ShowContent (List<RectTransform> otherButtonsList, int submitButton, RectTransform content, MenuComponent whichMenu, List<SecondaryContent> secondaryContentList = null)
	{
		StartCoroutine (ShowContentCoroutine (otherButtonsList, submitButton, content, whichMenu, secondaryContentList));
	}

	IEnumerator ShowContentCoroutine (List<RectTransform> otherButtonsList, int submitButton, RectTransform content, MenuComponent whichMenu, List<SecondaryContent> secondaryContentList = null)
	{
		PlaySubmitSound ();

		yield return HideOtherButtons (otherButtonsList, submitButton, whichMenu).WaitForCompletion();			

		content.anchoredPosition = new Vector2 (offScreenX, content.anchoredPosition.y);
		Enable (content);

		content.DOAnchorPosX (0, durationContent).SetEase (easeMenu).SetId ("Menu");

		//Secondary Content
		if(secondaryContentList != null)
		{
			for(int i = 0; i < secondaryContentList.Count; i++)
			{
				if(secondaryContentList [i].content.anchoredPosition != secondaryContentList [i].onScreenPos)
					secondaryContentList [i].content.anchoredPosition = secondaryContentList [i].offScreenPos;
				
				Enable (secondaryContentList [i].content);
				secondaryContentList [i].content.DOAnchorPos (secondaryContentList [i].onScreenPos, durationToShow).SetDelay (secondaryContentList [i].delay).SetEase (easeMenu).SetId ("Menu");
			}			
		}

		SelectPreviousElement (whichMenu);

		currentMenu = whichMenu;
	}

	Tween HideOtherButtons (List<RectTransform> otherButtonsList, int submitButton, MenuComponent whichMenu)
	{
		//Secondary Content
		if(whichMenu.secondaryContents.Count > 0)
		{
			for(int i = 0; i < whichMenu.secondaryContents.Count; i++)
			{
				Disable (whichMenu.secondaryContents [i].content, durationToHide + whichMenu.secondaryContents [i].delay);
				whichMenu.secondaryContents [i].content.DOAnchorPos (whichMenu.secondaryContents [i].offScreenPos, durationToHide).SetDelay (whichMenu.secondaryContents [i].delay).SetEase (easeMenu).SetId ("Menu");
			}
		}

		//Hide Other Buttons and Get Header Button At Top
		int aboveDelay = 0;

		for(int i = otherButtonsList.Count - 1; i >= 0; i--)
		{
			if(i != submitButton)
			{
				Disable (otherButtonsList [i], durationToHide + ButtonsDelay (aboveDelay));
				SetNonInteractable (otherButtonsList [i]);

				otherButtonsList [i].DOAnchorPosX (offScreenX, durationToHide).SetDelay (ButtonsDelay (aboveDelay)).SetEase (easeMenu).SetId ("Menu");
				aboveDelay++;
			}
		}

		if(hidePreviousHeaderButton)
		{
			if(headerButtonsList.Count > 0)
			{
				headerButtonsList [headerButtonsList.Count - 1].DOAnchorPosX (offScreenX, durationToHide).SetDelay (ButtonsDelay (aboveDelay)).SetEase (easeMenu).SetId ("Menu");
				headerButtonsList.RemoveAt (headerButtonsList.Count - 1);
			}

			if(whichMenu.aboveMenuScript.viewportContent)
				otherButtonsList [submitButton].transform.SetParent (whichMenu.aboveMenuScript.transform);
		}

		SetNonInteractable (otherButtonsList [submitButton]);
		return otherButtonsList [submitButton].DOAnchorPos (new Vector2(onScreenX, HeaderButtonPosition ()), durationToShow).SetDelay (ButtonsDelay (aboveDelay)).SetEase (easeMenu).OnComplete (()=> headerButtonsList.Add (otherButtonsList [submitButton])).SetId ("Menu");
	}

	public void ShowViewportButtons (List<RectTransform> otherButtonsList, int submitButton, List<RectTransform> underButtonsList, MenuComponent whichMenu, List<SecondaryContent> secondaryContentList = null)
	{
		StartCoroutine (ShowViewportButtonsCoroutine (otherButtonsList, submitButton, underButtonsList, whichMenu, secondaryContentList));	
	}

	IEnumerator ShowViewportButtonsCoroutine (List<RectTransform> otherButtonsList, int submitButton, List<RectTransform> underButtonsList, MenuComponent whichMenu, List<SecondaryContent> secondaryContentList = null)
	{
		PlaySubmitSound ();

		yield return HideOtherButtons (otherButtonsList, submitButton, whichMenu).WaitForCompletion();			

		//Show Under Menu Buttons
		for (int i = 0; i < underButtonsList.Count; i++)
			underButtonsList [i].anchoredPosition = new Vector2 (onScreenX, viewportYOffscreen);

		int underDelay = 0;

		for(int i = underButtonsList.Count - 1; i >= 0; i--)
		{
			Enable (underButtonsList [i]);
			SetInteractable (underButtonsList [i], viewportButtonsDuration + ButtonsDelay (underDelay));

			if(whichMenu.underMenusButtonsPositions.Count > 0)
				underButtonsList [i].DOAnchorPosY (whichMenu.underMenusButtonsPositions [i].y, viewportButtonsDuration).SetDelay (ViewportButtonsDelay (underDelay)).SetEase (easeMenu).SetId ("Menu");
			else
				underButtonsList [i].DOAnchorPosY (ButtonsYPos (i) + GapAfterHeaderButton (), viewportButtonsDuration).SetDelay (ViewportButtonsDelay (underDelay)).SetEase (easeMenu).SetId ("Menu");

			underDelay++;
		}

		//Secondary Content
		if(secondaryContentList != null)
		{
			for(int i = 0; i < secondaryContentList.Count; i++)
			{
				if(secondaryContentList [i].content.anchoredPosition != secondaryContentList [i].onScreenPos)
					secondaryContentList [i].content.anchoredPosition = secondaryContentList [i].offScreenPos;

				Enable (secondaryContentList [i].content);
				secondaryContentList [i].content.DOAnchorPos (secondaryContentList [i].onScreenPos, viewportButtonsDuration).SetDelay (secondaryContentList [i].delay).SetEase (easeMenu).SetId ("Menu");
			}			
		}

		//Select First Under Menu Button
		SetInteractable (underButtonsList [0], ButtonsDelay (underDelay) * 0.5f);
		DOVirtual.DelayedCall (ButtonsDelay (underDelay), ()=> SelectPreviousElement (whichMenu)); 

		currentMenu = whichMenu;
	}


	public void SubmitMenu (MenuComponent whichMenu, int submitButton)
	{
		StartCoroutine (SubmitMenuCoroutine (whichMenu, submitButton));
	}

	IEnumerator SubmitMenuCoroutine (MenuComponent whichMenu, int submitButton)
	{
		Debug.Log ("---------------------");

		float timeTemp = Time.time;

		menuTweening = true;
		PlaySubmitSound ();
		yield return StartCoroutine (HideMenuCoroutine (whichMenu.aboveMenuScript, submitButton));
		yield return StartCoroutine (ShowMenuCoroutine (whichMenu));
		menuTweening = false;

		Debug.Log ("-------- SUBMIT Menu : " + whichMenu.name + " took " + (Time.time - timeTemp));
	}

	public void CancelMenu (MenuComponent whichMenu, int cancelButton)
	{
		StartCoroutine (CancelMenuCoroutine (whichMenu, cancelButton));
	}

	IEnumerator CancelMenuCoroutine (MenuComponent whichMenu, int cancelButton)
	{
		Debug.Log ("---------------------");
		float timeTemp = Time.time;

		menuTweening = true;
		PlayReturnSound ();
		yield return StartCoroutine (HideMenuCoroutine (whichMenu));
		yield return StartCoroutine (ShowMenuCoroutine (whichMenu.aboveMenuScript, cancelButton));
		menuTweening = false;

		Debug.Log ("-------- CANCEL Menu : " + whichMenu.name + " took " + (Time.time - timeTemp));
	}


	public void HideMenu (MenuComponent whichMenu)
	{
		StartCoroutine (HideMenuCoroutine (whichMenu));
	}

	public void ShowMenu (MenuComponent whichMenu)
	{
		StartCoroutine (ShowMenuCoroutine (whichMenu));
	}


	IEnumerator ShowMenuCoroutine (MenuComponent whichMenu, int cancelButton = -1)
	{
		float timeTemp = Time.time;
		float timeTemp2 = Time.time;

		currentMenu = whichMenu;

		for(int i = 0; i < whichMenu.contentDisplay.Count; i++)
		{
			switch (whichMenu.contentDisplay [i].contentType)
			{
			case MenuContentType.Menus:
				timeTemp2 = Time.time;
				if(cancelButton != -1)
					yield return StartCoroutine (ShowUnderMenus (whichMenu, i, cancelButton));
				else
					yield return StartCoroutine (ShowUnderMenusSolo (whichMenu, i));
				Debug.Log ("Show Under Menus : " + whichMenu.name + " took " + (Time.time - timeTemp2));
				break;
			case MenuContentType.Buttons:
				yield return StartCoroutine (ShowUnderButtons (whichMenu, i));
				break;
			case MenuContentType.MainContent:
				timeTemp2 = Time.time;
				yield return StartCoroutine (ShowMainContent (whichMenu, i));
				Debug.Log ("Show Content : " + whichMenu.name + " took " + (Time.time - timeTemp2));
				break;
			case MenuContentType.SecondaryContent:
				yield return StartCoroutine (ShowUnderButtons (whichMenu, i));
				break;
			}
		}

		//Select First Under Menu Button
		SelectPreviousElement (whichMenu); 

		Debug.Log ("SHOW Menu : " + whichMenu.name + " took " + (Time.time - timeTemp));
	}

	IEnumerator ShowUnderMenus (MenuComponent whichMenu, int contentIndex, int cancelButton = -1)
	{
		int delay = 0;

		//Wait delay
		if (whichMenu.contentDisplay.Count > 0 && whichMenu.contentDisplay [contentIndex].delay > 0)
			yield return new WaitForSeconds (whichMenu.contentDisplay [contentIndex].delay);

		//Show Under Menus Buttons
		for(int i = whichMenu.underMenusButtons.Count - 1; i >= 0; i--)
		{
			if(i != cancelButton)
				whichMenu.underMenusButtons [i].anchoredPosition = new Vector2 (offScreenX, ButtonsYPos (i) + GapAfterHeaderButton ());

			Enable (whichMenu.underMenusButtons [i]);
			SetInteractable (whichMenu.underMenusButtons [i], durationToShow + ButtonsDelay (delay));
			
			if(whichMenu.underMenusButtonsPositions.Count > 0)
				whichMenu.underMenusButtons [i].DOAnchorPos (whichMenu.underMenusButtonsPositions [i], durationToShow).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
			else
				whichMenu.underMenusButtons [i].DOAnchorPos (new Vector2 (onScreenX, ButtonsYPos (i) + GapAfterHeaderButton ()), durationToShow).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
			
			delay++;
		}

		//Remove Current Header From List
		headerButtonsList.RemoveAt (headerButtonsList.Count - 1);

		//Show Previous Header Button
		if(whichMenu.menuButton != null)
		{
			Enable (whichMenu.menuButton);
			SetInteractable (whichMenu.menuButton, durationToShow);
			
			if(hidePreviousHeaderButton)
			{
				if(whichMenu.menuButton != null)
				{
					whichMenu.menuButton.DOAnchorPos (new Vector2(onScreenX, HeaderButtonPosition ()), durationToShow).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
					headerButtonsList.Add (whichMenu.menuButton);
				}
				
				/*if(whichMenu.aboveMenuScript.viewportContent)
				{
					whichMenu.underMenusButtons [cancelButton].SetParent (whichMenu.underMenusButtons [cancelButton].GetComponent<MenuButtonComponent> ().menuComponentParent.transform);
					whichMenu.underMenusButtons [cancelButton].SetAsFirstSibling ();
				}*/
			}
		}

		//Wait
		if(contentIndex == whichMenu.contentDisplay.Count - 1 || whichMenu.contentDisplay [contentIndex + 1].waitPreviousContent)
			yield return new WaitForSeconds (durationToShow + ButtonsDelay (delay));
		else
			yield break;
	}

	IEnumerator ShowUnderMenusSolo (MenuComponent whichMenu, int contentIndex)
	{
		//Wait delay
		if (whichMenu.contentDisplay.Count > 0 && whichMenu.contentDisplay [contentIndex].delay > 0)
			yield return new WaitForSeconds (whichMenu.contentDisplay [contentIndex].delay);

		//Show Under Menu Buttons
		int delay = 0;

		for(int i = whichMenu.underMenusButtons.Count - 1; i >= 0; i--)
		{
			whichMenu.underMenusButtons [i].anchoredPosition = new Vector2 (offScreenX, ButtonsYPos (i) + GapAfterHeaderButton ());

			Enable (whichMenu.underMenusButtons [i]);
			SetInteractable (whichMenu.underMenusButtons [i], durationToShow + ButtonsDelay (delay));

			if(whichMenu.underMenusButtonsPositions.Count > 0)
				whichMenu.underMenusButtons [i].DOAnchorPos (whichMenu.underMenusButtonsPositions [i], durationToShow).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
			else
				whichMenu.underMenusButtons [i].DOAnchorPos (new Vector2 (onScreenX, ButtonsYPos (i) + GapAfterHeaderButton ()), durationToShow).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");

			delay++;
		}

		//Wait
		if(contentIndex == whichMenu.contentDisplay.Count - 1 || whichMenu.contentDisplay [contentIndex + 1].waitPreviousContent)
			yield return new WaitForSeconds (durationToShow + ButtonsDelay (delay));
		else
			yield break;
	}
		
	IEnumerator ShowUnderButtons (MenuComponent whichMenu, int contentIndex)
	{
		//Wait delay
		if (whichMenu.contentDisplay.Count > 0 && whichMenu.contentDisplay [contentIndex].delay > 0)
			yield return new WaitForSeconds (whichMenu.contentDisplay [contentIndex].delay);

		//Show Under Menu Buttons
		int underDelay = 0;

		for(int i = whichMenu.underButtons.Count - 1; i >= 0; i--)
		{
			whichMenu.underButtons [i].anchoredPosition = new Vector2 (offScreenX, ButtonsYPos (i) + GapAfterHeaderButton ());

			Enable (whichMenu.underButtons [i]);
			SetInteractable (whichMenu.underButtons [i], durationToShow + ButtonsDelay (underDelay));

			if(whichMenu.underButtonsPositions.Count > 0)
				whichMenu.underButtons [i].DOAnchorPos (whichMenu.underButtonsPositions [i], durationToShow).SetDelay (ButtonsDelay (underDelay)).SetEase (easeMenu).SetId ("Menu");
			else
				whichMenu.underButtons [i].DOAnchorPos (new Vector2 (onScreenX, ButtonsYPos (i) + GapAfterHeaderButton ()), durationToShow).SetDelay (ButtonsDelay (underDelay)).SetEase (easeMenu).SetId ("Menu");
			
			underDelay++;
		}

		//Wait
		if(contentIndex == whichMenu.contentDisplay.Count - 1 || whichMenu.contentDisplay [contentIndex + 1].waitPreviousContent)
			yield return new WaitForSeconds (durationToShow + ButtonsDelay (underDelay));
		else
			yield break;
	}

	IEnumerator ShowMainContent (MenuComponent whichMenu, int contentIndex)
	{
		//Wait delay
		if (whichMenu.contentDisplay.Count > 0 && whichMenu.contentDisplay [contentIndex].delay > 0)
			yield return new WaitForSeconds (whichMenu.contentDisplay [contentIndex].delay);

		whichMenu.mainContent.anchoredPosition = new Vector2 (offScreenX, whichMenu.mainContent.anchoredPosition.y);
		Enable (whichMenu.mainContent);

		whichMenu.mainContent.DOAnchorPosX (0, durationContent).SetEase (easeMenu).SetId ("Menu");

		//Wait
		if(contentIndex == whichMenu.contentDisplay.Count - 1 || whichMenu.contentDisplay [contentIndex + 1].waitPreviousContent)
			yield return new WaitForSeconds (durationContent);
		else
			yield break;
	}

	IEnumerator ShowSecondaryContent (MenuComponent whichMenu, int contentIndex)
	{
		//Wait delay
		if (whichMenu.contentDisplay.Count > 0 && whichMenu.contentDisplay [contentIndex].delay > 0)
			yield return new WaitForSeconds (whichMenu.contentDisplay [contentIndex].delay);

		//Secondary Content
		if(whichMenu.secondaryContents != null)
		{
			for(int i = 0; i < whichMenu.secondaryContents.Count; i++)
			{
				if(whichMenu.secondaryContents [i].content.anchoredPosition != whichMenu.secondaryContents [i].onScreenPos)
					whichMenu.secondaryContents [i].content.anchoredPosition = whichMenu.secondaryContents [i].offScreenPos;

				Enable (whichMenu.secondaryContents [i].content);
				whichMenu.secondaryContents [i].content.DOAnchorPos (whichMenu.secondaryContents [i].onScreenPos, durationToShow).SetDelay (whichMenu.secondaryContents [i].delay).SetEase (easeMenu).SetId ("Menu");
			}			
		}

		//Wait
		if(contentIndex == whichMenu.contentDisplay.Count - 1 || whichMenu.contentDisplay [contentIndex + 1].waitPreviousContent)
			yield return new WaitForSeconds (durationToShow + whichMenu.secondaryContents [whichMenu.secondaryContents.Count - 1].delay);
		else
			yield break;
	}
		
	void HidePreviousHeader (MenuComponent whichMenu, int submitButton, int delay)
	{
		//Hide Previous Header Button
		if(hidePreviousHeaderButton)
		{
			if(headerButtonsList.Count > 0)
			{
				headerButtonsList [headerButtonsList.Count - 1].DOAnchorPosX (offScreenX, durationToHide).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
				headerButtonsList.RemoveAt (headerButtonsList.Count - 1);
			}

			if(whichMenu.aboveMenuScript && whichMenu.aboveMenuScript.viewportContent)
				whichMenu.underMenusButtons [submitButton].transform.SetParent (whichMenu.aboveMenuScript.transform);
		}
	}

	void ShowPreviousHeader (MenuComponent whichMenu, int cancelButton)
	{
		//Enable Previous Header
		Enable (whichMenu.aboveMenuScript.underMenusButtons [cancelButton]);
		SetInteractable (whichMenu.aboveMenuScript.underMenusButtons [cancelButton], durationToShow);

		//Show Previous Header Button
		if(hidePreviousHeaderButton)
		{
			if(whichMenu.aboveMenuScript.menuButton != null)
			{
				whichMenu.aboveMenuScript.menuButton.DOAnchorPos (new Vector2(onScreenX, HeaderButtonPosition ()), durationToShow).SetDelay (0).SetEase (easeMenu).SetId ("Menu");
				headerButtonsList.Add (whichMenu.aboveMenuScript.menuButton);
			}

			if(whichMenu.aboveMenuScript.viewportContent)
			{
				whichMenu.underMenusButtons [cancelButton].SetParent (whichMenu.underMenusButtons [cancelButton].GetComponent<MenuButtonComponent> ().menuComponentParent.transform);
				whichMenu.underMenusButtons [cancelButton].SetAsFirstSibling ();
			}
		}
	}

	IEnumerator PlaceHeaderButton ()
	{
		yield break;

		/*Enable (whichMenu.underMenusButtons [i]);
		SetInteractable (whichMenu.underMenusButtons [i], durationToShow + ButtonsDelay (underDelay));

		if(whichMenu.underMenusButtonsPositions.Count > 0)
			whichMenu.underMenusButtons [i].DOAnchorPos (whichMenu.underMenusButtonsPositions [i], durationToShow).SetDelay (ButtonsDelay (underDelay)).SetEase (easeMenu).SetId ("Menu");
		else
			whichMenu.underMenusButtons [i].DOAnchorPos (new Vector2 (onScreenX, ButtonsYPos (i) + GapAfterHeaderButton ()), durationToShow).SetDelay (ButtonsDelay (underDelay)).SetEase (easeMenu).SetId ("Menu");*/
	}




	IEnumerator HideMenuCoroutine (MenuComponent whichMenu, int submitButton = -1)
	{
		float timeTemp = Time.time;

		for(int i = whichMenu.contentDisplay.Count - 1; i >= 0; i--)
		{
			switch (whichMenu.contentDisplay [i].contentType)
			{
			case MenuContentType.Menus:
				if(submitButton != -1)
					yield return StartCoroutine (HideUnderMenus (whichMenu, i, submitButton));
				else
					yield return StartCoroutine (HideUnderMenusSolo (whichMenu, i));
				break;
			case MenuContentType.Buttons:
				yield return StartCoroutine (HideUnderButton (whichMenu, i));
				break;
			case MenuContentType.MainContent:
				yield return StartCoroutine (HideMainContent (whichMenu, i));
				break;
			case MenuContentType.SecondaryContent:
				yield return StartCoroutine (HideSecondaryContent (whichMenu, i));
				break;
			}
		}

		Debug.Log ("HIDE Menu : " + whichMenu.name + " took " + (Time.time - timeTemp));
	}

	IEnumerator HideUnderMenus (MenuComponent whichMenu, int contentIndex, int submitButton)
	{
		//Wait delay
		if (whichMenu.contentDisplay.Count > 0 && whichMenu.contentDisplay [contentIndex].delay > 0)
			yield return new WaitForSeconds (whichMenu.contentDisplay [contentIndex].delay);

		int delay = 0;

		//Under Menus Buttons
		for(int i = whichMenu.underMenusButtons.Count - 1; i >= 0; i--)
		{
			if(i != submitButton)
			{
				Disable (whichMenu.underMenusButtons [i], durationToHide + ButtonsDelay (delay));
				SetNonInteractable (whichMenu.underMenusButtons [i]);
				
				whichMenu.underMenusButtons [i].DOAnchorPosX (offScreenX, durationToHide).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
				delay++;
			}
		}

		//Hide Previous Header Button
		if(hidePreviousHeaderButton)
		{
			if(headerButtonsList.Count > 0)
			{
				headerButtonsList [headerButtonsList.Count - 1].DOAnchorPosX (offScreenX, durationToHide).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
				headerButtonsList.RemoveAt (headerButtonsList.Count - 1);
			}

			/*if(whichMenu.aboveMenuScript && whichMenu.aboveMenuScript.viewportContent)
				whichMenu.underMenusButtons [submitButton].transform.SetParent (whichMenu.aboveMenuScript.transform);*/
		}

		//Place Submit Button as Header
		SetNonInteractable (whichMenu.underMenusButtons [submitButton]);
		Tween tween = whichMenu.underMenusButtons [submitButton].DOAnchorPos (new Vector2(onScreenX, HeaderButtonPosition ()), durationToShow).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).OnComplete (()=> headerButtonsList.Add (whichMenu.underMenusButtons [submitButton])).SetId ("Menu");


		//Wait
		if(contentIndex == 0 || whichMenu.contentDisplay [contentIndex].waitPreviousContent)
			yield return tween.WaitForCompletion ();
		else
			yield break;
	}

	IEnumerator HideUnderMenusSolo (MenuComponent whichMenu, int contentIndex)
	{
		//Wait delay
		if (whichMenu.contentDisplay.Count > 0 && whichMenu.contentDisplay [contentIndex].delay > 0)
			yield return new WaitForSeconds (whichMenu.contentDisplay [contentIndex].delay);

		int delay = 0;

		//Under Buttons
		for(int i = whichMenu.underMenusButtons.Count - 1; i >= 0; i--)
		{
			Disable (whichMenu.underMenusButtons [i], durationToHide + ButtonsDelay (delay));
			SetNonInteractable (whichMenu.underMenusButtons [i]);
			
			whichMenu.underMenusButtons [i].DOAnchorPosX (offScreenX, durationToHide).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
			delay++;
		}

		//Wait
		if(contentIndex == 0 || whichMenu.contentDisplay [contentIndex].waitPreviousContent)
			yield return new WaitForSeconds (durationToHide + ButtonsDelay (delay));
		else
			yield break;
	}

	IEnumerator HideUnderButton (MenuComponent whichMenu, int contentIndex)
	{
		//Wait delay
		if (whichMenu.contentDisplay.Count > 0 && whichMenu.contentDisplay [contentIndex].delay > 0)
			yield return new WaitForSeconds (whichMenu.contentDisplay [contentIndex].delay);
		
		int delay = 0;

		//Under Buttons
		for(int i = whichMenu.underButtons.Count - 1; i >= 0; i--)
		{
			Disable (whichMenu.underButtons [i], durationToHide + ButtonsDelay (delay));
			SetNonInteractable (whichMenu.underButtons [i]);

			whichMenu.underButtons [i].DOAnchorPosX (offScreenX, durationToHide).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
			delay++;
		}

		//Wait
		if(contentIndex == 0 || whichMenu.contentDisplay [contentIndex].waitPreviousContent)
			yield return new WaitForSeconds (durationToHide + ButtonsDelay (delay));
		else
			yield break;
	}

	IEnumerator HideMainContent (MenuComponent whichMenu, int contentIndex)
	{
		//Wait delay
		if (whichMenu.contentDisplay.Count > 0 && whichMenu.contentDisplay [contentIndex].delay > 0)
			yield return new WaitForSeconds (whichMenu.contentDisplay [contentIndex].delay);
		
		whichMenu.mainContent.DOAnchorPosX (offScreenX, durationContent).SetEase (easeMenu).SetId ("Menu").OnComplete (()=> Disable(whichMenu.mainContent));

		//Wait
		if(contentIndex == 0 || whichMenu.contentDisplay [contentIndex].waitPreviousContent)
			yield return new WaitForSeconds (durationContent);
		else
			yield break;
	}

	IEnumerator HideSecondaryContent (MenuComponent whichMenu, int contentIndex)
	{
		//Wait delay
		if (whichMenu.contentDisplay.Count > 0 && whichMenu.contentDisplay [contentIndex].delay > 0)
			yield return new WaitForSeconds (whichMenu.contentDisplay [contentIndex].delay);
		
		//Secondary Content
		if(whichMenu.secondaryContents != null)
		{
			for(int i = 0; i < whichMenu.secondaryContents.Count; i++)
			{
				Disable (whichMenu.secondaryContents [i].content, durationToHide + whichMenu.secondaryContents [i].delay);
				whichMenu.secondaryContents [i].content.DOAnchorPos (whichMenu.secondaryContents [i].offScreenPos, durationToHide).SetDelay (whichMenu.secondaryContents [i].delay).SetEase (easeMenu).SetId ("Menu");
			}			
		}

		//Wait
		if(contentIndex == 0 || whichMenu.contentDisplay [contentIndex].waitPreviousContent)
			yield return new WaitForSeconds (durationToHide + whichMenu.secondaryContents [whichMenu.secondaryContents.Count - 1].delay);
		else
			yield break;
	}
	#endregion

	#region Cancel Methods
	public void HideUnderButtons (List<RectTransform> underButtonsList, MenuComponent aboveMenu, RectTransform menuButton, bool resume = false, List<SecondaryContent> secondaryContentList = null)
	{
		if(!resume)
			StartCoroutine (HideUnderButtonsCoroutine (underButtonsList, aboveMenu, menuButton, secondaryContentList));
		else
			StartCoroutine (HideUnderButtonsResumeCoroutine (underButtonsList, aboveMenu, menuButton, secondaryContentList));
	}

	IEnumerator HideUnderButtonsCoroutine (List<RectTransform> underButtonsList, MenuComponent aboveMenu, RectTransform menuButton, List<SecondaryContent> secondaryContentList = null)
	{
		int delay = 0;
		Tween tween = null;

		PlayReturnSound ();

		//Secondary Content
		if(secondaryContentList != null)
		{
			for(int i = 0; i < secondaryContentList.Count; i++)
			{
				Disable (secondaryContentList [i].content, durationToHide + secondaryContentList [i].delay);
				secondaryContentList [i].content.DOAnchorPos (secondaryContentList [i].offScreenPos, durationToHide).SetDelay (secondaryContentList [i].delay).SetEase (easeMenu).SetId ("Menu");
			}			
		}

		//Under Buttons
		for(int i = underButtonsList.Count - 1; i >= 0; i--)
		{
			Disable (underButtonsList [i], durationToHide + ButtonsDelay (delay));
			SetNonInteractable (underButtonsList [i]);

			tween = underButtonsList [i].DOAnchorPosX (offScreenX, durationToHide).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
			delay++;
		}

		yield return tween.WaitForCompletion();

		if (aboveMenu.menuComponentType == MenuComponentType.MainMenu)
			MainMenu (menuButton);
		else
		{
			if(!aboveMenu.viewportContent)
				StartCoroutine (ShowAboveMenu (aboveMenu, menuButton));
			else
				StartCoroutine (ShowAboveViewportMenu (aboveMenu, menuButton));
		}

	}

	IEnumerator HideUnderButtonsResumeCoroutine (List<RectTransform> underButtonsList, MenuComponent aboveMenu, RectTransform menuButton, List<SecondaryContent> secondaryContentList = null)
	{
		int delay = 0;
		Tween tween = null;

		PlayReturnSound ();

		//Secondary Content
		if(secondaryContentList != null)
		{
			for(int i = 0; i < secondaryContentList.Count; i++)
			{
				Disable (secondaryContentList [i].content, durationToHide + secondaryContentList [i].delay);
				secondaryContentList [i].content.DOAnchorPos (secondaryContentList [i].offScreenPos, durationToHide).SetDelay (secondaryContentList [i].delay).SetEase (easeMenu).SetId ("Menu");
			}
		}
		
		for(int i = underButtonsList.Count - 1; i >= 0; i--)
		{
			Disable (underButtonsList [i], durationToHide + ButtonsDelay (delay));
			SetNonInteractable (underButtonsList [i]);

			tween = underButtonsList [i].DOAnchorPosX (offScreenX, durationToHide).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
			delay++;
		}

		yield return tween.WaitForCompletion();

		HideHeaderButtons ();

		currentMenu = null;

	}

	public void HideContent (RectTransform content, MenuComponent aboveMenu, RectTransform menuButton, bool resume = false, List<SecondaryContent> secondaryContentList = null)
	{
		if(!resume)
			StartCoroutine (HideContentCoroutine (content, aboveMenu, menuButton, secondaryContentList));
		else
			StartCoroutine (HideContentResumeCoroutine (content, aboveMenu, menuButton, secondaryContentList));
	}

	IEnumerator HideContentCoroutine (RectTransform content, MenuComponent aboveMenu, RectTransform menuButton, List<SecondaryContent> secondaryContentList = null)
	{
		Tween tween = content.DOAnchorPosX (offScreenX, durationContent).SetEase (easeMenu).SetId ("Menu").OnComplete (()=> Disable(content));

		PlayReturnSound ();

		//Secondary Content
		if(secondaryContentList != null)
		{
			for(int i = 0; i < secondaryContentList.Count; i++)
			{
				Disable (secondaryContentList [i].content, durationContent + secondaryContentList [i].delay);
				secondaryContentList [i].content.DOAnchorPos (secondaryContentList [i].offScreenPos, durationContent).SetDelay (secondaryContentList [i].delay).SetEase (easeMenu).SetId ("Menu");
			}
		}

		yield return tween.WaitForCompletion();
	
		if (aboveMenu.menuComponentType == MenuComponentType.MainMenu)
			MainMenu (menuButton);
		else
		{
			if(!aboveMenu.viewportContent)
				StartCoroutine (ShowAboveMenu (aboveMenu, menuButton));
			else
				StartCoroutine (ShowAboveViewportMenu (aboveMenu, menuButton));
		}

	}

	IEnumerator HideContentResumeCoroutine (RectTransform content, MenuComponent aboveMenu, RectTransform menuButton, List<SecondaryContent> secondaryContentList = null)
	{
		Tween tween = content.DOAnchorPosX (offScreenX, durationContent).SetEase (easeMenu).SetId ("Menu").OnComplete (()=> Disable(content));

		PlayReturnSound ();

		//Secondary Content
		if(secondaryContentList != null)
		{
			for(int i = 0; i < secondaryContentList.Count; i++)
			{
				Disable (secondaryContentList [i].content, durationContent + secondaryContentList [i].delay);
				secondaryContentList [i].content.DOAnchorPos (secondaryContentList [i].offScreenPos, durationContent).SetDelay (secondaryContentList [i].delay).SetEase (easeMenu).SetId ("Menu");
			}			
		}

		yield return tween.WaitForCompletion();

		HideHeaderButtons ();

		currentMenu = null;
	}
		
	public void HideViewportButtons (List<RectTransform> underButtonsList, MenuComponent aboveMenu, RectTransform menuButton, bool resume = false, List<SecondaryContent> secondaryContentList = null)
	{
		if(!resume)
			StartCoroutine (HideViewportButtons (underButtonsList, aboveMenu, menuButton, secondaryContentList));
		else
			StartCoroutine (HideViewportButtonsResume (underButtonsList, aboveMenu, menuButton, secondaryContentList));
	}

	IEnumerator HideViewportButtons (List<RectTransform> underButtonsList, MenuComponent aboveMenu, RectTransform menuButton, List<SecondaryContent> secondaryContentList = null)
	{
		int delay = 0;
		Tween tween = null;

		PlayReturnSound ();

		//Secondary Content
		if(secondaryContentList != null)
		{
			for(int i = 0; i < secondaryContentList.Count; i++)
			{
				Disable (secondaryContentList [i].content, viewportButtonsDuration + secondaryContentList [i].delay);
				secondaryContentList [i].content.DOAnchorPos (secondaryContentList [i].offScreenPos, viewportButtonsDuration).SetDelay (secondaryContentList [i].delay).SetEase (easeMenu).SetId ("Menu");
			}			
		}

		//Under Buttons
		for(int i = 0; i < underButtonsList.Count; i++)
		{
			Disable (underButtonsList [i], viewportButtonsDuration + ViewportButtonsDelay (delay));
			SetNonInteractable (underButtonsList [i]);

			tween = underButtonsList [i].DOAnchorPosY (viewportYOffscreen, viewportButtonsDuration).SetDelay (ViewportButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
			delay++;
		}

		yield return tween.WaitForCompletion();

		if (aboveMenu.menuComponentType == MenuComponentType.MainMenu)
			MainMenu (menuButton);
		else
		{
			if(!aboveMenu.viewportContent)
				StartCoroutine (ShowAboveMenu (aboveMenu, menuButton));
			else
				StartCoroutine (ShowAboveViewportMenu (aboveMenu, menuButton));
		}
	}

	IEnumerator HideViewportButtonsResume (List<RectTransform> underButtonsList, MenuComponent aboveMenu, RectTransform menuButton, List<SecondaryContent> secondaryContentList = null)
	{
		int delay = 0;
		Tween tween = null;

		PlayReturnSound ();

		//Secondary Content
		if(secondaryContentList != null)
		{
			for(int i = 0; i < secondaryContentList.Count; i++)
			{
				Disable (secondaryContentList [i].content, viewportButtonsDuration + secondaryContentList [i].delay);
				secondaryContentList [i].content.DOAnchorPos (secondaryContentList [i].offScreenPos, viewportButtonsDuration).SetDelay (secondaryContentList [i].delay).SetEase (easeMenu).SetId ("Menu");
			}			
		}

		//Under Buttons
		for(int i = 0; i < underButtonsList.Count; i++)
		{
			Disable (underButtonsList [i], viewportButtonsDuration + ViewportButtonsDelay (delay));
			SetNonInteractable (underButtonsList [i]);

			tween = underButtonsList [i].DOAnchorPosY (viewportYOffscreen, viewportButtonsDuration).SetDelay (ViewportButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
			delay++;
		}

		yield return tween.WaitForCompletion();

		HideHeaderButtons ();

		currentMenu = null;
	}

	IEnumerator ShowAboveMenu (MenuComponent aboveMenu, RectTransform menuButton)
	{
		int cancelButton = 0;

		for (int i = 0; i < aboveMenu.underMenusButtons.Count; i++)
			if (aboveMenu.underMenusButtons [i] == menuButton)
				cancelButton = i;

		int delay = 0;

		SetInteractable (aboveMenu.underMenusButtons [cancelButton], durationToShow + ButtonsDelay (delay));
		headerButtonsList.RemoveAt (headerButtonsList.Count - 1);

		if(hidePreviousHeaderButton)
		{
			if(aboveMenu.menuButton != null)
			{
				aboveMenu.menuButton.DOAnchorPos (new Vector2(onScreenX, HeaderButtonPosition ()), durationToShow).SetDelay (0).SetEase (easeMenu).SetId ("Menu");
				headerButtonsList.Add (aboveMenu.menuButton);
			}

			if(aboveMenu.viewportContent)
			{
				aboveMenu.underMenusButtons [cancelButton].SetParent (aboveMenu.underMenusButtons [cancelButton].GetComponent<MenuButtonComponent> ().menuComponentParent.transform);
				aboveMenu.underMenusButtons [cancelButton].SetAsFirstSibling ();
			}
		}

		Tween tween = aboveMenu.underMenusButtons [cancelButton].DOAnchorPos (new Vector2(onScreenX, ButtonsYPos (cancelButton) + GapAfterHeaderButton ()), durationToShow).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");

		yield return tween.WaitForCompletion ();

		//Secondary Content
		if(aboveMenu.secondaryContents != null)
		{
			for(int i = 0; i < aboveMenu.secondaryContents.Count; i++)
			{
				if(aboveMenu.secondaryContents [i].content.anchoredPosition != aboveMenu.secondaryContents [i].onScreenPos)
					aboveMenu.secondaryContents [i].content.anchoredPosition = aboveMenu.secondaryContents [i].offScreenPos;

				Enable (aboveMenu.secondaryContents [i].content);
				aboveMenu.secondaryContents [i].content.DOAnchorPos (aboveMenu.secondaryContents [i].onScreenPos, durationToShow).SetDelay (aboveMenu.secondaryContents [i].delay).SetEase (easeMenu).SetId ("Menu");
			}			
		}


		delay++;

		for(int i = aboveMenu.underMenusButtons.Count - 1; i >= 0; i--)
		{
			if(i != cancelButton)
			{
				aboveMenu.underMenusButtons [i].anchoredPosition = new Vector2(offScreenX, ButtonsYPos (i) + GapAfterHeaderButton ());

				Enable (aboveMenu.underMenusButtons [i]);
				SetInteractable (aboveMenu.underMenusButtons [i], durationToShow + ButtonsDelay (delay));

				aboveMenu.underMenusButtons [i].DOAnchorPosX (onScreenX, durationToShow).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
				delay++;
			}
		}

		SetInteractable (aboveMenu.underMenusButtons [0], ButtonsDelay (delay) * 0.5f);
		DOVirtual.DelayedCall (ButtonsDelay (delay), ()=> SelectPreviousElement (aboveMenu)); 

		currentMenu = aboveMenu;
	}

	IEnumerator ShowAboveViewportMenu (MenuComponent aboveMenu, RectTransform menuButton)
	{
		int cancelButton = 0;

		for (int i = 0; i < aboveMenu.underMenusButtons.Count; i++)
			if (aboveMenu.underMenusButtons [i] == menuButton)
				cancelButton = i;

		int delay = 0;

		SetInteractable (aboveMenu.underMenusButtons [cancelButton], viewportButtonsDuration + ViewportButtonsDelay (delay));
		headerButtonsList.RemoveAt (headerButtonsList.Count - 1);

		if(hidePreviousHeaderButton)
		{
			if(aboveMenu.menuButton != null)
			{
				aboveMenu.menuButton.DOAnchorPos (new Vector2(onScreenX, HeaderButtonPosition ()), durationToShow).SetDelay (0).SetEase (easeMenu).SetId ("Menu");
				headerButtonsList.Add (aboveMenu.menuButton);
			}

			if(aboveMenu.viewportContent)
			{
				aboveMenu.underMenusButtons [cancelButton].SetParent (aboveMenu.underMenusButtons [cancelButton].GetComponent<MenuButtonComponent> ().menuComponentParent.transform);
				aboveMenu.underMenusButtons [cancelButton].SetAsFirstSibling ();
			}
		}

		Tween tween = null;

		if(aboveMenu.viewportContent && aboveMenu.underMenusButtonsPositions.Count > 0)
			tween = aboveMenu.underMenusButtons [cancelButton].DOAnchorPos (new Vector2(onScreenX, aboveMenu.underMenusButtonsPositions [cancelButton].y), viewportButtonsDuration).SetDelay (ViewportButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");

		else
			tween = aboveMenu.underMenusButtons [cancelButton].DOAnchorPos (new Vector2(onScreenX, ButtonsYPos (cancelButton) + GapAfterHeaderButton ()), viewportButtonsDuration).SetDelay (ViewportButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");


		yield return tween.WaitForCompletion ();

		//Secondary Content
		if(aboveMenu.secondaryContents != null)
		{
			for(int i = 0; i < aboveMenu.secondaryContents.Count; i++)
			{
				if(aboveMenu.secondaryContents [i].content.anchoredPosition != aboveMenu.secondaryContents [i].onScreenPos)
					aboveMenu.secondaryContents [i].content.anchoredPosition = aboveMenu.secondaryContents [i].offScreenPos;

				Enable (aboveMenu.secondaryContents [i].content);
				aboveMenu.secondaryContents [i].content.DOAnchorPos (aboveMenu.secondaryContents [i].onScreenPos, durationToShow).SetDelay (aboveMenu.secondaryContents [i].delay).SetEase (easeMenu).SetId ("Menu");
			}			
		}

		delay++;

		for(int i = aboveMenu.underMenusButtons.Count - 1; i >= 0; i--)
		{
			if(i != cancelButton)
			{
				if(aboveMenu.viewportContent && aboveMenu.underMenusButtonsPositions.Count > 0)
					aboveMenu.underMenusButtons [i].DOAnchorPos (new Vector2(onScreenX, aboveMenu.underMenusButtonsPositions [i].y), viewportButtonsDuration).SetDelay (ViewportButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
				else
					aboveMenu.underMenusButtons [i].anchoredPosition = new Vector2(offScreenX, ButtonsYPos (i) + GapAfterHeaderButton ());

				Enable (aboveMenu.underMenusButtons [i]);
				SetInteractable (aboveMenu.underMenusButtons [i], viewportButtonsDuration + ViewportButtonsDelay (delay));

				aboveMenu.underMenusButtons [i].DOAnchorPosX (onScreenX, viewportButtonsDuration).SetDelay (ViewportButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
				delay++;
			}
		}

		SetInteractable (aboveMenu.underMenusButtons [0], ButtonsDelay (delay) * 0.5f);
		DOVirtual.DelayedCall (ButtonsDelay (delay), ()=> SelectPreviousElement (aboveMenu)); 

		currentMenu = aboveMenu;
	}

	void HideHeaderButtons ()
	{
		int delay = 0;

		for(int i = headerButtonsList.Count - 1; i >= 0; i--)
		{
			Disable (headerButtonsList [i], durationToHide + ButtonsDelay (delay));
			SetNonInteractable (headerButtonsList [i]);

			headerButtonsList [i].DOAnchorPosX (offScreenX, durationToHide).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
			delay++;
		}

		headerButtonsList.Clear ();
	}

	#endregion

	#region Other Methods
	public float HeaderButtonPosition ()
	{
		return headerButtonsYPosition - gapBetweenButtons * headerButtonsList.Count;
	}

	public float GapAfterHeaderButton ()
	{
		return (-gapBetweenButtons * headerButtonsList.Count) + gapBetweenButtons;
	}

	public float ButtonsDelay (int i)
	{
		return buttonsDelay * i;
	}

	public float ViewportButtonsDelay (int i)
	{
		return viewportButtonsDelay * i;
	}

	public float ButtonsYPos (int i)
	{
		return firstButtonY - (gapBetweenButtons * i);
	}

	public float MainMenuButtonsYPos (int i)
	{
		return mainMenuFirstButtonY - (gapBetweenButtons * i);
	}

	public void Enable (RectTransform target)
	{
		target.gameObject.SetActive (true);
	}

	public void Disable (RectTransform target, float delayDuration = 0)
	{
		if (delayDuration == 0)
			target.gameObject.SetActive (false);
		else
			StartCoroutine (DisableDelay (target, delayDuration));
	}

	IEnumerator DisableDelay (RectTransform target, float delayDuration)
	{
		yield return new WaitForSecondsRealtime (delayDuration);

		target.gameObject.SetActive (false);
	}


	void SelectPreviousElement (MenuComponent whichMenu)
	{
		GameObject selectable = null;

		if (whichMenu.previousSelected != null && selectPreviousElement)
			selectable = whichMenu.previousSelected;

		else if(whichMenu.selectable != null)
			selectable = whichMenu.selectable;
		
		if(eventSyst.currentSelectedGameObject != selectable)
			eventSyst.SetSelectedGameObject (null);

		if(selectable != null)
		{
			if(selectable.GetComponent<Button> () != null && selectable.GetComponent<Button> ().interactable)
				eventSyst.SetSelectedGameObject (selectable);

			else if(selectable.GetComponent<Button> () == null)
				eventSyst.SetSelectedGameObject (selectable);
		}
	}

	void SetInteractable (RectTransform target, float delayDuration = 0)
	{
		StartCoroutine (SetInteractableCoroutine (target, delayDuration));
	}

	IEnumerator SetInteractableCoroutine (RectTransform target, float delayDuration)
	{
		yield return new WaitForSecondsRealtime (delayDuration);

		target.GetComponent<Button> ().interactable = true;

		Navigation navTemp = target.GetComponent<Button> ().navigation;
		navTemp.mode = Navigation.Mode.Explicit;

		target.GetComponent<Button> ().navigation = navTemp;
	}

	void SetNonInteractable (RectTransform target)
	{
		target.GetComponent<Button> ().interactable = false;

		//Weird Bug Patch
		target.GetComponent<Button> ().enabled = false;
		target.GetComponent<Button> ().enabled = true;

		Navigation navTemp = target.GetComponent<Button> ().navigation;
		navTemp.mode = Navigation.Mode.None;

		target.GetComponent<Button> ().navigation = navTemp;
	}


	void PlaySubmitSound ()
	{
		MasterAudio.PlaySound (SoundsManager.Instance.menuSubmit);
	}

	void PlayReturnSound ()
	{
		MasterAudio.PlaySound (SoundsManager.Instance.menuCancel);
	}


	void BackButtons ()
	{
		if(GlobalVariables.Instance.GameState != GameStateEnum.Playing 
			&& currentMenu 
			&& currentMenu.menuComponentType != MenuComponentType.MainMenu 
			&& currentMenu.menuComponentType != MenuComponentType.EndMode)
		{
			if (backButtons.gameObject.activeSelf == false)
				backButtons.gameObject.SetActive (true);

			if (backButtons.anchoredPosition.x != backButtonsXPos.y)
				backButtons.DOAnchorPosX (backButtonsXPos.y, durationContent).SetEase (easeMenu);
		}
		else
		{
			if (backButtons.anchoredPosition.x != backButtonsXPos.x)
				backButtons.DOAnchorPosX (backButtonsXPos.x, durationContent).SetEase (easeMenu).OnComplete (()=> backButtons.gameObject.SetActive (false));
		}
	}

	public void QuitGame ()
	{
		Application.Quit ();
	}
	#endregion

	#region Play Resume
	public void PauseResumeGame ()
	{
		StartCoroutine (PauseResumeGameCoroutine ());
	}

	IEnumerator PauseResumeGameCoroutine ()
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			GlobalVariables.Instance.GameState = GameStateEnum.Paused;

			MasterAudio.PlaySound (SoundsManager.Instance.openMenuSound);

			mainCamera.GetComponent<SlowMotionCamera> ().StartPauseSlowMotion ();
			cameraMovement.StartCoroutine ("PausePosition");

			yield return new WaitForSecondsRealtime(cameraMovement.movementDuration - 0.5f);

			MainMenu ();
		}
		else
		{
  			MasterAudio.PlaySound (SoundsManager.Instance.closeMenuSound);

			yield return cameraMovement.StartCoroutine ("PlayPosition");

			mainCamera.GetComponent<SlowMotionCamera> ().StopPauseSlowMotion ();

			GlobalVariables.Instance.GameState = GameStateEnum.Playing;
		}
	}
	#endregion

	#region StartMode
	public void MenuLoadMode (WhichMode whichMode)
	{
		LoadModeManager.Instance.LoadSceneVoid (whichMode);
	}

	public void StartMode ()
	{
		switch (GlobalVariables.Instance.ModeSequenceType)
		{
		case ModeSequenceType.Selection:
			StartCoroutine (StartModeCoroutine ());
			break;
		case ModeSequenceType.Random:
			StartCoroutine (StartRandomModeCoroutine ());
			break;
		case ModeSequenceType.Cocktail:
			break;
		}

	}

	IEnumerator StartModeCoroutine ()
	{
		mainCamera.GetComponent<SlowMotionCamera> ().StopPauseSlowMotion ();

		currentMenu.HideMenu ();

		yield return new WaitForSecondsRealtime(durationToHide);

		MasterAudio.PlaySound (SoundsManager.Instance.closeMenuSound);

		yield return cameraMovement.StartCoroutine ("PlayPosition");

		GlobalVariables.Instance.GameState = GameStateEnum.Playing;
	}

	IEnumerator StartRandomModeCoroutine ()
	{
		mainCamera.GetComponent<SlowMotionCamera> ().StopPauseSlowMotion ();
		currentMenu.HideMenu ();

		LoadModeManager.Instance.LoadRandomSceneVoid ();

		yield return new WaitForSeconds (cameraMovement.loadingMovementDuration * 2);

		yield return new WaitForSecondsRealtime(durationToHide);

		MasterAudio.PlaySound (SoundsManager.Instance.closeMenuSound);

		yield return cameraMovement.StartCoroutine ("PlayPosition");

		GlobalVariables.Instance.GameState = GameStateEnum.Playing;
	}
	#endregion

	#region EndMode
	public void ShowEndMode (RectTransform content, List<SecondaryContent> secondaryContentList, MenuComponent whichMenu)
	{
		endModecontent = content;
		endModesecondaryContentList = secondaryContentList;

		if(GlobalVariables.Instance.GameState == GameStateEnum.EndMode)
			StartCoroutine (ShowEndModeCoroutine (content, secondaryContentList, whichMenu));
	}

	IEnumerator ShowEndModeCoroutine (RectTransform content, List<SecondaryContent> secondaryContentList, MenuComponent whichMenu)
	{
		yield return cameraMovement.StartCoroutine ("EndModePosition");

		if(secondaryContentList != null)
		{
			for(int i = 0; i < secondaryContentList.Count; i++)
			{
				secondaryContentList [i].content.anchoredPosition = secondaryContentList [i].offScreenPos;

				Enable (secondaryContentList [i].content);
				secondaryContentList [i].content.DOAnchorPos (secondaryContentList [i].onScreenPos, durationToShow).SetDelay (secondaryContentList [i].delay).SetEase (easeMenu).SetId ("Menu");

				if (secondaryContentList [i].content.GetComponent<Button> ())
					SetInteractable (secondaryContentList [i].content);
			}			
		}

		panelBackground.sizeDelta = modifiedPanelSize;

		Enable (content);

		for (int i = 0; i < content.transform.childCount; i++)
			content.transform.GetChild (i).GetComponent<RectTransform> ().localScale = Vector3.zero;

		playerScore [0].transform.parent.gameObject.SetActive (true);

		for (int i = 0; i < playerScore.Length; i++)
		{
			Enable (playerScore[i]);
			playerScore[i].localScale = Vector3.zero;
		}

		yield return new WaitForSecondsRealtime (durationToShow);

		panelBackground.DOSizeDelta (initialPanelSize, durationToShow).SetEase (easeMenu).SetId ("Menu");

		for (int i = 0; i < content.transform.childCount; i++)
			content.transform.GetChild (i).GetComponent<RectTransform> ().DOScale(1, durationToShow).SetDelay(delayBetweenStats * i).SetEase (easeMenu).SetId ("Menu");

		for (int i = 0; i < playerScore.Length; i++)
			playerScore[i].DOScale(1, durationToShow).SetDelay(delayBetweenStats * i).SetEase (easeMenu).SetId ("Menu");
		

		SelectPreviousElement (whichMenu);

		currentMenu = whichMenu;
	}

	public void ReturnToMainMenu ()
	{
		StartCoroutine (ReturnToMainMenuCoroutine ());
	}

	IEnumerator ReturnToMainMenuCoroutine ()
	{
		yield return StartCoroutine (HideEndMode (endModecontent, endModesecondaryContentList));

		endModecontent = null;
		endModesecondaryContentList = null;

		yield return cameraMovement.StartCoroutine ("MainMenuPosition");

		MasterAudio.PlaySound (SoundsManager.Instance.openMenuSound);

		loadModeScript.ReloadSceneVoid ();

		GlobalVariables.Instance.GameState = GameStateEnum.Menu;
		MainMenu ();
	}

	public void RestartInstantly ()
	{
		switch (GlobalVariables.Instance.ModeSequenceType)
		{
		case ModeSequenceType.Selection:
			loadModeScript.RestartSceneVoid (false, false);
			break;
		case ModeSequenceType.Random:
			loadModeScript.RestartSceneVoid (false, true);
			break;
		case ModeSequenceType.Cocktail:
			break;
		}
	}

	public void RestartMode ()
	{
		StartCoroutine (RestartModeCoroutine ());
	}

	IEnumerator RestartModeCoroutine ()
	{
		yield return StartCoroutine (HideEndMode (endModecontent, endModesecondaryContentList));

		endModecontent = null;
		endModesecondaryContentList = null;

		MasterAudio.PlaySound (SoundsManager.Instance.closeMenuSound);

		switch (GlobalVariables.Instance.ModeSequenceType)
		{
		case ModeSequenceType.Selection:
			loadModeScript.RestartSceneVoid (false, false);
			break;
		case ModeSequenceType.Random:
			loadModeScript.RestartSceneVoid (false, true);
			break;
		case ModeSequenceType.Cocktail:
			break;
		}
	}

	IEnumerator HideEndMode (RectTransform content, List<SecondaryContent> secondaryContentList)
	{
		eventSyst.SetSelectedGameObject (null);
		currentMenu = null;

		if(content != null)
			for (int i = 0; i < content.transform.childCount; i++)
				content.transform.GetChild (i).GetComponent<RectTransform> ().DOScale(0, durationToHide).SetDelay(delayBetweenStats * i).SetEase (easeMenu).SetId ("Menu");
		

		for (int i = 0; i < playerScore.Length; i++)
		{
			Disable (playerScore [i], durationToShow);
			playerScore[i].DOScale(0, durationToShow).SetDelay(delayBetweenStats * i).SetEase (easeMenu).SetId ("Menu");
		}

		yield return new WaitForSecondsRealtime(0.01f);

		Tween myTween = panelBackground.DOSizeDelta(modifiedPanelSize, durationToHide).SetEase (easeMenu).SetId ("Menu");		
		yield return myTween.WaitForCompletion ();

		if(secondaryContentList != null)
		{
			for(int i = 0; i < secondaryContentList.Count; i++)
			{
				Disable (secondaryContentList [i].content, durationContent + secondaryContentList [i].delay);
				secondaryContentList [i].content.DOAnchorPos (secondaryContentList [i].offScreenPos, durationContent).SetDelay (secondaryContentList [i].delay).SetEase (easeMenu).SetId ("Menu");

				if (secondaryContentList [i].content.GetComponent<Button> ())
					SetNonInteractable (secondaryContentList [i].content);
			}
		}

		if(content != null)
			Disable (content);
	}
	#endregion

	#region Events
	public event EventHandler OnMenuChange;

	IEnumerator OnMenuChangeEvent (MenuComponent whichMenu)
	{
		yield return new WaitUntil (() => currentMenu != whichMenu);

		if (OnMenuChange != null)
			OnMenuChange ();


		StartCoroutine (OnMenuChangeEvent (currentMenu));
	}
	#endregion
}
