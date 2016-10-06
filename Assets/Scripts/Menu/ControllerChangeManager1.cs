using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XboxCtrlrInput;
using DG.Tweening;
using System.Collections.Generic;
using Rewired;

public class ControllerChangeManager1 : MonoBehaviour 
{
	public event EventHandler OnControllerChange;

	public GameObject[] gamepadsConnectText = new GameObject[4];

	public RectTransform[] logoRect = new RectTransform[5];

	public RectTransform[] sliderRect = new RectTransform[5];

	public float[] imagesAlignedPos = new float[] {107.1f, 242.4f, 395.9f, 544.8f, 692.6f};

	public int[] imagesNumber = new int[] {0, 0, 0, 0, 0};

	public float durationImageMovement = 0.1f;

	public float gapBetweenInputs = 0.2f;

	public float durationColor = 0.5f;

	private bool keyboardMoving = false;
	private bool gamepad1Moving = false;
	private bool gamepad2Moving = false;
	private bool gamepad3Moving = false;
	private bool gamepad4Moving = false;

	private Color gamepad1Color;
	private Color gamepad2Color;
	private Color gamepad3Color;
	private Color gamepad4Color;
	private Color disableColor;

	public Player mouseKeyboard;
	public Player gamepad1;
	public Player gamepad2;
	public Player gamepad3;
	public Player gamepad4;

	public bool getInput = true;

	void Start ()
	{
		imagesAlignedPos [0] = logoRect [0].anchoredPosition.x;
		imagesAlignedPos [1] = logoRect [1].anchoredPosition.x;
		imagesAlignedPos [2] = logoRect [2].anchoredPosition.x;
		imagesAlignedPos [3] = logoRect [3].anchoredPosition.x;
		imagesAlignedPos [4] = logoRect [4].anchoredPosition.x;

		ReInput.ControllerConnectedEvent += GetPlayers;
		ReInput.ControllerConnectedEvent += GamepadConnectedDisplay;

		ReInput.ControllerPreDisconnectEvent += GamepadDisconnectedDisplay;
		ReInput.ControllerPreDisconnectEvent += ResetGamepadOnDisconnect;

		ReInput.ControllerPreDisconnectEvent += UpdateGlobalVariables;
		ReInput.ControllerPreDisconnectEvent += UpdatePlayersControllers;

		OnControllerChange += UpdateGlobalVariables;
		OnControllerChange += UpdatePlayersControllers;

		GetPlayersAndControllers ();

		GetPlayers ();

		GamepadDisplay ();
	}
		
	void Update ()
	{
		if(getInput)
			GetInput ();

		DisplayConnectGamepadsText ();
	}

	void DisplayConnectGamepadsText ()
	{
		for(int i = 0; i < gamepadsConnectText.Length; i++)
		{
			if(sliderRect[i+1].GetComponent<Button>().interactable == true && gamepadsConnectText [i].activeSelf == true)
				gamepadsConnectText [i].SetActive (false);

			if(sliderRect[i+1].GetComponent<Button>().interactable == false && gamepadsConnectText [i].activeSelf == false)
				gamepadsConnectText [i].SetActive (true);
		}
	}

	void GetInput ()
	{
		if (mouseKeyboard.GetAxisRaw("Move Horizontal") < 0 && !keyboardMoving)
		{
			GoOnTheLeft (0);
		}

		if (mouseKeyboard.GetAxisRaw("Move Horizontal") > 0 && !keyboardMoving)
		{
			GoOnTheRight (0);
		}


		if (gamepad1.GetAxisRaw("Move Horizontal") < 0 && !gamepad1Moving)
		{
			GoOnTheLeft (1);
		}

		if (gamepad1.GetAxisRaw("Move Horizontal") > 0 && !gamepad1Moving)
		{
			GoOnTheRight (1);
		}


		if (gamepad2.GetAxisRaw("Move Horizontal") < 0 && !gamepad2Moving)
		{
			GoOnTheLeft (2);
		}

		if (gamepad2.GetAxisRaw("Move Horizontal") > 0 && !gamepad2Moving)
		{
			GoOnTheRight (2);
		}


		if (gamepad3.GetAxisRaw("Move Horizontal") < 0 && !gamepad3Moving)
		{
			GoOnTheLeft (3);
		}

		if (gamepad3.GetAxisRaw("Move Horizontal") > 0 && !gamepad3Moving)
		{
			GoOnTheRight (3);
		}


		if (gamepad4.GetAxisRaw("Move Horizontal") < 0 && !gamepad4Moving)
		{
			GoOnTheLeft (4);
		}

		if (gamepad4.GetAxisRaw("Move Horizontal") > 0 && !gamepad4Moving)
		{
			GoOnTheRight (4);
		}
	}
		

	void OnEnable ()
	{
		getInput = true;

		if(GlobalVariables.Instance.GameState == GameStateEnum.Over)
		{
			GetPlayersAndControllers ();

			GetPlayers ();

			GamepadDisplay ();

			UpdateGlobalVariables ();

			UpdatePlayersControllers ();
		}
	}

	public void GetPlayersAndControllers ()
	{
		if(GlobalVariables.Instance.ControllerNumberPlayer1 != -1)
		{
			sliderRect [GlobalVariables.Instance.ControllerNumberPlayer1].DOLocalMoveX (imagesAlignedPos [1], durationImageMovement);
			imagesNumber [GlobalVariables.Instance.ControllerNumberPlayer1] = 1;
		}

		if(GlobalVariables.Instance.ControllerNumberPlayer2 != -1)
		{
			sliderRect [GlobalVariables.Instance.ControllerNumberPlayer2].DOLocalMoveX (imagesAlignedPos [2], durationImageMovement);
			imagesNumber [GlobalVariables.Instance.ControllerNumberPlayer2] = 2;
		}

		if(GlobalVariables.Instance.ControllerNumberPlayer3 != -1)
		{
			sliderRect [GlobalVariables.Instance.ControllerNumberPlayer3].DOLocalMoveX (imagesAlignedPos [3], durationImageMovement);
			imagesNumber [GlobalVariables.Instance.ControllerNumberPlayer3] = 3;
		}

		if(GlobalVariables.Instance.ControllerNumberPlayer4 != -1)
		{
			sliderRect [GlobalVariables.Instance.ControllerNumberPlayer4].DOLocalMoveX (imagesAlignedPos [4], durationImageMovement);
			imagesNumber [GlobalVariables.Instance.ControllerNumberPlayer4] = 4;
		}
	}

	void GetPlayers (ControllerStatusChangedEventArgs arg)
	{
		mouseKeyboard = ReInput.players.GetPlayer (0);
		gamepad1 = ReInput.players.GetPlayer (1);
		gamepad2 = ReInput.players.GetPlayer (2);
		gamepad3 = ReInput.players.GetPlayer (3);
		gamepad4 = ReInput.players.GetPlayer (4);

		gamepad1.controllers.AddController (ControllerType.Joystick, 0, true);
		gamepad2.controllers.AddController (ControllerType.Joystick, 1, true);
		gamepad3.controllers.AddController (ControllerType.Joystick, 2, true);
		gamepad4.controllers.AddController (ControllerType.Joystick, 3, true);
	}

	void GetPlayers ()
	{
		mouseKeyboard = ReInput.players.GetPlayer (0);
		gamepad1 = ReInput.players.GetPlayer (1);
		gamepad2 = ReInput.players.GetPlayer (2);
		gamepad3 = ReInput.players.GetPlayer (3);
		gamepad4 = ReInput.players.GetPlayer (4);

		gamepad1.controllers.AddController (ControllerType.Joystick, 0, true);
		gamepad2.controllers.AddController (ControllerType.Joystick, 1, true);
		gamepad3.controllers.AddController (ControllerType.Joystick, 2, true);
		gamepad4.controllers.AddController (ControllerType.Joystick, 3, true);
	}

	void GamepadConnectedDisplay (ControllerStatusChangedEventArgs arg)
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Over)
		{
			switch (arg.name)
			{
			case "XInput Gamepad 1":
				sliderRect [1].GetComponent<Button> ().interactable = true;
				break;
			case "XInput Gamepad 2":
				sliderRect [2].GetComponent<Button> ().interactable = true;
				break;
			case "XInput Gamepad 3":
				sliderRect [3].GetComponent<Button> ().interactable = true;
				break;
			case "XInput Gamepad 4":
				sliderRect [4].GetComponent<Button> ().interactable = true;
				break;
			}
		}
	}

	void GamepadDisconnectedDisplay (ControllerStatusChangedEventArgs arg)
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Over)
		{
			switch (arg.name)
			{
			case "XInput Gamepad 1":
				sliderRect [1].GetComponent<Button> ().interactable = false;
				break;
			case "XInput Gamepad 2":
				sliderRect [2].GetComponent<Button> ().interactable = false;
				break;
			case "XInput Gamepad 3":
				sliderRect [3].GetComponent<Button> ().interactable = false;
				break;
			case "XInput Gamepad 4":
				sliderRect [4].GetComponent<Button> ().interactable = false;
				break;
			}
		}
	}

	void GamepadDisplay ()
	{
		if(XCI.IsPluggedIn(1))
		{
			sliderRect [1].GetComponent<Button> ().interactable = true;
		}

		if(!XCI.IsPluggedIn(1))
		{
			sliderRect [1].GetComponent<Button> ().interactable = false;
			sliderRect [1].DOLocalMoveX (imagesAlignedPos [0], durationImageMovement);
			imagesNumber [1] = 0;
		}

		if(XCI.IsPluggedIn(2))
		{
			sliderRect [2].GetComponent<Button> ().interactable = true;
		}

		if(!XCI.IsPluggedIn(2))
		{
			sliderRect [2].GetComponent<Button> ().interactable = false;
			sliderRect [2].DOLocalMoveX (imagesAlignedPos [0], durationImageMovement);
			imagesNumber [2] = 0;
		}

		if(XCI.IsPluggedIn(3))
		{
			sliderRect [3].GetComponent<Button> ().interactable = true;
		}

		if(!XCI.IsPluggedIn(3))
		{
			sliderRect [3].GetComponent<Button> ().interactable = false;
			sliderRect [3].DOLocalMoveX (imagesAlignedPos [0], durationImageMovement);
			imagesNumber [3] = 0;

		}

		if(XCI.IsPluggedIn(4))
		{
			sliderRect [4].GetComponent<Button> ().interactable = true;
		}

		if(!XCI.IsPluggedIn(4))
		{
			sliderRect [4].GetComponent<Button> ().interactable = false;
			sliderRect [4].DOLocalMoveX (imagesAlignedPos [0], durationImageMovement);
			imagesNumber [4] = 0;

		}
	}

	public void UpdateGlobalVariables ()
	{
		EraseControllerNumbers (0);
		EraseControllerNumbers (1);
		EraseControllerNumbers (2);
		EraseControllerNumbers (3);
		EraseControllerNumbers (4);

		switch (imagesNumber[0])
		{
		case 1:
			GlobalVariables.Instance.ControllerNumberPlayer1 = 0;
			break;
		case 2:
			GlobalVariables.Instance.ControllerNumberPlayer2 = 0;
			break;
		case 3:
			GlobalVariables.Instance.ControllerNumberPlayer3 = 0;
			break;
		case 4:
			GlobalVariables.Instance.ControllerNumberPlayer4 = 0;
			break;
		}

		switch (imagesNumber[1])
		{
		case 1:
			GlobalVariables.Instance.ControllerNumberPlayer1 = 1;
			break;
		case 2:
			GlobalVariables.Instance.ControllerNumberPlayer2 = 1;
			break;
		case 3:
			GlobalVariables.Instance.ControllerNumberPlayer3 = 1;
			break;
		case 4:
			GlobalVariables.Instance.ControllerNumberPlayer4 = 1;
			break;
		}

		switch (imagesNumber[2])
		{
		case 1:
			GlobalVariables.Instance.ControllerNumberPlayer1 = 2;
			break;
		case 2:
			GlobalVariables.Instance.ControllerNumberPlayer2 = 2;
			break;
		case 3:
			GlobalVariables.Instance.ControllerNumberPlayer3 = 2;
			break;
		case 4:
			GlobalVariables.Instance.ControllerNumberPlayer4 = 2;
			break;
		}

		switch (imagesNumber[3])
		{
		case 1:
			GlobalVariables.Instance.ControllerNumberPlayer1 = 3;
			break;
		case 2:
			GlobalVariables.Instance.ControllerNumberPlayer2 = 3;
			break;
		case 3:
			GlobalVariables.Instance.ControllerNumberPlayer3 = 3;
			break;
		case 4:
			GlobalVariables.Instance.ControllerNumberPlayer4 = 3;
			break;
		}

		switch (imagesNumber[4])
		{
		case 1:
			GlobalVariables.Instance.ControllerNumberPlayer1 = 4;
			break;
		case 2:
			GlobalVariables.Instance.ControllerNumberPlayer2 = 4;
			break;
		case 3:
			GlobalVariables.Instance.ControllerNumberPlayer3 = 4;
			break;
		case 4:
			GlobalVariables.Instance.ControllerNumberPlayer4 = 4;
			break;
		}

		if(ReInput.controllers.GetControllerCount(ControllerType.Joystick) == 0)
			GlobalVariables.Instance.ControllerNumberPlayer2 = 1;


		GlobalVariables.Instance.NumberOfPlayers = 0;
		GlobalVariables.Instance.NumberOfDisabledPlayers = 0;

		if (GlobalVariables.Instance.ControllerNumberPlayer1 != -1)
		{
			GlobalVariables.Instance.NumberOfPlayers++;
		}
		else
		{
			GlobalVariables.Instance.NumberOfDisabledPlayers++;

		}

		if (GlobalVariables.Instance.ControllerNumberPlayer2 != -1)
		{
			GlobalVariables.Instance.NumberOfPlayers++;

		}
		else
		{
			GlobalVariables.Instance.NumberOfDisabledPlayers++;

		}

		if (GlobalVariables.Instance.ControllerNumberPlayer3 != -1)
		{
			GlobalVariables.Instance.NumberOfPlayers++;

		}
		else
		{
			GlobalVariables.Instance.NumberOfDisabledPlayers++;

		}

		if (GlobalVariables.Instance.ControllerNumberPlayer4 != -1)
		{
			GlobalVariables.Instance.NumberOfPlayers++;

		}
		else
		{
			GlobalVariables.Instance.NumberOfDisabledPlayers++;
		}

		GlobalVariables.Instance.ListPlayers ();
	}

	public void UpdateGlobalVariables (ControllerStatusChangedEventArgs arg)
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Over)
		{
			EraseControllerNumbers (0);
			EraseControllerNumbers (1);
			EraseControllerNumbers (2);
			EraseControllerNumbers (3);
			EraseControllerNumbers (4);

			switch (imagesNumber[0])
			{
			case 1:
				GlobalVariables.Instance.ControllerNumberPlayer1 = 0;
				break;
			case 2:
				GlobalVariables.Instance.ControllerNumberPlayer2 = 0;
				break;
			case 3:
				GlobalVariables.Instance.ControllerNumberPlayer3 = 0;
				break;
			case 4:
				GlobalVariables.Instance.ControllerNumberPlayer4 = 0;
				break;
			}

			switch (imagesNumber[1])
			{
			case 1:
				GlobalVariables.Instance.ControllerNumberPlayer1 = 1;
				break;
			case 2:
				GlobalVariables.Instance.ControllerNumberPlayer2 = 1;
				break;
			case 3:
				GlobalVariables.Instance.ControllerNumberPlayer3 = 1;
				break;
			case 4:
				GlobalVariables.Instance.ControllerNumberPlayer4 = 1;
				break;
			}

			switch (imagesNumber[2])
			{
			case 1:
				GlobalVariables.Instance.ControllerNumberPlayer1 = 2;
				break;
			case 2:
				GlobalVariables.Instance.ControllerNumberPlayer2 = 2;
				break;
			case 3:
				GlobalVariables.Instance.ControllerNumberPlayer3 = 2;
				break;
			case 4:
				GlobalVariables.Instance.ControllerNumberPlayer4 = 2;
				break;
			}

			switch (imagesNumber[3])
			{
			case 1:
				GlobalVariables.Instance.ControllerNumberPlayer1 = 3;
				break;
			case 2:
				GlobalVariables.Instance.ControllerNumberPlayer2 = 3;
				break;
			case 3:
				GlobalVariables.Instance.ControllerNumberPlayer3 = 3;
				break;
			case 4:
				GlobalVariables.Instance.ControllerNumberPlayer4 = 3;
				break;
			}

			switch (imagesNumber[4])
			{
			case 1:
				GlobalVariables.Instance.ControllerNumberPlayer1 = 4;
				break;
			case 2:
				GlobalVariables.Instance.ControllerNumberPlayer2 = 4;
				break;
			case 3:
				GlobalVariables.Instance.ControllerNumberPlayer3 = 4;
				break;
			case 4:
				GlobalVariables.Instance.ControllerNumberPlayer4 = 4;
				break;
			}


			GlobalVariables.Instance.NumberOfPlayers = 0;
			GlobalVariables.Instance.NumberOfDisabledPlayers = 0;

			if (GlobalVariables.Instance.ControllerNumberPlayer1 != -1)
			{
				GlobalVariables.Instance.NumberOfPlayers++;
			}
			else
			{
				GlobalVariables.Instance.NumberOfDisabledPlayers++;

			}

			if (GlobalVariables.Instance.ControllerNumberPlayer2 != -1)
			{
				GlobalVariables.Instance.NumberOfPlayers++;

			}
			else
			{
				GlobalVariables.Instance.NumberOfDisabledPlayers++;

			}

			if (GlobalVariables.Instance.ControllerNumberPlayer3 != -1)
			{
				GlobalVariables.Instance.NumberOfPlayers++;

			}
			else
			{
				GlobalVariables.Instance.NumberOfDisabledPlayers++;

			}

			if (GlobalVariables.Instance.ControllerNumberPlayer4 != -1)
			{
				GlobalVariables.Instance.NumberOfPlayers++;

			}
			else
			{
				GlobalVariables.Instance.NumberOfDisabledPlayers++;
			}

			GlobalVariables.Instance.ListPlayers ();

			/*Debug.Log (GlobalVariables.Instance.ControllerNumberPlayer1);
		Debug.Log (GlobalVariables.Instance.ControllerNumberPlayer2);
		Debug.Log (GlobalVariables.Instance.ControllerNumberPlayer3);
		Debug.Log (GlobalVariables.Instance.ControllerNumberPlayer4);*/

		}
	}


	void ResetGamepadOnDisconnect (ControllerStatusChangedEventArgs arg)
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Over)
		{
			switch (arg.name)
			{
			case "XInput Gamepad 1":
				sliderRect [1].DOLocalMoveX (imagesAlignedPos [0], durationImageMovement);
				imagesNumber [1] = 0;
				EraseControllerNumbers (1);
				break;
			case "XInput Gamepad 2":
				sliderRect [2].DOLocalMoveX (imagesAlignedPos [0], durationImageMovement);
				imagesNumber [2] = 0;
				EraseControllerNumbers (2);
				break;
			case "XInput Gamepad 3":
				sliderRect [3].DOLocalMoveX (imagesAlignedPos [0], durationImageMovement);
				imagesNumber [3] = 0;
				EraseControllerNumbers (3);
				break;
			case "XInput Gamepad 4":
				sliderRect [4].DOLocalMoveX (imagesAlignedPos [0], durationImageMovement);
				imagesNumber [4] = 0;
				EraseControllerNumbers (4);
				break;
			}			
		}
	}



	void EraseControllerNumbers (int whichController)
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Over)
		{
			if (GlobalVariables.Instance.ControllerNumberPlayer1 == whichController)
				GlobalVariables.Instance.ControllerNumberPlayer1 = -1;

			if (GlobalVariables.Instance.ControllerNumberPlayer2 == whichController)
				GlobalVariables.Instance.ControllerNumberPlayer2 = -1;

			if (GlobalVariables.Instance.ControllerNumberPlayer3 == whichController)
				GlobalVariables.Instance.ControllerNumberPlayer3 = -1;

			if (GlobalVariables.Instance.ControllerNumberPlayer4 == whichController)
				GlobalVariables.Instance.ControllerNumberPlayer4 = -1;
		}
	}

	public void UpdatePlayersControllers ()
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Over && GlobalVariables.Instance.Player1 != null)
		{
			if (GlobalVariables.Instance.ControllerNumberPlayer1 != -1)
				GlobalVariables.Instance.Player1.SetActive (true);

			if (GlobalVariables.Instance.ControllerNumberPlayer2 != -1)
				GlobalVariables.Instance.Player2.SetActive (true);

			if (GlobalVariables.Instance.ControllerNumberPlayer3 != -1)
				GlobalVariables.Instance.Player3.SetActive (true);

			if (GlobalVariables.Instance.ControllerNumberPlayer4 != -1)
				GlobalVariables.Instance.Player4.SetActive (true);

			GlobalVariables.Instance.Player1.GetComponent<PlayersGameplay>().GetControllerNumber ();
			GlobalVariables.Instance.Player2.GetComponent<PlayersGameplay>().GetControllerNumber ();
			GlobalVariables.Instance.Player3.GetComponent<PlayersGameplay>().GetControllerNumber ();
			GlobalVariables.Instance.Player4.GetComponent<PlayersGameplay>().GetControllerNumber ();

			GlobalVariables.Instance.Player1.GetComponent<PlayersGameplay>().Controller ();
			GlobalVariables.Instance.Player2.GetComponent<PlayersGameplay>().Controller ();
			GlobalVariables.Instance.Player3.GetComponent<PlayersGameplay>().Controller ();
			GlobalVariables.Instance.Player4.GetComponent<PlayersGameplay>().Controller ();
		}
	}

	public void UpdatePlayersControllers (ControllerStatusChangedEventArgs arg)
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Over)
		{
			if (GlobalVariables.Instance.ControllerNumberPlayer1 != -1)
				GlobalVariables.Instance.Player1.SetActive (true);

			if (GlobalVariables.Instance.ControllerNumberPlayer2 != -1)
				GlobalVariables.Instance.Player2.SetActive (true);

			if (GlobalVariables.Instance.ControllerNumberPlayer3 != -1)
				GlobalVariables.Instance.Player3.SetActive (true);

			if (GlobalVariables.Instance.ControllerNumberPlayer4 != -1)
				GlobalVariables.Instance.Player4.SetActive (true);

			GlobalVariables.Instance.Player1.GetComponent<PlayersGameplay>().GetControllerNumber ();
			GlobalVariables.Instance.Player2.GetComponent<PlayersGameplay>().GetControllerNumber ();
			GlobalVariables.Instance.Player3.GetComponent<PlayersGameplay>().GetControllerNumber ();
			GlobalVariables.Instance.Player4.GetComponent<PlayersGameplay>().GetControllerNumber ();

			GlobalVariables.Instance.Player1.GetComponent<PlayersGameplay>().Controller ();
			GlobalVariables.Instance.Player2.GetComponent<PlayersGameplay>().Controller ();
			GlobalVariables.Instance.Player3.GetComponent<PlayersGameplay>().Controller ();
			GlobalVariables.Instance.Player4.GetComponent<PlayersGameplay>().Controller ();
		}
	}


	public void GoOnTheRight (int controllerNumber)
	{
		StartCoroutine (GapBetweenInputs (controllerNumber));

		switch (imagesNumber[controllerNumber])
		{
		case 0:
			sliderRect [controllerNumber].DOAnchorPos (new Vector2(imagesAlignedPos [1], sliderRect[controllerNumber].anchoredPosition.y), durationImageMovement);
			imagesNumber [controllerNumber] = 1;
			break;
		case 1 :
			sliderRect [controllerNumber].DOAnchorPos (new Vector2(imagesAlignedPos [2], sliderRect[controllerNumber].anchoredPosition.y), durationImageMovement);
			imagesNumber [controllerNumber] = 2;
			break;
		case 2 :
			sliderRect [controllerNumber].DOAnchorPos (new Vector2(imagesAlignedPos [3], sliderRect[controllerNumber].anchoredPosition.y), durationImageMovement);
			imagesNumber [controllerNumber] = 3;
			break;
		case 3 :
			sliderRect [controllerNumber].DOAnchorPos (new Vector2(imagesAlignedPos [4], sliderRect[controllerNumber].anchoredPosition.y), durationImageMovement);
			imagesNumber [controllerNumber] = 4;
			break;

		case 4 :
			break;
		case -1 :
			break;
		}

		if (OnControllerChange != null)
			OnControllerChange ();
	}

	public void GoOnTheLeft (int controllerNumber)
	{
		StartCoroutine (GapBetweenInputs (controllerNumber));

		switch (imagesNumber[controllerNumber])
		{
		case 0 :
			break;

		case 1 :
			sliderRect [controllerNumber].DOAnchorPos (new Vector2(imagesAlignedPos [0], sliderRect[controllerNumber].anchoredPosition.y), durationImageMovement);
			imagesNumber [controllerNumber] = 0;
			break;
		case 2 :
			sliderRect [controllerNumber].DOAnchorPos (new Vector2(imagesAlignedPos [1], sliderRect[controllerNumber].anchoredPosition.y), durationImageMovement);
			imagesNumber [controllerNumber] = 1;
			break;
		case 3 :
			sliderRect [controllerNumber].DOAnchorPos (new Vector2(imagesAlignedPos [2], sliderRect[controllerNumber].anchoredPosition.y), durationImageMovement);
			imagesNumber [controllerNumber] = 2;
			break;
		case 4 :
			sliderRect [controllerNumber].DOAnchorPos (new Vector2(imagesAlignedPos [3], sliderRect[controllerNumber].anchoredPosition.y), durationImageMovement);
			imagesNumber [controllerNumber] = 3;
			break;

		case -1 :
			break;
		}

		if (OnControllerChange != null)
			OnControllerChange ();
	}

	IEnumerator GapBetweenInputs (int controllerNumber)
	{
		switch (controllerNumber)
		{
		case 0:
			keyboardMoving = true;
			yield return StartCoroutine (CoroutineUtil.WaitForRealSeconds (gapBetweenInputs));
			keyboardMoving = false;
			break;
		case 1:
			gamepad1Moving = true;
			yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds (gapBetweenInputs));
			gamepad1Moving = false;
			break;
		case 2 :
			gamepad2Moving = true;
			yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds (gapBetweenInputs));
			gamepad2Moving = false;
			break;
		case 3 :
			gamepad3Moving = true;
			yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds (gapBetweenInputs));
			gamepad3Moving = false;
			break;
		case 4 :
			gamepad4Moving = true;
			yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds (gapBetweenInputs));
			gamepad4Moving = false;
			break;
		}
	}

	public void IgnoreInput ()
	{
		getInput = false;
	}
}
