using UnityEngine;
using System.Collections;
using Rewired;
using UnityEngine.UI;
using DG.Tweening;

public class BackButtonsFeedback : MonoBehaviour 
{
	public enum WhichButton {Esc, B, BackText, RightClick};

	public WhichButton whichButton;

	public Player mouseKeyboard;
	public Player gamepad1;
	public Player gamepad2;
	public Player gamepad3;
	public Player gamepad4;

	private float tweenDuration = 0.1f;
	private float modifiedScale = 1.3f;

	public Ease theEase = Ease.OutElastic;
	public BackButtonsFeedback backText;

	private float initialScale;
	private RectTransform rect;
	private Vector2 initialPos;

	private MenuButtonAnimationsAndSounds menuButton;

	// Use this for initialization
	void Start () 
	{
		menuButton = GetComponent<MenuButtonAnimationsAndSounds> ();

		rect = GetComponent<RectTransform> ();
		initialScale = rect.localScale.x;
		initialPos = rect.anchoredPosition;
	}

	public void Back (int rewiredIndex)
	{
		if(rewiredIndex == 0)
		{
			if (whichButton == WhichButton.Esc || whichButton == WhichButton.RightClick)
				ButtonPressed ();

			if (whichButton == WhichButton.BackText)
				BackText ();
		}

		if(rewiredIndex > 0)
		{
			if(whichButton == WhichButton.B)
				ButtonPressed ();

			if (whichButton == WhichButton.BackText)
				BackText ();
		}
	}

	public void Click ()
	{
		if (whichButton == WhichButton.Esc || whichButton == WhichButton.RightClick || whichButton == WhichButton.B)
			ButtonPressed ();

		if (whichButton == WhichButton.BackText)
			BackText ();
	}

	void ButtonPressed ()
	{
		menuButton.ShaderClickDuration ();

		rect.DOScale (modifiedScale, tweenDuration).SetEase (theEase).OnComplete ( ()=> rect.DOScale (initialScale, 0.2f));
	}

	public void BackText ()
	{
		rect.DOAnchorPos(new Vector2(initialPos.x + 25, initialPos.y), tweenDuration).SetEase(Ease.OutQuad).OnComplete( ()=> rect.DOAnchorPos(initialPos, tweenDuration * 0.5f));
	}
}
