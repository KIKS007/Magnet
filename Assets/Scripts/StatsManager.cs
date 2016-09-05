using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum WhichPlayer {Player1, Player2, Player3, Player4, None};
public enum WhichStat {Frags, Hits, Death, Dash, Shots, AimAccuracy, WinsInARow};

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

	void Awake ()
	{
		SetupLists ();
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

		mostStatsList [6].whichStat = WhichStat.WinsInARow;
		mostStatsList [6].whichPlayer = WhichPlayer.None;
	}

	public void GetPlayersEvents ()
	{
		GlobalVariables.Instance.Player1.GetComponent<PlayersGameplay> ().OnDash += ()=> DashPlayer(0);
		GlobalVariables.Instance.Player2.GetComponent<PlayersGameplay> ().OnDash += ()=> DashPlayer(1);
		GlobalVariables.Instance.Player3.GetComponent<PlayersGameplay> ().OnDash += ()=> DashPlayer(2);
		GlobalVariables.Instance.Player4.GetComponent<PlayersGameplay> ().OnDash += ()=> DashPlayer(3);

		GlobalVariables.Instance.Player1.GetComponent<PlayersGameplay> ().OnShoot += ()=> ShotPlayer(0);
		GlobalVariables.Instance.Player2.GetComponent<PlayersGameplay> ().OnShoot += ()=> ShotPlayer(1);
		GlobalVariables.Instance.Player3.GetComponent<PlayersGameplay> ().OnShoot += ()=> ShotPlayer(2);
		GlobalVariables.Instance.Player4.GetComponent<PlayersGameplay> ().OnShoot += ()=> ShotPlayer(3);
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
			playerStatsList [0].winsInARow++;
			winner = "Player 1";
			break;
		case WhichPlayer.Player2:
			playerStatsList [1].winsInARow++;
			winner = "Player 2";
			break;
		case WhichPlayer.Player3:
			playerStatsList [2].winsInARow++;
			winner = "Player 3";
			break;
		case WhichPlayer.Player4:
			playerStatsList [3].winsInARow++;
			winner = "Player 4";
			break;
		}
	}

	public void AimPrecision ()
	{
		for(int i = 0; i < playerStatsList.Count; i++)
		{
			if(playerStatsList [i].shots > 0)
			{
				float temp = ((float)playerStatsList [i].frags / (float)playerStatsList [i].shots) * 100f;
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

		int winsInARow = 0;

		for(int i = 0; i < playerStatsList.Count; i++)
		{
			if (playerStatsList [i].winsInARow > winsInARow)
			{
				winsInARow = playerStatsList [i].winsInARow;

				mostStatsList [6].statNumber = playerStatsList [i].winsInARow;

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

			if(resetWins)
				playerStatsList [i].winsInARow = 0;
		}

		totalFrags = 0;
		totalHits = 0;
		totalDeath = 0;
		totalDash = 0;
		totalShots = 0;

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
