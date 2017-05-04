using UnityEngine;
using System.Collections;
using Rewired;
using UnityEngine.UI;
using DG.Tweening;

public class BackButtonsFeedback : MonoBehaviour 
{
	public enum WhichButton {Esc, B, BackText, RightClick};

	public WhichButton whichButton;

	public Sprite modifiedSprite;
	private Sprite initialSprite;

	public Player mouseKeyboard;
	public Player gamepad1;
	public Player gamepad2;
	public Player gamepad3;
	public Player gamepad4;

	private float tweenDuration = 0.1f;
	private float modifiedScale = 1.3f;

	public Color newColor;
	public Ease theEase = Ease.OutElastic;
	public BackButtonsFeedback backText;

	private float initialScale;
	private RectTransform rect;
	private Color initialColor;
	private Vector2 initialPos;
	// Use this for initialization
	void Start () 
	{
		if(GetComponent<Image>() != null)
			initialSprite = GetComponent<Image> ().sprite;

		rect = GetComponent<RectTransform> ();
		initialScale = rect.localScale.x;
		initialPos = rect.anchoredPosition;

		if (GetComponent<Image> () != null)
			initialColor = GetComponent<Image> ().color;
	}
	
	// Update is called once per frame
	void Update () 
	{
		WhichFeedback ();
	}

	void WhichFeedback ()
	{
		if(GlobalVariables.Instance.rewiredPlayers [0].GetButtonDown("UI Cancel"))
		{
			if (whichButton == WhichButton.Esc || whichButton == WhichButton.RightClick)
				ButtonPressed ();

			if (whichButton == WhichButton.BackText)
				BackText ();
		}

		if(GlobalVariables.Instance.rewiredPlayers [1].GetButtonDown("UI Cancel"))
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
		GetComponent<Image> ().sprite = modifiedSprite;

		rect.DOScale (modifiedScale, tweenDuration).SetEase (theEase).OnComplete ( ()=> rect.DOScale (initialScale, 0.2f));

		GetComponent<Image> ().DOColor (newColor, tweenDuration).OnComplete ( () => ResetFeedback());
	}

	public void BackText ()
	{
		rect.DOAnchorPos(new Vector2(initialPos.x + 25, initialPos.y), tweenDuration).SetEase(Ease.OutQuad).OnComplete( ()=> rect.DOAnchorPos(initialPos, tweenDuration * 0.5f));
	}

	void ResetFeedback ()
	{
		GetComponent<Image> ().DOColor (initialColor, tweenDuration);
		GetComponent<Image> ().sprite = initialSprite;
	}
}
