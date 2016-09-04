using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Rewired;

public class FeedbackInputs : MonoBehaviour 
{
	public enum WhichButton {GamepadButton, KeyboardOrMouseButton, LeftJoystick, RightJoystick, Mouse};

	public WhichButton whichbutton;

	public string whichAction;

	public float movementMultiplicator;

	public KeyCode keycode;

	public float originScale;
	public float modifiedScale;

	public Color modifiedColor;

	public bool keyPressed;

	public Player mouseKeyboard;
	public Player gamepad1;

	private RectTransform rect;
	private Vector2 initialPos;
	//private Vector2 mouseInitialPos;

	// Use this for initialization
	void Start () 
	{
		ReInput.ControllerConnectedEvent += GetPlayersEvent;

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
				Feedback ();
				keyPressed = true;
			}
			
			if(Input.GetKeyUp(keycode))
			{
				ResetFeedback ();
				keyPressed = false;
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
		transform.DOScale (modifiedScale, 0.2f);

		if(transform.gameObject.GetComponent<SpriteRenderer> () != null)
			transform.gameObject.GetComponent<SpriteRenderer> ().color = modifiedColor;

		if(transform.gameObject.GetComponent<Image> () != null)
			transform.gameObject.GetComponent<Image> ().color = modifiedColor;
	}

	void ResetFeedback ()
	{
		transform.DOScale (originScale, 0.2f);

		if(transform.gameObject.GetComponent<SpriteRenderer> () != null)
			transform.gameObject.GetComponent<SpriteRenderer> ().color = Color.white;

		if(transform.gameObject.GetComponent<Image> () != null)
			transform.gameObject.GetComponent<Image> ().color = Color.white;
	}
}
