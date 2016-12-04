using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using DarkTonic.MasterAudio;
using System.Collections.Generic;

public class TagManager : MonoBehaviour 
{
	public WhichMode whichMode;

	[Header ("Tag Settings")]
	public List<CubesColorCount> cubesColorCountList;
	public WhichPlayer winner = WhichPlayer.None;

	[Header ("Timer Sounds")]
	[SoundGroupAttribute]
	public string lastSecondsSound;

	[Header ("Timer")]
	public Text timerText;
	public float timer;
	public string timerClock;
	public float timeBeforeEndGame = 1;

	// Use this for initialization
	void Start () 
	{
		for (int i = 0; i < GlobalVariables.Instance.EnabledPlayersList.Count; i++)
		{
			cubesColorCountList.Add (new CubesColorCount ());
			cubesColorCountList [i].playerName = GlobalVariables.Instance.EnabledPlayersList [i].GetComponent<PlayersGameplay> ().playerName;
		}

		GameObject[] allMovables = GameObject.FindGameObjectsWithTag ("Movable");

		for (int i = 0; i < allMovables.Length; i++)
			allMovables [i].SetActive (false);

		GlobalMethods.Instance.RandomPositionMovablesVoid (allMovables);
		
		StartCoroutine (Setup ());
	}

	IEnumerator Setup ()
	{
		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		string seconds = Mathf.Floor(timer % 60).ToString("00");
		timerClock = seconds;
		timerText.text = timerClock;

		StartCoroutine (Timer ());
	}
		
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void UpdateScores (PlayerName previousOwner, PlayerName currentOwner)
	{
		for (int i = 0; i < cubesColorCountList.Count; i++)
			if (cubesColorCountList [i].playerName == previousOwner)
				cubesColorCountList [i].cubesCount--;

		for (int i = 0; i < cubesColorCountList.Count; i++)
			if (cubesColorCountList [i].playerName == currentOwner)
				cubesColorCountList [i].cubesCount++;
	}

	public void UpdateScores (PlayerName currentOwner)
	{
		for (int i = 0; i < cubesColorCountList.Count; i++)
			if (cubesColorCountList [i].playerName == currentOwner)
				cubesColorCountList [i].cubesCount++;
	}

	IEnumerator Timer ()
	{
		timer -= Time.deltaTime;

		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		if(timer > 0)
		{
			string seconds = Mathf.Floor(timer % 60).ToString("00");
			timerClock = seconds;

			timerText.text = timerClock;

			StartCoroutine (Timer ());
		}
		else
		{
			StartCoroutine (GameEnd ());
		}
	}

	IEnumerator GameEnd ()
	{
		WhichPlayer whichPlayerTemp = WhichPlayer.None;
		CubesColorCount cubesColorTemp = new CubesColorCount ();
		cubesColorTemp.cubesCount = 1000;

		for (int i = 0; i < cubesColorCountList.Count; i++)
			if (cubesColorCountList [i].cubesCount < cubesColorTemp.cubesCount)
				cubesColorTemp = cubesColorCountList [i];


		for (int i = 0; i < cubesColorCountList.Count; i++)
		{
			if (cubesColorCountList [i] != cubesColorTemp && cubesColorCountList [i].cubesCount == cubesColorTemp.cubesCount)
			{
				StatsManager.Instance.Winner (WhichPlayer.Draw);
				winner = WhichPlayer.Draw;
			}			
		}

		if(winner != WhichPlayer.Draw)
		{
			StatsManager.Instance.Winner ((WhichPlayer)cubesColorTemp.playerName);
			winner = (WhichPlayer)cubesColorTemp.playerName;
		}
		

		GlobalVariables.Instance.GameState = GameStateEnum.EndMode;

		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().StartEndGameSlowMotion();
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking(SlowMotionType.ModeEnd);

		yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(timeBeforeEndGame));

		MenuManager.Instance.endModeMenu.EndMode (whichMode);
	}
}

[Serializable]
public class CubesColorCount
{
	public PlayerName playerName;
	public int cubesCount = 0;
}
