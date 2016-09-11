using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rewired;
using UnityEngine.SceneManagement;

public enum GameStateEnum {Playing, Paused, Over};

public enum WhichMode {Default, Repulse, Bomb, Hit, Crush, Football, Wrap, PushOut};

public class GlobalVariables : Singleton<GlobalVariables>
{
	public event EventHandler OnGameOver;
	public event EventHandler OnModeStarted;
	public event EventHandler OnPlaying;
	public event EventHandler OnPause;
	public event EventHandler OnResume;

	[Header ("Game State")]
	public GameStateEnum GameState = GameStateEnum.Over;
	//public bool GamePaused = true;
	//public bool GameOver = true;
	public bool FirstGameLaunch = true;

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

	public int[] TeamChoice = new int[] {-1, -1, -1, -1};

	public List<GameObject> Team1 = new List<GameObject>();
	public List<GameObject> Team2 = new List<GameObject>();
	public List<GameObject> Team3 = new List<GameObject>();
	public List<GameObject> Team4 = new List<GameObject>();

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

	[Header ("Particles Prefab")]
	public GameObject HitParticles;
	public GameObject WallHitParticles;
	public GameObject DeadParticles;
	public GameObject MovableExplosion;
	public GameObject PlayerSpawnParticles;
	public Transform ParticulesClonesParent;


	[Header ("Others")]
	public string firstSceneToLoad = "Crush";
	public WhichMode WhichModeLoaded;
	public string CurrentModeLoaded = "";

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
}
