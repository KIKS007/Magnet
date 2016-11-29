using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MenuComponent : MonoBehaviour 
{
	public MenuComponentType menuComponentType;

	[Header ("Above Menu")]
	public MenuComponent aboveMenuScript;

	[Header ("Other Menu")]
	public List<RectTransform> otherMenuList;
	public List<RectTransform> otherButtonsList;

	[Header ("Under Menu")]
	public List<RectTransform> underMenuList;
	public List<RectTransform> underButtonsList;

	[Header ("Button")]
	public RectTransform button;

	[Header ("Content")]
	public RectTransform content = null;

	[Header ("Selectable")]
	public GameObject selectable;

	[Header ("Secondary Content")]
	public List<SecondaryContent> secondaryContentList;

	void Awake ()
	{
		if (menuComponentType == MenuComponentType.MainMenu)
			MainMenuSetup ();
	}

	public void MainMenuSetup ()
	{
		for(int i = 0; i < transform.childCount; i++)
			underMenuList.Add (transform.GetChild (i).GetComponent<RectTransform> ());

		for (int i = 0; i < underMenuList.Count; i++)
			underButtonsList.Add (underMenuList [i].transform.GetChild (0).GetComponent<RectTransform> ());
		
		//Setup Buttons Child Index
		for (int i = 0; i < underButtonsList.Count; i++)
			underButtonsList [i].GetComponent<MenuButtonComponent> ().buttonIndex = i;

		//Call Setup In Under Menu
		if(underMenuList.Count > 0)
			for (int i = 0; i < underMenuList.Count; i++)
				underMenuList [i].GetComponent<MenuComponent> ().OtherMenuSetup ();

		HideAll ();

		DisableAll ();

		EnableUnderMenus ();

		SetupButtonsNavigation ();
	}

	public void OtherMenuSetup ()
	{
		/*if (transform.childCount > 2)
			menuComponentType = MenuComponentType.ButtonsListMenu;
		else
			menuComponentType = MenuComponentType.ContentMenu;*/

		//Get Menu Button
		button = transform.GetChild (0).GetComponent<RectTransform> ();

		//Get Above Menu
		aboveMenuScript = transform.parent.GetComponent<MenuComponent> ();

		otherMenuList = aboveMenuScript.underMenuList;
		otherButtonsList = aboveMenuScript.underButtonsList;

		//Get Buttons List or Content
		if(menuComponentType == MenuComponentType.ButtonsListMenu)
		{
			for(int i = 1; i < transform.childCount; i++)
				underMenuList.Add (transform.GetChild (i).GetComponent<RectTransform> ());

			for (int i = 0; i < underMenuList.Count; i++)
				underButtonsList.Add (underMenuList [i].transform.GetChild (0).GetComponent<RectTransform> ());
			
			//Setup Buttons Child Index
			for (int i = 0; i < underButtonsList.Count; i++)
				underButtonsList [i].GetComponent<MenuButtonComponent> ().buttonIndex = i;

			EnableUnderMenus ();
			SetupButtonsNavigation ();
		}
		else
			content = transform.GetChild (1).GetComponent<RectTransform> ();		

		//Call Setup In Under Menu
		if(underMenuList.Count > 0)
			for (int i = 0; i < underMenuList.Count; i++)
				underMenuList [i].GetComponent<MenuComponent> ().OtherMenuSetup ();

		HideAll ();

		DisableAll ();
	}

	void EnableUnderMenus ()
	{
		for (int i = 0; i < underMenuList.Count; i++)
			underMenuList [i].gameObject.SetActive (true);
	}

	void HideAll ()
	{
		if(menuComponentType == MenuComponentType.MainMenu || menuComponentType == MenuComponentType.ButtonsListMenu)
			for (int i = 0; i < underButtonsList.Count; i++)
				underButtonsList[i].anchoredPosition = new Vector2(MenuManager.Instance.offScreenX, MenuManager.Instance.buttonsYPositions [i]);

		if (button != null)
			button.anchoredPosition = new Vector2 (MenuManager.Instance.offScreenX, button.anchoredPosition.y);

		if (content != null)
			content.anchoredPosition = new Vector2 (MenuManager.Instance.offScreenX, content.anchoredPosition.y);
	}

	void DisableAll ()
	{
		if(menuComponentType == MenuComponentType.MainMenu || menuComponentType == MenuComponentType.ButtonsListMenu)
			for (int i = 0; i < underButtonsList.Count; i++)
				underButtonsList[i].gameObject.SetActive (false);
		
		if (button != null)
			button.gameObject.SetActive (false);

		if (content != null)
			content.gameObject.SetActive (false);

		if(secondaryContentList.Count > 0)
			for (int i = 0; i < secondaryContentList.Count; i++)
				secondaryContentList [i].content.gameObject.SetActive (false);
		
	}
		
	void SetupButtonsNavigation ()
	{
		for(int i = 0; i < underButtonsList.Count; i++)
		{
			Navigation nav = underButtonsList [i].GetComponent<Button> ().navigation;
			nav.mode = Navigation.Mode.Explicit;

			if(i == 0)
			{
				nav.selectOnUp = underButtonsList [underButtonsList.Count - 1].GetComponent<Button> ();
				nav.selectOnDown = underButtonsList [1].GetComponent<Button> ();
			}
			else if(i == underButtonsList.Count - 1)
			{
				nav.selectOnUp = underButtonsList [underButtonsList.Count - 2].GetComponent<Button> ();
				nav.selectOnDown = underButtonsList [0].GetComponent<Button> ();
			}
			else
			{
				nav.selectOnUp = underButtonsList [i - 1].GetComponent<Button> ();
				nav.selectOnDown = underButtonsList [i + 1].GetComponent<Button> ();
			}

			underButtonsList [i].GetComponent<Button> ().navigation = nav;
		}
	}

	//Menu Manager Call Methods
	public void Submit (int buttonIndex)
	{
		if (menuComponentType == MenuComponentType.ButtonsListMenu || menuComponentType == MenuComponentType.MainMenu)
			MenuManager.Instance.ShowUnderButtons (otherButtonsList, buttonIndex, underButtonsList, this, secondaryContentList);
		
		else
			MenuManager.Instance.ShowContent (otherButtonsList, buttonIndex, content, this, secondaryContentList);
	}

	public void Cancel ()
	{
		if (menuComponentType == MenuComponentType.ButtonsListMenu)
			MenuManager.Instance.HideUnderButtons (underButtonsList, aboveMenuScript, button, false, secondaryContentList);
		
		else if (menuComponentType == MenuComponentType.ContentMenu)
			MenuManager.Instance.HideContent (content, aboveMenuScript, button, false, secondaryContentList);
	}

	public void Resume ()
	{
		switch (menuComponentType)
		{
		case MenuComponentType.MainMenu:
			MenuManager.Instance.HideMainMenu ();
			break;
		case MenuComponentType.ContentMenu:
			MenuManager.Instance.HideContent (content, aboveMenuScript, button, true, secondaryContentList);
			break;
		case MenuComponentType.ButtonsListMenu:
			MenuManager.Instance.HideUnderButtons (underButtonsList, aboveMenuScript, button, true, secondaryContentList);
			break;
		}
	}
}

[Serializable]
public class SecondaryContent
{
	public RectTransform content;
	public Vector2 onScreenPos;
	public Vector2 offScreenPos;
	public float delay = 0;
}
