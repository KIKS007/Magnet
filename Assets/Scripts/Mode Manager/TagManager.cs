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
	public float[] timersValue = new float[4];

	[Header ("Explosion")]
	public float explosionForce = 50;
	public float explosionRadius = 50;
	public LayerMask explosionMask;

	[Header ("Timer")]
	public Text timerText;
	public float timer = 0;
	public string timerClock;
	public float timeBeforeEndGame = 1;

	// Use this for initialization
	void Start () 
	{
		GameObject[] allMovables = GameObject.FindGameObjectsWithTag ("Movable");

		ResetCubesColors ();

		GlobalMethods.Instance.RandomPositionMovablesVoid (allMovables);

		StartCoroutine (Setup ());
	}

	IEnumerator Setup ()
	{
		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		SetupCubesColorsList ();

		string seconds = Mathf.Floor(timer % 60).ToString("00");
		timerClock = seconds;
		timerText.text = timerClock;

		timer = timersValue [4 - GlobalVariables.Instance.NumberOfAlivePlayers];

		StartCoroutine (Timer ());
	}
		
	void SetupCubesColorsList ()
	{
		cubesColorCountList.Clear ();

		for (int i = 0; i < GlobalVariables.Instance.AlivePlayersList.Count; i++)
		{
			cubesColorCountList.Add (new CubesColorCount ());
			cubesColorCountList [i].playerName = GlobalVariables.Instance.AlivePlayersList [i].GetComponent<PlayersGameplay> ().playerName;
			cubesColorCountList [i].cubesCount = 0;
		}

	}

	void ResetCubesColors ()
	{
		GameObject[] allMovables = GameObject.FindGameObjectsWithTag ("Movable");

		for (int i = 0; i < allMovables.Length; i++)
		{
			allMovables [i].GetComponent<MovableScript> ().ToNeutralColor ();
			allMovables [i].GetComponent<MovableTag> ().previousOwner = null;
		}
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
			timerText.text = "00";

			FindLooser ();

			if(GlobalVariables.Instance.NumberOfAlivePlayers > 1)
			{
				timer = timersValue [4 - GlobalVariables.Instance.NumberOfAlivePlayers];

				SetupCubesColorsList ();
				StartCoroutine (Timer ());
				ResetCubesColors ();
			}
			else
			{
				StartCoroutine (GameEnd ());				
			}

		}
	}

	public void FindLooser ()
	{
		CubesColorCount cubesColorTemp = new CubesColorCount ();
		cubesColorTemp.cubesCount = 0;

		for (int i = 0; i < cubesColorCountList.Count; i++)
			if (cubesColorCountList [i].cubesCount > cubesColorTemp.cubesCount)
				cubesColorTemp = cubesColorCountList [i];


		//Debug.Log ("NAME : " + cubesColorTemp.playerName + " FIRST LOOSER : " + GlobalVariables.Instance.Players [(int)cubesColorTemp.playerName]);

		GlobalVariables.Instance.Players [(int)cubesColorTemp.playerName].GetComponent<PlayersGameplay> ().DeathExplosionFX ();
		GlobalVariables.Instance.Players [(int)cubesColorTemp.playerName].GetComponent<PlayersGameplay> ().Death ();

		GlobalMethods.Instance.Explosion (GlobalVariables.Instance.Players [(int)cubesColorTemp.playerName].transform.position, explosionForce, explosionRadius, explosionMask);


		for (int i = 0; i < cubesColorCountList.Count; i++)
		{
			if (cubesColorCountList [i] != cubesColorTemp && cubesColorCountList [i].cubesCount == cubesColorTemp.cubesCount)
			{
				//Debug.Log ("NAME : " + cubesColorCountList [i].playerName + " OTHER LOOSER : " + GlobalVariables.Instance.Players [(int)cubesColorCountList [i].playerName]);
				GlobalVariables.Instance.Players [(int)cubesColorCountList [i].playerName].GetComponent<PlayersGameplay> ().DeathExplosionFX ();
				GlobalVariables.Instance.Players [(int)cubesColorCountList [i].playerName].GetComponent<PlayersGameplay> ().Death ();

				GlobalMethods.Instance.Explosion (GlobalVariables.Instance.Players [(int)cubesColorTemp.playerName].transform.position, explosionForce, explosionRadius, explosionMask);
			}			
		}
	}

	IEnumerator GameEnd ()
	{
		if(GlobalVariables.Instance.NumberOfAlivePlayers == 1)
		{
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
		}
		else
		{
			StatsManager.Instance.Winner (WhichPlayer.None);
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
