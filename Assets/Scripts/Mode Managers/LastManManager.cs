using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class LastManManager : MonoBehaviour 
{
	[Header ("Settings")]
	public WhichMode whichMode;
	public float timeBeforeEndGame = 2;

	[Header ("Death Count")]
	public int[] livesCount = new int[4];
	public float timeBeforePlayerRespawn = 2;

	[Header ("Cubes Spawn")]
	public bool spawnCubes = true;
	public float durationBetweenSpawn = 0.1f;

	[Header ("Dead Cube")]
	public bool playerDeadCube = true;

	protected bool gameEndLoopRunning = false;

	protected virtual void OnEnable () 
	{
		StopCoroutine (WaitForBeginning ());
		StartCoroutine (WaitForBeginning ());
	}

	protected virtual IEnumerator WaitForBeginning ()
	{
		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		foreach (GameObject g in GlobalVariables.Instance.EnabledPlayersList)
			g.GetComponent<PlayersGameplay> ().livesCount = GlobalVariables.Instance.LivesCount;

		livesCount = new int[GlobalVariables.Instance.NumberOfPlayers];

		for (int i = 0; i < livesCount.Length; i++)
			livesCount [i] = GlobalVariables.Instance.LivesCount;
		
		if(GlobalVariables.Instance.AllMovables.Count > 0 && spawnCubes)
			GlobalMethods.Instance.RandomPositionMovablesVoid (GlobalVariables.Instance.AllMovables.ToArray (), durationBetweenSpawn);
	}

	public virtual void PlayerDeath (PlayerName playerName, GameObject player)
	{
		PlayersGameplay playerScript = player.GetComponent<PlayersGameplay> ();

		livesCount [(int)playerName]--;

		playerScript.livesCount--;
		
		GlobalVariables.Instance.ListPlayers ();

		//Check Game End
		int playersCount = 0;
		int lastPlayer = 0;

		for (int i = 0; i < livesCount.Length; i++)
		{
			if (livesCount [i] != 0)
			{
				playersCount++;
				lastPlayer = i;
			}
		}

		if(playersCount == 1 && gameEndLoopRunning == false)
		{
			gameEndLoopRunning = true;
			StatsManager.Instance.Winner(GlobalVariables.Instance.Players [lastPlayer].GetComponent<PlayersGameplay> ().playerName);

			StartCoroutine (GameEnd ());
			return;
		}

		if(playersCount == 0 && gameEndLoopRunning == false)
		{
			gameEndLoopRunning = true;
			StatsManager.Instance.Winner(WhichPlayer.Draw);

			StartCoroutine (GameEnd ());
			return;
		}

		//Spawn Play if has lives left
		if(playerScript.livesCount != 0 && !gameEndLoopRunning)
		{
			GlobalMethods.Instance.SpawnDeathText (playerName, player, playerScript.livesCount);
			GlobalMethods.Instance.SpawnExistingPlayerRandomVoid (player, timeBeforePlayerRespawn, true);
		}
		else if(playerDeadCube && !gameEndLoopRunning)
			playerScript.SpawnDeadCube ();
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

			LoadModeManager.Instance.RestartSceneVoid (false);
		}

		else if(GlobalVariables.Instance.CurrentGamesCount > 0)
		{
			yield return new WaitForSecondsRealtime (timeBeforeEndGame * 1.5f);

			LoadModeManager.Instance.RestartSceneVoid (true);
		}
		else
		{
			GlobalVariables.Instance.CurrentGamesCount = GlobalVariables.Instance.GamesCount;

			yield return new WaitForSecondsRealtime (timeBeforeEndGame);

			MenuManager.Instance.endModeMenu.EndMode (whichMode);
		}
	}
}
