﻿using UnityEngine;
using System.Collections;

public class CrushManager : MonoBehaviour 
{
	public GameObject[] playersList;

	public float timeBeforeEndGame = 2;

	private bool gameEndLoopRunning = false;

	// Update is called once per frame
	void Update () 
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing)
			FindPlayers ();
	}

	void FindPlayers ()
	{
		playersList = GameObject.FindGameObjectsWithTag("Player");

		if(playersList.Length == 1 && gameEndLoopRunning == false)
		{
			gameEndLoopRunning = true;

			StartCoroutine("GameEnd");
		}
	}

	IEnumerator GameEnd ()
	{
		switch (playersList [0].name)
		{
		case "Player 1":
			StatsManager.Instance.Winner(WhichPlayer.Player1);
			break;
		case "Player 2":
			StatsManager.Instance.Winner(WhichPlayer.Player2);
			break;
		case "Player 3":
			StatsManager.Instance.Winner(WhichPlayer.Player3);
			break;
		case "Player 4":
			StatsManager.Instance.Winner(WhichPlayer.Player4);
			break;
		}

		GlobalVariables.Instance.GameState = GameStateEnum.Over;

		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().StartPauseSlowMotion();
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking();

		yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(timeBeforeEndGame));

		GameObject.FindGameObjectWithTag("MainMenuManager").GetComponent<MainMenuManagerScript>().GameOverMenuVoid ();
	}
	
}
