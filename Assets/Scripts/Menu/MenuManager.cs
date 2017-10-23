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
using GameAnalyticsSDK.Setup;
using Steamworks;
using Replay;

public class MenuManager : Singleton <MenuManager>
{
    #region Variables Declaration

    [Header("Infos")]
    public bool isTweening;
    public bool menuTweening;
    public MenuComponent currentMenu = null;
    public MenuAnimationType menuAnimationType = MenuAnimationType.None;
    public bool mouseControl = true;

    [Header("Animations Duration")]
    public float animationDuration = 0.15f;

    [Header("Buttons Delay")]
    public float buttonsDelay = 0.05f;

    [Header("Menus Buttons Positions")]
    public Vector2 offScreenButton;
    public Vector2 onScreenButton;

    [Header("MainContent Positions")]
    public Vector2 offScreenContent = new Vector2(-2000, 0);
    public Vector2 onScreenContent = new Vector2(0, 0);

    [Header("Header Buttons")]
    public Vector2 offScreenHeader;
    public Vector2 onScreenHeader;
    public float menuHeaderY = 540;
    public List<RectTransform> headerButtonsList;

    [Header("Main Menu")]
    public GameObject mainMenu;
    public Text playText;

    [Header("Instructions Menu")]
    public MenuComponent instructionsMenu;

    [Header("Buttons Positions")]
    public float menuFirstButtonY = 278;
    public float buttonFirstButtonY = 278;
    public float gapBetweenButtons = 131;

    [Header("Selectable")]
    public bool selectPreviousElement = true;

    [Header("Pass Fight Button")]
    public RectTransform passFightButton;

    [Header("Menu Elements To Enable")]
    public List<GameObject> elementsToEnable;

    [Header("Back Buttons")]
    public RectTransform backButtons;
    public GameObject escBackButton;

    [Header("Resume Buttons")]
    public RectTransform resumeButtons;
    public Vector2 resumeButtonsPositions;

    [Header("Disconnected Players")]
    public RectTransform[] unpluggedPlayers = new RectTransform[4];
    public Vector2 disconnectedPlayersYPos;

    [Header("End Mode Menu")]
    public MenuComponent endModeMenu;

    [Header("REPULSE Logos")]
    public Transform logosParent;

    [Header("Modes Logos")]
    public bool useStaticLogos = false;
    public Transform staticModesLogosParent;
    public Transform modesLogosCanvas;
    public float modesLogoDelay;
    public float modesLogoDuration;
    public float modesLogoDuration2;
    public Ease modesLogoEase;

    [HideInInspector]
    public Ease easeMenu = Ease.OutQuad;
    private MenuComponent mainMenuScript;

    private RectTransform endModecontent;
    private List<SecondaryContent> endModesecondaryContentList;

    [HideInInspector]
    public EventSystem eventSyst;
    private GameObject mainCamera;
    private LoadModeManager loadModeScript;
    private MenuCameraMovement cameraMovement;
    private bool isWaitingToSelect = false;
    private BackButtonsFeedback[] backButtonsScript = new BackButtonsFeedback[0];
    private ResumeButtonsFeedback[] resumeButtonsScript = new ResumeButtonsFeedback[0];
    private float modesLogoScale;
    private bool logoLoading = false;

    [HideInInspector]
    public bool startScreen = true;

    protected Callback<GameOverlayActivated_t> GameOverlayActivated;

    public Action OnQuitGame;

    #endregion

    #region Setup

    void Start()
    {
        DOTween.Init();
        DOTween.defaultTimeScaleIndependent = true;

        ReInput.ControllerConnectedEvent += (ControllerStatusChangedEventArgs obj) => GamepadsChange();
        ReInput.ControllerDisconnectedEvent += (ControllerStatusChangedEventArgs obj) => GamepadsChange();

        GlobalVariables.Instance.OnGamepadDisconnected += GamepadDisconnected;
        GlobalVariables.Instance.OnMenu += HideStaticModesLogos;
        GlobalVariables.Instance.OnStartMode += ModeLogo;
        GlobalVariables.Instance.OnRestartMode += ModeLogo;
        GlobalVariables.Instance.OnPlayerDeath += OnPlayerDeath;
        GlobalVariables.Instance.OnPlaying += () => eventSyst.SetSelectedGameObject(null);
        GlobalVariables.Instance.OnEndMode += () =>
        {
            HidePassFightButton();
            StopCoroutine(PassFight());
        };

        GlobalVariables.Instance.OnMenu += () =>
        {
            playText.text = "PLAY";

            escBackButton.SetActive(true);

            if (resumeButtons.anchoredPosition.x != resumeButtonsPositions.x)
                resumeButtons.DOAnchorPosX(resumeButtonsPositions.x, animationDuration).SetEase(easeMenu);
        };

        GlobalVariables.Instance.OnEndMode += () =>
        {
            playText.text = "PLAY";

            escBackButton.SetActive(true);

            if (resumeButtons.anchoredPosition.x != resumeButtonsPositions.x)
                resumeButtons.DOAnchorPosX(resumeButtonsPositions.x, animationDuration).SetEase(easeMenu);
        };

        GlobalVariables.Instance.OnPause += () =>
        {
            playText.text = "NEW";

            escBackButton.SetActive(false);

            if (resumeButtons.anchoredPosition.x != resumeButtonsPositions.y)
                resumeButtons.DOAnchorPosX(resumeButtonsPositions.y, animationDuration).SetEase(easeMenu);
        };

        OnStartModeClick += ShowStaticModesLogos;

        resumeButtons.DOAnchorPosX(resumeButtonsPositions.x, 0).SetEase(easeMenu);

        mainMenu.SetActive(true);
        mainMenuScript = mainMenu.GetComponent<MenuComponent>();
			
        eventSyst = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystem>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        cameraMovement = mainCamera.GetComponent<MenuCameraMovement>();

        backButtonsScript = backButtons.transform.GetComponentsInChildren<BackButtonsFeedback>();
        resumeButtonsScript = resumeButtons.transform.GetComponentsInChildren<ResumeButtonsFeedback>();

        modesLogoScale = modesLogosCanvas.localScale.x;
        modesLogosCanvas.localScale = Vector3.zero;
        modesLogosCanvas.gameObject.SetActive(false);

        foreach (Transform t in staticModesLogosParent)
            foreach (Transform c in t)
                c.gameObject.SetActive(false);

        foreach (var m in Resources.FindObjectsOfTypeAll<MenuComponent> ())
            m.SetupMenu();

        for (int i = 0; i < elementsToEnable.Count; i++)
            if (elementsToEnable[i] != null)
                elementsToEnable[i].SetActive(true);

        StartCoroutine(WaitStartScreen());
    }

    void OnEnable()
    {
        if (SteamManager.Initialized)
            GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnSteamGameOverlay);
    }

    void OnSteamGameOverlay(GameOverlayActivated_t callback)
    {
        if (callback.m_bActive != 0 && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
            PauseResumeGame();	
    }

    IEnumerator WaitStartScreen()
    {
        foreach (Transform t in logosParent)
            t.gameObject.SetActive(false);

        ShowLogo();

        bool startScreenInput = false;

        do
        {
            if (Input.GetMouseButton(0))
            {
                startScreenInput = true;
                break;
            }	

            for (int i = 0; i < 2; i++)
            {
                if (GlobalVariables.Instance.rewiredPlayers[i].GetAnyButton())
                {
                    startScreenInput = true;
                    break;
                }
            }

            yield return 0;
        }
        while (!startScreenInput);

        VibrationManager.Instance.Vibrate(1, FeedbackType.ButtonClick);

        MasterAudio.PlaySound(SoundsManager.Instance.gameStartSound);

        yield return StartCoroutine(cameraMovement.StartPosition());

        StartCoroutine(OnMenuChangeEvent(mainMenuScript));

        startScreen = false;
    }

    public IEnumerator HideLogo(bool start = false)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        //yield return new WaitForSeconds (cameraMovement.newMovementDuration);

        logoLoading = true;

        bool startScreenInput = false;

        do
        {
            for (int i = 0; i < 2; i++)
            {
                if (GlobalVariables.Instance.rewiredPlayers[i].GetAnyButtonDown())
                {
                    startScreenInput = true;
                    break;
                }
            }

            yield return 0;
        }
        while (!startScreenInput);

        if (cameraMovement.farPosition)
            yield break;

        logosParent.GetChild((int)GlobalVariables.Instance.environementChroma).transform.DOScale(Vector3.zero, animationDuration).SetEase(Ease.OutQuad);

        backButtons.DOAnchorPosX(-resumeButtonsPositions.y, animationDuration).SetEase(Ease.OutQuad);

        yield return new WaitForSecondsRealtime(animationDuration);

        if (cameraMovement.farPosition)
            yield break;
		
        logosParent.GetChild((int)GlobalVariables.Instance.environementChroma).gameObject.SetActive(false);

        ShowMenu(mainMenuScript);

        logoLoading = false;
    }

    public void ShowLogo()
    {
        if (currentMenu)
            currentMenu.HideMenu();

        if (logosParent.GetChild((int)GlobalVariables.Instance.environementChroma).gameObject.activeSelf)
            return;

        logosParent.GetChild((int)GlobalVariables.Instance.environementChroma).transform.localScale = Vector3.zero;
        logosParent.GetChild((int)GlobalVariables.Instance.environementChroma).gameObject.SetActive(true);
        logosParent.GetChild((int)GlobalVariables.Instance.environementChroma).transform.DOScale(Vector3.one, animationDuration).SetEase(Ease.OutQuad);

        backButtons.DOAnchorPosX(-resumeButtonsPositions.x, animationDuration).SetEase(Ease.OutQuad);
    }

    void OnApplicationFocus(bool value)
    {
        if (!value && GlobalVariables.Instance.GameState == GameStateEnum.Playing && !Application.isEditor)
            PauseResumeGame();	
    }

    #endregion

    #region Update

    void Update()
    {
        CheckPauseInput();

        if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
        {
            CheckNothingSelected();

            CheckMenuInput();

            MouseControl();
        }

        if (DOTween.IsTweening("Menu") || menuTweening)
            isTweening = true;
        else
            isTweening = false;
    }

    void CheckMenuInput()
    {
        if (isTweening || DOTween.IsTweening("MenuCamera") || startScreen)
            return;

        if (GlobalVariables.Instance.GameState == GameStateEnum.Playing || GlobalVariables.Instance.GameState == GameStateEnum.EndMode)
            return;

        for (int i = 0; i < GlobalVariables.Instance.rewiredPlayers.Length; i++)
        //for (int i = 0; i < 2; i++)
        {
            //Quit Far Position
            if (cameraMovement.farPosition && GlobalVariables.Instance.rewiredPlayers[i].GetAnyButtonDown())
            {
                if (cameraMovement.farPosition && GlobalVariables.Instance.GameState != GameStateEnum.Paused)
                {
                    cameraMovement.ToggleFarPosition();

                    if (OnFarPosition != null)
                        OnFarPosition();

                    break;
                }
            }

            if (GlobalVariables.Instance.rewiredPlayers[i].GetButtonDown("UI Cancel"))
            {
                if (Input.GetMouseButton(1) && currentMenu == instructionsMenu)
                    return;

                foreach (var b in backButtonsScript)
                    b.Back(i);

                //Cancel Menu
                if (currentMenu && currentMenu != mainMenuScript)
                {
                    currentMenu.Cancel();

                    break;
                }

				//Go To Far Position
				else if (!cameraMovement.farPosition && GlobalVariables.Instance.GameState != GameStateEnum.Paused && !logoLoading)
                {
                    cameraMovement.ToggleFarPosition();
					
                    if (OnFarPosition != null)
                        OnFarPosition();

                    break;
                }
            }
        }
    }

    void CheckPauseInput()
    {
        if (isTweening || DOTween.IsTweening("MenuCamera"))
            return;

        if (GlobalVariables.Instance.GameState != GameStateEnum.Paused && GlobalVariables.Instance.GameState != GameStateEnum.Playing)
            return;
		
        for (int i = 0; i < GlobalVariables.Instance.rewiredPlayers.Length; i++)
        //for (int i = 0; i < 2; i++)
        {
            if (i == 0 && currentMenu == mainMenu && GlobalVariables.Instance.GameState == GameStateEnum.Paused && GlobalVariables.Instance.rewiredPlayers[i].GetButtonDown("UI Cancel"))
            {
                currentMenu.HideMenu();
                PauseResumeGame();
            }

            if (GlobalVariables.Instance.GameState == GameStateEnum.Paused && GlobalVariables.Instance.rewiredPlayers[i].GetButtonDown("UI Start"))
            {
                foreach (var b in resumeButtonsScript)
                    b.Back(i);
				
                PauseResumeGame();
            }
			
            if (GlobalVariables.Instance.GameState == GameStateEnum.Playing && GlobalVariables.Instance.rewiredPlayers[i].GetButtonDown("UI Start"))
            {
                if (!passFightButton.gameObject.activeSelf)
                    PauseResumeGame();
                else
                    StartCoroutine(PassFight());
            }
        }			
    }

    public void ExitMenu()
    {
        if (!isTweening)
            currentMenu.Cancel();
    }

    void CheckNothingSelected()
    {
        if (eventSyst.currentSelectedGameObject == null && currentMenu != null && !isTweening && !isWaitingToSelect && !ReplayManager.Instance.isReplaying)
            StartCoroutine(SelectPreviousElement(currentMenu));
    }

    void GamepadDisconnected()
    {
        if (GlobalVariables.Instance.GameState == GameStateEnum.Playing)
            PauseResumeGame();

        /*if (GlobalVariables.Instance.GameState == GameStateEnum.EndMode)
			ReturnToMainMenu ();*/
    }

    void GamepadsChange()
    {
        if (GlobalVariables.Instance.GameState == GameStateEnum.Menu)
            return;

        List<int> gamepadsIndex = new List<int>(){ 1, 2, 3, 4 };

        foreach (GameObject p in GlobalVariables.Instance.EnabledPlayersList)
        {
            if (p == null)
                continue;

            PlayersGameplay script = p.GetComponent<PlayersGameplay>();

            if (script.rewiredPlayer == null)
                continue;

            //Gamepad
            if (gamepadsIndex.Contains(script.controllerNumber))
            {
                gamepadsIndex.Remove(script.controllerNumber);

                //Unplugged
                if (script.rewiredPlayer.controllers.joystickCount == 0)
                {
                    unpluggedPlayers[(int)script.controllerNumber - 1].gameObject.SetActive(true);
                    unpluggedPlayers[(int)script.controllerNumber - 1].DOAnchorPosY(disconnectedPlayersYPos.y, animationDuration).SetEase(easeMenu);
                }
                //Plugged
                else
                {
                    unpluggedPlayers[(int)script.controllerNumber - 1].DOAnchorPosY(disconnectedPlayersYPos.x, animationDuration).SetEase(easeMenu).OnComplete(() =>
                        {
                            unpluggedPlayers[(int)script.controllerNumber - 1].gameObject.SetActive(false);
                        });
                }
            }
        }

        foreach (var i in gamepadsIndex)
        {
            unpluggedPlayers[i - 1].DOAnchorPosY(disconnectedPlayersYPos.x, animationDuration).SetEase(easeMenu).OnComplete(() =>
                {
                    unpluggedPlayers[i - 1].gameObject.SetActive(false);
                });
        }
    }

    void MouseControl()
    {
        if (!mouseControl)
        {
            if (Mathf.Abs(Input.GetAxis("Mouse X")) > 0 || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0)
                mouseControl = true;
        }
        else
        {
            for (int i = 0; i < GlobalVariables.Instance.rewiredPlayers.Length; i++)
                if (GlobalVariables.Instance.rewiredPlayers[i] != null && GlobalVariables.Instance.rewiredPlayers[i].GetAxis("UI Vertical") != 0)
                    mouseControl = false;
        }
    }

    public void BackButton()
    {
        if (isTweening || DOTween.IsTweening("MenuCamera") || startScreen)
            return;

        if (GlobalVariables.Instance.GameState == GameStateEnum.Playing || GlobalVariables.Instance.GameState == GameStateEnum.EndMode)
            return;

        if (currentMenu == mainMenuScript && !cameraMovement.farPosition && GlobalVariables.Instance.GameState != GameStateEnum.Paused && !logoLoading)
        {
            cameraMovement.ToggleFarPosition();

            if (OnFarPosition != null)
                OnFarPosition();
        }
        else
            ExitMenu();
    }

    #endregion

    #region Submit Methods

    public enum MenuAnimationType
    {
        Show,
        Hide,
        Cancel,
        Submit,
        None}

    ;

    public void ShowMenu(MenuComponent whichMenu)
    {
        menuAnimationType = MenuAnimationType.Show;

        StartCoroutine(ShowMenuCoroutine(whichMenu));
    }

    public void SubmitMenu(MenuComponent whichMenu, int submitButton)
    {
        menuAnimationType = MenuAnimationType.Submit;

        StartCoroutine(SubmitMenuCoroutine(whichMenu, submitButton));
    }

    public void SubmitMenu(MenuComponent whichMenu)
    {
        menuAnimationType = MenuAnimationType.Submit;

        StartCoroutine(SubmitMenuCoroutine(whichMenu));
    }

    IEnumerator SubmitMenuCoroutine(MenuComponent whichMenu, int submitButton)
    {
        menuTweening = true;
        PlaySubmitSound();

        if (cameraMovement.farPosition)
            cameraMovement.ToggleFarPosition();

        yield return StartCoroutine(HideMenuCoroutine(whichMenu.aboveMenuScript, submitButton));

        yield return StartCoroutine(ShowMenuCoroutine(whichMenu));

        menuTweening = false;

        yield return new WaitWhile(() => DOTween.IsTweening("Menu"));

        menuAnimationType = MenuAnimationType.None;
    }

    //Hide Previous Menu and Show Target Menu
    IEnumerator SubmitMenuCoroutine(MenuComponent whichMenu)
    {
        menuTweening = true;
        PlaySubmitSound();

        yield return StartCoroutine(HideMenuCoroutine(whichMenu.aboveMenuScript));

        yield return StartCoroutine(ShowMenuCoroutine(whichMenu));

        menuTweening = false;
    }


    //Show Menu
    IEnumerator ShowMenuCoroutine(MenuComponent whichMenu, int cancelButton = -1)
    {
        currentMenu = whichMenu;

        whichMenu.OnShow();

        if (!whichMenu.gameObject.activeSelf)
            whichMenu.gameObject.SetActive(true);
		
        //Select First Under Menu Button
        StartCoroutine(SelectPreviousElement(whichMenu)); 

        StartCoroutine(ShowUnderMenus(whichMenu, cancelButton));
        StartCoroutine(ShowMainContent(whichMenu));
        StartCoroutine(ShowSecondaryContent(whichMenu));

        yield return new WaitForSecondsRealtime(animationDuration);
    }

    //Show Menus Contents
    IEnumerator ShowUnderMenus(MenuComponent whichMenu, int cancelButton = -1)
    {
        int delay = 0;

        //Show Under Menus Buttons
        for (int i = whichMenu.underMenusButtons.Count - 1; i >= 0; i--)
        {
            float duration = animationDuration - ButtonsDelay(delay);

            //If Canceling
            if (i != cancelButton || cancelButton == -1)
                whichMenu.underMenusButtons[i].anchoredPosition = new Vector2(offScreenButton.x, MenuButtonYPos(i) + GapAfterHeaderButton());

            Enable(whichMenu.underMenusButtons[i]);
            SetInteractable(whichMenu.underMenusButtons[i], duration + ButtonsDelay(delay));
			
//			whichMenu.underMenusButtons [i].DOAnchorPos (new Vector2 (menuOnScreenX, MenuButtonYPos (i) + GapAfterHeaderButton ()), duration).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
            whichMenu.underMenusButtons[i].DOAnchorPos(new Vector2(onScreenButton.x, MenuButtonYPos(i) + GapAfterHeaderButton()), duration).SetDelay(ButtonsDelay(delay)).SetEase(easeMenu).SetId("Menu");

            delay++;
        }

        //If Canceling
        if (menuAnimationType == MenuAnimationType.Cancel || menuAnimationType == MenuAnimationType.Show)
        {
            //Remove Current Header From List
            if (headerButtonsList.Count > 0)
                headerButtonsList.RemoveAt(headerButtonsList.Count - 1);
			
            ShowPreviousHeader(whichMenu, delay);
        }

        if (menuAnimationType == MenuAnimationType.Submit)
            PlaceCurrentHeader(whichMenu, delay);
		
        yield break;
    }

    void ShowPreviousHeader(MenuComponent whichMenu, int delay)
    {
        //Show Previous Header Button
        if (whichMenu.menuButton == null)
            return;
		
        Enable(whichMenu.menuButton);
        SetNonInteractable(whichMenu.menuButton);
		
        if (whichMenu.menuButton != null)
        {
//			whichMenu.menuButton.DOAnchorPos (new Vector2(menuOnScreenX, MenuHeaderButtonPosition ()), animationDuration).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
            whichMenu.menuButton.DOAnchorPos(new Vector2(onScreenButton.x, MenuHeaderButtonPosition()), animationDuration).SetDelay(ButtonsDelay(delay)).SetEase(easeMenu).SetId("Menu");
        }

        headerButtonsList.Add(whichMenu.menuButton);
    }

    IEnumerator ShowMainContent(MenuComponent whichMenu)
    {
        if (whichMenu.mainContent == null)
            yield break;

        whichMenu.mainContent.anchoredPosition = offScreenContent;
        Enable(whichMenu.mainContent);

        whichMenu.mainContent.DOAnchorPos(onScreenContent, animationDuration).SetEase(easeMenu).SetId("Menu");

        yield break;
    }

    IEnumerator ShowSecondaryContent(MenuComponent whichMenu)
    {
        float waitDelay = 0;

        //Secondary Content
        for (int i = 0; i < whichMenu.secondaryContents.Count; i++)
        {
            if (whichMenu.secondaryContents[i].content.anchoredPosition != whichMenu.secondaryContents[i].onScreenPos)
                whichMenu.secondaryContents[i].content.anchoredPosition = whichMenu.secondaryContents[i].offScreenPos;
			
            if (whichMenu.secondaryContents[i].delay > waitDelay)
                waitDelay = whichMenu.secondaryContents[i].delay;
			
            Enable(whichMenu.secondaryContents[i].content);

            if (whichMenu.secondaryContents[i].content.GetComponent<Button>() != null)
                whichMenu.secondaryContents[i].content.GetComponent<Button>().interactable = true;

            whichMenu.secondaryContents[i].content.DOAnchorPos(whichMenu.secondaryContents[i].onScreenPos, animationDuration).SetDelay(whichMenu.secondaryContents[i].delay).SetEase(easeMenu).SetId("Menu");
        }	

        yield break;
    }

    #endregion

    #region Cancel Methods

    public void HideMenu(MenuComponent whichMenu)
    {
        menuAnimationType = MenuAnimationType.Hide;

        StartCoroutine(HideMenuCoroutine(whichMenu, -1));
    }

    public void CancelMenu(MenuComponent whichMenu, int cancelButton)
    {
        menuAnimationType = MenuAnimationType.Cancel;

        StartCoroutine(CancelMenuCoroutine(whichMenu, cancelButton));
    }

    IEnumerator CancelMenuCoroutine(MenuComponent whichMenu, int cancelButton)
    {
        menuTweening = true;

        PlayReturnSound();

        yield return StartCoroutine(HideMenuCoroutine(whichMenu));

        yield return StartCoroutine(ShowMenuCoroutine(whichMenu.aboveMenuScript, cancelButton));

        menuTweening = false;

        yield return new WaitWhile(() => DOTween.IsTweening("Menu"));

        menuAnimationType = MenuAnimationType.None;
    }


    IEnumerator HideMenuCoroutine(MenuComponent whichMenu, int submitButton = -1)
    {
        if (!whichMenu.gameObject.activeSelf)
            whichMenu.gameObject.SetActive(true);
		
        StartCoroutine(HideUnderMenus(whichMenu, submitButton));
        StartCoroutine(HideMainContent(whichMenu));
        StartCoroutine(HideSecondaryContent(whichMenu));

        yield return new WaitForSecondsRealtime(animationDuration);

        whichMenu.OnHide();

        currentMenu = null;
    }


    IEnumerator HideUnderMenus(MenuComponent whichMenu, int submitButton = -1)
    {
        int delay = 0;

        //Under Menus Buttons
        for (int i = whichMenu.underMenusButtons.Count - 1; i >= 0; i--)
        {
            if (menuAnimationType != MenuAnimationType.Submit || i != submitButton)
            {
                Disable(whichMenu.underMenusButtons[i], animationDuration + ButtonsDelay(delay));
                SetNonInteractable(whichMenu.underMenusButtons[i]);

                whichMenu.underMenusButtons[i].DOAnchorPosX(offScreenButton.x, animationDuration).SetDelay(ButtonsDelay(delay)).SetEase(easeMenu).SetId("Menu");

                //whichMenu.underMenusButtons [i].DOAnchorPosX (menuOffScreenX, animationDuration).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
                delay++;
            }
        }

        if (menuAnimationType == MenuAnimationType.Submit || menuAnimationType == MenuAnimationType.Hide)
            HidePreviousHeader(delay);
        else if (!whichMenu.aboveMenuScript.underMenusButtons.Contains(whichMenu.menuButton))
            HidePreviousHeader(delay);

        //If Submiting
        if (menuAnimationType == MenuAnimationType.Submit && submitButton != -1)
            PlaceCurrentHeader(whichMenu, submitButton, delay);

        yield break;
    }

    void HidePreviousHeader(int delay)
    {
        //Hide Previous Header Button
        if (headerButtonsList.Count > 0)
        {
            Disable(headerButtonsList[headerButtonsList.Count - 1], animationDuration + ButtonsDelay(delay));
            SetNonInteractable(headerButtonsList[headerButtonsList.Count - 1]);
			
            headerButtonsList[headerButtonsList.Count - 1].DOAnchorPos(offScreenHeader, animationDuration).SetDelay(ButtonsDelay(delay)).SetEase(easeMenu).SetId("Menu");
			
            //headerButtonsList [headerButtonsList.Count - 1].DOAnchorPosX (menuOffScreenX, animationDuration).SetDelay (ButtonsDelay (delay)).SetEase (easeMenu).SetId ("Menu");
			
            headerButtonsList.RemoveAt(headerButtonsList.Count - 1);
        }
    }

    Tween PlaceCurrentHeader(MenuComponent whichMenu, int submitButton, int delay)
    {
        if (headerButtonsList.Contains(whichMenu.underMenusButtons[submitButton]))
            return null;
		
        if (whichMenu.underMenusButtons[submitButton].anchoredPosition.x != onScreenHeader.x)
            whichMenu.underMenusButtons[submitButton].anchoredPosition = offScreenHeader;

        headerButtonsList.Add(whichMenu.underMenusButtons[submitButton]);

        //Place Submit Button as Header
        Enable(whichMenu.underMenusButtons[submitButton]);
        SetNonInteractable(whichMenu.underMenusButtons[submitButton]);

        return whichMenu.underMenusButtons[submitButton].DOAnchorPos(onScreenHeader, animationDuration)
			.SetDelay(ButtonsDelay(delay))
			.SetEase(easeMenu)
			.SetId("Menu");
    }

    Tween PlaceCurrentHeader(MenuComponent whichMenu, int delay)
    {
        if (headerButtonsList.Contains(whichMenu.menuButton))
            return null;

        if (whichMenu.menuButton.anchoredPosition.x != onScreenHeader.x)
            whichMenu.menuButton.anchoredPosition = offScreenHeader;

        headerButtonsList.Add(whichMenu.menuButton);

        //Place Submit Button as Header
        Enable(whichMenu.menuButton);
        SetNonInteractable(whichMenu.menuButton);

        return whichMenu.menuButton.DOAnchorPos(onScreenHeader, animationDuration)
			.SetDelay(ButtonsDelay(delay))
			.SetEase(easeMenu)
			.SetId("Menu");
    }

    IEnumerator HideMainContent(MenuComponent whichMenu)
    {
        whichMenu.mainContent.DOAnchorPos(offScreenContent, animationDuration).SetEase(easeMenu).SetId("Menu").OnComplete(() => Disable(whichMenu.mainContent));

        yield break;
    }

    IEnumerator HideSecondaryContent(MenuComponent whichMenu)
    {
        float waitDelay = 0;

        //Secondary Content
        for (int i = 0; i < whichMenu.secondaryContents.Count; i++)
        {
            if (whichMenu.secondaryContents[i].delay > waitDelay)
                waitDelay = whichMenu.secondaryContents[i].delay;
			
            Disable(whichMenu.secondaryContents[i].content, animationDuration + whichMenu.secondaryContents[i].delay);

            if (whichMenu.secondaryContents[i].content.GetComponent<Button>() != null)
                whichMenu.secondaryContents[i].content.GetComponent<Button>().interactable = false;

            whichMenu.secondaryContents[i].content.DOAnchorPos(whichMenu.secondaryContents[i].offScreenPos, animationDuration).SetDelay(whichMenu.secondaryContents[i].delay).SetEase(easeMenu).SetId("Menu");
        }			

        yield break;
    }

    #endregion

    #region Other Methods

    public enum WhichOverrideSettings
    {
        HeaderPos = 1,
        MenuPos = 2,
        ButtonPos = 4,
        ContentPos = 8,
        All = 16

    }

    public float ButtonsDelay(int i)
    {
        return buttonsDelay * i;
    }

    public float MenuHeaderButtonPosition()
    {
        return menuHeaderY - gapBetweenButtons * headerButtonsList.Count;
    }

    public float GapAfterHeaderButton()
    {
        if (headerButtonsList.Count > 0)
            return (-gapBetweenButtons * headerButtonsList.Count) + gapBetweenButtons;
        else
            return 0;
    }

    public float MenuButtonYPos(int i)
    {
        return menuFirstButtonY - (gapBetweenButtons * i);
    }

    public float ButtonYPos(int i)
    {
        return buttonFirstButtonY - (gapBetweenButtons * i);
    }

    public void Enable(RectTransform target)
    {
        target.gameObject.SetActive(true);
    }

    public void Disable(RectTransform target, float delayDuration = 0)
    {
        if (delayDuration == 0)
            target.gameObject.SetActive(false);
        else
            StartCoroutine(DisableDelay(target, delayDuration));
    }

    IEnumerator DisableDelay(RectTransform target, float delayDuration)
    {
        yield return new WaitForSecondsRealtime(delayDuration);

        target.gameObject.SetActive(false);
    }


    IEnumerator SelectPreviousElement(MenuComponent whichMenu)
    {
        isWaitingToSelect = true;

        GameObject selectable = null;

        if (whichMenu.previousSelected != null && selectPreviousElement)
            selectable = whichMenu.previousSelected;
        else if (whichMenu.selectable != null)
            selectable = whichMenu.selectable;

        eventSyst.SetSelectedGameObject(null);

        yield return new WaitWhile(() => isTweening || DOTween.IsTweening("MenuCamera"));

        if (selectable != null)
        {
            Button button = selectable.GetComponent<Button>();

            if (selectable.GetComponent<MenuButtonAnimationsAndSounds>() != null)
                selectable.GetComponent<MenuButtonAnimationsAndSounds>().selectableSelection = true;

            if (button != null)
                yield return new WaitUntil(() => button.interactable == true);

            eventSyst.SetSelectedGameObject(selectable);
        }

        isWaitingToSelect = false;

        yield return 0;
    }

    void SetInteractable(RectTransform target, float delayDuration = 0)
    {
        StartCoroutine(SetInteractableCoroutine(target, delayDuration));
    }

    IEnumerator SetInteractableCoroutine(RectTransform target, float delayDuration)
    {
        yield return new WaitForSecondsRealtime(delayDuration);

        target.GetComponent<Button>().interactable = true;

        Navigation navTemp = target.GetComponent<Button>().navigation;
        navTemp.mode = Navigation.Mode.Explicit;

        target.GetComponent<Button>().navigation = navTemp;
    }

    void SetNonInteractable(RectTransform target)
    {
        target.GetComponent<Button>().interactable = false;

        //Weird Bug Patch
        target.GetComponent<Button>().enabled = false;
        target.GetComponent<Button>().enabled = true;

        Navigation navTemp = target.GetComponent<Button>().navigation;
        navTemp.mode = Navigation.Mode.None;

        target.GetComponent<Button>().navigation = navTemp;
    }


    void PlaySubmitSound()
    {
        MasterAudio.PlaySound(SoundsManager.Instance.menuSubmit);
    }

    void PlayReturnSound()
    {
        MasterAudio.PlaySound(SoundsManager.Instance.menuCancel);
    }

    public void QuitGame()
    {
        if (OnQuitGame != null)
            OnQuitGame();

        Application.Quit();
    }

    #endregion

    #region Play Resume

    public void PauseResumeGame()
    {
        StartCoroutine(PauseResumeGameCoroutine());
    }

    IEnumerator PauseResumeGameCoroutine()
    {
        if (GlobalVariables.Instance.GameState == GameStateEnum.Playing)
        {
            GlobalVariables.Instance.GameState = GameStateEnum.Paused;

            MasterAudio.PlaySound(SoundsManager.Instance.openMenuSound);

            mainCamera.GetComponent<SlowMotionCamera>().StartPauseSlowMotion();
            StartCoroutine(cameraMovement.NewMenuPosition());

            yield return new WaitForSecondsRealtime(cameraMovement.newMovementDuration - 0.5f);

            ShowMenu(mainMenuScript);
        }
        else
        {
            currentMenu.HideMenu();

            MasterAudio.PlaySound(SoundsManager.Instance.closeMenuSound);

            yield return StartCoroutine(cameraMovement.NewPlayPosition());

            mainCamera.GetComponent<SlowMotionCamera>().StopPauseSlowMotion();

            GlobalVariables.Instance.GameState = GameStateEnum.Playing;
        }
    }

    #endregion

    #region Mode Methods

    public void MenuLoadMode(WhichMode whichMode)
    {
        LoadModeManager.Instance.LoadSceneVoid(whichMode);
    }

    public void StartMode()
    {
        StartCoroutine(StartModeCoroutine());
    }

    void ModeLogo()
    {
        if (useStaticLogos)
            return;

        foreach (Transform t in modesLogosCanvas)
            t.gameObject.SetActive(false);
		
        if (GlobalVariables.Instance.CurrentModeLoaded == WhichMode.Default || GlobalVariables.Instance.CurrentModeLoaded == WhichMode.None || GlobalVariables.Instance.CurrentModeLoaded == WhichMode.Tutorial)
            return;
		
        foreach (Transform t in modesLogosCanvas)
            if (t.name == GlobalVariables.Instance.CurrentModeLoaded.ToString().ToUpper())
            {
                t.gameObject.SetActive(true);
                break;
            }
		
        modesLogosCanvas.gameObject.SetActive(true);
		
        float scale = modesLogoScale * 0.7f;
        modesLogosCanvas.localScale = Vector3.zero;
		
        DOVirtual.DelayedCall(modesLogoDelay, () =>
            {
                modesLogosCanvas.DOScale(modesLogoScale, modesLogoDuration).SetEase(modesLogoEase).OnComplete(() =>
                    {
                        modesLogosCanvas.DOScale(scale, modesLogoDuration2).SetEase(Ease.OutQuad).SetUpdate(false);
						
                        modesLogosCanvas.DOScale(0, modesLogoDuration).SetEase(Ease.OutQuad).OnComplete(() => 
							modesLogosCanvas.gameObject.SetActive(false)).SetDelay(modesLogoDuration2).SetUpdate(false);
                    }).SetUpdate(false);
				
                /*modesLogosCanvas.DOScale (modesLogoScale, modesLogoDuration).SetEase (modesLogoEase).OnComplete (()=> 
					modesLogosCanvas.DOScale (scale, modesLogoDuration2).SetEase (Ease.OutQuad).OnComplete (()=> 
						modesLogosCanvas.gameObject.SetActive (false) ));*/
            }).SetUpdate(false);
    }

    public void ShowStaticModesLogos()
    {
        foreach (Transform t in staticModesLogosParent)
            foreach (Transform c in t)
                c.gameObject.SetActive(false);

        foreach (Transform t in staticModesLogosParent.GetChild ((int)GlobalVariables.Instance.environementChroma))
            if (t.name == GlobalVariables.Instance.CurrentModeLoaded.ToString().ToUpper())
            {
                t.gameObject.SetActive(true);
                break;
            }
    }

    void HideStaticModesLogos()
    {
        if (!useStaticLogos)
        {
            foreach (Transform t in modesLogosCanvas)
                t.gameObject.SetActive(false);
        }
        else
        {
            foreach (Transform t in staticModesLogosParent)
                foreach (Transform c in t)
                    c.gameObject.SetActive(false);
        }
    }

    IEnumerator StartModeCoroutine()
    {
        if (OnStartModeClick != null)
            OnStartModeClick();

        mainCamera.GetComponent<SlowMotionCamera>().StopPauseSlowMotion();

        currentMenu.HideMenu();

        yield return new WaitForSecondsRealtime(animationDuration);

        MasterAudio.PlaySound(SoundsManager.Instance.closeMenuSound);

        yield return StartCoroutine(cameraMovement.NewPlayPosition());

        GlobalVariables.Instance.GameState = GameStateEnum.Playing;
    }

    public void ResetMode()
    {
        LoadModeManager.Instance.UnLoadSceneVoid();

        foreach (var r in unpluggedPlayers)
        {
            r.DOAnchorPosY(disconnectedPlayersYPos.x, animationDuration).SetEase(easeMenu).OnComplete(() =>
                {
                    r.gameObject.SetActive(false);
                });
        }
    }

    void OnPlayerDeath()
    {
        if (GlobalVariables.Instance.NumberOfPlayers > 0)
        {
            foreach (var p in GlobalVariables.Instance.AlivePlayersList)
            {
                PlayersGameplay s = p.GetComponent<PlayersGameplay>();

                if (s.GetType() != typeof(AIGameplay) && !s.GetType().IsSubclassOf(typeof(AIGameplay)))
                    return;
            }

            ShowPassFightButton();
        }
    }

    IEnumerator PassFight()
    {
        HidePassFightButton();

        foreach (var p in GlobalVariables.Instance.AlivePlayersList)
            p.GetComponent<PlayersGameplay>().livesCount = 1;

        while (GlobalVariables.Instance.GameState == GameStateEnum.Playing)
        {
            GameObject player = GlobalVariables.Instance.AlivePlayersList[UnityEngine.Random.Range(0, GlobalVariables.Instance.AlivePlayersList.Count)];
            player.GetComponent<PlayersGameplay>().Death(DeathFX.All, player.transform.position);

            yield return new WaitForSeconds(0.1f);
        }
    }

    void ShowPassFightButton()
    {
        passFightButton.gameObject.SetActive(true);
        passFightButton.localScale = Vector3.zero;

        passFightButton.DOScale(1, animationDuration).SetEase(easeMenu);
    }

    void HidePassFightButton()
    {
        passFightButton.DOScale(0, animationDuration).SetEase(easeMenu).OnComplete(() => passFightButton.gameObject.SetActive(false));
    }

    #endregion

    #region EndMode

    public void ShowEndMode()
    {
        if (GlobalVariables.Instance.GameState == GameStateEnum.EndMode)
            StartCoroutine(ShowEndModeCoroutine());
    }

    IEnumerator ShowEndModeCoroutine()
    {
        yield return StartCoroutine(cameraMovement.NewMenuPosition());

        ShowMenu(endModeMenu);
    }

    public void ReturnToMainMenu()
    {
        StartCoroutine(ReturnToMainMenuCoroutine());
    }

    IEnumerator ReturnToMainMenuCoroutine()
    {
        HideMenu(endModeMenu);

        yield return new WaitForSecondsRealtime(animationDuration);

        MasterAudio.PlaySound(SoundsManager.Instance.openMenuSound);

        GlobalVariables.Instance.GameState = GameStateEnum.Menu;

        ShowMenu(mainMenuScript);
    }

    public void RestartMode()
    {
        if (isTweening)
            return;

        StartCoroutine(RestartModeCoroutine());
    }

    IEnumerator RestartModeCoroutine()
    {
        HideMenu(endModeMenu);

        yield return new WaitForSecondsRealtime(animationDuration);

        MasterAudio.PlaySound(SoundsManager.Instance.closeMenuSound);

        LoadModeManager.Instance.RestartSceneVoid(false);
    }

    public void StartReplay()
    {
        GlobalVariables.Instance.slowMotionCamera.StopPauseSlowMotion();

        ReplayManager.Instance.SetupReplay();

        ReplayManager.Instance.StartReplay();

        eventSyst.SetSelectedGameObject(null);

        StartCoroutine(cameraMovement.NewPlayPosition());

        DOVirtual.DelayedCall(cameraMovement.newMovementDuration, () =>
            {
                ReplayManager.Instance.Play();
            });
    }

    public void StopReplay()
    {
        StartCoroutine(cameraMovement.NewMenuPosition());

        DOVirtual.DelayedCall(cameraMovement.newMovementDuration * 0.5f, () =>
            {
                ReplayManager.Instance.StopReplay();
                eventSyst.SetSelectedGameObject(currentMenu.selectable);
            });
    }

    #endregion

    #region Tutorial

    public void LoadTutorial()
    {
        LoadModeManager.Instance.LoadSceneVoid(WhichMode.Tutorial);
    }

    #endregion

    #region Events

    public event EventHandler OnStartModeClick;
    public event EventHandler OnMenuChange;

    public Action OnFarPosition;

    IEnumerator OnMenuChangeEvent(MenuComponent whichMenu)
    {
        if (whichMenu != null)
            yield return new WaitUntil(() => currentMenu != whichMenu);
        else
            yield return new WaitUntil(() => currentMenu != null);

        if (OnMenuChange != null)
            OnMenuChange();

        StartCoroutine(OnMenuChangeEvent(currentMenu));
    }

    #endregion
}
