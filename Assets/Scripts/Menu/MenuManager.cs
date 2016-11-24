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

	[Header ("MainMenu Buttons Positions")]
	public float mainMenuFirstButtonY = 100;
	public float[] mainMenuButtonsYPositions = new float[9];
	
	[Header ("Buttons Positions")]
	public float firstButtonY = 100;
	public float gapBetweenButtons = 131;
	public float[] buttonsYPositions = new float[9];
	
	[Header ("Header Buttons")]
	public float headerButtonsYPosition = 540;
	public List<RectTransform> headerButtonsList;

	[Header ("Buttons Delay")]
	public float[] buttonsDelay = {0, 0.05f, 0.1f, 0.15f, 0.2f, 0.25f};

	private EventSystem eventSyst;
	private GameObject mainCamera;
	private ControlMapper controlMapper;
	private LoadModeManager loadModeScript;
	private MenuCameraMovement cameraMovement;
	private Player[] playerList = new Player[5];
	private ControllerChangeManager1 controllerManager;

	private bool startScreen = true;

	public bool test =false;


	#endregion

	#region Setup
	void Start () 
	{
		DOTween.Init();
		DOTween.defaultTimeScaleIndependent = true;

		mainMenu.SetActive (true);
		mainMenuScript = mainMenu.GetComponent<MenuComponent> ();
			
		eventSyst = GameObject.FindGameObjectWithTag ("EventSystem").GetComponent<EventSystem> ();
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		cameraMovement = mainCamera.GetComponent<MenuCameraMovement> ();
		controlMapper = GameObject.FindGameObjectWithTag ("ControlMapper").GetComponent<ControlMapper> ();
		loadModeScript = GameObject.FindObjectOfType<LoadModeManager> ();
		//controllerManager = GameObject.FindGameObjectWithTag ("ControllerChangeManager").GetComponent<ControllerChangeManager1> ();

		SetupButtonsPositions ();
	}

	void SetupButtonsPositions ()
	{
		for (int i = 0; i < mainMenuButtonsYPositions.Length; i++)
			mainMenuButtonsYPositions [i] = mainMenuFirstButtonY - (gapBetweenButtons * i);
		
		for (int i = 0; i < buttonsYPositions.Length; i++)
			buttonsYPositions [i] = firstButtonY - (gapBetweenButtons * i);
	}

	void StartScreen ()
	{
		MasterAudio.PlaySound (GameSoundsManager.Instance.gameStartSound);

		cameraMovement.StartScreen ();

		MainMenu ();
	}
	#endregion

	void Update () 
	{
		GetMenuPlayers ();

		for(int i = 0; i < playerList.Length; i++)
		{
			if (playerList[i] != null && playerList [i].GetButtonDown ("UI Cancel"))
				currentMenu.Cancel ();

			if(startScreen)
			{
				if(playerList[i] != null  && playerList[i].GetButtonDown("UI Submit") || playerList[i] != null && playerList[i].GetButtonDown("UI Start") || Input.GetMouseButtonDown(0))
				{
					StartScreen ();
					startScreen = false;
				}
			}
		}

		if(test)
		{
			test = false;

			currentMenu.Resume ();			
		}
	}

	void GetMenuPlayers ()
	{
		playerList [0] = ReInput.players.GetPlayer (0);

		for(int i = 0; i < GamepadsManager.Instance.gamepadsList.Count; i++)
		{
			if(GamepadsManager.Instance.gamepadsList[i].GamepadId == 1)
			{
				playerList [1] = ReInput.players.GetPlayer (1);
				playerList [1].controllers.AddController(GamepadsManager.Instance.gamepadsList[i].GamepadController, true);
			}

			if(GamepadsManager.Instance.gamepadsList[i].GamepadId == 2)
			{
				playerList [2] = ReInput.players.GetPlayer (2);
				playerList [2].controllers.AddController(GamepadsManager.Instance.gamepadsList[i].GamepadController, true);
			}

			if(GamepadsManager.Instance.gamepadsList[i].GamepadId == 3)
			{
				playerList [3] = ReInput.players.GetPlayer (3);
				playerList [3].controllers.AddController(GamepadsManager.Instance.gamepadsList[i].GamepadController, true);
			}

			if(GamepadsManager.Instance.gamepadsList[i].GamepadId == 4)
			{
				playerList [4] = ReInput.players.GetPlayer (4);
				playerList [4].controllers.AddController(GamepadsManager.Instance.gamepadsList[i].GamepadController, true);
			}
		}
	}

	#region Main Menu		
	void MainMenu ()
	{
		List<RectTransform> underButtonsList = mainMenuScript.underButtonsList;

		for (int i = 0; i < underButtonsList.Count; i++)
			underButtonsList [i].anchoredPosition = new Vector2 (offScreenX, mainMenuButtonsYPositions [i]);

		int underDelay = 0;

		for(int i = underButtonsList.Count - 1; i >= 0; i--)
		{
			Enable (underButtonsList [i]);
			SetInteractable (underButtonsList [i], durationToShow + buttonsDelay [underDelay]);

			underButtonsList [i].DOAnchorPosX (onScreenX, durationToShow).SetDelay (buttonsDelay [underDelay]).SetEase (easeMenu);
			underDelay++;
		}

		if(!startScreen)
			cameraMovement.ShowLogo ();

		underButtonsList [0].GetComponent<Button> ().Select ();

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
		underButtonsList [cancelButton].DOAnchorPos (new Vector2(onScreenX, mainMenuButtonsYPositions[cancelButton]), durationToShow).SetDelay (buttonsDelay [delay]).SetEase (easeMenu);
		headerButtonsList.RemoveAt (headerButtonsList.Count - 1);

		delay++;

		for(int i = underButtonsList.Count - 1; i >= 0; i--)
		{
			if(i != cancelButton)
			{
				Enable (underButtonsList [i]);
				SetInteractable (underButtonsList [i], durationToShow + buttonsDelay [delay]);

				underButtonsList [i].DOAnchorPosX (onScreenX, durationToShow).SetDelay (buttonsDelay [delay]).SetEase (easeMenu);
				delay++;
			}
		}

		if(!startScreen)
			cameraMovement.ShowLogo ();

		underButtonsList [0].gameObject.GetComponent<Button> ().Select ();

		currentMenu = mainMenuScript;
	}

	public void HideMainMenu ()
	{
		List<RectTransform> underButtonsList = mainMenuScript.underButtonsList;

		int underDelay = 0;

		for(int i = underButtonsList.Count - 1; i >= 0; i--)
		{
			Disable (underButtonsList [i], durationToHide + buttonsDelay [underDelay]);
			SetNonInteractable (underButtonsList [i]);

			underButtonsList [i].DOAnchorPosX (offScreenX, durationToHide).SetDelay (buttonsDelay [underDelay]).SetEase (easeMenu);
			underDelay++;
		}

		cameraMovement.HideLogo ();

		currentMenu = null;
	}
	#endregion		

	#region Submit Methods
	public void ShowUnderButtons (List<RectTransform> otherButtonsList, int submitButton, List<RectTransform> underButtonsList, MenuComponent whichMenu)
	{
		StartCoroutine (ShowUnderButtonsCoroutine (otherButtonsList, submitButton, underButtonsList, whichMenu));
	}

	IEnumerator ShowUnderButtonsCoroutine (List<RectTransform> otherButtonsList, int submitButton, List<RectTransform> underButtonsList, MenuComponent whichMenu)
	{
		PlaySubmitSound ();

		yield return HideOtherButtons (otherButtonsList, submitButton).WaitForCompletion();			

		//Show Under Menu Buttons
		for (int i = 0; i < underButtonsList.Count; i++)
			underButtonsList [i].anchoredPosition = new Vector2 (offScreenX, buttonsYPositions [i] + GapAfterHeaderButton ());

		int underDelay = 0;

		for(int i = underButtonsList.Count - 1; i >= 0; i--)
		{
			Enable (underButtonsList [i]);
			SetInteractable (underButtonsList [i], durationToShow + buttonsDelay [underDelay]);

			underButtonsList [i].DOAnchorPosX (onScreenX, durationToShow).SetDelay (buttonsDelay [underDelay]).SetEase (easeMenu);
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
		PlaySubmitSound ();

		yield return HideOtherButtons (otherButtonsList, submitButton).WaitForCompletion();			

		content.anchoredPosition = new Vector2 (offScreenX, 0);
		Enable (content);

		content.DOAnchorPosX (0, durationContent).SetEase (easeMenu);

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
				Disable (otherButtonsList [i], durationToHide + buttonsDelay [aboveDelay]);
				SetNonInteractable (otherButtonsList [i]);

				otherButtonsList [i].DOAnchorPosX (offScreenX, durationToHide).SetDelay (buttonsDelay [aboveDelay]).SetEase (easeMenu);
				aboveDelay++;
			}
		}

		SetNonInteractable (otherButtonsList [submitButton]);
		return otherButtonsList [submitButton].DOAnchorPos (new Vector2(onScreenX, HeaderButtonPosition ()), durationToShow).SetDelay (buttonsDelay [aboveDelay]).SetEase (easeMenu).OnComplete (()=> headerButtonsList.Add (otherButtonsList [submitButton]));
	}
	#endregion

	#region Cancel Methods
	public void HideUnderButtons (List<RectTransform> underButtonsList, MenuComponent aboveMenu, RectTransform menuButton, bool resume = false)
	{
		if(!resume)
			StartCoroutine (HideUnderButtonsCoroutine (underButtonsList, aboveMenu, menuButton));
		else
			StartCoroutine (HideUnderButtonsResumeCoroutine (underButtonsList, aboveMenu, menuButton));
	}

	IEnumerator HideUnderButtonsCoroutine (List<RectTransform> underButtonsList, MenuComponent aboveMenu, RectTransform menuButton)
	{
		int delay = 0;
		Tween tween = null;

		PlayReturnSound ();

		for(int i = underButtonsList.Count - 1; i >= 0; i--)
		{
			Disable (underButtonsList [i], durationToHide + buttonsDelay [delay]);
			SetNonInteractable (underButtonsList [i]);

			tween = underButtonsList [i].DOAnchorPosX (offScreenX, durationToHide).SetDelay (buttonsDelay [delay]).SetEase (easeMenu);
			delay++;
		}

		yield return tween.WaitForCompletion();

		if (aboveMenu.menuComponentType == MenuComponentType.MainMenu)
			MainMenu (menuButton);
		else
			ShowAboveButtons (aboveMenu, menuButton);

	}

	IEnumerator HideUnderButtonsResumeCoroutine (List<RectTransform> underButtonsList, MenuComponent aboveMenu, RectTransform menuButton)
	{
		int delay = 0;
		Tween tween = null;

		PlayReturnSound ();

		for(int i = underButtonsList.Count - 1; i >= 0; i--)
		{
			Disable (underButtonsList [i], durationToHide + buttonsDelay [delay]);
			SetNonInteractable (underButtonsList [i]);

			tween = underButtonsList [i].DOAnchorPosX (offScreenX, durationToHide).SetDelay (buttonsDelay [delay]).SetEase (easeMenu);
			delay++;
		}

		yield return tween.WaitForCompletion();

		HideHeaderButtons ();

		currentMenu = null;

	}

	public void HideContent (RectTransform content, MenuComponent aboveMenu, RectTransform menuButton, bool resume = false)
	{
		if(!resume)
			StartCoroutine (HideContentCoroutine (content, aboveMenu, menuButton));
		else
			StartCoroutine (HideContentResumeCoroutine (content, aboveMenu, menuButton));
	}

	IEnumerator HideContentCoroutine (RectTransform content, MenuComponent aboveMenu, RectTransform menuButton)
	{
		Tween tween = content.DOAnchorPosX (offScreenX, durationContent).SetEase (easeMenu).OnComplete (()=> Disable(content));

		PlayReturnSound ();

		yield return tween.WaitForCompletion();
	
		if (aboveMenu.menuComponentType == MenuComponentType.MainMenu)
			MainMenu (menuButton);
		else
			ShowAboveButtons (aboveMenu, menuButton);

	}

	IEnumerator HideContentResumeCoroutine (RectTransform content, MenuComponent aboveMenu, RectTransform menuButton)
	{
		Tween tween = content.DOAnchorPosX (offScreenX, durationContent).SetEase (easeMenu).OnComplete (()=> Disable(content));

		PlayReturnSound ();

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

		SetInteractable (aboveMenu.underButtonsList [cancelButton], durationToShow + buttonsDelay [delay]);
		headerButtonsList.RemoveAt (headerButtonsList.Count - 1);
		aboveMenu.underButtonsList [cancelButton].DOAnchorPos (new Vector2(onScreenX, buttonsYPositions[cancelButton] + GapAfterHeaderButton ()), durationToShow).SetDelay (buttonsDelay [delay]).SetEase (easeMenu);
		delay++;

		for(int i = aboveMenu.underButtonsList.Count - 1; i >= 0; i--)
		{
			if(i != cancelButton)
			{
				Enable (aboveMenu.underButtonsList [i]);
				SetInteractable (aboveMenu.underButtonsList [i], durationToShow + buttonsDelay [delay]);

				aboveMenu.underButtonsList [i].DOAnchorPosX (onScreenX, durationToShow).SetDelay (buttonsDelay [delay]).SetEase (easeMenu);
				delay++;
			}
		}

		aboveMenu.underButtonsList [0].gameObject.GetComponent<Button> ().Select ();

		currentMenu = aboveMenu;
	}

	void HideHeaderButtons ()
	{
		int delay = 0;
		Tween tween = null;

		for(int i = headerButtonsList.Count - 1; i >= 0; i--)
		{
			Disable (headerButtonsList [i], durationToHide + buttonsDelay [delay]);
			SetNonInteractable (headerButtonsList [i]);

			tween = headerButtonsList [i].DOAnchorPosX (offScreenX, durationToHide).SetDelay (buttonsDelay [delay]).SetEase (easeMenu);
			delay++;
		}
	}

	#endregion

	#region Other Methods
	float HeaderButtonPosition ()
	{
		return headerButtonsYPosition - gapBetweenButtons * headerButtonsList.Count;
	}

	float GapAfterHeaderButton ()
	{
		return -gapBetweenButtons * headerButtonsList.Count;
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
			HideMainMenu ();
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
}
