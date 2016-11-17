using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuComponent : MonoBehaviour 
{
	public MenuComponentType menuComponentType;

	[Header ("Above Menu")]
	public List<RectTransform> aboveMenuList;
	public List<RectTransform> aboveButtonsList;

	[Header ("Button")]
	public RectTransform button;

	[Header ("Content")]
	public RectTransform content = null;

	[Header ("Under Menu")]
	public List<RectTransform> underMenuList;
	public List<RectTransform> underButtonsList;

	void Awake ()
	{
		//Main Menu Init
		if (menuComponentType == MenuComponentType.MainMenu)
		{
			for(int i = 0; i < transform.childCount; i++)
				underMenuList.Add (transform.GetChild (i).GetComponent<RectTransform> ());

			for (int i = 0; i < underMenuList.Count; i++)
				underButtonsList.Add (underMenuList [i].transform.GetChild (0).GetComponent<RectTransform> ());

			return;
		}

		//Other Menu Init
		if (transform.childCount > 2)
			menuComponentType = MenuComponentType.ButtonsListMenu;
		else
			menuComponentType = MenuComponentType.ContentMenu;

		button = transform.GetChild (0).GetComponent<RectTransform> ();

		if(menuComponentType == MenuComponentType.ButtonsListMenu)
		{
			for(int i = 1; i < transform.childCount; i++)
				underMenuList.Add (transform.GetChild (i).GetComponent<RectTransform> ());

			for (int i = 0; i < underMenuList.Count; i++)
				underButtonsList.Add (underMenuList [i].transform.GetChild (0).GetComponent<RectTransform> ());
		}
		else
		{
			content = transform.GetChild (1).GetComponent<RectTransform> ();
		}
	}

	void Start ()
	{
		if (menuComponentType != MenuComponentType.MainMenu)
		{
			if (transform.parent.parent != null && transform.parent.parent.GetComponent<MenuComponent> () != null)
			{
				aboveMenuList = transform.parent.parent.GetComponent<MenuComponent> ().underMenuList;		
				
				for (int i = 0; i < aboveMenuList.Count; i++)
					aboveButtonsList.Add (aboveMenuList [i].transform.GetChild (0).GetComponent<RectTransform> ());
			}			
		}

		HideAll ();
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

	public void Submit (int whichChildButton)
	{
		MenuManager.Instance.ButtonsListSubmit (underButtonsList, aboveButtonsList, whichChildButton);
	}
}
