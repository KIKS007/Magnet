using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuCocktailButton : MonoBehaviour, IPointerClickHandler, ISubmitHandler
{
	public WhichMode mode;
	public bool activate = false;

	public MenuButtonAnimationsAndSounds menuAnims;

	private static List<MenuCocktailButton> allButtons = new List<MenuCocktailButton> ();

	// Use this for initialization
	void Start () 
	{
		allButtons.Add (this);

		menuAnims = GetComponent<MenuButtonAnimationsAndSounds> ();

		if (mode == WhichMode.Bomb || mode == WhichMode.Bounce || mode == WhichMode.Burden)
			Enable ();
	}
	
	public void Enable ()
	{
		activate = true;

		menuAnims.selected = true;
		menuAnims.ShaderHighlight ();
		menuAnims.OnSelect ();
		menuAnims.buttonComponent.interactable = false;

		//menuAnims.ShaderClick ();

		if (!GlobalVariables.Instance.selectedCocktailModes.Contains (mode))
			GlobalVariables.Instance.selectedCocktailModes.Add (mode);
	}

	public void Disable ()
	{
		menuAnims.selected = false;
		menuAnims.ShaderHighlight ();
		menuAnims.buttonComponent.interactable = true;

		//menuAnims.ShaderClick ();

		activate = false;

		if (GlobalVariables.Instance.selectedCocktailModes.Contains (mode))
			GlobalVariables.Instance.selectedCocktailModes.Remove (mode);

		if (GlobalVariables.Instance.selectedCocktailModes.Count == 0)
			transform.parent.GetChild (0).GetComponent<MenuCocktailButton> ().Enable ();
	}

	public void OnPointerClick (PointerEventData eventData)
	{
		if (activate)
			Disable ();
		else
			Enable ();
	}

	public void OnSubmit (BaseEventData eventData)
	{
		if (activate)
			Disable ();
		else
			Enable ();
	}

	public void SelectAll ()
	{
		foreach (MenuCocktailButton button in allButtons)
			if(!button.activate)
				button.Enable ();
	}

	public void DeselectAll ()
	{
		foreach (MenuCocktailButton button in allButtons)
		{
			if(button.mode != (WhichMode)0)
			{
				if(button.activate)
					button.Disable ();
			}
			else
			{
				if(!button.activate)
					button.Enable ();
			}
		}

		if (GlobalVariables.Instance.selectedCocktailModes.Count == 0)
			transform.parent.GetChild (0).GetComponent<MenuCocktailButton> ().Enable ();
	}
}
