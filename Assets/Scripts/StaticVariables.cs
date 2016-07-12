using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StaticVariables : Singleton<StaticVariables>
{
	public int ControllerNumberPlayer1 = -1;
	public int ControllerNumberPlayer2 = -1;
	public int ControllerNumberPlayer3 = -1;
	public int ControllerNumberPlayer4 = -1;

	public GameObject Player1;
	public GameObject Player2;
	public GameObject Player3;
	public GameObject Player4;

	public bool GamePaused = false;

	public bool GameOver = true;

	public bool FirstGameLaunch = true;

	public int NumberOfPlayers;

	public int NumberOfDisabledPlayers;

	public int[] TeamChoice = new int[] {-1, -1, -1, -1};

	public List<GameObject> Team1 = new List<GameObject>();
	public List<GameObject> Team2 = new List<GameObject>();
	public List<GameObject> Team3 = new List<GameObject>();
	public List<GameObject> Team4 = new List<GameObject>();

	public string CurrentModeLoaded = "";

	public Transform ParticulesClonesParent;

	public GameObject HitParticles;
	public GameObject WallHitParticles;
	public GameObject DeadParticles;
	public GameObject MovableExplosion;

}
