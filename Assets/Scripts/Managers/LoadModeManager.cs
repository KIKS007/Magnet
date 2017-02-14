using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LoadModeManager : Singleton<LoadModeManager> 
{
	public event EventHandler OnLevelLoaded;

	[Header ("Movement")]

	public float loadingX = -150;
	public float reloadingX = 150;
	public float movementDuration = 0.25f;
	public Ease movementEase = Ease.InOutCubic;

	private Transform mainCamera;

	// Use this for initialization
	void Awake () 
	{
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera").transform;

		StartCoroutine (FirstLoadedScene (GlobalVariables.Instance.firstSceneToLoad));
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

		if (OnLevelLoaded != null)
			OnLevelLoaded ();
		
		mainCamera.GetComponent<SlowMotionCamera> ().StopEndGameSlowMotion ();

		GlobalVariables.Instance.CurrentModeLoaded = sceneToLoad;
		GlobalVariables.Instance.SetWhichModeEnum ();

		StatsManager.Instance.ResetStats (true);

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

		if (OnLevelLoaded != null)
			OnLevelLoaded ();
		
		mainCamera.DOMoveX (orginalPosition, movementDuration).SetEase(movementEase);

		GlobalVariables.Instance.CurrentModeLoaded = sceneToLoad;
		GlobalVariables.Instance.SetWhichModeEnum ();
		GlobalVariables.Instance.GameState = GameStateEnum.Menu;

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

		if (OnLevelLoaded != null)
			OnLevelLoaded ();
		
		myTween = mainCamera.DOMoveX (0, movementDuration).SetEase(movementEase);
	
		yield return myTween.WaitForCompletion ();

		mainCamera.GetComponent<SlowMotionCamera> ().StopEndGameSlowMotion ();

		GlobalVariables.Instance.GameState = GameStateEnum.Playing;

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

		if (OnLevelLoaded != null)
			OnLevelLoaded ();

		mainCamera.DOMoveX (orginalPosition, movementDuration).SetEase(movementEase);
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

	void UpdateGlobalVariables ()
	{
		StatsManager.Instance.GetPlayersEvents ();

		GlobalVariables.Instance.SetPlayersControllerNumbers ();
		GlobalVariables.Instance.ListPlayers ();
	}
		
}
