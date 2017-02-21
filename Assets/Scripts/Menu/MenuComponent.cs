using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum MenuComponentType { BasicMenu, MainMenu, EndModeMenu, RootMenu };

public enum MenuContentType { Menus, Buttons, MainContent, SecondaryContent };

public class MenuComponent : MonoBehaviour 
{
	public MenuComponentType menuComponentType;
	[HideInInspector]
	public bool scrollViewButtons = false;
	[HideInInspector]
	public bool scrollViewContent = false;

	[Header ("Content Order")]
	public List<MenuContent> contentDisplay = new List<MenuContent> ();

	[Header ("Secondary Content")]
	public List<SecondaryContent> secondaryContents;

	[Header ("Selectable")]
	public GameObject selectable;
	[HideInInspector]
	public GameObject previousSelected;

	[Header ("End Mode Content List")]
	public RectTransform[] endModeContents = new RectTransform[0];

	[Header ("Override Headers Properties")]
	public bool overrideHeaderPos = false;
	public float menuHeaderY = 540;
	public float menuFirstButtonY = 278;
	public float buttonFirstButtonY = 278;

	[Header ("Override Positions Properties")]
	public bool overrideMenuPos = false;
	public float menuOffScreenX = -2000;
	public float menuOnScreenX = -650;

	public bool overrideButtonPos = false;
	public float buttonOffScreenX = -2000;
	public float buttonOnScreenX = -650;

	public bool overrideContentPos = false;
	public Vector2 offScreenContent = new Vector2 (-2000, 0);
	public Vector2 onScreenContent = new Vector2 (0, 0);

	[Header ("Override Display Properties")]
	public bool overrideButtonsDisplay = false;
	public bool showButtonsOnSubmit = true;
	public bool hideButtonsOnCancel = true;
	public bool hideButtonsOnUnderSubmit = true;

	public bool overrideContentDisplay = false;
	public bool showContentOnSubmit = true;
	public bool hideContentOnCancel = true;
	public bool hideContentOnUnderSubmit = true;

	public RectTransform menuButton;
	[HideInInspector]
	public MenuComponent aboveMenuScript;
	[HideInInspector]
	public List<RectTransform> underMenus;
	[HideInInspector]
	public List<RectTransform> underMenusButtons;
	[HideInInspector]
	public List<RectTransform> underButtons;
	[HideInInspector]
	public RectTransform mainContent;

	[HideInInspector]
	public List<Vector2> underMenusButtonsPositions;
	[HideInInspector]
	public List<Vector2> underButtonsPositions;

	[HideInInspector]
	public RectTransform menusParent;
	[HideInInspector]
	public RectTransform buttonsParent;
	[HideInInspector]
	public RectTransform mainContentParent;

	void Awake ()
	{
		SetupMenu ();

		if (menuComponentType == MenuComponentType.EndModeMenu)
			EnableSecondaryContentParent ();
	}

	public void SetupMenu ()
	{
		//GET ABOVE MENU
		if(menuComponentType == MenuComponentType.BasicMenu)
		{
			if(transform.parent.parent.GetComponent<MenuComponent> () != null)
				aboveMenuScript = transform.parent.parent.GetComponent<MenuComponent> ();
		}

		//CLEAR ALL
		underMenus.Clear ();
		underMenusButtons.Clear ();
		underButtons.Clear ();

		//MENU BUTTON
		if(menuComponentType == MenuComponentType.BasicMenu)
		{
			menuButton = transform.GetChild (0).GetComponent<RectTransform> ();
			menuButton.GetComponent<MenuButtonComponent> ().menuComponentParent = this;
		}

		//UNDER MENUS
		if(transform.Find ("Menus") != null)
		{
			menusParent = transform.Find ("Menus").GetComponent<RectTransform> ();
			menusParent.gameObject.SetActive (true);

			if (menusParent.GetComponent<MenuScrollView> () != null)
				scrollViewButtons = true;

			for(int i = 0; i < menusParent.childCount; i++)
				underMenus.Add (menusParent.GetChild (i).GetComponent<RectTransform> ());
		}

		for (int i = 0; i < underMenus.Count; i++)
			underMenusButtons.Add (underMenus [i].transform.GetChild (0).GetComponent<RectTransform> ());

		//Setup Buttons Child Index
		for (int i = 0; i < underMenusButtons.Count; i++)
			underMenusButtons [i].GetComponent<MenuButtonComponent> ().buttonIndex = i;

		//UNDER BUTTONS
		if(transform.Find ("Buttons") != null)
		{
			buttonsParent = transform.Find ("Buttons").GetComponent<RectTransform> ();
			buttonsParent.gameObject.SetActive (true);

			if (buttonsParent.GetComponent<MenuScrollView> () != null)
				scrollViewButtons = true;

			for(int i = 0; i < buttonsParent.childCount; i++)
			{
				underButtons.Add (buttonsParent.GetChild (i).GetComponent<RectTransform> ());
				buttonsParent.GetChild (i).GetComponent<MenuButtonComponent> ().menuComponentParent = this;
			}
		}

		//CONTENT
		if(transform.Find ("MainContent") != null)
		{
			mainContentParent = transform.Find ("MainContent").GetComponent<RectTransform> ();
			mainContentParent.gameObject.SetActive (true);

			if (mainContentParent.GetComponent<MenuScrollView> () != null)
				scrollViewContent = true;
			
			mainContent = transform.Find ("MainContent").GetComponent<RectTransform> ();
		}

		//SECONDARY CONTENT
		bool sorted = false;

		do
		{
			sorted = true;

			for (int i = 0; i < secondaryContents.Count; i++)
				if(secondaryContents [i].content == null)
				{
					secondaryContents.RemoveAt (i);
					sorted = false;
				}
		}
		while (!sorted);


		HideAll ();

		DisableAll ();

		EnableUnderMenus ();

		SetupButtonsNavigation (underMenusButtons);
		SetupButtonsNavigation (underButtons);

		EnableSecondaryContentParent ();

		if (contentDisplay.Count == 0)
			SetupContentDisplay ();
	}


	void EnableUnderMenus ()
	{
		for (int i = 0; i < underMenus.Count; i++)
			underMenus [i].gameObject.SetActive (true);
	}

	void EnableSecondaryContentParent ()
	{
		if(secondaryContents.Count > 0)
			for (int i = 0; i < secondaryContents.Count; i++)
				if(secondaryContents [i].content.transform.parent != null)
					secondaryContents [i].content.transform.parent.gameObject.SetActive (true);
	}

	void HideAll ()
	{
		//MENU BUTTON
		if(menuComponentType == MenuComponentType.BasicMenu)
			menuButton.anchoredPosition = new Vector2 (MenuManager.Instance.menuOffScreenX, menuButton.anchoredPosition.y);
		
		//UNDER MENUS BUTTONS
		for (int i = 0; i < underMenusButtons.Count; i++)
			underMenusButtons[i].anchoredPosition = new Vector2(MenuManager.Instance.menuOffScreenX, MenuManager.Instance.MenuButtonYPos (i));

		//UNDER BUTTONS
		for (int i = 0; i < underButtons.Count; i++)
			underButtons[i].anchoredPosition = new Vector2(MenuManager.Instance.menuOffScreenX, MenuManager.Instance.MenuButtonYPos (i));

		//CONTENT
		if(mainContent != null)
			mainContent.anchoredPosition = new Vector2 (MenuManager.Instance.menuOffScreenX, mainContent.anchoredPosition.y);

		//SECONDARY CONTENT
		if (secondaryContents.Count > 0)
			for (int i = 0; i < secondaryContents.Count; i++)
				secondaryContents [i].content.anchoredPosition = secondaryContents [i].offScreenPos;
	}

	void DisableAll ()
	{
		//MENU BUTTON
		if(menuComponentType == MenuComponentType.BasicMenu)
			menuButton.gameObject.SetActive (false);

		//UNDER MENUS BUTTONS
		for (int i = 0; i < underMenusButtons.Count; i++)
			underMenusButtons[i].gameObject.SetActive (false);

		//UNDER BUTTONS
		for (int i = 0; i < underMenusButtons.Count; i++)
			underMenusButtons[i].gameObject.SetActive (false);
		
		//CONTENT
		if(mainContent != null)
			mainContent.gameObject.SetActive (false);

		//SECONDARY CONTENT
		if(secondaryContents.Count > 0)
			for (int i = 0; i < secondaryContents.Count; i++)
				secondaryContents [i].content.gameObject.SetActive (false);
	}
		
	void SetupButtonsNavigation (List<RectTransform> buttonsList)
	{
		if (buttonsList.Count == 0)
			return;

		if(buttonsList.Count == 1)
		{
			Navigation nav = buttonsList [0].GetComponent<Button> ().navigation;
			nav.mode = Navigation.Mode.Automatic;

			buttonsList [0].GetComponent<Button> ().navigation = nav;
		}
		else
		{
			for(int i = 0; i < buttonsList.Count; i++)
			{
				Navigation nav = buttonsList [i].GetComponent<Button> ().navigation;
				nav.mode = Navigation.Mode.Explicit;
				
				if(i == 0)
				{
					nav.selectOnUp = buttonsList [buttonsList.Count - 1].GetComponent<Button> ();
					nav.selectOnDown = buttonsList [1].GetComponent<Button> ();
				}
				else if(i == buttonsList.Count - 1)
				{
					nav.selectOnUp = buttonsList [buttonsList.Count - 2].GetComponent<Button> ();
					nav.selectOnDown = buttonsList [0].GetComponent<Button> ();
				}
				else
				{
					nav.selectOnUp = buttonsList [i - 1].GetComponent<Button> ();
					nav.selectOnDown = buttonsList [i + 1].GetComponent<Button> ();
				}
				
				buttonsList [i].GetComponent<Button> ().navigation = nav;
			}
		}
	}

	void SetupContentDisplay ()
	{
		//UNDER MENUS
		if(underMenusButtons.Count > 0)
		{
			contentDisplay.Add (new MenuContent ());
			contentDisplay [contentDisplay.Count - 1].contentType = MenuContentType.Menus;
		}

		//UNDER BUTTONS
		if(underButtons.Count > 0)
		{
			contentDisplay.Add (new MenuContent ());
			contentDisplay [contentDisplay.Count - 1].contentType = MenuContentType.Buttons;
		}

		//CONTENT
		if(mainContent != null)
		{
			contentDisplay.Add (new MenuContent ());
			contentDisplay [contentDisplay.Count - 1].contentType = MenuContentType.MainContent;
		}

		//SECONDARY CONTENT
		if(secondaryContents.Count > 0)
		{
			contentDisplay.Add (new MenuContent ());
			contentDisplay [contentDisplay.Count - 1].contentType = MenuContentType.SecondaryContent;
		}
	}


	//Menu Manager Call Methods
	public void Submit (int buttonIndex)
	{
		if (aboveMenuScript != null)
			aboveMenuScript.OnUnderMenuSubmit ();

		MenuManager.Instance.SubmitMenu (this, buttonIndex);
	}

	public void Cancel ()
	{
		if(menuComponentType == MenuComponentType.BasicMenu)
			MenuManager.Instance.CancelMenu (this, menuButton.GetComponent<MenuButtonComponent> ().buttonIndex);

		SetButtonsPositions ();
	}

	public void ShowMenu ()
	{
		MenuManager.Instance.ShowMenu (this);
	}

	public void HideMenu ()
	{
		MenuManager.Instance.HideMenu (this);

		if(menuButton != null && menuButton.parent != transform)
		{
			menuButton.SetParent (transform);
			menuButton.SetAsFirstSibling ();
		}
	}


	public void EndMode (WhichMode whichMode)
	{
		bool foundContent = false;

		for(int i = 0; i < endModeContents.Length; i++)
		{
			if(endModeContents [i].name == whichMode.ToString ())
			{
				MenuManager.Instance.ShowEndMode (endModeContents [i], secondaryContents, this);
				foundContent = true;
				break;
			}
		}

		if(!foundContent)
		{
			Debug.LogWarning ("Missing Stats Content");
			MenuManager.Instance.ShowEndMode (endModeContents [0], secondaryContents, this);
		}

	}


	public void OnUnderMenuSubmit ()
	{
		SetButtonsPositions ();
	}

	void SetButtonsPositions ()
	{
		if (scrollViewButtons)
		{
			underMenusButtonsPositions.Clear ();
			underButtonsPositions.Clear ();

			for (int i = 0; i < underMenusButtons.Count; i++)
				underMenusButtonsPositions.Add (underMenusButtons [i].anchoredPosition);

			for (int i = 0; i < underButtons.Count; i++)
				underButtonsPositions.Add (underButtons [i].anchoredPosition);
		}
	}


	#region Editor Methods
	[ContextMenu ("Show Menu")]
	public void SetInEditorMenuPosition ()
	{
		/*MenuManager menuManager = GameObject.FindGameObjectWithTag ("MenuManager").GetComponent<MenuManager> ();

		underMenus.Clear ();
		underMenusButtons.Clear ();

		switch (menuComponentType)
		{
		case MenuComponentType.MainMenu:

			for(int i = 0; i < transform.childCount; i++)
				underMenus.Add (transform.GetChild (i).GetComponent<RectTransform> ());

			for (int i = 0; i < underMenus.Count; i++)
				underMenusButtons.Add (underMenus [i].transform.GetChild (0).GetComponent<RectTransform> ());


			for (int i = 0; i < underMenusButtons.Count; i++)
				underMenusButtons [i].anchoredPosition = new Vector2 (menuManager.onScreenX, menuManager.MainMenuButtonsYPos (i));
			break;

		case MenuComponentType.MenusList:
			menuButton = transform.GetChild (0).GetComponent<RectTransform> ();
			aboveMenuScript = transform.parent.GetComponent<MenuComponent> ();

			Transform mainParent = null;

			if (!viewportContent)
			{
				mainParent = transform;

				for(int i = 1; i < mainParent.childCount; i++)
					underMenus.Add (mainParent.GetChild (i).GetComponent<RectTransform> ());
			}
			else
			{
				mainParent = transform.GetChild (1);

				for(int i = 0; i < mainParent.childCount; i++)
					underMenus.Add (mainParent.GetChild (i).GetComponent<RectTransform> ());
			}

			for (int i = 0; i < underMenus.Count; i++)
				underMenusButtons.Add (underMenus [i].transform.GetChild (0).GetComponent<RectTransform> ());

			for (int i = 0; i < underMenusButtons.Count; i++)
				underMenusButtons [i].anchoredPosition = new Vector2 (menuManager.onScreenX, menuManager.ButtonsYPos (i));
			break;

		case MenuComponentType.Content:
			menuButton = transform.GetChild (0).GetComponent<RectTransform> ();
			aboveMenuScript = transform.parent.GetComponent<MenuComponent> ();

			mainContent = transform.GetChild (1).GetComponent<RectTransform> ();	

			mainContent.anchoredPosition = new Vector2 (0, 0);
			break;
		}
			
		EnableSecondaryContentParent ();

		for (int i = 0; i < secondaryContents.Count; i++)
			secondaryContents [i].content.gameObject.SetActive (true);

		for (int i = 0; i < secondaryContents.Count; i++)
			secondaryContents [i].content.anchoredPosition = secondaryContents [i].onScreenPos;


		if(menuComponentType != MenuComponentType.MainMenu || menuComponentType != MenuComponentType.EndModeMenu)
		{
			inEditorHeaderButtons.Clear ();
			
			SetInEditorHeaderButtons (this);
			
			for (int i = 0; i < inEditorHeaderButtons.Count; i++)
				inEditorHeaderButtons [i].anchoredPosition = new Vector2 (menuManager.onScreenX, menuManager.headerButtonsYPosition - menuManager.gapBetweenButtons * i);
		}*/
	}

	[HideInInspector]
	public List<RectTransform> inEditorHeaderButtons = new List<RectTransform> ();

	void SetInEditorHeaderButtons (MenuComponent menu)
	{
		/*inEditorHeaderButtons.Insert (0, menu.menuButton);

		if (menu.aboveMenuScript && menu.aboveMenuScript.menuComponentType == MenuComponentType.MenusList)
		{
			menu.aboveMenuScript.menuButton = menu.aboveMenuScript.transform.GetChild (0).GetComponent<RectTransform> ();

			SetInEditorHeaderButtons (menu.aboveMenuScript);
		}*/
	}
	#endregion
}

[Serializable]
public class SecondaryContent
{
	public RectTransform content;
	public Vector2 onScreenPos;
	public Vector2 offScreenPos;
	public float delay = 0;
	public bool showOnSubmit = true;
	public bool hideOnCancel = true;
	public bool hideOnUnderSubmit = true;
}

[Serializable]
public class MenuContent 
{
	public MenuContentType contentType;
	public bool waitPreviousContent = false;
	public float delay = 0;
}
