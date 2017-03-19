using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeastDeathManager : MonoBehaviour 
{
	[Header ("Settings")]
	public WhichMode whichMode;
	public float timeBeforeEndGame = 2;

	[Header ("Death Count")]
	public int maxDeath = 15;
	public int[] deathCount = new int[4];
	public float timeBeforePlayerRespawn = 2;

	[Header ("Cubes Spawn")]
	public float durationBetweenSpawn = 0.1f;

	protected bool gameEndLoopRunning = false;

	protected virtual void OnEnable () 
	{
		if (GlobalVariables.Instance.modeObjective != ModeObjective.LeastDeath)
			return;

		for(int i = 0; i < deathCount.Length; i++)
			deathCount[i] = 0;

		StopCoroutine (WaitForBeginning ());
		StartCoroutine (WaitForBeginning ());
	}

	protected virtual IEnumerator WaitForBeginning ()
	{
		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		if(GlobalVariables.Instance.AllMovables.Count > 0)
			GlobalMethods.Instance.RandomPositionMovablesVoid (GlobalVariables.Instance.AllMovables.ToArray (), durationBetweenSpawn);
	}

	public virtual void PlayerDeath (PlayerName playerName, GameObject player)
	{
		if (GlobalVariables.Instance.modeObjective != ModeObjective.LeastDeath)
			return;
		
		deathCount [(int)playerName]++;

		foreach(int death in deathCount)
			if(death >= maxDeath && !gameEndLoopRunning)
			{
				gameEndLoopRunning = true;
				StartCoroutine (GameEnd ());	
			}

		if(!gameEndLoopRunning)
			GlobalMethods.Instance.SpawnExistingPlayerRandomVoid (player, timeBeforePlayerRespawn);
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
