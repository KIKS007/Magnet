using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class NumberOfLives : MonoBehaviour 
{
	public GameObject increaseButton;
	public GameObject decreaseButton;

	[MinMaxSliderAttribute (1, 99)]
	public Vector2 livesCountBounds;

	private InputField input;

	// Use this for initialization
	void Start ()
	{
		LoadData ();
		
		input = GetComponent<InputField> ();
		input.text = GlobalVariables.Instance.LivesCount.ToString ();

		CheckBounds ();
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

		CheckBounds ();
	}

	public void Increase ()
	{
		GlobalVariables.Instance.LivesCountChange (GlobalVariables.Instance.LivesCount + 1);

		input.text = GlobalVariables.Instance.LivesCount.ToString ();

		CheckBounds ();
	}

	public void Decrease ()
	{
		GlobalVariables.Instance.LivesCountChange (GlobalVariables.Instance.LivesCount - 1);

		input.text = GlobalVariables.Instance.LivesCount.ToString ();

		CheckBounds ();
	}

	void CheckBounds ()
	{
		if (GlobalVariables.Instance.LivesCount <= livesCountBounds.x)
			decreaseButton.SetActive (false);
		else
			decreaseButton.SetActive (true);

		if (GlobalVariables.Instance.LivesCount >= livesCountBounds.y)
			increaseButton.SetActive (false);

		else
			increaseButton.SetActive (true);
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
