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

	[Header ("Head Buttons")]
	public RectTransform[] topMenuButtons;

	[Header ("Choose Mode Menu")]
	public GameObject[] modesDescription = new GameObject[3];
	public GameObject[] modesTeam = new GameObject[3];
	public RectTransform playButton;
	public float playButtonMinY = -700;
	public float playButtonMaxY = -457;
	public float playButtonDuration = 0.25f;

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
	public RectTransform gameOverContent;
	public float bottomYPosition = -400;

	[Header ("Ease")]
	public Ease easeTypeMainMenu;

	[Header ("Menu Sounds")]
	[SoundGroupAttribute]
	public string returnSound;

	[Header ("Buttons To Select When Nothing Is")]
	public GameObject start;
	public GameObject players;
	public GameObject no;
	public GameObject high;
	public GameObject overall;
	public GameObject crush;
	public GameObject play;

	[Header ("All Canvas")]
	public GameObject mainMenuCanvas;
	public GameObject instructionsMenuCanvas;
	public GameObject chooseOptionsMenuCanvas;
	public GameObject soundsMenuCanvas;
	public GameObject qualityMenuCanvas;
	public GameObject playersMenuCanvas;
	public GameObject creditsMenuCanvas;
	public GameObject quitMenuCanvas;
	public GameObject chooseModeCanvas;
	public GameObject crushMenuCanvas;
	public GameObject footballMenuCanvas;
	public GameObject hitMenuCanvas;
	public GameObject chooseTeamCanvas;

	[Header ("All Contents")]
	public RectTransform instructionsMenuContent;
	public RectTransform optionsMenuContent;
	public RectTransform soundsMenuContent;
	public RectTransform qualityMenuContent;
	public RectTransform playersMenuContent;
	public RectTransform creditsMenuContent;
	public RectTransform quitMenuContent;
	public RectTransform chooseModeContent;
	public RectTransform chooseTeamContent;

	private RectTransform startRect;
	private RectTransform instructionsRect;
	private RectTransform optionsRect;
	private RectTransform creditsRect;
	private RectTransform quitRect;
	private RectTransform resumeRect;

	private RectTransform optionsButtonRect;
	private RectTransform playersButtonRect;
	private RectTransform soundsButtonRect;
	private RectTransform qualityButtonRect;

	private RectTransform crushButtonRect;
	private RectTransform footballButtonRect;
	private RectTransform hitButtonRect;

	private RectTransform backTextRect;
	private RectTransform bButtonRect;
	private RectTransform escButtonRect;

	private RectTransform backTextRect2;
	private RectTransform bButtonRect2;
	private RectTransform escButtonRect2;

	private bool startScreen = true;

	private LoadModeManager loadModeScript;

	private GameObject mainCamera;

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
		qualityButtonRect = chooseOptionsMenuCanvas.transform.GetChild(1).GetChild(2).GetComponent<RectTransform>();

		crushButtonRect = chooseModeCanvas.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>();
		footballButtonRect = chooseModeCanvas.transform.GetChild(1).GetChild(1).GetComponent<RectTransform>();
		hitButtonRect = chooseModeCanvas.transform.GetChild(1).GetChild(2).GetComponent<RectTransform>();

		backTextRect = chooseOptionsMenuCanvas.transform.GetChild(1).GetChild(3).GetComponent<RectTransform>();
		bButtonRect = chooseOptionsMenuCanvas.transform.GetChild(1).GetChild(4).GetComponent<RectTransform>();
		escButtonRect = chooseOptionsMenuCanvas.transform.GetChild(1).GetChild(5).GetComponent<RectTransform>();

		backTextRect2 = chooseModeCanvas.transform.GetChild(1).GetChild(3).GetComponent<RectTransform>();
		bButtonRect2 = chooseModeCanvas.transform.GetChild(1).GetChild(4).GetComponent<RectTransform>();
		escButtonRect2 = chooseModeCanvas.transform.GetChild(1).GetChild(5).GetComponent<RectTransform>();

		instructionsMenuCanvas.SetActive(false);
		chooseOptionsMenuCanvas.SetActive(false);
		creditsMenuCanvas.SetActive(false);
		soundsMenuCanvas.SetActive(false);
		qualityMenuCanvas.SetActive(false);
		playersMenuCanvas.SetActive(false);
		chooseModeCanvas.SetActive(false);
		crushMenuCanvas.SetActive(false);
		footballMenuCanvas.SetActive(false);
		hitMenuCanvas.SetActive(false);
		chooseTeamCanvas.SetActive(false);
		gameOverCanvas.SetActive(false);


		mainMenuCanvas.SetActive(true);
		start.GetComponent<Button>().Select();


		startRect.anchoredPosition = new Vector2(offScreenX, yPositions [2]);
		instructionsRect.anchoredPosition = new Vector2(offScreenX, yPositions [3]);
		optionsRect.anchoredPosition =new Vector2(offScreenX, yPositions [4]);
		creditsRect.anchoredPosition = new Vector2(offScreenX, yPositions [5]);
		quitRect.anchoredPosition = new Vector2(offScreenX, yPositions [6]);
		resumeRect.anchoredPosition = new Vector2(offScreenX, yPositions [1] - 16);

		//LoadMainMenu();

		mainMenuCanvas.SetActive(false);
		smallLogo.transform.GetChild(0).GetComponent<Image>().enabled = false;
		smallLogo.transform.GetChild(1).GetComponent<Image>().enabled = false;
		smallLogo.transform.GetChild(2).GetComponent<Image>().enabled = false;

		logoMenu.gameObject.SetActive(true);
		logoMenu.transform.parent.GetChild(1).gameObject.SetActive(true);

		GameObject.FindGameObjectWithTag("MainCamera").transform.position = new Vector3 (-140, 93, 16);
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
		if(startScreen == true && Input.GetAxisRaw("Submit") > 0)
		{
			startScreen = false;

			StartCoroutine(StartScreen ());
			Tweening ();
		}

		if(Input.GetAxisRaw("Cancel") > 0 && !tweening)
        {

			if(instructionsMenuCanvas.activeSelf == true)
			{
				ExitInstructions ();
				Tweening ();
			}

			if(chooseOptionsMenuCanvas.activeSelf == true && playersMenuCanvas.activeSelf == false && soundsMenuCanvas.activeSelf == false && qualityMenuCanvas.activeSelf == false)
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

			if(qualityMenuCanvas.activeSelf == true)
			{
				StartCoroutine("ExitQuality");
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

			if(footballMenuCanvas.activeSelf == true)
			{
				StartCoroutine (ExitFootball ());
				Tweening ();
			}

			if(hitMenuCanvas.activeSelf == true)
			{
				StartCoroutine (ExitHit ());
				Tweening ();
			}

        }

		if(eventSyst.currentSelectedGameObject == null && mainMenuCanvas.activeSelf == true)
		{
			start.GetComponent<Button>().Select();
		}

		if(eventSyst.currentSelectedGameObject == null && chooseOptionsMenuCanvas.activeSelf == true)
		{
			players.GetComponent<Button>().Select();
		}

		if(eventSyst.currentSelectedGameObject == null && quitMenuCanvas.activeSelf == true)
		{
			no.GetComponent<Button>().Select();
		}

		if(eventSyst.currentSelectedGameObject == null && chooseModeCanvas.activeSelf == true)
		{
			crush.GetComponent<Button>().Select();
		}

		if(crushMenuCanvas.activeSelf == true || footballMenuCanvas.activeSelf == true || hitMenuCanvas.activeSelf == true)
		{
			if(eventSyst.currentSelectedGameObject == null)
				play.GetComponent<Button>().Select();

			CheckCanPlay ();
		}

		SetButtonsNavigation ();
	}

	void SetButtonsNavigation ()
	{
		if(StaticVariables.Instance.GameOver && resumeRect.gameObject.activeSelf == true)
		{
			resumeRect.gameObject.SetActive (false);

			var startNavigation = startRect.GetChild (1).GetComponent<Selectable> ().navigation;
			startNavigation.selectOnUp = quitRect.GetChild (1).GetComponent<Selectable>();

			startRect.GetChild (1).GetComponent<Selectable> ().navigation = startNavigation;

			var quitNavigation = quitRect.GetChild (1).GetComponent<Selectable> ().navigation;
			quitNavigation.selectOnDown = startRect.GetChild (1).GetComponent<Selectable>();

			quitRect.GetChild (1).GetComponent<Selectable> ().navigation = quitNavigation;
		}

		else if(!StaticVariables.Instance.GameOver && resumeRect.gameObject.activeSelf == false)
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
		logoMenu.transform.parent.GetChild(1).gameObject.SetActive(false);

		logoMenu.transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 365),  shrinkDuration);
		logoMenu.transform.GetChild(1).GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 365),  shrinkDuration);
		logoMenu.transform.GetChild(2).GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 365),  shrinkDuration);

		logoMenu.transform.GetChild(0).GetComponent<RectTransform>().DOScale(0.366f, shrinkDuration);
		logoMenu.transform.GetChild(1).GetComponent<RectTransform>().DOScale(0.366f, shrinkDuration);

		Tween myTween = logoMenu.transform.GetChild(2).GetComponent<RectTransform>().DOScale(0.366f, shrinkDuration);

		yield return myTween.WaitForCompletion();

		logoMenu.gameObject.SetActive(false);

		smallLogo.transform.GetChild(0).GetComponent<Image>().enabled = true;
		smallLogo.transform.GetChild(1).GetComponent<Image>().enabled = true;
		smallLogo.transform.GetChild(2).GetComponent<Image>().enabled = true;

		startRect.anchoredPosition = new Vector2(offScreenX, yPositions [2]);
		instructionsRect.anchoredPosition = new Vector2(offScreenX, yPositions [3]);
		optionsRect.anchoredPosition = new Vector2(offScreenX, yPositions [4]);
		creditsRect.anchoredPosition = new Vector2(offScreenX, yPositions [5]);
		quitRect.anchoredPosition = new Vector2(offScreenX, yPositions [6]);
		resumeRect.anchoredPosition = new Vector2(offScreenX, yPositions [1] - 16);

		GameObject.FindGameObjectWithTag("MainCamera").transform.DOMoveX(cameraNewXPosition, 1).SetEase(Ease.InOutCubic);


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
		if(StaticVariables.Instance.GamePaused == false && StaticVariables.Instance.GameOver == false)
		{
			Tweening ();

			mainCamera.GetComponent<SlowMotionCamera> ().StartPauseSlowMotion ();
			//Wait Slowmotion
			yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(timeBeforePause));

			StaticVariables.Instance.GamePaused = true;


			//GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DOTweenPath>().DOPlayBackwards();
			mainCamera.transform.DOMove (pausePosition, cameraMovementDuration).SetEase(cameraEaseMovement);

			yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(cameraMovementDuration - 0.5f));

			LoadMainMenu ();
		}

		else if(StaticVariables.Instance.GamePaused == true && StaticVariables.Instance.GameOver == false)
		{
			Tweening ();

			resumeRect.DOAnchorPos(new Vector2(offScreenX, yPositions [1] - 16), durationCancel).SetDelay(delayCancel[5]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

			startRect.DOAnchorPos(new Vector2(offScreenX, yPositions [2]), durationSubmit).SetDelay(delaySubmit[4]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			instructionsRect.DOAnchorPos(new Vector2(offScreenX, yPositions [3]), durationCancel).SetDelay(delayCancel[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			optionsRect.DOAnchorPos(new Vector2(offScreenX, yPositions [4]), durationCancel).SetDelay(delayCancel[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			creditsRect.DOAnchorPos(new Vector2(offScreenX, yPositions [5]), durationCancel).SetDelay(delayCancel[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
			quitRect.DOAnchorPos(new Vector2(offScreenX, yPositions [6]), durationCancel).SetDelay(delayCancel[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");	

			smallLogo.DOAnchorPos(new Vector2(0, 400), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");


			//GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DOTweenAnimation>().();
			mainCamera.transform.DOMove (playPosition, cameraMovementDuration).SetEase(cameraEaseMovement);

			yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(cameraMovementDuration));

			mainCamera.GetComponent<SlowMotionCamera> ().StopPauseSlowMotion ();
			//Wait Slowmotion
			yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(timeBeforeUnpause));

			mainMenuCanvas.SetActive(false);

			NotTweening ();

			StaticVariables.Instance.GamePaused = false;

		}

	}



	void CheckCanPlay ()
	{
		if(CorrectTeams () && play.GetComponent<Button>().interactable == false)
		{
			play.GetComponent<Button> ().interactable = true;
			playButton.DOAnchorPos (new Vector2(playButton.anchoredPosition.x, playButtonMaxY), playButtonDuration).SetEase(easeTypeMainMenu);
		}

		else if(!CorrectTeams () && play.GetComponent<Button>().interactable == true)
		{
			play.GetComponent<Button> ().interactable = false;
			playButton.DOAnchorPos (new Vector2(playButton.anchoredPosition.x, playButtonMinY), playButtonDuration).SetEase(easeTypeMainMenu);
		}
	}

	bool CorrectTeams ()
	{
		StaticVariables.Instance.NumberOfDisabledPlayers = 0;

		for(int i = 0; i < StaticVariables.Instance.TeamChoice.Length; i++)
		{
			if (StaticVariables.Instance.TeamChoice [i] == -1)
				StaticVariables.Instance.NumberOfDisabledPlayers++;

		}

		if (StaticVariables.Instance.NumberOfDisabledPlayers >= 3)
			return false;

		else if(StaticVariables.Instance.NumberOfPlayers == 2)
		{
			if(StaticVariables.Instance.Team1.Count == 2 || StaticVariables.Instance.Team2.Count == 2 || StaticVariables.Instance.Team3.Count == 2 || StaticVariables.Instance.Team4.Count == 2)
				return false;
			else
				return true;
		}

		else if(StaticVariables.Instance.NumberOfPlayers == 3)
		{
			if(StaticVariables.Instance.Team1.Count == 3 || StaticVariables.Instance.Team2.Count == 3 || StaticVariables.Instance.Team3.Count == 3 || StaticVariables.Instance.Team4.Count == 3)
				return false;
			else
				return true;
		}

		else if(StaticVariables.Instance.NumberOfPlayers == 4)
		{
			if(StaticVariables.Instance.Team1.Count == 4 || StaticVariables.Instance.Team2.Count == 4 || StaticVariables.Instance.Team3.Count == 4 || StaticVariables.Instance.Team4.Count == 4)
				return false;
			else
				return true;
		}

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

		switch (StaticVariables.Instance.CurrentModeLoaded)
		{
		case "Crush":
			playButton.DOAnchorPos (new Vector2 (playButton.anchoredPosition.x, playButtonMinY), playButtonDuration).SetEase (easeTypeMainMenu);
			crushButtonRect.DOAnchorPos (new Vector2 (offScreenX, yPositions [3]), durationSubmit).SetDelay (delaySubmit [0]).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			crushMenuCanvas.transform.GetChild (0).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (offScreenX, topYpositionButton), durationSubmit).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			crushMenuCanvas.transform.GetChild (1).GetChild(0).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (offScreenX, topYpositionButton - 131), durationSubmit).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			myTween = chooseTeamContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);
			break;
		case "Football":
			playButton.DOAnchorPos (new Vector2 (playButton.anchoredPosition.x, playButtonMinY), playButtonDuration).SetEase (easeTypeMainMenu);
			footballButtonRect.DOAnchorPos (new Vector2 (offScreenX, yPositions [3]), durationSubmit).SetDelay (delaySubmit [0]).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			footballMenuCanvas.transform.GetChild (0).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (offScreenX, topYpositionButton), durationSubmit).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			footballMenuCanvas.transform.GetChild (1).GetChild(0).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (offScreenX, topYpositionButton - 131), durationSubmit).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			myTween = chooseTeamContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);
			break;
		case "Hit":
			playButton.DOAnchorPos (new Vector2 (playButton.anchoredPosition.x, playButtonMinY), playButtonDuration).SetEase (easeTypeMainMenu);
			hitButtonRect.DOAnchorPos (new Vector2 (offScreenX, yPositions [3]), durationSubmit).SetDelay (delaySubmit [0]).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			hitMenuCanvas.transform.GetChild (0).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (offScreenX, topYpositionButton), durationSubmit).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			hitMenuCanvas.transform.GetChild (1).GetChild(0).GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (offScreenX, topYpositionButton - 131), durationSubmit).SetEase (easeTypeMainMenu).SetId ("MainMenuTween");
			myTween = chooseTeamContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);
			break;
		}
			
		smallLogo.DOAnchorPos(new Vector2(0, 400), durationSubmit).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		yield return myTween.WaitForCompletion ();

		chooseTeamCanvas.SetActive(false);
		crushMenuCanvas.SetActive(false);
		footballMenuCanvas.SetActive(false);
		hitMenuCanvas.SetActive(false);


		//GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DOTweenPath>().DOPlayForward();
		mainCamera.transform.DOMove (playPosition, cameraMovementDuration).SetEase(cameraEaseMovement);

		yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(cameraMovementDuration));

		NotTweening ();

		StaticVariables.Instance.GamePaused = false;
		StaticVariables.Instance.GameOver = false;

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

		instructionsMenuContent.anchoredPosition = new Vector2(-1086, 0);
		
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

		playersButtonRect.anchoredPosition = new Vector2(offScreenX, yPositions [3]);
		soundsButtonRect.anchoredPosition = new Vector2(offScreenX, yPositions [4]);
		qualityButtonRect.anchoredPosition = new Vector2(offScreenX, yPositions [5]);

		backTextRect.anchoredPosition = new Vector2(offScreenX, -433.08f);
		bButtonRect.anchoredPosition = new Vector2(offScreenX, -485);
		escButtonRect.anchoredPosition = new Vector2(offScreenX, -485);


		mainMenuCanvas.SetActive(false);
		chooseOptionsMenuCanvas.SetActive(true);
		
		playersButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(NotTweening);; 
		soundsButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		qualityButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		backTextRect.DOAnchorPos(new Vector2(-911, -433.08f), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		bButtonRect.DOAnchorPos(new Vector2(-840, -485), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		escButtonRect.DOAnchorPos(new Vector2(-904.2f, -485), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 

		players.GetComponent<Button>().Select();
	}

	public void ExitOptions ()
	{
		playersButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		soundsButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		qualityButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		backTextRect.DOAnchorPos(new Vector2(offScreenX, -433.08f), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		bButtonRect.DOAnchorPos(new Vector2(offScreenX, -485), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		escButtonRect.DOAnchorPos(new Vector2(offScreenX, -485), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(LoadMainMenu);

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

		crushButtonRect.anchoredPosition = new Vector2(offScreenX, yPositions [3]);
		footballButtonRect.anchoredPosition = new Vector2(offScreenX, yPositions [4]);
		hitButtonRect.anchoredPosition = new Vector2(offScreenX, yPositions [5]);

		backTextRect.anchoredPosition = new Vector2(offScreenX, -433.08f);
		bButtonRect.anchoredPosition = new Vector2(offScreenX, -485);
		escButtonRect.anchoredPosition = new Vector2(offScreenX, -485);


		mainMenuCanvas.SetActive(false);
		chooseModeCanvas.SetActive(true);

		crushButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(NotTweening); 
		footballButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		hitButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		backTextRect2.DOAnchorPos(new Vector2(-911, -433.08f), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		bButtonRect2.DOAnchorPos(new Vector2(-840, -485), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		escButtonRect2.DOAnchorPos(new Vector2(-904.2f, -485), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 

		crush.GetComponent<Button>().Select();
	}

	public void ExitChooseMode ()
	{
		crushButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		footballButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		hitButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		backTextRect2.DOAnchorPos(new Vector2(offScreenX, -433.08f), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		bButtonRect2.DOAnchorPos(new Vector2(offScreenX, -485), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		escButtonRect2.DOAnchorPos(new Vector2(offScreenX, -485), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(LoadMainMenu);

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
		backTextRect.DOAnchorPos(new Vector2(offScreenX, -433.08f), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnStart(Tweening);
		bButtonRect.DOAnchorPos(new Vector2(offScreenX, -485), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		escButtonRect.DOAnchorPos(new Vector2(offScreenX, -485), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");


		optionsButtonRect.DOAnchorPos(new Vector2(offScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		soundsButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		qualityButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 

		Tween myTween = playersButtonRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		
		yield return myTween.WaitForCompletion();
		
		playersMenuContent.anchoredPosition = new Vector2(offScreenX, 0);
		
		chooseOptionsMenuCanvas.SetActive(false);
		playersMenuCanvas.SetActive(true);
		
		playersMenuContent.DOAnchorPos(new Vector2(0, 0), durationContent).SetEase(easeTypeMainMenu).OnComplete(NotTweening);
	}
	
	public IEnumerator ExitPlayers ()
	{
		playersMenuContent.GetComponent<ControllerChangeManager1> ().UpdateStaticVariables ();
		playersMenuContent.GetComponent<ControllerChangeManager1> ().UpdatePlayersControllers ();

		Tween myTween = playersMenuContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);

		yield return myTween.WaitForCompletion();

		chooseOptionsMenuCanvas.SetActive(true);
		playersMenuCanvas.SetActive(false);

		optionsButtonRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnStart(Tweening).OnComplete(NotTweening);
		playersButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		soundsButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		qualityButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		backTextRect.DOAnchorPos(new Vector2(-911, -433.08f), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		bButtonRect.DOAnchorPos(new Vector2(-840, -485), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		escButtonRect.DOAnchorPos(new Vector2(-904.2f, -485), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		players.GetComponent<Button>().Select();

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
		backTextRect.DOAnchorPos(new Vector2(offScreenX, -433.08f), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnStart(Tweening);
		bButtonRect.DOAnchorPos(new Vector2(offScreenX, -485), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		escButtonRect.DOAnchorPos(new Vector2(offScreenX, -485), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		
		
		optionsButtonRect.DOAnchorPos(new Vector2(offScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		playersButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		qualityButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
	
		
		Tween myTween = soundsButtonRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		
		yield return myTween.WaitForCompletion();
		
		soundsMenuContent.anchoredPosition = new Vector2(offScreenX, 0);
		
		chooseOptionsMenuCanvas.SetActive(false);
		soundsMenuCanvas.SetActive(true);
		
		soundsMenuContent.DOAnchorPos(new Vector2(0, 0), durationContent).SetEase(easeTypeMainMenu).OnComplete(NotTweening);

		overall.GetComponent<Scrollbar>().Select();
	}
	
	public IEnumerator ExitSounds ()
	{
		Tween myTween = soundsMenuContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);
		
		yield return myTween.WaitForCompletion();
		
		chooseOptionsMenuCanvas.SetActive(true);
		soundsMenuCanvas.SetActive(false);
		
		optionsButtonRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(NotTweening);
		playersButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		soundsButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		qualityButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		
		backTextRect.DOAnchorPos(new Vector2(-911, -433.08f), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		bButtonRect.DOAnchorPos(new Vector2(-840, -485), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		escButtonRect.DOAnchorPos(new Vector2(-904.2f, -485), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		players.GetComponent<Button>().Select();

		PlayReturnSound ();
	}

	public void LoadQualityVoid()
	{
		if(!tweening)
		{
			StartCoroutine("LoadQuality");
			Tweening ();
		}		
	}
	
	IEnumerator LoadQuality ()
	{
		backTextRect.DOAnchorPos(new Vector2(offScreenX, -433.08f), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnStart(Tweening);
		bButtonRect.DOAnchorPos(new Vector2(offScreenX, -485), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		escButtonRect.DOAnchorPos(new Vector2(offScreenX, -485), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		
		
		optionsButtonRect.DOAnchorPos(new Vector2(offScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		playersButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		soundsButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 

		Tween myTween = qualityButtonRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		
		yield return myTween.WaitForCompletion();
		
		qualityMenuContent.anchoredPosition = new Vector2(offScreenX, 0);
		
		chooseOptionsMenuCanvas.SetActive(false);
		qualityMenuCanvas.SetActive(true);
		
		qualityMenuContent.DOAnchorPos(new Vector2(0, 0), durationContent).SetEase(easeTypeMainMenu).OnComplete(NotTweening);

		high.GetComponent<Toggle>().Select();
	}
	
	public IEnumerator ExitQuality ()
	{
		Tween myTween = qualityMenuContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);
		
		yield return myTween.WaitForCompletion();
		
		chooseOptionsMenuCanvas.SetActive(true);
		qualityMenuCanvas.SetActive(false);
		
		optionsButtonRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(NotTweening);
		playersButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		soundsButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		qualityButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		
		backTextRect.DOAnchorPos(new Vector2(-911, -433.08f), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		bButtonRect.DOAnchorPos(new Vector2(-840, -485), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		escButtonRect.DOAnchorPos(new Vector2(-904.2f, -485), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		players.GetComponent<Button>().Select();

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

		footballButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		hitButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 

		Tween myTween = crushButtonRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton - 131), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		yield return myTween.WaitForCompletion();

		modesTeam [0].SetActive (true);
		modesTeam [1].SetActive (false);
		modesTeam [2].SetActive (false);

		modesDescription [0].SetActive (true);
		modesDescription [1].SetActive (false);
		modesDescription [2].SetActive (false);

		chooseTeamContent.anchoredPosition = new Vector2(offScreenX, 0);
		playButton.anchoredPosition = new Vector2(playButton.anchoredPosition.x, playButtonMinY);
		play.GetComponent<Button> ().interactable = false;


		chooseTeamCanvas.SetActive(true);
		crushMenuCanvas.SetActive(true);
		chooseModeCanvas.SetActive(false);

		myTween = chooseTeamContent.DOAnchorPos(new Vector2(0, 0), durationContent).SetEase(easeTypeMainMenu).OnComplete(NotTweening);
		yield return myTween.WaitForCompletion();

		//playButton.DOAnchorPos (new Vector2(playButton.anchoredPosition.x, playButtonMaxY), playButtonDuration).SetEase(easeTypeMainMenu).OnComplete(NotTweening);
		play.GetComponent<Button> ().Select ();
	}

	public IEnumerator ExitCrush ()
	{
		playButton.DOAnchorPos (new Vector2(playButton.anchoredPosition.x, playButtonMinY), playButtonDuration).SetEase(easeTypeMainMenu);
		Tween myTween = chooseTeamContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);
		yield return myTween.WaitForCompletion();

		chooseTeamCanvas.SetActive(false);
		crushMenuCanvas.SetActive(false);
		chooseModeCanvas.SetActive(true);

		crushButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		footballButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(NotTweening); 
		hitButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 

		crush.GetComponent<Button>().Select();

		PlayReturnSound ();
	}

	public void LoadFootballVoid()
	{
		if(!tweening)
		{
			StartCoroutine(LoadFootball ());
			Tweening ();
		}
	}

	IEnumerator LoadFootball ()
	{
		LoadModeManager.Instance.LoadSceneVoid ("Football");

		//Reset Top Button Position
		footballMenuCanvas.transform.GetChild (0).GetComponent<RectTransform> ().anchoredPosition = (new Vector2 (onScreenX, topYpositionButton));
		footballMenuCanvas.transform.GetChild (1).GetChild(0).GetComponent<RectTransform> ().anchoredPosition = (new Vector2 (onScreenX, topYpositionButton - 131));

		crushButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		hitButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 

		Tween myTween = footballButtonRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton - 131), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		yield return myTween.WaitForCompletion();

		modesTeam [0].SetActive (false);
		modesTeam [1].SetActive (true);
		modesTeam [2].SetActive (false);

		modesDescription [0].SetActive (false);
		modesDescription [1].SetActive (true);
		modesDescription [2].SetActive (false);

		chooseTeamContent.anchoredPosition = new Vector2(offScreenX, 0);
		playButton.anchoredPosition = new Vector2(playButton.anchoredPosition.x, playButtonMinY);
		play.GetComponent<Button> ().interactable = false;


		chooseTeamCanvas.SetActive(true);
		footballMenuCanvas.SetActive(true);
		chooseModeCanvas.SetActive(false);

		myTween = chooseTeamContent.DOAnchorPos(new Vector2(0, 0), durationContent).SetEase(easeTypeMainMenu).OnComplete(NotTweening);
		yield return myTween.WaitForCompletion();

		//playButton.DOAnchorPos (new Vector2(playButton.anchoredPosition.x, playButtonMaxY), playButtonDuration).SetEase(easeTypeMainMenu).OnComplete(NotTweening);
		play.GetComponent<Button> ().Select ();
	}

	public IEnumerator ExitFootball ()
	{
		Tween myTween = playButton.DOAnchorPos (new Vector2(playButton.anchoredPosition.x, playButtonMinY), playButtonDuration).SetEase(easeTypeMainMenu);
		chooseTeamContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);
		yield return myTween.WaitForCompletion();

		chooseTeamCanvas.SetActive(false);
		footballMenuCanvas.SetActive(false);
		chooseModeCanvas.SetActive(true);

		footballButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 

		crushButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(NotTweening);
		hitButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 

		crush.GetComponent<Button>().Select();

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

		crushButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 
		footballButtonRect.DOAnchorPos(new Vector2(offScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 

		Tween myTween = hitButtonRect.DOAnchorPos(new Vector2(onScreenX, topYpositionButton - 131), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		yield return myTween.WaitForCompletion();

		modesTeam [0].SetActive (true);
		modesTeam [1].SetActive (false);
		modesTeam [2].SetActive (false);

		modesDescription [0].SetActive (false);
		modesDescription [1].SetActive (false);
		modesDescription [2].SetActive (true);

		chooseTeamContent.anchoredPosition = new Vector2(offScreenX, 0);
		playButton.anchoredPosition = new Vector2(playButton.anchoredPosition.x, playButtonMinY);
		play.GetComponent<Button> ().interactable = false;


		chooseTeamCanvas.SetActive(true);
		hitMenuCanvas.SetActive(true);
		chooseModeCanvas.SetActive(false);

		myTween = chooseTeamContent.DOAnchorPos(new Vector2(0, 0), durationContent).SetEase(easeTypeMainMenu).OnComplete(NotTweening);
		yield return myTween.WaitForCompletion();

		//playButton.DOAnchorPos (new Vector2(playButton.anchoredPosition.x, playButtonMaxY), playButtonDuration).SetEase(easeTypeMainMenu).OnComplete(NotTweening);
		play.GetComponent<Button> ().Select ();
	}

	public IEnumerator ExitHit ()
	{
		playButton.DOAnchorPos (new Vector2(playButton.anchoredPosition.x, playButtonMinY), playButtonDuration).SetEase(easeTypeMainMenu);
		Tween myTween = chooseTeamContent.DOAnchorPos(new Vector2(offScreenX, 0), durationContent).SetEase(easeTypeMainMenu);
		yield return myTween.WaitForCompletion();

		chooseTeamCanvas.SetActive(false);
		hitMenuCanvas.SetActive(false);
		chooseModeCanvas.SetActive(true);

		hitButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [5]), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 

		crushButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [3]), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete(NotTweening);
		footballButtonRect.DOAnchorPos(new Vector2(onScreenX, yPositions [4]), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween"); 

		crush.GetComponent<Button>().Select();

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
	
		else if(startRect.anchoredPosition.y == 700)
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

		if(smallLogo.anchoredPosition.y == 400)
			smallLogo.DOAnchorPos(new Vector2(0, 0), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");


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

		gameOverCanvas.SetActive (true);

		gameOverButton.anchoredPosition = new Vector2 (gameOverButton.anchoredPosition.x, 700);
		gameOverButton.DOAnchorPos (new Vector2(gameOverButton.anchoredPosition.x, topYpositionButton), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		restartButton.anchoredPosition = new Vector2 (restartButton.anchoredPosition.x, -700);
		menuButton.anchoredPosition = new Vector2 (menuButton.anchoredPosition.x, -700);
		restartButton.DOAnchorPos (new Vector2(restartButton.anchoredPosition.x, bottomYPosition), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		menuButton.DOAnchorPos (new Vector2(menuButton.anchoredPosition.x, bottomYPosition), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		gameOverContent.anchoredPosition = new Vector2 (-offScreenX, gameOverContent.anchoredPosition.y);
		gameOverContent.DOAnchorPos (new Vector2(-onScreenX, gameOverContent.anchoredPosition.y), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete (NotTweening);

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
		gameOverContent.DOAnchorPos (new Vector2(-offScreenX, gameOverContent.anchoredPosition.y), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete (NotTweening);

		menuButton.DOAnchorPos (new Vector2(menuButton.anchoredPosition.x, -700), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		restartButton.DOAnchorPos (new Vector2(restartButton.anchoredPosition.x, -700), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		gameOverButton.DOAnchorPos (new Vector2(gameOverButton.anchoredPosition.x, 700), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

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
		gameOverContent.DOAnchorPos (new Vector2(-offScreenX, gameOverContent.anchoredPosition.y), durationSubmit).SetDelay(delaySubmit[0]).SetEase(easeTypeMainMenu).SetId("MainMenuTween").OnComplete (NotTweening);

		menuButton.DOAnchorPos (new Vector2(menuButton.anchoredPosition.x, -700), durationSubmit).SetDelay(delaySubmit[1]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		restartButton.DOAnchorPos (new Vector2(restartButton.anchoredPosition.x, -700), durationSubmit).SetDelay(delaySubmit[2]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");

		Tween myTween = gameOverButton.DOAnchorPos (new Vector2(gameOverButton.anchoredPosition.x, 700), durationSubmit).SetDelay(delaySubmit[3]).SetEase(easeTypeMainMenu).SetId("MainMenuTween");
		yield return myTween.WaitForCompletion ();

		gameOverCanvas.SetActive (false);

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