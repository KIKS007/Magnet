using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rewired;
using UnityEngine.SceneManagement;

public class GlobalVariables : Singleton<GlobalVariables>
{
	[Header ("Controller Numbers")]
	public int ControllerNumberPlayer1 = -1;
	public int ControllerNumberPlayer2 = -1;
	public int ControllerNumberPlayer3 = -1;
	public int ControllerNumberPlayer4 = -1;

	[Header ("Players")]
	public GameObject Player1;
	public GameObject Player2;
	public GameObject Player3;
	public GameObject Player4;
	public List<GameObject> EnabledPlayersList = new List<GameObject>();

	[Header ("Game State")]
	public bool GamePaused = true;
	public bool GameOver = true;
	public bool FirstGameLaunch = true;

	[Header ("Players States")]
	public int NumberOfPlayers;

	public int NumberOfDisabledPlayers;

	public int[] TeamChoice = new int[] {-1, -1, -1, -1};

	public List<GameObject> Team1 = new List<GameObject>();
	public List<GameObject> Team2 = new List<GameObject>();
	public List<GameObject> Team3 = new List<GameObject>();
	public List<GameObject> Team4 = new List<GameObject>();

	[Header ("Particles Prefab")]
	public GameObject HitParticles;
	public GameObject WallHitParticles;
	public GameObject DeadParticles;
	public GameObject MovableExplosion;
	public GameObject PlayerSpawnParticles;
	public Transform ParticulesClonesParent;


	[Header ("Others")]
	public string firstSceneToLoad = "Hit";
	public string CurrentModeLoaded = "";

	void Start ()
	{
		if(SceneManager.GetActiveScene().name == "Scene Testing")
		{
			GamePaused = false;
			GameOver = false;
		}

		ParticulesClonesParent = GameObject.FindGameObjectWithTag ("ParticulesClonesParent").transform;
	}

	public IEnumerator ListPlayers ()
	{
		while(ControllerNumberPlayer1 == -1)
		{
			yield return null;
		}

		EnabledPlayersList.Clear ();

		if (ControllerNumberPlayer1 != -1 && !EnabledPlayersList.Contains (Player1))
			EnabledPlayersList.Add (Player1);

		if (ControllerNumberPlayer2 != -1 && !EnabledPlayersList.Contains (Player2))
			EnabledPlayersList.Add (Player2);

		if (ControllerNumberPlayer3 != -1 && !EnabledPlayersList.Contains (Player3))
			EnabledPlayersList.Add (Player3);

		if (ControllerNumberPlayer4 != -1 && !EnabledPlayersList.Contains (Player4))
			EnabledPlayersList.Add (Player4);


		if (ControllerNumberPlayer1 == -1 && EnabledPlayersList.Contains (Player1))
			EnabledPlayersList.Remove (Player1);

		if (ControllerNumberPlayer2 == -1 && EnabledPlayersList.Contains (Player2))
			EnabledPlayersList.Remove (Player2);

		if (ControllerNumberPlayer3 == -1 && EnabledPlayersList.Contains (Player3))
			EnabledPlayersList.Remove (Player3);

		if (ControllerNumberPlayer4 == -1 && EnabledPlayersList.Contains (Player4))
			EnabledPlayersList.Remove (Player4);

		PlayersNumber ();
	}

	void PlayersNumber ()
	{
		NumberOfPlayers = EnabledPlayersList.Count;
		NumberOfDisabledPlayers = 4 - NumberOfPlayers;
	}
}
