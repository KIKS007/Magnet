using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Rewired;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class MenuScrollView : MonoBehaviour
{
	public enum ScrollViewType {Menus, Content, Buttons};
	public ScrollViewType scrollViewType;

	[Header ("Buttons")]
	public float buttonsMovementDuration = 0.01f;
	public Ease buttonsEase = Ease.OutQuad;
	public bool overrideButtonCenterYPos = false;
	public float buttonsCenterYPos = -1;

	[Header ("Buttons Wheel")]
	public float buttonsWheelSpeed = 50;
	public Vector2 buttonsWheelLimits = new Vector2(-200, 200);

	[Header ("Content")]
	public float contentSpeed = 50;
	public float contentDuration = 0.5f;
	public Ease contentEase = Ease.OutQuad;
	public Vector2 contentLimits = new Vector2(-200, 200);

	public List<RectTransform> underMenuList;
	public List<RectTransform> underButtonsList;

	private MenuComponent aboveMenu;

	private RectTransform content;

	public bool mouseControl = false;

	public bool scrollViewEnabled = false;

	// Use this for initialization
	void Start () 
	{
		underMenuList.Clear ();
		underButtonsList.Clear ();

		aboveMenu = transform.parent.GetComponent<MenuComponent> ();

		if(!overrideButtonCenterYPos)
			buttonsCenterYPos = MenuManager.Instance.menuFirstButtonY;

		MenuManager.Instance.OnMenuChange += CheckScrollEnabled;

		if(scrollViewType == ScrollViewType.Menus)
		{
			for(int i = 0; i < transform.childCount; i++)
				underMenuList.Add (transform.GetChild (i).GetComponent<RectTransform> ());
			
			for (int i = 0; i < underMenuList.Count; i++)
				underButtonsList.Add (underMenuList [i].transform.GetChild (0).GetComponent<RectTransform> ());			
		}

		if(scrollViewType == ScrollViewType.Buttons)
		{
			for (int i = 0; i < transform.childCount; i++)
				underButtonsList.Add (transform.GetChild (i).GetComponent<RectTransform> ());	
		}

	}

	void CheckScrollEnabled ()
	{
		if (MenuManager.Instance.currentMenu == aboveMenu)
			scrollViewEnabled = true;
		else
			scrollViewEnabled = false;
	}

	void OnEnable ()
	{
		if(scrollViewType == ScrollViewType.Content)
		{
			content = transform.GetChild (0).GetComponent<RectTransform> ();

			content.anchoredPosition = Vector2.zero;

			CheckScrollEnabled ();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(scrollViewEnabled)
		{
			if (scrollViewType == ScrollViewType.Content)
				CheckContentInput ();
			
			if (scrollViewType == ScrollViewType.Menus && mouseControl || scrollViewType == ScrollViewType.Buttons && mouseControl)
				CheckButtonsWheelInput ();
			
			
			if (MenuManager.Instance.isTweening || DOTween.IsTweening ("MenuCamera"))
			{
				if(DOTween.IsTweening ("ScrollView"))
					DOTween.Kill ("ScrollView");
			}
			
			if(!mouseControl)
			{
				if (Mathf.Abs (Input.GetAxis ("Mouse X")) > 0 || Mathf.Abs (Input.GetAxis ("Mouse Y")) > 0)
					mouseControl = true;
			}
			else
			{
				for(int i = 0; i < GlobalVariables.Instance.rewiredPlayers.Length; i++)
					if (GlobalVariables.Instance.rewiredPlayers [i] != null && GlobalVariables.Instance.rewiredPlayers [i].GetAxis ("UI Vertical") != 0)
						mouseControl = false;
			}			
		}
	}

	void CheckContentInput ()
	{
		for(int i = 0; i < GlobalVariables.Instance.rewiredPlayers.Length; i++)
		{
			if (GlobalVariables.Instance.rewiredPlayers [i] != null && GlobalVariables.Instance.rewiredPlayers [i].GetAxis ("UI Vertical") > 0)
			{
				if((content.anchoredPosition.y - contentSpeed) > contentLimits.x)
					content.DOAnchorPosY (content.anchoredPosition.y - contentSpeed, contentDuration).SetEase (contentEase).SetId ("ScrollView");
				else
					content.DOAnchorPosY (contentLimits.x, contentDuration).SetEase (contentEase).SetId ("ScrollView");
			}

			if (GlobalVariables.Instance.rewiredPlayers [i] != null && GlobalVariables.Instance.rewiredPlayers [i].GetAxis ("UI Vertical") < 0)
			{
				if((content.anchoredPosition.y + contentSpeed) < contentLimits.y)
					content.DOAnchorPosY (content.anchoredPosition.y + contentSpeed, contentDuration).SetEase (contentEase).SetId ("ScrollView");
				else
					content.DOAnchorPosY (contentLimits.y, contentDuration).SetEase (contentEase).SetId ("ScrollView");

			}
		}

		if (Input.GetAxis ("Mouse ScrollWheel") > 0)
		{
			if((content.anchoredPosition.y - buttonsWheelSpeed) > contentLimits.x)
				content.DOAnchorPosY (content.anchoredPosition.y - buttonsWheelSpeed, contentDuration).SetEase (contentEase).SetId ("ScrollView");
			else
				content.DOAnchorPosY (contentLimits.x, contentDuration).SetEase (contentEase).SetId ("ScrollView");
		}

		if (Input.GetAxis ("Mouse ScrollWheel") < 0)
		{
			if((content.anchoredPosition.y + buttonsWheelSpeed) < contentLimits.y)
				content.DOAnchorPosY (content.anchoredPosition.y + buttonsWheelSpeed, contentDuration).SetEase (contentEase).SetId ("ScrollView");
			else
				content.DOAnchorPosY (contentLimits.y, contentDuration).SetEase (contentEase).SetId ("ScrollView");
		}
	}

	void CheckButtonsWheelInput ()
	{
		if (Input.GetAxis ("Mouse ScrollWheel") > 0)
		{
			if((underButtonsList [0].anchoredPosition.y - buttonsWheelSpeed) > buttonsWheelLimits.x)
			{
				for (int j = 0; j < underButtonsList.Count; j++)
					underButtonsList [j].DOAnchorPosY (underButtonsList [j].anchoredPosition.y - buttonsWheelSpeed, buttonsMovementDuration).SetEase (buttonsEase).SetId ("ScrollView");	
			}
			else
			{
				float difference = buttonsWheelLimits.x - underButtonsList [0].anchoredPosition.y;

				for (int j = 0; j < underButtonsList.Count; j++)
					underButtonsList [j].DOAnchorPosY (underButtonsList [j].anchoredPosition.y + difference, buttonsMovementDuration).SetEase (buttonsEase).SetId ("ScrollView");	
			}
		}
		
		if (Input.GetAxis ("Mouse ScrollWheel") < 0)
		{
			if((underButtonsList [0].anchoredPosition.y + buttonsWheelSpeed) < buttonsWheelLimits.y)
			{
				for (int j = 0; j < underButtonsList.Count; j++)
					underButtonsList [j].DOAnchorPosY (underButtonsList [j].anchoredPosition.y + buttonsWheelSpeed, buttonsMovementDuration).SetEase (buttonsEase).SetId ("ScrollView");
			}
			else
			{
				float difference = buttonsWheelLimits.y - underButtonsList [0].anchoredPosition.y;

				for (int j = 0; j < underButtonsList.Count; j++)
					underButtonsList [j].DOAnchorPosY (underButtonsList [j].anchoredPosition.y + difference, buttonsMovementDuration).SetEase (buttonsEase).SetId ("ScrollView");	
			}
			
		}
	}

	public void ButtonsMovement (RectTransform whichButton)
	{
		//Debug.Log ("IsTweening : " + DOTween.IsTweening ("Menu"));

		if(scrollViewType == ScrollViewType.Menus || scrollViewType == ScrollViewType.Buttons)
		{
			if(!MenuManager.Instance.isTweening && !mouseControl && scrollViewEnabled)
			{
				DOTween.Kill ("ScrollView");
				
				float yMovement = buttonsCenterYPos - whichButton.anchoredPosition.y;
				
				for (int i = 0; i < underButtonsList.Count; i++)
					underButtonsList [i].DOAnchorPosY (underButtonsList [i].anchoredPosition.y + yMovement, buttonsMovementDuration).SetEase (buttonsEase).SetId ("ScrollView");			
			}
		}
	}
}
