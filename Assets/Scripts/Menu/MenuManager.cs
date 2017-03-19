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
	[HideInInspector]
	public Ease easeMenu = Ease.OutQuad;

	[Header ("Animations Duration")]
	public float durationToShow = 0.15f;
	public float durationToHide = 0.15f;
	public float durationContent = 0.15f;

	[Header ("Buttons Delay")]
	public float buttonsDelay = 0.05f;

	[Header ("Menus Buttons Positions")]
	public float menuOffScreenX = -2000;
	public float menuOnScreenX = -650;

	[Header ("Buttons Positions")]
	public float buttonOffScreenX = -2000;
	public float buttonOnScreenX = -650;

	[Header ("MainContent Positions")]
	public Vector2 offScreenContent = new Vector2 (-2000, 0);
	public Vector2 onScreenContent = new Vector2 (0, 0);

	[Header ("Header Buttons")]
	public bool hidePreviousHeaderButton;
	public float menuHeaderY = 540;
	public List<RectTransform> headerButtonsList;

	[Header ("MainMenu Buttons Positions")]
	public float mainMenuFirstButtonY = 100;
	
	[Header ("Buttons Positions")]
	public float menuFirstButtonY = 278;
	public float buttonFirstButtonY = 278;
	public float gapBetweenButtons = 131;

	[Header ("Selectable")]
	public bool selectPreviousElement = true;

	[Header ("Menu Elements To Enable")]
	public List<GameObject> elementsToEnable;

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

	private float initialMenuHeaderY;
	private float initialMenuFirstButtonY;
	private float initialButtonFirstButtonY;

	private float initialMenuOffScreenX;
	private float initialMenuOnScreenX;
	private float initialButtonOffScreenX;
	private float initialButtonOnScreenX;
	private Vector2 initialOffScreenContent;
	private Vector2 initialOnScreenContent;

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

		backButtons.anchoredPosition = new Vector2(backButtonsXPos.x, backButtons.anchoredPosition.y);

		SetupLogo ();

		SetupInitialSettings ();

		for (int i = 0; i < elementsToEnable.Count; i++)
			elementsToEnable [i].SetActive (true);
	}

	void SetupLogo ()
	{
		mainMenuScript.secondaryContents [0].content.gameObject.SetActive (true);
		mainMenuScript.secondaryContents [0].content.anchoredPosition = mainMenuScript.secondaryContents [0].onScreenPos;
	}
		
	void StartScreen ()
	{
		MasterAudio.PlaySound (SoundsManager.Instance.gameStartSound);

		cameraMovement.StartCoroutine ("StartScreen");

		ShowMenu (mainMenuScript);
		StartCoroutine (OnMenuChangeEvent (mainMenuScript));

		startScreen = false;
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
						StartScreen ();
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

	#region Submit Methods
	public enum MenuAnimationType { Show, Hide, Cancel, Submit, UnderSubmit, UnderCancel };

	public void ShowMenu (MenuComponent whichMenu)
	{
		StartCoroutine (ShowMenuCoroutine (whichMenu));
	}

	IEnumerator ShowMenuCoroutine (MenuComponent whichMenu)
	{
		CheckOverrideSettings (whichMenu, WhichOverrideSettings.All);
		yield return StartCoroutine (ShowMenuCoroutine (whichMenu, MenuAnimationType.Show));
		ResetOverrideSettings (WhichOverrideSettings.All);
	}

	public void SubmitMenu (MenuComponent whichMenu, int submitButton)
	{
		StartCoroutine (SubmitMenuCoroutine (whichMenu, submitButton));
	}

	IEnumerator SubmitMenuCoroutine (MenuComponent whichMenu, int submitButton)
	{
		menuTweening = true;
		PlaySubmitSound ();

		CheckOverrideSettings (whichMenu, WhichOverrideSettings.HeaderPos);
		CheckOverrideSettings (whichMenu.aboveMenuScript, WhichOverrideSettings.MenuPos | WhichOverrideSettings.ButtonPos | WhichOverrideSettings.ContentPos);

		yield return StartCoroutine (HideMenuCoroutine (whichMenu.aboveMenuScript, MenuAnimationType.UnderSubmit, submitButton));

		ResetOverrideSettings (WhichOverrideSettings.All);

		CheckOverrideSettings (whichMenu, WhichOverrideSettings.All);

		yield return StartCoroutine (ShowMenuCoroutine (whichMenu, MenuAnimationType.Submit));

		ResetOverrideSettings (WhichOverrideSettings.All);

		menuTweening = false;
	}


	IEnumerator ShowMenuCoroutine (MenuComponent whichMenu, MenuAnimationType animationType, int cancelButton = -1)
	{
		currentMenu = whichMenu;

		for(int i = 0; i < whichMenu.contentDisplay.Count; i++)
		{
			switch (whichMenu.contentDisplay [i].contentType)
			{
			case MenuContentType.Menus:
				if(cancelButton != -1)
					yield return StartCoroutine (ShowUnderMenus (whichMenu, animationType, i, cancelButton));
				else
					yield return StartCoroutine (ShowUnderMenusSolo (whichMenu, animationType, i));
				break;
			case MenuContentType.Buttons:
				yield return StartCoroutine (ShowUnderButtons (whichMenu, animationType, i));
				break;
			case MenuContentType.MainContent:
				yield return StartCoroutine (ShowMainContent (whichMenu, animationType, i));
				break;
			case MenuContentType.SecondaryContent:
				yield return StartCoroutine (ShowSecondaryContent (whichMenu, animationType, i));
				break;
			}
		}

		//Select First Under Menu Button
		SelectPreviousElement (whichMenu); 
	}


	IEnumerator ShowUnderMenus (MenuComponent whichMenu, MenuAnimationType animationType, int contentIndex, int cancelButton = -1)
	{
		int delay = 0;

		//Wait delay
		if (whichMenu.contentDisplay.Count > 0 && whichMenu.contentDisplay [contentIndex].delay > 0)
			yield return new WaitForSecondsRealtime (whichMenu.contentDisplay [contentIndex].delay);

		//Show Under Menus Buttons
		for(int i = whichMenu.underMenusButtons.Count - 1; i >= 0; i--)
		{
			if(i != cancelButton)
			{
				if(whichMenu.underMenusButtonsPositions.Count > 0)
					whichMenu.underMenusButtons [i].anchoredPosition = new Vector2 (menuOffScreenX, whichMenu.underMenusButtonsPositions [i].y);
				else
					whichMenu.underMenusButtons [i].anchoredPosition = new Vector2 (menuOffScreenX, MenuButtonYPos (i) + GapAfterHeaderButton ()) - whichMenu.menusParent.anchoredPosition;
			}

			Enable (whichMenu.underMenusButtons [i]);
			SetInteractable (whichMenu.underMenusButtons [i], durationToShow + ButtonsDelay (delay));
			
			if(whichMenu.underMenusButtonsPositions.Count > 0)
				whichMenu.underMenusButtons [i].DOAnchorPos (whichMenu.underMenusButtonsPositions [i], durationToShow).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
			else
				whichMenu.underMenusButtons [i].DOAnchorPos (new Vector2 (menuOnScreenX, MenuButtonYPos (i) + GapAfterHeaderButton ()) - whichMenu.menusParent.anchoredPosition, durationToShow).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");

			if (i == cancelButton)
				StartCoroutine (EnableViewScroll (whichMenu, durationToShow + ButtonsDelay (delay) * 0.5f));

			delay++;
		}

		//Remove Current Header From List
		headerButtonsList.RemoveAt (headerButtonsList.Count - 1);

		ShowPreviousHeader (whichMenu, cancelButton, delay);

		//Wait
		if(contentIndex == whichMenu.contentDisplay.Count - 1 || whichMenu.contentDisplay [contentIndex + 1].waitPreviousContent)
			yield return new WaitForSecondsRealtime (durationToShow + ButtonsDelay (delay));
		else
			yield break;
	}

	IEnumerator ShowUnderMenusSolo (MenuComponent whichMenu, MenuAnimationType animationType, int contentIndex)
	{
		//Wait delay
		if (whichMenu.contentDisplay.Count > 0 && whichMenu.contentDisplay [contentIndex].delay > 0)
			yield return new WaitForSecondsRealtime (whichMenu.contentDisplay [contentIndex].delay);

		StartCoroutine (EnableViewScroll (whichMenu, 0));

		//Show Under Menu Buttons
		int delay = 0;

		for(int i = whichMenu.underMenusButtons.Count - 1; i >= 0; i--)
		{
			if(whichMenu.underMenusButtonsPositions.Count > 0)
				whichMenu.underMenusButtons [i].anchoredPosition = new Vector2 (menuOffScreenX, whichMenu.underMenusButtonsPositions [i].y);
			else
				whichMenu.underMenusButtons [i].anchoredPosition = new Vector2 (menuOffScreenX, MenuButtonYPos (i) + GapAfterHeaderButton ()) - whichMenu.menusParent.anchoredPosition;

			Enable (whichMenu.underMenusButtons [i]);
			SetInteractable (whichMenu.underMenusButtons [i], durationToShow + ButtonsDelay (delay));

			if(whichMenu.underMenusButtonsPositions.Count > 0)
				whichMenu.underMenusButtons [i].DOAnchorPos (whichMenu.underMenusButtonsPositions [i], durationToShow).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
			else
				whichMenu.underMenusButtons [i].DOAnchorPos (new Vector2 (menuOnScreenX, MenuButtonYPos (i) + GapAfterHeaderButton ()) - whichMenu.menusParent.anchoredPosition, durationToShow).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");

			delay++;
		}
			
		//Wait
		if(contentIndex == whichMenu.contentDisplay.Count - 1 || whichMenu.contentDisplay [contentIndex + 1].waitPreviousContent)
			yield return new WaitForSecondsRealtime (durationToShow + ButtonsDelay (delay));
		else
			yield break;
	}

	IEnumerator EnableViewScroll (MenuComponent whichMenu, float delay)
	{
		yield return new WaitForSecondsRealtime (delay);

		if (whichMenu.scrollViewButtons)
			whichMenu.menusParent.GetComponent<Image> ().enabled = true;
	}

	void ShowPreviousHeader (MenuComponent whichMenu, int cancelButton, int delay)
	{
		//Show Previous Header Button
		if(whichMenu.menuButton != null)
		{
			Enable (whichMenu.menuButton);
			SetInteractable (whichMenu.menuButton, durationToShow);

			if(hidePreviousHeaderButton)
			{
				if(whichMenu.menuButton != null)
				{
					whichMenu.menuButton.DOAnchorPos (new Vector2(menuOnScreenX, MenuHeaderButtonPosition ()), durationToShow).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
					headerButtonsList.Add (whichMenu.menuButton);
				}
			}
		}
	}

	IEnumerator ShowUnderButtons (MenuComponent whichMenu, MenuAnimationType animationType, int contentIndex)
	{
		if(whichMenu.overrideButtonsDisplay && !whichMenu.showButtonsOnSubmit)
			yield break;

		//Wait delay
		if (whichMenu.contentDisplay.Count > 0 && whichMenu.contentDisplay [contentIndex].delay > 0)
			yield return new WaitForSecondsRealtime (whichMenu.contentDisplay [contentIndex].delay);

		//Show Under Menu Buttons
		int underDelay = 0;

		for(int i = whichMenu.underButtons.Count - 1; i >= 0; i--)
		{
			whichMenu.underButtons [i].anchoredPosition = new Vector2 (buttonOffScreenX, ButtonYPos (i) + GapAfterHeaderButton ());

			Enable (whichMenu.underButtons [i]);
			SetInteractable (whichMenu.underButtons [i], durationToShow + ButtonsDelay (underDelay));

			if(whichMenu.underButtonsPositions.Count > 0)
				whichMenu.underButtons [i].DOAnchorPos (whichMenu.underButtonsPositions [i], durationToShow).SetDelay (ButtonsDelay (underDelay)).SetEase (easeMenu).SetId ("Menu");
			else
				whichMenu.underButtons [i].DOAnchorPos (new Vector2 (buttonOnScreenX, ButtonYPos (i) + GapAfterHeaderButton ()), durationToShow).SetDelay (ButtonsDelay (underDelay)).SetEase (easeMenu).SetId ("Menu");
			
			underDelay++;
		}

		//Wait
		if(contentIndex == whichMenu.contentDisplay.Count - 1 || whichMenu.contentDisplay [contentIndex + 1].waitPreviousContent)
			yield return new WaitForSecondsRealtime (durationToShow + ButtonsDelay (underDelay));
		else
			yield break;
	}

	IEnumerator ShowMainContent (MenuComponent whichMenu, MenuAnimationType animationType, int contentIndex)
	{
		if(whichMenu.overrideContentDisplay && !whichMenu.showContentOnSubmit)
			yield break;

		//Wait delay
		if (whichMenu.contentDisplay.Count > 0 && whichMenu.contentDisplay [contentIndex].delay > 0)
			yield return new WaitForSecondsRealtime (whichMenu.contentDisplay [contentIndex].delay);

		whichMenu.mainContent.anchoredPosition = offScreenContent;
		Enable (whichMenu.mainContent);

		whichMenu.mainContent.DOAnchorPos (onScreenContent, durationContent).SetEase (easeMenu).SetId ("Menu");

		//Wait
		if(contentIndex == whichMenu.contentDisplay.Count - 1 || whichMenu.contentDisplay [contentIndex + 1].waitPreviousContent)
			yield return new WaitForSecondsRealtime (durationContent);
		else
			yield break;
	}

	IEnumerator ShowSecondaryContent (MenuComponent whichMenu, MenuAnimationType animationType, int contentIndex)
	{
		float waitDelay = 0;

		//Wait delay
		if (whichMenu.contentDisplay.Count > 0 && whichMenu.contentDisplay [contentIndex].delay > 0)
			yield return new WaitForSecondsRealtime (whichMenu.contentDisplay [contentIndex].delay);

		//Secondary Content
		for(int i = 0; i < whichMenu.secondaryContents.Count; i++)
		{
			if(whichMenu.secondaryContents [i].content.anchoredPosition != whichMenu.secondaryContents [i].onScreenPos)
				whichMenu.secondaryContents [i].content.anchoredPosition = whichMenu.secondaryContents [i].offScreenPos;
			
			if (whichMenu.secondaryContents [i].showOnSubmit)
			{
				if (whichMenu.secondaryContents [i].delay > waitDelay)
					waitDelay = whichMenu.secondaryContents [i].delay;
				
				Enable (whichMenu.secondaryContents [i].content);
				whichMenu.secondaryContents [i].content.DOAnchorPos (whichMenu.secondaryContents [i].onScreenPos, durationToShow).SetDelay (whichMenu.secondaryContents [i].delay).SetEase (easeMenu).SetId ("Menu");
			}
		}			

		//Wait
		if(contentIndex == whichMenu.contentDisplay.Count - 1 || whichMenu.contentDisplay [contentIndex + 1].waitPreviousContent)
			yield return new WaitForSecondsRealtime (durationToShow + waitDelay);
		else
			yield break;
	}
	#endregion

	#region Cancel Methods
	public void HideMenu (MenuComponent whichMenu)
	{
		StartCoroutine (HideMenuCoroutine (whichMenu));
	}

	IEnumerator HideMenuCoroutine (MenuComponent whichMenu)
	{
		CheckOverrideSettings (whichMenu, WhichOverrideSettings.All);
		yield return StartCoroutine (HideMenuCoroutine (whichMenu, MenuAnimationType.Hide));
		ResetOverrideSettings (WhichOverrideSettings.All);

		currentMenu = null;
	}

	public void CancelMenu (MenuComponent whichMenu, int cancelButton)
	{
		StartCoroutine (CancelMenuCoroutine (whichMenu, cancelButton));
	}

	IEnumerator CancelMenuCoroutine (MenuComponent whichMenu, int cancelButton)
	{
		menuTweening = true;
		PlayReturnSound ();

		CheckOverrideSettings (whichMenu, WhichOverrideSettings.All);

		yield return StartCoroutine (HideMenuCoroutine (whichMenu, MenuAnimationType.Cancel));

		ResetOverrideSettings (WhichOverrideSettings.All);

		CheckOverrideSettings (whichMenu.aboveMenuScript, WhichOverrideSettings.All);

		yield return StartCoroutine (ShowMenuCoroutine (whichMenu.aboveMenuScript, MenuAnimationType.UnderCancel, cancelButton));

		ResetOverrideSettings (WhichOverrideSettings.All);

		menuTweening = false;
	}


	IEnumerator HideMenuCoroutine (MenuComponent whichMenu, MenuAnimationType animationType, int submitButton = -1)
	{
		for(int i = whichMenu.contentDisplay.Count - 1; i >= 0; i--)
		{
			switch (whichMenu.contentDisplay [i].contentType)
			{
			case MenuContentType.Menus:
				if (submitButton != -1)
					yield return StartCoroutine (HideUnderMenus (whichMenu, animationType, i, submitButton));
				else
					yield return StartCoroutine (HideUnderMenusSolo (whichMenu, animationType, i));
				break;
			case MenuContentType.Buttons:
				yield return StartCoroutine (HideUnderButtons (whichMenu, animationType, i));
				break;
			case MenuContentType.MainContent:
				yield return StartCoroutine (HideMainContent (whichMenu, animationType, i));
				break;
			case MenuContentType.SecondaryContent:
				yield return StartCoroutine (HideSecondaryContent (whichMenu, animationType, i));
				break;
			}
		}

		if (animationType == MenuAnimationType.Hide && headerButtonsList.Count > 0)
			HidePreviousHeader (0);
	}


	IEnumerator HideUnderMenus (MenuComponent whichMenu, MenuAnimationType animationType, int contentIndex, int submitButton)
	{
		//Wait delay
		if (whichMenu.contentDisplay.Count > 0 && whichMenu.contentDisplay [contentIndex].delay > 0)
			yield return new WaitForSecondsRealtime (whichMenu.contentDisplay [contentIndex].delay);

		int delay = 0;

		//Under Menus Buttons
		for(int i = whichMenu.underMenusButtons.Count - 1; i >= 0; i--)
		{
			if(i != submitButton)
			{
				Disable (whichMenu.underMenusButtons [i], durationToHide + ButtonsDelay (delay));
				SetNonInteractable (whichMenu.underMenusButtons [i]);

				whichMenu.underMenusButtons [i].DOAnchorPosX (menuOffScreenX, durationToHide).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
				delay++;
			}
		}

		HidePreviousHeader (delay);
		Tween tween = PlaceCurrentHeader (whichMenu, submitButton, delay);

		//Wait
		if(contentIndex == 0 || whichMenu.contentDisplay [contentIndex].waitPreviousContent)
			yield return tween.WaitForCompletion ();
		else
			yield break;
	}

	void HidePreviousHeader (int delay)
	{
		//Hide Previous Header Button
		if(hidePreviousHeaderButton)
		{
			if(headerButtonsList.Count > 0)
			{
				Disable (headerButtonsList [headerButtonsList.Count - 1], durationToHide + ButtonsDelay (delay));
				SetNonInteractable (headerButtonsList [headerButtonsList.Count - 1]);

				headerButtonsList [headerButtonsList.Count - 1].DOAnchorPosX (menuOffScreenX, durationToHide).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
				headerButtonsList.RemoveAt (headerButtonsList.Count - 1);
			}
		}
	}

	Tween PlaceCurrentHeader (MenuComponent whichMenu, int submitButton, int delay)
	{
		//Get Cancel Button Out Of ScrollView
		if (whichMenu.scrollViewButtons)
			StartCoroutine (DisableViewScroll (whichMenu, durationToShow + ButtonsDelay (delay) * 0.5f));

		//Place Submit Button as Header
		Enable (whichMenu.underMenusButtons [submitButton]);
		SetNonInteractable (whichMenu.underMenusButtons [submitButton]);

		return whichMenu.underMenusButtons [submitButton].DOAnchorPos (new Vector2(menuOnScreenX, MenuHeaderButtonPosition ()) - whichMenu.menusParent.anchoredPosition, durationToShow)
			.SetDelay (ButtonsDelay (delay))
			.SetEase (easeMenu)
			.OnComplete (()=> headerButtonsList.Add (whichMenu.underMenusButtons [submitButton]))
			.SetId ("Menu");
	}

	IEnumerator DisableViewScroll (MenuComponent whichMenu, float delay)
	{
		yield return new WaitForSecondsRealtime (delay);

		if (whichMenu.scrollViewButtons)
			whichMenu.menusParent.GetComponent<Image> ().enabled = false;
	}
		
	IEnumerator HideUnderMenusSolo (MenuComponent whichMenu, MenuAnimationType animationType, int contentIndex)
	{
		//Wait delay
		if (whichMenu.contentDisplay.Count > 0 && whichMenu.contentDisplay [contentIndex].delay > 0)
			yield return new WaitForSecondsRealtime (whichMenu.contentDisplay [contentIndex].delay);

		int delay = 0;

		//Under Buttons
		for(int i = whichMenu.underMenusButtons.Count - 1; i >= 0; i--)
		{
			Disable (whichMenu.underMenusButtons [i], durationToHide + ButtonsDelay (delay));
			SetNonInteractable (whichMenu.underMenusButtons [i]);

			whichMenu.underMenusButtons [i].DOAnchorPosX (menuOffScreenX, durationToHide).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
			delay++;
		}

		if(animationType == MenuAnimationType.Hide)
			HidePreviousHeader (delay);

		//Wait
		if(contentIndex == 0 || whichMenu.contentDisplay [contentIndex].waitPreviousContent)
			yield return new WaitForSecondsRealtime (durationToHide + ButtonsDelay (delay));
		else
			yield break;
	}

	IEnumerator HideUnderButtons (MenuComponent whichMenu, MenuAnimationType animationType, int contentIndex)
	{
		if(whichMenu.overrideButtonsDisplay)
		{
			if(animationType == MenuAnimationType.UnderSubmit && !whichMenu.hideButtonsOnUnderSubmit)
				yield break;

			if(animationType == MenuAnimationType.Cancel && !whichMenu.hideButtonsOnCancel)
				yield break;
		}

		//Wait delay
		if (whichMenu.contentDisplay.Count > 0 && whichMenu.contentDisplay [contentIndex].delay > 0)
			yield return new WaitForSecondsRealtime (whichMenu.contentDisplay [contentIndex].delay);

		int delay = 0;

		//Under Buttons
		for(int i = whichMenu.underButtons.Count - 1; i >= 0; i--)
		{
			Disable (whichMenu.underButtons [i], durationToHide + ButtonsDelay (delay));
			SetNonInteractable (whichMenu.underButtons [i]);

			whichMenu.underButtons [i].DOAnchorPosX (buttonOffScreenX, durationToHide).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
			delay++;
		}

		//Wait
		if(contentIndex == 0 || whichMenu.contentDisplay [contentIndex].waitPreviousContent)
			yield return new WaitForSecondsRealtime (durationToHide + ButtonsDelay (delay));
		else
			yield break;
	}

	IEnumerator HideMainContent (MenuComponent whichMenu, MenuAnimationType animationType, int contentIndex)
	{
		if(whichMenu.overrideContentDisplay)
		{
			if(animationType == MenuAnimationType.UnderSubmit && !whichMenu.hideContentOnUnderSubmit)
				yield break;

			if(animationType == MenuAnimationType.Cancel && !whichMenu.hideContentOnCancel)
				yield break;
		}

		//Wait delay
		if (whichMenu.contentDisplay.Count > 0 && whichMenu.contentDisplay [contentIndex].delay > 0)
			yield return new WaitForSecondsRealtime (whichMenu.contentDisplay [contentIndex].delay);

		whichMenu.mainContent.DOAnchorPos (offScreenContent, durationContent).SetEase (easeMenu).SetId ("Menu").OnComplete (()=> Disable(whichMenu.mainContent));

		//Wait
		if(contentIndex == 0 || whichMenu.contentDisplay [contentIndex].waitPreviousContent)
			yield return new WaitForSecondsRealtime (durationContent);
		else
			yield break;
	}

	IEnumerator HideSecondaryContent (MenuComponent whichMenu, MenuAnimationType animationType, int contentIndex)
	{
		float waitDelay = 0;

		//Wait delay
		if (whichMenu.contentDisplay.Count > 0 && whichMenu.contentDisplay [contentIndex].delay > 0)
			yield return new WaitForSecondsRealtime (whichMenu.contentDisplay [contentIndex].delay);

		//Secondary Content
		for(int i = 0; i < whichMenu.secondaryContents.Count; i++)
		{
			if (animationType == MenuAnimationType.UnderSubmit && whichMenu.secondaryContents [i].hideOnUnderSubmit 
				|| animationType == MenuAnimationType.Cancel && whichMenu.secondaryContents [i].hideOnCancel 
				|| animationType == MenuAnimationType.Hide)
			{
				if (whichMenu.secondaryContents [i].delay > waitDelay)
					waitDelay = whichMenu.secondaryContents [i].delay;
				
				Disable (whichMenu.secondaryContents [i].content, durationToHide + whichMenu.secondaryContents [i].delay);
				whichMenu.secondaryContents [i].content.DOAnchorPos (whichMenu.secondaryContents [i].offScreenPos, durationToHide).SetDelay (whichMenu.secondaryContents [i].delay).SetEase (easeMenu).SetId ("Menu");
			}
		}			
	
		//Wait
		if(contentIndex == 0 || whichMenu.contentDisplay [contentIndex].waitPreviousContent)
			yield return new WaitForSecondsRealtime (durationToHide + waitDelay);
		else
			yield break;
	}
	#endregion

	#region Other Methods
	public enum WhichOverrideSettings { HeaderPos = 1, MenuPos = 2, ButtonPos = 4, ContentPos = 8, All = 16}

	void SetupInitialSettings ()
	{
		initialMenuHeaderY = menuHeaderY;
		initialMenuFirstButtonY = menuFirstButtonY;
		initialButtonFirstButtonY = buttonFirstButtonY;

		initialMenuOffScreenX = menuOffScreenX;
		initialMenuOnScreenX = menuOnScreenX;

		initialButtonOffScreenX = buttonOffScreenX;
		initialButtonOnScreenX = buttonOnScreenX;

		initialOffScreenContent = offScreenContent;
		initialOnScreenContent = onScreenContent;
	}

	void CheckOverrideSettings (MenuComponent whichMenu, WhichOverrideSettings whichSettings)
	{
		if((whichSettings & WhichOverrideSettings.HeaderPos) == WhichOverrideSettings.HeaderPos || whichSettings == WhichOverrideSettings.All)
		{
			if (whichMenu.overrideHeaderPos)
			{
				menuHeaderY = whichMenu.menuHeaderY;
				menuFirstButtonY = whichMenu.menuFirstButtonY;
				buttonFirstButtonY = whichMenu.buttonFirstButtonY;
			}
		}
		if(whichSettings == WhichOverrideSettings.All || (whichSettings & WhichOverrideSettings.MenuPos) == WhichOverrideSettings.MenuPos)
		{
			if (whichMenu.overrideMenuPos)
			{
				menuOffScreenX = whichMenu.menuOffScreenX;
				menuOnScreenX = whichMenu.menuOnScreenX;
			}
		}
		if((whichSettings & WhichOverrideSettings.ButtonPos) == WhichOverrideSettings.ButtonPos || whichSettings == WhichOverrideSettings.All)
		{
			if (whichMenu.overrideButtonPos)
			{
				buttonOffScreenX = whichMenu.buttonOffScreenX;
				buttonOnScreenX = whichMenu.buttonOnScreenX;
			}
		}
		if((whichSettings & WhichOverrideSettings.ContentPos) == WhichOverrideSettings.ContentPos || whichSettings == WhichOverrideSettings.All)
		{
			if (whichMenu.overrideContentPos)
			{
				offScreenContent = whichMenu.offScreenContent;
				onScreenContent = whichMenu.onScreenContent;
			}			
		}
	}

	void ResetOverrideSettings (WhichOverrideSettings whichSettings)
	{
		if((whichSettings & WhichOverrideSettings.HeaderPos) == WhichOverrideSettings.HeaderPos || whichSettings == WhichOverrideSettings.All)
		{
			menuHeaderY = initialMenuHeaderY;
			menuFirstButtonY = initialMenuFirstButtonY;
			buttonFirstButtonY = initialButtonFirstButtonY;
		}
		if((whichSettings & WhichOverrideSettings.MenuPos) == WhichOverrideSettings.MenuPos || whichSettings == WhichOverrideSettings.All)
		{
			
			menuOffScreenX = initialMenuOffScreenX;
			menuOnScreenX = initialMenuOnScreenX;
		}
		if((whichSettings & WhichOverrideSettings.ButtonPos) == WhichOverrideSettings.ButtonPos || whichSettings == WhichOverrideSettings.All)
		{
			buttonOffScreenX = initialButtonOffScreenX;
			buttonOnScreenX = initialButtonOnScreenX;
			
		}
		if((whichSettings & WhichOverrideSettings.ContentPos) == WhichOverrideSettings.ContentPos || whichSettings == WhichOverrideSettings.All)
		{
			offScreenContent = initialOffScreenContent;
			onScreenContent = initialOnScreenContent;
			
		}
	}


	public float MenuHeaderButtonPosition ()
	{
		return menuHeaderY - gapBetweenButtons * headerButtonsList.Count;
	}

	public float GapAfterHeaderButton ()
	{
		if (headerButtonsList.Count > 0)
			return (-gapBetweenButtons * headerButtonsList.Count) + gapBetweenButtons;
		else
			return 0;
	}

	public float ButtonsDelay (int i)
	{
		return buttonsDelay * i;
	}

	public float MenuButtonYPos (int i)
	{
		return menuFirstButtonY - (gapBetweenButtons * i);
	}

	public float ButtonYPos (int i)
	{
		return buttonFirstButtonY - (gapBetweenButtons * i);
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
			&& currentMenu.menuComponentType != MenuComponentType.RootMenu 
			&& currentMenu.menuComponentType != MenuComponentType.EndModeMenu)
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

			ShowMenu (mainMenuScript);
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
			StartCoroutine (StartRandomCocktailModeCoroutine ());
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

		LoadModeManager.Instance.LoadRandomScene ();

		yield return new WaitForSecondsRealtime (cameraMovement.loadingMovementDuration * 2);

		yield return new WaitForSecondsRealtime(durationToHide);

		MasterAudio.PlaySound (SoundsManager.Instance.closeMenuSound);

		yield return cameraMovement.StartCoroutine ("PlayPosition");

		GlobalVariables.Instance.GameState = GameStateEnum.Playing;
	}

	IEnumerator StartRandomCocktailModeCoroutine ()
	{
		mainCamera.GetComponent<SlowMotionCamera> ().StopPauseSlowMotion ();
		currentMenu.HideMenu ();

		GlobalVariables.Instance.currentCocktailModes.AddRange (GlobalVariables.Instance.selectedCocktailModes);

		LoadModeManager.Instance.LoadRandomCocktailScene ();

		yield return new WaitForSecondsRealtime (cameraMovement.loadingMovementDuration * 2);

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

		LoadModeManager.Instance.ReloadSceneVoid ();

		GlobalVariables.Instance.GameState = GameStateEnum.Menu;

		ShowMenu (mainMenuScript);
	}

	public void RestartInstantly ()
	{
		switch (GlobalVariables.Instance.ModeSequenceType)
		{
		case ModeSequenceType.Selection:
			LoadModeManager.Instance.RestartSceneVoid (false);
			break;
		case ModeSequenceType.Random:
			LoadModeManager.Instance.RestartSceneVoid (false);
			break;
		case ModeSequenceType.Cocktail:
			LoadModeManager.Instance.RestartSceneVoid (false);
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
			LoadModeManager.Instance.RestartSceneVoid (false);
			break;
		case ModeSequenceType.Random:
			LoadModeManager.Instance.RestartSceneVoid (false);
			break;
		case ModeSequenceType.Cocktail:
			LoadModeManager.Instance.RestartSceneVoid (false);
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
