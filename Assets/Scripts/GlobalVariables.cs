using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

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

	[Header ("Game State")]
	public bool GamePaused = false;
	public bool GameOver = false;
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
	public string CurrentModeLoaded = "";
}
