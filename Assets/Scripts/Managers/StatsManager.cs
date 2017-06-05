using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GameAnalyticsSDK;
using Sirenix.OdinInspector;
using System.Linq;

public enum WhichPlayer {Player1, Player2, Player3, Player4, None, Draw};
public enum WhichStat {Frags, Hits, Death, Dash, Shots, AimAccuracy, Wins, WinsInARow};

public class StatsManager : SerializedMonoBehaviour
{
	[Header ("Player Stats")]
	public Dictionary<string, PlayerStat> playersStats = new Dictionary<string, PlayerStat> ();

	[Header ("Total Stats")]
	public Dictionary<string, int> totalStats = new Dictionary<string, int> ();

	[Header ("Most Stats")]
	public Dictionary<string, MostStat> mostStats = new Dictionary<string, MostStat> ();

	[Header ("Total")]
	public int totalFrags = 0;
	public int totalHits = 0;
	public int totalDeath = 0;
	public int totalDash = 0;
	public int totalShots = 0;

	[Header ("Players")]
	public List<PlayerStats> playerStatsList = new List<PlayerStats> ();

	[Header ("Player With Most")]
	public List<MostStats> mostStatsList = new List<MostStats> ();

	[Header ("Winner")]
	public string winner;

	[Header ("Wins In A Row")]
	public WhichPlayer mostWinsInARowPlayer = WhichPlayer.None;
	public int winsInARowNumber = 0;

	[Header ("Game Duration")]
	public string gameDuration;
	private float timerDuration;

	private WhichPlayer previousWinner = WhichPlayer.None;

	public static StatsManager Instance;

	void Awake ()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy (gameObject);
	}

	void Start ()
	{
		GlobalVariables.Instance.OnStartMode += SetupStats;

		SetupStats ();

		SetupLists ();

		StartCoroutine (StartTimer ());
	}

	void SetupStats ()
	{
		//Players Stats
		playersStats.Clear ();

		foreach(GameObject g in GlobalVariables.Instance.Players)
		{
			PlayersGameplay playerScript = g.GetComponent<PlayersGameplay> ();

			if (!GlobalVariables.Instance.EnabledPlayersList.Contains (g))
				continue;

			playersStats.Add ( playerScript.playerName.ToString (), new PlayerStat () );
			
			for(int i = 0; i < Enum.GetValues (typeof (WhichStat)).Cast<int> ().Max () + 1; i++)
				playersStats [ playerScript.playerName.ToString () ].playersStats.Add (((WhichStat)i).ToString (), 0);
		}

		//Total Stats
		totalStats.Clear ();

		for (int i = 0; i < Enum.GetValues (typeof(WhichStat)).Cast<int> ().Max () + 1; i++)
			totalStats.Add ( ((WhichStat)i).ToString (), 0);

		//Most Stats
		mostStats.Clear ();

		for (int i = 0; i < Enum.GetValues (typeof(WhichStat)).Cast<int> ().Max () + 1; i++)
		{
			mostStats.Add ( ((WhichStat)i).ToString (), new MostStat ());
			mostStats [((WhichStat)i).ToString ()].whichPlayer = WhichPlayer.None;
		}
	}

	void UpdateMostStats ()
	{
		foreach(KeyValuePair<string, MostStat> d in mostStats)
		{
			int mostValue = -5;
			WhichPlayer player = WhichPlayer.None;

			foreach(KeyValuePair<string, PlayerStat> p in playersStats)
			{
				if(p.Value.playersStats [d.Key] > mostValue)
				{
					mostValue = p.Value.playersStats [d.Key];
					player = (WhichPlayer) Enum.Parse (typeof (WhichPlayer), p.Key);
				}
			}

			if(mostValue != 0)
			{
				d.Value.value = mostValue;
				d.Value.whichPlayer = player;
			}
		}
	}



	void SetupLists ()
	{
		for (int i = 0; i < 4; i++)
			playerStatsList.Add (new PlayerStats ());

		playerStatsList[0].whichPlayer = WhichPlayer.Player1;
		playerStatsList[1].whichPlayer = WhichPlayer.Player2;
		playerStatsList[2].whichPlayer = WhichPlayer.Player3;
		playerStatsList[3].whichPlayer = WhichPlayer.Player4;

		for (int i = 0; i < 7; i++)
		{
			mostStatsList.Add (new MostStats ());
			mostStatsList [i].whichStat = (WhichStat)i;
			mostStatsList [i].whichPlayer = WhichPlayer.None;
		}
	}

	public void GetPlayersEvents ()
	{
		GlobalVariables.Instance.Players[0].GetComponent<PlayersGameplay> ().OnDash += ()=> UpdateMostStats ();

		foreach(GameObject g in GlobalVariables.Instance.Players)
		{
			PlayersGameplay playerScript = g.GetComponent<PlayersGameplay> ();

			playerScript.OnDash += () => 
			{
				playersStats [playerScript.playerName.ToString ()].playersStats ["Dash"]++;
				totalStats ["Dash"]++;
			};

			playerScript.OnShoot += () =>
			{
				playersStats [playerScript.playerName.ToString ()].playersStats ["Shots"]++;
				totalStats ["Shots"]++;

				AimPrecision ();
			};
				

			playerScript.OnDeath += () => 
			{
				playersStats [ playerScript.playerName.ToString () ].playersStats ["Death"]++;
				totalStats ["Death"]++;
			};
		}
	}

	public void PlayersFragsAndHits (GameObject playerThatThrew, GameObject playerHit)
	{
		playersStats [ playerThatThrew.GetComponent<PlayersGameplay> ().playerName.ToString () ].playersStats ["Frags"]++;
		playersStats [ playerHit.GetComponent<PlayersGameplay> ().playerName.ToString () ].playersStats ["Hits"]++;

		totalFrags++;
		totalHits++;

		AimPrecision ();

		UpdateMostStats ();
	}

	public void Winner (WhichPlayer whichPlayerWon)
	{
		playersStats [ whichPlayerWon.ToString () ].playersStats ["Wins"]++;
		WinsInARow (whichPlayerWon);

		switch (whichPlayerWon)
		{
		case WhichPlayer.Player1:
			winner = "Player 1";
			break;
		case WhichPlayer.Player2:
			winner = "Player 2";
			break;
		case WhichPlayer.Player3:
			winner = "Player 3";
			break;
		case WhichPlayer.Player4:
			winner = "Player 4";
			break;
		case WhichPlayer.Draw:
			winner = "Draw";
			break;
		case WhichPlayer.None:
			winner = "None";
			break;
		}
	}

	public void Winner (PlayerName playerName)
	{
		playersStats [ playerName.ToString () ].playersStats ["Wins"]++;
		WinsInARow ((WhichPlayer)(int)playerName);

		switch (playerName)
		{
		case PlayerName.Player1:
			winner = "Player 1";
			break;
		case PlayerName.Player2:
			winner = "Player 2";
			break;
		case PlayerName.Player3:
			winner = "Player 3";
			break;
		case PlayerName.Player4:
			winner = "Player 4";
			break;
		}

	}

	void WinsInARow (WhichPlayer whichPlayerWon)
	{
		if(whichPlayerWon == WhichPlayer.Draw || whichPlayerWon == WhichPlayer.None)
		{
			previousWinner = WhichPlayer.None;

			foreach(KeyValuePair<string, PlayerStat> p in playersStats)
				p.Value.playersStats ["WinsInARow"] = 0;

			mostWinsInARowPlayer = whichPlayerWon;
			winsInARowNumber = 0;
		}
		else
		{
			if (previousWinner == WhichPlayer.None)
			{
				previousWinner = whichPlayerWon;

				playersStats [ whichPlayerWon.ToString () ].playersStats ["WinsInARow"] = 1;
			}
			
			else if(previousWinner == whichPlayerWon)
			{
				foreach(KeyValuePair<string, PlayerStat> p in playersStats)
				{
					if(p.Key != whichPlayerWon.ToString ())
						p.Value.playersStats ["WinsInARow"] = 0;
				}

//				for(int i = 0; i < playerStatsList.Count; i++)
//				{
//					if (playerStatsList [i].whichPlayer != whichPlayerWon)
//						playerStatsList [i].winsInARow = 0;
//				}

				if (playersStats [ whichPlayerWon.ToString () ].playersStats ["WinsInARow"] == 0)
					playersStats [ whichPlayerWon.ToString () ].playersStats ["WinsInARow"] = 2;
				else
					playersStats [ whichPlayerWon.ToString () ].playersStats ["WinsInARow"]++;
			
			}
			
			else if(previousWinner != whichPlayerWon)
			{
				previousWinner = whichPlayerWon;
				
				foreach(KeyValuePair<string, PlayerStat> p in playersStats)
				{
					if(p.Key != whichPlayerWon.ToString ())
						p.Value.playersStats ["WinsInARow"] = 0;
				}
				
				playersStats [ whichPlayerWon.ToString () ].playersStats ["WinsInARow"]++;
			}
			
			mostWinsInARowPlayer = whichPlayerWon;
			winsInARowNumber = playersStats [ whichPlayerWon.ToString () ].playersStats ["WinsInARow"];			
		}
	}

	public void AimPrecision ()
	{
		foreach(KeyValuePair<string, PlayerStat> p in playersStats)
		{
			if (p.Value.playersStats ["Shots"] == 0)
				continue;

			float temp = (float)(p.Value.playersStats ["Frags"] / p.Value.playersStats ["Shots"] * 100f);
			p.Value.playersStats ["AimAccuracy"] = (int) temp;
		}
	}

	public void MostStatsUpdate  ()
	{
		int frags = 0;
		int hits = 0;
		int deaths = 0;
		int dashs = 0;
		int shots = 0;
		int accuracy = 0;
		int wins = 0;

		for(int i = 0; i < playerStatsList.Count; i++)
		{
			if (playerStatsList [i].frags > frags)
			{
				frags = playerStatsList [i].frags;

				mostStatsList [0].statNumber = playerStatsList [i].frags;
				mostStatsList [0].whichPlayer = (WhichPlayer)i;
			}

			if (playerStatsList [i].hits > hits)
			{
				hits = playerStatsList [i].hits;

				mostStatsList [1].statNumber = playerStatsList [i].hits;
				mostStatsList [1].whichPlayer = (WhichPlayer)i;
			}

			if (playerStatsList [i].death > deaths)
			{
				deaths = playerStatsList [i].death;

				mostStatsList [2].statNumber = playerStatsList [i].death;
				mostStatsList [2].whichPlayer = (WhichPlayer)i;
			}

			if (playerStatsList [i].dash > dashs)
			{
				dashs = playerStatsList [i].dash;

				mostStatsList [3].statNumber = playerStatsList [i].dash;
				mostStatsList [3].whichPlayer = (WhichPlayer)i;
			}

			if (playerStatsList [i].shots > shots)
			{
				shots = playerStatsList [i].shots;

				mostStatsList [4].statNumber = playerStatsList [i].shots;
				mostStatsList [4].whichPlayer = (WhichPlayer)i;
			}

			if (playerStatsList [i].aimAccuracy > accuracy)
			{
				accuracy = playerStatsList [i].aimAccuracy;

				mostStatsList [5].statNumber = playerStatsList [i].aimAccuracy;
				mostStatsList [5].whichPlayer = (WhichPlayer)i;
			}

			if (playerStatsList [i].wins > wins)
			{
				wins = playerStatsList [i].wins;

				mostStatsList [6].statNumber = playerStatsList [i].wins;
				mostStatsList [6].whichPlayer = (WhichPlayer)i;
			}
		}
	}

	public void ResetStats (bool resetWins = false)
	{
		for(int i = 0; i < playerStatsList.Count; i++)
		{
			playerStatsList [i].frags = 0;
			playerStatsList [i].hits = 0;
			playerStatsList [i].death = 0;
			playerStatsList [i].dash = 0;
			playerStatsList [i].shots = 0;

			playerStatsList [i].aimAccuracy = 0;

			if(resetWins)
			{
				playerStatsList [i].wins = 0;
				playerStatsList [i].winsInARow = 0;
				previousWinner = WhichPlayer.None;
			}
		}

		totalFrags = 0;
		totalHits = 0;
		totalDeath = 0;
		totalDash = 0;
		totalShots = 0;

		timerDuration = 0;

		for(int i = 0; i < mostStatsList.Count; i++)
		{
			mostStatsList [i].statNumber = 0;
			mostStatsList [i].whichPlayer = WhichPlayer.None;
		}

	}

	IEnumerator StartTimer ()
	{
		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		timerDuration = 0;

		StartCoroutine (Timer ());
	}

	IEnumerator Timer ()
	{
		yield return new WaitForSecondsRealtime (1);

		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);
	
		timerDuration += 1;

		string minutes = Mathf.Floor(timerDuration / 60).ToString("0");
		string seconds = Mathf.Floor(timerDuration % 60).ToString("00");

		gameDuration = minutes + ":" + seconds;

		StartCoroutine (Timer ());
	}
}

[Serializable]
public class PlayerStats 
{
	public WhichPlayer whichPlayer;

	public int frags = 0;
	public int hits = 0;
	public int death = 0;
	public int dash = 0;
	public int shots = 0;

	[Range (0, 100)]
	public int aimAccuracy = 0;

	public int wins = 0;

	public int winsInARow = 0;
}

[Serializable]
public class PlayerStat
{
	public Dictionary<string, int> playersStats = new Dictionary<string, int> ();
}

[Serializable]
public class MostStat
{
	public WhichPlayer whichPlayer;
	public int value = 0;
}

[Serializable]
public class MostStats 
{
	public WhichStat whichStat;
	public int statNumber = 0;
	public WhichPlayer whichPlayer;
}

[Serializable]
public class HitByWhichPlayer
{
	public WhichPlayer hitBy;
	public int hitsCount = 0;
}
