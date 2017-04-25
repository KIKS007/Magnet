using System.Collections;
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

	private float controllersOnPosition;
	private float[] playersLogosInitialPos = new float[4];
	private bool[] hasJoined = new bool[5];
	private float playerChangeMovement;

	public event EventHandler OnControllerChange; 

	// Use this for initialization
	void Awake () 
	{
		ReInput.ControllerConnectedEvent += (ControllerStatusChangedEventArgs obj) => GamepadOn (obj.controllerId);
		ReInput.ControllerDisconnectedEvent += (ControllerStatusChangedEventArgs obj) => GamepadOff (obj.controllerId);

		controllersOnPosition = controllers [0].anchoredPosition.y;
		playerChangeMovement = controllers [1].anchoredPosition.x - controllers [0].anchoredPosition.x;

		for (int i = 0; i < playersLogos.Length; i++)
			playersLogosInitialPos [i] = playersLogos [i].anchoredPosition.x;

		for (int i = 1; i < controllers.Length; i++)
			Leave (i);

		if (ReInput.controllers.Joysticks.Count == 0)
			Join (0);
		else
			Leave (0);
	}

	void Start ()
	{
		for(int i = 0; i < 4; i++)
		{
			if (i < ReInput.controllers.Joysticks.Count)
				GamepadOn (i, true);
			else
				GamepadOff (i);
		}
	}

	void OnEnable ()
	{
		for(int i = 0; i < 4; i++)
		{
			if (i < ReInput.controllers.Joysticks.Count)
				GamepadOn (i);
			else
				GamepadOff (i);
		}

		CheckCanPlay ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Menu && gameObject.activeSelf == true) 
			CheckInput ();
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
					if (hasJoined [0] && i == 3)
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

		//Allow to play Alone and choose any character 
		if(ReInput.controllers.GetControllerCount(ControllerType.Joystick) == 0) 
			GlobalVariables.Instance.PlayersControllerNumber[1] = 1; 
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
		yield return new WaitWhile (() => MenuManager.Instance.isTweening); 

		if(GlobalVariables.Instance.EnabledPlayersList.Count > 1 && playButton.anchoredPosition.y != playButtonYPos.y) 
		{ 
			playButton.gameObject.SetActive (true);
			playButton.GetComponent<Button> ().interactable = true; 
			playButton.GetComponent<Button> ().Select (); 
			playButton.DOAnchorPosY (playButtonYPos.y, MenuManager.Instance.durationContent).SetEase(MenuManager.Instance.easeMenu).SetId ("PlayButton"); 
		} 

		if(GlobalVariables.Instance.EnabledPlayersList.Count < 2 && playButton.anchoredPosition.y != playButtonYPos.x) 
		{ 
			playButton.GetComponent<Button> ().interactable = false; 
			playButton.DOAnchorPosY (playButtonYPos.x, MenuManager.Instance.durationContent).SetEase(MenuManager.Instance.easeMenu).OnComplete (()=> playButton.gameObject.SetActive (false)); 
		} 
	} 

	void PunchPlayersScale (int controller)
	{
//		if(hasJoined [0])
//			playersLogos [controller].DOPunchScale (Vector3.one * punchScale, tweenDuration).SetEase (tweenEase);
//		else
//			playersLogos [controller - 1].DOPunchScale (Vector3.one * punchScale, tweenDuration).SetEase (tweenEase);
	}

	public void ToggleJoinLeave (int controller)
	{
		if (hasJoined [controller])
			Leave (controller);
		else
			Join (controller);
	}

	void Join (int controller)
	{
		hasJoined [controller] = true;
		controllers [controller].DOAnchorPosY (controllersOnPosition, tweenDuration).SetEase (tweenEase);

		UpdateSettings ();
		PunchPlayersScale (controller);
	}

	void Leave (int controller)
	{
		hasJoined [controller] = false;
		controllers [controller].DOAnchorPosY (controllersOnPosition - controllersOffPositionGap, tweenDuration).SetEase (tweenEase);

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
}
