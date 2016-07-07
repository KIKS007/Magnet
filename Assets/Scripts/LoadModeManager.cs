using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LoadModeManager : Singleton<LoadModeManager> 
{
	[Header ("Scene Test")]
	public string firstSceneToLoad = "Test";

	[Header ("Load Mode Manager")]
	public GameObject[] rootGameObjects;

	public float loadingX = -140;
	public float reloadingX = 140;
	public float movementDuration = 0.25f;
	public Ease movementEase = Ease.InOutCubic;

	public MainMenuManagerScript menuScript;

	public static GameObject player1;
	public static GameObject player2;
	public static GameObject player3;
	public static GameObject player4;

	public static GameObject mirrorForward;
	public static GameObject mirrorBackward;
	public static GameObject mirrorRight;
	public static GameObject mirrorLeft;

	public static GameObject probeForward;
	public static GameObject probeBackward;
	public static GameObject probeRight;
	public static GameObject probeLeft;

	private GameObject reflection;
	private Transform mainCamera;

	// Use this for initialization
	void Awake () 
	{
		StartCoroutine (FirstLoadedScene (firstSceneToLoad));

		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera").transform;
	}

	IEnumerator FirstLoadedScene (string sceneToLoad)
	{
		yield return SceneManager.LoadSceneAsync (sceneToLoad, LoadSceneMode.Additive);

		StaticVariables.Instance.CurrentModeLoaded = sceneToLoad;

		rootGameObjects = SceneManager.GetSceneByName (sceneToLoad).GetRootGameObjects ();

		FindGameObjects ();
	}

	public void LoadSceneVoid (string sceneToLoad)
	{
		mainCamera.GetComponent<SlowMotionCamera> ().StopPauseSlowMotion ();

		if(StaticVariables.Instance.CurrentModeLoaded != sceneToLoad && StaticVariables.Instance.GameOver == true)
		{
			mainCamera.GetComponent<ProbesPlacement> ().followCamera = false;

			StartCoroutine (LoadScene (sceneToLoad));
		}
		
		else if(StaticVariables.Instance.GameOver == false)
		{
			mainCamera.GetComponent<ProbesPlacement> ().followCamera = false;

			StaticVariables.Instance.GameOver = true;
			StartCoroutine (LoadScene (sceneToLoad));
		}

	}

	IEnumerator LoadScene (string sceneToLoad)
	{
		float orginalPosition = mainCamera.transform.position.x;

		Tween myTween = mainCamera.DOMoveX (loadingX, movementDuration).SetEase(movementEase);
		yield return myTween.WaitForCompletion ();

		if (StaticVariables.Instance.CurrentModeLoaded != "")
			yield return SceneManager.UnloadScene (StaticVariables.Instance.CurrentModeLoaded);

		yield return SceneManager.LoadSceneAsync (sceneToLoad, LoadSceneMode.Additive);

		DestroyParticules ();

		mainCamera.DOMoveX (orginalPosition, movementDuration).SetEase(movementEase);

		StaticVariables.Instance.CurrentModeLoaded = sceneToLoad;

		rootGameObjects = SceneManager.GetSceneByName (sceneToLoad).GetRootGameObjects ();

		FindGameObjects ();
	}

	public void RestartSceneVoid ()
	{
		mainCamera.GetComponent<ProbesPlacement> ().followCamera = false;

		StartCoroutine (RestartScene ());
	}

	IEnumerator RestartScene ()
	{
		string sceneToLoad = StaticVariables.Instance.CurrentModeLoaded;

		Tween myTween = mainCamera.DOMoveX (reloadingX, movementDuration).SetEase(movementEase);
		yield return myTween.WaitForCompletion ();

		if (StaticVariables.Instance.CurrentModeLoaded != "")
			yield return SceneManager.UnloadScene (StaticVariables.Instance.CurrentModeLoaded);

		DestroyParticules ();

		StaticVariables.Instance.CurrentModeLoaded = sceneToLoad;

		yield return SceneManager.LoadSceneAsync (sceneToLoad, LoadSceneMode.Additive);

		myTween = mainCamera.DOMoveX (0, movementDuration).SetEase(movementEase);

		rootGameObjects = SceneManager.GetSceneByName (StaticVariables.Instance.CurrentModeLoaded).GetRootGameObjects ();
		FindGameObjects ();
	
		yield return myTween.WaitForCompletion ();

		myTween = mainCamera.DOMove (new Vector3(0, 30, 0), menuScript.cameraMovementDuration).SetEase(movementEase);

		yield return myTween.WaitForCompletion ();

		mainCamera.GetComponent<SlowMotionCamera> ().StopPauseSlowMotion ();

		StaticVariables.Instance.GameOver = false;
		StaticVariables.Instance.GamePaused = false;
	}

	public void ReloadSceneVoid ()
	{
		mainCamera.GetComponent<ProbesPlacement> ().followCamera = false;

		StartCoroutine (ReloadScene ());
	}

	IEnumerator ReloadScene ()
	{
		float orginalPosition = mainCamera.transform.position.x;
		string sceneToLoad = StaticVariables.Instance.CurrentModeLoaded;

		Tween myTween = mainCamera.DOMoveX (loadingX, movementDuration).SetEase(movementEase);
		yield return myTween.WaitForCompletion ();


		if (StaticVariables.Instance.CurrentModeLoaded != "")
			yield return SceneManager.UnloadScene (StaticVariables.Instance.CurrentModeLoaded);

		DestroyParticules ();

		StaticVariables.Instance.CurrentModeLoaded = sceneToLoad;

		mainCamera.GetComponent<SlowMotionCamera> ().StopPauseSlowMotion ();

		yield return SceneManager.LoadSceneAsync (sceneToLoad, LoadSceneMode.Additive);

		mainCamera.DOMoveX (orginalPosition, movementDuration).SetEase(movementEase);

		rootGameObjects = SceneManager.GetSceneByName (StaticVariables.Instance.CurrentModeLoaded).GetRootGameObjects ();

		FindGameObjects ();
	}



	void DestroyParticules ()
	{
		if(StaticVariables.Instance.ParticulesClonesParent.childCount != 0)
		{
			for(int i = 0; i < StaticVariables.Instance.ParticulesClonesParent.childCount; i++)
			{
				Destroy (StaticVariables.Instance.ParticulesClonesParent.GetChild (i).gameObject);
			}
		}
	}

	void FindGameObjects ()
	{
		for(int i = 0; i < rootGameObjects.Length; i++)
		{
			if (rootGameObjects [i].name == "Player 1")
				player1 = rootGameObjects [i];

			if (rootGameObjects [i].name == "Player 2")
				player2 = rootGameObjects [i];

			if (rootGameObjects [i].name == "Player 3")
				player3 = rootGameObjects [i];

			if (rootGameObjects [i].name == "Player 4")
				player4 = rootGameObjects [i];

			if (rootGameObjects [i].name == "Reflection")
				reflection = rootGameObjects [i];
		}

		UpdateStaticVariables ();

		mirrorForward = reflection.transform.GetChild (0).GetChild (0).gameObject;
		mirrorBackward = reflection.transform.GetChild (0).GetChild (1).gameObject;
		mirrorRight = reflection.transform.GetChild (0).GetChild (2).gameObject;
		mirrorLeft = reflection.transform.GetChild (0).GetChild (3).gameObject;

		probeForward = reflection.transform.GetChild (1).GetChild (0).gameObject;
		probeBackward = reflection.transform.GetChild (1).GetChild (1).gameObject;
		probeRight = reflection.transform.GetChild (1).GetChild (2).gameObject;
		probeLeft = reflection.transform.GetChild (1).GetChild (3).gameObject;

		mainCamera.GetComponent<ProbesPlacement> ().GetReflectionGameObjects ();
		mainCamera.GetComponent<ProbesPlacement> ().followCamera = true;
	}

	void UpdateStaticVariables ()
	{
		StaticVariables.Instance.Player1 = player1;
		StaticVariables.Instance.Player2 = player2;
		StaticVariables.Instance.Player3 = player3;
		StaticVariables.Instance.Player4 = player4;
	}
		
}
