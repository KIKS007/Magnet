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
	public float animationDuration = 0.15f;

	[Header ("Buttons Delay")]
	public float buttonsDelay = 0.05f;

	[Header ("Menus Buttons Positions")]
	public float menuOffScreenX = -2000;
	public float menuOnScreenX = -650;

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

	[Header ("Disconnected Players")]
	public RectTransform[] unpluggedPlayers = new RectTransform[4];
	public Vector2 disconnectedPlayersYPos;

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

	[HideInInspector]
	public bool startScreen = true;
	#endregion

	#region Setup
	void Start () 
	{
		DOTween.Init();
		DOTween.defaultTimeScaleIndependent = true;

		OnMenuChange += BackButtons;

		ReInput.ControllerConnectedEvent += (ControllerStatusChangedEventArgs obj) => GamepadsChange ();
		ReInput.ControllerDisconnectedEvent += (ControllerStatusChangedEventArgs obj) => GamepadsChange ();

		GlobalVariables.Instance.OnGamepadDisconnected += GamepadDisconnected;

		mainMenu.SetActive (true);
		mainMenuScript = mainMenu.GetComponent<MenuComponent> ();
			
		eventSyst = GameObject.FindGameObjectWithTag ("EventSystem").GetComponent<EventSystem> ();
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		cameraMovement = mainCamera.GetComponent<MenuCameraMovement> ();

		backButtons.anchoredPosition = new Vector2(backButtonsXPos.x, backButtons.anchoredPosition.y);

		SetupLogo ();

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

		//cameraMovement.StartCoroutine ("StartScreen");
		cameraMovement.StartCoroutine ("StartPosition");

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

			GamepadsChange ();

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
			//for(int i = 0; i < GlobalVariables.Instance.rewiredPlayers.Length; i++)
			for(int i = 0; i < 2; i++)
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
			//for(int i = 0; i < GlobalVariables.Instance.rewiredPlayers.Length; i++)
			for(int i = 0; i < 2; i++)
			{
				if (GlobalVariables.Instance.GameState == GameStateEnum.Paused && GlobalVariables.Instance.rewiredPlayers [i].GetButtonDown ("UI Start") && !GlobalVariables.Instance.OneGamepadUnplugged)
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
			StartCoroutine (SelectPreviousElement (currentMenu));
	}

	void GamepadDisconnected ()
	{
		if (GlobalVariables.Instance.GameState == GameStateEnum.Playing)
			PauseResumeGame ();

		if (GlobalVariables.Instance.GameState == GameStateEnum.EndMode)
			ReturnToMainMenu ();
	}

	void GamepadsChange ()
	{
		if (GlobalVariables.Instance.GameState == GameStateEnum.Playing)
			return;

		foreach(PlayerGamepad p in GlobalVariables.Instance.PlayersGamepadList)
		{
			//Unplugged
			if(!p.GamepadIsPlugged)
			{
				unpluggedPlayers [(int)p.PlayerName].gameObject.SetActive (true);
				unpluggedPlayers [(int)p.PlayerName].DOAnchorPosY (disconnectedPlayersYPos.y, animationDuration).SetEase (easeMenu);
			}
			//Plugged
			else
			{
				unpluggedPlayers [(int)p.PlayerName].DOAnchorPosY (disconnectedPlayersYPos.x, animationDuration).SetEase (easeMenu).OnComplete (()=> 
				{
					unpluggedPlayers [(int)p.PlayerName].gameObject.SetActive (false);
				});
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

	public void SubmitMenu (MenuComponent whichMenu, int submitButton)
	{
		StartCoroutine (SubmitMenuCoroutine (whichMenu, submitButton));
	}

	public void SubmitMenu (MenuComponent whichMenu)
	{
		StartCoroutine (SubmitMenuCoroutine (whichMenu));
	}

	IEnumerator SubmitMenuCoroutine (MenuComponent whichMenu, int submitButton)
	{
		float time = Time.time;

		menuTweening = true;
		PlaySubmitSound ();

		yield return StartCoroutine (HideMenuCoroutine (whichMenu.aboveMenuScript, submitButton));

		yield return StartCoroutine (ShowMenuCoroutine (whichMenu));

		menuTweening = false;

		yield return new WaitWhile (() => DOTween.IsTweening ("Menu"));

		Debug.Log ("Show : " + (Time.time - time).ToString ());
	}

	//Hide Previous Menu and Show Target Menu
	IEnumerator SubmitMenuCoroutine (MenuComponent whichMenu)
	{
		menuTweening = true;
		PlaySubmitSound ();

		yield return StartCoroutine (HideMenuCoroutine (whichMenu.aboveMenuScript));

		yield return StartCoroutine (ShowMenuCoroutine (whichMenu));

		menuTweening = false;
	}


	//Show Menu
	IEnumerator ShowMenuCoroutine (MenuComponent whichMenu, int cancelButton = -1)
	{
		currentMenu = whichMenu;

		//Select First Under Menu Button
		StartCoroutine (SelectPreviousElement (whichMenu)); 

		StartCoroutine (ShowUnderMenus (whichMenu, cancelButton));
		StartCoroutine (ShowMainContent (whichMenu));
		StartCoroutine (ShowSecondaryContent (whichMenu));

		yield return new WaitForSecondsRealtime (animationDuration);
	}

	//Show Menus Contents
	IEnumerator ShowUnderMenus (MenuComponent whichMenu, int cancelButton = -1)
	{
		int delay = 0;


		//Show Under Menus Buttons
		for(int i = whichMenu.underMenusButtons.Count - 1; i >= 0; i--)
		{
			float duration = animationDuration - ButtonsDelay (delay);

			//If Canceling
			if(cancelButton != -1 && i != cancelButton)
				whichMenu.underMenusButtons [i].anchoredPosition = new Vector2 (menuOffScreenX, MenuButtonYPos (i) + GapAfterHeaderButton ()) - whichMenu.menusParent.anchoredPosition;

			Enable (whichMenu.underMenusButtons [i]);
			SetInteractable (whichMenu.underMenusButtons [i], duration + ButtonsDelay (delay));
			
			whichMenu.underMenusButtons [i].DOAnchorPos (new Vector2 (menuOnScreenX, MenuButtonYPos (i) + GapAfterHeaderButton ()) - whichMenu.menusParent.anchoredPosition, duration).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");

			delay++;
		}

		//If Canceling
		if(cancelButton != -1)
		{
			//Remove Current Header From List
			if(headerButtonsList.Count > 0)
				headerButtonsList.RemoveAt (headerButtonsList.Count - 1);
			
			ShowPreviousHeader (whichMenu, delay);
		}

		yield break;
	}

	void ShowPreviousHeader (MenuComponent whichMenu, int delay)
	{
		//Show Previous Header Button
		if (whichMenu.menuButton == null)
			return;
		
		Enable (whichMenu.menuButton);
		SetInteractable (whichMenu.menuButton, animationDuration);
		
		if(hidePreviousHeaderButton && whichMenu.menuButton != null)
			whichMenu.menuButton.DOAnchorPos (new Vector2(menuOnScreenX, MenuHeaderButtonPosition ()), animationDuration).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");

		headerButtonsList.Add (whichMenu.menuButton);
	}

	IEnumerator ShowMainContent (MenuComponent whichMenu)
	{
		if (whichMenu.mainContent == null)
			yield break;

		whichMenu.mainContent.anchoredPosition = offScreenContent;
		Enable (whichMenu.mainContent);

		whichMenu.mainContent.DOAnchorPos (onScreenContent, animationDuration).SetEase (easeMenu).SetId ("Menu");

		yield break;
	}

	IEnumerator ShowSecondaryContent (MenuComponent whichMenu)
	{
		float waitDelay = 0;

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
				whichMenu.secondaryContents [i].content.DOAnchorPos (whichMenu.secondaryContents [i].onScreenPos, animationDuration).SetDelay (whichMenu.secondaryContents [i].delay).SetEase (easeMenu).SetId ("Menu");
			}
		}	

		yield break;
	}
	#endregion

	#region Cancel Methods
	public void HideMenu (MenuComponent whichMenu)
	{
		StartCoroutine (HideMenuCoroutine (whichMenu));
	}

	public void CancelMenu (MenuComponent whichMenu, int cancelButton)
	{
		StartCoroutine (CancelMenuCoroutine (whichMenu, cancelButton));
	}

	IEnumerator CancelMenuCoroutine (MenuComponent whichMenu, int cancelButton)
	{
		menuTweening = true;

		float time = Time.time;

		PlayReturnSound ();

		yield return StartCoroutine (HideMenuCoroutine (whichMenu));

		yield return StartCoroutine (ShowMenuCoroutine (whichMenu.aboveMenuScript, cancelButton));

		menuTweening = false;

		yield return new WaitWhile (() => DOTween.IsTweening ("Menu"));

		Debug.Log ("Hide : " + (Time.time - time).ToString ());
	}


	IEnumerator HideMenuCoroutine (MenuComponent whichMenu, int submitButton = -1)
	{
		StartCoroutine (HideUnderMenus (whichMenu, submitButton));
		StartCoroutine (HideMainContent (whichMenu));
		StartCoroutine (HideSecondaryContent (whichMenu));

		if (submitButton != -1 && headerButtonsList.Count > 0)
			HidePreviousHeader (0);

		yield return new WaitForSecondsRealtime (animationDuration);

		currentMenu = null;
	}


	IEnumerator HideUnderMenus (MenuComponent whichMenu, int submitButton = -1)
	{
		int delay = 0;

		//Under Menus Buttons
		for(int i = whichMenu.underMenusButtons.Count - 1; i >= 0; i--)
		{
			if(i != submitButton)
			{
				Disable (whichMenu.underMenusButtons [i], animationDuration + ButtonsDelay (delay));
				SetNonInteractable (whichMenu.underMenusButtons [i]);

				whichMenu.underMenusButtons [i].DOAnchorPosX (menuOffScreenX, animationDuration).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
				delay++;
			}
		}

		//If Submiting
		if(submitButton != -1)
		{
			HidePreviousHeader (delay);
			
			PlaceCurrentHeader (whichMenu, submitButton, delay);
		}

		yield break;
	}

	void HidePreviousHeader (int delay)
	{
		//Hide Previous Header Button
		if(hidePreviousHeaderButton)
		{
			if(headerButtonsList.Count > 0)
			{
				Disable (headerButtonsList [headerButtonsList.Count - 1], animationDuration + ButtonsDelay (delay));
				SetNonInteractable (headerButtonsList [headerButtonsList.Count - 1]);

				headerButtonsList [headerButtonsList.Count - 1].DOAnchorPosX (menuOffScreenX, animationDuration).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
				headerButtonsList.RemoveAt (headerButtonsList.Count - 1);
			}
		}
	}

	Tween PlaceCurrentHeader (MenuComponent whichMenu, int submitButton, int delay)
	{
		//Place Submit Button as Header
		Enable (whichMenu.underMenusButtons [submitButton]);
		SetNonInteractable (whichMenu.underMenusButtons [submitButton]);

		return whichMenu.underMenusButtons [submitButton].DOAnchorPos (new Vector2(menuOnScreenX, MenuHeaderButtonPosition ()) - whichMenu.menusParent.anchoredPosition, animationDuration)
			.SetDelay (ButtonsDelay (delay))
			.SetEase (easeMenu)
			.OnComplete (()=> headerButtonsList.Add (whichMenu.underMenusButtons [submitButton]))
			.SetId ("Menu");
	}
		
	IEnumerator HideMainContent (MenuComponent whichMenu)
	{
		whichMenu.mainContent.DOAnchorPos (offScreenContent, animationDuration).SetEase (easeMenu).SetId ("Menu").OnComplete (()=> Disable(whichMenu.mainContent));

		yield break;
	}

	IEnumerator HideSecondaryContent (MenuComponent whichMenu)
	{
		float waitDelay = 0;

		//Secondary Content
		for(int i = 0; i < whichMenu.secondaryContents.Count; i++)
		{
			if (whichMenu.secondaryContents [i].delay > waitDelay)
				waitDelay = whichMenu.secondaryContents [i].delay;
			
			Disable (whichMenu.secondaryContents [i].content, animationDuration + whichMenu.secondaryContents [i].delay);
			whichMenu.secondaryContents [i].content.DOAnchorPos (whichMenu.secondaryContents [i].offScreenPos, animationDuration).SetDelay (whichMenu.secondaryContents [i].delay).SetEase (easeMenu).SetId ("Menu");
		}			

		yield break;
	}
	#endregion

	#region Other Methods
	public enum WhichOverrideSettings { HeaderPos = 1, MenuPos = 2, ButtonPos = 4, ContentPos = 8, All = 16}

	public float ButtonsDelay (int i)
	{
		return buttonsDelay * i;
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


	IEnumerator SelectPreviousElement (MenuComponent whichMenu)
	{
		GameObject selectable = null;

		if (whichMenu.previousSelected != null && selectPreviousElement)
			selectable = whichMenu.previousSelected;

		else if(whichMenu.selectable != null)
			selectable = whichMenu.selectable;

		eventSyst.SetSelectedGameObject (null);

		if(selectable != null)
		{
			Button button = selectable.GetComponent<Button> ();

			if(button != null)
				yield return new WaitUntil (() => button.interactable == true);

			eventSyst.SetSelectedGameObject (selectable);
		}

		yield return 0;
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
				backButtons.DOAnchorPosX (backButtonsXPos.y, animationDuration).SetEase (easeMenu);
		}
		else
		{
			if (backButtons.anchoredPosition.x != backButtonsXPos.x)
				backButtons.DOAnchorPosX (backButtonsXPos.x, animationDuration).SetEase (easeMenu).OnComplete (()=> backButtons.gameObject.SetActive (false));
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
			cameraMovement.StartCoroutine ("NewMenuPosition");
			//cameraMovement.StartCoroutine ("PausePosition");

			yield return new WaitForSecondsRealtime(cameraMovement.newMovementDuration - 0.5f);

			ShowMenu (mainMenuScript);
		}
		else
		{
  			MasterAudio.PlaySound (SoundsManager.Instance.closeMenuSound);

			yield return cameraMovement.StartCoroutine ("NewPlayPosition");
			//yield return cameraMovement.StartCoroutine ("PlayPosition");

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

		yield return new WaitForSecondsRealtime(animationDuration);

		MasterAudio.PlaySound (SoundsManager.Instance.closeMenuSound);

		yield return cameraMovement.StartCoroutine ("NewPlayPosition");
//		yield return cameraMovement.StartCoroutine ("PlayPosition");

		GlobalVariables.Instance.GameState = GameStateEnum.Playing;
	}

	IEnumerator StartRandomModeCoroutine ()
	{
		mainCamera.GetComponent<SlowMotionCamera> ().StopPauseSlowMotion ();
		currentMenu.HideMenu ();

		LoadModeManager.Instance.LoadRandomScene ();

		yield return new WaitForSecondsRealtime (cameraMovement.newMovementDuration * 2);

		yield return new WaitForSecondsRealtime(animationDuration);

		MasterAudio.PlaySound (SoundsManager.Instance.closeMenuSound);

		yield return cameraMovement.StartCoroutine ("NewPlayPosition");
		//yield return cameraMovement.StartCoroutine ("PlayPosition");

		GlobalVariables.Instance.GameState = GameStateEnum.Playing;
	}

	IEnumerator StartRandomCocktailModeCoroutine ()
	{
		mainCamera.GetComponent<SlowMotionCamera> ().StopPauseSlowMotion ();
		currentMenu.HideMenu ();

		GlobalVariables.Instance.currentCocktailModes.AddRange (GlobalVariables.Instance.selectedCocktailModes);

		LoadModeManager.Instance.LoadRandomCocktailScene ();

		yield return new WaitForSecondsRealtime (cameraMovement.newMovementDuration * 2);

		yield return new WaitForSecondsRealtime(animationDuration);

		MasterAudio.PlaySound (SoundsManager.Instance.closeMenuSound);

		yield return cameraMovement.StartCoroutine ("NewPlayPosition");
		//yield return cameraMovement.StartCoroutine ("PlayPosition");

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
		yield return cameraMovement.StartCoroutine ("NewMenuPosition");
		//yield return cameraMovement.StartCoroutine ("EndModePosition");



		//Secondary Content
		if(secondaryContentList != null)
		{
			for(int i = 0; i < secondaryContentList.Count; i++)
			{
				secondaryContentList [i].content.anchoredPosition = secondaryContentList [i].offScreenPos;

				Enable (secondaryContentList [i].content);
				secondaryContentList [i].content.DOAnchorPos (secondaryContentList [i].onScreenPos, animationDuration).SetDelay (secondaryContentList [i].delay).SetEase (easeMenu).SetId ("Menu");

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

		yield return new WaitForSecondsRealtime (animationDuration);

		panelBackground.DOSizeDelta (initialPanelSize, animationDuration).SetEase (easeMenu).SetId ("Menu");

		for (int i = 0; i < content.transform.childCount; i++)
			content.transform.GetChild (i).GetComponent<RectTransform> ().DOScale(1, animationDuration).SetDelay(delayBetweenStats * i).SetEase (easeMenu).SetId ("Menu");

		for (int i = 0; i < playerScore.Length; i++)
			playerScore[i].DOScale(1, animationDuration).SetDelay(delayBetweenStats * i).SetEase (easeMenu).SetId ("Menu");
		
		StartCoroutine (SelectPreviousElement (whichMenu));

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

		MasterAudio.PlaySound (SoundsManager.Instance.openMenuSound);

		//LoadModeManager.Instance.ReloadSceneVoid ();

		GlobalVariables.Instance.GameState = GameStateEnum.Menu;

		ShowMenu (mainMenuScript);
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

		LoadModeManager.Instance.RestartSceneVoid (false);
	}

	IEnumerator HideEndMode (RectTransform content, List<SecondaryContent> secondaryContentList)
	{
		eventSyst.SetSelectedGameObject (null);
		currentMenu = null;

		if(content != null)
			for (int i = 0; i < content.transform.childCount; i++)
				content.transform.GetChild (i).GetComponent<RectTransform> ().DOScale(0, animationDuration).SetDelay(delayBetweenStats * i).SetEase (easeMenu).SetId ("Menu");
		

		for (int i = 0; i < playerScore.Length; i++)
		{
			Disable (playerScore [i], animationDuration);
			playerScore[i].DOScale(0, animationDuration).SetDelay(delayBetweenStats * i).SetEase (easeMenu).SetId ("Menu");
		}

		yield return new WaitForSecondsRealtime(0.01f);

		Tween myTween = panelBackground.DOSizeDelta(modifiedPanelSize, animationDuration).SetEase (easeMenu).SetId ("Menu");		
		yield return myTween.WaitForCompletion ();

		if(secondaryContentList != null)
		{
			for(int i = 0; i < secondaryContentList.Count; i++)
			{
				Disable (secondaryContentList [i].content, animationDuration + secondaryContentList [i].delay);
				secondaryContentList [i].content.DOAnchorPos (secondaryContentList [i].offScreenPos, animationDuration).SetDelay (secondaryContentList [i].delay).SetEase (easeMenu).SetId ("Menu");

				if (secondaryContentList [i].content.GetComponent<Button> ())
					SetNonInteractable (secondaryContentList [i].content);
			}
		}

		if(content != null)
			Disable (content);
	}
	#endregion

	#region Tutorial
	public void LoadTutorial ()
	{
		LoadModeManager.Instance.LoadSceneVoid (WhichMode.Tutorial);
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
