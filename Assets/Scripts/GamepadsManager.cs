using UnityEngine;
using System.Collections;
using XboxCtrlrInput;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;

public class GamepadsManager : Singleton<GamepadsManager>
{
	public event EventHandler OnGamepadConnectionChange;

	public GameObject[] gamepadLine;

	[Header ("Number of Gamepads")]
	public int numberOfGamepads;

	[Header ("Gamepads plugged at Start")]
	public bool[] gamepadsPluggedAtStart = new bool[4] {false, false, false, false};

	[Header ("Gamepad unplugged")]
	public bool[] gamepadsUnplugged = new bool[4] {false, false, false, false};


	void OnLevelWasLoaded () 
	{	
		FindGamepadsPluggedAtStart ();
	}

	// Use this for initialization
	void Awake () 
	{
		ReInput.ControllerConnectedEvent += OnGamepadConnectedOrDisconneted;
		ReInput.ControllerDisconnectedEvent += OnGamepadConnectedOrDisconneted;

		FindGamepadsPluggedAtStart ();
		SetupPlayersAndControllers ();
	}

	void OnGamepadConnectedOrDisconneted (ControllerStatusChangedEventArgs args)
	{
		if (OnGamepadConnectionChange != null)
			OnGamepadConnectionChange ();

		GamepadUnplugged ();

		numberOfGamepads = ReInput.controllers.joystickCount;
	}

	void FindGamepadsPluggedAtStart ()
	{
		for (int i = 0; i < gamepadsPluggedAtStart.Length; i++)
			gamepadsPluggedAtStart [i] = false;

		foreach(Joystick j in ReInput.controllers.Joysticks)
		{
			if (j.name == "XInput Gamepad 1")
				gamepadsPluggedAtStart [0] = true;

			if (j.name == "XInput Gamepad 2")
				gamepadsPluggedAtStart [1] = true;

			if (j.name == "XInput Gamepad 3")
				gamepadsPluggedAtStart [2] = true;

			if (j.name == "XInput Gamepad 4")
				gamepadsPluggedAtStart [3] = true;
		}
	}
		
	void GamepadUnplugged ()
	{
		if(gamepadsPluggedAtStart[0] && XCI.IsPluggedIn(1) == false)
		{
			gamepadsUnplugged[0] = true;
		}
		else if (gamepadsPluggedAtStart[0] && XCI.IsPluggedIn(1) == true)
		{
			gamepadsUnplugged[0] = false;
		}

		if(gamepadsPluggedAtStart[1] && XCI.IsPluggedIn(2) == false)
		{
			gamepadsUnplugged[1] = true;
			
		}
		else if (gamepadsPluggedAtStart[1] && XCI.IsPluggedIn(2) == true)
		{
			gamepadsUnplugged[1] = false;
		}

		if(gamepadsPluggedAtStart[2] && XCI.IsPluggedIn(3) == false)
		{
			gamepadsUnplugged[2] = true;
		}
		else if (gamepadsPluggedAtStart[2] && XCI.IsPluggedIn(3) == true)
		{
			gamepadsUnplugged[2] = false;
		}

		if(gamepadsPluggedAtStart[3] && XCI.IsPluggedIn(4) == false)
		{
			gamepadsUnplugged[3] = true;
		}
		else if (gamepadsPluggedAtStart[3] && XCI.IsPluggedIn(4) == true)
		{
			gamepadsUnplugged[3] = false;
		}
			
	}

	void SetupPlayersAndControllers ()
	{
		GlobalVariables.Instance.ControllerNumberPlayer1 = 0;

		for(int i = 0; i < 4; i++)
		{
			if (gamepadsPluggedAtStart[i])
			{
				SetControllerNumber (i);
			}
		}
	}

	void SetControllerNumber (int whichNumber)
	{
		switch (whichNumber) 
		{
		case 0:
			GlobalVariables.Instance.ControllerNumberPlayer2 = 1;
			break;
		case 1:
			GlobalVariables.Instance.ControllerNumberPlayer3 = 2;
			break;
		case 2:
			GlobalVariables.Instance.ControllerNumberPlayer4 = 3;
			break;
		case 3:
			break;
		}
	}
}
