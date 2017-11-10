using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Rewired;

public class FeedbackInputs : MonoBehaviour
{
    public enum WhichButton
    {
        GamepadButton,
        KeyboardOrMouseButton,
        LeftJoystick,
        RightJoystick,
        Mouse}

    ;

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

    private RectTransform rect;
    private Vector2 initialPos;
    //private Vector2 mouseInitialPos;

    private MenuButtonAnimationsAndSounds menuButton;

    // Use this for initialization
    void Start()
    {
        ReInput.ControllerConnectedEvent += GetPlayersEvent;

        menuButton = GetComponent<MenuButtonAnimationsAndSounds>();

        GetPlayers();

        rect = GetComponent<RectTransform>();
        initialPos = rect.anchoredPosition;
    }

    void OnEnable()
    {
        //mouseInitialPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        ResetFeedback();
        keyPressed = false;
    }
	
    // Update is called once per frame
    void Update()
    {
        switch (whichbutton)
        {
            case WhichButton.GamepadButton:
                GamepadButton();
                break;
            case WhichButton.KeyboardOrMouseButton:
                KeyboardOrMouse();
                break;
            case WhichButton.LeftJoystick:
                LeftJoystick();
                break;
            case WhichButton.RightJoystick:
                RightJoystick();
                break;
            case WhichButton.Mouse:
                Mouse();
                break;
        }
    }

    void GamepadButton()
    {
        if (GlobalVariables.Instance.rewiredPlayers[GlobalVariables.Instance.menuGamepadNumber].GetButtonDown(whichAction) && !keyPressed)
        {
            Feedback();
            keyPressed = true;
        }

        if (GlobalVariables.Instance.rewiredPlayers[GlobalVariables.Instance.menuGamepadNumber].GetButtonUp(whichAction) && keyPressed)
        {
            ResetFeedback();
            keyPressed = false;
        }
    }

    void KeyboardOrMouse()
    {
        if (Input.GetKeyDown(keycode) && !keyPressed)
        {
            Feedback();
            keyPressed = true;
        }

        if (Input.GetKeyUp(keycode) && keyPressed)
        {
            ResetFeedback();
            keyPressed = false;
        }

        if (keycodeAlternate != KeyCode.None)
        {
            if (Input.GetKeyDown(keycodeAlternate) && !keyPressed)
            {
                Feedback();
                keyPressed = true;
            }

            if (Input.GetKeyUp(keycodeAlternate) && keyPressed)
            {
                ResetFeedback();
                keyPressed = false;
            }				
        }
    }

    void LeftJoystick()
    {
        Vector2 joystickMovement = new Vector2(GlobalVariables.Instance.rewiredPlayers[GlobalVariables.Instance.menuGamepadNumber].GetAxisRaw("Move Horizontal"), GlobalVariables.Instance.rewiredPlayers[GlobalVariables.Instance.menuGamepadNumber].GetAxisRaw("Move Vertical"));

        if (joystickMovement.magnitude != 0 && !keyPressed)
        {
            Feedback();
            keyPressed = true;
        }

        if (joystickMovement.magnitude == 0 && keyPressed)
        {
            ResetFeedback();
            keyPressed = false;
        }


        rect.anchoredPosition = initialPos + joystickMovement * movementMultiplicator;
    }

    void RightJoystick()
    {
        Vector2 joystickMovement = new Vector2(GlobalVariables.Instance.rewiredPlayers[GlobalVariables.Instance.menuGamepadNumber].GetAxisRaw("Aim Horizontal"), GlobalVariables.Instance.rewiredPlayers[GlobalVariables.Instance.menuGamepadNumber].GetAxisRaw("Aim Vertical"));

        if (joystickMovement.magnitude != 0 && !keyPressed)
        {
            Feedback();
            keyPressed = true;
        }

        if (joystickMovement.magnitude == 0 && keyPressed)
        {
            ResetFeedback();
            keyPressed = false;
        }


        rect.anchoredPosition = initialPos + joystickMovement * movementMultiplicator;
    }

    void Mouse()
    {
        Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        //mouseMovement = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - mouseInitialPos;
        mouseMovement = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        mouseMovement.Normalize();

        if (mouseMovement.magnitude != 0 && !keyPressed)
        {
            Feedback();
            keyPressed = true;
        }

        if (mouseMovement.magnitude == 0 && keyPressed)
        {
            ResetFeedback();
            keyPressed = false;
        }

        rect.anchoredPosition = initialPos + mouseMovement * movementMultiplicator;
    }

    void GetPlayersEvent(ControllerStatusChangedEventArgs arg)
    {
        mouseKeyboard = GlobalVariables.Instance.rewiredPlayers[0]; 
    }

    void GetPlayers()
    {
        mouseKeyboard = GlobalVariables.Instance.rewiredPlayers[0]; 
    }

    void Feedback()
    {
        //GetComponent<Image> ().sprite = modifiedSprite;

        if (menuButton)
            menuButton.ShaderClick(true);

        transform.DOScale(modifiedScale, 0.2f);

        if (descriptionText != null)
        {
            descriptionText.DOScale(modifiedScaleText, 0.2f);
            descriptionText.GetComponent<Text>().DOColor(modifiedColor, 0.2f);
			
        }

        /*if(transform.gameObject.GetComponent<SpriteRenderer> () != null)
			transform.gameObject.GetComponent<SpriteRenderer> ().color = modifiedColor;

		if(transform.gameObject.GetComponent<Image> () != null)
			transform.gameObject.GetComponent<Image> ().color = modifiedColor;*/
    }

    void ResetFeedback()
    {
        if (menuButton)
            menuButton.ShaderClick(false);
		
        transform.DOScale(originScale, 0.2f);
	
        if (descriptionText != null)
        {
            descriptionText.DOScale(originScale, 0.2f);
            descriptionText.GetComponent<Text>().DOColor(Color.white, 0.2f);
			
        }

        /*if(transform.gameObject.GetComponent<SpriteRenderer> () != null)
			transform.gameObject.GetComponent<SpriteRenderer> ().color = Color.white;

		if(transform.gameObject.GetComponent<Image> () != null)
			transform.gameObject.GetComponent<Image> ().color = Color.white;*/
    }
}
