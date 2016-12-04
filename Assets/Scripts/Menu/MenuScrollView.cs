using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Rewired;
using System.Collections.Generic;
using DG.Tweening;

public class MenuScrollView : MonoBehaviour
{
	public float position;
	public float speed = 0.01f;
	public float duration = 0.01f;
	public Ease ease;

	private ScrollRect scrollRect;
	private Player[] playerList = new Player[5];

	// Use this for initialization
	void Start () 
	{
		scrollRect = GetComponent<ScrollRect> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		GetMenuPlayers ();

		CheckMenuInput ();

		position = scrollRect.verticalNormalizedPosition;
	}

	void GetMenuPlayers ()
	{
		playerList [0] = ReInput.players.GetPlayer (0);

		for(int i = 0; i < GamepadsManager.Instance.gamepadsList.Count; i++)
		{
			if(GamepadsManager.Instance.gamepadsList[i].GamepadId == 1)
			{
				playerList [1] = ReInput.players.GetPlayer (1);
				playerList [1].controllers.AddController(GamepadsManager.Instance.gamepadsList[i].GamepadController, true);
			}

			if(GamepadsManager.Instance.gamepadsList[i].GamepadId == 2)
			{
				playerList [2] = ReInput.players.GetPlayer (2);
				playerList [2].controllers.AddController(GamepadsManager.Instance.gamepadsList[i].GamepadController, true);
			}

			if(GamepadsManager.Instance.gamepadsList[i].GamepadId == 3)
			{
				playerList [3] = ReInput.players.GetPlayer (3);
				playerList [3].controllers.AddController(GamepadsManager.Instance.gamepadsList[i].GamepadController, true);
			}

			if(GamepadsManager.Instance.gamepadsList[i].GamepadId == 4)
			{
				playerList [4] = ReInput.players.GetPlayer (4);
				playerList [4].controllers.AddController(GamepadsManager.Instance.gamepadsList[i].GamepadController, true);
			}
		}
	}

	void CheckMenuInput ()
	{
		for(int i = 0; i < playerList.Length; i++)
		{
			if (playerList [i] != null && playerList [i].GetAxis ("Move Vertical") > 0 && scrollRect.verticalNormalizedPosition + speed <= 1)
			{
				//scrollRect.verticalNormalizedPosition += speed;
				DOTween.To (()=> scrollRect.verticalNormalizedPosition, x=> scrollRect.verticalNormalizedPosition =x, scrollRect.verticalNormalizedPosition + speed, duration).SetEase (ease).SetId ("Scroll");
			}

			if (playerList [i] != null && playerList [i].GetAxis ("Move Vertical") < 0 && scrollRect.verticalNormalizedPosition - speed >= 0)
			{
				//scrollRect.verticalNormalizedPosition -= speed;
				DOTween.To (()=> scrollRect.verticalNormalizedPosition, x=> scrollRect.verticalNormalizedPosition =x, scrollRect.verticalNormalizedPosition - speed, duration).SetEase (ease).SetId ("Scroll");

			}
		}		
	}
}
