using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rewired;
using UnityEngine.SceneManagement;

public enum GameStateEnum {Playing, Paused, Over};

public enum WhichMode {Bomb, Crush, Training, Default};

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
	[HideInInspector]
	public string CurrentModeLoaded = "";

	[Header ("Controller Numbers")]
	public int[] PlayersControllerNumber = new int[4] {-1, -1, -1, -1};

	[Header ("Players")]
	public GameObject[] Players = new GameObject[4];
	public List<GameObject> EnabledPlayersList = new List<GameObject>();

	[Header ("Players States")]
	public int NumberOfPlayers;
	public int NumberOfDisabledPlayers;

	[Header ("Cubes Stripes")]
	public ControllerChangeManager1 controllerManager;
	public Texture2D[] mouseCursor = new Texture2D[4];

	[Header ("Cubes Color")]
	public Color[] cubePlayersColor = new Color[5];

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
		Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

		if (GameState != GameStateEnum.Playing && mouseMovement.magnitude > 1 && Cursor.visible == false)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	public void SetPlayersControllerNumbers ()
	{
		for (int i = 0; i < Players.Length; i++)
			Players [i].GetComponent<PlayersGameplay> ().controllerNumber = PlayersControllerNumber [i];
	}

	public void ListPlayers ()
	{
		EnabledPlayersList.Clear ();

		for(int i = 0; i < Players.Length; i++)
		{
			if (PlayersControllerNumber [i] != -1 && !EnabledPlayersList.Contains (Players [i]))
				EnabledPlayersList.Add (Players [i]);

			if (PlayersControllerNumber[i] == -1 && EnabledPlayersList.Contains (Players [i]))
				EnabledPlayersList.Remove (Players [i]);
		}

		PlayersNumber ();
	}

	void PlayersNumber ()
	{
		NumberOfPlayers = EnabledPlayersList.Count;
		NumberOfDisabledPlayers = 4 - NumberOfPlayers;
	}

	public void SetPlayerMouseCursor ()
	{

		for(int i = 0; i < PlayersControllerNumber.Length; i++)
			if(PlayersControllerNumber[i] == 0)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				Cursor.SetCursor (mouseCursor[i], Vector2.zero, CursorMode.Auto);
			}
	}

	void HideMouseCursor ()
	{
		if(PlayersControllerNumber[0] != 0 && PlayersControllerNumber[1] != 0 && PlayersControllerNumber[2] != 0 && PlayersControllerNumber[3] != 0)
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
		case "Bomb":
			WhichModeLoaded = WhichMode.Bomb;
			break;
		case "Crush":
			WhichModeLoaded = WhichMode.Crush;
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
