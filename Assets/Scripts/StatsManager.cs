using UnityEngine;
using System.Collections;

public class StatsManager : Singleton<StatsManager> 
{
	public int[] PlayersFrags = new int[5];

	public int[] PlayersHits = new int[5];

	public int[] PlayersDeath = new int[5];

	public int[] PlayersDash = new int[5];

	public int[] PlayersShots = new int[5];


	public string mostFrags;
	public string mostHits;
	public string mostDeath;
	public string mostDash;
	public string mostShots;


	// Use this for initialization
	void Start () 
	{
		GetPlayersEvents ();
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	public void GetPlayersEvents ()
	{
		GlobalVariables.Instance.Player1.GetComponent<PlayersGameplay> ().OnDash += DashPlayer1;
		GlobalVariables.Instance.Player2.GetComponent<PlayersGameplay> ().OnDash += DashPlayer2;
		GlobalVariables.Instance.Player3.GetComponent<PlayersGameplay> ().OnDash += DashPlayer3;
		GlobalVariables.Instance.Player4.GetComponent<PlayersGameplay> ().OnDash += DashPlayer4;

		GlobalVariables.Instance.Player1.GetComponent<PlayersGameplay> ().OnShoot += ShotPlayer1;
		GlobalVariables.Instance.Player2.GetComponent<PlayersGameplay> ().OnShoot += ShotPlayer2;
		GlobalVariables.Instance.Player3.GetComponent<PlayersGameplay> ().OnShoot += ShotPlayer3;
		GlobalVariables.Instance.Player4.GetComponent<PlayersGameplay> ().OnShoot += ShotPlayer4;
	}

	public void PlayersFragsAndHits (GameObject playerThatThrew, GameObject playerHit)
	{
		PlayersFrags [4]++;
		PlayersHits [4]++;

		switch (playerThatThrew.name)
		{
		case "Player 1":
			PlayersFrags [0]++;
			break;
		case "Player 2":
			PlayersFrags [1]++;
			break;
		case "Player 3":
			PlayersFrags [2]++;
			break;
		case "Player 4":
			PlayersFrags [3]++;
			break;
		}

		switch (playerHit.name)
		{
		case "Player 1":
			PlayersHits [0]++;
			break;
		case "Player 2":
			PlayersHits [1]++;
			break;
		case "Player 3":
			PlayersHits [2]++;
			break;
		case "Player 4":
			PlayersHits [3]++;
			break;
		}

		StatsUpdate ();
	}

	void DashPlayer1 ()
	{
		PlayersDash [0]++;

		PlayersDash [4]++;

		StatsUpdate ();
	}

	void DashPlayer2 ()
	{
		PlayersDash [1]++;

		PlayersDash [4]++;

		StatsUpdate ();
	}

	void DashPlayer3 ()
	{
		PlayersDash [2]++;

		PlayersDash [4]++;

		StatsUpdate ();
	}

	void DashPlayer4 ()
	{
		PlayersDash [3]++;

		PlayersDash [4]++;

		StatsUpdate ();
	}

	void ShotPlayer1 ()
	{
		PlayersShots [0]++;

		PlayersShots [4]++;

		StatsUpdate ();
	}

	void ShotPlayer2 ()
	{
		PlayersShots [1]++;

		PlayersShots [4]++;

		StatsUpdate ();
	}

	void ShotPlayer3 ()
	{
		PlayersShots [2]++;

		PlayersShots [4]++;

		StatsUpdate ();
	}

	void ShotPlayer4 ()
	{
		PlayersShots [3]++;

		PlayersShots [4]++;

		StatsUpdate ();
	}

	public void StatsUpdate ()
	{
		int frags = 0;

		for(int i = 0; i < PlayersFrags.Length; i++)
		{
			if (PlayersFrags [i] > frags)
			{
				frags = PlayersFrags [i];

				switch (i)
				{
				case 0:
					mostFrags = "Player 1";
					break;
				case 1:
					mostFrags = "Player 2";
					break;
				case 2:
					mostFrags = "Player 3";
					break;
				case 3:
					mostFrags = "Player 4";
					break;
				}
			}
				
		}

		int hits = 0;

		for(int i = 0; i < PlayersHits.Length; i++)
		{
			if (PlayersHits [i] > hits)
			{
				hits = PlayersHits [i];

				switch (i)
				{
				case 0:
					mostHits = "Player 1";
					break;
				case 1:
					mostHits = "Player 2";
					break;
				case 2:
					mostHits = "Player 3";
					break;
				case 3:
					mostHits = "Player 4";
					break;
				}
			}

		}

		int deaths = 0;

		for(int i = 0; i < PlayersDeath.Length; i++)
		{
			if (PlayersDeath [i] > deaths)
			{
				deaths = PlayersDeath [i];

				switch (i)
				{
				case 0:
					mostDeath = "Player 1";
					break;
				case 1:
					mostDeath = "Player 2";
					break;
				case 2:
					mostDeath = "Player 3";
					break;
				case 3:
					mostDeath = "Player 4";
					break;
				}
			}

		}

		int dashs = 0;

		for(int i = 0; i < PlayersDash.Length; i++)
		{
			if (PlayersDash [i] > dashs)
			{
				dashs = PlayersDash [i];

				switch (i)
				{
				case 0:
					mostDash = "Player 1";
					break;
				case 1:
					mostDash = "Player 2";
					break;
				case 2:
					mostDash = "Player 3";
					break;
				case 3:
					mostDash = "Player 4";
					break;
				}
			}

		}

		int shots = 0;

		for(int i = 0; i < PlayersShots.Length; i++)
		{
			if (PlayersShots [i] > shots)
			{
				shots = PlayersShots [i];

				switch (i)
				{
				case 0:
					mostShots = "Player 1";
					break;
				case 1:
					mostShots = "Player 2";
					break;
				case 2:
					mostShots = "Player 3";
					break;
				case 3:
					mostShots = "Player 4";
					break;
				}
			}

		}
	}
}
