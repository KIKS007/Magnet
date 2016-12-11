using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum MenuComponentType {ContentMenu, ButtonsListMenu, MainMenu, EndModeMenu};

public class MenuComponent : MonoBehaviour 
{
	public MenuComponentType menuComponentType;
	public bool viewportContent = false;

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

	public GameObject selectable;

	public List<SecondaryContent> secondaryContentList;

	[Header ("End Mode Content List")]
	public RectTransform[] endModeContentList = new RectTransform[0];

	void Awake ()
	{
		if (menuComponentType == MenuComponentType.MainMenu)
			MainMenuSetup ();

		if (menuComponentType == MenuComponentType.EndModeMenu)
			EnableSecondaryContentParent ();
	}

	public void MainMenuSetup ()
	{
		underMenuList.Clear ();
		underButtonsList.Clear ();

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

		EnableSecondaryContentParent ();
	}

	public void OtherMenuSetup ()
	{
		underMenuList.Clear ();
		underButtonsList.Clear ();

		//Get Menu Button
		button = transform.GetChild (0).GetComponent<RectTransform> ();

		//Get Above Menu
		if(transform.parent.GetComponent<MenuComponent> () != null)
			aboveMenuScript = transform.parent.GetComponent<MenuComponent> ();
		
		else if(transform.parent.parent.GetComponent<MenuComponent> () != null && transform.parent.parent.GetComponent<MenuComponent> ().viewportContent == true)
			aboveMenuScript = transform.parent.parent.GetComponent<MenuComponent> ();

		otherMenuList = aboveMenuScript.underMenuList;
		otherButtonsList = aboveMenuScript.underButtonsList;

		//Get Buttons List or Content
		if (menuComponentType == MenuComponentType.ButtonsListMenu)
			SetupButtonsListMenu ();
		else
			SetupContent ();	

		//Call Setup In Under Menu
		if(underMenuList.Count > 0)
			for (int i = 0; i < underMenuList.Count; i++)
				underMenuList [i].GetComponent<MenuComponent> ().OtherMenuSetup ();

		HideAll ();

		DisableAll ();

		EnableSecondaryContentParent ();
	}

	void SetupButtonsListMenu ()
	{
		Transform mainParent = null;

		if (!viewportContent)
		{
			mainParent = transform;

			for(int i = 1; i < mainParent.childCount; i++)
				underMenuList.Add (mainParent.GetChild (i).GetComponent<RectTransform> ());
		}
		else
		{
			mainParent = transform.GetChild (1);
			
			for(int i = 0; i < mainParent.childCount; i++)
				underMenuList.Add (mainParent.GetChild (i).GetComponent<RectTransform> ());
		}

		for (int i = 0; i < underMenuList.Count; i++)
			underButtonsList.Add (underMenuList [i].transform.GetChild (0).GetComponent<RectTransform> ());

		//Setup Buttons Child Index
		for (int i = 0; i < underButtonsList.Count; i++)
			underButtonsList [i].GetComponent<MenuButtonComponent> ().buttonIndex = i;

		EnableUnderMenus ();
		SetupButtonsNavigation ();
	}

	void SetupContent ()
	{
		content = transform.GetChild (1).GetComponent<RectTransform> ();		
	}


	void EnableUnderMenus ()
	{
		for (int i = 0; i < underMenuList.Count; i++)
			underMenuList [i].gameObject.SetActive (true);
	}

	void EnableSecondaryContentParent ()
	{
		if(secondaryContentList.Count > 0)
			for (int i = 0; i < secondaryContentList.Count; i++)
				if(secondaryContentList [i].content.transform.parent != null)
					secondaryContentList [i].content.transform.parent.gameObject.SetActive (true);
	}

	void HideAll ()
	{
		if(menuComponentType == MenuComponentType.MainMenu || menuComponentType == MenuComponentType.ButtonsListMenu)
			for (int i = 0; i < underButtonsList.Count; i++)
				underButtonsList[i].anchoredPosition = new Vector2(MenuManager.Instance.offScreenX, MenuManager.Instance.ButtonsYPos (i));

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
		if (menuComponentType == MenuComponentType.MainMenu)
			MenuManager.Instance.ShowUnderButtons (otherButtonsList, buttonIndex, underButtonsList, this, secondaryContentList);

		else if (menuComponentType == MenuComponentType.ButtonsListMenu)
		{
			if(!viewportContent)
				MenuManager.Instance.ShowUnderButtons (otherButtonsList, buttonIndex, underButtonsList, this, secondaryContentList);
			else
				MenuManager.Instance.ShowViewportButtons (otherButtonsList, buttonIndex, underButtonsList, this, secondaryContentList);
		}
		
		else
			MenuManager.Instance.ShowContent (otherButtonsList, buttonIndex, content, this, secondaryContentList);
	}

	public void Cancel ()
	{
		if (menuComponentType == MenuComponentType.ButtonsListMenu)
		{
			if(!viewportContent)
				MenuManager.Instance.HideUnderButtons (underButtonsList, aboveMenuScript, button, false, secondaryContentList);
			else
				MenuManager.Instance.HideViewportButtons (underButtonsList, aboveMenuScript, button, false, secondaryContentList);
		}
		
		else if (menuComponentType == MenuComponentType.ContentMenu)
			MenuManager.Instance.HideContent (content, aboveMenuScript, button, false, secondaryContentList);
	}

	public void HideMenu ()
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
			if(!viewportContent)
				MenuManager.Instance.HideUnderButtons (underButtonsList, aboveMenuScript, button, true, secondaryContentList);
			else
				MenuManager.Instance.HideViewportButtons (underButtonsList, aboveMenuScript, button, true, secondaryContentList);
			break;
		}
	}

	public void EndMode (WhichMode whichMode)
	{
		MenuManager.Instance.ShowEndMode (endModeContentList [(int)whichMode], secondaryContentList, this);
	}

	#region Editor Methods
	[ContextMenu ("Show Menu")]
	public void SetInEditorMenuPosition ()
	{
		MenuManager menuManager = GameObject.FindGameObjectWithTag ("MenuManager").GetComponent<MenuManager> ();

		underMenuList.Clear ();
		underButtonsList.Clear ();

		switch (menuComponentType)
		{
		case MenuComponentType.MainMenu:

			for(int i = 0; i < transform.childCount; i++)
				underMenuList.Add (transform.GetChild (i).GetComponent<RectTransform> ());

			for (int i = 0; i < underMenuList.Count; i++)
				underButtonsList.Add (underMenuList [i].transform.GetChild (0).GetComponent<RectTransform> ());


			for (int i = 0; i < underButtonsList.Count; i++)
				underButtonsList [i].anchoredPosition = new Vector2 (menuManager.onScreenX, menuManager.MainMenuButtonsYPos (i));
			break;

		case MenuComponentType.ButtonsListMenu:
			button = transform.GetChild (0).GetComponent<RectTransform> ();
			aboveMenuScript = transform.parent.GetComponent<MenuComponent> ();

			Transform mainParent = null;

			if (!viewportContent)
			{
				mainParent = transform;

				for(int i = 1; i < mainParent.childCount; i++)
					underMenuList.Add (mainParent.GetChild (i).GetComponent<RectTransform> ());
			}
			else
			{
				mainParent = transform.GetChild (1);

				for(int i = 0; i < mainParent.childCount; i++)
					underMenuList.Add (mainParent.GetChild (i).GetComponent<RectTransform> ());
			}

			for (int i = 0; i < underMenuList.Count; i++)
				underButtonsList.Add (underMenuList [i].transform.GetChild (0).GetComponent<RectTransform> ());

			for (int i = 0; i < underButtonsList.Count; i++)
				underButtonsList [i].anchoredPosition = new Vector2 (menuManager.onScreenX, menuManager.ButtonsYPos (i));
			break;

		case MenuComponentType.ContentMenu:
			button = transform.GetChild (0).GetComponent<RectTransform> ();
			aboveMenuScript = transform.parent.GetComponent<MenuComponent> ();

			content = transform.GetChild (1).GetComponent<RectTransform> ();	

			content.anchoredPosition = new Vector2 (0, 0);
			break;
		}
			
		EnableSecondaryContentParent ();

		for (int i = 0; i < secondaryContentList.Count; i++)
			secondaryContentList [i].content.gameObject.SetActive (true);

		for (int i = 0; i < secondaryContentList.Count; i++)
			secondaryContentList [i].content.anchoredPosition = secondaryContentList [i].onScreenPos;


		if(menuComponentType != MenuComponentType.MainMenu)
		{
			inEditorHeaderButtons.Clear ();
			
			SetInEditorHeaderButtons (this);
			
			for (int i = 0; i < inEditorHeaderButtons.Count; i++)
				inEditorHeaderButtons [i].anchoredPosition = new Vector2 (menuManager.onScreenX, menuManager.headerButtonsYPosition - menuManager.gapBetweenButtons * i);
		}
	}

	public List<RectTransform> inEditorHeaderButtons = new List<RectTransform> ();

	void SetInEditorHeaderButtons (MenuComponent menu)
	{
		inEditorHeaderButtons.Insert (0, menu.button);

		if (menu.aboveMenuScript && menu.aboveMenuScript.menuComponentType == MenuComponentType.ButtonsListMenu)
		{
			menu.aboveMenuScript.button = menu.aboveMenuScript.transform.GetChild (0).GetComponent<RectTransform> ();

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
