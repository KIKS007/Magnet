using UnityEngine; 
using System.Collections; 
using UnityEngine.UI; 
using DG.Tweening; 
using System.Collections.Generic; 
using Rewired; 

public class ControllerChangeManager : MonoBehaviour  
{ 
	public event EventHandler OnControllerChange; 

	[Header ("Connect Gamepad Text")] 
	public GameObject[] gamepadsConnectText = new GameObject[4]; 

	[Header ("Player Logos")] 
	public RectTransform[] logoRect = new RectTransform[5]; 

	[Header ("Sliders")] 
	public RectTransform[] sliderRect = new RectTransform[5]; 

	[Header ("Images")] 
	public float[] imagesAlignedPos = new float[] {107.1f, 242.4f, 395.9f, 544.8f, 692.6f}; 

	public int[] imagesNumber = new int[] {0, 0, 0, 0, 0}; 

	[Header ("Settings")] 
	public float durationImageMovement = 0.1f; 
	public float gapBetweenInputs = 0.2f; 
	public float durationColor = 0.5f; 

	[Header ("Play Button")] 
	public RectTransform playButton; 
	public Vector2 playButtonYPos; 

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

	private bool getInput = true; 

	void Awake () 
	{ 
		OnControllerChange += GlobalVariables.Instance.SetPlayerMouseCursor; 
	} 

	void Start () 
	{ 
		imagesAlignedPos [0] = logoRect [0].anchoredPosition.x; 
		imagesAlignedPos [1] = logoRect [1].anchoredPosition.x; 
		imagesAlignedPos [2] = logoRect [2].anchoredPosition.x; 
		imagesAlignedPos [3] = logoRect [3].anchoredPosition.x; 
		imagesAlignedPos [4] = logoRect [4].anchoredPosition.x; 

		ReInput.ControllerDisconnectedEvent += (ControllerStatusChangedEventArgs arg) => UpdateAllSettings (); 
		ReInput.ControllerConnectedEvent += (ControllerStatusChangedEventArgs arg) => UpdateAllSettings (); 

		UpdateAllSettings (); 
	} 

	void OnEnable () 
	{ 
		getInput = true; 

		if(GlobalVariables.Instance.GameState == GameStateEnum.Menu) 
		{ 
			GamepadsManager.Instance.FindGamepadsPluggedAtStart (); 

			UpdateAllSettings (); 
		} 
	} 

	void UpdateAllSettings () 
	{ 
		if(GlobalVariables.Instance.GameState == GameStateEnum.Menu && gameObject.activeSelf == true) 
		{ 
			SetSlidersPosition (); 

			GetPlayers (); 

			GamepadDisplay (); 

			UpdateGlobalVariables (); 

			UpdatePlayersControllers (); 

			CheckCanPlay ();       
		} 
	} 

	void UpdateControllerChange () 
	{ 
		UpdateGlobalVariables (); 
		UpdatePlayersControllers (); 
		CheckCanPlay (); 
	} 

	#region Update 
	void Update () 
	{ 
		if(getInput) 
			GetInput (); 

		DisplayConnectGamepadsText (); 
	} 

	void GetInput () 
	{ 
		if (mouseKeyboard.GetAxisRaw("Move Horizontal") < -0.5f && !keyboardMoving) 
		{ 
			GoOnTheLeft (0); 
		} 

		if (mouseKeyboard.GetAxisRaw("Move Horizontal") > 0.5f && !keyboardMoving) 
		{ 
			GoOnTheRight (0); 
		} 


		if (gamepad1.GetAxisRaw("Move Horizontal") < -0.5f && !gamepad1Moving) 
		{ 
			GoOnTheLeft (1); 
		} 

		if (gamepad1.GetAxisRaw("Move Horizontal") > 0.5f && !gamepad1Moving) 
		{ 
			GoOnTheRight (1); 
		} 


		if (gamepad2.GetAxisRaw("Move Horizontal") < -0.5f && !gamepad2Moving) 
		{ 
			GoOnTheLeft (2); 
		} 

		if (gamepad2.GetAxisRaw("Move Horizontal") > 0.5f && !gamepad2Moving) 
		{ 
			GoOnTheRight (2); 
		} 


		if (gamepad3.GetAxisRaw("Move Horizontal") < -0.5f && !gamepad3Moving) 
		{ 
			GoOnTheLeft (3); 
		} 

		if (gamepad3.GetAxisRaw("Move Horizontal") > 0.5f && !gamepad3Moving) 
		{ 
			GoOnTheRight (3); 
		} 


		if (gamepad4.GetAxisRaw("Move Horizontal") < -0.5f && !gamepad4Moving) 
		{ 
			GoOnTheLeft (4); 
		} 

		if (gamepad4.GetAxisRaw("Move Horizontal") > 0.5f && !gamepad4Moving) 
		{ 
			GoOnTheRight (4); 
		} 
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
	#endregion 

	#region Correct Players Selection 
	void CheckCanPlay () 
	{ 
		StartCoroutine (WaitEndMenuAnimation ()); 
	} 

	IEnumerator WaitEndMenuAnimation () 
	{ 
		yield return new WaitWhile (() => DOTween.IsTweening ("Menu")); 

		if(CorrectPlayerChoice () && playButton.anchoredPosition.y != playButtonYPos.y) 
		{ 
			playButton.GetComponent<Button> ().interactable = true; 
			playButton.GetComponent<Button> ().Select (); 
			playButton.DOAnchorPosY (playButtonYPos.y, MenuManager.Instance.durationContent).SetEase(MenuManager.Instance.easeMenu).SetId ("PlayButton"); 
		} 

		if(!CorrectPlayerChoice () && playButton.anchoredPosition.y != playButtonYPos.x) 
		{ 
			playButton.GetComponent<Button> ().interactable = false; 
			playButton.DOAnchorPosY (playButtonYPos.x, MenuManager.Instance.durationContent).SetEase(MenuManager.Instance.easeMenu); 
		} 
	} 

	bool CorrectPlayerChoice () 
	{ 
		int player1Choice = 0; 
		int player2Choice = 0; 
		int player3Choice = 0; 
		int player4Choice = 0; 

		for(int i = 0; i < imagesNumber.Length; i++) 
		{ 
			switch(imagesNumber[i]) 
			{ 
			case 1: 
				player1Choice++; 
				break; 
			case 2: 
				player2Choice++; 
				break; 
			case 3: 
				player3Choice++; 
				break; 
			case 4: 
				player4Choice++; 
				break; 
			} 
		} 

		if (player1Choice > 1 || player2Choice > 1 || player3Choice > 1 || player4Choice > 1) 
			return false; 

		else if (GlobalVariables.Instance.NumberOfPlayers < 2) 
			return false; 

		else 
			return true; 
	} 
	#endregion 

	#region Update All Settings 
	public void SetSlidersPosition () 
	{ 
		for(int i = 0; i < GlobalVariables.Instance.PlayersControllerNumber.Length; i++) 
			if(GlobalVariables.Instance.PlayersControllerNumber[i] != -1) 
			{ 
				sliderRect [GlobalVariables.Instance.PlayersControllerNumber[i]].DOLocalMoveX (imagesAlignedPos [i + 1], durationImageMovement); 
				imagesNumber [GlobalVariables.Instance.PlayersControllerNumber[i]] = i + 1; 
			} 
	} 

	void GetPlayers () 
	{ 
		mouseKeyboard = GlobalVariables.Instance.rewiredPlayers [0]; 
		gamepad1 = GlobalVariables.Instance.rewiredPlayers [1]; 
		gamepad2 = GlobalVariables.Instance.rewiredPlayers [2]; 
		gamepad3 = GlobalVariables.Instance.rewiredPlayers [3]; 
		gamepad4 = GlobalVariables.Instance.rewiredPlayers [4]; 
	} 

	public void GamepadDisplay () 
	{ 
		for(int i = 1; i < 5; i++) 
		{ 
			if(GamepadsManager.Instance.gamepadsPluggedAtStart[i - 1] && !GamepadsManager.Instance.gamepadsUnplugged[i - 1]) 
				sliderRect [i].GetComponent<Button> ().interactable = true; 

			else 
			{ 
				sliderRect [i].GetComponent<Button> ().interactable = false; 
				sliderRect [i].DOLocalMoveX (imagesAlignedPos [0], durationImageMovement); 
				imagesNumber [i] = 0; 
			} 
		} 
	} 

	public void UpdateGlobalVariables () 
	{ 
		EraseControllerNumbers (0); 
		EraseControllerNumbers (1); 
		EraseControllerNumbers (2); 
		EraseControllerNumbers (3); 
		EraseControllerNumbers (4); 

		//Set Controller Number 
		for(int i = 0; i < imagesNumber.Length; i++) 
			if(imagesNumber [i] > 0) 
				GlobalVariables.Instance.PlayersControllerNumber [imagesNumber [i] - 1] = i; 


		//Allow to play Alone and choose any character 
		if(ReInput.controllers.GetControllerCount(ControllerType.Joystick) == 0) 
		{ 
			if(GlobalVariables.Instance.PlayersControllerNumber[0] == 0) 
				GlobalVariables.Instance.PlayersControllerNumber[1] = 1; 

			else 
				GlobalVariables.Instance.PlayersControllerNumber[0] = 1; 
		} 

		GlobalVariables.Instance.ListPlayers (); 
	} 

	void EraseControllerNumbers (int whichController) 
	{ 
		if(GlobalVariables.Instance.GameState == GameStateEnum.Menu) 
		{ 
			for(int i = 0; i < GlobalVariables.Instance.PlayersControllerNumber.Length; i++) 
				if (GlobalVariables.Instance.PlayersControllerNumber[i] == whichController) 
					GlobalVariables.Instance.PlayersControllerNumber[i] = -1; 
		} 
	} 

	public void UpdatePlayersControllers () 
	{ 
		if(GlobalVariables.Instance.GameState == GameStateEnum.Menu && GlobalVariables.Instance.Players[0] != null) 
		{ 
			for(int i = 0; i < GlobalVariables.Instance.Players.Length; i++) 
			{ 
				if (GlobalVariables.Instance.PlayersControllerNumber[i] != -1) 
					GlobalVariables.Instance.Players[i].SetActive (true); 

				GlobalVariables.Instance.Players[i].GetComponent<PlayersGameplay>().GetControllerNumber (); 

				GlobalVariables.Instance.Players[i].GetComponent<PlayersGameplay>().Controller (); 
			} 
		} 
	} 
	#endregion 

	#region Sliders Movements 
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

		UpdateControllerChange (); 

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

		UpdateControllerChange (); 

		if (OnControllerChange != null) 
			OnControllerChange (); 

	} 

	IEnumerator GapBetweenInputs (int controllerNumber) 
	{ 
		switch (controllerNumber) 
		{ 
		case 0: 
			keyboardMoving = true; 
			yield return new WaitForSecondsRealtime (gapBetweenInputs);
			keyboardMoving = false; 
			break; 
		case 1: 
			gamepad1Moving = true; 
			yield return new WaitForSecondsRealtime (gapBetweenInputs);
			gamepad1Moving = false; 
			break; 
		case 2 : 
			gamepad2Moving = true; 
			yield return new WaitForSecondsRealtime (gapBetweenInputs);
			gamepad2Moving = false; 
			break; 
		case 3 : 
			gamepad3Moving = true; 
			yield return new WaitForSecondsRealtime (gapBetweenInputs);
			gamepad3Moving = false; 
			break; 
		case 4 : 
			gamepad4Moving = true; 
			yield return new WaitForSecondsRealtime (gapBetweenInputs);
			gamepad4Moving = false; 
			break; 
		} 
	} 

	public void IgnoreInput () 
	{ 
		getInput = false; 
	} 
	#endregion 
}