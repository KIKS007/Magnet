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

	public bool gamepadIdControl = false;

	[Header ("Number of Gamepads")]
	public int numberOfGamepads;

	[Header ("Gamepads plugged at Start")]
	public bool[] gamepadsPluggedAtStart = new bool[4] {false, false, false, false};

	[Header ("Gamepad unplugged")]
	public bool[] gamepadsUnplugged = new bool[4] {false, false, false, false};

	[Header ("Gamepads List")]
	public List<Gamepad> gamepadsList = new List<Gamepad> ();

	// Use this for initialization
	void Awake () 
	{
		ReInput.ControllerConnectedEvent += CheckIfGamepadReconnected;

		LoadModeManager.Instance.OnLevelLoaded += ResetUnpluggedArray;

		GlobalVariables.Instance.OnStartMode += () => {
			ResetUnpluggedArray ();
			FindGamepadsPluggedAtStart ();
		};

		GlobalVariables.Instance.OnRestartMode += () => {
			ResetUnpluggedArray ();
			FindGamepadsPluggedAtStart ();
		};

		ReInput.ControllerDisconnectedEvent += (ControllerStatusChangedEventArgs obj) => 
		{
			GamepadUnplugged (obj);

			if(GlobalVariables.Instance.GameState == GameStateEnum.Menu)
			{
				ResetUnpluggedArray ();
				FindGamepadsPluggedAtStart ();
			}
		};

		ReInput.ControllerConnectedEvent += (ControllerStatusChangedEventArgs obj) => 
		{
			if(GlobalVariables.Instance.GameState == GameStateEnum.Menu)
			{
				ResetUnpluggedArray ();
				FindGamepadsPluggedAtStart ();
			}
		};

		ResetUnpluggedArray ();
		FindGamepadsPluggedAtStart ();
		SetupPlayersAndControllers ();
	}

	//Setup Players Controllers 
	void SetupPlayersAndControllers ()
	{
		if (SceneManager.GetActiveScene ().name == "Scene Testing")
		{
			GlobalVariables.Instance.PlayersControllerNumber [0] = 0;

			for(int i = 0; i < 3; i++)
			{
				if (gamepadsPluggedAtStart[i])
					GlobalVariables.Instance.PlayersControllerNumber [i + 1] = i + 1;
			}
		}
		else
		{
			for(int i = 0; i < 4; i++)
			{
				if (gamepadsPluggedAtStart[i])
					GlobalVariables.Instance.PlayersControllerNumber [i] = i + 1;
			}			
		}

		//Enable Possiblity to play alone with one gamepad
		if(ReInput.controllers.GetControllerCount(ControllerType.Joystick) == 0)
			GlobalVariables.Instance.PlayersControllerNumber[1] = 1;
	}

	void Update ()
	{
		numberOfGamepads = ReInput.controllers.joystickCount;
	}

	public void FindGamepadsPluggedAtStart ()
	{
		for (int i = 0; i < gamepadsPluggedAtStart.Length; i++)
			gamepadsPluggedAtStart [i] = false;

		SetupGamepads ();

		if(!gamepadIdControl)
		{
			for (int i = 0; i < gamepadsPluggedAtStart.Length; i++)
				gamepadsPluggedAtStart [i] = false;
			
			for(int i = 0; i < ReInput.controllers.joystickCount; i++)
				if(i < 4)
					gamepadsPluggedAtStart [i] = true;
		}

		GlobalVariables.Instance.SetupRewiredPlayers ();
	}

	void ResetUnpluggedArray ()
	{
		for (int i = 0; i < gamepadsUnplugged.Length; i++)
			gamepadsUnplugged [i] = false;
	}

	void SetupGamepads ()
	{
		foreach(Joystick j in ReInput.controllers.Joysticks)
		{
			bool gamepadAlreadyContained = false;

			for(int i = 0; i < gamepadsList.Count; i++)
			{
				if (gamepadsList [i].GamepadRewiredId == j.id)
				{
					gamepadAlreadyContained = true;
					gamepadsList [i].GamepadIsDiconnected = false;

					if(gamepadIdControl)
						gamepadsPluggedAtStart [gamepadsList [i].GamepadId - 1] = true;
				}
			}

			if (!gamepadAlreadyContained)
			{
				if (j.name == "XInput Gamepad 1" || j.name == "XInput Gamepad 2" || j.name == "XInput Gamepad 3" || j.name == "XInput Gamepad 4")
				{
					gamepadsList.Add (new Gamepad());
					gamepadsList [gamepadsList.Count - 1].GamepadController = ReInput.controllers.GetController(ControllerType.Joystick, j.id);
					gamepadsList [gamepadsList.Count - 1].GamepadName = j.name;
					gamepadsList [gamepadsList.Count - 1].GamepadIsDiconnected = false;
					gamepadsList [gamepadsList.Count - 1].GamepadRewiredId = j.id;
				}

				if(j.name == "XInput Gamepad 1")
				{
					gamepadsList [gamepadsList.Count - 1].GamepadType = WhichGamepadType.Xbox1;
					gamepadsList [gamepadsList.Count - 1].GamepadId = 1;

					if(gamepadIdControl)
						gamepadsPluggedAtStart [0] = true;
				}

				else if (j.name == "XInput Gamepad 2")
				{
					gamepadsList [gamepadsList.Count - 1].GamepadType = WhichGamepadType.Xbox2;
					gamepadsList [gamepadsList.Count - 1].GamepadId = 2;

					if(gamepadIdControl)
						gamepadsPluggedAtStart [1] = true;
				}

				else if (j.name == "XInput Gamepad 3")
				{
					gamepadsList [gamepadsList.Count - 1].GamepadType = WhichGamepadType.Xbox3;
					gamepadsList [gamepadsList.Count - 1].GamepadId = 3;

					if(gamepadIdControl)
						gamepadsPluggedAtStart [2] = true;
				}

				else if (j.name == "XInput Gamepad 4")
				{
					gamepadsList [gamepadsList.Count - 1].GamepadType = WhichGamepadType.Xbox4;
					gamepadsList [gamepadsList.Count - 1].GamepadId = 4;

					if(gamepadIdControl)
						gamepadsPluggedAtStart [3] = true;
				}

				for(int i = 0; i < gamepadsList.Count; i++)
				{
					if (gamepadsList [i].GamepadId == gamepadsList [gamepadsList.Count - 1].GamepadId && gamepadsList [i].GamepadRewiredId != gamepadsList [gamepadsList.Count - 1].GamepadRewiredId)
					{
						gamepadsList.Remove (gamepadsList [i]);
					}
				}
			}
		}


		foreach(Joystick j in ReInput.controllers.Joysticks)
		{
			bool gamepadAlreadyContained = false;

			for(int i = 0; i < gamepadsList.Count; i++)
			{
				if (gamepadsList [i].GamepadRewiredId == j.id)
				{
					gamepadAlreadyContained = true;
					gamepadsList [i].GamepadIsDiconnected = false;

					if(gamepadIdControl)
						gamepadsPluggedAtStart [gamepadsList [i].GamepadId - 1] = true;
				}
			}

			if (!gamepadAlreadyContained)
			{
				gamepadsList.Add (new Gamepad());
				gamepadsList [gamepadsList.Count - 1].GamepadController = ReInput.controllers.GetController(ControllerType.Joystick, j.id);
				gamepadsList [gamepadsList.Count - 1].GamepadName = j.name;
				gamepadsList [gamepadsList.Count - 1].GamepadIsDiconnected = false;
				gamepadsList [gamepadsList.Count - 1].GamepadRewiredId = j.id;
				gamepadsList [gamepadsList.Count - 1].GamepadType = WhichGamepadType.Custom;

				if (!gamepadsPluggedAtStart [0])
				{
					if(gamepadIdControl)
						gamepadsPluggedAtStart [0] = true;
					
					gamepadsList [gamepadsList.Count - 1].GamepadId = 1;

				}
				

				else if (!gamepadsPluggedAtStart [1])
				{
					if(gamepadIdControl)
						gamepadsPluggedAtStart [1] = true;
					gamepadsList [gamepadsList.Count - 1].GamepadId = 2;

				}
				

				else if (!gamepadsPluggedAtStart [2])
				{
					if(gamepadIdControl)
						gamepadsPluggedAtStart [2] = true;
					gamepadsList [gamepadsList.Count - 1].GamepadId = 3;

				}
				

				else if (!gamepadsPluggedAtStart [3])
				{
					if(gamepadIdControl)
						gamepadsPluggedAtStart [3] = true;
					gamepadsList [gamepadsList.Count - 1].GamepadId = 4;
				}
			}
		}
	}

	void CheckIfGamepadReconnected (ControllerStatusChangedEventArgs arg)
	{
		SetupGamepads ();
		
		GlobalVariables.Instance.SetupRewiredPlayers ();

		if(gamepadIdControl)
		{
			for(int i = 0; i < gamepadsList.Count; i++)
			{
				int id = gamepadsList [i].GamepadId;
				
				if(arg.controllerId == gamepadsList[i].GamepadRewiredId)
				{
					gamepadsList [i].GamepadIsDiconnected = false;
					
					if(id != -1)
						gamepadsUnplugged[gamepadsList [i].GamepadId - 1] = false;
				}
			}			
		}
		else if(arg.controllerId < 4)
		{
			gamepadsUnplugged[arg.controllerId] = false;
		}


		if (GamepadsChange != null)
			GamepadsChange ();
	}

	//Check Which Gamepad Unplugged
	void GamepadUnplugged (ControllerStatusChangedEventArgs arg)
	{
		if(gamepadIdControl)
		{
			for(int i = 0; i < gamepadsList.Count; i++)
			{
				int id = gamepadsList [i].GamepadId;
				
				if(arg.controllerId == gamepadsList[i].GamepadRewiredId)
				{
					gamepadsList [i].GamepadIsDiconnected = true;
					
					gamepadsUnplugged[id - 1] = true;
				}
			}			
		}
		else if(arg.controllerId < 4)
		{
			gamepadsUnplugged[arg.controllerId] = true;
		}

		//Debug.Log (arg.controllerId);

		if (gamepadsUnplugged [0] || gamepadsUnplugged [1] || gamepadsUnplugged [2] || gamepadsUnplugged [3])
		{
			if(GlobalVariables.Instance.GameState == GameStateEnum.Playing)
				MenuManager.Instance.PauseResumeGame ();

			if (GlobalVariables.Instance.GameState == GameStateEnum.EndMode)
			{
				GlobalVariables.Instance.GameState = GameStateEnum.Menu;
				MenuManager.Instance.ReturnToMainMenu ();
			}
		}

		if (GamepadsChange != null)
			GamepadsChange ();
	}
}

[Serializable]
public class Gamepad
{
	public WhichGamepadType GamepadType;
	public Controller GamepadController;
	public string GamepadName;
	public int GamepadRewiredId;
	public int GamepadId = -1;
	public bool GamepadIsDiconnected = false;
}