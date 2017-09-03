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
	private MenuCameraMovement cameraMovement;
	private List<WhichMode> modesEnum = new List<WhichMode> ();

	// Use this for initialization
	void Awake () 
	{
		modesEnum = new List<WhichMode> ();

		foreach (WhichMode e in Enum.GetValues(typeof(WhichMode)))
			if (e == WhichMode.Tutorial || e == WhichMode.None || e == WhichMode.Default)
				continue;
			else
				modesEnum.Add (e);

		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera").transform;
		cameraMovement = mainCamera.GetComponent<MenuCameraMovement> ();

		StartCoroutine (FirstLoadedScene (GlobalVariables.Instance.firstSceneToLoad));
	}

	//Game First Scene Loaded
	IEnumerator FirstLoadedScene (WhichMode sceneToLoad)
	{
		//Unload All other Scenes than Menu
		if(SceneManager.GetActiveScene ().name != "Scene Testing")
		{
			for (int i = 0; i < SceneManager.sceneCount; i++)
				if(SceneManager.GetSceneAt(i).name != "Scene Testing" && SceneManager.GetSceneAt(i).name != "Menu")
					yield return SceneManager.UnloadSceneAsync (SceneManager.GetSceneAt (i).name);
		}

		for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
		{
			if(SceneManager.GetSceneByBuildIndex (i).isLoaded && SceneManager.GetSceneByBuildIndex (i).name != "Scene Testing" && SceneManager.GetSceneByBuildIndex (i).name != "Menu")
				yield return SceneManager.UnloadSceneAsync (SceneManager.GetSceneByBuildIndex (i).name);
		}

		//Unload Scene if already loaded
		if(SceneManager.GetSceneByName(sceneToLoad.ToString ()).isLoaded)
			yield return SceneManager.UnloadSceneAsync (sceneToLoad.ToString ());


		if(SceneManager.GetActiveScene ().name != "Scene Testing")
		{
			yield return SceneManager.LoadSceneAsync (sceneToLoad.ToString (), LoadSceneMode.Additive);
			LevelWasLoaded (sceneToLoad, GameStateEnum.Menu);
		}
		else
		{
			//Scene Testing
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

	WhichMode RandomScene ()
	{
		WhichMode randomScene = WhichMode.Bomb;
		do
		{
			randomScene = (WhichMode) modesEnum [UnityEngine.Random.Range (0, modesEnum.Count)];
		}
		while (GlobalVariables.Instance.lastPlayedModes.Contains (randomScene));

		return randomScene;
	}

	WhichMode RandomCocktailScene ()
	{
		if (GlobalVariables.Instance.currentCocktailModes.Count == 0)
			GlobalVariables.Instance.currentCocktailModes.AddRange (GlobalVariables.Instance.selectedCocktailModes);

		WhichMode randomMode = GlobalVariables.Instance.currentCocktailModes [UnityEngine.Random.Range (0, GlobalVariables.Instance.currentCocktailModes.Count)];

		GlobalVariables.Instance.currentCocktailModes.Remove (randomMode);

		return randomMode;
	}

	public void LoadSceneVoid (WhichMode sceneToLoad)
	{
		StartCoroutine (LoadScene (sceneToLoad));
	}

	public void LoadRandomScene ()
	{
		StartCoroutine (LoadScene (RandomScene ()));
	}

	public void LoadRandomCocktailScene ()
	{
		StartCoroutine (LoadScene (RandomCocktailScene ()));
	}


	//Menu Load Scene to choose mode
	IEnumerator LoadScene (WhichMode sceneToLoad, GameStateEnum gameState = GameStateEnum.Menu, bool resetStats = true)
	{
		if (SceneManager.GetSceneByName (GlobalVariables.Instance.CurrentModeLoaded.ToString ()).isLoaded)
			yield return SceneManager.UnloadSceneAsync (GlobalVariables.Instance.CurrentModeLoaded.ToString ());

		DestroyParticules ();

		yield return SceneManager.LoadSceneAsync (sceneToLoad.ToString (), LoadSceneMode.Additive);

		LevelWasLoaded (sceneToLoad, gameState);

		if(resetStats)
			StatsManager.Instance.ResetStats ();
	}
		
	public void RestartSceneVoid (bool instantly = false)
	{
		StartCoroutine (RestartScene (instantly));
	}

	IEnumerator RestartScene (bool instantly = false)
	{
		if(instantly)
		{
			cameraMovement.StartCoroutine ("NewRestartRotation");
			yield return new WaitForSecondsRealtime (cameraMovement.newMovementDuration * 0.15f);
		}

		switch(GlobalVariables.Instance.ModeSequenceType)
		{
		case ModeSequenceType.Selection:
			yield return StartCoroutine (LoadScene (GlobalVariables.Instance.CurrentModeLoaded, GameStateEnum.Playing, !instantly));
			break;
		case ModeSequenceType.Random:
			yield return StartCoroutine (LoadScene (RandomScene (), GameStateEnum.Playing, !instantly));
			break;
		case ModeSequenceType.Cocktail:
			yield return StartCoroutine (LoadScene (RandomCocktailScene (), GameStateEnum.Playing, !instantly));
			break;
		}

		if(!instantly)
			yield return cameraMovement.StartCoroutine ("NewPlayPosition");
	}


	public void UnLoadSceneVoid ()
	{
		StartCoroutine (UnLoadScene ());
	}

	IEnumerator UnLoadScene ()
	{
		if (SceneManager.GetSceneByName (GlobalVariables.Instance.CurrentModeLoaded.ToString ()).isLoaded)
			yield return SceneManager.UnloadSceneAsync (GlobalVariables.Instance.CurrentModeLoaded.ToString ());

		DestroyParticules ();

		LevelWasUnloaded (GameStateEnum.Menu);
	
		StatsManager.Instance.ResetStats ();
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

	public void DestroyParticules ()
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
