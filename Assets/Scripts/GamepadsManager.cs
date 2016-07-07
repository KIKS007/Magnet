using UnityEngine;
using System.Collections;
using XboxCtrlrInput;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class GamepadsManager : Singleton<GamepadsManager>
{
	public GameObject[] gamepadLine;

	[Header ("Number of Gamepads")]
	public int numberOfGamepads;

	[Header ("Gamepads plugged at Start")]
	public bool gamepad1PluggedAtStart;
	public bool gamepad2PluggedAtStart;
	public bool gamepad3PluggedAtStart;
	public bool gamepad4PluggedAtStart;

	[Header ("Gamepad plugged")]
	public string gamepad1;
	public string gamepad2;
	public string gamepad3;
	public string gamepad4;

	[Header ("Gamepad unplugged")]
	public bool gamepad1Unplugged;
	public bool gamepad2Unplugged;
	public bool gamepad3Unplugged;
	public bool gamepad4Unplugged;
	
	private Color originalColorGamepad1;
	private Color originalColorGamepad2;
	private Color originalColorGamepad3;
	private Color originalColorGamepad4;

	public GameObject[] players;

	void OnLevelWasLoaded () 
	{		
		GetPlayers ();

		if(XCI.IsPluggedIn(1))
			gamepad1PluggedAtStart = true;
		
		if(XCI.IsPluggedIn(2))
			gamepad2PluggedAtStart = true;
		
		if(XCI.IsPluggedIn(3))
			gamepad3PluggedAtStart = true;
		
		if(XCI.IsPluggedIn(4))
			gamepad4PluggedAtStart = true;


	}

	// Use this for initialization
	void Start () 
	{
		//GetPlayers ();

		if(StaticVariables.Instance.FirstGameLaunch)
		{
			GetGamepadsPlugged ();
			UpdateStaticVariables ();
			StaticVariables.Instance.FirstGameLaunch = false;
			StaticVariables.Instance.ParticulesClonesParent = GameObject.FindGameObjectWithTag ("ParticulesClonesParent").transform;
		}


		if(XCI.IsPluggedIn(1))
			gamepad1PluggedAtStart = true;

		if(XCI.IsPluggedIn(2))
			gamepad2PluggedAtStart = true;
	
		if(XCI.IsPluggedIn(3))
			gamepad3PluggedAtStart = true;
		
		if(XCI.IsPluggedIn(4))
			gamepad4PluggedAtStart = true;

	}

	// Update is called once per frame
	void Update () 
	{
		numberOfGamepads = XCI.GetNumPluggedCtrlrs();

		GamepadUnplugged ();
	}
		
	void GamepadUnplugged ()
	{
		if(gamepad1PluggedAtStart && XCI.IsPluggedIn(1) == false)
		{
			gamepad1Unplugged = true;
		}
		else if (gamepad1PluggedAtStart && XCI.IsPluggedIn(1) == true)
		{
			gamepad1Unplugged = false;
		}

		if(gamepad2PluggedAtStart && XCI.IsPluggedIn(2) == false)
		{
			gamepad2Unplugged = true;
			
		}
		else if (gamepad2PluggedAtStart && XCI.IsPluggedIn(2) == true)
		{
			gamepad2Unplugged = false;
		}

		if(gamepad3PluggedAtStart && XCI.IsPluggedIn(3) == false)
		{
			gamepad3Unplugged = true;
		}
		else if (gamepad3PluggedAtStart && XCI.IsPluggedIn(3) == true)
		{
			gamepad3Unplugged = false;
		}

		if(gamepad4PluggedAtStart && XCI.IsPluggedIn(4) == false)
		{
			gamepad4Unplugged = true;
		}
		else if (gamepad4PluggedAtStart && XCI.IsPluggedIn(4) == true)
		{
			gamepad4Unplugged = false;
		}
			
	}

	void GetPlayers ()
	{
		players = GameObject.FindGameObjectsWithTag("Player").OrderBy( go => go.name ).ToArray();

		StaticVariables.Instance.Player1 = players[0];
		StaticVariables.Instance.Player2 = players[1];
		StaticVariables.Instance.Player3 = players[2];
		StaticVariables.Instance.Player4 = players[3];
	}

	void GetGamepadsPlugged ()
	{
		if(XCI.IsPluggedIn(1) && !XCI.IsPluggedIn(2) && !XCI.IsPluggedIn(3) && !XCI.IsPluggedIn(4))
		{
			StaticVariables.Instance.ControllerNumberPlayer1 = 0;
			StaticVariables.Instance.Player1.GetComponent<PlayersGameplay> ().controllerNumber = 0;

			StaticVariables.Instance.ControllerNumberPlayer2 = 1;
			StaticVariables.Instance.Player2.GetComponent<PlayersGameplay> ().controllerNumber = 1;

			EnablePlayers (2);
		}

		else if(XCI.IsPluggedIn(1) && XCI.IsPluggedIn(2) && !XCI.IsPluggedIn(3) && !XCI.IsPluggedIn(4))
		{
			StaticVariables.Instance.ControllerNumberPlayer1 = 0;
			StaticVariables.Instance.Player1.GetComponent<PlayersGameplay> ().controllerNumber = 0;

			StaticVariables.Instance.ControllerNumberPlayer2 = 1;
			StaticVariables.Instance.Player2.GetComponent<PlayersGameplay> ().controllerNumber = 1;

			StaticVariables.Instance.ControllerNumberPlayer3 = 2;
			StaticVariables.Instance.Player3.GetComponent<PlayersGameplay> ().controllerNumber = 2;

			EnablePlayers (3);
		}

		else if(XCI.IsPluggedIn(1) && XCI.IsPluggedIn(2) && XCI.IsPluggedIn(3) && !XCI.IsPluggedIn(4))
		{
			StaticVariables.Instance.ControllerNumberPlayer1 = 0;
			StaticVariables.Instance.Player1.GetComponent<PlayersGameplay> ().controllerNumber = 0;

			StaticVariables.Instance.ControllerNumberPlayer2 = 1;
			StaticVariables.Instance.Player2.GetComponent<PlayersGameplay> ().controllerNumber = 1;

			StaticVariables.Instance.ControllerNumberPlayer3 = 2;
			StaticVariables.Instance.Player3.GetComponent<PlayersGameplay> ().controllerNumber = 2;

			StaticVariables.Instance.ControllerNumberPlayer4 = 3;
			StaticVariables.Instance.Player4.GetComponent<PlayersGameplay> ().controllerNumber = 3;

			EnablePlayers (4);
		}

		else if(XCI.IsPluggedIn(1) && XCI.IsPluggedIn(2) && XCI.IsPluggedIn(3) && XCI.IsPluggedIn(4))
		{
			StaticVariables.Instance.ControllerNumberPlayer1 = 1;
			StaticVariables.Instance.Player1.GetComponent<PlayersGameplay> ().controllerNumber = 1;

			StaticVariables.Instance.ControllerNumberPlayer2 = 2;
			StaticVariables.Instance.Player2.GetComponent<PlayersGameplay> ().controllerNumber = 2;

			StaticVariables.Instance.ControllerNumberPlayer3 = 3;
			StaticVariables.Instance.Player3.GetComponent<PlayersGameplay> ().controllerNumber = 3;

			StaticVariables.Instance.ControllerNumberPlayer4 = 4;
			StaticVariables.Instance.Player4.GetComponent<PlayersGameplay> ().controllerNumber = 4;

			EnablePlayers (4);
		}

		else
		{
			StaticVariables.Instance.ControllerNumberPlayer1 = 0;
			StaticVariables.Instance.Player1.GetComponent<PlayersGameplay> ().controllerNumber = 0;

			EnablePlayers (1);
		}

	}

	void EnablePlayers (int enabledPlayers)
	{
		switch (enabledPlayers) {
		case 1:
			StaticVariables.Instance.Player1.SetActive (true);
			break;
		case 2:
			StaticVariables.Instance.Player1.SetActive (true);
			StaticVariables.Instance.Player2.SetActive (true);
			break;
		case 3:
			StaticVariables.Instance.Player1.SetActive (true);
			StaticVariables.Instance.Player2.SetActive (true);
			StaticVariables.Instance.Player3.SetActive (true);
			break;
		case 4:
			StaticVariables.Instance.Player1.SetActive (true);
			StaticVariables.Instance.Player2.SetActive (true);
			StaticVariables.Instance.Player3.SetActive (true);
			StaticVariables.Instance.Player4.SetActive (true);
			break;
		}
	}
  
	void UpdateStaticVariables ()
		{
			StaticVariables.Instance.NumberOfPlayers = 0;
			StaticVariables.Instance.NumberOfDisabledPlayers = 0;

			if (StaticVariables.Instance.ControllerNumberPlayer1 != -1)
				StaticVariables.Instance.NumberOfPlayers++;
			else
				StaticVariables.Instance.NumberOfDisabledPlayers++;

			if (StaticVariables.Instance.ControllerNumberPlayer2 != -1)
				StaticVariables.Instance.NumberOfPlayers++;
			else
				StaticVariables.Instance.NumberOfDisabledPlayers++;

			if (StaticVariables.Instance.ControllerNumberPlayer3 != -1)
				StaticVariables.Instance.NumberOfPlayers++;
			else
				StaticVariables.Instance.NumberOfDisabledPlayers++;

			if (StaticVariables.Instance.ControllerNumberPlayer4 != -1)
				StaticVariables.Instance.NumberOfPlayers++;
			else
				StaticVariables.Instance.NumberOfDisabledPlayers++;


			/*Debug.Log (StaticVariables.Instance.ControllerNumberPlayer1);
		Debug.Log (StaticVariables.Instance.ControllerNumberPlayer2);
		Debug.Log (StaticVariables.Instance.ControllerNumberPlayer3);
		Debug.Log (StaticVariables.Instance.ControllerNumberPlayer4);*/
		}
}
