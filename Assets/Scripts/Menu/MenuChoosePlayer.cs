﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Rewired;
using UnityEngine.UI;

public class MenuChoosePlayer : MonoBehaviour 
{
	[Header ("Tween")]
	public float tweenDuration = 0.2f;
	public Ease tweenEase = Ease.OutQuad;

	[Header ("Scale")]
	public float punchScale = 0.5f;

	[Header ("Rects")]
	public RectTransform[] playersLogos = new RectTransform[0];
	public RectTransform[] controllers = new RectTransform[0];

	[Header ("Positions")]
	public float controllersOffPositionGap;

	[Header ("Play Button")] 
	public RectTransform playButton; 
	public Vector2 playButtonYPos; 

	private List<List<Button>> aiButtons = new List<List<Button>> ();
	private float controllersOnPosition;
	private float[] playersLogosInitialPos = new float[4];
	private bool[] hasJoined = new bool[5];
	public bool[] aiHasJoined = new bool[4];
	private float playerChangeMovement;
	private bool noInput = false;
	private bool canPlay = false;
	private Button playButtonComponent;

	public event EventHandler OnControllerChange; 

	// Use this for initialization
	void Awake () 
	{
		ReInput.ControllerConnectedEvent += (ControllerStatusChangedEventArgs obj) => GamepadOn (obj.controllerId, true);
		ReInput.ControllerDisconnectedEvent += (ControllerStatusChangedEventArgs obj) => GamepadOff (obj.controllerId);

		playButtonComponent = playButton.GetComponent<Button> ();

		controllersOnPosition = controllers [0].anchoredPosition.y;
		playerChangeMovement = controllers [1].anchoredPosition.x - controllers [0].anchoredPosition.x;

		SetupAIButtons ();

		for (int i = 0; i < playersLogos.Length; i++)
			playersLogosInitialPos [i] = playersLogos [i].anchoredPosition.x;

		for (int i = 1; i < controllers.Length; i++)
			Leave (i);

		if (ReInput.controllers.Joysticks.Count < 2)
			Join (0);
		else
			Leave (0);
	}

	void Start ()
	{
		noInput = false;
	
		List<int> connectedJoystick = new List<int> ();

		if(ReInput.controllers.joystickCount > 0)
			foreach (var j in ReInput.controllers.GetJoysticks ())
				connectedJoystick.Add (j.id);

		for(int i = 0; i < 4; i++)
		{
			if(connectedJoystick.Contains (i))
				GamepadOn (i, true);
			else
				GamepadOff (i);
		}

		CheckCanPlay ();
	}

	void OnEnable ()
	{
		noInput = false;

		List<int> connectedJoystick = new List<int> ();

		if(ReInput.controllers.joystickCount > 0)
			foreach (var j in ReInput.controllers.GetJoysticks ())
				connectedJoystick.Add (j.id);

		for(int i = 0; i < 4; i++)
		{
			if(connectedJoystick.Contains (i))
				GamepadOn (i);
			else
				GamepadOff (i);
		}

		CheckCanPlay ();
	}

	void SetupAIButtons ()
	{
		aiButtons.Clear ();

		foreach(RectTransform t in playersLogos)
		{
			aiButtons.Add (new List<Button> ());

			foreach(Transform child in t.GetChild (1))
				aiButtons [aiButtons.Count - 1].Add (child.GetComponent<Button> ());
		}

		for(int listIndex = 0; listIndex < aiButtons.Count; listIndex++)
		{
			for(int i = 0; i < aiButtons [listIndex].Count; i++)
			{
				aiButtons [listIndex] [i].gameObject.SetActive (i == 0);

				int player = listIndex;
				int level = i;

				if(i == 3)
					aiButtons [listIndex] [i].onClick.AddListener (()=> AILeave (player));
				else
					aiButtons [listIndex] [i].onClick.AddListener (()=> AIJoin (player, level));
			}
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Menu && gameObject.activeSelf == true && !noInput)
			CheckInput ();

		if (!canPlay && playButtonComponent.interactable || MenuManager.Instance.isTweening)
			playButtonComponent.interactable = false;
	}

	void CheckInput ()
	{
		for(int i = 0; i < GlobalVariables.Instance.rewiredPlayers.Length; i++)
		{
			if (GlobalVariables.Instance.rewiredPlayers [i].GetButtonDown ("UI Join Leave"))
			{
				if(hasJoined [i])
					Leave (i);
				else
				{
					if (hasJoined [0] && i == 4)
						Leave (0);
					
					Join (i);
				}
			}

		}
	}

	void UpdateSettings ()
	{
		ChangePlayersPosition ();
		UpdateControllerNumber ();
		UpdatePlayersControllers ();
		GlobalVariables.Instance.UpdateGamepadList ();
		CheckCanPlay ();

		UpdateBotsCount ();

		if (OnControllerChange != null) 
			OnControllerChange (); 
	}

	void UpdatePlayersControllers () 
	{ 
		if(GlobalVariables.Instance.GameState == GameStateEnum.Menu && GlobalVariables.Instance.Players[0] != null) 
		{ 
			for(int i = 0; i < GlobalVariables.Instance.Players.Length; i++) 
			{ 
				if (GlobalVariables.Instance.PlayersControllerNumber[i] != -1) 
					GlobalVariables.Instance.Players[i].SetActive (true); 

				GlobalVariables.Instance.Players[i].GetComponent<PlayersGameplay>().SetupController (); 
			} 
		} 
	} 

	void UpdateControllerNumber ()
	{
		if(hasJoined [0])
		{
			GlobalVariables.Instance.PlayersControllerNumber [0] = 0;

			for(int i = 1; i < hasJoined.Length - 1; i++)
				if(hasJoined [i])
					GlobalVariables.Instance.PlayersControllerNumber [i] = i;
				else
					GlobalVariables.Instance.PlayersControllerNumber [i] = -1;
		}
		else
		{
			for(int i = 1; i < hasJoined.Length; i++)
				if(hasJoined [i])
					GlobalVariables.Instance.PlayersControllerNumber [i - 1] = i;
				else
					GlobalVariables.Instance.PlayersControllerNumber [i - 1] = -1;
		}

		/*//Allow to play Alone and choose any character 
		if(ReInput.controllers.GetControllerCount(ControllerType.Joystick) == 0) 
			GlobalVariables.Instance.PlayersControllerNumber[1] = 1; */
	}

	void ChangePlayersPosition ()
	{
		if(!hasJoined [0])
		{
			for(int i = 0; i < playersLogos.Length; i++)
				playersLogos [i].DOAnchorPosX (playersLogosInitialPos [i] + playerChangeMovement, tweenDuration).SetEase (tweenEase);
		}
		else
		{
			for(int i = 0; i < playersLogos.Length; i++)
				playersLogos [i].DOAnchorPosX (playersLogosInitialPos [i], tweenDuration).SetEase (tweenEase);
		}
	}

	void CheckCanPlay () 
	{
		if(gameObject.activeSelf)
			StartCoroutine (WaitEndMenuAnimation ()); 
	} 

	IEnumerator WaitEndMenuAnimation () 
	{ 
		if(MenuManager.Instance.isTweening)
			yield return new WaitWhile (() => MenuManager.Instance.isTweening); 

		int playersCount = 0;

		for (int i = 0; i < GlobalVariables.Instance.PlayersControllerNumber.Length; i++)
			if (GlobalVariables.Instance.PlayersControllerNumber [i] != -1)
				playersCount++;

		playersCount += GlobalVariables.Instance.NumberOfBots;

		if(playersCount > 1 && playButton.anchoredPosition.y != playButtonYPos.y) 
		{ 
			canPlay = true;

			DOTween.Kill ("PlayButton");

			playButton.gameObject.SetActive (true);
			playButton.GetComponent<Button> ().interactable = true; 
			MenuManager.Instance.eventSyst.SetSelectedGameObject (null);
			playButton.GetComponent<Button> ().Select (); 
			playButton.DOAnchorPosY (playButtonYPos.y, MenuManager.Instance.animationDuration).SetEase(MenuManager.Instance.easeMenu).SetId ("PlayButton"); 
		} 

		if(playersCount < 2 && playButton.anchoredPosition.y != playButtonYPos.x) 
		{ 
			canPlay = false;

			DOTween.Kill ("PlayButton");

			playButton.GetComponent<Button> ().interactable = false; 
			playButton.DOAnchorPosY (playButtonYPos.x, MenuManager.Instance.animationDuration).SetEase(MenuManager.Instance.easeMenu).SetId ("PlayButton").OnComplete (()=> playButton.gameObject.SetActive (false)); 
		} 

		yield return 0;
	} 

	void PunchPlayersScale (int controller)
	{
//		if(hasJoined [0])
//			playersLogos [controller].DOPunchScale (Vector3.one * punchScale, tweenDuration).SetEase (tweenEase);
//		else
//			playersLogos [controller - 1].DOPunchScale (Vector3.one * punchScale, tweenDuration).SetEase (tweenEase);
	}

	void AIJoin (int player, int level)
	{
		aiHasJoined [player] = true;
		GlobalVariables.Instance.aiEnabled [player] = true;
		GlobalVariables.Instance.aiLevels [player] = (AILevel)level;

		if (hasJoined [0])
			Leave ((int)player);
		else
			Leave ((int)player + 1);

		if (player == 0 && hasJoined [1])
			Leave (1);

		UpdateBotsCount ();
		UpdateSettings ();
	}

	void AILeave (int player)
	{
		aiHasJoined [player] = false;
		GlobalVariables.Instance.aiEnabled [player] = false;

		for (int i = 0; i < aiButtons [player].Count; i++)
			aiButtons [player] [i].gameObject.SetActive (i == 0);

		UpdateBotsCount ();
		UpdateSettings ();
	}

	void UpdateBotsCount ()
	{
		int botsCount = 0;

		foreach (var b in aiHasJoined)
			if (b)
				botsCount++;

		GlobalVariables.Instance.NumberOfBots = botsCount;

	}

	public void ToggleJoinLeave (int controller)
	{
		if (noInput)
			return;

		if (hasJoined [controller])
			Leave (controller);
		else
			Join (controller);
	}

	void Join (int controller)
	{
		hasJoined [controller] = true;
		controllers [controller].DOAnchorPosY (controllersOnPosition, tweenDuration).SetEase (tweenEase).OnComplete (CheckCanPlay);

		UpdateSettings ();
		PunchPlayersScale (controller);

		if (hasJoined [0])
			AILeave (controller);
		else
			AILeave (controller - 1);

		if(controller == 0 && aiHasJoined [1] && hasJoined [1])
			AILeave (1);
	}

	void Leave (int controller)
	{
		hasJoined [controller] = false;
		controllers [controller].DOAnchorPosY (controllersOnPosition - controllersOffPositionGap, tweenDuration).SetEase (tweenEase).OnComplete (CheckCanPlay);

		UpdateSettings ();
	}

	void GamepadOn (int gamepad, bool forceJoin = false)
	{
		if (GlobalVariables.Instance.GameState != GameStateEnum.Menu)
			return;
		
		controllers [gamepad + 1].GetComponent<Button> ().interactable = true;

		if (hasJoined [0] && gamepad == 3)
			Leave (0);

		if(forceJoin)
			Join (gamepad + 1);
	}

	void GamepadOff (int gamepad)
	{
		if (GlobalVariables.Instance.GameState != GameStateEnum.Menu)
			return;
		
		Leave (gamepad + 1);

		controllers [gamepad + 1].GetComponent<Button> ().interactable = false;
	}

	public void NoInput ()
	{
		noInput = true;
	}
}
