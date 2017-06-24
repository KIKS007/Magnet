using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GameAnalyticsSDK;
using Sirenix.OdinInspector;
using System.Linq;

public enum WhichPlayer {Player1, Player2, Player3, Player4, None, Draw};
public enum WhichStat { HitsGiven, HitsTaken, Death, Dash, Shots, AimAccuracy, Wins, WinsInARow, LifeDuration, Kills, Suicides, Stun };

public class StatsManager : SerializedMonoBehaviour
{
	[PropertyOrder (-1)]
	[Button("Update Stats")] 
	void UpdateStatsButton ()
	{
		UpdateStats ();
	}

	public bool settingUp = false;

	[Header ("Stats Text")]
	public Dictionary<string, WhichStat> statsText = new Dictionary<string, WhichStat> ();

	[Header ("Player Stats")]
	public Dictionary<string, PlayerStats> playersStats = new Dictionary<string, PlayerStats> ();

	[Header ("Total Stats")]
	public List<WhichStat> totalExcludeStats = new List<WhichStat> ();
	public Dictionary<string, int> totalStats = new Dictionary<string, int> ();

	[Header ("Most Stats")]
	public Dictionary<string, Stats> mostStats = new Dictionary<string, Stats> ();

	[Header ("Least Stats")]
	public Dictionary<string, Stats> leastStats = new Dictionary<string, Stats> ();

	[Header ("Reset Exclude Stats")]
	public List<WhichStat> resetExcludeStats = new List<WhichStat> ();

	[Header ("Winner")]
	public WhichPlayer winnerName = WhichPlayer.None;
	public string winner;

	[Header ("Wins In A Row")]
	public WhichPlayer mostWinsInARowPlayer = WhichPlayer.None;
	public int winsInARowNumber = 0;

	[Header ("Game Duration")]
	public string allRoundsDuration;
	public string roundDuration;

	private float roundsDurationValue;
	private float allRoundsDurationValue;
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
		GlobalVariables.Instance.OnStartMode += () => 
		{
			StopAllCoroutines ();
			StartCoroutine (StartTimer ());
		};
		GlobalVariables.Instance.OnRestartMode += () => 
		{
			StopAllCoroutines ();
			StartCoroutine (StartTimer ());
		};

		GlobalVariables.Instance.OnMenu += ()=> ResetStats(true);

		SetupStats ();
	}

	void SetupStats ()
	{
		settingUp = true;

		//Players Stats
		playersStats.Clear ();

		foreach(GameObject g in GlobalVariables.Instance.Players)
		{
			if (g == null)
				break;
			
			PlayersGameplay playerScript = g.GetComponent<PlayersGameplay> ();

			if (!GlobalVariables.Instance.EnabledPlayersList.Contains (g))
				continue;

			playersStats.Add ( playerScript.playerName.ToString (), new PlayerStats () );
			
			for(int i = 0; i < Enum.GetValues (typeof (WhichStat)).Cast<int> ().Max () + 1; i++)
				playersStats [ playerScript.playerName.ToString () ].playersStats.Add (((WhichStat)i).ToString (), 0);
		}

		//Total Stats
		totalStats.Clear ();

		for (int i = 0; i < Enum.GetValues (typeof(WhichStat)).Cast<int> ().Max () + 1; i++)
			if(!totalExcludeStats.Contains ( (WhichStat)i ))
				totalStats.Add ( ((WhichStat)i).ToString (), 0);

		//Most Stats
		mostStats.Clear ();

		for (int i = 0; i < Enum.GetValues (typeof(WhichStat)).Cast<int> ().Max () + 1; i++)
		{
			mostStats.Add ( ((WhichStat)i).ToString (), new Stats ());
			mostStats [((WhichStat)i).ToString ()].whichPlayer = WhichPlayer.None;
		}

		//Least Stats
		leastStats.Clear ();

		for (int i = 0; i < Enum.GetValues (typeof(WhichStat)).Cast<int> ().Max () + 1; i++)
		{
			leastStats.Add ( ((WhichStat)i).ToString (), new Stats ());
			leastStats [((WhichStat)i).ToString ()].whichPlayer = WhichPlayer.None;
		}

		settingUp = false;
	}

	void UpdateStats ()
	{
		//Most Values
		foreach(KeyValuePair<string, Stats> d in mostStats)
		{
			int mostValue = -5;
			WhichPlayer player = WhichPlayer.None;
			bool severalMost = false;

			foreach(KeyValuePair<string, PlayerStats> p in playersStats)
			{
				if(p.Value.playersStats [d.Key] > mostValue)
				{
					mostValue = p.Value.playersStats [d.Key];
					player = (WhichPlayer) Enum.Parse (typeof (WhichPlayer), p.Key);
				}
				else if(p.Value.playersStats [d.Key] == mostValue)
					severalMost = true;
			}

			if(mostValue != 0 && !severalMost)
			{
				d.Value.value = mostValue;
				d.Value.whichPlayer = player;
			}
		}

		//Least Values
		foreach(KeyValuePair<string, Stats> d in leastStats)
		{
			int leastValue = playersStats [PlayerName.Player1.ToString ()].playersStats [d.Key];
			WhichPlayer player = WhichPlayer.Player1;
			bool severalLeast = false;

			foreach(KeyValuePair<string, PlayerStats> p in playersStats)
			{
				if(p.Value.playersStats [d.Key] < leastValue)
				{
					leastValue = p.Value.playersStats [d.Key];
					player = (WhichPlayer) Enum.Parse (typeof (WhichPlayer), p.Key);
				}

				else if(p.Value.playersStats [d.Key] == leastValue && p.Key != PlayerName.Player1.ToString ())
					severalLeast = true;
			}

			if(!severalLeast)
			{
				d.Value.value = leastValue;
				d.Value.whichPlayer = player;
			}
			else
			{
				d.Value.value = 0;
				d.Value.whichPlayer = WhichPlayer.None;
			}
		}
	}

	public void GetPlayersEvents ()
	{
		foreach(GameObject g in GlobalVariables.Instance.Players)
		{
			PlayersGameplay playerScript = g.GetComponent<PlayersGameplay> ();

			playerScript.OnDash += () => 
			{
				playersStats [playerScript.playerName.ToString ()].playersStats [WhichStat.Dash.ToString ()]++;
				totalStats [WhichStat.Dash.ToString ()]++;
			};

			playerScript.OnShoot += () =>
			{
				playersStats [playerScript.playerName.ToString ()].playersStats [WhichStat.Shots.ToString ()]++;
				totalStats [WhichStat.Shots.ToString ()]++;

				AimPrecision ();
			};
				

			playerScript.OnDeath += () => 
			{
				playersStats [ playerScript.playerName.ToString () ].playersStats [WhichStat.Death.ToString ()]++;
				totalStats [WhichStat.Death.ToString ()]++;
			};

			playerScript.OnStun += () => 
			{
				playersStats [ playerScript.playerName.ToString () ].playersStats [WhichStat.Stun.ToString ()]++;
				totalStats [WhichStat.Stun.ToString ()]++;
			};
		}
	}

	public void PlayersHits (GameObject playerThatThrew, GameObject playerHit)
	{
		playersStats [ playerThatThrew.GetComponent<PlayersGameplay> ().playerName.ToString () ].playersStats [WhichStat.HitsGiven.ToString ()]++;
		playersStats [ playerHit.GetComponent<PlayersGameplay> ().playerName.ToString () ].playersStats [WhichStat.HitsTaken.ToString ()]++;

		totalStats [WhichStat.HitsGiven.ToString ()]++;
		totalStats [WhichStat.HitsTaken.ToString ()]++;

		AimPrecision ();

		UpdateStats ();
	}

	public void PlayerKills (PlayersGameplay playerThatKilled)
	{
		playersStats [ playerThatKilled.playerName.ToString () ].playersStats [WhichStat.Kills.ToString ()]++;

		totalStats [WhichStat.Kills.ToString ()]++;
	}

	public void PlayerSuicides (PlayersGameplay player)
	{
		playersStats [ player.playerName.ToString () ].playersStats [WhichStat.Death.ToString ()]++;
		totalStats [WhichStat.Death.ToString ()]++;

		playersStats [ player.playerName.ToString () ].playersStats [WhichStat.Suicides.ToString ()]++;
		totalStats [WhichStat.Suicides.ToString ()]++;
	}

	public void PlayersLifeDuration ()
	{
		foreach(var p in GlobalVariables.Instance.EnabledPlayersList)
		{
			PlayersGameplay script = p.GetComponent<PlayersGameplay> ();

			playersStats [script.playerName.ToString ()].playersStats [WhichStat.LifeDuration.ToString ()] += script.lifeDuration;
		}
	}

	public void Winner (WhichPlayer whichPlayerWon)
	{
		winnerName = whichPlayerWon;
		WinsInARow (whichPlayerWon);

		playersStats [ whichPlayerWon.ToString () ].playersStats [WhichStat.Wins.ToString ()]++;

		SetAllRoundsDuration ();
		PlayersLifeDuration ();

		UpdateStats ();

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

		StopAllCoroutines ();
	}

	public void Winner (PlayerName playerName)
	{
		winnerName = (WhichPlayer)(int)playerName;
		WinsInARow ((WhichPlayer)(int)playerName);

		playersStats [ playerName.ToString () ].playersStats [WhichStat.Wins.ToString ()]++;

		SetAllRoundsDuration ();
		PlayersLifeDuration ();

		UpdateStats ();

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

		StopAllCoroutines ();
	}

	void SetAllRoundsDuration ()
	{
		allRoundsDurationValue += roundsDurationValue;

		string minutes = Mathf.Floor(allRoundsDurationValue / 60).ToString("00");
		string seconds = Mathf.Floor(allRoundsDurationValue % 60).ToString("00");

		allRoundsDuration = minutes + ":" + seconds;
	}

	void WinsInARow (WhichPlayer whichPlayerWon)
	{
		if(whichPlayerWon == WhichPlayer.Draw || whichPlayerWon == WhichPlayer.None)
		{
			previousWinner = WhichPlayer.None;

			foreach(KeyValuePair<string, PlayerStats> p in playersStats)
				p.Value.playersStats [WhichStat.WinsInARow.ToString ()] = 0;

			mostWinsInARowPlayer = whichPlayerWon;
			winsInARowNumber = 0;
		}
		else
		{
			if (previousWinner == WhichPlayer.None)
			{
				previousWinner = whichPlayerWon;

				playersStats [ whichPlayerWon.ToString () ].playersStats [WhichStat.WinsInARow.ToString ()] = 1;
			}
			
			else if(previousWinner == whichPlayerWon)
			{
				foreach(KeyValuePair<string, PlayerStats> p in playersStats)
				{
					if(p.Key != whichPlayerWon.ToString ())
						p.Value.playersStats ["WinsInARow"] = 0;
				}

				if (playersStats [ whichPlayerWon.ToString () ].playersStats ["WinsInARow"] == 0)
					playersStats [ whichPlayerWon.ToString () ].playersStats ["WinsInARow"] = 2;
				else
					playersStats [ whichPlayerWon.ToString () ].playersStats ["WinsInARow"]++;
			
			}
			
			else if(previousWinner != whichPlayerWon)
			{
				previousWinner = whichPlayerWon;
				
				foreach(KeyValuePair<string, PlayerStats> p in playersStats)
				{
					if(p.Key != whichPlayerWon.ToString ())
						p.Value.playersStats ["WinsInARow"] = 0;
				}
				
				playersStats [ whichPlayerWon.ToString () ].playersStats ["WinsInARow"]++;
			}
			
			mostWinsInARowPlayer = whichPlayerWon;
			winsInARowNumber = playersStats [ whichPlayerWon.ToString () ].playersStats [WhichStat.WinsInARow.ToString ()];			
		}
	}

	public void AimPrecision ()
	{
		foreach(KeyValuePair<string, PlayerStats> p in playersStats)
		{
			if (p.Value.playersStats [WhichStat.Shots.ToString ()] == 0)
				continue;

			float temp = (float)p.Value.playersStats [WhichStat.HitsGiven.ToString ()] / (float)p.Value.playersStats [WhichStat.Shots.ToString ()] * 100f;

			p.Value.playersStats [WhichStat.AimAccuracy.ToString ()] = (int) Mathf.Round (temp);
		}
	}

	public void ResetStats (bool resetAll = false)
	{
		settingUp = true;

		roundsDurationValue = 0;
		winnerName = WhichPlayer.None;
		roundDuration = "";
		winner = "";

		if(resetAll)
		{
			SetupStats ();
			previousWinner = WhichPlayer.None;
			allRoundsDurationValue = 0;
			allRoundsDuration = "";
		}
		else
		{
			//Players
			foreach(KeyValuePair<string, PlayerStats> p in playersStats)
			{
				foreach(var key in p.Value.playersStats.Keys.ToList ())
				{
					if (resetExcludeStats.Contains ((WhichStat)Enum.Parse (typeof(WhichStat), key)))
						continue;
					
					p.Value.playersStats [key] = 0;
				}
			}

			//Total Stats
			foreach(var key in totalStats.Keys.ToList ())
			{
				if(resetExcludeStats.Contains ((WhichStat)Enum.Parse (typeof(WhichStat), key)))
					continue;

				totalStats [key] = 0;
			}
			
			//Most Stats
			foreach(KeyValuePair<string, Stats> d in mostStats)
			{
				if (resetExcludeStats.Contains ((WhichStat)Enum.Parse (typeof(WhichStat), d.Key)))
					continue;
				
				d.Value.value = 0;
				d.Value.whichPlayer = WhichPlayer.None;
			}

			//Least Stats
			foreach(KeyValuePair<string, Stats> d in leastStats)
			{
				if (resetExcludeStats.Contains ((WhichStat)Enum.Parse (typeof(WhichStat), d.Key)))
					continue;

				d.Value.value = 0;
				d.Value.whichPlayer = WhichPlayer.None;
			}
		}
	}

	IEnumerator StartTimer ()
	{
		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		roundsDurationValue = 0;

		StartCoroutine (Timer ());
	}

	IEnumerator Timer ()
	{
		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		yield return new WaitForSecondsRealtime (1);

		roundsDurationValue += 1;

		string minutes = Mathf.Floor(roundsDurationValue / 60).ToString("00");
		string seconds = Mathf.Floor(roundsDurationValue % 60).ToString("00");

		roundDuration = minutes + ":" + seconds;

		StartCoroutine (Timer ());
	}
}

[Serializable]
public class PlayerStats
{
	public Dictionary<string, int> playersStats = new Dictionary<string, int> ();
}

[Serializable]
public class Stats
{
	public WhichPlayer whichPlayer;
	public int value = 0;
}
	
