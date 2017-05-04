using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuCocktailButton : MonoBehaviour, IPointerClickHandler, ISubmitHandler
{
	public WhichMode mode;
	public bool activate = false;

	private Image imageComponent;
	private Button buttonComponent;
	private Sprite initialSprite;
	private Sprite pressedSprite;

	private static List<MenuCocktailButton> allButtons = new List<MenuCocktailButton> ();

	// Use this for initialization
	void Start () 
	{
		allButtons.Add (this);

		imageComponent = GetComponent<Image> ();
		buttonComponent = GetComponent<Button> ();
		initialSprite = imageComponent.sprite;
		pressedSprite = buttonComponent.spriteState.pressedSprite;

		SpriteState spriteState = buttonComponent.spriteState;
		spriteState.pressedSprite = null;
		buttonComponent.spriteState = spriteState;

		if (mode == WhichMode.Bomb || mode == WhichMode.Bounce || mode == WhichMode.Burden)
			Enable ();
	}
	
	public void Enable ()
	{
		activate = true;

		imageComponent.sprite = pressedSprite;

		if (!GlobalVariables.Instance.selectedCocktailModes.Contains (mode))
			GlobalVariables.Instance.selectedCocktailModes.Add (mode);
	}

	public void Disable ()
	{
		activate = false;

		imageComponent.sprite = initialSprite;

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
			button.Enable ();
	}

	public void DeselectAll ()
	{
		foreach (MenuCocktailButton button in allButtons)
			if(button.mode != (WhichMode)0)
				button.Disable ();
			else
				button.Enable ();
	}
}
