using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamesCountInput : MonoBehaviour 
{
	public GameObject increaseButton;
	public GameObject decreaseButton;

	private InputField input;

	// Use this for initialization
	void Start () 
	{
		input = GetComponent<InputField> ();

		if (GlobalVariables.Instance.GamesCount == 0)
			GlobalVariables.Instance.GamesCount = 1;

		input.text = GlobalVariables.Instance.GamesCount.ToString ();
	}

	void OnEnable ()
	{
		GlobalVariables.Instance.CurrentGamesCount = GlobalVariables.Instance.GamesCount;

		input = GetComponent<InputField> ();
		input.text = GlobalVariables.Instance.GamesCount.ToString ();

		CheckBounds ();
	}

	void OnDisable ()
	{
		GlobalVariables.Instance.CurrentGamesCount = GlobalVariables.Instance.GamesCount;
	}

	public void GetValue ()
	{
		int value = 0;

		if (!int.TryParse (input.text, out value) || value == 0)
		{
			value = 1;
			input.text = value.ToString ();
		}

		GlobalVariables.Instance.GamesCount = value;

		CheckBounds ();
	}

	public void Increase ()
	{
		GlobalVariables.Instance.GamesCount++;
		input.text = GlobalVariables.Instance.GamesCount.ToString ();

		CheckBounds ();
	}

	public void Decrease ()
	{
		GlobalVariables.Instance.GamesCount--;
		input.text = GlobalVariables.Instance.GamesCount.ToString ();

		CheckBounds ();
	}

	void CheckBounds ()
	{
		if (GlobalVariables.Instance.GamesCount <= 1)
			decreaseButton.SetActive (false);
		else
			decreaseButton.SetActive (true);
		
		if (GlobalVariables.Instance.GamesCount >= 99)
			increaseButton.SetActive (false);

		else
			increaseButton.SetActive (true);
	}
}
