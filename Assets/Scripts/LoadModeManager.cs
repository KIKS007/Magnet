using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LoadModeManager : Singleton<LoadModeManager> 
{
	public event EventHandler OnLevelLoaded;

	[Header ("Load Mode Manager")]
	public GameObject[] rootGameObjects;

	public float loadingX = -140;
	public float reloadingX = 140;
	public float movementDuration = 0.25f;
	public Ease movementEase = Ease.InOutCubic;

	private Transform mainCamera;

	// Use this for initialization
	void Awake () 
	{
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera").transform;

		if (SceneManager.GetActiveScene ().name != "Scene Testing")
			StartCoroutine (FirstLoadedScene (GlobalVariables.Instance.firstSceneToLoad));
	}

	void Start ()
	{
		if (SceneManager.GetActiveScene ().name == "Scene Testing")
			FindGameObjects ();
		
	}

	//Game First Scene Loaded
	IEnumerator FirstLoadedScene (string sceneToLoad)
	{
		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			if(SceneManager.GetSceneAt(i).name != "Scene Testing" && SceneManager.GetSceneAt(i).name != "Menu")
			{
				string name = SceneManager.GetSceneAt (i).name;
				yield return SceneManager.UnloadSceneAsync (name);
			}
		}

		if(SceneManager.GetSceneByName(sceneToLoad).isLoaded)
		{
			yield return SceneManager.UnloadSceneAsync (sceneToLoad);
		}

		yield return SceneManager.LoadSceneAsync (sceneToLoad, LoadSceneMode.Additive);

		rootGameObjects = SceneManager.GetSceneByName (sceneToLoad).GetRootGameObjects ();
		FindGameObjects ();

		mainCamera.GetComponent<SlowMotionCamera> ().StopEndGameSlowMotion ();

		GlobalVariables.Instance.CurrentModeLoaded = sceneToLoad;
		GlobalVariables.Instance.SetWhichModeEnum ();

		StatsManager.Instance.ResetStats (true);

		if (OnLevelLoaded != null)
			OnLevelLoaded ();
	}

	public void LoadSceneVoid (string sceneToLoad)
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Paused)
			mainCamera.GetComponent<SlowMotionCamera> ().StopPauseSlowMotion ();
		else
			mainCamera.GetComponent<SlowMotionCamera> ().StopEndGameSlowMotion ();

		if(GlobalVariables.Instance.CurrentModeLoaded != sceneToLoad)
		{
			StatsManager.Instance.ResetStats (true);

			StartCoroutine (LoadScene (sceneToLoad));
		}
		
		else if(GlobalVariables.Instance.CurrentModeLoaded == sceneToLoad && GlobalVariables.Instance.GameState == GameStateEnum.Paused)
		{
			StatsManager.Instance.ResetStats (true);

			GlobalVariables.Instance.GameState = GameStateEnum.Menu;
			StartCoroutine (LoadScene (sceneToLoad));
		}

	}

	//Menu Load Scene to choose mode
	IEnumerator LoadScene (string sceneToLoad)
	{
		float orginalPosition = mainCamera.transform.position.x;

		Tween myTween = mainCamera.DOMoveX (loadingX, movementDuration).SetEase(movementEase);
		yield return myTween.WaitForCompletion ();

		DestroyParticules ();

		if (GlobalVariables.Instance.CurrentModeLoaded != "")
			yield return SceneManager.UnloadSceneAsync (GlobalVariables.Instance.CurrentModeLoaded);

		yield return SceneManager.LoadSceneAsync (sceneToLoad, LoadSceneMode.Additive);


		rootGameObjects = SceneManager.GetSceneByName (sceneToLoad).GetRootGameObjects ();

		FindGameObjects ();

		mainCamera.DOMoveX (orginalPosition, movementDuration).SetEase(movementEase);

		GlobalVariables.Instance.CurrentModeLoaded = sceneToLoad;
		GlobalVariables.Instance.SetWhichModeEnum ();
		GlobalVariables.Instance.GameState = GameStateEnum.Menu;

		if (OnLevelLoaded != null)
			OnLevelLoaded ();
	}

	public void RestartSceneVoid ()
	{
		StartCoroutine (RestartScene ());
	}

	IEnumerator RestartScene ()
	{
		string sceneToLoad = GlobalVariables.Instance.CurrentModeLoaded;

		Tween myTween = mainCamera.DOMoveX (reloadingX, movementDuration).SetEase(movementEase);
		yield return myTween.WaitForCompletion ();

		StatsManager.Instance.ResetStats (false);

		DestroyParticules ();

		if (GlobalVariables.Instance.CurrentModeLoaded != "")
			yield return SceneManager.UnloadSceneAsync (GlobalVariables.Instance.CurrentModeLoaded);

		GlobalVariables.Instance.CurrentModeLoaded = sceneToLoad;
		GlobalVariables.Instance.SetWhichModeEnum ();

		yield return SceneManager.LoadSceneAsync (sceneToLoad, LoadSceneMode.Additive);

		myTween = mainCamera.DOMoveX (0, movementDuration).SetEase(movementEase);

		rootGameObjects = SceneManager.GetSceneByName (GlobalVariables.Instance.CurrentModeLoaded).GetRootGameObjects ();
		FindGameObjects ();
	
		yield return myTween.WaitForCompletion ();

		mainCamera.GetComponent<SlowMotionCamera> ().StopEndGameSlowMotion ();

		GlobalVariables.Instance.GameState = GameStateEnum.Playing;

		if (OnLevelLoaded != null)
			OnLevelLoaded ();
	}

	public void ReloadSceneVoid ()
	{
		StartCoroutine (ReloadScene ());
	}

	IEnumerator ReloadScene ()
	{
		float orginalPosition = mainCamera.transform.position.x;
		string sceneToLoad = GlobalVariables.Instance.CurrentModeLoaded;

		Tween myTween = mainCamera.DOMoveX (loadingX, movementDuration).SetEase(movementEase);
		yield return myTween.WaitForCompletion ();

		StatsManager.Instance.ResetStats (true);

		DestroyParticules ();

		if (GlobalVariables.Instance.CurrentModeLoaded != "")
			yield return SceneManager.UnloadSceneAsync (GlobalVariables.Instance.CurrentModeLoaded);


		GlobalVariables.Instance.CurrentModeLoaded = sceneToLoad;
		GlobalVariables.Instance.SetWhichModeEnum ();

		mainCamera.GetComponent<SlowMotionCamera> ().StopEndGameSlowMotion ();

		yield return SceneManager.LoadSceneAsync (sceneToLoad, LoadSceneMode.Additive);

		mainCamera.DOMoveX (orginalPosition, movementDuration).SetEase(movementEase);

		rootGameObjects = SceneManager.GetSceneByName (GlobalVariables.Instance.CurrentModeLoaded).GetRootGameObjects ();

		FindGameObjects ();

		if (OnLevelLoaded != null)
			OnLevelLoaded ();
	}



	void DestroyParticules ()
	{
		if(GlobalVariables.Instance.ParticulesClonesParent.childCount != 0)
		{
			for(int i = 0; i < GlobalVariables.Instance.ParticulesClonesParent.childCount; i++)
			{
				Destroy (GlobalVariables.Instance.ParticulesClonesParent.GetChild (i).gameObject);
			}
		}

		GameObject[] particlesFX = GameObject.FindGameObjectsWithTag ("Particles_FX");

		for(int i = 0; i < particlesFX.Length; i++)
		{
			Destroy (particlesFX [i]);
		}
	}

	void FindGameObjects ()
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");

		for(int i = 0; i < players.Length; i++)
		{
			if (players [i].GetComponent <PlayersGameplay> ().playerName == PlayerName.Player1)
				GlobalVariables.Instance.Players [0] = players [i];

			if (players [i].GetComponent <PlayersGameplay>().playerName == PlayerName.Player2)
				GlobalVariables.Instance.Players [1] = players [i];

			if (players [i].GetComponent <PlayersGameplay>().playerName == PlayerName.Player3)
				GlobalVariables.Instance.Players [2] = players [i];

			if (players [i].GetComponent <PlayersGameplay>().playerName == PlayerName.Player4)
				GlobalVariables.Instance.Players [3] = players [i];
		}

		mainCamera.GetComponent<SlowMotionCamera> ().mirrorScript = GameObject.FindGameObjectWithTag("Environment").transform.GetComponentInChildren<MirrorReflection>();

		UpdateGlobalVariables ();
	}

	void UpdateGlobalVariables ()
	{
		StatsManager.Instance.GetPlayersEvents ();

		GlobalVariables.Instance.SetPlayersControllerNumbers ();
		GlobalVariables.Instance.ListPlayers ();
	}
		
}
