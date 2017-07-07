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
	public float endGameDelay = 2;
	public float endGameDelayInstantRestart = 3;

	[Header ("Death Count")]
	public float timeBeforePlayerRespawn = 2;

	[Header ("Cubes Spawn")]
	public bool spawnCubes = true;

	[Header ("Dead Cube")]
	public MovableScript movableExampleScript;
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

		if(GlobalVariables.Instance.AllMovables.Count > 0 && spawnCubes)
			GlobalMethods.Instance.RandomPositionMovablesVoid (GlobalVariables.Instance.AllMovables.ToArray ());
	}

	public virtual void PlayerDeath (PlayerName playerName, GameObject player)
	{
		PlayersGameplay playerScript = player.GetComponent<PlayersGameplay> ();

		playerScript.livesCount--;
		
		GlobalVariables.Instance.ListPlayers ();

		//Check Game End
		int playersCount = 0;
		GameObject lastPlayer = null;

		foreach(GameObject g in GlobalVariables.Instance.EnabledPlayersList)
			if(g.GetComponent<PlayersGameplay> ().livesCount != 0)
			{
				playersCount++;
				lastPlayer = g;
			}

		if(playersCount == 1 && gameEndLoopRunning == false)
		{
			gameEndLoopRunning = true;
			StatsManager.Instance.Winner(lastPlayer.GetComponent<PlayersGameplay> ().playerName);

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
		if (playerScript.livesCount != 0 && !gameEndLoopRunning) 
		{
			GlobalMethods.Instance.SpawnDeathText (playerName, player, playerScript.livesCount);
			GlobalMethods.Instance.SpawnExistingPlayerRandomVoid (player, timeBeforePlayerRespawn, true);
		} 
		else if (playerDeadCube && !gameEndLoopRunning && player.GetComponent<AIFXAnimations> () == null)
			PlayerDeadCube (playerScript);
	}

	public virtual void PlayerDeadCube (PlayersGameplay playerScript)
	{
		GlobalMethods.Instance.SpawnPlayerDeadCubeVoid (playerScript.playerName, playerScript.controllerNumber, movableExampleScript);
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
			yield return new WaitForSecondsRealtime (endGameDelay);

			LoadModeManager.Instance.RestartSceneVoid (false);
		}

		else if(GlobalVariables.Instance.CurrentGamesCount > 0)
		{
			yield return new WaitForSecondsRealtime (endGameDelayInstantRestart);

			LoadModeManager.Instance.RestartSceneVoid (true);
		}
		else
		{
			GlobalVariables.Instance.CurrentGamesCount = GlobalVariables.Instance.GamesCount;

			yield return new WaitForSecondsRealtime (endGameDelay);

//			MenuManager.Instance.endModeMenu.EndMode (whichMode);
			MenuManager.Instance.ShowEndMode ();
		}
	}
}
