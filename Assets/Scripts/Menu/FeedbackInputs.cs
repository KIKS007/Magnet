using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Rewired;

public class FeedbackInputs : MonoBehaviour 
{
	public enum WhichButton {GamepadButton, KeyboardOrMouseButton, LeftJoystick, RightJoystick, Mouse};

	public WhichButton whichbutton;
	public Transform descriptionText;

	public Sprite modifiedSprite;
	public Sprite alternateSprite;
	public Sprite modifiedAlternateSprite;

	public string whichAction;

	public float movementMultiplicator;

	public KeyCode keycode;
	public KeyCode keycodeAlternate;

	public float originScale;
	public float modifiedScale;
	public float modifiedScaleText;

	public Color modifiedColor;

	public bool keyPressed;

	public Player mouseKeyboard;
	public Player gamepad1;

	private RectTransform rect;
	private Vector2 initialPos;
	//private Vector2 mouseInitialPos;

	private Sprite initialSprite;

	// Use this for initialization
	void Start () 
	{
		ReInput.ControllerConnectedEvent += GetPlayersEvent;
		initialSprite = GetComponent<Image> ().sprite;

		GetPlayers ();

		rect = GetComponent<RectTransform> ();
		initialPos = rect.anchoredPosition;
	}

	void OnEnable ()
	{
		//mouseInitialPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(whichbutton == WhichButton.KeyboardOrMouseButton)
		{
			if(Input.GetKey(keycode) || Input.GetKeyDown(keycode))
			{
				keyPressed = true;

				GetComponent<Image> ().sprite = modifiedSprite;

				transform.DOScale (modifiedScale, 0.2f);

				if(descriptionText != null)
				{
					descriptionText.DOScale (modifiedScaleText, 0.2f);
					descriptionText.GetComponent<Text>().DOColor(modifiedColor, 0.2f);

				}

				if(transform.gameObject.GetComponent<SpriteRenderer> () != null)
					transform.gameObject.GetComponent<SpriteRenderer> ().color = modifiedColor;

				if(transform.gameObject.GetComponent<Image> () != null)
					transform.gameObject.GetComponent<Image> ().color = modifiedColor;
			}
			
			if(Input.GetKeyUp(keycode))
			{
				keyPressed = false;

				GetComponent<Image> ().sprite = initialSprite;

				transform.DOScale (originScale, 0.2f);

				if(descriptionText != null)
				{
					descriptionText.DOScale (originScale, 0.2f);
					descriptionText.GetComponent<Text>().DOColor(Color.white, 0.2f);

				}

				if(transform.gameObject.GetComponent<SpriteRenderer> () != null)
					transform.gameObject.GetComponent<SpriteRenderer> ().color = Color.white;

				if(transform.gameObject.GetComponent<Image> () != null)
					transform.gameObject.GetComponent<Image> ().color = Color.white;
			}

			if(keycodeAlternate != KeyCode.None)
			{
				if(Input.GetKey(keycodeAlternate) || Input.GetKeyDown(keycodeAlternate))
				{
					keyPressed = true;

					GetComponent<Image> ().sprite = modifiedAlternateSprite;

					transform.DOScale (modifiedScale, 0.2f);

					if(descriptionText != null)
					{
						descriptionText.DOScale (modifiedScaleText, 0.2f);
						descriptionText.GetComponent<Text>().DOColor(modifiedColor, 0.2f);

					}

					if(transform.gameObject.GetComponent<SpriteRenderer> () != null)
						transform.gameObject.GetComponent<SpriteRenderer> ().color = modifiedColor;

					if(transform.gameObject.GetComponent<Image> () != null)
						transform.gameObject.GetComponent<Image> ().color = modifiedColor;
				}
				
				if(Input.GetKeyUp(keycodeAlternate))
				{
					keyPressed = false;

					GetComponent<Image> ().sprite = alternateSprite;

					transform.DOScale (originScale, 0.2f);

					if(descriptionText != null)
					{
						descriptionText.DOScale (originScale, 0.2f);
						descriptionText.GetComponent<Text>().DOColor(Color.white, 0.2f);

					}

					if(transform.gameObject.GetComponent<SpriteRenderer> () != null)
						transform.gameObject.GetComponent<SpriteRenderer> ().color = Color.white;

					if(transform.gameObject.GetComponent<Image> () != null)
						transform.gameObject.GetComponent<Image> ().color = Color.white;
				}				
			}
		}

		if(whichbutton == WhichButton.GamepadButton)
		{
			if(gamepad1.GetButtonDown(whichAction))
			{
				Feedback ();
				keyPressed = true;
			}

			if(gamepad1.GetButtonUp(whichAction))
			{
				ResetFeedback ();
				keyPressed = false;
			}
		}

		if(whichbutton == WhichButton.LeftJoystick)
		{
			Vector2 joystickMovement = new Vector2(gamepad1.GetAxisRaw("Move Horizontal"), gamepad1.GetAxisRaw("Move Vertical"));

			if(joystickMovement.magnitude != 0 && !keyPressed)
			{
				Feedback ();
				keyPressed = true;
			}

			if(joystickMovement.magnitude == 0 && keyPressed)
			{
				ResetFeedback ();
				keyPressed = false;
			}


			rect.anchoredPosition = initialPos + joystickMovement * movementMultiplicator;
		}

		if(whichbutton == WhichButton.RightJoystick)
		{
			Vector2 joystickMovement = new Vector2(gamepad1.GetAxisRaw("Aim Horizontal"), gamepad1.GetAxisRaw("Aim Vertical"));

			if(joystickMovement.magnitude != 0 && !keyPressed)
			{
				Feedback ();
				keyPressed = true;
			}

			if(joystickMovement.magnitude == 0 && keyPressed)
			{
				ResetFeedback ();
				keyPressed = false;
			}


			rect.anchoredPosition = initialPos + joystickMovement * movementMultiplicator;
		}

		if(whichbutton == WhichButton.Mouse)
		{
			Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

			//mouseMovement = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - mouseInitialPos;
			mouseMovement = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
			mouseMovement.Normalize ();

			if(mouseMovement.magnitude != 0 && !keyPressed)
			{
				Feedback ();
				keyPressed = true;
			}

			if(mouseMovement.magnitude == 0 && keyPressed)
			{
				ResetFeedback ();
				keyPressed = false;
			}

			rect.anchoredPosition = initialPos + mouseMovement * movementMultiplicator;
		}
	}

	void GetPlayersEvent (ControllerStatusChangedEventArgs arg)
	{
		mouseKeyboard = ReInput.players.GetPlayer (0);
		gamepad1 = ReInput.players.GetPlayer (1);

		gamepad1.controllers.AddController (ControllerType.Joystick, 0, true);
	}

	void GetPlayers ()
	{
		mouseKeyboard = ReInput.players.GetPlayer (0);
		gamepad1 = ReInput.players.GetPlayer (1);

		gamepad1.controllers.AddController (ControllerType.Joystick, 0, true);
	}

	void Feedback ()
	{
		GetComponent<Image> ().sprite = modifiedSprite;

		transform.DOScale (modifiedScale, 0.2f);

		if(descriptionText != null)
		{
			descriptionText.DOScale (modifiedScaleText, 0.2f);
			descriptionText.GetComponent<Text>().DOColor(modifiedColor, 0.2f);
			
		}

		if(transform.gameObject.GetComponent<SpriteRenderer> () != null)
			transform.gameObject.GetComponent<SpriteRenderer> ().color = modifiedColor;

		if(transform.gameObject.GetComponent<Image> () != null)
			transform.gameObject.GetComponent<Image> ().color = modifiedColor;
	}

	void ResetFeedback ()
	{
		GetComponent<Image> ().sprite = initialSprite;

		transform.DOScale (originScale, 0.2f);
	
		if(descriptionText != null)
		{
			descriptionText.DOScale (originScale, 0.2f);
			descriptionText.GetComponent<Text>().DOColor(Color.white, 0.2f);
			
		}

		if(transform.gameObject.GetComponent<SpriteRenderer> () != null)
			transform.gameObject.GetComponent<SpriteRenderer> ().color = Color.white;

		if(transform.gameObject.GetComponent<Image> () != null)
			transform.gameObject.GetComponent<Image> ().color = Color.white;
	}
}
