using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisplayStaticVariables : MonoBehaviour 
{
	public int ControllerNumberPlayer1 = -1;
	public int ControllerNumberPlayer2 = -1;
	public int ControllerNumberPlayer3 = -1;
	public int ControllerNumberPlayer4 = -1;

	[Space (15)]

	public GameObject Player1;
	public GameObject Player2;
	public GameObject Player3;
	public GameObject Player4;

	[Space (15)]

	public bool GamePaused = true;

	public bool GameOver = true;

	public bool FirstGameLaunch = true;

	[Space (15)]

	public int NumberOfPlayers;

	public int NumberOfDisabledPlayers;

	[Space (15)]

	public int[] TeamChoice = new int[] {-1, -1, -1, -1};

	[Space (15)]

	public List<GameObject> Team1 = new List<GameObject>();
	public List<GameObject> Team2 = new List<GameObject>();
	public List<GameObject> Team3 = new List<GameObject>();
	public List<GameObject> Team4 = new List<GameObject>();

	[Space (15)]

	public string CurrentModeLoaded = "";

	public Transform ParticulesClonesParent;
	
	// Update is called once per frame
	void Update () 
	{
		ControllerNumberPlayer1 = StaticVariables.ControllerNumberPlayer1;
		ControllerNumberPlayer2 = StaticVariables.ControllerNumberPlayer2;
		ControllerNumberPlayer3 = StaticVariables.ControllerNumberPlayer3;
		ControllerNumberPlayer4 = StaticVariables.ControllerNumberPlayer4;

		Player1 = StaticVariables.Player1;
		Player2 = StaticVariables.Player2;
		Player3 = StaticVariables.Player3;
		Player4 = StaticVariables.Player4;

		GamePaused = StaticVariables.GamePaused;

		GameOver = StaticVariables.GameOver;

		FirstGameLaunch = StaticVariables.FirstGameLaunch;

		NumberOfPlayers = StaticVariables.NumberOfPlayers;

		NumberOfDisabledPlayers = StaticVariables.NumberOfDisabledPlayers;

		TeamChoice = StaticVariables.TeamChoice;

		Team1 = StaticVariables.Team1;
		Team2 = StaticVariables.Team2;
		Team3 = StaticVariables.Team3;
		Team4 = StaticVariables.Team4;

		CurrentModeLoaded = StaticVariables.CurrentModeLoaded;

		ParticulesClonesParent = StaticVariables.ParticulesClonesParent;
	}
}
