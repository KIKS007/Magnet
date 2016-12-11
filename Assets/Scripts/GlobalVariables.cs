using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rewired;
using UnityEngine.SceneManagement;

public enum GameStateEnum {Menu, Playing, Paused, EndMode};

public enum WhichMode {Bomb, Crush, Training, Ram, Flow, Tag, Banner, Plague, Freeze, Countdown, Standoff, Default};

public class GlobalVariables : Singleton<GlobalVariables>
{
	[Header ("Game State")]
	public GameStateEnum GameState = GameStateEnum.Menu;
	public bool FirstGameLaunch = true;
	public ControllerChangeManager controllerManager;

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
	public List<GameObject> AlivePlayersList = new List<GameObject>();

	[Header ("Players Count")]
	public int NumberOfPlayers;
	public int NumberOfDisabledPlayers;
	public int NumberOfAlivePlayers;
	public int NumberOfDeadPlayers;

	[Header ("Mouse Cursor")]
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
		{
			GameState = GameStateEnum.Playing;
			CurrentModeLoaded = "Scene Testing";
			SetWhichModeEnum ();
		}
		
		ParticulesClonesParent = GameObject.FindGameObjectWithTag ("ParticulesClonesParent").transform;

		StartCoroutine (OnEndModeEvent ());
		StartCoroutine (OnStartModeEvent ());
		StartCoroutine (OnRestartModeEvent ());
		StartCoroutine (OnPlayingEvent ());
		StartCoroutine (OnPauseEvent ());
		StartCoroutine (OnResumeEvent ());
		StartCoroutine (OnMenuEvent ());

		OnPlaying += ()=> HideMouseCursor();
		OnRestartMode += ()=> SetPlayerMouseCursor();
	}
		
	void Update ()
	{
		Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

		if (GameState != GameStateEnum.Playing && Cursor.visible == false && mouseMovement.magnitude > 1)
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

		AlivePlayersNumber ();
	}

	void PlayersNumber ()
	{
		NumberOfPlayers = EnabledPlayersList.Count;
		NumberOfDisabledPlayers = 4 - NumberOfPlayers;
	}

	void AlivePlayersNumber ()
	{
		AlivePlayersList.Clear ();

		for (int i = 0; i < Players.Length; i++)
		{
			if (Players [i] != null && Players [i].activeSelf == true)
				AlivePlayersList.Add (Players [i]);
		}

		NumberOfAlivePlayers = AlivePlayersList.Count;
		NumberOfDeadPlayers = 4 - NumberOfAlivePlayers;
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

	public void HideMouseCursor (bool forcedHide = false)
	{
		if(forcedHide)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else
		{
			if(PlayersControllerNumber[0] != 0 && PlayersControllerNumber[1] != 0 && PlayersControllerNumber[2] != 0 && PlayersControllerNumber[3] != 0)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}			
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

	public event EventHandler OnEndMode;
	public event EventHandler OnStartMode;
	public event EventHandler OnRestartMode;
	public event EventHandler OnPlaying;
	public event EventHandler OnPause;
	public event EventHandler OnResume;
	public event EventHandler OnMenu;

	IEnumerator OnEndModeEvent ()
	{
		yield return new WaitUntil (() => GameState == GameStateEnum.Playing);

		yield return new WaitUntil (() => GameState == GameStateEnum.EndMode);

		if (OnEndMode != null)
			OnEndMode ();

		yield return null;

		StartCoroutine (OnEndModeEvent ());
	}

	IEnumerator OnStartModeEvent ()
	{
		yield return new WaitUntil (() => GameState == GameStateEnum.Menu);

		yield return new WaitUntil (() => GameState == GameStateEnum.Playing);

		if (OnStartMode != null)
			OnStartMode ();

		yield return null;

		StartCoroutine (OnStartModeEvent ());
	}

	IEnumerator OnRestartModeEvent ()
	{
		yield return new WaitUntil (() => GameState == GameStateEnum.EndMode);

		yield return new WaitUntil (() => GameState == GameStateEnum.Playing);

		if (OnRestartMode != null)
			OnRestartMode ();

		yield return null;

		StartCoroutine (OnRestartModeEvent ());
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

	IEnumerator OnMenuEvent ()
	{
		yield return new WaitUntil (() => GameState != GameStateEnum.Menu);

		yield return new WaitUntil (() => GameState == GameStateEnum.Menu);

		if (OnMenu != null)
			OnMenu ();

		yield return null;

		StartCoroutine (OnMenuEvent ());
	}
}
