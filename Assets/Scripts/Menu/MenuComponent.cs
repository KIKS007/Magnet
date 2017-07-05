using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using NUnit.Framework;

public enum MenuComponentType { BasicMenu, MainMenu, RootMenu };

public enum MenuContentType { Menus, Buttons, MainContent, SecondaryContent };

public class MenuComponent : MonoBehaviour 
{
	public MenuComponentType menuComponentType;

	[Header ("Secondary Content")]
	public List<SecondaryContent> secondaryContents;

	[Header ("Selectable")]
	public GameObject selectable;
	[HideInInspector]
	public GameObject previousSelected;

	[HideInInspector]
	public RectTransform menuButton;
	public MenuComponent aboveMenuScript;
	[HideInInspector]
	public List<RectTransform> underMenus;
	[HideInInspector]
	public List<RectTransform> underMenusButtons;
	[HideInInspector]
	public RectTransform mainContent;
	[HideInInspector]
	public RectTransform menusParent;

	void Awake ()
	{
		SetupMenu ();
	}

	#region Setup
	public void SetupMenu ()
	{
		//GET ABOVE MENU
		if(menuComponentType == MenuComponentType.BasicMenu)
		{
			if(transform.parent.parent.GetComponent<MenuComponent> () != null)
				aboveMenuScript = transform.parent.parent.GetComponent<MenuComponent> ();
			else
				aboveMenuScript = transform.GetComponentInParent <MenuComponent> ();
		}

		//CLEAR ALL
		underMenus.Clear ();
		underMenusButtons.Clear ();

		//MENU BUTTON
		if(menuComponentType == MenuComponentType.BasicMenu)
		{
			menuButton = transform.GetChild (0).GetComponent<RectTransform> ();
			menuButton.GetComponent<MenuButtonComponent> ().menuComponentParent = this;
			menuButton.GetComponent<Button> ().interactable = false;
		}

		//UNDER MENUS
		if(transform.Find ("Menus") != null)
		{
			menusParent = transform.Find ("Menus").GetComponent<RectTransform> ();
			menusParent.gameObject.SetActive (true);

			for(int i = 0; i < menusParent.childCount; i++)
				underMenus.Add (menusParent.GetChild (i).GetComponent<RectTransform> ());
		}


		for (int i = 0; i < underMenus.Count; i++)
		{
			if (underMenus [i].transform.childCount == 0)
				underMenus [i].transform.GetComponent<MenuComponent> ().SetupMenu ();

			underMenusButtons.Add (underMenus [i].transform.GetChild (0).GetComponent<RectTransform> ());
		}

		//Setup Buttons Child Index
		for (int i = 0; i < underMenusButtons.Count; i++)
			underMenusButtons [i].GetComponent<MenuButtonComponent> ().buttonIndex = i;

		//CONTENT
		if(transform.Find ("MainContent") != null)
		{
			mainContent = transform.Find ("MainContent").GetComponent<RectTransform> ();
			mainContent.gameObject.SetActive (true);
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

		EnableSecondaryContentParent ();
	}

	void EnableUnderMenus ()
	{
		foreach (Transform t in transform)
		{
			if(t.name != "MainContent" && t.name != "Menus" && t.gameObject != menuButton)
				t.gameObject.SetActive (true);
			
		}

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
			menuButton.anchoredPosition = new Vector2 (MenuManager.Instance.menuOffScreen.x, menuButton.anchoredPosition.y);
//			menuButton.anchoredPosition = new Vector2 (MenuManager.Instance.menuOffScreenX, menuButton.anchoredPosition.y);

		//UNDER MENUS BUTTONS
		for (int i = 0; i < underMenusButtons.Count; i++)
			underMenusButtons[i].anchoredPosition = new Vector2(MenuManager.Instance.menuOffScreen.x, MenuManager.Instance.MenuButtonYPos (i));
//			underMenusButtons[i].anchoredPosition = new Vector2(MenuManager.Instance.menuOffScreenX, MenuManager.Instance.MenuButtonYPos (i));

		//CONTENT
		if(mainContent != null)
			mainContent.anchoredPosition = new Vector2 (MenuManager.Instance.menuOffScreen.x, mainContent.anchoredPosition.y);
//			mainContent.anchoredPosition = new Vector2 (MenuManager.Instance.menuOffScreenX, mainContent.anchoredPosition.y);

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
	#endregion

	//Menu Manager Call Methods
	public void Submit (int buttonIndex)
	{
		MenuManager.Instance.SubmitMenu (this, buttonIndex);
	}

	public void Submit (MenuComponent target)
	{
		MenuManager.Instance.SubmitMenu (target);
	}

	public void Cancel ()
	{
		if(menuComponentType == MenuComponentType.BasicMenu)
			MenuManager.Instance.CancelMenu (this, menuButton.GetComponent<MenuButtonComponent> ().buttonIndex);
	}

	[ButtonGroupAttribute ("Group B")]	
	public void ShowMenu ()
	{
		MenuManager.Instance.ShowMenu (this);
	}

	[ButtonGroupAttribute ("Group B")]	
	public void HideMenu ()
	{
		MenuManager.Instance.HideMenu (this);

		if(menuButton != null && menuButton.parent != transform)
		{
			menuButton.SetParent (transform);
			menuButton.SetAsFirstSibling ();
		}
	}

	#region Editor Methods
	[ButtonGroupAttribute ("Group A")]
	public void ShowMenuEditor ()
	{
		MenuManager menuManager = FindObjectOfType<MenuManager> ();

		//GET ABOVE MENU
		if(menuComponentType == MenuComponentType.BasicMenu)
		{
			if(transform.parent.parent.GetComponent<MenuComponent> () != null)
				aboveMenuScript = transform.parent.parent.GetComponent<MenuComponent> ();
		}

		gameObject.SetActive (true);
		if(aboveMenuScript != null)
		aboveMenuScript.gameObject.SetActive (true);
		transform.parent.gameObject.SetActive (true);

		//CLEAR ALL
		underMenus.Clear ();
		underMenusButtons.Clear ();

		//MENU BUTTON
		if(menuComponentType == MenuComponentType.BasicMenu)
		{
			menuButton = transform.GetChild (0).GetComponent<RectTransform> ();
			menuButton.gameObject.SetActive (true);

			if (transform.parent.GetComponent<MenuScrollRect> () != null)
			{
				menuButton.SetParent (aboveMenuScript.transform);
				menuButton.SetSiblingIndex (0);
			}

			menuButton.anchoredPosition = new Vector2(menuManager.menuOnScreen.x, menuManager.menuHeaderY);
//			menuButton.anchoredPosition = new Vector2(menuManager.menuOnScreenX, menuManager.menuHeaderY);
		}

		//UNDER MENUS
		if(transform.Find ("Menus") != null)
		{
			menusParent = transform.Find ("Menus").GetComponent<RectTransform> ();
			menusParent.gameObject.SetActive (true);

			for(int i = 0; i < menusParent.childCount; i++)
			{
				underMenus.Add (menusParent.GetChild (i).GetComponent<RectTransform> ());
				underMenus [i].gameObject.SetActive (true);
			}
		}

		for (int i = 0; i < underMenus.Count; i++)
			underMenusButtons.Add (underMenus [i].transform.GetChild (0).GetComponent<RectTransform> ());

		//UNDER MENUS BUTTONS
		for (int i = 0; i < underMenusButtons.Count; i++)
		{
			underMenusButtons [i].anchoredPosition = new Vector2 (menuManager.menuOnScreen.x, menuManager.MenuButtonYPos (i) + menuManager.GapAfterHeaderButton ()) - menusParent.anchoredPosition;
//			underMenusButtons [i].anchoredPosition = new Vector2 (menuManager.menuOnScreenX, menuManager.MenuButtonYPos (i) + menuManager.GapAfterHeaderButton ()) - menusParent.anchoredPosition;

			underMenusButtons [i].gameObject.SetActive (true);
		}

		//CONTENT
		if(transform.Find ("MainContent") != null)
		{
			mainContent = transform.Find ("MainContent").GetComponent<RectTransform> ();
			mainContent.gameObject.SetActive (true);

			mainContent.anchoredPosition = menuManager.onScreenContent;
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

		for(int i = 0; i < secondaryContents.Count; i++)
		{
			secondaryContents [i].content.gameObject.SetActive (true);
			secondaryContents [i].content.anchoredPosition = secondaryContents [i].onScreenPos;
		}

		EnableUnderMenus ();
		EnableSecondaryContentParent ();
	}

	[ButtonGroupAttribute ("Group A")]
	public void HideMenuEditor ()
	{
		gameObject.SetActive (false);
		transform.parent.gameObject.SetActive (false);

		//GET ABOVE MENU
		if(menuComponentType == MenuComponentType.BasicMenu)
		{
			if(transform.parent.parent.GetComponent<MenuComponent> () != null)
				aboveMenuScript = transform.parent.parent.GetComponent<MenuComponent> ();
		}

		//CLEAR ALL
		underMenus.Clear ();
		underMenusButtons.Clear ();

		//MENU BUTTON
		if(menuComponentType == MenuComponentType.BasicMenu)
		{
			if (transform.childCount == 0 || transform.childCount > 0 && transform.GetChild (0).GetComponent<MenuButtonComponent> () == null)
			{
				menuButton = aboveMenuScript.transform.GetChild (0).GetComponent<RectTransform> ();

				if (transform.parent.GetComponent<MenuScrollRect> () != null)
				{
					menuButton.SetParent (transform);
					menuButton.SetSiblingIndex (0);
				}
			}

			menuButton = transform.GetChild (0).GetComponent<RectTransform> ();
			menuButton.gameObject.SetActive (false);
		}

		//UNDER MENUS
		if(transform.Find ("Menus") != null)
		{
			menusParent = transform.Find ("Menus").GetComponent<RectTransform> ();
			menusParent.gameObject.SetActive (false);

			for(int i = 0; i < menusParent.childCount; i++)
			{
				underMenus.Add (menusParent.GetChild (i).GetComponent<RectTransform> ());
				underMenus [i].gameObject.SetActive (false);
			}
		}

		for (int i = 0; i < underMenus.Count; i++)
			underMenusButtons.Add (underMenus [i].transform.GetChild (0).GetComponent<RectTransform> ());

		//UNDER MENUS BUTTONS
		for (int i = 0; i < underMenusButtons.Count; i++)
			underMenusButtons [i].gameObject.SetActive (false);

		//CONTENT
		if(transform.Find ("MainContent") != null)
		{
			mainContent = transform.Find ("MainContent").GetComponent<RectTransform> ();
			mainContent.gameObject.SetActive (false);
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

		for(int i = 0; i < secondaryContents.Count; i++)
			secondaryContents [i].content.gameObject.SetActive (false);

		//CLEAR ALL
		underMenus.Clear ();
		underMenusButtons.Clear ();
		aboveMenuScript = null;
		menuButton = null;
		menusParent = null;
		mainContent = null;
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
