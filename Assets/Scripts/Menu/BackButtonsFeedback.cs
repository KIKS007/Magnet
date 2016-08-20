using UnityEngine;
using System.Collections;
using DG.Tweening;
using Rewired;
using UnityEngine.UI;

public class BackButtonsFeedback : MonoBehaviour 
{
	public enum WhichButton {Esc, B, BackText};

	public WhichButton whichButton;

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
		GetPlayers ();

		rect = GetComponent<RectTransform> ();
		initialScale = rect.localScale.x;
		initialPos = rect.anchoredPosition;

		if (GetComponent<Image> () != null)
			initialColor = GetComponent<Image> ().color;
	}

	void GetPlayers ()
	{
		mouseKeyboard = ReInput.players.GetPlayer (0);
		gamepad1 = ReInput.players.GetPlayer (1);
		gamepad2 = ReInput.players.GetPlayer (2);
		gamepad3 = ReInput.players.GetPlayer (3);
		gamepad4 = ReInput.players.GetPlayer (4);

		gamepad1.controllers.AddController (ControllerType.Joystick, 0, true);
		gamepad2.controllers.AddController (ControllerType.Joystick, 1, true);
		gamepad3.controllers.AddController (ControllerType.Joystick, 2, true);
		gamepad4.controllers.AddController (ControllerType.Joystick, 3, true);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(mouseKeyboard.GetButtonDown("Cancel"))
		{
			switch(whichButton)
			{
			case WhichButton.Esc:
				Esc ();
				break;
			case WhichButton.BackText:
				BackText ();
				break;
			}
		}

		if(gamepad1.GetButtonDown("Cancel") || gamepad2.GetButtonDown("Cancel") || gamepad3.GetButtonDown("Cancel") || gamepad4.GetButtonDown("Cancel"))
		{
			switch(whichButton)
			{
			case WhichButton.B:
				B ();
				break;
			case WhichButton.BackText:
				BackText ();
				break;
			}
		}
	}

	public void Click ()
	{
		switch(whichButton)
		{
		case WhichButton.B:
			B ();
			break;
		case WhichButton.Esc:
			Esc ();
			break;
		}

		backText.BackText ();
	}

	void Esc ()
	{
		rect.DOScale (modifiedScale, tweenDuration).SetEase (theEase).OnComplete ( ()=> rect.DOScale (initialScale, 0.2f));

		GetComponent<Image> ().DOColor (newColor, tweenDuration).OnComplete ( () => GetComponent<Image> ().DOColor(initialColor, tweenDuration));
	}

	void B ()
	{
		rect.DOScale (modifiedScale, tweenDuration).SetEase (theEase).OnComplete ( ()=> rect.DOScale (initialScale, 0.2f));

		GetComponent<Image> ().DOColor (newColor, tweenDuration).OnComplete ( () => GetComponent<Image> ().DOColor(initialColor, tweenDuration));
	}

	public void BackText ()
	{
		rect.DOAnchorPos(new Vector2(initialPos.x + 25, initialPos.y), tweenDuration).SetEase(Ease.OutQuad).OnComplete( ()=> rect.DOAnchorPos(initialPos, tweenDuration * 0.5f));
	}
}
