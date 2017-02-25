using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GameAnalyticsSDK;

public enum WhichPlayer {Player1, Player2, Player3, Player4, None, Draw};
public enum WhichStat {Frags, Hits, Death, Dash, Shots, AimAccuracy, Wins};

public class StatsManager : Singleton<StatsManager> 
{
	public List<PlayerStats> playerStatsList = new List<PlayerStats> ();

	[Header ("Total")]
	public int totalFrags = 0;
	public int totalHits = 0;
	public int totalDeath = 0;
	public int totalDash = 0;
	public int totalShots = 0;

	[Header ("Player With Most")]
	public List<MostStats> mostStatsList = new List<MostStats> ();

	[Header ("Winner")]
	public string winner;

	[Header ("Wins In A Row")]
	public WhichPlayer mostWinsInARow = WhichPlayer.None;
	public int winsInARowNumber = 0;

	[Header ("Game Duration")]
	public string gameDuration;
	private float timerDuration;

	private WhichPlayer previousWinner = WhichPlayer.None;

	void Start ()
	{
		GlobalVariables.Instance.OnEndMode += UpdateTotalStats;

		SetupLists ();

		StartCoroutine (StartTimer ());
	}

	void SetupLists ()
	{
		for (int i = 0; i < 4; i++)
			playerStatsList.Add (new PlayerStats ());

		playerStatsList[0].whichPlayer = WhichPlayer.Player1;
		playerStatsList[1].whichPlayer = WhichPlayer.Player2;
		playerStatsList[2].whichPlayer = WhichPlayer.Player3;
		playerStatsList[3].whichPlayer = WhichPlayer.Player4;

		for (int i = 0; i < playerStatsList.Count; i++)
		{
			for (int j = 0; j < 3; j++)
				playerStatsList [i].HitByPlayersList.Add (new HitByWhichPlayer ());
		}

		playerStatsList [0].HitByPlayersList [0].hitBy = WhichPlayer.Player2;
		playerStatsList [0].HitByPlayersList [1].hitBy = WhichPlayer.Player3;
		playerStatsList [0].HitByPlayersList [2].hitBy = WhichPlayer.Player4;

		playerStatsList [1].HitByPlayersList [0].hitBy = WhichPlayer.Player1;
		playerStatsList [1].HitByPlayersList [1].hitBy = WhichPlayer.Player3;
		playerStatsList [1].HitByPlayersList [2].hitBy = WhichPlayer.Player4;

		playerStatsList [2].HitByPlayersList [0].hitBy = WhichPlayer.Player1;
		playerStatsList [2].HitByPlayersList [1].hitBy = WhichPlayer.Player2;
		playerStatsList [2].HitByPlayersList [2].hitBy = WhichPlayer.Player4;
	
		playerStatsList [3].HitByPlayersList [0].hitBy = WhichPlayer.Player1;
		playerStatsList [3].HitByPlayersList [1].hitBy = WhichPlayer.Player2;
		playerStatsList [3].HitByPlayersList [2].hitBy = WhichPlayer.Player3;


		for (int i = 0; i < 7; i++)
			mostStatsList.Add (new MostStats ());

		mostStatsList [0].whichStat = WhichStat.Frags;
		mostStatsList [0].whichPlayer = WhichPlayer.None;

		mostStatsList [1].whichStat = WhichStat.Hits;
		mostStatsList [1].whichPlayer = WhichPlayer.None;

		mostStatsList [2].whichStat = WhichStat.Death;
		mostStatsList [2].whichPlayer = WhichPlayer.None;

		mostStatsList [3].whichStat = WhichStat.Dash;
		mostStatsList [3].whichPlayer = WhichPlayer.None;

		mostStatsList [4].whichStat = WhichStat.Shots;
		mostStatsList [4].whichPlayer = WhichPlayer.None;

		mostStatsList [5].whichStat = WhichStat.AimAccuracy;
		mostStatsList [5].whichPlayer = WhichPlayer.None;

		mostStatsList [6].whichStat = WhichStat.Wins;
		mostStatsList [6].whichPlayer = WhichPlayer.None;
	}

	public void GetPlayersEvents ()
	{
		GlobalVariables.Instance.Players[0].GetComponent<PlayersGameplay> ().OnDash += ()=> DashPlayer(0);
		GlobalVariables.Instance.Players[1].GetComponent<PlayersGameplay> ().OnDash += ()=> DashPlayer(1);
		GlobalVariables.Instance.Players[2].GetComponent<PlayersGameplay> ().OnDash += ()=> DashPlayer(2);
		GlobalVariables.Instance.Players[3].GetComponent<PlayersGameplay> ().OnDash += ()=> DashPlayer(3);

		GlobalVariables.Instance.Players[0].GetComponent<PlayersGameplay> ().OnShoot += ()=> ShotPlayer(0);
		GlobalVariables.Instance.Players[1].GetComponent<PlayersGameplay> ().OnShoot += ()=> ShotPlayer(1);
		GlobalVariables.Instance.Players[2].GetComponent<PlayersGameplay> ().OnShoot += ()=> ShotPlayer(2);
		GlobalVariables.Instance.Players[3].GetComponent<PlayersGameplay> ().OnShoot += ()=> ShotPlayer(3);
	}

	public void PlayersFragsAndHits (GameObject playerThatThrew, GameObject playerHit)
	{
		totalFrags++;
		totalHits++;

		switch (playerThatThrew.name)
		{
		case "Player 1":
			playerStatsList [0].frags++;
			break;
		case "Player 2":
			playerStatsList [1].frags++;
			break;
		case "Player 3":
			playerStatsList [2].frags++;
			break;
		case "Player 4":
			playerStatsList [3].frags++;
			break;
		}

		switch (playerHit.name)
		{
		case "Player 1":
			playerStatsList [0].hits++;
			break;
		case "Player 2":
			playerStatsList [1].hits++;
			break;
		case "Player 3":
			playerStatsList [2].hits++;
			break;
		case "Player 4":
			playerStatsList [3].hits++;
			break;
		}

		switch (playerHit.name)
		{
		case "Player 1":
			switch (playerThatThrew.name)
			{
			case "Player 2":
				playerStatsList [0].HitByPlayersList [0].hitsCount ++;
				break;
			case "Player 3":
				playerStatsList [0].HitByPlayersList [1].hitsCount ++;
				break;
			case "Player 4":
				playerStatsList [0].HitByPlayersList [2].hitsCount ++;
				break;
			}
			break;

		case "Player 2":
			switch (playerThatThrew.name)
			{
			case "Player 1":
				playerStatsList [1].HitByPlayersList [0].hitsCount ++;
				break;
			case "Player 3":
				playerStatsList [1].HitByPlayersList [1].hitsCount ++;
				break;
			case "Player 4":
				playerStatsList [1].HitByPlayersList [2].hitsCount ++;
				break;
			}
			break;

		case "Player 3":
			switch (playerThatThrew.name)
			{
			case "Player 1":
				playerStatsList [2].HitByPlayersList [0].hitsCount ++;
				break;
			case "Player 2":
				playerStatsList [2].HitByPlayersList [1].hitsCount ++;
				break;
			case "Player 4":
				playerStatsList [2].HitByPlayersList [2].hitsCount ++;
				break;
			}
			break;

		case "Player 4":
			switch (playerThatThrew.name)
			{
			case "Player 1":
				playerStatsList [3].HitByPlayersList [0].hitsCount ++;
				break;
			case "Player 2":
				playerStatsList [3].HitByPlayersList [1].hitsCount ++;
				break;
			case "Player 3":
				playerStatsList [3].HitByPlayersList [2].hitsCount ++;
				break;
			}
			break;
		}

		AimPrecision ();

		MostStatsUpdate ();
	}

	void DashPlayer (int whichPlayer)
	{
		totalDash++;
		playerStatsList [whichPlayer].dash++;

		MostStatsUpdate ();
	}

	void ShotPlayer (int whichPlayer)
	{
		totalShots++;
		playerStatsList [whichPlayer].shots++;

		AimPrecision ();

		MostStatsUpdate ();
	}

	public void Winner (WhichPlayer whichPlayerWon)
	{
		switch (whichPlayerWon)
		{
		case WhichPlayer.Player1:
			playerStatsList [0].wins++;
			WinsInARow (whichPlayerWon);
			winner = "Player 1";
			break;
		case WhichPlayer.Player2:
			playerStatsList [1].wins++;
			WinsInARow (whichPlayerWon);
			winner = "Player 2";
			break;
		case WhichPlayer.Player3:
			playerStatsList [2].wins++;
			WinsInARow (whichPlayerWon);
			winner = "Player 3";
			break;
		case WhichPlayer.Player4:
			playerStatsList [3].wins++;
			WinsInARow (whichPlayerWon);
			winner = "Player 4";
			break;
		case WhichPlayer.Draw:
			WinsInARow (whichPlayerWon);
			winner = "Draw";
			break;
		case WhichPlayer.None:
			WinsInARow (whichPlayerWon);
			winner = "None";
			break;
		}
	}

	public void Winner (PlayerName playerName)
	{
		WhichPlayer whichPlayerTemp = WhichPlayer.None;

		switch (playerName)
		{
		case PlayerName.Player1:
			whichPlayerTemp = WhichPlayer.Player1;
			winner = "Player 1";
			break;
		case PlayerName.Player2:
			whichPlayerTemp = WhichPlayer.Player2;
			winner = "Player 2";
			break;
		case PlayerName.Player3:
			whichPlayerTemp = WhichPlayer.Player3;
			winner = "Player 3";
			break;
		case PlayerName.Player4:
			whichPlayerTemp = WhichPlayer.Player4;
			winner = "Player 4";
			break;
		}

		playerStatsList [(int)playerName].wins++;
		WinsInARow (whichPlayerTemp);
	}

	void WinsInARow (WhichPlayer whichPlayerWon)
	{
		if(whichPlayerWon == WhichPlayer.Draw || whichPlayerWon == WhichPlayer.None)
		{
			previousWinner = WhichPlayer.None;

			for(int i = 0; i < playerStatsList.Count; i++)
			{
				playerStatsList [i].winsInARow = 0;
			}

			mostWinsInARow = whichPlayerWon;
			winsInARowNumber = 0;
		}
		else
		{
			if (previousWinner == WhichPlayer.None)
			{
				previousWinner = whichPlayerWon;
				
				playerStatsList [(int)whichPlayerWon].winsInARow = 1;
			}
			
			else if(previousWinner == whichPlayerWon)
			{
				for(int i = 0; i < playerStatsList.Count; i++)
				{
					if (playerStatsList [i].whichPlayer != whichPlayerWon)
						playerStatsList [i].winsInARow = 0;
				}
				
				if (playerStatsList [(int)whichPlayerWon].winsInARow == 0)
					playerStatsList [(int)whichPlayerWon].winsInARow = 2;
				else
					playerStatsList [(int)whichPlayerWon].winsInARow++;
			}
			
			else if(previousWinner != whichPlayerWon)
			{
				previousWinner = whichPlayerWon;
				
				for(int i = 0; i < playerStatsList.Count; i++)
				{
					if (playerStatsList [i].whichPlayer != whichPlayerWon)
						playerStatsList [i].winsInARow = 0;
				}
				
				playerStatsList [(int)whichPlayerWon].winsInARow++;
			}
			
			mostWinsInARow = whichPlayerWon;
			winsInARowNumber = playerStatsList [(int)whichPlayerWon].winsInARow;			
		}
	}

	public void AimPrecision ()
	{
		for(int i = 0; i < playerStatsList.Count; i++)
		{
			if(playerStatsList [i].shots > 0)
			{
				float temp = ((float)playerStatsList [i].frags / (float)playerStatsList [i].shots) * 100f;
				if (temp > 100)
					temp = 100;
				playerStatsList [i].aimAccuracy = (int)temp;
			}
		}
	}

	public void MostStatsUpdate  ()
	{
		int frags = 0;

		for(int i = 0; i < playerStatsList.Count; i++)
		{
			if (playerStatsList [i].frags > frags)
			{
				frags = playerStatsList [i].frags;

				mostStatsList [0].statNumber = playerStatsList [i].frags;

				switch (i)
				{
				case 0:
					mostStatsList [0].whichPlayer = WhichPlayer.Player1;
					break;
				case 1:
					mostStatsList [0].whichPlayer = WhichPlayer.Player2;
					break;
				case 2:
					mostStatsList [0].whichPlayer = WhichPlayer.Player3;
					break;
				case 3:
					mostStatsList [0].whichPlayer = WhichPlayer.Player4;
					break;
				}
			}
				
		}

		int hits = 0;

		for(int i = 0; i < playerStatsList.Count; i++)
		{
			if (playerStatsList [i].hits > hits)
			{
				hits = playerStatsList [i].hits;

				mostStatsList [1].statNumber = playerStatsList [i].hits;

				switch (i)
				{
				case 0:
					mostStatsList [1].whichPlayer = WhichPlayer.Player1;
					break;
				case 1:
					mostStatsList [1].whichPlayer = WhichPlayer.Player2;
					break;
				case 2:
					mostStatsList [1].whichPlayer = WhichPlayer.Player3;
					break;
				case 3:
					mostStatsList [1].whichPlayer = WhichPlayer.Player4;
					break;
				}
			}

		}

		int deaths = 0;

		for(int i = 0; i < playerStatsList.Count; i++)
		{
			if (playerStatsList [i].death > deaths)
			{
				deaths = playerStatsList [i].death;

				mostStatsList [2].statNumber = playerStatsList [i].death;

				switch (i)
				{
				case 0:
					mostStatsList [2].whichPlayer = WhichPlayer.Player1;
					break;
				case 1:
					mostStatsList [2].whichPlayer = WhichPlayer.Player2;
					break;
				case 2:
					mostStatsList [2].whichPlayer = WhichPlayer.Player3;
					break;
				case 3:
					mostStatsList [2].whichPlayer = WhichPlayer.Player4;
					break;
				}
			}

		}

		int dashs = 0;

		for(int i = 0; i < playerStatsList.Count; i++)
		{
			if (playerStatsList [i].dash > dashs)
			{
				dashs = playerStatsList [i].dash;

				mostStatsList [3].statNumber = playerStatsList [i].dash;

				switch (i)
				{
				case 0:
					mostStatsList [3].whichPlayer = WhichPlayer.Player1;
					break;
				case 1:
					mostStatsList [3].whichPlayer = WhichPlayer.Player2;
					break;
				case 2:
					mostStatsList [3].whichPlayer = WhichPlayer.Player3;
					break;
				case 3:
					mostStatsList [3].whichPlayer = WhichPlayer.Player4;
					break;
				}
			}

		}

		int shots = 0;

		for(int i = 0; i < playerStatsList.Count; i++)
		{
			if (playerStatsList [i].shots > shots)
			{
				shots = playerStatsList [i].shots;

				mostStatsList [4].statNumber = playerStatsList [i].shots;

				switch (i)
				{
				case 0:
					mostStatsList [4].whichPlayer = WhichPlayer.Player1;
					break;
				case 1:
					mostStatsList [4].whichPlayer = WhichPlayer.Player2;
					break;
				case 2:
					mostStatsList [4].whichPlayer = WhichPlayer.Player3;
					break;
				case 3:
					mostStatsList [4].whichPlayer = WhichPlayer.Player4;
					break;
				}
			}

		}

		int accuracy = 0;

		for(int i = 0; i < playerStatsList.Count; i++)
		{
			if (playerStatsList [i].aimAccuracy > accuracy)
			{
				accuracy = playerStatsList [i].aimAccuracy;

				mostStatsList [5].statNumber = playerStatsList [i].aimAccuracy;

				switch (i)
				{
				case 0:
					mostStatsList [5].whichPlayer = WhichPlayer.Player1;
					break;
				case 1:
					mostStatsList [5].whichPlayer = WhichPlayer.Player2;
					break;
				case 2:
					mostStatsList [5].whichPlayer = WhichPlayer.Player3;
					break;
				case 3:
					mostStatsList [5].whichPlayer = WhichPlayer.Player4;
					break;
				}
			}

		}

		int wins = 0;

		for(int i = 0; i < playerStatsList.Count; i++)
		{
			if (playerStatsList [i].wins > wins)
			{
				wins = playerStatsList [i].wins;

				mostStatsList [6].statNumber = playerStatsList [i].wins;

				switch (i)
				{
				case 0:
					mostStatsList [6].whichPlayer = WhichPlayer.Player1;
					break;
				case 1:
					mostStatsList [6].whichPlayer = WhichPlayer.Player2;
					break;
				case 2:
					mostStatsList [6].whichPlayer = WhichPlayer.Player3;
					break;
				case 3:
					mostStatsList [6].whichPlayer = WhichPlayer.Player4;
					break;
				}
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

		for (int i = 0; i < playerStatsList.Count; i++)
		{
			for (int j = 0; j < 3; j++)
				playerStatsList [i].HitByPlayersList[j].hitsCount = 0;
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

	private int totalScorePlayer1 = -1;
	private int totalDashPlayer1 = -1;
	private int totalShotPlayer1 = -1;
	private int totalHitPlayer1 = -1;

	private int totalScorePlayer2 = -1;
	private int totalDashPlayer2 = -1;
	private int totalShotPlayer2 = -1;
	private int totalHitPlayer2 = -1;

	private int totalScorePlayer3 = -1;
	private int totalDashPlayer3 = -1;
	private int totalShotPlayer3 = -1;
	private int totalHitPlayer3 = -1;

	private int totalScorePlayer4 = -1;
	private int totalDashPlayer4 = -1;
	private int totalShotPlayer4 = -1;
	private int totalHitPlayer4 = -1;

	void UpdateTotalStats ()
	{
		if(GlobalVariables.Instance.PlayersControllerNumber[0] != -1)
		{
			if (totalScorePlayer1 == -1)
				totalScorePlayer1 = 0;

			if (totalDashPlayer1 == -1)
				totalDashPlayer1 = 0;

			if (totalShotPlayer1 == -1)
				totalShotPlayer1 = 0;

			if (totalHitPlayer1 == -1)
				totalHitPlayer1 = 0;

			totalScorePlayer1 += playerStatsList [0].wins;
			totalDashPlayer1 += playerStatsList [0].dash;
			totalShotPlayer1 += playerStatsList [0].shots;
			totalHitPlayer1 += playerStatsList [0].frags;
		}
		if(GlobalVariables.Instance.PlayersControllerNumber[1] != -1)
		{
			if (totalScorePlayer2 == -1)
				totalScorePlayer2 = 0;

			if (totalDashPlayer2 == -1)
				totalDashPlayer2 = 0;

			if (totalShotPlayer2 == -1)
				totalShotPlayer2 = 0;

			if (totalHitPlayer2 == -1)
				totalHitPlayer2 = 0;

			totalScorePlayer2 += playerStatsList [1].wins;
			totalDashPlayer2 += playerStatsList [1].dash;
			totalShotPlayer2 += playerStatsList [1].shots;
			totalHitPlayer2 += playerStatsList [1].frags;
		}
		if(GlobalVariables.Instance.PlayersControllerNumber[2] != -1)
		{
			if (totalScorePlayer3 == -1)
				totalScorePlayer3 = 0;

			if (totalDashPlayer3 == -1)
				totalDashPlayer3 = 0;

			if (totalShotPlayer3 == -1)
				totalShotPlayer3 = 0;

			if (totalHitPlayer3 == -1)
				totalHitPlayer3 = 0;

			totalScorePlayer3 += playerStatsList [2].wins;
			totalDashPlayer3 += playerStatsList [2].dash;
			totalShotPlayer3 += playerStatsList [2].shots;
			totalHitPlayer3 += playerStatsList [2].frags;
		}
		if(GlobalVariables.Instance.PlayersControllerNumber[3] != -1)
		{
			if (totalScorePlayer4 == -1)
				totalScorePlayer4 = 0;

			if (totalDashPlayer4 == -1)
				totalDashPlayer4 = 0;

			if (totalShotPlayer4 == -1)
				totalShotPlayer4 = 0;

			if (totalHitPlayer4 == -1)
				totalHitPlayer4 = 0;

			totalScorePlayer4 += playerStatsList [3].wins;
			totalDashPlayer4 += playerStatsList [3].dash;
			totalShotPlayer4 += playerStatsList [3].shots;
			totalHitPlayer4 += playerStatsList [3].frags;
		}
	}

	void SendStats ()
	{
		if(GlobalVariables.Instance.PlayersControllerNumber[0] != -1)
		{
			if(totalShotPlayer1 != -1)
				GameAnalytics.NewDesignEvent ("Player:" + "Player 1" + ":" + GlobalVariables.Instance.CurrentModeLoaded.ToString () + ":Score", totalScorePlayer1);
			if(totalDashPlayer1 != -1)
				GameAnalytics.NewDesignEvent ("Player:" + "Player 1" + ":" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":DashCount", totalDashPlayer1);
			if(totalHitPlayer1 != -1)
				GameAnalytics.NewDesignEvent ("Player:" + "Player 1" + ":" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":HitCount", totalHitPlayer1);
			if(totalShotPlayer1 != -1)
				GameAnalytics.NewDesignEvent ("Player:" + "Player 1" + ":" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":ShotCount", totalShotPlayer1);
		}

		if(GlobalVariables.Instance.PlayersControllerNumber[1] != -1)
		{
			if(totalScorePlayer2 != -1)
			GameAnalytics.NewDesignEvent ("Player:" + "Player 2" + ":" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":Score", totalScorePlayer2);
			if(totalDashPlayer2 != -1)
			GameAnalytics.NewDesignEvent ("Player:" + "Player 2" + ":" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":DashCount", totalDashPlayer2);
			if(totalHitPlayer2 != -1)
			GameAnalytics.NewDesignEvent ("Player:" + "Player 2" + ":" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":HitCount", totalHitPlayer2);
			if(totalShotPlayer2 != -1)
			GameAnalytics.NewDesignEvent ("Player:" + "Player 2" + ":" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":ShotCount", totalShotPlayer2);
		}

		if(GlobalVariables.Instance.PlayersControllerNumber[2] != -1)
		{
			if(totalScorePlayer3 != -1)
			GameAnalytics.NewDesignEvent ("Player:" + "Player 3" + ":" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":Score", totalScorePlayer3);
			if(totalDashPlayer3 != -1)
			GameAnalytics.NewDesignEvent ("Player:" + "Player 3" + ":" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":DashCount", totalDashPlayer3);
			if(totalHitPlayer3 != -1)
			GameAnalytics.NewDesignEvent ("Player:" + "Player 3" + ":" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":HitCount", totalHitPlayer3);
			if(totalShotPlayer3 != -1)
			GameAnalytics.NewDesignEvent ("Player:" + "Player 3" + ":" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":ShotCount", totalShotPlayer3);
		}

		if(GlobalVariables.Instance.PlayersControllerNumber[3] != -1)
		{
			if(totalScorePlayer4 != -1)
			GameAnalytics.NewDesignEvent ("Player:" + "Player 4" + ":" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":Score", totalScorePlayer4);
			if(totalDashPlayer4 != -1)
			GameAnalytics.NewDesignEvent ("Player:" + "Player 4" + ":" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":DashCount", totalDashPlayer4);
			if(totalHitPlayer4 != -1)
			GameAnalytics.NewDesignEvent ("Player:" + "Player 4" + ":" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":HitCount", totalHitPlayer4);
			if(totalShotPlayer4 != -1)
			GameAnalytics.NewDesignEvent ("Player:" + "Player 4" + ":" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":ShotCount", totalShotPlayer4);
		}
	}

	void OnApplicationQuit ()
	{
		SendStats ();
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

	public List<HitByWhichPlayer> HitByPlayersList = new List<HitByWhichPlayer> ();

	[Range (0, 100)]
	public int aimAccuracy = 0;

	public int wins = 0;

	public int winsInARow = 0;
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
