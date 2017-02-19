using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum MenuComponentType {Content, ButtonsList, MenusList, MainMenu, EndMode};

public enum MenuContentType { Menus, Buttons, MainContent, SecondaryContent };

public class MenuComponent : MonoBehaviour 
{
	public MenuComponentType menuComponentType;
	public bool viewportContent = false;

	[Header ("Content Order")]
	public List<MenuContent> contentDisplay = new List<MenuContent> ();

	[Header ("Secondary Content")]
	public List<SecondaryContent> secondaryContents;

	[Header ("Selectable")]
	public GameObject selectable;
	[HideInInspector]
	public GameObject previousSelected;

	public RectTransform menuButton;
	[HideInInspector]
	public MenuComponent aboveMenuScript;
	[HideInInspector]
	public List<RectTransform> otherMenusButtons;
	[HideInInspector]
	public List<RectTransform> underMenus;
	[HideInInspector]
	public List<RectTransform> underMenusButtons;
	[HideInInspector]
	public List<RectTransform> underButtons;
	[HideInInspector]
	public RectTransform mainContent;

	[Header ("End Mode Content List")]
	public RectTransform[] endModeContents = new RectTransform[0];

	[HideInInspector]
	public List<Vector2> underMenusButtonsPositions;
	[HideInInspector]
	public List<Vector2> underButtonsPositions;

	void Awake ()
	{
		if (menuComponentType == MenuComponentType.MainMenu)
			SetupMenu ();

		/*if (menuComponentType == MenuComponentType.MainMenu)
			MainMenuSetup ();

		if (menuComponentType == MenuComponentType.EndMode)
			EnableSecondaryContentParent ();*/
	}

	public void SetupMenu ()
	{
		Transform temp = null;

		//GET ABOVE MENU
		if(menuComponentType != MenuComponentType.MainMenu && menuComponentType != MenuComponentType.EndMode)
		{
			if(transform.parent.parent.GetComponent<MenuComponent> () != null)
				aboveMenuScript = transform.parent.parent.GetComponent<MenuComponent> ();
		}

		//CLEAR ALL
		underMenus.Clear ();
		underMenusButtons.Clear ();
		underButtons.Clear ();

		//MENU BUTTON
		if(menuComponentType != MenuComponentType.MainMenu && menuComponentType != MenuComponentType.EndMode)
			menuButton = transform.GetChild (0).GetComponent<RectTransform> ();

		//UNDER MENUS
		if(transform.Find ("Menus") != null)
		{
			temp = transform.Find ("Menus");

			for(int i = 0; i < temp.childCount; i++)
				underMenus.Add (temp.GetChild (i).GetComponent<RectTransform> ());
		}

		for (int i = 0; i < underMenus.Count; i++)
			underMenusButtons.Add (underMenus [i].transform.GetChild (0).GetComponent<RectTransform> ());

		//Setup Buttons Child Index
		for (int i = 0; i < underMenusButtons.Count; i++)
			underMenusButtons [i].GetComponent<MenuButtonComponent> ().buttonIndex = i;

		//UNDER BUTTONS
		if(transform.Find ("Buttons") != null)
		{
			temp = transform.Find ("Buttons");

			for(int i = 0; i < temp.childCount; i++)
				underButtons.Add (temp.GetChild (i).GetComponent<RectTransform> ());
		}

		//CONTENT
		if(transform.Find ("MainContent") != null)
			mainContent = transform.Find ("MainContent").GetComponent<RectTransform> ();

		//SETUP UNDER MENUS
		if(underMenus.Count > 0)
			for (int i = 0; i < underMenus.Count; i++)
				underMenus [i].GetComponent<MenuComponent> ().SetupMenu ();
		
		HideAll ();

		DisableAll ();

		EnableUnderMenus ();

		SetupButtonsNavigation (underMenusButtons);
		SetupButtonsNavigation (underButtons);

		EnableSecondaryContentParent ();

		if (contentDisplay.Count == 0)
			SetupContentDisplay ();
	}


	public void MainMenuSetup ()
	{
		underMenus.Clear ();
		underMenusButtons.Clear ();

		for(int i = 0; i < transform.childCount; i++)
			underMenus.Add (transform.GetChild (i).GetComponent<RectTransform> ());

		for (int i = 0; i < underMenus.Count; i++)
			underMenusButtons.Add (underMenus [i].transform.GetChild (0).GetComponent<RectTransform> ());
		
		//Setup Buttons Child Index
		for (int i = 0; i < underMenusButtons.Count; i++)
			underMenusButtons [i].GetComponent<MenuButtonComponent> ().buttonIndex = i;

		//Call Setup In Under Menu
		if(underMenus.Count > 0)
			for (int i = 0; i < underMenus.Count; i++)
				underMenus [i].GetComponent<MenuComponent> ().OtherMenuSetup ();

		HideAll ();

		DisableAll ();

		EnableUnderMenus ();

		SetupButtonsNavigation (underMenusButtons);

		EnableSecondaryContentParent ();
	}

	public void OtherMenuSetup ()
	{
		underMenus.Clear ();
		underMenusButtons.Clear ();

		//Get Menu Button
		menuButton = transform.GetChild (0).GetComponent<RectTransform> ();

		//Get Above Menu
		if(transform.parent.GetComponent<MenuComponent> () != null)
			aboveMenuScript = transform.parent.GetComponent<MenuComponent> ();

		else if(transform.parent.parent.GetComponent<MenuComponent> () != null && transform.parent.parent.GetComponent<MenuComponent> ().viewportContent)
			aboveMenuScript = transform.parent.parent.GetComponent<MenuComponent> ();		

		otherMenusButtons = aboveMenuScript.underMenusButtons;

		//Get MenusList or Content
		switch (menuComponentType)
		{
		case MenuComponentType.MenusList:
			SetupMenusList ();
			break;
		case MenuComponentType.Content:
			SetupContent ();	
			break;
		case MenuComponentType.ButtonsList:
			SetupButtonsList ();
			break;
		}

		//Call Setup In Under Menu
		if(underMenus.Count > 0)
			for (int i = 0; i < underMenus.Count; i++)
				underMenus [i].GetComponent<MenuComponent> ().OtherMenuSetup ();

		HideAll ();

		DisableAll ();

		EnableSecondaryContentParent ();
	}

	void SetupMenusList ()
	{
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

		//Setup Buttons Child Index
		for (int i = 0; i < underMenusButtons.Count; i++)
			underMenusButtons [i].GetComponent<MenuButtonComponent> ().buttonIndex = i;

		EnableUnderMenus ();
		SetupButtonsNavigation (underMenusButtons);
	}

	void SetupButtonsList ()
	{
		Transform mainParent = transform.GetChild (1);

		for (int i = 0; i < mainParent.childCount; i++)
			underMenusButtons.Add (mainParent.GetChild (i).GetComponent<RectTransform> ());

		SetupButtonsNavigation (underMenusButtons);
	}

	void SetupContent ()
	{
		mainContent = transform.GetChild (1).GetComponent<RectTransform> ();		
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
		if(menuComponentType != MenuComponentType.MainMenu)
			menuButton.anchoredPosition = new Vector2 (MenuManager.Instance.offScreenX, menuButton.anchoredPosition.y);
		
		//UNDER MENUS BUTTONS
		for (int i = 0; i < underMenusButtons.Count; i++)
			underMenusButtons[i].anchoredPosition = new Vector2(MenuManager.Instance.offScreenX, MenuManager.Instance.ButtonsYPos (i));

		//UNDER BUTTONS
		for (int i = 0; i < underButtons.Count; i++)
			underButtons[i].anchoredPosition = new Vector2(MenuManager.Instance.offScreenX, MenuManager.Instance.ButtonsYPos (i));

		//CONTENT
		if(mainContent != null)
			mainContent.anchoredPosition = new Vector2 (MenuManager.Instance.offScreenX, mainContent.anchoredPosition.y);

		//SECONDARY CONTENT
		if (secondaryContents.Count > 0)
			for (int i = 0; i < secondaryContents.Count; i++)
				secondaryContents [i].content.anchoredPosition = secondaryContents [i].offScreenPos;
	}

	void DisableAll ()
	{
		//MENU BUTTON
		if(menuComponentType != MenuComponentType.MainMenu)
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
			contentDisplay [0].contentType = MenuContentType.Menus;
		}

		//UNDER BUTTONS
		if(underButtons.Count > 0)
		{
			contentDisplay.Add (new MenuContent ());
			contentDisplay [0].contentType = MenuContentType.Buttons;
		}

		//CONTENT
		if(mainContent != null)
		{
			contentDisplay.Add (new MenuContent ());
			contentDisplay [0].contentType = MenuContentType.MainContent;
		}

		//SECONDARY CONTENT
		if(secondaryContents.Count > 0)
		{
			contentDisplay.Add (new MenuContent ());
			contentDisplay [0].contentType = MenuContentType.SecondaryContent;
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
		if(menuComponentType != MenuComponentType.MainMenu && menuComponentType != MenuComponentType.EndMode)
			MenuManager.Instance.CancelMenu (this, menuButton.GetComponent<MenuButtonComponent> ().buttonIndex);

		if(menuButton != null && menuButton.parent != transform)
		{
			menuButton.SetParent (transform);
			menuButton.SetAsFirstSibling ();
		}
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
		if (menuComponentType == MenuComponentType.MenusList && viewportContent)
				SetButtonsPositions ();
	}

	void SetButtonsPositions ()
	{
		if (menuComponentType == MenuComponentType.MenusList && viewportContent)
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
		MenuManager menuManager = GameObject.FindGameObjectWithTag ("MenuManager").GetComponent<MenuManager> ();

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


		if(menuComponentType != MenuComponentType.MainMenu || menuComponentType != MenuComponentType.EndMode)
		{
			inEditorHeaderButtons.Clear ();
			
			SetInEditorHeaderButtons (this);
			
			for (int i = 0; i < inEditorHeaderButtons.Count; i++)
				inEditorHeaderButtons [i].anchoredPosition = new Vector2 (menuManager.onScreenX, menuManager.headerButtonsYPosition - menuManager.gapBetweenButtons * i);
		}
	}

	[HideInInspector]
	public List<RectTransform> inEditorHeaderButtons = new List<RectTransform> ();

	void SetInEditorHeaderButtons (MenuComponent menu)
	{
		inEditorHeaderButtons.Insert (0, menu.menuButton);

		if (menu.aboveMenuScript && menu.aboveMenuScript.menuComponentType == MenuComponentType.MenusList)
		{
			menu.aboveMenuScript.menuButton = menu.aboveMenuScript.transform.GetChild (0).GetComponent<RectTransform> ();

			SetInEditorHeaderButtons (menu.aboveMenuScript);
		}
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
}

[Serializable]
public class MenuContent 
{
	public MenuContentType contentType;
	public bool waitPreviousContent = false;
	public float delay = 0;
}
