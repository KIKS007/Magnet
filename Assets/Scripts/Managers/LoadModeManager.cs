using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;
using System.Collections.Generic;

public class LoadModeManager : Singleton<LoadModeManager> 
{
	public event EventHandler OnLevelLoaded;
	public event EventHandler OnLevelUnloaded;

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
		GlobalVariables.Instance.GameState = GameStateEnum.Loading;

		//Unload All other Scenes than Menu
		if(SceneManager.GetActiveScene ().name != "Scene Testing")
		{
			for (int i = 0; i < SceneManager.sceneCount; i++)
				if(SceneManager.GetSceneAt(i).name != "Scene Testing" && SceneManager.GetSceneAt(i).name != "Menu")
					yield return SceneManager.UnloadSceneAsync (SceneManager.GetSceneAt (i).name);
		}

		//Unload Scene if already loaded
		if(SceneManager.GetSceneByName(sceneToLoad.ToString ()).isLoaded)
			yield return SceneManager.UnloadSceneAsync (sceneToLoad.ToString ());

		StatsManager.Instance.ResetStats (true);

		if(SceneManager.GetActiveScene ().name != "Scene Testing")
		{
			yield return SceneManager.LoadSceneAsync (sceneToLoad.ToString (), LoadSceneMode.Additive);
			LevelWasLoaded (sceneToLoad, GameStateEnum.Menu);
		}
		else
		{
			if(SceneManager.sceneCount > 1)
			{
				yield return new WaitUntil (()=> SceneManager.GetSceneAt (1).isLoaded);
				LevelWasLoaded ((WhichMode) Enum.Parse(typeof(WhichMode), SceneManager.GetSceneAt (1).name), GameStateEnum.Playing);
			}

			else
			{
				yield return SceneManager.LoadSceneAsync (sceneToLoad.ToString (), LoadSceneMode.Additive);
				LevelWasLoaded (sceneToLoad, GameStateEnum.Playing);
			}
		}
	}

	public void LoadSceneVoid (WhichMode sceneToLoad)
	{
		if (GlobalVariables.Instance.CurrentModeLoaded == sceneToLoad && GlobalVariables.Instance.GameState == GameStateEnum.Menu)
			return;
		
		StartCoroutine (LoadScene (sceneToLoad));
	}

	public void LoadRandomScene ()
	{
		WhichMode randomScene = WhichMode.Bomb;

		do
		{
			randomScene = (WhichMode)UnityEngine.Random.Range (0, (int)Enum.GetNames (typeof(WhichMode)).Length - 2);
		}
		while (GlobalVariables.Instance.lastPlayedModes.Contains (randomScene));

		StartCoroutine (LoadScene (randomScene));
	}

	public void LoadRandomCocktailScene ()
	{
		if (GlobalVariables.Instance.currentCocktailModes.Count == 0)
			GlobalVariables.Instance.currentCocktailModes.AddRange (GlobalVariables.Instance.selectedCocktailModes);
		
		WhichMode randomMode = GlobalVariables.Instance.currentCocktailModes [UnityEngine.Random.Range (0, GlobalVariables.Instance.currentCocktailModes.Count)];

		GlobalVariables.Instance.currentCocktailModes.Remove (randomMode);

		StartCoroutine (LoadScene (randomMode));
	}

	//Menu Load Scene to choose mode
	IEnumerator LoadScene (WhichMode sceneToLoad)
	{
		yield return cameraMovement.StartCoroutine ("LoadingPosition");

		GlobalVariables.Instance.GameState = GameStateEnum.Loading;

		if (SceneManager.GetSceneByName (GlobalVariables.Instance.CurrentModeLoaded.ToString ()).isLoaded)
			yield return SceneManager.UnloadSceneAsync (GlobalVariables.Instance.CurrentModeLoaded.ToString ());

		DestroyParticules ();
		StopSlowMotion ();
		StatsManager.Instance.ResetStats (true);

		yield return SceneManager.LoadSceneAsync (sceneToLoad.ToString (), LoadSceneMode.Additive);

		LevelWasLoaded (sceneToLoad, GameStateEnum.Menu);

		yield return cameraMovement.StartCoroutine ("LoadingPosition");
	}

	public void RestartSceneVoid (bool resetStats = true)
	{
		StartCoroutine (RestartScene (resetStats));
	}

	IEnumerator RestartScene (bool resetStats = true)
	{
		yield return cameraMovement.StartCoroutine ("RestartPosition");

		GlobalVariables.Instance.GameState = GameStateEnum.Loading;

		if (SceneManager.GetSceneByName (GlobalVariables.Instance.CurrentModeLoaded.ToString ()).isLoaded)
			yield return SceneManager.UnloadSceneAsync (GlobalVariables.Instance.CurrentModeLoaded.ToString ());

		DestroyParticules ();
		StopSlowMotion ();

		if(resetStats)
			StatsManager.Instance.ResetStats (false);

		switch(GlobalVariables.Instance.ModeSequenceType)
		{
		case ModeSequenceType.Selection:
			yield return SceneManager.LoadSceneAsync (GlobalVariables.Instance.CurrentModeLoaded.ToString (), LoadSceneMode.Additive);

			LevelWasLoaded (GlobalVariables.Instance.CurrentModeLoaded, GameStateEnum.Playing);

			yield return cameraMovement.StartCoroutine ("RestartPosition");
			break;

		case ModeSequenceType.Random:
			WhichMode randomScene = WhichMode.Bomb;

			do
			{
				randomScene = (WhichMode)UnityEngine.Random.Range (0, (int)Enum.GetNames (typeof(WhichMode)).Length - 2);
			}
			while (GlobalVariables.Instance.lastPlayedModes.Contains (randomScene));	

			yield return SceneManager.LoadSceneAsync ( randomScene.ToString (), LoadSceneMode.Additive);

			LevelWasLoaded (randomScene, GameStateEnum.Playing);

			yield return cameraMovement.StartCoroutine ("RestartPosition");
			break;

		case ModeSequenceType.Cocktail:
			if (GlobalVariables.Instance.currentCocktailModes.Count == 0)
				GlobalVariables.Instance.currentCocktailModes.AddRange (GlobalVariables.Instance.selectedCocktailModes);
			
			WhichMode randomCocktailScene = GlobalVariables.Instance.currentCocktailModes [UnityEngine.Random.Range (0, GlobalVariables.Instance.currentCocktailModes.Count)];

			GlobalVariables.Instance.currentCocktailModes.Remove (randomCocktailScene);

			yield return SceneManager.LoadSceneAsync ( randomCocktailScene.ToString (), LoadSceneMode.Additive);

			LevelWasLoaded (randomCocktailScene, GameStateEnum.Playing);

			yield return cameraMovement.StartCoroutine ("RestartPosition");
			break;
		}
	}

	public void ReloadSceneVoid ()
	{
		StartCoroutine (ReloadScene ());
	}

	IEnumerator ReloadScene ()
	{
		yield return cameraMovement.StartCoroutine ("LoadingPosition");

		GlobalVariables.Instance.GameState = GameStateEnum.Loading;

		if (SceneManager.GetSceneByName (GlobalVariables.Instance.CurrentModeLoaded.ToString ()).isLoaded)
			yield return SceneManager.UnloadSceneAsync (GlobalVariables.Instance.CurrentModeLoaded.ToString ());

		DestroyParticules ();
		StopSlowMotion ();
		StatsManager.Instance.ResetStats (true);

		yield return SceneManager.LoadSceneAsync (GlobalVariables.Instance.CurrentModeLoaded.ToString (), LoadSceneMode.Additive);

		LevelWasLoaded (GlobalVariables.Instance.CurrentModeLoaded, GameStateEnum.Menu);

		yield return cameraMovement.StartCoroutine ("LoadingPosition");
	}

	public void UnLoadSceneVoid ()
	{
		StartCoroutine (UnLoadScene ());
	}

	IEnumerator UnLoadScene ()
	{
		GlobalVariables.Instance.GameState = GameStateEnum.Loading;

		if (SceneManager.GetSceneByName (GlobalVariables.Instance.CurrentModeLoaded.ToString ()).isLoaded)
			yield return SceneManager.UnloadSceneAsync (GlobalVariables.Instance.CurrentModeLoaded.ToString ());

		DestroyParticules ();
		StopSlowMotion ();
		StatsManager.Instance.ResetStats (true);
	
		LevelWasUnloaded (GameStateEnum.Menu);

		yield return cameraMovement.StartCoroutine ("LoadingPosition");
	}


	void LevelWasLoaded (WhichMode sceneLoaded, GameStateEnum gameState)
	{
		GlobalVariables.Instance.LevelWasLoaded (sceneLoaded, gameState);

		if (OnLevelLoaded != null)
			OnLevelLoaded ();
	}

	void LevelWasUnloaded (GameStateEnum gameState)
	{
		GlobalVariables.Instance.LevelWasUnloaded (gameState);

		if (OnLevelUnloaded != null)
			OnLevelUnloaded ();
	}

	void StopSlowMotion ()
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Paused)
			slowMo.StopPauseSlowMotion ();
		else
			slowMo.StopEndGameSlowMotion ();
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
