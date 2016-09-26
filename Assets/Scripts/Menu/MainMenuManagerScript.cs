using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DarkTonic.MasterAudio;

public class MainMenuManagerScript : MonoBehaviour
{
	public bool tweening;

	[Header ("Positions")]
	public float offScreenX = -1400;
	public float onScreenX = -580;
	public float screenCenterY = -93;
	public float gapBetweenButtons = 131;
	public float topYpositionButton = 404;
	public float[] yPositions = new float[9];

	[Header ("Event System")]
	public EventSystem eventSyst;

	[Header ("Logos")]
	public RectTransform smallLogo;
	public GameObject logoMenu;
	public float shrinkDuration = 0.5f;
	public float cameraNewXPosition = -48;
	public RectTransform textToResume;

	[Header ("Head Buttons")]
	public RectTransform[] topMenuButtons;

	[Header ("Choose Mode Menu")]
	public GameObject[] modesDescription = new GameObject[4];
	public GameObject[] modesTeam = new GameObject[3];
	public RectTransform playButton;
	public float playButtonMinY = -700;
	public float playButtonMaxY = -457;
	public float playButtonDuration = 0.25f;
	public float modesDescriptionXPos = 527;
	public float modesDescriptionOnScreen = 198;
	public float modesDescriptionOffScreen = 700;

	[Header ("Gamepad Disconnection")]
	public bool oneGamepadDisconnected = false;
	public GameObject gamepadsDisconnectedCanvas;
	public float maxYGamepad;
	public float minYGamepad;
	public RectTransform[] gamepadsDisconnected = new RectTransform[4];

	[Header ("Animations Duration")]
	public float durationSubmit;
	public float durationCancel;
	public float durationContent;

	[Header ("Animations Delay")]
	public float[] delaySubmit;
	public float[] delayCancel;

	[Header ("Pause SlowMotion")]
	public float timeBeforePause;
	public float timeBeforeUnpause;

	[Header ("Camera Movements")]
	public Vector3 pausePosition = new Vector3 (-48, 93, 16);
	public Vector3 playPosition = new Vector3 (0, 30, 0);
	public Vector3 gameOverPosition = new Vector3 (48, 93, 16);
	public float cameraMovementDuration = 1.2f;
	public Ease cameraEaseMovement = Ease.InOutCubic;

	[Header ("Game Over Menu")]
	public GameObject gameOverCanvas;
	public GameObject restart;
	public RectTransform gameOverButton;
	public RectTransform restartButton;
	public RectTransform menuButton;
	public float bottomYPosition = -400;
	public RectTransform goRepulseContent;
	public RectTransform goBombContent;
	public RectTransform goHitContent;
	public RectTransform goCrushContent;
	public RectTransform goTrainingContent;

	[Header ("Back Buttons")]
	public GameObject backButtonsCanvas;
	public RectTransform backButtonsContent;
	public Vector2 backButtonsInitialPos;

	[Header ("Ease")]
	public Ease easeTypeMainMenu;

	[Header ("Menu Sounds")]
	[SoundGroupAttribute]
	public string gameStartSound;
	[SoundGroupAttribute]
	public string returnSound;
	[SoundGroupAttribute]
	public string openMenuSound;
	[SoundGroupAttribute]
	public string closeMenuSound;

	[Header ("Buttons To Select When Nothing Is")]
	public GameObject start;
	public GameObject sounds;
	public GameObject no;
	public GameObject high;
	public GameObject overall;
	public GameObject repulse;
	public GameObject play;
	public GameObject soundsBar;
	public GameObject bomb;

	[Header ("All Canvas")]
	public GameObject mainMenuCanvas;
	public GameObject instructionsMenuCanvas;
	public GameObject chooseOptionsMenuCanvas;
	public GameObject soundsMenuCanvas;
	public GameObject graphicsMenuCanvas;
	public GameObject playersMenuCanvas;
	public GameObject creditsMenuCanvas;
	public GameObject quitMenuCanvas;
	public GameObject chooseModeCanvas;
	public GameObject repulseMenuCanvas;
	public GameObject bombMenuCanvas;
	public GameObject crushMenuCanvas;
	public GameObject trainingMenuCanvas;
	public GameObject footballMenuCanvas;
	public GameObject hitMenuCanvas;
	public GameObject choosePlayerCanvas;

	[Header ("All Contents")]
	public RectTransform instructionsMenuContent;
	public RectTransform optionsMenuContent;
	public RectTransform soundsMenuContent;
	public RectTransform graphicsMenuContent;
	public RectTransform playersMenuContent;
	public RectTransform creditsMenuContent;
	public RectTransform quitMenuContent;
	public RectTransform chooseModeContent;
	public RectTransform choosePlayerContent;

	private RectTransform startRect;
	private RectTransform instructionsRect;
	private RectTransform optionsRect;
	private RectTransform creditsRect;
	private RectTransform quitRect;
	private RectTransform resumeRect;

	private RectTransform optionsButtonRect;
	private RectTransform playersButtonRect;
	private RectTransform soundsButtonRect;
	private RectTransform graphicsButtonRect;

	private RectTransform repulseButtonRect;
	private RectTransform bombButtonRect;
	private RectTransform crushButtonRect;
	private RectTransform trainingButtonRect;
	private RectTransform footballButtonRect;
	private RectTransform hitButtonRect;

	private bool startScreen = true;

	private LoadModeManager loadModeScript;

	private GameObject mainCamera;

	private ControllerChangeManager1 controllerManager;

	private bool backButtonsEnabled = false;

	private Vector3 cameraPosBeforePause;

	void Awake ()
	{
		DOTween.Init();
		DOTween.defaultTimeScaleIndependent = true;
	}

    // Use this for initialization
    void Start ()
    {
		SetGapsAndButtons ();

		pausePosition.x = cameraNewXPosition;
		gameOverPosition.x = -cameraNewXPosition;

		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");

		loadModeScript = GameObject.FindObjectOfType<LoadModeManager> ();

		for (int i = 0; i < topMenuButtons.Length; i++)
			topMenuButtons [i].anchoredPosition = new Vector2 (onScreenX, topMenuButtons [i].anchoredPosition.y);

		startRect = mainMenuCanvas.transform.GetChild(0).GetComponent<RectTransform>();
		instructionsRect = mainMenuCanvas.transform.GetChild(1).GetComponent<RectTransform>();
		optionsRect = mainMenuCanvas.transform.GetChild(2).GetComponent<RectTransform>();
		creditsRect = mainMenuCanvas.transform.GetChild(3).GetComponent<RectTransform>();
		quitRect = mainMenuCanvas.transform.GetChild(4).GetComponent<RectTransform>();
		resumeRect = mainMenuCanvas.transform.GetChild(5).GetComponent<RectTransform>();

		optionsButtonRect = chooseOptionsMenuCanvas.transform.GetChild(0).GetComponent<RectTransform>();
		playersButtonRect = chooseOptionsMenuCanvas.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>();
		soundsButtonRect = chooseOptionsMenuCanvas.transform.GetChild(1).GetChild(1).GetComponent<RectTransform>();
		graphicsButtonRect = chooseOptionsMenuCanvas.transform.GetChild(1).GetChild(2).GetComponent<RectTransform>();

		repulseButtonRect = chooseModeCanvas.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>();
		bombButtonRect = chooseModeCanvas.transform.GetChild(1).GetChild(1).GetComponent<RectTransform>();
		hitButtonRect = chooseModeCanvas.transform.GetChild(1).GetChild(2).GetComponent<RectTransform>();
		crushButtonRect = chooseModeCanvas.transform.GetChild(1).GetChild(3).GetComponent<RectTransform>();
		trainingButtonRect = chooseModeCanvas.transform.GetChild(1).GetChild(4).GetComponent<RectTransform>();


		instructionsMenuCanvas.SetActive(false);
		chooseOptionsMenuCanvas.SetActive(false);
		creditsMenuCanvas.SetActive(false);
		soundsMenuCanvas.SetActive(false);
		graphicsMenuCanvas.SetActive(false);
		playersMenuCanvas.SetActive(false);
		chooseModeCanvas.SetActive(false);
		crushMenuCanvas.SetActive(false);
		trainingMenuCanvas.SetActive(false);
		repulseMenuCanvas.SetActive(false);
		bombMenuCanvas.SetActive(false);
		footballMenuCanvas.SetActive(false);
		hitMenuCanvas.SetActive(false);
		gameOverCanvas.SetActive(false);
		choosePlayerCanvas.SetActive (false);
		quitMenuCanvas.SetActive (false);

		mainMenuCanvas.SetActive(true);
		start.GetComponent<Button>().Select();

		textToResume.gameObject.SetActive (true);
		backButtonsContent.gameObject.SetActive (true);
		gamepadsDisconnectedCanvas.SetActive (true);
		backButtonsCanvas.SetActive (true);

		startRect.anchoredPosition = new Vector2(offScreenX, yPositions [2]);
		instructionsRect.anchoredPosition = new Vector2(offScreenX, yPositions [3]);
		optionsRect.anchoredPosition =new Vector2(offScreenX, yPositions [4]);
		creditsRect.anchoredPosition = new Vector2(offScreenX, yPositions [5]);
		quitRect.anchoredPosition = new Vector2(offScreenX, yPositions [6]);
		resumeRect.anchoredPosition = new Vector2(offScreenX, yPositions [1] - 16);

		//LoadMainMenu();

		mainMenuCanvas.SetActive(false);
		smallLogo.transform.parent.gameObject.SetActive(false);

		logoMenu.gameObject.SetActive(true);
		logoMenu.transform.parent.GetChild(1).gameObject.SetActive(true);

		GameObject.FindGameObjectWithTag("MainCamera").transform.position = new Vector3 (-140, pausePosition.y, pausePosition.z);

		controllerManager = choosePlayerContent.GetComponent<ControllerChangeManager1> ();

		for(int i = 0; i < 4; i++)
		{
			gamepadsDisconnected [i].anchoredPosition = new Vector2 (gamepadsDisconnected [i].anchoredPosition.x, maxYGamepad);
		}
	}

	void SetGapsAndButtons ()
	{
		yPositions [0] = screenCenterY + (gapBetweenButtons * 4);
		yPositions [1] = screenCenterY + (gapBetweenButtons * 3);
		yPositions [2] = screenCenterY + (gapBetweenButtons * 2);
		yPositions [3] = screenCenterY + gapBetweenButtons;
		yPositions [4] = screenCenterY;
		yPositions [5] = screenCenterY - gapBetweenButtons;
		yPositions [6] = screenCenterY - (gapBetweenButtons * 2);
		yPositions [7] = screenCenterY - (gapBetweenButtons * 3);
		yPositions [8] = screenCenterY - (gapBetweenButtons * 4);

		for(int i = 0; i < topMenuButtons.Length; i++)
		{
			topMenuButtons [i].anchoredPosition = new Vector2 (onScreenX, topYpositionButton);
		}
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(startScreen == true)
		{
			if(Input.GetAxisRaw("Submit") > 0 || Input.GetKeyDown(KeyCode.Mouse0))
			{
				startScreen = false;

				StartCoroutine(StartScreen ());
				Tweening ();
			}
		}

		if(GlobalVariables.Instance.GameState != GameStateEnum.Playing && mainCamera.transform.position == pausePosition)
		{
			if (mainMenuCanvas.activeSelf == false && backButtonsEnabled && backButtonsContent.anchoredPosition != backButtonsInitialPos && !DOTween.IsTweening("BackButtons"))
				backButtonsContent.DOAnchorPos (backButtonsInitialPos, durationContent).SetEase (easeTypeMainMenu).SetId("BackButtons");
		}
		else if(backButtonsEnabled && backButtonsContent.anchoredPosition.x == backButtonsInitialPos.x && !DOTween.IsTweening("BackButtons"))
			backButtonsContent.DOAnchorPos (new Vector2(offScreenX, backButtonsContent.anchoredPosition.y), durationContent).SetEase (easeTypeMainMenu).SetId("BackButtons");


		if(Input.GetAxisRaw("Cancel") > 0 && !tweening)
        {

			if(instructionsMenuCanvas.activeSelf == true)
			{
				ExitInstructions ();
				Tweening ();
			}

			if(chooseOptionsMenuCanvas.activeSelf == true && playersMenuCanvas.activeSelf == false && soundsMenuCanvas.activeSelf == false && graphicsMenuCanvas.activeSelf == false)
			{
				ExitOptions ();
				Tweening ();
			}

			if(creditsMenuCanvas.activeSelf == true)
			{
				ExitCredits ();
				Tweening ();
			}

			if(quitMenuCanvas.activeSelf == true)
			{
				ExitQuit ();
				Tweening ();
			}

			if(playersMenuCanvas.activeSelf == true)
			{
				StartCoroutine("ExitPlayers");
				Tweening ();
			}

			if(soundsMenuCanvas.activeSelf == true)
			{
				StartCoroutine("ExitSounds");
				Tweening ();
			}

			if(graphicsMenuCanvas.activeSelf == true)
			{
				StartCoroutine("ExitGraphics");
				Tweening ();
			}

			if(chooseModeCanvas.activeSelf == true)
			{
				ExitChooseMode ();
				Tweening ();
			}

			if(crushMenuCanvas.activeSelf == true)
			{
				StartCoroutine (ExitCrush ());
				Tweening ();
			}

			if(hitMenuCanvas.activeSelf == true)
			{
				StartCoroutine (ExitHit ());
				Tweening ();
			}

			if(repulseMenuCanvas.activeSelf == true)
			{
				StartCoroutine (ExitRepulse ());
				Tweening ();
			}

			if(bombMenuCanvas.activeSelf == true)
			{
				StartCoroutine (ExitBomb ());
				Tweening ();
			}

			if(trainingMenuCanvas.activeSelf == true)
			{
				StartCoroutine (ExitTraining ());
				Tweening ();
			}
        }

		if(eventSyst.currentSelectedGameObject == null && mainMenuCanvas.activeSelf == true)
		{
			start.GetComponent<Button>().Select();
		}

		if(eventSyst.currentSelectedGameObject == null && chooseOptionsMenuCanvas.activeSelf == true)
		{
			sounds.GetComponent<Button>().Select();
		}

		if(eventSyst.currentSelectedGameObject == null && quitMenuCanvas.activeSelf == true)
		{
			no.GetComponent<Button>().Select();
		}

		if(eventSyst.currentSelectedGameObject == null && chooseModeCanvas.activeSelf == true)
		{
			repulse.GetComponent<Button>().Select();
		}

		if(eventSyst.currentSelectedGameObject == null && gameOverCanvas.activeSelf == true)
		{
			restart.GetComponent<Button>().Select();
		}

		if(eventSyst.currentSelectedGameObject == null && soundsMenuCanvas.activeSelf == true)
		{
			soundsBar.GetComponent<Scrollbar>().Select();
		}

		if(eventSyst.currentSelectedGameObject == null && chooseModeCanvas.activeSelf == true)
		{
			bomb.GetComponent<Button>().Select();
		}

		if(crushMenuCanvas.activeSelf == true || footballMenuCanvas.activeSelf == true || hitMenuCanvas.activeSelf == true || bombMenuCanvas.activeSelf == true || repulseMenuCanvas.activeSelf == true || trainingMenuCanvas.activeSelf == true)
		{
			if(eventSyst.currentSelectedGameObject == null)
				play.GetComponent<Button>().Select();

			CheckCanPlay ();
		}

		//SetButtonsNavigation ();

		TextResume ();

		GamepadsDisconnection ();
	}

	public void ClickExitMenu ()
	{
		if(instructionsMenuCanvas.activeSelf == true)
		{
			ExitInstructions ();
			Tweening ();
		}

		if(chooseOptionsMenuCanvas.activeSelf == true && playersMenuCanvas.activeSelf == false && soundsMenuCanvas.activeSelf == false && graphicsMenuCanvas.activeSelf == false)
		{
			ExitOptions ();
			Tweening ();
		}

		if(creditsMenuCanvas.activeSelf == true)
		{
			ExitCredits ();
			Tweening ();
		}

		if(quitMenuCanvas.activeSelf == true)
		{
			ExitQuit ();
			Tweening ();
		}

		if(playersMenuCanvas.activeSelf == true)
		{
			StartCoroutine("ExitPlayers");
			Tweening ();
		}

		if(soundsMenuCanvas.activeSelf == true)
		{
			StartCoroutine("ExitSounds");
			Tweening ();
		}

		if(graphicsMenuCanvas.activeSelf == true)
		{
			StartCoroutine("ExitGraphics");
			Tweening ();
		}

		if(chooseModeCanvas.activeSelf == true)
		{
			ExitChooseMode ();
			Tweening ();
		}

		if(crushMenuCanvas.activeSelf == true)
		{
			StartCoroutine (ExitCrush ());
			Tweening ();
		}

		if(hitMenuCanvas.activeSelf == true)
		{
			StartCoroutine (ExitHit ());
			Tweening ();
		}

		if(repulseMenuCanvas.activeSelf == true)
		{
			StartCoroutine (ExitRepulse ());
			Tweening ();
		}

		if(bombMenuCanvas.activeSelf == true)
		{
			StartCoroutine (ExitBomb ());
			Tweening ();
		}

		if(trainingMenuCanvas.activeSelf == true)
		{
			StartCoroutine (ExitTraining ());
			Tweening ();
		}
	}

	void TextResume ()
	{
		if (GlobalVariables.Instance.GameState == GameStateEnum.Paused && mainMenuCanvas.activeSelf == true && !tweening && !oneGamepadDisconnected)
		{
			if(textToResume.anchoredPosition.y != -517 && !DOTween.IsTweening("TextToResume"))
				textToResume.DOAnchorPos (new Vector2 (textToResume.anchoredPosition.x, -517), durationContent).SetEase (easeTypeMainMenu).SetId("TextToResume");
		}

		else 
		{
			if(textToResume.anchoredPosition.y != -700 && !DOTween.IsTweening("TextToResume"))
				textToResume.DOAnchorPos (new Vector2 (textToResume.anchoredPosition.x, -700), durationContent).SetEase (easeTypeMainMenu).SetId("TextToResume");
		}
	}

	void GamepadsDisconnection ()
	{
		for(int i = 0; i < 4; i++)
		{
			if (GamepadsManager.Instance.gamepadsUnplugged [i] == true)
				oneGamepadDisconnected = true;
		}

		if(GamepadsManager.Instance.gamepadsUnplugged [0] == false && GamepadsManager.Instance.gamepadsUnplugged [1] == false && GamepadsManager.Instance.gamepadsUnplugged [2] == false && GamepadsManager.Instance.gamepadsUnplugged [3] == false)
			oneGamepadDisconnected = false;
		

		if (GlobalVariables.Instance.GameState == GameStateEnum.Paused && mainMenuCanvas.activeSelf == true && !tweening)
		{
			for(int i = 0; i < 4; i++)
			{
				if(GamepadsManager.Instance.gamepadsUnplugged[i] == true && gamepadsDisconnected[i].anchoredPosition.y != minYGamepad && !DOTween.IsTweening("GamepadDisconnected"))
				{
					gamepadsDisconnected[i].DOAnchorPos (new Vector2 (gamepadsDisconnected[i].anchoredPosition.x, minYGamepad), durationContent).SetEase (easeTypeMainMenu).SetId("GamepadDisconnected");
				}

				if(GamepadsManager.Instance.gamepadsUnplugged[i] == false && gamepadsDisconnected[i].anchoredPosition.y != maxYGamepad && !DOTween.IsTweening("GamepadDisconnected"))
				{
					gamepadsDisconnected[i].DOAnchorPos (new Vector2 (gamepadsDisconnected[i].anchoredPosition.x, maxYGamepad), durationContent).SetEase (easeTypeMainMenu).SetId("GamepadDisconnected");

				}
			}
		}

		else
		{
			for(int i = 0; i < 4; i++)
			{
				if(GamepadsManager.Instance.gamepadsUnplugged[i] == false && gamepadsDisconnected[i].anchoredPosition.y != maxYGamepad && !DOTween.IsTweening("GamepadDisconnected"))
					gamepadsDisconnected[i].DOAnchorPos (new Vector2 (gamepadsDisconnected[i].anchoredPosition.x, maxYGamepad), durationContent).SetEase (easeTypeMainMenu).SetId("GamepadDisconnected");
			}
		}
	}

	void SetButtonsNavigation ()
	{
		if(GlobalVariables.Instance.GameState != GameStateEnum.Playing && resumeRect.gameObject.activeSelf == true)
		{
			resumeRect.gameObject.SetActive (false);

			var startNavigation = startRect.GetChild (1).GetComponent<Selectable> ().navigation;
			startNavigation.selectOnUp = quitRect.GetChild (1).GetComponent<Selectable>();

			startRect.GetChild (1).GetComponent<Selectable> ().navigation = startNavigation;

			var quitNavigation = quitRect.GetChild (1).GetComponent<Selectable> ().navigation;
			quitNavigation.selectOnDown = startRect.GetChild (1).GetComponent<Selectable>();

			quitRect.GetChild (1).GetComponent<Selectable> ().navigation = quitNavigation;
		}

		else if(GlobalVariables.Instance.GameState == GameStateEnum.Playing && resumeRect.gameObject.activeSelf == false)
		{
			resumeRect.gameObject.SetActive (true);

			var startNavigation = startRect.GetChild (1).GetComponent<Selectable> ().navigation;
			startNavigation.selectOnUp = resumeRect.GetChild (1).GetComponent<Selectable>();

			startRect.GetChild (1).GetComponent<Selectable> ().navigation = startNavigation;

			var quitNavigation = quitRect.GetChild (1).GetComponent<Selectable> ().navigation;
			quitNavigation.selectOnDown = resumeRect.GetChild (1).GetComponent<Selectable>();

			quitRect.GetChild (1).GetComponent<Selectable> ().navigation = quitNavigation;
		}
	}

	IEnumerator StartScreen ()
	{
		MasterAudio.PlaySound (gameStartSound);

		logoMenu.transform.parent.GetChild(1).gameObject.SetActive(false);

		logoMenu.transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 365),  shrinkDuration);
		logoMenu.transform.GetChild(1).GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 365),  shrinkDuration);
		logoMenu.transform.GetChild(2).GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 365),  shrinkDuration);

		logoMenu.transform.GetChild(0).GetComponent<RectTransform>().DOScale(0.4f, shrinkDuration);
		logoMenu.transform.GetChild(1).GetComponent<RectTransform>().DOScale(0.4f, shrinkDuration);

		Tween myTween = logoMenu.transform.GetChild(2).GetComponent<RectTransform>().DOScale(0.4f, shrinkDuration);

		yield return myTween.WaitForCompletion();

		logoMenu.gameObject.SetActive(false);

		smallLogo.transform.parent.gameObject.SetActive(true);

		startRect.anchoredPosition = new Vector2(offScreenX, yPositions [2]);
		instructionsRect.anchoredPosition = new Vector2(offScreenX, yPositions [3]);
		optionsRect.anchoredPosition = new Vector2(offScreenX, yPositions [4]);
		creditsRect.anchoredPosition = new Vector2(offScreenX, yPositions [5]);
		quitRect.anchoredPosition = new Vector2(offScreenX, yPositions [6]);
		resumeRect.anchoredPosition = new Vector2(offScreenX, yPositions [1] - 16);

		GameObject.FindGameObjectWithTag("MainCamera").transform.DOMoveX(cameraNewXPosition, 1).SetEase(Ease.InOutCubic);

		backButtonsEnabled = true;

		LoadMainMenu ();

		yield return null;
	}

	public void GamePauseResumeVoid ()
	{
		if(!tweening)
			StartCoroutine(GamePauseResume ());
	}

	IEnumerator GamePauseResume ()
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			cameraPosBeforePause = mainCamera.transform.position;

			GlobalVariables.Instance.GameState = GameStateEnum.Paused;
			mainMenuCanvas.SetActive(true);

			Tweening ();

			mainCamera.GetComponent<SlowMotionCamera> ().StartPauseSlowMotion ();
			//Wait Slowmotion
			yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(timeBeforePause));

			MasterAudio.PlaySound (openMenuSound);

			//GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DOTweenPath>().DOPlayBackwards();
			mainCamera.transform.DOMove (pausePosition, cameraMovementDuration).SetEase(cameraEaseMovement);

			yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(cameraMovementDuration - 0.5f));

			LoadMainMenu ();
		}

		else if(GlobalVariables.Instance.GameState == GameStateEnum.Paused && mainMenuCanvas.activeSelf == true && !oneGamepadDisconnected)
		{
			Tweening ();

			resumeRect.DOAnchorPos(new Vector2(offScreenX, yPositions [1] - 16), durationCancel).SetDelay(delayCancel[5]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

			startRect.DOAnchorPos(new Vector2(offScreenX, yPositions [2]), durationSubmit).SetDelay(delaySubmit[4]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			instructionsRect.DOAnchorPos(new Vector2(offScreenX, yPositions [3]), durationCancel).SetDelay(delayCancel[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			optionsRect.DOAnchorPos(new Vector2(offScreenX, yPositions [4]), durationCancel).SetDelay(delayCancel[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			creditsRect.DOAnchorPos(new Vector2(offScreenX, yPositions [5]), durationCancel).SetDelay(delayCancel[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			quitRect.DOAnchorPos(new Vector2(offScreenX, yPositions [6]), durationCancel).SetDelay(delayCancel[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");	

			smallLogo.DOAnchorPos(new Vector2(0, 400), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

			yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(durationCancel));

			MasterAudio.PlaySound (closeMenuSound);

			//GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DOTweenAnimation>().();
			mainCamera.transform.DOMove (cameraPosBeforePause, cameraMovementDuration).SetEase(cameraEaseMovement);

			yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(cameraMovementDuration));

			mainCamera.GetComponent<SlowMotionCamera> ().StopPauseSlowMotion ();
			//Wait Slowmotion
			yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(timeBeforeUnpause));

			mainMenuCanvas.SetActive(false);

			NotTweening ();

			GlobalVariables.Instance.GameState = GameStateEnum.Playing;
		}

	}



	void CheckCanPlay ()
	{
		/*if(CorrectTeams () && play.GetComponent<Button>().interactable == false)
		{
			play.GetComponent<Button> ().interactable = true;
			playButton.DOAnchorPos (new Vector2(playButton.anchoredPosition.x, playButtonMaxY), playButtonDuration).SetEase(easeTypeMainMenu);
		}

		else if(!CorrectTeams () && play.GetComponent<Button>().interactable == true)
		{
			play.GetComponent<Button> ().interactable = false;
			playButton.DOAnchorPos (new Vector2(playButton.anchoredPosition.x, playButtonMinY), playButtonDuration).SetEase(easeTypeMainMenu);
		}*/

		if(CorrectPlayerChoice () && play.GetComponent<Button>().interactable == false)
		{
			play.GetComponent<Button> ().interactable = true;
			playButton.DOAnchorPos (new Vector2(playButton.anchoredPosition.x, playButtonMaxY), playButtonDuration).SetEase(easeTypeMainMenu);
		}

		else if(!CorrectPlayerChoice () && play.GetComponent<Button>().interactable == true)
		{
			play.GetComponent<Button> ().interactable = false;
			playButton.DOAnchorPos (new Vector2(playButton.anchoredPosition.x, playButtonMinY), playButtonDuration).SetEase(easeTypeMainMenu);
		}
	}

	bool CorrectPlayerChoice ()
	{
		int player1Choice = 0;
		int player2Choice = 0;
		int player3Choice = 0;
		int player4Choice = 0;

		for(int i = 0; i < controllerManager.imagesNumber.Length; i++)
		{
			switch(controllerManager.imagesNumber[i])
			{
			case 1:
				player1Choice++;
				break;
			case 2:
				player2Choice++;
				break;
			case 3:
				player3Choice++;
				break;
			case 4:
				player4Choice++;
				break;
			}
		}

		if (player1Choice > 1 || player2Choice > 1 || player3Choice > 1 || player4Choice > 1)
			return false;
		
		else if (GlobalVariables.Instance.NumberOfPlayers < 2)
			return false;
		
		else
			return true;

	}

	public void StartModeVoid ()
	{
		if(!tweening)
		{
			StartCoroutine (StartMode ());
			Tweening ();
		}

	}

	IEnumerator StartMode ()
	{
		mainCamera.GetComponent<SlowMotionCamera> ().StopPauseSlowMotion ();

		Tween myTween;

		switch (GlobalVariables.Instance.CurrentModeLoaded)
		{
		case "Crush":
			playButton.DOAnchorPos (new Vector2 (playButton.anchoredPosition.x, playButtonMinY), playButtonDuration).SetEase (easeTypeMainMenu);
			crushButtonRect.DOAnchorPos (new Vector2 (offScreenX, yPositions [5]), durationSubmit).SetDelay (delaySubmit [0]).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			crushMenuCanvas.transform.GetChild (0).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (offScreenX, topYpositionButton), durationSubmit).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			crushMenuCanvas.transform.GetChild (1).GetChild(0).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (offScreenX, topYpositionButton - 131), durationSubmit).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			crushMenuCanvas.transform.GetChild (1).GetChild (1).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (modesDescriptionXPos, modesDescriptionOffScreen), durationContent).SetEase(easeTypeMainMenu);
			myTween = choosePlayerContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);
			break;
		case "Hit":
			playButton.DOAnchorPos (new Vector2 (playButton.anchoredPosition.x, playButtonMinY), playButtonDuration).SetEase (easeTypeMainMenu);
			hitButtonRect.DOAnchorPos (new Vector2 (offScreenX, yPositions [4]), durationSubmit).SetDelay (delaySubmit [0]).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			hitMenuCanvas.transform.GetChild (0).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (offScreenX, topYpositionButton), durationSubmit).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			hitMenuCanvas.transform.GetChild (1).GetChild(0).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (offScreenX, topYpositionButton - 131), durationSubmit).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			hitMenuCanvas.transform.GetChild (1).GetChild (1).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (modesDescriptionXPos, modesDescriptionOffScreen), durationContent).SetEase(easeTypeMainMenu);
			myTween = choosePlayerContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);
			break;
		case "Repulse":
			playButton.DOAnchorPos (new Vector2 (playButton.anchoredPosition.x, playButtonMinY), playButtonDuration).SetEase (easeTypeMainMenu);
			repulseButtonRect.DOAnchorPos (new Vector2 (offScreenX, yPositions [2]), durationSubmit).SetDelay (delaySubmit [0]).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			repulseMenuCanvas.transform.GetChild (0).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (offScreenX, topYpositionButton), durationSubmit).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			repulseMenuCanvas.transform.GetChild (1).GetChild(0).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (offScreenX, topYpositionButton - 131), durationSubmit).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			repulseMenuCanvas.transform.GetChild (1).GetChild (1).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (modesDescriptionXPos, modesDescriptionOffScreen), durationContent).SetEase(easeTypeMainMenu);
			myTween = choosePlayerContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);
			break;
		case "Bomb":
			playButton.DOAnchorPos (new Vector2 (playButton.anchoredPosition.x, playButtonMinY), playButtonDuration).SetEase (easeTypeMainMenu);
			bombButtonRect.DOAnchorPos (new Vector2 (offScreenX, yPositions [3]), durationSubmit).SetDelay (delaySubmit [0]).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			bombMenuCanvas.transform.GetChild (0).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (offScreenX, topYpositionButton), durationSubmit).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			bombMenuCanvas.transform.GetChild (1).GetChild(0).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (offScreenX, topYpositionButton - 131), durationSubmit).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			bombMenuCanvas.transform.GetChild (1).GetChild (1).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (modesDescriptionXPos, modesDescriptionOffScreen), durationContent).SetEase(easeTypeMainMenu);
			myTween = choosePlayerContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);
			break;
		case "Training":
			playButton.DOAnchorPos (new Vector2 (playButton.anchoredPosition.x, playButtonMinY), playButtonDuration).SetEase (easeTypeMainMenu);
			trainingButtonRect.DOAnchorPos (new Vector2 (offScreenX, yPositions [4]), durationSubmit).SetDelay (delaySubmit [0]).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			trainingMenuCanvas.transform.GetChild (0).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (offScreenX, topYpositionButton), durationSubmit).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			trainingMenuCanvas.transform.GetChild (1).GetChild(0).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (offScreenX, topYpositionButton - 131), durationSubmit).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			trainingMenuCanvas.transform.GetChild (1).GetChild (1).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (modesDescriptionXPos, modesDescriptionOffScreen), durationContent).SetEase(easeTypeMainMenu);
			myTween = choosePlayerContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);
			break;
		}
			
		smallLogo.DOAnchorPos(new Vector2(0, 400), durationSubmit).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		yield return myTween.WaitForCompletion ();

		backButtonsContent.DOAnchorPos (new Vector2(offScreenX, backButtonsInitialPos.y), durationContent).SetEase (easeTypeMainMenu).SetId("BackButtons");

		crushMenuCanvas.SetActive(false);
		footballMenuCanvas.SetActive(false);
		hitMenuCanvas.SetActive(false);
		repulseMenuCanvas.SetActive(false);
		bombMenuCanvas.SetActive(false);
		trainingMenuCanvas.SetActive(false);
		choosePlayerCanvas.SetActive (false);

		startRect.anchoredPosition = new Vector2 (offScreenX, yPositions[2]);

		MasterAudio.PlaySound (closeMenuSound);

		//GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DOTweenPath>().DOPlayForward();
		mainCamera.transform.DOMove (playPosition, cameraMovementDuration).SetEase(cameraEaseMovement);

		yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(cameraMovementDuration));

		NotTweening ();

		GlobalVariables.Instance.GameState = GameStateEnum.Playing;
	}




	public void LoadInstructionsVoid ()
	{
		if(!tweening)
		{
			StartCoroutine("LoadInstructions");
			Tweening ();
		}
	}

	IEnumerator LoadInstructions ()
	{
		//resumeRect.DOAnchorPos(new Vector2(onScreenX, 700), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnStart(Tweening);
		resumeRect.DOAnchorPos(new Vector2(offScreenX, yPositions [1] - 16), durationSubmit).SetDelay(delaySubmit[4] - 0.05f).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		startRect.DOAnchorPos(new Vector2(offScreenX, yPositions [2]), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		optionsRect.DOAnchorPos(new Vector2(offScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		creditsRect.DOAnchorPos(new Vector2(offScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		quitRect.DOAnchorPos(new Vector2(offScreenX, yPositions [6]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		Tween myTween = instructionsRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[5]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		yield return myTween.WaitForCompletion();

		instructionsMenuContent.anchoredPosition = new Vector2(offScreenX, 0);
		
		mainMenuCanvas.SetActive(false);
		instructionsMenuCanvas.SetActive(true);
		
		instructionsMenuContent.DOAnchorPos(new Vector2(0, 0), durationContent).SetEase(easeTypeMainMenu).OnComplete(NotTweening);
	}

	public void ExitInstructions ()
	{
		instructionsMenuContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu).OnComplete(LoadMainMenu);
		PlayReturnSound ();
	}

	public void LoadOptionsVoid()
	{
		if(!tweening)
		{
			StartCoroutine("LoadOptions");
			Tweening ();
		}
	}

	IEnumerator LoadOptions ()
	{
		//resumeRect.DOAnchorPos(new Vector2(onScreenX, 700), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnStart(Tweening);
		resumeRect.DOAnchorPos(new Vector2(offScreenX, yPositions [1] - 16), durationSubmit).SetDelay(delaySubmit[4] - 0.05f).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		startRect.DOAnchorPos(new Vector2(offScreenX, yPositions [2]), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnStart(Tweening);
		instructionsRect.DOAnchorPos(new Vector2(offScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		creditsRect.DOAnchorPos(new Vector2(offScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		quitRect.DOAnchorPos(new Vector2(offScreenX, yPositions [6]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		
		Tween myTween = optionsRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[5]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		
		yield return myTween.WaitForCompletion();

		optionsMenuContent.anchoredPosition = new Vector2(offScreenX, 0);

		//playersButtonRect.anchoredPosition = new Vector2(offScreenX, yPositions [3]);
		soundsButtonRect.anchoredPosition = new Vector2(offScreenX, yPositions [3]);
		graphicsButtonRect.anchoredPosition = new Vector2(offScreenX, yPositions [4]);

		mainMenuCanvas.SetActive(false);
		chooseOptionsMenuCanvas.SetActive(true);
		
		//playersButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(NotTweening);; 
		soundsButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		graphicsButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		optionsMenuContent.DOAnchorPos(new Vector2(0, 0), durationContent).SetEase(easeTypeMainMenu).OnComplete(NotTweening);

		sounds.GetComponent<Button>().Select();
	}

	public void ExitOptions ()
	{
		//playersButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(LoadMainMenu); 
		soundsButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(LoadMainMenu); 
		graphicsButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		optionsMenuContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);

		PlayReturnSound ();
	}

	public void LoadChooseModeVoid()
	{
		if(!tweening)
		{
			StartCoroutine(LoadChooseMode ());
			Tweening ();
		}
	}

	IEnumerator LoadChooseMode ()
	{
		//resumeRect.DOAnchorPos(new Vector2(onScreenX, 700), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnStart(Tweening);
		resumeRect.DOAnchorPos(new Vector2(offScreenX, yPositions [1] - 16), durationSubmit).SetDelay(delaySubmit[4] - 0.05f).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		instructionsRect.DOAnchorPos(new Vector2(offScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		optionsRect.DOAnchorPos(new Vector2(offScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		creditsRect.DOAnchorPos(new Vector2(offScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		quitRect.DOAnchorPos(new Vector2(offScreenX, yPositions [6]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		Tween myTween = startRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[4]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");


		yield return myTween.WaitForCompletion();

		chooseModeContent.anchoredPosition = new Vector2(offScreenX, 0);

		bombButtonRect.anchoredPosition = new Vector2(offScreenX, yPositions [2]);
		crushButtonRect.anchoredPosition = new Vector2(offScreenX, yPositions [3]);
		trainingButtonRect.anchoredPosition = new Vector2(offScreenX, yPositions [4]);


		mainMenuCanvas.SetActive(false);
		chooseModeCanvas.SetActive(true);

		bombButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [2]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(NotTweening);  
		crushButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");  
		trainingButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");  

		chooseModeContent.DOAnchorPos(new Vector2(0, 0), durationContent).SetEase(easeTypeMainMenu).OnComplete(NotTweening);

		bombButtonRect.transform.GetChild(1).GetComponent<Button>().Select();
	}

	public void ExitChooseMode ()
	{
		bombButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [2]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(LoadMainMenu); 
		crushButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		trainingButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 

		PlayReturnSound ();
	}

	public void LoadCreditsVoid()
	{
		if(!tweening)
		{
			StartCoroutine("LoadCredits");
			Tweening ();
		}
	}
	
	IEnumerator LoadCredits ()
	{
		//resumeRect.DOAnchorPos(new Vector2(onScreenX, 700), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnStart(Tweening);
		resumeRect.DOAnchorPos(new Vector2(offScreenX, yPositions [1] - 16), durationSubmit).SetDelay(delaySubmit[4] - 0.05f).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		startRect.DOAnchorPos(new Vector2(offScreenX, yPositions [2]), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnStart(Tweening);
		instructionsRect.DOAnchorPos(new Vector2(offScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		optionsRect.DOAnchorPos(new Vector2(offScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		quitRect.DOAnchorPos(new Vector2(offScreenX, yPositions [6]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		
		Tween myTween = creditsRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[5]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		
		yield return myTween.WaitForCompletion();
		
		creditsMenuContent.anchoredPosition = new Vector2(offScreenX, 0);
		
		mainMenuCanvas.SetActive(false);
		creditsMenuCanvas.SetActive(true);
		
		creditsMenuContent.DOAnchorPos(new Vector2(0, 0), durationContent).SetEase(easeTypeMainMenu).OnComplete(NotTweening);;
	}
	
	public void ExitCredits ()
	{
		creditsMenuContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu).OnComplete(LoadMainMenu);
		PlayReturnSound ();
	}
		
	public void LoadQuitVoid()
	{
		if(!tweening)
		{
			StartCoroutine("LoadQuit");
			Tweening ();
		}
	}
	
	IEnumerator LoadQuit ()
	{
		//resumeRect.DOAnchorPos(new Vector2(onScreenX, 700), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnStart(Tweening);
		resumeRect.DOAnchorPos(new Vector2(offScreenX, yPositions [1] - 16), durationSubmit).SetDelay(delaySubmit[4] - 0.05f).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		startRect.DOAnchorPos(new Vector2(offScreenX, yPositions [2]), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnStart(Tweening);
		instructionsRect.DOAnchorPos(new Vector2(offScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		optionsRect.DOAnchorPos(new Vector2(offScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		creditsRect.DOAnchorPos(new Vector2(offScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		
		Tween myTween = quitRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[5]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		
		yield return myTween.WaitForCompletion();
		
		quitMenuContent.anchoredPosition = new Vector2(offScreenX, 0);
		
		mainMenuCanvas.SetActive(false);
		quitMenuCanvas.SetActive(true);
		
		quitMenuContent.DOAnchorPos(new Vector2(0, 0), durationContent).SetEase(easeTypeMainMenu).OnComplete(NotTweening);;

		no.GetComponent<Button>().Select();
	}
	
	public void ExitQuit ()
	{
		quitMenuContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu).OnComplete(LoadMainMenu);
		PlayReturnSound ();
	}

	public void LoadPlayersVoid()
	{
		if(!tweening)
		{
			StartCoroutine("LoadPlayers");
			Tweening ();
		}
	}

	IEnumerator LoadPlayers ()
	{
		optionsButtonRect.DOAnchorPos(new Vector2(offScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		soundsButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		graphicsButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 

		Tween myTween = playersButtonRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		
		yield return myTween.WaitForCompletion();
		
		playersMenuContent.anchoredPosition = new Vector2(offScreenX, 0);
		
		chooseOptionsMenuCanvas.SetActive(false);
		playersMenuCanvas.SetActive(true);
		
		playersMenuContent.DOAnchorPos(new Vector2(0, 0), durationContent).SetEase(easeTypeMainMenu).OnComplete(NotTweening);
	}
	
	public IEnumerator ExitPlayers ()
	{
		playersMenuContent.GetComponent<ControllerChangeManager1> ().UpdateGlobalVariables ();
		playersMenuContent.GetComponent<ControllerChangeManager1> ().UpdatePlayersControllers ();

		Tween myTween = playersMenuContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);

		yield return myTween.WaitForCompletion();

		chooseOptionsMenuCanvas.SetActive(true);
		playersMenuCanvas.SetActive(false);

		optionsButtonRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnStart(Tweening).OnComplete(NotTweening);
		playersButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		soundsButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		graphicsButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		sounds.GetComponent<Button>().Select();

		PlayReturnSound ();
	}

	public void LoadSoundsVoid()
	{
		if(!tweening)
		{
			StartCoroutine("LoadSounds");
			Tweening ();
		}
	}
	
	IEnumerator LoadSounds ()
	{
		optionsButtonRect.DOAnchorPos(new Vector2(offScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		//playersButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		graphicsButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
	
		
		Tween myTween = soundsButtonRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		
		yield return myTween.WaitForCompletion();
		
		soundsMenuContent.anchoredPosition = new Vector2(offScreenX, 0);
		
		chooseOptionsMenuCanvas.SetActive(false);
		soundsMenuCanvas.SetActive(true);
		
		soundsMenuContent.DOAnchorPos(new Vector2(0, 0), durationContent).SetEase(easeTypeMainMenu).OnComplete(NotTweening);

		soundsBar.GetComponent<Scrollbar>().Select();
	}
	
	public IEnumerator ExitSounds ()
	{
		Tween myTween = soundsMenuContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);
		
		yield return myTween.WaitForCompletion();
		
		chooseOptionsMenuCanvas.SetActive(true);
		soundsMenuCanvas.SetActive(false);
		
		optionsButtonRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(NotTweening);
		//playersButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		soundsButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		graphicsButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		sounds.GetComponent<Button>().Select();

		PlayReturnSound ();
	}

	public void LoadGraphicsVoid()
	{
		if(!tweening)
		{
			StartCoroutine(LoadGraphics ());
			Tweening ();
		}		
	}
	
	IEnumerator LoadGraphics ()
	{
		optionsButtonRect.DOAnchorPos(new Vector2(offScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		//playersButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		soundsButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 

		Tween myTween = graphicsButtonRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		
		yield return myTween.WaitForCompletion();
		
		graphicsMenuContent.anchoredPosition = new Vector2(offScreenX, 0);
		
		chooseOptionsMenuCanvas.SetActive(false);
		graphicsMenuCanvas.SetActive(true);
		
		graphicsMenuContent.DOAnchorPos(new Vector2(0, 0), durationContent).SetEase(easeTypeMainMenu).OnComplete(NotTweening);

		high.GetComponent<Toggle>().Select();
	}
	
	public IEnumerator ExitGraphics ()
	{
		Tween myTween = graphicsMenuContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);
		
		yield return myTween.WaitForCompletion();
		
		chooseOptionsMenuCanvas.SetActive(true);
		graphicsMenuCanvas.SetActive(false);
		
		optionsButtonRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(NotTweening);
		//playersButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		soundsButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		graphicsButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		sounds.GetComponent<Button>().Select();

		PlayReturnSound ();
	}


	public void LoadRepulseVoid()
	{
		if(!tweening)
		{
			StartCoroutine(LoadRepulse ());
			Tweening ();
		}
	}

	IEnumerator LoadRepulse ()
	{
		LoadModeManager.Instance.LoadSceneVoid ("Repulse");

		//Reset Top Button Position
		repulseMenuCanvas.transform.GetChild (0).GetComponent<RectTransform> ().anchoredPosition = (new Vector2 (onScreenX, topYpositionButton));
		repulseMenuCanvas.transform.GetChild (1).GetChild(0).GetComponent<RectTransform> ().anchoredPosition = (new Vector2 (onScreenX, topYpositionButton - 131));

		crushButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		hitButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		bombButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 

		Tween myTween = repulseButtonRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton - 131), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		yield return myTween.WaitForCompletion();

		choosePlayerContent.anchoredPosition = new Vector2(offScreenX, 0);
		repulseMenuCanvas.transform.GetChild (1).GetChild (1).GetComponent<RectTransform> ().anchoredPosition = new Vector2 (modesDescriptionXPos, modesDescriptionOffScreen);

		playButton.anchoredPosition = new Vector2(playButton.anchoredPosition.x, playButtonMinY);
		play.GetComponent<Button> ().interactable = false;


		choosePlayerCanvas.SetActive(true);
		repulseMenuCanvas.SetActive(true);
		chooseModeCanvas.SetActive(false);

		repulseMenuCanvas.transform.GetChild (1).GetChild (1).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (modesDescriptionXPos, modesDescriptionOnScreen), durationContent).SetEase(easeTypeMainMenu);

		myTween = choosePlayerContent.DOAnchorPos(new Vector2(0, 0), durationContent).SetEase(easeTypeMainMenu).OnComplete(NotTweening);
		yield return myTween.WaitForCompletion();

		//playButton.DOAnchorPos (new Vector2(playButton.anchoredPosition.x, playButtonMaxY), playButtonDuration).SetEase(easeTypeMainMenu).OnComplete(NotTweening);
		play.GetComponent<Button> ().Select ();
	}

	public IEnumerator ExitRepulse ()
	{
		playButton.DOAnchorPos (new Vector2(playButton.anchoredPosition.x, playButtonMinY), playButtonDuration).SetEase(easeTypeMainMenu);
		repulseMenuCanvas.transform.GetChild (1).GetChild (1).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (modesDescriptionXPos, modesDescriptionOffScreen), durationContent).SetEase(easeTypeMainMenu);

		Tween myTween = choosePlayerContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);
		yield return myTween.WaitForCompletion();

		choosePlayerCanvas.SetActive(false);
		repulseMenuCanvas.SetActive(false);
		chooseModeCanvas.SetActive(true);

		repulseButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [2]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		crushButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		hitButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		bombButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(NotTweening); 

		repulse.GetComponent<Button>().Select();

		PlayReturnSound ();
	}
		
	public void LoadBombVoid()
	{
		if(!tweening)
		{
			StartCoroutine(LoadBomb ());
			Tweening ();
		}
	}

	IEnumerator LoadBomb ()
	{
		LoadModeManager.Instance.LoadSceneVoid ("Bomb");

		//Reset Top Button Position
		bombMenuCanvas.transform.GetChild (0).GetComponent<RectTransform> ().anchoredPosition = (new Vector2 (onScreenX, topYpositionButton));
		bombMenuCanvas.transform.GetChild (1).GetChild(0).GetComponent<RectTransform> ().anchoredPosition = (new Vector2 (onScreenX, topYpositionButton - 131));

		crushButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		trainingButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 

		Tween myTween = bombButtonRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton - 131), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		yield return myTween.WaitForCompletion();

		choosePlayerContent.anchoredPosition = new Vector2(offScreenX, 0);
		bombMenuCanvas.transform.GetChild (1).GetChild (1).GetComponent<RectTransform> ().anchoredPosition = new Vector2 (modesDescriptionXPos, modesDescriptionOffScreen);

		playButton.anchoredPosition = new Vector2(playButton.anchoredPosition.x, playButtonMinY);
		play.GetComponent<Button> ().interactable = false;


		choosePlayerCanvas.SetActive(true);
		bombMenuCanvas.SetActive(true);
		chooseModeCanvas.SetActive(false);

		bombMenuCanvas.transform.GetChild (1).GetChild (1).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (modesDescriptionXPos, modesDescriptionOnScreen), durationContent).SetEase(easeTypeMainMenu);

		myTween = choosePlayerContent.DOAnchorPos(new Vector2(0, 0), durationContent).SetEase(easeTypeMainMenu).OnComplete(NotTweening);
		yield return myTween.WaitForCompletion();

		//playButton.DOAnchorPos (new Vector2(playButton.anchoredPosition.x, playButtonMaxY), playButtonDuration).SetEase(easeTypeMainMenu).OnComplete(NotTweening);
		play.GetComponent<Button>().Select();

	}

	public IEnumerator ExitBomb ()
	{
		playButton.DOAnchorPos (new Vector2(playButton.anchoredPosition.x, playButtonMinY), playButtonDuration).SetEase(easeTypeMainMenu);
		bombMenuCanvas.transform.GetChild (1).GetChild (1).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (modesDescriptionXPos, modesDescriptionOffScreen), durationContent).SetEase(easeTypeMainMenu);

		Tween myTween = choosePlayerContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);
		yield return myTween.WaitForCompletion();

		choosePlayerCanvas.SetActive(false);
		bombMenuCanvas.SetActive(false);
		chooseModeCanvas.SetActive(true);

		trainingButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		bombButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [2]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		crushButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(NotTweening); 

		bombButtonRect.transform.GetChild(1).GetComponent<Button>().Select();

		PlayReturnSound ();
	}

	public void LoadHitVoid()
	{
		if(!tweening)
		{
			StartCoroutine(LoadHit ());
			Tweening ();
		}
	}

	IEnumerator LoadHit ()
	{
		LoadModeManager.Instance.LoadSceneVoid ("Hit");

		//Reset Top Button Position
		hitMenuCanvas.transform.GetChild (0).GetComponent<RectTransform> ().anchoredPosition = (new Vector2 (onScreenX, topYpositionButton));
		hitMenuCanvas.transform.GetChild (1).GetChild(0).GetComponent<RectTransform> ().anchoredPosition = (new Vector2 (onScreenX, topYpositionButton - 131));

		crushButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		bombButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		repulseButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [2]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 

		Tween myTween = hitButtonRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton - 131), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		yield return myTween.WaitForCompletion();

		choosePlayerContent.anchoredPosition = new Vector2(offScreenX, 0);
		hitMenuCanvas.transform.GetChild (1).GetChild (1).GetComponent<RectTransform> ().anchoredPosition = new Vector2 (modesDescriptionXPos, modesDescriptionOffScreen);

		playButton.anchoredPosition = new Vector2(playButton.anchoredPosition.x, playButtonMinY);
		play.GetComponent<Button> ().interactable = false;


		choosePlayerCanvas.SetActive(true);
		hitMenuCanvas.SetActive(true);
		chooseModeCanvas.SetActive(false);

		hitMenuCanvas.transform.GetChild (1).GetChild (1).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (modesDescriptionXPos, modesDescriptionOnScreen), durationContent).SetEase(easeTypeMainMenu);

		myTween = choosePlayerContent.DOAnchorPos(new Vector2(0, 0), durationContent).SetEase(easeTypeMainMenu).OnComplete(NotTweening);
		yield return myTween.WaitForCompletion();

		//playButton.DOAnchorPos (new Vector2(playButton.anchoredPosition.x, playButtonMaxY), playButtonDuration).SetEase(easeTypeMainMenu).OnComplete(NotTweening);
		play.GetComponent<Button> ().Select ();
	}

	public IEnumerator ExitHit ()
	{
		playButton.DOAnchorPos (new Vector2(playButton.anchoredPosition.x, playButtonMinY), playButtonDuration).SetEase(easeTypeMainMenu);
		hitMenuCanvas.transform.GetChild (1).GetChild (1).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (modesDescriptionXPos, modesDescriptionOffScreen), durationContent).SetEase(easeTypeMainMenu);

		Tween myTween = choosePlayerContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);
		yield return myTween.WaitForCompletion();

		choosePlayerCanvas.SetActive(false);
		hitMenuCanvas.SetActive(false);
		chooseModeCanvas.SetActive(true);

		hitButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		crushButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		bombButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		repulseButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [2]), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(NotTweening); 

		repulse.GetComponent<Button>().Select();

		PlayReturnSound ();
	}

	public void LoadCrushVoid()
	{
		if(!tweening)
		{
			StartCoroutine(LoadCrush ());
			Tweening ();
		}
	}

	IEnumerator LoadCrush ()
	{
		LoadModeManager.Instance.LoadSceneVoid ("Crush");

		//Reset Top Button Position
		crushMenuCanvas.transform.GetChild (0).GetComponent<RectTransform> ().anchoredPosition = (new Vector2 (onScreenX, topYpositionButton));
		crushMenuCanvas.transform.GetChild (1).GetChild(0).GetComponent<RectTransform> ().anchoredPosition = (new Vector2 (onScreenX, topYpositionButton - 131));

		bombButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [2]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		trainingButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 

		Tween myTween = crushButtonRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton - 131), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		yield return myTween.WaitForCompletion();

		choosePlayerContent.anchoredPosition = new Vector2(offScreenX, 0);
		crushMenuCanvas.transform.GetChild (1).GetChild (1).GetComponent<RectTransform> ().anchoredPosition = new Vector2 (modesDescriptionXPos, modesDescriptionOffScreen);

		playButton.anchoredPosition = new Vector2(playButton.anchoredPosition.x, playButtonMinY);
		play.GetComponent<Button> ().interactable = false;


		choosePlayerCanvas.SetActive(true);
		crushMenuCanvas.SetActive(true);
		chooseModeCanvas.SetActive(false);

		crushMenuCanvas.transform.GetChild (1).GetChild (1).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (modesDescriptionXPos, modesDescriptionOnScreen), durationContent).SetEase(easeTypeMainMenu);

		myTween = choosePlayerContent.DOAnchorPos(new Vector2(0, 0), durationContent).SetEase(easeTypeMainMenu).OnComplete(NotTweening);
		yield return myTween.WaitForCompletion();

		//playButton.DOAnchorPos (new Vector2(playButton.anchoredPosition.x, playButtonMaxY), playButtonDuration).SetEase(easeTypeMainMenu).OnComplete(NotTweening);
		play.GetComponent<Button>().Select();
	}

	public IEnumerator ExitCrush ()
	{
		playButton.DOAnchorPos (new Vector2(playButton.anchoredPosition.x, playButtonMinY), playButtonDuration).SetEase(easeTypeMainMenu);
		crushMenuCanvas.transform.GetChild (1).GetChild (1).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (modesDescriptionXPos, modesDescriptionOffScreen), durationContent).SetEase(easeTypeMainMenu);

		Tween myTween = choosePlayerContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);
		yield return myTween.WaitForCompletion();

		choosePlayerCanvas.SetActive(false);
		crushMenuCanvas.SetActive(false);
		chooseModeCanvas.SetActive(true);

		trainingButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		crushButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		bombButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [2]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(NotTweening); 

		bombButtonRect.transform.GetChild(1).GetComponent<Button>().Select();

		PlayReturnSound ();
	}

	public void LoadTrainingVoid()
	{
		if(!tweening)
		{
			StartCoroutine(LoadTraining ());
			Tweening ();
		}
	}

	IEnumerator LoadTraining ()
	{
		LoadModeManager.Instance.LoadSceneVoid ("Training");

		//Reset Top Button Position
		trainingMenuCanvas.transform.GetChild (0).GetComponent<RectTransform> ().anchoredPosition = (new Vector2 (onScreenX, topYpositionButton));
		trainingMenuCanvas.transform.GetChild (1).GetChild(0).GetComponent<RectTransform> ().anchoredPosition = (new Vector2 (onScreenX, topYpositionButton - 131));

		bombButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [2]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		crushButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 

		Tween myTween = trainingButtonRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton - 131), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		yield return myTween.WaitForCompletion();

		choosePlayerContent.anchoredPosition = new Vector2(offScreenX, 0);
		trainingMenuCanvas.transform.GetChild (1).GetChild (1).GetComponent<RectTransform> ().anchoredPosition = new Vector2 (modesDescriptionXPos, modesDescriptionOffScreen);

		playButton.anchoredPosition = new Vector2(playButton.anchoredPosition.x, playButtonMinY);
		play.GetComponent<Button> ().interactable = false;


		choosePlayerCanvas.SetActive(true);
		trainingMenuCanvas.SetActive(true);
		chooseModeCanvas.SetActive(false);

		trainingMenuCanvas.transform.GetChild (1).GetChild (1).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (modesDescriptionXPos, modesDescriptionOnScreen), durationContent).SetEase(easeTypeMainMenu);

		myTween = choosePlayerContent.DOAnchorPos(new Vector2(0, 0), durationContent).SetEase(easeTypeMainMenu).OnComplete(NotTweening);
		yield return myTween.WaitForCompletion();

		//playButton.DOAnchorPos (new Vector2(playButton.anchoredPosition.x, playButtonMaxY), playButtonDuration).SetEase(easeTypeMainMenu).OnComplete(NotTweening);
		play.GetComponent<Button>().Select();
	}

	public IEnumerator ExitTraining ()
	{
		playButton.DOAnchorPos (new Vector2(playButton.anchoredPosition.x, playButtonMinY), playButtonDuration).SetEase(easeTypeMainMenu);
		trainingMenuCanvas.transform.GetChild (1).GetChild (1).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (modesDescriptionXPos, modesDescriptionOffScreen), durationContent).SetEase(easeTypeMainMenu);

		Tween myTween = choosePlayerContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);
		yield return myTween.WaitForCompletion();

		choosePlayerCanvas.SetActive(false);
		trainingMenuCanvas.SetActive(false);
		chooseModeCanvas.SetActive(true);

		crushButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		trainingButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		bombButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [2]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(NotTweening); 

		bombButtonRect.transform.GetChild(1).GetComponent<Button>().Select();

		PlayReturnSound ();
	}

	public void LoadModeSelection ()
	{
		SceneManager.LoadScene("ModeSelection");
	}
		
	public void ExitGame ()
	{
		Application.Quit ();
	}

	public void LoadMainMenu ()
	{
		Tweening ();

		if(instructionsRect.anchoredPosition.x == onScreenX)
		{
			resumeRect.anchoredPosition = new Vector2 (onScreenX, 700);
			resumeRect.DOAnchorPos(new Vector2(onScreenX, yPositions [1] - 16), durationCancel).SetDelay(delayCancel[5]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(NotTweening);

			startRect.DOAnchorPos(new Vector2(onScreenX, yPositions [2]), durationCancel).SetDelay(delayCancel[4]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			optionsRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationCancel).SetDelay(delayCancel[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			creditsRect.DOAnchorPos(new Vector2(onScreenX, yPositions [5]), durationCancel).SetDelay(delayCancel[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			quitRect.DOAnchorPos(new Vector2(onScreenX, yPositions [6]), durationCancel).SetDelay(delayCancel[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			
			instructionsRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationCancel).SetDelay(delayCancel[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		}
		
		else if(optionsRect.anchoredPosition.x == onScreenX)
		{
			resumeRect.anchoredPosition = new Vector2 (onScreenX, 700);
			resumeRect.DOAnchorPos(new Vector2(onScreenX, yPositions [1] - 16), durationCancel).SetDelay(delayCancel[5]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(NotTweening);

			startRect.DOAnchorPos(new Vector2(onScreenX, yPositions [2]), durationCancel).SetDelay(delayCancel[4]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			instructionsRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationCancel).SetDelay(delayCancel[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			creditsRect.DOAnchorPos(new Vector2(onScreenX, yPositions [5]), durationCancel).SetDelay(delayCancel[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			quitRect.DOAnchorPos(new Vector2(onScreenX, yPositions [6]), durationCancel).SetDelay(delayCancel[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			
			optionsRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationCancel).SetDelay(delayCancel[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		}
		
		else if(creditsRect.anchoredPosition.x == onScreenX)
		{
			resumeRect.anchoredPosition = new Vector2 (onScreenX, 700);
			resumeRect.DOAnchorPos(new Vector2(onScreenX, yPositions [1] - 16), durationCancel).SetDelay(delayCancel[5]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(NotTweening);

			startRect.DOAnchorPos(new Vector2(onScreenX, yPositions [2]), durationCancel).SetDelay(delayCancel[4]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			instructionsRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationCancel).SetDelay(delayCancel[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			optionsRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationCancel).SetDelay(delayCancel[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			quitRect.DOAnchorPos(new Vector2(onScreenX, yPositions [6]), durationCancel).SetDelay(delayCancel[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			
			creditsRect.DOAnchorPos(new Vector2(onScreenX, yPositions [5]), durationCancel).SetDelay(delayCancel[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		}
		
		else if(quitRect.anchoredPosition.x == onScreenX)
		{
			resumeRect.anchoredPosition = new Vector2 (onScreenX, 700);
			resumeRect.DOAnchorPos(new Vector2(onScreenX, yPositions [1] - 16), durationCancel).SetDelay(delayCancel[5]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(NotTweening);

			startRect.DOAnchorPos(new Vector2(onScreenX, yPositions [2]), durationCancel).SetDelay(delayCancel[4]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			instructionsRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationCancel).SetDelay(delayCancel[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			optionsRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationCancel).SetDelay(delayCancel[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			creditsRect.DOAnchorPos(new Vector2(onScreenX, yPositions [5]), durationCancel).SetDelay(delayCancel[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			
			quitRect.DOAnchorPos(new Vector2(onScreenX, yPositions [6]), durationCancel).SetDelay(delayCancel[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		}
	
		else if(startRect.anchoredPosition.x == onScreenX)
		{
			resumeRect.anchoredPosition = new Vector2 (onScreenX, 700);
			resumeRect.DOAnchorPos(new Vector2(onScreenX, yPositions [1] - 16), durationCancel).SetDelay(delayCancel[5]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(NotTweening);

			instructionsRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationCancel).SetDelay(delayCancel[4]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			optionsRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationCancel).SetDelay(delayCancel[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			creditsRect.DOAnchorPos(new Vector2(onScreenX, yPositions [5]), durationCancel).SetDelay(delayCancel[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			quitRect.DOAnchorPos(new Vector2(onScreenX, yPositions [6]), durationCancel).SetDelay(delayCancel[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

			startRect.DOAnchorPos(new Vector2(onScreenX, yPositions [2]), durationCancel).SetDelay(delayCancel[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		}

		else
		{
			resumeRect.anchoredPosition = new Vector2 (onScreenX, 700);
			resumeRect.DOAnchorPos(new Vector2(onScreenX, yPositions [1] - 16), durationCancel).SetDelay(delayCancel[5]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(NotTweening);

			startRect.DOAnchorPos(new Vector2(onScreenX, yPositions [2]), durationCancel).SetDelay(delayCancel[4]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			instructionsRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationCancel).SetDelay(delayCancel[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			optionsRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationCancel).SetDelay(delayCancel[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			creditsRect.DOAnchorPos(new Vector2(onScreenX, yPositions [5]), durationCancel).SetDelay(delayCancel[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			quitRect.DOAnchorPos(new Vector2(onScreenX, yPositions [6]), durationCancel).SetDelay(delayCancel[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		}

		if(smallLogo.anchoredPosition.y != 0)
			smallLogo.DOAnchorPos(new Vector2(0, 0), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		if(backButtonsContent.anchoredPosition.x != offScreenX)
			backButtonsContent.DOAnchorPos (new Vector2(offScreenX, backButtonsInitialPos.y), durationContent).SetEase (easeTypeMainMenu).SetId("BackButtons");

		instructionsMenuCanvas.SetActive(false);
		chooseOptionsMenuCanvas.SetActive(false);
		creditsMenuCanvas.SetActive(false);
		quitMenuCanvas.SetActive(false);
		chooseModeCanvas.SetActive(false);
		gameOverCanvas.SetActive(false);

		mainMenuCanvas.SetActive(true);
		start.GetComponent<Button>().Select();
	}
		



	public void GameOverMenuVoid ()
	{
		if(!tweening)
		{
			Tweening ();
			StartCoroutine (GameOverMenu ());
		}
	}

	IEnumerator GameOverMenu ()
	{
		mainCamera.transform.DOMove(gameOverPosition, cameraMovementDuration).SetEase(cameraEaseMovement);
		yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(cameraMovementDuration - 0.5f));

		goRepulseContent.anchoredPosition = new Vector2 (-offScreenX, goRepulseContent.anchoredPosition.y);
		goBombContent.anchoredPosition = new Vector2 (-offScreenX, goBombContent.anchoredPosition.y);
		goHitContent.anchoredPosition = new Vector2 (-offScreenX, goHitContent.anchoredPosition.y);
		goCrushContent.anchoredPosition = new Vector2 (-offScreenX, goCrushContent.anchoredPosition.y);
		goTrainingContent.anchoredPosition = new Vector2 (-offScreenX, goTrainingContent.anchoredPosition.y);

		gameOverCanvas.SetActive (true);

		gameOverButton.anchoredPosition = new Vector2 (gameOverButton.anchoredPosition.x, 800);
		gameOverButton.DOAnchorPos (new Vector2(gameOverButton.anchoredPosition.x, topYpositionButton), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		restartButton.anchoredPosition = new Vector2 (restartButton.anchoredPosition.x, playButtonMinY);
		menuButton.anchoredPosition = new Vector2 (menuButton.anchoredPosition.x, playButtonMinY);
		restartButton.DOAnchorPos (new Vector2(restartButton.anchoredPosition.x, bottomYPosition), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		menuButton.DOAnchorPos (new Vector2(menuButton.anchoredPosition.x, bottomYPosition), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");


		switch(GlobalVariables.Instance.CurrentModeLoaded)
		{
		case "Repulse":
			goRepulseContent.gameObject.SetActive (true);
			goRepulseContent.DOAnchorPos (new Vector2(0, goRepulseContent.anchoredPosition.y), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete (NotTweening);
			break;
		case "Bomb":
			goBombContent.gameObject.SetActive (true);
			goBombContent.DOAnchorPos (new Vector2(0, goBombContent.anchoredPosition.y), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete (NotTweening);
			break;
		case "Hit":
			goHitContent.gameObject.SetActive (true);
			goHitContent.DOAnchorPos (new Vector2(0, goHitContent.anchoredPosition.y), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete (NotTweening);
			break;
		case "Crush":
			goCrushContent.gameObject.SetActive (true);
			goCrushContent.DOAnchorPos (new Vector2(0, goCrushContent.anchoredPosition.y), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete (NotTweening);
			break;
		case "Training":
			goTrainingContent.gameObject.SetActive (true);
			goTrainingContent.DOAnchorPos (new Vector2(0, goTrainingContent.anchoredPosition.y), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete (NotTweening);
			break;
		}

		eventSyst.SetSelectedGameObject(null);
		restart.GetComponent<Button> ().Select ();
	}

	public void MainMenuVoid ()
	{
		if(!tweening)
		{
			Tweening ();
			StartCoroutine (MainMenu ());
		}
	}

	IEnumerator MainMenu ()
	{
		switch(GlobalVariables.Instance.CurrentModeLoaded)
		{
		case "Repulse":
			goRepulseContent.DOAnchorPos (new Vector2(-offScreenX, goRepulseContent.anchoredPosition.y), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			break;
		case "Bomb":
			goBombContent.DOAnchorPos (new Vector2 (-offScreenX, goBombContent.anchoredPosition.y), durationSubmit).SetDelay (delaySubmit [0]).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			break;
		case "Hit":
			goHitContent.DOAnchorPos (new Vector2 (-offScreenX, goHitContent.anchoredPosition.y), durationSubmit).SetDelay (delaySubmit [0]).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			break;
		case "Crush":
			goCrushContent.DOAnchorPos (new Vector2 (-offScreenX, goCrushContent.anchoredPosition.y), durationSubmit).SetDelay (delaySubmit [0]).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			break;
		case "Training":
			goTrainingContent.DOAnchorPos (new Vector2 (-offScreenX, goTrainingContent.anchoredPosition.y), durationSubmit).SetDelay (delaySubmit [0]).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			break;
		}

		menuButton.DOAnchorPos (new Vector2(menuButton.anchoredPosition.x, -800), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		restartButton.DOAnchorPos (new Vector2(restartButton.anchoredPosition.x, -800), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		gameOverButton.DOAnchorPos (new Vector2(gameOverButton.anchoredPosition.x, 800), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete (NotTweening);

		Tween myTween = mainCamera.transform.DOMove (pausePosition, cameraMovementDuration).SetEase(cameraEaseMovement);
		yield return myTween.WaitForCompletion ();

		gameOverCanvas.SetActive (false);
		mainMenuCanvas.SetActive (true);

		startRect.anchoredPosition = new Vector2(offScreenX, yPositions [2]);
		instructionsRect.anchoredPosition = new Vector2(offScreenX, yPositions [3]);
		optionsRect.anchoredPosition = new Vector2(offScreenX, yPositions [4]);
		creditsRect.anchoredPosition = new Vector2(offScreenX, yPositions [5]);
		quitRect.anchoredPosition = new Vector2(offScreenX, yPositions [6]);
		resumeRect.anchoredPosition = new Vector2(offScreenX, yPositions [1] - 16);

		MasterAudio.PlaySound (openMenuSound);

		GlobalVariables.Instance.OnMainMenuVoid ();

		loadModeScript.ReloadSceneVoid ();

		LoadMainMenu ();
	}

	public void RestartVoid ()
	{
		if(!tweening)
		{
			Tweening ();
			StartCoroutine (Restart ());
		}
	}

	IEnumerator Restart ()
	{
		switch(GlobalVariables.Instance.CurrentModeLoaded)
		{
		case "Repulse":
			goRepulseContent.DOAnchorPos (new Vector2(-offScreenX, goRepulseContent.anchoredPosition.y), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			break;
		case "Bomb":
			goBombContent.DOAnchorPos (new Vector2 (-offScreenX, goBombContent.anchoredPosition.y), durationSubmit).SetDelay (delaySubmit [0]).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			break;
		case "Hit":
			goHitContent.DOAnchorPos (new Vector2 (-offScreenX, goHitContent.anchoredPosition.y), durationSubmit).SetDelay (delaySubmit [0]).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			break;
		case "Crush":
			goCrushContent.DOAnchorPos (new Vector2 (-offScreenX, goCrushContent.anchoredPosition.y), durationSubmit).SetDelay (delaySubmit [0]).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			break;
		case "Training":
			goTrainingContent.DOAnchorPos (new Vector2 (-offScreenX, goTrainingContent.anchoredPosition.y), durationSubmit).SetDelay (delaySubmit [0]).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			break;
		}

		menuButton.DOAnchorPos (new Vector2(menuButton.anchoredPosition.x, playButtonMinY), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		restartButton.DOAnchorPos (new Vector2(restartButton.anchoredPosition.x, playButtonMinY), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		Tween myTween = gameOverButton.DOAnchorPos (new Vector2(gameOverButton.anchoredPosition.x, 800), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete (NotTweening);
		yield return myTween.WaitForCompletion ();

		gameOverCanvas.SetActive (false);

		MasterAudio.PlaySound (closeMenuSound);

		loadModeScript.RestartSceneVoid ();
	}
		


	void Tweening ()
	{
		tweening = true;
	}

	void NotTweening ()
	{
		tweening = false;
	}

	void PlayReturnSound ()
	{
		MasterAudio.PlaySound (returnSound);
	}
}