using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;

public class GamepadsManager : Singleton<GamepadsManager>
{
	public event EventHandler OnGamepadConnectionChange;

	[Header ("Number of Gamepads")]
	public int numberOfGamepads;

	[Header ("Gamepads plugged at Start")]
	public bool[] gamepadsPluggedAtStart = new bool[4] {false, false, false, false};

	[Header ("Gamepad unplugged")]
	public bool[] gamepadsUnplugged = new bool[4] {false, false, false, false};


	// Use this for initialization
	void Awake () 
	{
		ReInput.ControllerConnectedEvent += OnGamepadConnectedOrDisconneted;
		ReInput.ControllerPreDisconnectEvent += OnGamepadConnectedOrDisconneted;

		ReInput.ControllerPreDisconnectEvent += GamepadUnplugged;

		ReInput.ControllerConnectedEvent += CheckIfGamepadReconnected;

		GlobalVariables.Instance.OnModeStarted += ResetUnpluggedArray;
		GlobalVariables.Instance.OnModeStarted += FindGamepadsPluggedAtStart;

		GlobalVariables.Instance.OnGameOver += ResetUnpluggedArray;

		FindGamepadsPluggedAtStart ();
		SetupPlayersAndControllers ();
	}

	void OnGamepadConnectedOrDisconneted (ControllerStatusChangedEventArgs args)
	{
		if (OnGamepadConnectionChange != null)
			OnGamepadConnectionChange ();

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

	void CheckIfGamepadReconnected (ControllerStatusChangedEventArgs arg)
	{
		switch (arg.name)
		{
		case "XInput Gamepad 1":
			if(gamepadsPluggedAtStart[0])
				gamepadsUnplugged[0] = false;
			break;
		case "XInput Gamepad 2":
			if(gamepadsPluggedAtStart[1])
				gamepadsUnplugged[1] = false;
			break;
		case "XInput Gamepad 3":
			if(gamepadsPluggedAtStart[2])
				gamepadsUnplugged[2] = false;
			break;
		case "XInput Gamepad 4":
			if(gamepadsPluggedAtStart[3])
				gamepadsUnplugged[3] = false;
			break;
		}
	}

	void GamepadUnplugged (ControllerStatusChangedEventArgs arg)
	{
		switch (arg.name)
		{
		case "XInput Gamepad 1":
			if(gamepadsPluggedAtStart[0])
				gamepadsUnplugged[0] = true;
			break;
		case "XInput Gamepad 2":
			if(gamepadsPluggedAtStart[1])
				gamepadsUnplugged[1] = true;
			break;
		case "XInput Gamepad 3":
			if(gamepadsPluggedAtStart[2])
				gamepadsUnplugged[2] = true;
			break;
		case "XInput Gamepad 4":
			if(gamepadsPluggedAtStart[3])
				gamepadsUnplugged[3] = true;
			break;
		}

		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			if (gamepadsUnplugged [0] || gamepadsUnplugged [1] || gamepadsUnplugged [2] || gamepadsUnplugged [3])
				PauseGame ();
		}
	}

	void PauseGame ()
	{
		GameObject.FindGameObjectWithTag ("MainMenuManager").GetComponent<MainMenuManagerScript> ().GamePauseResumeVoid ();
	}

	void ResetUnpluggedArray ()
	{
		for (int i = 0; i < gamepadsUnplugged.Length; i++)
			gamepadsUnplugged [i] = false;
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
