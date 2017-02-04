using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LastManManager : MonoBehaviour 
{
	[Header ("Settings")]
	public WhichMode whichMode;

	public float timeBeforeEndGame = 2;

	private bool gameEndLoopRunning = false;

	void Start ()
	{
		StartCoroutine (WaitForBeginning ());
	}

	IEnumerator WaitForBeginning ()
	{
		GameObject[] allMovables = GameObject.FindGameObjectsWithTag ("Movable");

		if(allMovables.Length == 0)
			allMovables = GameObject.FindGameObjectsWithTag ("Suggestible");

		if(allMovables.Length == 0)
			allMovables = GameObject.FindGameObjectsWithTag ("DeadCube");

		for (int i = 0; i < allMovables.Length; i++)
			allMovables [i].SetActive (false);

		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		if(allMovables.Length > 0)
			GlobalMethods.Instance.RandomPositionMovablesVoid (allMovables);
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
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ScreenShakeCamera>().CameraShaking(SlowMotionType.ModeEnd);

		yield return new WaitForSecondsRealtime (timeBeforeEndGame);

		if(SceneManager.GetActiveScene().name != "Scene Testing")
			MenuManager.Instance.endModeMenu.EndMode (whichMode);
	}

	IEnumerator GameEndDraw ()
	{
		StatsManager.Instance.Winner(WhichPlayer.Draw);

		GlobalVariables.Instance.GameState = GameStateEnum.EndMode;

		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().StartEndGameSlowMotion();
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ScreenShakeCamera>().CameraShaking(SlowMotionType.ModeEnd);

		yield return new WaitForSecondsRealtime (timeBeforeEndGame);

		if(SceneManager.GetActiveScene().name != "Scene Testing")
			MenuManager.Instance.endModeMenu.EndMode (whichMode);
	}
	
}
