using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class NumberOfLives : MonoBehaviour 
{
	public Button increaseButton;
	public Button decreaseButton;

	[MinMaxSliderAttribute (1, 99)]
	public Vector2 livesCountBounds;

	private InputField input;

	// Use this for initialization
	void Start ()
	{
		LoadData ();
		
		input = GetComponent<InputField> ();
		input.text = GlobalVariables.Instance.LivesCount.ToString ();

		//CheckBounds ();
	}

	public void GetValue ()
	{
		int value = 0;

		if (!int.TryParse (input.text, out value) || value == 0)
		{
			value = 1;
			input.text = value.ToString ();
		}

		GlobalVariables.Instance.LivesCountChange (value);

		//CheckBounds ();
	}

	public void Increase ()
	{
		GlobalVariables.Instance.LivesCountChange (GlobalVariables.Instance.LivesCount + 1);

		if(GlobalVariables.Instance.LivesCount > livesCountBounds.y)
			GlobalVariables.Instance.LivesCountChange ((int)livesCountBounds.x);

		input.text = GlobalVariables.Instance.LivesCount.ToString ();

		//CheckBounds ();
	}

	public void Decrease ()
	{
		GlobalVariables.Instance.LivesCountChange (GlobalVariables.Instance.LivesCount - 1);

		if(GlobalVariables.Instance.LivesCount < livesCountBounds.x)
			GlobalVariables.Instance.LivesCountChange ((int)livesCountBounds.y);

		input.text = GlobalVariables.Instance.LivesCount.ToString ();

		//CheckBounds ();
	}

	void CheckBounds ()
	{
		if (GlobalVariables.Instance.LivesCount <= livesCountBounds.x)
			DisableButton (decreaseButton);
		else
			EnableButton (decreaseButton);

		if (GlobalVariables.Instance.LivesCount >= livesCountBounds.y)
			DisableButton (increaseButton);
		else
			EnableButton (increaseButton);
	}

	void EnableButton (Button button)
	{
		button.gameObject.SetActive (true);
		button.interactable = true;
	}

	void DisableButton (Button button)
	{
		button.gameObject.SetActive (false);
		button.interactable = false;
	}

	void SaveData ()
	{
		input = GetComponent<InputField> ();

		int value = 0;

		if (!int.TryParse (input.text, out value) || value == 0)
		{
			value = 1;
			input.text = value.ToString ();
		}

		PlayerPrefs.SetInt ("LivesCount", value);
	}

	void LoadData ()
	{
		if(PlayerPrefs.HasKey ("LivesCount"))
			GlobalVariables.Instance.LivesCount = PlayerPrefs.GetInt ("LivesCount");
	}

	void OnDestroy ()
	{
		if(SceneManager.GetActiveScene().name != "Scene Testing")
			SaveData ();
	}
}
