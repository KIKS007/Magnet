using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LastManManager : MonoBehaviour 
{
	[Header ("Settings")]
	public WhichMode whichMode;
	public float timeBeforeEndGame = 2;

	[Header ("Cubes Spawn")]
	public float durationBetweenSpawn = 0.1f;

	private bool gameEndLoopRunning = false;

	void Start ()
	{
		StartCoroutine (WaitForBeginning ());
	}

	IEnumerator WaitForBeginning ()
	{
		List<GameObject> allMovables = new List<GameObject>();

		if(GameObject.FindGameObjectsWithTag ("Movable").Length != 0)
			foreach (GameObject movable in GameObject.FindGameObjectsWithTag ("Movable"))
				allMovables.Add (movable);

		if(GameObject.FindGameObjectsWithTag ("Suggestible").Length != 0)
			foreach (GameObject movable in GameObject.FindGameObjectsWithTag ("Suggestible"))
				allMovables.Add (movable);

		if(GameObject.FindGameObjectsWithTag ("DeadCube").Length != 0)
			foreach (GameObject movable in GameObject.FindGameObjectsWithTag ("DeadCube"))
				allMovables.Add (movable);
		

		for (int i = 0; i < allMovables.Count; i++)
			allMovables [i].SetActive (false);

		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		if(allMovables.Count > 0)
			GlobalMethods.Instance.RandomPositionMovablesVoid (allMovables.ToArray (), durationBetweenSpawn);
	}

	// Update is called once per frame
	void Update () 
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing)
			FindPlayers ();
	}

	void FindPlayers ()
	{
		if(GlobalVariables.Instance.NumberOfAlivePlayers == 1 && gameEndLoopRunning == false)
		{
			gameEndLoopRunning = true;

			StartCoroutine (GameEnd ());
		}

		if(GlobalVariables.Instance.NumberOfAlivePlayers == 0 && gameEndLoopRunning == false)
		{
			Debug.Log ("End Game : " + GlobalVariables.Instance.NumberOfAlivePlayers + " - Time : " + Time.time.ToString ());
			gameEndLoopRunning = true;

			StartCoroutine (GameEndDraw ());
		}
	}

	IEnumerator GameEnd ()
	{
		StatsManager.Instance.Winner(GlobalVariables.Instance.AlivePlayersList [0].GetComponent<PlayersGameplay> ().playerName);
		
		GlobalVariables.Instance.GameState = GameStateEnum.EndMode;
		
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().StartEndGameSlowMotion();
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ScreenShakeCamera>().CameraShaking(FeedbackType.ModeEnd);
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ZoomCamera>().Zoom(FeedbackType.ModeEnd);
		
		yield return new WaitForSecondsRealtime (timeBeforeEndGame);

		GlobalVariables.Instance.GamesCount--;

		if(GlobalVariables.Instance.GamesCount == 0)
		{
			if(SceneManager.GetActiveScene().name != "Scene Testing")
				MenuManager.Instance.endModeMenu.EndMode (whichMode);
		}
		else
			MenuManager.Instance.RestartInstantly ();
	}

	IEnumerator GameEndDraw ()
	{
		StatsManager.Instance.Winner(WhichPlayer.Draw);

		GlobalVariables.Instance.GameState = GameStateEnum.EndMode;

		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().StartEndGameSlowMotion();
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ScreenShakeCamera>().CameraShaking(FeedbackType.ModeEnd);
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ZoomCamera>().Zoom(FeedbackType.ModeEnd);

		yield return new WaitForSecondsRealtime (timeBeforeEndGame);

		GlobalVariables.Instance.GamesCount--;

		if(GlobalVariables.Instance.GamesCount == 0)
		{
			if(SceneManager.GetActiveScene().name != "Scene Testing")
				MenuManager.Instance.endModeMenu.EndMode (whichMode);
		}
		else
			MenuManager.Instance.RestartInstantly ();
	}
	
}
