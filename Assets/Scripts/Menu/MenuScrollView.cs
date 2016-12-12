using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Rewired;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;

public class MenuScrollView : MonoBehaviour
{
	public enum ViewportType {Buttons, Content};
	public ViewportType viewportType;

	[Header ("Buttons")]
	public float buttonsMovementDuration = 0.01f;
	public Ease buttonsEase = Ease.OutQuad;

	private float buttonsCenterYPos = 0;

	[Header ("Buttons Wheel")]
	public float buttonsWheelSpeed = 50;
	public Vector2 buttonsWheelLimits = new Vector2(-200, 200);

	[Header ("Content")]
	public float contentSpeed = 50;
	public float contentDuration = 0.5f;
	public Ease contentEase = Ease.OutQuad;
	public Vector2 contentLimits = new Vector2(-200, 200);

	[HideInInspector]
	public List<RectTransform> underMenuList;
	[HideInInspector]
	public List<RectTransform> underButtonsList;

	private RectTransform content;

	private Player[] playerList = new Player[5];

	public bool mouseControl = true;

	// Use this for initialization
	void Start () 
	{
		buttonsCenterYPos = MenuManager.Instance.firstButtonY;

		if(viewportType == ViewportType.Buttons)
		{
			for(int i = 0; i < transform.childCount; i++)
				underMenuList.Add (transform.GetChild (i).GetComponent<RectTransform> ());
			
			for (int i = 0; i < underMenuList.Count; i++)
				underButtonsList.Add (underMenuList [i].transform.GetChild (0).GetComponent<RectTransform> ());			
		}
	}

	void OnEnable ()
	{
		if(viewportType == ViewportType.Content)
		{
			content = transform.GetChild (0).GetComponent<RectTransform> ();

			content.anchoredPosition = Vector2.zero;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		GetMenuPlayers ();

		if (viewportType == ViewportType.Content)
			CheckContentInput ();

		if (viewportType == ViewportType.Buttons && mouseControl)
			CheckButtonsWheelInput ();


		if (DOTween.IsTweening ("Menu") || DOTween.IsTweening ("MenuCamera"))
		{
			if(DOTween.IsTweening ("Viewport"))
			DOTween.Kill ("Viewport");
		}

		if(!mouseControl)
		{
			if (Input.GetAxis ("Mouse X") > 0 || Input.GetAxis ("Mouse Y") > 0)
				mouseControl = true;
		}
		else
		{
			for(int i = 0; i < playerList.Length; i++)
				if (playerList [i] != null && playerList [i].GetAxis ("UI Vertical") != 0)
					mouseControl = false;
		}

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

	void CheckContentInput ()
	{
		for(int i = 0; i < playerList.Length; i++)
		{
			if (playerList [i] != null && playerList [i].GetAxis ("UI Vertical") > 0)
			{
				if((content.anchoredPosition.y - contentSpeed) > contentLimits.x)
					content.DOAnchorPosY (content.anchoredPosition.y - contentSpeed, contentDuration).SetEase (contentEase).SetId ("Viewport");
				else
					content.DOAnchorPosY (contentLimits.x, contentDuration).SetEase (contentEase).SetId ("Viewport");
			}

			if (playerList [i] != null && playerList [i].GetAxis ("UI Vertical") < 0)
			{
				if((content.anchoredPosition.y + contentSpeed) < contentLimits.y)
					content.DOAnchorPosY (content.anchoredPosition.y + contentSpeed, contentDuration).SetEase (contentEase).SetId ("Viewport");
				else
					content.DOAnchorPosY (contentLimits.y, contentDuration).SetEase (contentEase).SetId ("Viewport");

			}
		}

		if (Input.GetAxis ("Mouse ScrollWheel") > 0)
		{
			if((content.anchoredPosition.y - buttonsWheelSpeed) > contentLimits.x)
				content.DOAnchorPosY (content.anchoredPosition.y - buttonsWheelSpeed, contentDuration).SetEase (contentEase).SetId ("Viewport");
			else
				content.DOAnchorPosY (contentLimits.x, contentDuration).SetEase (contentEase).SetId ("Viewport");
		}

		if (Input.GetAxis ("Mouse ScrollWheel") < 0)
		{
			if((content.anchoredPosition.y + buttonsWheelSpeed) < contentLimits.y)
				content.DOAnchorPosY (content.anchoredPosition.y + buttonsWheelSpeed, contentDuration).SetEase (contentEase).SetId ("Viewport");
			else
				content.DOAnchorPosY (contentLimits.y, contentDuration).SetEase (contentEase).SetId ("Viewport");
		}
	}

	void CheckButtonsWheelInput ()
	{
		if (Input.GetAxis ("Mouse ScrollWheel") > 0)
		{
			if((underButtonsList [0].anchoredPosition.y - buttonsWheelSpeed) > buttonsWheelLimits.x)
			{
				for (int j = 0; j < underButtonsList.Count; j++)
					underButtonsList [j].DOAnchorPosY (underButtonsList [j].anchoredPosition.y - buttonsWheelSpeed, buttonsMovementDuration).SetEase (buttonsEase).SetId ("Viewport");	
			}
			else
			{
				float difference = buttonsWheelLimits.x - underButtonsList [0].anchoredPosition.y;

				for (int j = 0; j < underButtonsList.Count; j++)
					underButtonsList [j].DOAnchorPosY (underButtonsList [j].anchoredPosition.y + difference, buttonsMovementDuration).SetEase (buttonsEase).SetId ("Viewport");	
			}
		}
		
		if (Input.GetAxis ("Mouse ScrollWheel") < 0)
		{
			if((underButtonsList [0].anchoredPosition.y + buttonsWheelSpeed) < buttonsWheelLimits.y)
			{
				for (int j = 0; j < underButtonsList.Count; j++)
					underButtonsList [j].DOAnchorPosY (underButtonsList [j].anchoredPosition.y + buttonsWheelSpeed, buttonsMovementDuration).SetEase (buttonsEase).SetId ("Viewport");
			}
			else
			{
				float difference = buttonsWheelLimits.y - underButtonsList [0].anchoredPosition.y;

				for (int j = 0; j < underButtonsList.Count; j++)
					underButtonsList [j].DOAnchorPosY (underButtonsList [j].anchoredPosition.y + difference, buttonsMovementDuration).SetEase (buttonsEase).SetId ("Viewport");	
			}
			
		}
	}

	public void ButtonsMovement (RectTransform whichButton)
	{
		if(!DOTween.IsTweening ("Menu") && viewportType == ViewportType.Buttons && !mouseControl)
		{
			float yMovement = buttonsCenterYPos - whichButton.anchoredPosition.y;

			for (int i = 0; i < underButtonsList.Count; i++)
				underButtonsList [i].DOAnchorPosY (underButtonsList [i].anchoredPosition.y + yMovement, buttonsMovementDuration).SetEase (buttonsEase).SetId ("Viewport");			
		}
	}
}
