using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DarkTonic.MasterAudio;
using Rewired;
using GameAnalyticsSDK;
using Rewired.UI.ControlMapper;
using System;

public enum MenuComponentType {ContentMenu, ButtonsListMenu, MainMenu};

public class MenuManager : Singleton <MenuManager> 
{
	public bool tweening;

	public GameObject mainMenu;
	public MenuComponent currentMenu;

	private MenuComponent mainMenuScript;

	[Header ("Ease")]
	public Ease easeMenu = Ease.OutQuad;

	[Header ("Animations Duration")]
	public float durationToShow = 0.15f;
	public float durationToHide = 0.15f;
	public float durationContent = 0.15f;
	
	[Header ("Positions")]
	public float offScreenX = -1600;
	public float onScreenX = -650;
	
	[Header ("MainMenu Buttons Positions")]
	public float mainMenuFirstButtonY;
	public float[] mainMenuButtonsYPositions = new float[9];
	
	[Header ("Buttons Positions")]
	public float gapAfterHeaderButton;
	public float gapBetweenButtons = 131;
	public float[] buttonsYPositions = new float[9];
	
	[Header ("Header Buttons")]
	public float headerButtonsYPosition = 540;
	public List<RectTransform> headerButtonsList;

	[Header ("Buttons Delay")]
	public float[] buttonsDelay = {0, 0.05f, 0.1f, 0.15f, 0.2f, 0.25f};

	// Use this for initialization
	void Start () 
	{
		mainMenu.SetActive (true);
		mainMenuScript = mainMenu.GetComponent<MenuComponent> ();

		SetupButtonsPositions ();

		MainMenu ();
	}

	void SetupButtonsPositions ()
	{
		buttonsYPositions [0] = headerButtonsYPosition - gapAfterHeaderButton;

		for (int i = 1; i < buttonsYPositions.Length; i++)
			buttonsYPositions [i] = buttonsYPositions [0] - (gapBetweenButtons * i);

		for (int i = 0; i < mainMenuButtonsYPositions.Length; i++)
			mainMenuButtonsYPositions [i] = mainMenuFirstButtonY - (gapBetweenButtons * i);
	}
	
	// Update is called once per frame
	void Update () 
	{
		List<Player> playersListTemp = new List<Player> (ReInput.players.GetPlayers ());

		for(int i = 0; i < playersListTemp.Count; i++)
		{
			if (playersListTemp [i].GetButtonDown ("UI Cancel") && !tweening)
				currentMenu.Cancel ();
		}
	}

	//Main Menu		
	void MainMenu ()
	{
		List<RectTransform> underButtonsList = mainMenuScript.underButtonsList;

		for (int i = 0; i < underButtonsList.Count; i++)
			underButtonsList [i].anchoredPosition = new Vector2 (offScreenX, mainMenuButtonsYPositions [i]);

		int underDelay = 0;

		for(int i = underButtonsList.Count - 1; i >= 0; i--)
		{
			Enable (underButtonsList [i]);
			SetInteractable (underButtonsList [i]);

			underButtonsList [i].DOAnchorPosX (onScreenX, durationToShow).SetDelay (buttonsDelay [underDelay]).SetEase (easeMenu);
			underDelay++;
		}

		underButtonsList [0].GetComponent<Button> ().Select ();

		currentMenu = mainMenuScript;
	}

	void MainMenu (RectTransform menuButton)
	{
		List<RectTransform> underButtonsList = mainMenuScript.underButtonsList;

		int cancelButton = 0;

		for (int i = 0; i < underButtonsList.Count; i++)
			if (underButtonsList [i] == menuButton)
				cancelButton = i;		

		int delay = 0;

		SetInteractable (underButtonsList [cancelButton]);
		underButtonsList [cancelButton].DOAnchorPos (new Vector2(onScreenX, mainMenuButtonsYPositions[cancelButton]), durationToShow).SetDelay (buttonsDelay [delay]).SetEase (easeMenu);
		headerButtonsList.RemoveAt (headerButtonsList.Count - 1);

		delay++;

		for(int i = underButtonsList.Count - 1; i >= 0; i--)
		{
			if(i != cancelButton)
			{
				Enable (underButtonsList [i]);
				SetInteractable (underButtonsList [i]);

				underButtonsList [i].DOAnchorPosX (onScreenX, durationToShow).SetDelay (buttonsDelay [delay]).SetEase (easeMenu);
				delay++;
			}
		}

		underButtonsList [0].gameObject.GetComponent<Button> ().Select ();

		currentMenu = mainMenuScript;
	}


	//Sumbit Methods
	public void ShowUnderButtons (List<RectTransform> otherButtonsList, int submitButton, List<RectTransform> underButtonsList, MenuComponent whichMenu)
	{
		StartCoroutine (ShowUnderButtonsCoroutine (otherButtonsList, submitButton, underButtonsList, whichMenu));
	}

	IEnumerator ShowUnderButtonsCoroutine (List<RectTransform> otherButtonsList, int submitButton, List<RectTransform> underButtonsList, MenuComponent whichMenu)
	{
		yield return HideOtherButtons (otherButtonsList, submitButton).WaitForCompletion();			

		//Show Under Menu Buttons
		for (int i = 0; i < underButtonsList.Count; i++)
			underButtonsList [i].anchoredPosition = new Vector2 (offScreenX, buttonsYPositions [i]);

		int underDelay = 0;

		for(int i = underButtonsList.Count - 1; i >= 0; i--)
		{
			Enable (underButtonsList [i]);
			SetInteractable (underButtonsList [i]);

			underButtonsList [i].DOAnchorPosX (onScreenX, durationToShow).SetDelay (buttonsDelay [underDelay]).SetEase (easeMenu);
			underDelay++;
		}

		//Select First Under Menu Button
		underButtonsList [0].gameObject.GetComponent<Button> ().Select ();

		currentMenu = whichMenu;
	}

	public void ShowContent (List<RectTransform> otherButtonsList, int submitButton, RectTransform content, MenuComponent whichMenu, Selectable selectable = null)
	{
		StartCoroutine (ShowContentCoroutine (otherButtonsList, submitButton, content, whichMenu, selectable));
	}

	IEnumerator ShowContentCoroutine (List<RectTransform> otherButtonsList, int submitButton, RectTransform content, MenuComponent whichMenu, Selectable selectable = null)
	{
		yield return HideOtherButtons (otherButtonsList, submitButton).WaitForCompletion();			

		content.anchoredPosition = new Vector2 (offScreenX, 0);
		Enable (content);

		content.DOAnchorPosX (0, durationContent).SetEase (easeMenu);

		if(selectable != null)
			selectable.Select ();

		currentMenu = whichMenu;
	}

	Tween HideOtherButtons (List<RectTransform> otherButtonsList, int submitButton)
	{
		//Hide Other Buttons and Get Header Button At Top
		int aboveDelay = 0;

		for(int i = otherButtonsList.Count - 1; i >= 0; i--)
		{
			if(i != submitButton)
			{
				Disable (otherButtonsList [i], durationToHide + buttonsDelay [aboveDelay]);
				SetNonInteractable (otherButtonsList [i]);

				otherButtonsList [i].DOAnchorPosX (offScreenX, durationToHide).SetDelay (buttonsDelay [aboveDelay]).SetEase (easeMenu);
				aboveDelay++;
			}
		}

		SetNonInteractable (otherButtonsList [submitButton]);
		return otherButtonsList [submitButton].DOAnchorPos (new Vector2(onScreenX, HeaderButtonPosition ()), durationToShow).SetDelay (buttonsDelay [aboveDelay]).SetEase (easeMenu).OnComplete (()=> headerButtonsList.Add (otherButtonsList [submitButton]));
	}



	//Cancel Methods
	public void HideUnderButtons (List<RectTransform> underButtonsList, MenuComponent aboveMenu, RectTransform menuButton)
	{
		StartCoroutine (HiderUnderButtonsCoroutine (underButtonsList, aboveMenu, menuButton));
	}

	IEnumerator HiderUnderButtonsCoroutine (List<RectTransform> underButtonsList, MenuComponent aboveMenu, RectTransform menuButton)
	{
		int delay = 0;
		Tween tween = null;

		for(int i = underButtonsList.Count - 1; i >= 0; i--)
		{
			Disable (underButtonsList [i], durationToHide + buttonsDelay [delay]);
			SetNonInteractable (underButtonsList [i]);

			tween = underButtonsList [i].DOAnchorPosX (offScreenX, durationToHide).SetDelay (buttonsDelay [delay]).SetEase (easeMenu);
			delay++;
		}

		yield return tween.WaitForCompletion();

		if (aboveMenu.menuComponentType == MenuComponentType.MainMenu)
			MainMenu (menuButton);
		else
			ShowAboveButtons (aboveMenu, menuButton);
	}

	public void HideContent (RectTransform content, MenuComponent aboveMenu, RectTransform menuButton)
	{
		StartCoroutine (HideContentCoroutine (content, aboveMenu, menuButton));
	}

	IEnumerator HideContentCoroutine (RectTransform content, MenuComponent aboveMenu, RectTransform menuButton)
	{
		Tween tween = content.DOAnchorPosX (offScreenX, durationContent).SetEase (easeMenu).OnComplete (()=> Disable(content));

		yield return tween.WaitForCompletion();
	
		if (aboveMenu.menuComponentType == MenuComponentType.MainMenu)
			MainMenu (menuButton);
		else
			ShowAboveButtons (aboveMenu, menuButton);
	}
		
	void ShowAboveButtons (MenuComponent aboveMenu, RectTransform menuButton)
	{
		int cancelButton = 0;

		for (int i = 0; i < aboveMenu.underButtonsList.Count; i++)
			if (aboveMenu.underButtonsList [i] == menuButton)
				cancelButton = i;

		int delay = 0;

		SetInteractable (aboveMenu.underButtonsList [cancelButton]);
		aboveMenu.underButtonsList [cancelButton].DOAnchorPos (new Vector2(onScreenX, buttonsYPositions[cancelButton]), durationToShow).SetDelay (buttonsDelay [delay]).SetEase (easeMenu);
		headerButtonsList.RemoveAt (headerButtonsList.Count - 1);
		delay++;

		for(int i = aboveMenu.underButtonsList.Count - 1; i >= 0; i--)
		{
			if(i != cancelButton)
			{
				Enable (aboveMenu.underButtonsList [i]);
				SetInteractable (aboveMenu.underButtonsList [i]);

				aboveMenu.underButtonsList [i].DOAnchorPosX (onScreenX, durationToShow).SetDelay (buttonsDelay [delay]).SetEase (easeMenu);
				delay++;
			}
		}

		aboveMenu.underButtonsList [0].gameObject.GetComponent<Button> ().Select ();

		currentMenu = aboveMenu;
	}


	float HeaderButtonPosition ()
	{
		return headerButtonsYPosition - gapBetweenButtons * headerButtonsList.Count;
	}

	void Enable (RectTransform target, float delayDuration = 0)
	{
		if (delayDuration == 0)
			target.gameObject.SetActive (true);
		else
			StartCoroutine (EnableDelay (target, delayDuration));
	}

	IEnumerator EnableDelay (RectTransform target, float delayDuration)
	{
		yield return new WaitForSeconds (delayDuration);

		target.gameObject.SetActive (true);
	}

	void Disable (RectTransform target, float delayDuration = 0)
	{
		if (delayDuration == 0)
			target.gameObject.SetActive (false);
		else
			StartCoroutine (DisableDelay (target, delayDuration));
	}

	IEnumerator DisableDelay (RectTransform target, float delayDuration)
	{
		yield return new WaitForSeconds (delayDuration);

		target.gameObject.SetActive (false);
	}

	void SetInteractable (RectTransform target)
	{
		target.GetComponent<Button> ().interactable = true;

		Navigation navTemp = target.GetComponent<Button> ().navigation;
		navTemp.mode = Navigation.Mode.Automatic;

		target.GetComponent<Button> ().navigation = navTemp;
	}

	void SetNonInteractable (RectTransform target)
	{
		target.GetComponent<Button> ().interactable = false;

		Navigation navTemp = target.GetComponent<Button> ().navigation;
		navTemp.mode = Navigation.Mode.None;

		target.GetComponent<Button> ().navigation = navTemp;
	}

	void Tweening ()
	{
		tweening = true;
	}

	void NotTweening ()
	{
		tweening = false;
	}
}
