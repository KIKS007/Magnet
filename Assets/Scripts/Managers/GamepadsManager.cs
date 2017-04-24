using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;
using System;
using System.Configuration;

public enum WhichGamepadType {Xbox1, Xbox2, Xbox3, Xbox4, Custom};


public class GamepadsManager : Singleton<GamepadsManager>
{
	public event EventHandler GamepadsChange;

	[Header ("Number of Gamepads")]
	public int numberOfGamepads;

	[Header ("Gamepads List")]
	public List<Gamepad> gamepadsList = new List<Gamepad> ();

	// Use this for initialization
	void Awake () 
	{
		SetupPlayersAndControllers ();
	}

	//Setup Players Controllers 
	void SetupPlayersAndControllers ()
	{
		if (SceneManager.GetActiveScene ().name == "Scene Testing")
		{
			GlobalVariables.Instance.PlayersControllerNumber [0] = 0;

			for(int i = 0; i < ReInput.controllers.joystickCount; i++)
			{
				if (i == 3)
					break;

				GlobalVariables.Instance.PlayersControllerNumber [i + 1] = i + 1;
			}
		}
	}

	void Update ()
	{
		numberOfGamepads = ReInput.controllers.joystickCount;
	}

	void SetupGamepads ()
	{
//		if (j.name == "XInput Gamepad 1" || j.name == "XInput Gamepad 2" || j.name == "XInput Gamepad 3" || j.name == "XInput Gamepad 4")
//		{
//			gamepadsList.Add (new Gamepad());
//			gamepadsList [gamepadsList.Count - 1].GamepadController = ReInput.controllers.GetController(ControllerType.Joystick, j.id);
//			gamepadsList [gamepadsList.Count - 1].GamepadName = j.name;
//			gamepadsList [gamepadsList.Count - 1].GamepadIsDiconnected = false;
//			gamepadsList [gamepadsList.Count - 1].GamepadRewiredId = j.id;
//		}


	}

	void CheckIfGamepadReconnected (ControllerStatusChangedEventArgs arg)
	{
		SetupGamepads ();
		
		GlobalVariables.Instance.SetupRewiredPlayers ();

		if (GamepadsChange != null)
			GamepadsChange ();
	}

	//Check Which Gamepad Unplugged
	void GamepadUnplugged (ControllerStatusChangedEventArgs arg)
	{

		if (GamepadsChange != null)
			GamepadsChange ();
	}
}