using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public enum LoadType { LoadMode, LoadMenu, Restart };

public class LoadModeManager : Singleton<LoadModeManager> 
{
	public event EventHandler OnLevelLoaded;

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
	IEnumerator FirstLoadedScene (WhichMode sceneToLoad)
	{
		//Unload All other Scenes than Menu
		for (int i = 0; i < SceneManager.sceneCount; i++)
			if(SceneManager.GetSceneAt(i).name != "Scene Testing" && SceneManager.GetSceneAt(i).name != "Menu")
				yield return SceneManager.UnloadSceneAsync (SceneManager.GetSceneAt (i).name);

		//Unload Scene if already loaded
		if(SceneManager.GetSceneByName(sceneToLoad.ToString ()).isLoaded)
			yield return SceneManager.UnloadSceneAsync (sceneToLoad.ToString ());

		StatsManager.Instance.ResetStats (true);
		UpdateGlobalVariables (sceneToLoad, GameStateEnum.Menu);

		yield return SceneManager.LoadSceneAsync (sceneToLoad.ToString (), LoadSceneMode.Additive);

		if (OnLevelLoaded != null)
			OnLevelLoaded ();
	}

	public void LoadSceneVoid (WhichMode sceneToLoad)
	{
		if (GlobalVariables.Instance.CurrentModeLoaded == sceneToLoad && GlobalVariables.Instance.GameState == GameStateEnum.Menu)
			return;
		
		StartCoroutine (LoadScene (sceneToLoad));
	}

	public void LoadRandomSceneVoid ()
	{
		StartCoroutine (LoadScene ( (WhichMode) UnityEngine.Random.Range (0, (int) Enum.GetNames (typeof (WhichMode)).Length - 2)) );
	}

	//Menu Load Scene to choose mode
	IEnumerator LoadScene (WhichMode sceneToLoad)
	{
		yield return cameraMovement.StartCoroutine ("LoadingPosition");

		if (SceneManager.GetSceneByName (GlobalVariables.Instance.CurrentModeLoaded.ToString ()).isLoaded)
			yield return SceneManager.UnloadSceneAsync (GlobalVariables.Instance.CurrentModeLoaded.ToString ());

		DestroyParticules ();
		StopSlowMotion ();
		StatsManager.Instance.ResetStats (true);
		UpdateGlobalVariables (sceneToLoad, GameStateEnum.Menu);

		yield return SceneManager.LoadSceneAsync (sceneToLoad.ToString (), LoadSceneMode.Additive);

		if (OnLevelLoaded != null)
			OnLevelLoaded ();

		yield return cameraMovement.StartCoroutine ("LoadingPosition");
	}

	public void RestartSceneVoid (bool resetStats = true, bool random = false)
	{
		StartCoroutine (RestartScene (resetStats, random));
	}

	IEnumerator RestartScene (bool resetStats = true, bool random = false)
	{
		yield return cameraMovement.StartCoroutine ("RestartPosition");

		if (SceneManager.GetSceneByName (GlobalVariables.Instance.CurrentModeLoaded.ToString ()).isLoaded)
			yield return SceneManager.UnloadSceneAsync (GlobalVariables.Instance.CurrentModeLoaded.ToString ());

		DestroyParticules ();
		StopSlowMotion ();

		if(resetStats)
			StatsManager.Instance.ResetStats (false);

		if(!random)
			yield return SceneManager.LoadSceneAsync (GlobalVariables.Instance.CurrentModeLoaded.ToString (), LoadSceneMode.Additive);

		else
		{
			WhichMode randomScene = (WhichMode)UnityEngine.Random.Range (0, (int)Enum.GetNames (typeof(WhichMode)).Length - 2);
			UpdateGlobalVariables (randomScene);

			yield return SceneManager.LoadSceneAsync ( randomScene.ToString (), LoadSceneMode.Additive);
		}

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

		if (SceneManager.GetSceneByName (GlobalVariables.Instance.CurrentModeLoaded.ToString ()).isLoaded)
			yield return SceneManager.UnloadSceneAsync (GlobalVariables.Instance.CurrentModeLoaded.ToString ());

		DestroyParticules ();
		StopSlowMotion ();
		StatsManager.Instance.ResetStats (true);

		yield return SceneManager.LoadSceneAsync (GlobalVariables.Instance.CurrentModeLoaded.ToString (), LoadSceneMode.Additive);

		if (OnLevelLoaded != null)
			OnLevelLoaded ();

		yield return cameraMovement.StartCoroutine ("LoadingPosition");
	}

	public void UnLoadSceneVoid ()
	{
		StartCoroutine (UnLoadScene ());
	}

	IEnumerator UnLoadScene ()
	{
		yield return cameraMovement.StartCoroutine ("LoadingPosition");

		if (SceneManager.GetSceneByName (GlobalVariables.Instance.CurrentModeLoaded.ToString ()).isLoaded)
			yield return SceneManager.UnloadSceneAsync (GlobalVariables.Instance.CurrentModeLoaded.ToString ());

		DestroyParticules ();
		StopSlowMotion ();
		StatsManager.Instance.ResetStats (true);

		yield return cameraMovement.StartCoroutine ("LoadingPosition");
	}


	void StopSlowMotion ()
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Paused)
			slowMo.StopPauseSlowMotion ();
		else
			slowMo.StopEndGameSlowMotion ();
	}

	void UpdateGlobalVariables (WhichMode sceneToLoad)
	{
		GlobalVariables.Instance.CurrentModeLoaded = sceneToLoad;
	}

	void UpdateGlobalVariables (WhichMode sceneToLoad, GameStateEnum gameState)
	{
		GlobalVariables.Instance.CurrentModeLoaded = sceneToLoad;
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
