using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rewired;
using UnityEngine.SceneManagement;

public enum GameStateEnum {Playing, Paused, Over};

public enum WhichMode {Default, Repulse, Bomb, Hit, Crush, Football, Wrap, PushOut, Training};

public class GlobalVariables : Singleton<GlobalVariables>
{
	public event EventHandler OnGameOver;
	public event EventHandler OnModeStarted;
	public event EventHandler OnModeEnded;
	public event EventHandler OnPlaying;
	public event EventHandler OnPause;
	public event EventHandler OnResume;
	public event EventHandler OnMainMenu;

	[Header ("Game State")]
	public GameStateEnum GameState = GameStateEnum.Over;
	public bool FirstGameLaunch = true;

	[Header ("Scenes")]
	public string firstSceneToLoad = "Crush";
	public WhichMode WhichModeLoaded;
	public string CurrentModeLoaded = "";

	public bool Stun = false;

	[Header ("Controller Numbers")]
	public int ControllerNumberPlayer1 = -1;
	public int ControllerNumberPlayer2 = -1;
	public int ControllerNumberPlayer3 = -1;
	public int ControllerNumberPlayer4 = -1;

	[Header ("Players")]
	public GameObject Player1;
	public GameObject Player2;
	public GameObject Player3;
	public GameObject Player4;
	public List<GameObject> EnabledPlayersList = new List<GameObject>();

	[Header ("Players States")]
	public int NumberOfPlayers;
	public int NumberOfDisabledPlayers;

	[Header ("Cubes Stripes")]
	public ControllerChangeManager1 controllerManager;
	public Texture2D[] mouseCursor = new Texture2D[4];

	[Header ("Cubes Color")]
	public Color cubeColorplayer1;
	public Color cubeColorplayer2;
	public Color cubeColorplayer3;
	public Color cubeColorplayer4;
	public Color cubeNeutralColor;

	[Header ("Cubes Stripes")]
	public Mesh[] cubesStripes = new Mesh[4];

	[Header ("FX Prefabs")]
	public GameObject[] shootFX = new GameObject[4];
	public GameObject[] explosionFX = new GameObject[4];
	public GameObject[] attractFX = new GameObject[4];
	public GameObject[] repulseFX = new GameObject[4];
	public GameObject[] wallImpactFX = new GameObject[5];

	[Header ("Particles Prefab")]
	public GameObject HitParticles;
	public GameObject WallHitParticles;
	public GameObject DeadParticles;
	public GameObject MovableExplosion;
	public GameObject PlayerSpawnParticles;
	public Transform ParticulesClonesParent;

	[Header ("Buttons Color")]
	public Color mainButtonIdleColorText;
	public Color mainButtonHighlightedColorText;
	public Color mainButtonClickedColorText;
	public Color secondaryButtonIdleColorText;
	public Color secondaryButtonHighlightedColorText;
	public Color secondaryClickedColorText;

	void Start ()
	{
		if(SceneManager.GetActiveScene().name == "Scene Testing")
			GlobalVariables.Instance.GameState = GameStateEnum.Playing;
		
		ParticulesClonesParent = GameObject.FindGameObjectWithTag ("ParticulesClonesParent").transform;

		StartCoroutine (OnGameOverEvent ());
		StartCoroutine (OnPlayingEvent ());
		StartCoroutine (OnResumeEvent ());
		StartCoroutine (OnPauseEvent ());
		StartCoroutine (OnModeStartedEvent ());
		StartCoroutine (OnModeEndedEvent ());

		OnPlaying += HideMouseCursor;
	}
		
	void Update ()
	{
		if(Stun)
		{
			Stun = false;
			Player2.GetComponent<PlayersGameplay> ().StunVoid (false);
		}

		Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

		if (GameState != GameStateEnum.Playing && mouseMovement.magnitude > 1 && Cursor.visible == false)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	public void SetPlayersControllerNumbers ()
	{
		Player1.GetComponent<PlayersGameplay> ().controllerNumber = ControllerNumberPlayer1;
		Player2.GetComponent<PlayersGameplay> ().controllerNumber = ControllerNumberPlayer2;
		Player3.GetComponent<PlayersGameplay> ().controllerNumber = ControllerNumberPlayer3;
		Player4.GetComponent<PlayersGameplay> ().controllerNumber = ControllerNumberPlayer4;
	}

	public void ListPlayers ()
	{
		EnabledPlayersList.Clear ();

		if (ControllerNumberPlayer1 != -1 && !EnabledPlayersList.Contains (Player1))
			EnabledPlayersList.Add (Player1);

		if (ControllerNumberPlayer2 != -1 && !EnabledPlayersList.Contains (Player2))
			EnabledPlayersList.Add (Player2);

		if (ControllerNumberPlayer3 != -1 && !EnabledPlayersList.Contains (Player3))
			EnabledPlayersList.Add (Player3);

		if (ControllerNumberPlayer4 != -1 && !EnabledPlayersList.Contains (Player4))
			EnabledPlayersList.Add (Player4);


		if (ControllerNumberPlayer1 == -1 && EnabledPlayersList.Contains (Player1))
			EnabledPlayersList.Remove (Player1);

		if (ControllerNumberPlayer2 == -1 && EnabledPlayersList.Contains (Player2))
			EnabledPlayersList.Remove (Player2);

		if (ControllerNumberPlayer3 == -1 && EnabledPlayersList.Contains (Player3))
			EnabledPlayersList.Remove (Player3);

		if (ControllerNumberPlayer4 == -1 && EnabledPlayersList.Contains (Player4))
			EnabledPlayersList.Remove (Player4);

		PlayersNumber ();
	}

	void PlayersNumber ()
	{
		NumberOfPlayers = EnabledPlayersList.Count;
		NumberOfDisabledPlayers = 4 - NumberOfPlayers;
	}

	public void SetPlayerMouseCursor ()
	{

		if (ControllerNumberPlayer1 == 0)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			Cursor.SetCursor (mouseCursor[0], Vector2.zero, CursorMode.Auto);
		}

		if (ControllerNumberPlayer2 == 0)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			Cursor.SetCursor (mouseCursor[1], Vector2.zero, CursorMode.Auto);
		}

		if (ControllerNumberPlayer3 == 0)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			Cursor.SetCursor (mouseCursor[2], Vector2.zero, CursorMode.Auto);
		}

		if (ControllerNumberPlayer4 == 0)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			Cursor.SetCursor (mouseCursor[3], Vector2.zero, CursorMode.Auto);
		}
	}

	void HideMouseCursor ()
	{
		if(ControllerNumberPlayer1 != 0 && ControllerNumberPlayer2 != 0 && ControllerNumberPlayer3 != 0 && ControllerNumberPlayer4 != 0)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	public void SetWhichModeEnum ()
	{
		switch(CurrentModeLoaded)
		{
		default:
			WhichModeLoaded = WhichMode.Default;
			break;
		case "Repulse":
			WhichModeLoaded = WhichMode.Repulse;
			break;
		case "Bomb":
			WhichModeLoaded = WhichMode.Bomb;
			break;
		case "Hit":
			WhichModeLoaded = WhichMode.Hit;
			break;
		case "Crush":
			WhichModeLoaded = WhichMode.Crush;
			break;
		case "Football":
			WhichModeLoaded = WhichMode.Football;
			break;
		case "Wrap":
			WhichModeLoaded = WhichMode.Wrap;
			break;
		case "PushOut":
			WhichModeLoaded = WhichMode.PushOut;
			break;
		case "Training":
			WhichModeLoaded = WhichMode.Training;
			break;
		}

		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<DynamicCamera> ().GetNewSettings ();
	}

	IEnumerator OnGameOverEvent ()
	{
		yield return new WaitUntil (() => GameState != GameStateEnum.Over);

		yield return new WaitUntil (() => GameState == GameStateEnum.Over);

		if (OnGameOver != null)
			OnGameOver ();

		yield return null;

		StartCoroutine (OnGameOverEvent ());
	}

	IEnumerator OnModeEndedEvent ()
	{
		yield return new WaitUntil (() => GameState == GameStateEnum.Playing);

		yield return new WaitUntil (() => GameState == GameStateEnum.Over);

		if (OnModeEnded != null)
			OnModeEnded ();

		yield return null;

		StartCoroutine (OnModeEndedEvent ());
	}

	IEnumerator OnPlayingEvent ()
	{
		yield return new WaitUntil (() => GameState != GameStateEnum.Playing);

		yield return new WaitUntil (() => GameState == GameStateEnum.Playing);

		if (OnPlaying != null)
			OnPlaying ();

		yield return null;

		StartCoroutine (OnPlayingEvent ());
	}

	IEnumerator OnPauseEvent ()
	{
		yield return new WaitUntil (() => GameState == GameStateEnum.Playing);

		yield return new WaitUntil (() => GameState == GameStateEnum.Paused);

		if (OnPause != null)
			OnPause ();

		yield return null;

		StartCoroutine (OnPauseEvent ());
	}

	IEnumerator OnResumeEvent ()
	{
		yield return new WaitUntil (() => GameState == GameStateEnum.Paused);

		yield return new WaitUntil (() => GameState == GameStateEnum.Playing);

		if (OnResume != null)
			OnResume ();

		yield return null;

		StartCoroutine (OnResumeEvent ());
	}

	IEnumerator OnModeStartedEvent ()
	{
		yield return new WaitUntil (() => GameState == GameStateEnum.Over);

		yield return new WaitUntil (() => GameState == GameStateEnum.Playing);

		if (OnModeStarted != null)
			OnModeStarted ();

		yield return null;

		StartCoroutine (OnModeStartedEvent ());
	}

	public void OnMainMenuVoid ()
	{
		if (OnMainMenu != null)
			OnMainMenu ();
	}
}
