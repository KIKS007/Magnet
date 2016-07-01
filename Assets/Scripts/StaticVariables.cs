using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class StaticVariables 
{
	public static int ControllerNumberPlayer1 = -1;
	public static int ControllerNumberPlayer2 = -1;
	public static int ControllerNumberPlayer3 = -1;
	public static int ControllerNumberPlayer4 = -1;

	public static GameObject Player1;
	public static GameObject Player2;
	public static GameObject Player3;
	public static GameObject Player4;

	public static bool GamePaused = true;

	public static bool GameOver = true;

	public static bool FirstGameLaunch = true;

	public static int NumberOfPlayers;

	public static int NumberOfDisabledPlayers;

	public static int[] TeamChoice = new int[] {-1, -1, -1, -1};

	public static List<GameObject> Team1 = new List<GameObject>();
	public static List<GameObject> Team2 = new List<GameObject>();
	public static List<GameObject> Team3 = new List<GameObject>();
	public static List<GameObject> Team4 = new List<GameObject>();

	public static string CurrentModeLoaded = "";

	public static Transform ParticulesClonesParent;
}
