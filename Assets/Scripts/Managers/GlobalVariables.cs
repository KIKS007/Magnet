using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rewired;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public enum GameStateEnum { Menu, Playing, Paused, EndMode, Loading };

public enum StartupType {Delayed, Wave, Done};

public enum ModeSequenceType {Selection, Random, Cocktail};

public enum ModeObjective {LastMan, LeastDeath};

public enum WhichMode {Bomb, Bounce, Burden, Crush, Flow, Plague, Pool, Ram, Standoff, Star, Tutorial, None, Default};

public class GlobalVariables : Singleton<GlobalVariables>
{
	[PropertyOrder (-1)]
	[Button] 
	void DeletePlayersPrefs ()
	{
		PlayerPrefs.DeleteAll ();
	}

	[Header ("Game State")]
	public GameStateEnum GameState = GameStateEnum.Menu;
	public bool FirstGameLaunch = true;

	[Header ("Modes")]
	public WhichMode firstSceneToLoad;
	public WhichMode CurrentModeLoaded;
	public List<WhichMode> lastPlayedModes = new List<WhichMode>();
	public List<WhichMode> selectedCocktailModes = new List<WhichMode>();
	public List<WhichMode> currentCocktailModes = new List<WhichMode>();
	public LastManManager lastManManager;

	[Header ("Mode Sequence")]
	public ModeSequenceType ModeSequenceType = ModeSequenceType.Selection;
	public int GamesCount = 1;
	public int CurrentGamesCount = 1;
	public int LivesCount = 5;

	[Header ("Startup")]
	public StartupType Startup = StartupType.Wave;
	public float delayedStartupDuration = 1f;
	public float delayBetweenPlayerWaves = 0.2f;
	public float delayBetweenWavesFX = 0.15f;

	[Header ("Controller Numbers")]
	public int[] PlayersControllerNumber = new int[4] {-1, -1, -1, -1};

	[Header ("Gamepads")]
	public List<PlayerGamepad> gamepadsList = new List<PlayerGamepad> ();
	public bool OneGamepadUnplugged = false;

	[Header ("Players")]
	public GameObject[] Players = new GameObject[4];
	public List<GameObject> EnabledPlayersList = new List<GameObject>();
	public List<GameObject> AlivePlayersList = new List<GameObject>();

	[Header ("AI")]
	public bool[] aiEnabled = new bool[4];
	public AILevel[] aiLevels = new AILevel[4];
	public List<AIPrefab> aiPrefabs = new List<AIPrefab> ();

	[Header ("Players Count")]
	public int NumberOfPlayers;
	public int NumberOfAlivePlayers;
	public int NumberOfDeadPlayers;
	public int NumberOfBots;

	[Header ("Players Colors")]
	public Color[] playersColors = new Color[5];
	public Material[] playersMaterials = new Material[4];
	[PropertyOrder (-1)]
	[Button] 
	void UpdateAllColors ()
	{
		foreach (var p in FindObjectsOfType<MenuPlayerColor> ())
			p.Setup ();
	}

	[Header ("Movables")]
	public List<GameObject> AllMovables = new List<GameObject> ();

	[Header ("Mouse Cursor")]
	public Texture2D[] mouseCursor = new Texture2D[4];

	[Header ("Cubes")]
	public GameObject[] cubesPrefabs = new GameObject[3];
	public Mesh[] cubesStripes = new Mesh[4];

	[Header ("Players Dead Cubes")]
	public GameObject[] deadCubesPrefabs = new GameObject[3];
	public Mesh deadCubesMeshFilter;

	[Header ("FX Prefabs")]
	public GameObject[] shootFX = new GameObject[4];
	public GameObject[] explosionFX = new GameObject[4];
	public GameObject[] attractFX = new GameObject[4];
	public GameObject[] repulseFX = new GameObject[4];
	public GameObject[] wallImpactFX = new GameObject[5];
	public GameObject[] waveFX = new GameObject[4];
	public GameObject[] reincarnationFX = new GameObject[4];

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

	public Player[] rewiredPlayers = new Player[5];

	public GameObject mainCamera;
	public ScreenShakeCamera screenShakeCamera;
	public ZoomCamera zoomCamera;
	public DynamicCamera dynamicCamera;
	public MenuCameraMovement menuCameraMovement;

	void Awake ()
	{
		StartCoroutine (OnGameStateChange (GameState));
		StartCoroutine (OnStartupDoneEvent ());
		StartCoroutine (OnCocktailModes (selectedCocktailModes.Count));
		StartCoroutine (OnSequenceChangement (ModeSequenceType));

		SetupRewiredPlayers ();

		ReInput.ControllerConnectedEvent += (ControllerStatusChangedEventArgs obj) => UpdateGamepadList ();
		ReInput.ControllerDisconnectedEvent += (ControllerStatusChangedEventArgs obj) => UpdateGamepadList ();

		//OnPlaying += UpdateGamepadList;
		OnMenu += UpdateGamepadList;

		LoadModeManager.Instance.OnLevelUnloaded += UpdateGamepadList;

		OnPlaying += ()=> SetMouseVisibility();
		OnPlaying += UpdatePlayedModes;
		OnRestartMode += ()=> SetPlayerMouseCursor();
		OnMenu += () => Startup = StartupType.Wave;
		OnEndMode += ()=> Startup = StartupType.Delayed;

		mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		screenShakeCamera = mainCamera.GetComponent<ScreenShakeCamera>();
		zoomCamera = mainCamera.GetComponent<ZoomCamera>();
		dynamicCamera = mainCamera.GetComponent<DynamicCamera>();
		menuCameraMovement = mainCamera.GetComponent<MenuCameraMovement>();
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

	public void LevelWasLoaded (WhichMode levelLoaded, GameStateEnum gameState)
	{
		GetPlayers ();
		SetModeLimits ();
		GetMovables ();

		if(gameState == GameStateEnum.Playing)
			CreateAIs ();

		if (levelLoaded != WhichMode.Tutorial)
			lastManManager = GameObject.FindGameObjectWithTag("LastManManager").GetComponent<LastManManager> ();

		CurrentModeLoaded = levelLoaded;
		GameState = gameState;
	}

	public void LevelWasUnloaded (GameStateEnum gameState)
	{
		CurrentModeLoaded = WhichMode.None;
		GameState = gameState;
	}

	public void LivesCountChange (int lives)
	{
		LivesCount = lives;

//		foreach (GameObject g in Players)
//			g.GetComponent<PlayersGameplay> ().livesCount = 0;
//
//		foreach (GameObject g in EnabledPlayersList)
//			g.GetComponent<PlayersGameplay> ().livesCount = LivesCount;
//
//		ListPlayers ();

		if (OnLivesCountChange != null)
			OnLivesCountChange ();
	}

	void GetMovables ()
	{
		AllMovables.Clear ();

		if(GameObject.FindGameObjectsWithTag ("Movable").Length != 0)
			foreach (GameObject movable in GameObject.FindGameObjectsWithTag ("Movable"))
				AllMovables.Add (movable);

		if(GameObject.FindGameObjectsWithTag ("Suggestible").Length != 0)
			foreach (GameObject movable in GameObject.FindGameObjectsWithTag ("Suggestible"))
				AllMovables.Add (movable);

		if(GameObject.FindGameObjectsWithTag ("DeadCube").Length != 0)
			foreach (GameObject movable in GameObject.FindGameObjectsWithTag ("DeadCube"))
				AllMovables.Add (movable);

		for (int i = 0; i < AllMovables.Count; i++)
			AllMovables [i].SetActive (false);
	}

	void GetPlayers ()
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");

		for(int i = 0; i < players.Length; i++)
		{
			switch (players [i].GetComponent <PlayersGameplay> ().playerName)
			{
			case PlayerName.Player1:
				Players [0] = players [i];
				break;
			case PlayerName.Player2:
				Players [1] = players [i];
				break;
			case PlayerName.Player3:
				Players [2] = players [i];
				break;
			case PlayerName.Player4:
				Players [3] = players [i];
				break;
			}
		}

		if (Players.Length == 0)
		{
			Debug.LogWarning ("No Players Found !");
			return;
		}

		foreach (GameObject player in Players)
			player.GetComponent <PlayersGameplay> ().SetupController ();

		StatsManager.Instance.GetPlayersEvents ();

		SetPlayersControllerNumbers ();
		ListPlayers ();
	}

	void SetModeLimits ()
	{
		GlobalMethods.Instance.SetLimits ();
	}

	public void SetPlayersControllerNumbers ()
	{
		for (int i = 0; i < Players.Length; i++)
			Players [i].GetComponent<PlayersGameplay> ().controllerNumber = PlayersControllerNumber [i];
	}

	public void SetupRewiredPlayers ()
	{
		for(int i = 0; i < 5; i++)
			rewiredPlayers [i] = ReInput.players.GetPlayer (i);

		if (SceneManager.GetActiveScene ().name == "Scene Testing")
		{
			GlobalVariables.Instance.PlayersControllerNumber [0] = 0;

			if(ReInput.controllers.joystickCount != 0)
			{
				for(int i = 0; i < ReInput.controllers.joystickCount; i++)
				{
					if (i == 3)
						break;
					
					GlobalVariables.Instance.PlayersControllerNumber [i + 1] = i + 1;
				}
			}
			else
				GlobalVariables.Instance.PlayersControllerNumber [1] = 1;

		}
	}

	public void ListPlayers ()
	{
		//ENABLED PLAYERS
		EnabledPlayersList.Clear ();
		
		for(int i = 0; i < Players.Length; i++)
		{
			if (Players [i] == null)
				continue;

			if (Players [i].GetComponent<PlayersGameplay> ().controllerNumber != -1 && !EnabledPlayersList.Contains (Players [i]))
				EnabledPlayersList.Add (Players [i]);
			
			if (Players [i].GetComponent<PlayersGameplay> ().controllerNumber == -1 && EnabledPlayersList.Contains (Players [i]))
				EnabledPlayersList.Remove (Players [i]);
		}

		NumberOfPlayers = EnabledPlayersList.Count;

		//ALIVE PLAYERS
		AlivePlayersList.Clear ();

		for(int i = 0; i < Players.Length; i++)
		{
			if (Players [i] != null && Players [i].GetComponent<PlayersGameplay> ().livesCount != 0)
				AlivePlayersList.Add (Players [i]);
		}

		NumberOfAlivePlayers = AlivePlayersList.Count;
		NumberOfDeadPlayers = 4 - NumberOfAlivePlayers;

		if (OnPlayerListed != null)
			OnPlayerListed ();
	}

	public void CreateAIs ()
	{
		GameObject aiPrefab = aiPrefabs [0].prefab;

		foreach(AIPrefab a in aiPrefabs)
			if(a.mode == CurrentModeLoaded)
			{
				aiPrefab = a.prefab;
				break;
			}

		for(int i = 0; i < aiEnabled.Length; i++)
		{
			if(aiEnabled [i])
			{
				GameObject aiClone = Instantiate (aiPrefab, Players [i].transform.position, Players [i].transform.rotation) as GameObject;

				SceneManager.MoveGameObjectToScene (aiClone, SceneManager.GetSceneAt (1));

				aiClone.GetComponent<AIGameplay> ().Setup ((PlayerName)i, aiLevels [i]);

				Players [i] = aiClone;
			}
		}
	}

	void UpdatePlayedModes ()
	{
		lastPlayedModes.Add (CurrentModeLoaded);

		if (lastPlayedModes.Count > 5)
			lastPlayedModes.RemoveAt (0);
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

	public void SetMouseVisibility (bool forcedHide = false)
	{
		if(forcedHide || PlayersControllerNumber[0] != 0 && PlayersControllerNumber[1] != 0 && PlayersControllerNumber[2] != 0 && PlayersControllerNumber[3] != 0)
		{
			if(Cursor.lockState != CursorLockMode.Locked)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}
		else
		{
			if(Cursor.lockState != CursorLockMode.None)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}
	}

	public void ChangeSequence (int modeSequence)
	{
		ModeSequenceType = (ModeSequenceType) modeSequence;
	}

	public void UpdateGamepadList ()
	{
		gamepadsList.Clear ();
		bool oneGamepadUnplugged = false;

		foreach(GameObject p in GlobalVariables.Instance.EnabledPlayersList)
		{
			if (p == null)
				continue;

			PlayersGameplay script = p.GetComponent<PlayersGameplay> ();

			if (script.GetType () == typeof(AIGameplay))
				continue;

			//Unplugged
			if(script.controllerNumber != 0 && script.controllerNumber != -1)
			{
				gamepadsList.Add (new PlayerGamepad ());
				gamepadsList [gamepadsList.Count - 1].PlayerName = script.playerName;

				if(script.rewiredPlayer.controllers.joystickCount == 0)
				{
					gamepadsList [gamepadsList.Count - 1].GamepadIsPlugged = false;
					oneGamepadUnplugged = true;
				}
				else
					gamepadsList [gamepadsList.Count - 1].GamepadIsPlugged = true;
			}
		}

		OneGamepadUnplugged = oneGamepadUnplugged;

		if (oneGamepadUnplugged && OnGamepadDisconnected != null)
			OnGamepadDisconnected ();
	}

	public event EventHandler OnGamepadDisconnected;

	public event EventHandler OnPlayerListed;

	public event EventHandler OnEndMode;
	public event EventHandler OnStartMode;
	public event EventHandler OnRestartMode;
	public event EventHandler OnPlaying;
	public event EventHandler OnPause;
	public event EventHandler OnResume;
	public event EventHandler OnMenu;

	public event EventHandler OnStartupDone;
	public event EventHandler OnLivesCountChange;
	public event EventHandler OnCocktailModesChange;
	public event EventHandler OnSequenceChange;

	IEnumerator OnGameStateChange (GameStateEnum state)
	{
		yield return new WaitUntil (() => GameState != state);

		switch(GameState)
		{
		case GameStateEnum.EndMode:
			if(state == GameStateEnum.Playing)
			if (OnEndMode != null)
				OnEndMode ();
			break;
		case GameStateEnum.Menu:
			if (OnMenu != null)
				OnMenu ();
			break;
		case GameStateEnum.Paused:
			if (OnPause != null)
				OnPause ();
			break;
		case GameStateEnum.Playing:
			if (OnPlaying != null)
				OnPlaying ();
			
			if(state == GameStateEnum.Menu)
			if (OnStartMode != null)
				OnStartMode ();
			
			if(state == GameStateEnum.EndMode)
			if (OnRestartMode != null)
				OnRestartMode ();
			
			if(state == GameStateEnum.Paused)
			if (OnResume != null)
				OnResume ();
			break;
		}

		StartCoroutine (OnGameStateChange (GameState));
	}

	IEnumerator OnCocktailModes (int count)
	{
		yield return new WaitUntil (() => selectedCocktailModes.Count != count);

		if (OnCocktailModesChange != null)
			OnCocktailModesChange ();

		StartCoroutine (OnCocktailModes (selectedCocktailModes.Count));
	}

	IEnumerator OnStartupDoneEvent ()
	{
		yield return new WaitUntil (() => Startup != StartupType.Done);

		yield return new WaitUntil (() => Startup == StartupType.Done);

		if (OnStartupDone != null)
			OnStartupDone ();

		StartCoroutine (OnStartupDoneEvent ());
	}

	IEnumerator OnSequenceChangement (ModeSequenceType sequence)
	{
		if (OnSequenceChange != null)
			OnSequenceChange ();

		yield return new WaitUntil (() => sequence != ModeSequenceType);

		StartCoroutine (OnSequenceChangement (ModeSequenceType));
	}
}

[System.Serializable]
public class PlayerGamepad
{
	public PlayerName PlayerName;
	public bool GamepadIsPlugged = false;
}

[System.Serializable]
public class AIPrefab
{
	public WhichMode mode;
	public GameObject prefab;
}
