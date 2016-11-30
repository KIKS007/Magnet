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

		GetPlayers ();
	}

	void OnEnable ()
	{
		GetPlayers ();
	}

	void GetPlayers ()
	{
		mouseKeyboard = ReInput.players.GetPlayer (0);
		gamepad1 = ReInput.players.GetPlayer (1);
		gamepad2 = ReInput.players.GetPlayer (2);
		gamepad3 = ReInput.players.GetPlayer (3);
		gamepad4 = ReInput.players.GetPlayer (4);

		for(int i = 0; i < GamepadsManager.Instance.gamepadsList.Count; i++)
		{
			if(GamepadsManager.Instance.gamepadsList[i].GamepadId == 1)
				gamepad1.controllers.AddController(GamepadsManager.Instance.gamepadsList[i].GamepadController, true);

			if(GamepadsManager.Instance.gamepadsList[i].GamepadId == 2)
				gamepad2.controllers.AddController(GamepadsManager.Instance.gamepadsList[i].GamepadController, true);

			if(GamepadsManager.Instance.gamepadsList[i].GamepadId == 3)
				gamepad3.controllers.AddController(GamepadsManager.Instance.gamepadsList[i].GamepadController, true);

			if(GamepadsManager.Instance.gamepadsList[i].GamepadId == 4)
				gamepad4.controllers.AddController(GamepadsManager.Instance.gamepadsList[i].GamepadController, true);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		WhichFeedback ();
	}

	void WhichFeedback ()
	{
		if(mouseKeyboard.GetButtonDown("UI Cancel"))
		{
			if (whichButton == WhichButton.Esc || whichButton == WhichButton.RightClick)
				ButtonPressed ();

			if (whichButton == WhichButton.BackText)
				BackText ();
		}

		if(gamepad1.GetButtonDown("UI Cancel") || gamepad2.GetButtonDown("UI Cancel") || gamepad3.GetButtonDown("UI Cancel") || gamepad4.GetButtonDown("UI Cancel"))
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
