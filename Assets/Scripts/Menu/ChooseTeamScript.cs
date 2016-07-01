using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XboxCtrlrInput;
using System.Linq;
using DG.Tweening;
using System.Collections.Generic;

public class ChooseTeamScript : MonoBehaviour 
{
	public enum TeamMode {TwoTeams, FourTeams, All};

	public TeamMode teamMode;

	public ControllerChangeManager1 playersManager;

	public GameObject[] gamepadsLines = new GameObject[4];

	public Sprite[] gamepadSprites = new Sprite[4];

	public float[] imagesAlignedPos = new float[] {20.6f, 172.8f, 320.8f, 468.1f};

	public int[] teamNumber = new int[] {1, 1, 1, 1};

	public float durationImageMovement = 0.1f;

	public float gapBetweenInputs = 0.2f;

	private bool keyboardMoving = false;
	private bool gamepad1Moving = false;
	private bool gamepad2Moving = false;
	private bool gamepad3Moving = false;
	private bool gamepad4Moving = false;

	private Color player1Color;
	private Color player2Color;
	private Color player3Color;
	private Color player4Color;

	private RectTransform[] sliderRect = new RectTransform[4];

	private int keyboardPlayerNumber;
	private int gamepad1PlayerNumber;
	private int gamepad2PlayerNumber;
	private int gamepad3PlayerNumber;
	private int gamepad4PlayerNumber;

	void OnEnable ()
	{
		player1Color = gamepadsLines [0].GetComponent<Text> ().color;
		player2Color = gamepadsLines [1].GetComponent<Text> ().color;
		player3Color = gamepadsLines [2].GetComponent<Text> ().color;
		player4Color = gamepadsLines [3].GetComponent<Text> ().color;

		GetControllersSettings ();

		UpdateStaticVariables ();
	}

	void OnDisable ()
	{
		//UpdateStaticVariables ();
	}

	void Start ()
	{
		
	}

	void GetPreviousTeamChoice ()
	{
		teamNumber [0] = StaticVariables.TeamChoice [0];
		teamNumber [1] = StaticVariables.TeamChoice [1];
		teamNumber [2] = StaticVariables.TeamChoice [2];
		teamNumber [3] = StaticVariables.TeamChoice [3];
	}

	public void GetControllersSettings ()
	{
		keyboardPlayerNumber = -1;
		gamepad1PlayerNumber = -1;
		gamepad2PlayerNumber = -1;
		gamepad3PlayerNumber = -1;
		gamepad4PlayerNumber = -1;


		if(StaticVariables.ControllerNumberPlayer1 == -1)
		{
			gamepadsLines [0].transform.GetChild (2).gameObject.SetActive (false);
			gamepadsLines [0].transform.GetChild (1).gameObject.SetActive (false);
			gamepadsLines [0].GetComponent<Text> ().color = new Color(103, 103, 103, 255) / 255;
			gamepadsLines [0].transform.GetChild (0).GetComponent<Image> ().DOFade (0.5f, 0.5f);
		}
		else
		{
			gamepadsLines [0].GetComponent<Text> ().color = player1Color;
			gamepadsLines [0].transform.GetChild (0).GetComponent<Image> ().DOFade (1, 0.5f);
		}

		switch (StaticVariables.ControllerNumberPlayer1)
		{
		case 0:
			gamepadsLines [0].transform.GetChild (1).gameObject.SetActive (false);
			gamepadsLines [0].transform.GetChild (2).gameObject.SetActive (true);
			keyboardPlayerNumber = 0;
			break;
		case 1:
			gamepadsLines [0].transform.GetChild (2).gameObject.SetActive (false);
			gamepadsLines [0].transform.GetChild (1).gameObject.SetActive (true);
			gamepadsLines [0].transform.GetChild (1).GetComponent<Image> ().sprite = gamepadSprites[0];
			gamepad1PlayerNumber = 0;
			break;
		case 2:
			gamepadsLines [0].transform.GetChild (2).gameObject.SetActive (false);
			gamepadsLines [0].transform.GetChild (1).gameObject.SetActive (true);
			gamepadsLines [0].transform.GetChild (1).GetComponent<Image> ().sprite = gamepadSprites[0];
			gamepad2PlayerNumber = 0;
			break;
		case 3:
			gamepadsLines [0].transform.GetChild (2).gameObject.SetActive (false);
			gamepadsLines [0].transform.GetChild (1).gameObject.SetActive (true);
			gamepadsLines [0].transform.GetChild (1).GetComponent<Image> ().sprite = gamepadSprites[0];
			gamepad3PlayerNumber = 0;
			break;
		case 4:
			gamepadsLines [0].transform.GetChild (2).gameObject.SetActive (false);
			gamepadsLines [0].transform.GetChild (1).gameObject.SetActive (true);
			gamepadsLines [0].transform.GetChild (1).GetComponent<Image> ().sprite = gamepadSprites[0];
			gamepad4PlayerNumber = 0;
			break;
		}


		if(StaticVariables.ControllerNumberPlayer2 == -1)
		{
			gamepadsLines [1].transform.GetChild (2).gameObject.SetActive (false);
			gamepadsLines [1].transform.GetChild (1).gameObject.SetActive (false);
			gamepadsLines [1].GetComponent<Text> ().color = new Color(103, 103, 103, 255) / 255;
			gamepadsLines [1].transform.GetChild (0).GetComponent<Image> ().DOFade (0.5f, 0.5f);
		}
		else
		{
			gamepadsLines [1].GetComponent<Text> ().color = player2Color;
			gamepadsLines [1].transform.GetChild (0).GetComponent<Image> ().DOFade (1, 0.5f);
		}

		switch (StaticVariables.ControllerNumberPlayer2)
		{
		case 0:
			gamepadsLines [1].transform.GetChild (1).gameObject.SetActive (false);
			gamepadsLines [1].transform.GetChild (2).gameObject.SetActive (true);
			keyboardPlayerNumber = 1;
			break;
		case 1:
			gamepadsLines [1].transform.GetChild (2).gameObject.SetActive (false);
			gamepadsLines [1].transform.GetChild (1).gameObject.SetActive (true);
			gamepadsLines [1].transform.GetChild (1).GetComponent<Image> ().sprite = gamepadSprites[1];
			gamepad1PlayerNumber = 1;
			break;
		case 2:
			gamepadsLines [1].transform.GetChild (2).gameObject.SetActive (false);
			gamepadsLines [1].transform.GetChild (1).gameObject.SetActive (true);
			gamepadsLines [1].transform.GetChild (1).GetComponent<Image> ().sprite = gamepadSprites[1];
			gamepad2PlayerNumber = 1;
			break;
		case 3:
			gamepadsLines [1].transform.GetChild (2).gameObject.SetActive (false);
			gamepadsLines [1].transform.GetChild (1).gameObject.SetActive (true);
			gamepadsLines [1].transform.GetChild (1).GetComponent<Image> ().sprite = gamepadSprites[1];
			gamepad3PlayerNumber = 1;
			break;
		case 4:
			gamepadsLines [1].transform.GetChild (2).gameObject.SetActive (false);
			gamepadsLines [1].transform.GetChild (1).gameObject.SetActive (true);
			gamepadsLines [1].transform.GetChild (1).GetComponent<Image> ().sprite = gamepadSprites[1];
			gamepad4PlayerNumber = 1;
			break;
		}


		if(StaticVariables.ControllerNumberPlayer3 == -1)
		{
			gamepadsLines [2].transform.GetChild (2).gameObject.SetActive (false);
			gamepadsLines [2].transform.GetChild (1).gameObject.SetActive (false);
			gamepadsLines [2].GetComponent<Text> ().color = new Color(103, 103, 103, 255) / 255;
			gamepadsLines [2].transform.GetChild (0).GetComponent<Image> ().DOFade (0.5f, 0.5f);
		}
		else
		{
			gamepadsLines [2].GetComponent<Text> ().color = player3Color;
			gamepadsLines [2].transform.GetChild (0).GetComponent<Image> ().DOFade (1, 0.5f);
		}

		switch (StaticVariables.ControllerNumberPlayer3)
		{
		case 0:
			gamepadsLines [2].transform.GetChild (1).gameObject.SetActive (false);
			gamepadsLines [2].transform.GetChild (2).gameObject.SetActive (true);
			keyboardPlayerNumber = 2;
			break;
		case 1:
			gamepadsLines [2].transform.GetChild (2).gameObject.SetActive (false);
			gamepadsLines [2].transform.GetChild (1).gameObject.SetActive (true);
			gamepadsLines [2].transform.GetChild (1).GetComponent<Image> ().sprite = gamepadSprites[2];
			gamepad1PlayerNumber = 2;
			break;
		case 2:
			gamepadsLines [2].transform.GetChild (2).gameObject.SetActive (false);
			gamepadsLines [2].transform.GetChild (1).gameObject.SetActive (true);
			gamepadsLines [2].transform.GetChild (1).GetComponent<Image> ().sprite = gamepadSprites[2];
			gamepad2PlayerNumber = 2;
			break;
		case 3:
			gamepadsLines [2].transform.GetChild (2).gameObject.SetActive (false);
			gamepadsLines [2].transform.GetChild (1).gameObject.SetActive (true);
			gamepadsLines [2].transform.GetChild (1).GetComponent<Image> ().sprite = gamepadSprites[2];
			gamepad3PlayerNumber = 2;
			break;
		case 4:
			gamepadsLines [2].transform.GetChild (2).gameObject.SetActive (false);
			gamepadsLines [2].transform.GetChild (1).gameObject.SetActive (true);
			gamepadsLines [2].transform.GetChild (1).GetComponent<Image> ().sprite = gamepadSprites[2];
			gamepad3PlayerNumber = 2;
			break;
		}


		if(StaticVariables.ControllerNumberPlayer4 == -1)
		{
			gamepadsLines [3].transform.GetChild (2).gameObject.SetActive (false);
			gamepadsLines [3].transform.GetChild (1).gameObject.SetActive (false);
			gamepadsLines [3].GetComponent<Text> ().color = new Color(103, 103, 103, 255) / 255;
			gamepadsLines [3].transform.GetChild (0).GetComponent<Image> ().DOFade (0.5f, 0.5f);
		}
		else
		{
			gamepadsLines [3].GetComponent<Text> ().color = player4Color;
			gamepadsLines [3].transform.GetChild (0).GetComponent<Image> ().DOFade (1, 0.5f);
		}

		switch (StaticVariables.ControllerNumberPlayer4)
		{
		case 0:
			gamepadsLines [3].transform.GetChild (1).gameObject.SetActive (false);
			gamepadsLines [3].transform.GetChild (2).gameObject.SetActive (true);
			keyboardPlayerNumber = 3;
			break;
		case 1:
			gamepadsLines [3].transform.GetChild (2).gameObject.SetActive (false);
			gamepadsLines [3].transform.GetChild (1).gameObject.SetActive (true);
			gamepadsLines [3].transform.GetChild (1).GetComponent<Image> ().sprite = gamepadSprites[3];
			gamepad1PlayerNumber = 3;
			break;
		case 2:
			gamepadsLines [3].transform.GetChild (2).gameObject.SetActive (false);
			gamepadsLines [3].transform.GetChild (1).gameObject.SetActive (true);
			gamepadsLines [3].transform.GetChild (1).GetComponent<Image> ().sprite = gamepadSprites[3];
			gamepad2PlayerNumber = 3;
			break;
		case 3:
			gamepadsLines [3].transform.GetChild (2).gameObject.SetActive (false);
			gamepadsLines [3].transform.GetChild (1).gameObject.SetActive (true);
			gamepadsLines [3].transform.GetChild (1).GetComponent<Image> ().sprite = gamepadSprites[3];
			gamepad3PlayerNumber = 3;
			break;
		case 4:
			gamepadsLines [3].transform.GetChild (2).gameObject.SetActive (false);
			gamepadsLines [3].transform.GetChild (1).gameObject.SetActive (true);
			gamepadsLines [3].transform.GetChild (1).GetComponent<Image> ().sprite = gamepadSprites[3];
			gamepad4PlayerNumber = 3;
			break;
		}

		UpdateSlidersRect ();
	}

	void UpdateSlidersRect ()
	{
		sliderRect [0] = null;
		sliderRect [1] = null;
		sliderRect [2] = null;
		sliderRect [3] = null;

		if (gamepadsLines [0].transform.GetChild (2).gameObject.activeSelf == true)
			sliderRect [0] = gamepadsLines [0].transform.GetChild (2).GetComponent<RectTransform> ();
		else if (gamepadsLines [0].transform.GetChild (1).gameObject.activeSelf == true)
			sliderRect [0] = gamepadsLines [0].transform.GetChild (1).GetComponent<RectTransform> ();

		if (gamepadsLines [1].transform.GetChild (2).gameObject.activeSelf == true)
			sliderRect [1] = gamepadsLines [1].transform.GetChild (2).GetComponent<RectTransform> ();
		else if (gamepadsLines [1].transform.GetChild (1).gameObject.activeSelf == true)
			sliderRect [1] = gamepadsLines [1].transform.GetChild (1).GetComponent<RectTransform> ();

		if (gamepadsLines [2].transform.GetChild (2).gameObject.activeSelf == true)
			sliderRect [2] = gamepadsLines [2].transform.GetChild (2).GetComponent<RectTransform> ();
		else if (gamepadsLines [2].transform.GetChild (1).gameObject.activeSelf == true)
			sliderRect [2] = gamepadsLines [2].transform.GetChild (1).GetComponent<RectTransform> ();

		if (gamepadsLines [3].transform.GetChild (2).gameObject.activeSelf == true)
			sliderRect [3] = gamepadsLines [3].transform.GetChild (2).GetComponent<RectTransform> ();
		else if (gamepadsLines [3].transform.GetChild (1).gameObject.activeSelf == true)
			sliderRect [3] = gamepadsLines [3].transform.GetChild (1).GetComponent<RectTransform> ();
	}

	void Update ()
	{
		if(teamMode != TeamMode.All)
			GetInput ();
	}

	void GetInput ()
	{
		if(keyboardPlayerNumber != -1)
		{
			if (Input.GetAxisRaw ("HorizontalPlayer") < 0 && !keyboardMoving)
			{
				StartCoroutine (GapBetweenInputs (0));
				GoOnTheLeft (keyboardPlayerNumber);
			}

			if (Input.GetAxisRaw ("HorizontalPlayer") > 0 && !keyboardMoving)
			{
				GoOnTheRight (keyboardPlayerNumber);
				StartCoroutine (GapBetweenInputs (0));
			}
		}

		if(gamepad1PlayerNumber != -1)
		{
			if (XCI.GetAxisRaw (XboxAxis.LeftStickX, 1) < 0 && !gamepad1Moving)
			{
				GoOnTheLeft (gamepad1PlayerNumber);
				StartCoroutine (GapBetweenInputs (1));
			}

			if (XCI.GetAxisRaw (XboxAxis.LeftStickX, 1) > 0 && !gamepad1Moving)
			{
				GoOnTheRight (gamepad1PlayerNumber);
				StartCoroutine (GapBetweenInputs (1));
			}
		}
	
		if(gamepad2PlayerNumber != -1)
		{
			if (XCI.GetAxisRaw (XboxAxis.LeftStickX, 2) < 0 && !gamepad2Moving)
			{
				GoOnTheLeft (gamepad2PlayerNumber);
				StartCoroutine (GapBetweenInputs (2));
			}

			if (XCI.GetAxisRaw (XboxAxis.LeftStickX, 2) > 0 && !gamepad2Moving)
			{
				GoOnTheRight (gamepad2PlayerNumber);
				StartCoroutine (GapBetweenInputs (2));
			}
		}
		
		if(gamepad3PlayerNumber != -1)
		{
			if (XCI.GetAxisRaw (XboxAxis.LeftStickX, 3) < 0 && !gamepad3Moving)
			{
				GoOnTheLeft (gamepad3PlayerNumber);
				StartCoroutine (GapBetweenInputs (3));
			}

			if (XCI.GetAxisRaw (XboxAxis.LeftStickX, 3) > 0 && !gamepad3Moving)
			{
				GoOnTheRight (gamepad3PlayerNumber);
				StartCoroutine (GapBetweenInputs (3));
			}
		}

		if(gamepad4PlayerNumber != -1)
		{
			if (XCI.GetAxisRaw (XboxAxis.LeftStickX, 4) < 0 && !gamepad4Moving)
			{
				GoOnTheLeft (gamepad4PlayerNumber);
				StartCoroutine (GapBetweenInputs (4));
			}

			if (XCI.GetAxisRaw (XboxAxis.LeftStickX, 4) > 0 && !gamepad4Moving)
			{
				GoOnTheRight (gamepad4PlayerNumber);
				StartCoroutine (GapBetweenInputs (4));
			}
		}
	}
		
	public void UpdateStaticVariables ()
	{
		StaticVariables.TeamChoice = new int[] {-1, -1, -1, -1};

		if (teamNumber [0] != -1 && StaticVariables.ControllerNumberPlayer1 != -1)
			StaticVariables.TeamChoice [0] = teamNumber [0];

		if (teamNumber [1] != -1 && StaticVariables.ControllerNumberPlayer2 != -1)
			StaticVariables.TeamChoice [1] = teamNumber [1];

		if (teamNumber [2] != -1 && StaticVariables.ControllerNumberPlayer3 != -1)
			StaticVariables.TeamChoice [2] = teamNumber [2];

		if (teamNumber [3] != -1 && StaticVariables.ControllerNumberPlayer4 != -1)
			StaticVariables.TeamChoice [3] = teamNumber [3];


		StaticVariables.Team1.Clear ();
		StaticVariables.Team2.Clear ();
		StaticVariables.Team3.Clear ();
		StaticVariables.Team4.Clear ();

		if(StaticVariables.ControllerNumberPlayer1 != -1)
		{
			switch (teamNumber [0])
			{
			case 1:
				StaticVariables.Team1.Add (StaticVariables.Player1);
				break;
			case 2:
				StaticVariables.Team2.Add (StaticVariables.Player1);
				break;
			case 3:
				StaticVariables.Team3.Add (StaticVariables.Player1);
				break;
			case 4:
				StaticVariables.Team4.Add (StaticVariables.Player1);
				break;
			}
		}

		if(StaticVariables.ControllerNumberPlayer2 != -1)
		{
			switch (teamNumber [1])
			{
			case 1:
				StaticVariables.Team1.Add (StaticVariables.Player2);
				break;
			case 2:
				StaticVariables.Team2.Add (StaticVariables.Player2);
				break;
			case 3:
				StaticVariables.Team3.Add (StaticVariables.Player2);
				break;
			case 4:
				StaticVariables.Team4.Add (StaticVariables.Player2);
				break;
			}
		}

		if(StaticVariables.ControllerNumberPlayer3 != -1)
		{
			switch (teamNumber [2])
			{
			case 1:
				StaticVariables.Team1.Add (StaticVariables.Player3);
				break;
			case 2:
				StaticVariables.Team2.Add (StaticVariables.Player3);
				break;
			case 3:
				StaticVariables.Team3.Add (StaticVariables.Player3);
				break;
			case 4:
				StaticVariables.Team4.Add (StaticVariables.Player3);
				break;
			}
		}

		if(StaticVariables.ControllerNumberPlayer4 != -1)
		{
			switch (teamNumber [3])
			{
			case 1:
				StaticVariables.Team1.Add (StaticVariables.Player4);
				break;
			case 2:
				StaticVariables.Team2.Add (StaticVariables.Player4);
				break;
			case 3:
				StaticVariables.Team3.Add (StaticVariables.Player4);
				break;
			case 4:
				StaticVariables.Team4.Add (StaticVariables.Player4);
				break;
			}
		}

		//Debug.Log (StaticVariables.TeamChoice [0]);
		//Debug.Log (StaticVariables.TeamChoice [1]);
		//Debug.Log (StaticVariables.TeamChoice [2]);
		//Debug.Log (StaticVariables.TeamChoice [3]);
	}
		
	void GoOnTheRight (int whichPlayer)
	{
		if(teamMode == TeamMode.TwoTeams)
		{
			switch (teamNumber[whichPlayer])
			{
			case 1:
				sliderRect [whichPlayer].DOLocalMoveX (imagesAlignedPos [1], durationImageMovement);
				teamNumber [whichPlayer] = 2;
				break;

			case 2:
				break;
			}
		}

		else
		{
			switch (teamNumber[whichPlayer])
			{
			case 1:
				sliderRect [whichPlayer].DOLocalMoveX (imagesAlignedPos [1], durationImageMovement);
				teamNumber [whichPlayer] = 2;
				break;
			case 2 :
				sliderRect [whichPlayer].DOLocalMoveX (imagesAlignedPos [2], durationImageMovement);
				teamNumber [whichPlayer] = 3;
				break;
			case 3 :
				sliderRect [whichPlayer].DOLocalMoveX (imagesAlignedPos [3], durationImageMovement);
				teamNumber [whichPlayer] = 4;
				break;

			case 4 :
				break;
			}	
		}

		UpdateStaticVariables ();
	}

	void GoOnTheLeft (int whichPlayer)
	{
		if(teamMode == TeamMode.TwoTeams)
		{
			switch (teamNumber[whichPlayer])
			{
			case 1:
				break;

			case 2:
				sliderRect [whichPlayer].DOLocalMoveX (imagesAlignedPos [0], durationImageMovement);
				teamNumber [whichPlayer] = 1;
				break;
			}
		}

		else
		{
			switch (teamNumber[whichPlayer])
			{
			case 1 :
				break;

			case 2 :
				sliderRect [whichPlayer].DOLocalMoveX (imagesAlignedPos [0], durationImageMovement);
				teamNumber [whichPlayer] = 1;
				break;
			case 3 :
				sliderRect [whichPlayer].DOLocalMoveX (imagesAlignedPos [1], durationImageMovement);
				teamNumber [whichPlayer] = 2;
				break;
			case 4 :
				sliderRect [whichPlayer].DOLocalMoveX (imagesAlignedPos [2], durationImageMovement);
				teamNumber [whichPlayer] = 3;
				break;

			case -1 :
				break;
			}
		}

		UpdateStaticVariables ();
	}

	IEnumerator GapBetweenInputs (int controllerNumber)
	{
		switch (controllerNumber)
		{
		case 0:
			keyboardMoving = true;
			yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds (gapBetweenInputs));
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
}
