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

	protected bool gameEndLoopRunning = false;

	protected virtual void Start ()
	{
		if (GlobalVariables.Instance.modeObjective != ModeObjective.LastMan)
			return;
		
		StartCoroutine (WaitForBeginning ());
	}

	protected virtual IEnumerator WaitForBeginning ()
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
	protected virtual void Update () 
	{
		if (GlobalVariables.Instance.modeObjective != ModeObjective.LastMan)
			return;
		
		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing)
			FindPlayers ();
	}

	protected virtual void FindPlayers ()
	{
		if(GlobalVariables.Instance.NumberOfAlivePlayers == 1 && gameEndLoopRunning == false)
		{
			gameEndLoopRunning = true;
			StatsManager.Instance.Winner(GlobalVariables.Instance.AlivePlayersList [0].GetComponent<PlayersGameplay> ().playerName);

			StartCoroutine (GameEnd ());
		}

		if(GlobalVariables.Instance.NumberOfAlivePlayers == 0 && gameEndLoopRunning == false)
		{
			gameEndLoopRunning = true;
			StatsManager.Instance.Winner(WhichPlayer.Draw);

			StartCoroutine (GameEnd ());
		}
	}

	protected virtual IEnumerator GameEnd ()
	{
		GlobalVariables.Instance.GameState = GameStateEnum.EndMode;
		
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().StartEndGameSlowMotion();
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ScreenShakeCamera>().CameraShaking(FeedbackType.ModeEnd);
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ZoomCamera>().Zoom(FeedbackType.ModeEnd);
		

		GlobalVariables.Instance.CurrentGamesCount--;

		if(SceneManager.GetActiveScene().name == "Scene Testing")
		{
			yield return new WaitForSecondsRealtime (timeBeforeEndGame);

			LoadModeManager.Instance.RestartSceneVoid (false, false);
		}

		else if(GlobalVariables.Instance.CurrentGamesCount > 0)
		{
			yield return new WaitForSecondsRealtime (timeBeforeEndGame * 2);

			MenuManager.Instance.RestartInstantly ();
		}
		else
		{
			GlobalVariables.Instance.CurrentGamesCount = GlobalVariables.Instance.GamesCount;

			yield return new WaitForSecondsRealtime (timeBeforeEndGame);

			MenuManager.Instance.endModeMenu.EndMode (whichMode);
		}
	}	
}
