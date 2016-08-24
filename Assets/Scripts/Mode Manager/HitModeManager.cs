﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HitModeManager : MonoBehaviour 
{
	[Header ("Hit Settings")]
	public int timerDuration = 300;
	public float timeBetweenSpawn = 2;
	public float timeBeforeEndGame = 2;

	[Header ("Timer")]
	public float timer;
	public string timerClock;

	// Use this for initialization
	void Start () 
	{
		timer = timerDuration;

		StartCoroutine (StartTimer ());
	}

	IEnumerator StartTimer ()
	{
		while(GlobalVariables.Instance.GameState != GameStateEnum.Playing)
		{
			yield return null;
		}

		StartCoroutine (Timer ());
	}

	IEnumerator Timer ()
	{
		timer -= Time.deltaTime;

		string minutes = Mathf.Floor(timer / 60).ToString("0");
		string seconds = Mathf.Floor(timer % 60).ToString("00");

		timerClock = minutes + ":" + seconds;

		transform.GetChild (0).GetChild (0).GetComponent<Text> ().text = timerClock;

		yield return null;

		while(GlobalVariables.Instance.GameState != GameStateEnum.Playing)
		{
			yield return null;
		}

		if(timer > 0.01f)
			StartCoroutine (Timer ());

		else if(GlobalVariables.Instance.GameState != GameStateEnum.Over)
		{
			StartCoroutine (GameEnded ());
			transform.GetChild (0).GetChild (0).GetComponent<Text> ().text = "00:00";
		}
			
	}

	IEnumerator GameEnded ()
	{
		StatsManager.Instance.Winner(StatsManager.Instance.mostStatsList [(int)WhichStat.Frags].whichPlayer);

		GlobalVariables.Instance.GameState = GameStateEnum.Over;

		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().StartPauseSlowMotion();
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking();

		yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(timeBeforeEndGame));

		GameObject.FindGameObjectWithTag("MainMenuManager").GetComponent<MainMenuManagerScript>().GameOverMenuVoid ();
	}
}
