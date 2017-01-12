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
	public bool tweening;

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

	[Header ("Menu Element To Enable")]
	public List<GameObject> elementsToEnable;

	[Header ("MainMenu Buttons Positions")]
	public float mainMenuFirstButtonY = 100;
	
	[Header ("Buttons Positions")]
	public float firstButtonY = 100;
	public float gapBetweenButtons = 131;
	
	[Header ("Header Buttons")]
	public float headerButtonsYPosition = 540;
	public List<RectTransform> headerButtonsList;

	[Header ("Buttons Delay")]
	public float buttonsDelay = 0.05f;

	[Header ("Viewport Buttons Positions")]
	public float viewportYOffscreen;
	public float viewportButtonsDelay = 0.05f;
	public float viewportButtonsMovementDuration;
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
		MasterAudio.PlaySound (GameSoundsManager.Instance.gameStartSound);

		cameraMovement.StartScreen ();

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
	}

	void CheckMenuInput ()
	{
		if(!DOTween.IsTweening ("Menu") && !DOTween.IsTweening ("MenuCamera"))
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
				
				if (GlobalVariables.Instance.GameState != GameStateEnum.Playing && GlobalVariables.Instance.rewiredPlayers [i].GetButtonDown ("UI Cancel"))
					currentMenu.Cancel ();
			}			
		}
	}

	void CheckPauseInput ()
	{
		if(!DOTween.IsTweening ("Menu") && !DOTween.IsTweening ("MenuCamera"))
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
		if(!DOTween.IsTweening ("Menu"))
			currentMenu.Cancel ();
	}

	void CheckNothingSelected ()
	{
		if(eventSyst.currentSelectedGameObject == null && currentMenu != null)
		{
			if(currentMenu.selectable != null)
			{
				if (currentMenu.selectable.GetComponent<Button> () && currentMenu.selectable.GetComponent<Button> ().interactable == false)
					return;

				eventSyst.SetSelectedGameObject (null);
				eventSyst.SetSelectedGameObject (currentMenu.selectable);
			}
		}
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
		List<RectTransform> underButtonsList = mainMenuScript.underButtonsList;

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
			cameraMovement.ShowLogo ();

		SetInteractable (underButtonsList [0], ButtonsDelay (underDelay) * 0.5f);
		DOVirtual.DelayedCall (ButtonsDelay (underDelay), ()=> {underButtonsList [0].GetComponent<Button> ().Select (); Debug.Log (underButtonsList [0].GetComponent<Button> ().interactable);}); 

		currentMenu = mainMenuScript;
	}

	void MainMenu (RectTransform menuButton)
	{
		List<RectTransform> underButtonsList = mainMenuScript.underButtonsList;

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
			cameraMovement.ShowLogo ();

		SetInteractable (underButtonsList [0], ButtonsDelay (delay) * 0.5f);
		DOVirtual.DelayedCall (ButtonsDelay (delay), ()=> underButtonsList [0].GetComponent<Button> ().Select ()); 

		currentMenu = mainMenuScript;
	}

	public void HideMainMenu ()
	{
		List<RectTransform> underButtonsList = mainMenuScript.underButtonsList;

		int underDelay = 0;

		for(int i = underButtonsList.Count - 1; i >= 0; i--)
		{
			Disable (underButtonsList [i], durationToHide + ButtonsDelay (underDelay));
			SetNonInteractable (underButtonsList [i]);

			underButtonsList [i].DOAnchorPosX (offScreenX, durationToHide).SetDelay (ButtonsDelay (underDelay)).SetEase (easeMenu).SetId ("Menu");
			underDelay++;
		}

		cameraMovement.HideLogo ();

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

		yield return HideOtherButtons (otherButtonsList, submitButton).WaitForCompletion();			

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
				secondaryContentList [i].content.anchoredPosition = secondaryContentList [i].offScreenPos;
				
				Enable (secondaryContentList [i].content);
				secondaryContentList [i].content.DOAnchorPos (secondaryContentList [i].onScreenPos, durationToShow).SetDelay (secondaryContentList [i].delay).SetEase (easeMenu).SetId ("Menu");
			}			
		}

		//Select First Under Menu Button
		SetInteractable (underButtonsList [0], ButtonsDelay (underDelay) * 0.5f);
		DOVirtual.DelayedCall (ButtonsDelay (underDelay), ()=> underButtonsList [0].GetComponent<Button> ().Select ()); 

		currentMenu = whichMenu;
	}

	public void ShowContent (List<RectTransform> otherButtonsList, int submitButton, RectTransform content, MenuComponent whichMenu, List<SecondaryContent> secondaryContentList = null)
	{
		StartCoroutine (ShowContentCoroutine (otherButtonsList, submitButton, content, whichMenu, secondaryContentList));
	}

	IEnumerator ShowContentCoroutine (List<RectTransform> otherButtonsList, int submitButton, RectTransform content, MenuComponent whichMenu, List<SecondaryContent> secondaryContentList = null)
	{
		PlaySubmitSound ();

		yield return HideOtherButtons (otherButtonsList, submitButton).WaitForCompletion();			

		content.anchoredPosition = new Vector2 (offScreenX, content.anchoredPosition.y);
		Enable (content);

		content.DOAnchorPosX (0, durationContent).SetEase (easeMenu).SetId ("Menu");

		//Secondary Content
		if(secondaryContentList != null)
		{
			for(int i = 0; i < secondaryContentList.Count; i++)
			{
				secondaryContentList [i].content.anchoredPosition = secondaryContentList [i].offScreenPos;
				
				Enable (secondaryContentList [i].content);
				secondaryContentList [i].content.DOAnchorPos (secondaryContentList [i].onScreenPos, durationToShow).SetDelay (secondaryContentList [i].delay).SetEase (easeMenu).SetId ("Menu");
			}			
		}

		if (whichMenu.selectable != null)
		{
			if(whichMenu.selectable.GetComponent<Button> () != null && whichMenu.selectable.GetComponent<Button> ().interactable)
				eventSyst.SetSelectedGameObject (whichMenu.selectable);
			else if(whichMenu.selectable.GetComponent<Button> () == null)
				eventSyst.SetSelectedGameObject (whichMenu.selectable);
		}

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
				Disable (otherButtonsList [i], durationToHide + ButtonsDelay (aboveDelay));
				SetNonInteractable (otherButtonsList [i]);

				otherButtonsList [i].DOAnchorPosX (offScreenX, durationToHide).SetDelay (ButtonsDelay (aboveDelay)).SetEase (easeMenu).SetId ("Menu");
				aboveDelay++;
			}
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

		yield return HideOtherButtons (otherButtonsList, submitButton).WaitForCompletion();			

		//Show Under Menu Buttons
		for (int i = 0; i < underButtonsList.Count; i++)
			underButtonsList [i].anchoredPosition = new Vector2 (onScreenX, viewportYOffscreen);

		int underDelay = 0;

		for(int i = underButtonsList.Count - 1; i >= 0; i--)
		{
			Enable (underButtonsList [i]);
			SetInteractable (underButtonsList [i], durationToShow + ButtonsDelay (underDelay));

			underButtonsList [i].DOAnchorPosY (ButtonsYPos (i) + GapAfterHeaderButton (), durationToShow).SetDelay (ViewportButtonsDelay (underDelay)).SetEase (easeMenu).SetId ("Menu");
			underDelay++;
		}

		//Secondary Content
		if(secondaryContentList != null)
		{
			for(int i = 0; i < secondaryContentList.Count; i++)
			{
				secondaryContentList [i].content.anchoredPosition = secondaryContentList [i].offScreenPos;

				Enable (secondaryContentList [i].content);
				secondaryContentList [i].content.DOAnchorPos (secondaryContentList [i].onScreenPos, durationToShow).SetDelay (secondaryContentList [i].delay).SetEase (easeMenu).SetId ("Menu");
			}			
		}

		//Select First Under Menu Button
		SetInteractable (underButtonsList [0], ButtonsDelay (underDelay) * 0.5f);
		DOVirtual.DelayedCall (ButtonsDelay (underDelay), ()=> underButtonsList [0].GetComponent<Button> ().Select ()); 

		currentMenu = whichMenu;
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
			ShowAboveButtons (aboveMenu, menuButton);

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
			ShowAboveButtons (aboveMenu, menuButton);

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
				Disable (secondaryContentList [i].content, durationToHide + secondaryContentList [i].delay);
				secondaryContentList [i].content.DOAnchorPos (secondaryContentList [i].offScreenPos, durationToHide).SetDelay (secondaryContentList [i].delay).SetEase (easeMenu).SetId ("Menu");
			}			
		}

		//Under Buttons
		for(int i = 0; i < underButtonsList.Count; i++)
		{
			Disable (underButtonsList [i], durationToHide + ViewportButtonsDelay (delay));
			SetNonInteractable (underButtonsList [i]);

			tween = underButtonsList [i].DOAnchorPosY (viewportYOffscreen, durationToHide).SetDelay (ViewportButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
			delay++;
		}

		yield return tween.WaitForCompletion();

		if (aboveMenu.menuComponentType == MenuComponentType.MainMenu)
			MainMenu (menuButton);
		else
			ShowAboveButtons (aboveMenu, menuButton);
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
				Disable (secondaryContentList [i].content, durationToHide + secondaryContentList [i].delay);
				secondaryContentList [i].content.DOAnchorPos (secondaryContentList [i].offScreenPos, durationToHide).SetDelay (secondaryContentList [i].delay).SetEase (easeMenu).SetId ("Menu");
			}			
		}

		//Under Buttons
		for(int i = 0; i < underButtonsList.Count; i++)
		{
			Disable (underButtonsList [i], durationToHide + ViewportButtonsDelay (delay));
			SetNonInteractable (underButtonsList [i]);

			tween = underButtonsList [i].DOAnchorPosY (viewportYOffscreen, durationToHide).SetDelay (ViewportButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
			delay++;
		}

		yield return tween.WaitForCompletion();

		HideHeaderButtons ();

		currentMenu = null;
	}

	void ShowAboveButtons (MenuComponent aboveMenu, RectTransform menuButton)
	{
		int cancelButton = 0;

		for (int i = 0; i < aboveMenu.underButtonsList.Count; i++)
			if (aboveMenu.underButtonsList [i] == menuButton)
				cancelButton = i;

		int delay = 0;

		SetInteractable (aboveMenu.underButtonsList [cancelButton], durationToShow + ButtonsDelay (delay));
		headerButtonsList.RemoveAt (headerButtonsList.Count - 1);
		aboveMenu.underButtonsList [cancelButton].DOAnchorPos (new Vector2(onScreenX, ButtonsYPos (cancelButton) + GapAfterHeaderButton ()), durationToShow).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
		delay++;

		for(int i = aboveMenu.underButtonsList.Count - 1; i >= 0; i--)
		{
			if(i != cancelButton)
			{
				aboveMenu.underButtonsList [i].anchoredPosition = new Vector2(offScreenX, ButtonsYPos (i) + GapAfterHeaderButton ());

				Enable (aboveMenu.underButtonsList [i]);
				SetInteractable (aboveMenu.underButtonsList [i], durationToShow + ButtonsDelay (delay));

				aboveMenu.underButtonsList [i].DOAnchorPosX (onScreenX, durationToShow).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
				delay++;
			}
		}

		SetInteractable (aboveMenu.underButtonsList [0], ButtonsDelay (delay) * 0.5f);
		DOVirtual.DelayedCall (ButtonsDelay (delay), ()=> aboveMenu.underButtonsList [0].gameObject.GetComponent<Button> ().Select ()); 

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

	void Enable (RectTransform target)
	{
		target.gameObject.SetActive (true);
	}

	void Disable (RectTransform target, float delayDuration = 0)
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


	void Tweening ()
	{
		tweening = true;
	}

	void NotTweening ()
	{
		tweening = false;
	}


	void PlaySubmitSound ()
	{
		MasterAudio.PlaySound (GameSoundsManager.Instance.menuSubmit);
	}

	void PlayReturnSound ()
	{
		MasterAudio.PlaySound (GameSoundsManager.Instance.menuCancel);
	}


	void BackButtons ()
	{
		if(GlobalVariables.Instance.GameState != GameStateEnum.Playing && currentMenu && currentMenu.menuComponentType != MenuComponentType.MainMenu && currentMenu.menuComponentType != MenuComponentType.EndModeMenu)
		{
			if (backButtons.gameObject.activeSelf == false)
				backButtons.gameObject.SetActive (true);

			if (backButtons.anchoredPosition.x != backButtonsXPos.y)
				backButtons.DOAnchorPosX (backButtonsXPos.y, durationContent).SetEase (easeMenu).SetId ("Menu");
		}
		else
		{
			if (backButtons.anchoredPosition.x != backButtonsXPos.x)
				backButtons.DOAnchorPosX (backButtonsXPos.x, durationContent).SetEase (easeMenu).SetId ("Menu").OnComplete (()=> backButtons.gameObject.SetActive (false));
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

			MasterAudio.PlaySound (GameSoundsManager.Instance.openMenuSound);

			mainCamera.GetComponent<SlowMotionCamera> ().StartPauseSlowMotion ();
			cameraMovement.PausePosition ();

			yield return new WaitForSecondsRealtime(cameraMovement.cameraMovementDuration - 0.5f);

			MainMenu ();
		}
		else
		{
			cameraMovement.HideLogo ();

			yield return new WaitForSecondsRealtime(durationToHide);

			MasterAudio.PlaySound (GameSoundsManager.Instance.closeMenuSound);

			cameraMovement.PlayPosition ();

			yield return new WaitForSecondsRealtime(cameraMovement.cameraMovementDuration);

			mainCamera.GetComponent<SlowMotionCamera> ().StopPauseSlowMotion ();

			GlobalVariables.Instance.GameState = GameStateEnum.Playing;
		}
	}
	#endregion

	#region StartMode
	public void MenuLoadMode (string whichMode)
	{
		LoadModeManager.Instance.LoadSceneVoid (whichMode);
	}

	public void StartMode ()
	{
		StartCoroutine (StartModeCoroutine ());
	}

	IEnumerator StartModeCoroutine ()
	{
		mainCamera.GetComponent<SlowMotionCamera> ().StopPauseSlowMotion ();

		currentMenu.HideMenu ();

		yield return new WaitForSecondsRealtime(durationToHide);

		MasterAudio.PlaySound (GameSoundsManager.Instance.closeMenuSound);

		cameraMovement.PlayPosition ();

		yield return new WaitForSecondsRealtime(cameraMovement.cameraMovementDuration);

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
		cameraMovement.EndModePosition ();

		yield return new WaitForSecondsRealtime (cameraMovement.cameraMovementDuration);

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

		if (whichMenu.selectable != null)
		{
			if(whichMenu.selectable.GetComponent<Button> () != null && whichMenu.selectable.GetComponent<Button> ().interactable)
				eventSyst.SetSelectedGameObject (whichMenu.selectable);
			else if(whichMenu.selectable.GetComponent<Button> () == null)
				eventSyst.SetSelectedGameObject (whichMenu.selectable);
		}

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

		cameraMovement.MainMenuPosition ();
		yield return new WaitForSecondsRealtime(cameraMovement.cameraMovementDuration);

		MasterAudio.PlaySound (GameSoundsManager.Instance.openMenuSound);

		loadModeScript.ReloadSceneVoid ();

		GlobalVariables.Instance.GameState = GameStateEnum.Menu;
		MainMenu ();
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

		MasterAudio.PlaySound (GameSoundsManager.Instance.closeMenuSound);

		loadModeScript.RestartSceneVoid ();
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
