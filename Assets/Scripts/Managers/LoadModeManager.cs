using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using DG.Tweening;

public enum LoadType { LoadMode, LoadMenu, Restart };

public class LoadModeManager : Singleton<LoadModeManager> 
{
	public event EventHandler OnLevelLoaded;

	[Header ("Movement")]

	public float loadingX = -150;
	public float reloadingX = 150;
	public float movementDuration = 0.25f;
	public Ease movementEase = Ease.InOutCubic;

	private Transform mainCamera;
	private SlowMotionCamera slowMo;
	private MenuCameraMovement cameraMovement;

	// Use this for initialization
	void Awake () 
	{
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera").transform;
		slowMo = mainCamera.GetComponent<SlowMotionCamera> ();
		cameraMovement = mainCamera.GetComponent<MenuCameraMovement> ();

		StartCoroutine (FirstLoadedScene (GlobalVariables.Instance.firstSceneToLoad));
	}

	//Game First Scene Loaded
	IEnumerator FirstLoadedScene (string sceneToLoad)
	{
		//Unload All other Scenes than Menu
		for (int i = 0; i < SceneManager.sceneCount; i++)
			if(SceneManager.GetSceneAt(i).name != "Scene Testing" && SceneManager.GetSceneAt(i).name != "Menu")
				yield return SceneManager.UnloadSceneAsync (SceneManager.GetSceneAt (i).name);

		//Unload Scene if already loaded
		if(SceneManager.GetSceneByName(sceneToLoad).isLoaded)
			yield return SceneManager.UnloadSceneAsync (sceneToLoad);

		StatsManager.Instance.ResetStats (true);
		UpdateGlobalVariables (sceneToLoad, GameStateEnum.Menu);

		yield return SceneManager.LoadSceneAsync (sceneToLoad, LoadSceneMode.Additive);

		if (OnLevelLoaded != null)
			OnLevelLoaded ();
	}

	public void LoadSceneVoid (string sceneToLoad)
	{
		if (GlobalVariables.Instance.CurrentModeLoaded == sceneToLoad && GlobalVariables.Instance.GameState == GameStateEnum.Menu)
			return;
		
		StartCoroutine (LoadScene (sceneToLoad));
	}

	//Menu Load Scene to choose mode
	IEnumerator LoadScene (string sceneToLoad)
	{
		yield return cameraMovement.StartCoroutine ("LoadingPosition");

		if (GlobalVariables.Instance.CurrentModeLoaded != "")
			yield return SceneManager.UnloadSceneAsync (GlobalVariables.Instance.CurrentModeLoaded);

		DestroyParticules ();
		StopSlowMotion ();
		StatsManager.Instance.ResetStats (true);
		UpdateGlobalVariables (sceneToLoad, GameStateEnum.Menu);

		yield return SceneManager.LoadSceneAsync (sceneToLoad, LoadSceneMode.Additive);

		if (OnLevelLoaded != null)
			OnLevelLoaded ();

		yield return cameraMovement.StartCoroutine ("LoadingPosition");
	}

	public void RestartSceneVoid (bool resetStats = true)
	{
		StartCoroutine (RestartScene (resetStats));
	}

	IEnumerator RestartScene (bool resetStats = true)
	{
		yield return cameraMovement.StartCoroutine ("RestartPosition");

		if (GlobalVariables.Instance.CurrentModeLoaded != "")
			yield return SceneManager.UnloadSceneAsync (GlobalVariables.Instance.CurrentModeLoaded);

		DestroyParticules ();
		StopSlowMotion ();

		if(resetStats)
			StatsManager.Instance.ResetStats (false);

		yield return SceneManager.LoadSceneAsync (GlobalVariables.Instance.CurrentModeLoaded, LoadSceneMode.Additive);

		if (OnLevelLoaded != null)
			OnLevelLoaded ();
		
		yield return cameraMovement.StartCoroutine ("RestartPosition");

		GlobalVariables.Instance.GameState = GameStateEnum.Playing;
	}

	public void ReloadSceneVoid ()
	{
		StartCoroutine (ReloadScene ());
	}

	IEnumerator ReloadScene ()
	{
		yield return cameraMovement.StartCoroutine ("LoadingPosition");

		if (GlobalVariables.Instance.CurrentModeLoaded != "")
			yield return SceneManager.UnloadSceneAsync (GlobalVariables.Instance.CurrentModeLoaded);

		DestroyParticules ();
		StopSlowMotion ();
		StatsManager.Instance.ResetStats (true);

		yield return SceneManager.LoadSceneAsync (GlobalVariables.Instance.CurrentModeLoaded, LoadSceneMode.Additive);

		if (OnLevelLoaded != null)
			OnLevelLoaded ();

		yield return cameraMovement.StartCoroutine ("LoadingPosition");
	}


	void StopSlowMotion ()
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Paused)
			slowMo.StopPauseSlowMotion ();
		else
			slowMo.StopEndGameSlowMotion ();
	}

	void UpdateGlobalVariables (string sceneToLoad)
	{
		GlobalVariables.Instance.CurrentModeLoaded = sceneToLoad;
		GlobalVariables.Instance.SetWhichModeEnum ();
	}

	void UpdateGlobalVariables (string sceneToLoad, GameStateEnum gameState)
	{
		GlobalVariables.Instance.CurrentModeLoaded = sceneToLoad;
		GlobalVariables.Instance.SetWhichModeEnum ();
		GlobalVariables.Instance.GameState = gameState;
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
}
