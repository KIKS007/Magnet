using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamesCountInput : MonoBehaviour 
{
	public GameObject increaseButton;
	public GameObject decreaseButton;

	private InputField input;
	private int previousValue;

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
		GlobalVariables.Instance.GamesCount = previousValue;

		input = GetComponent<InputField> ();
		input.text = GlobalVariables.Instance.GamesCount.ToString ();
	}

	void OnDisable ()
	{
		previousValue = GlobalVariables.Instance.GamesCount;
	}

	public void GetValue ()
	{
		int value = 0;

		if (!int.TryParse (input.text, out value))
		{
			value = 1;
			input.text = value.ToString ();
		}

		GlobalVariables.Instance.GamesCount = value;
	}

	public void Increase ()
	{
		GlobalVariables.Instance.GamesCount++;
		input.text = GlobalVariables.Instance.GamesCount.ToString ();

		if (GlobalVariables.Instance.GamesCount >= 99)
			increaseButton.SetActive (false);

		if(decreaseButton.activeSelf == false)
			decreaseButton.SetActive (false);

	}

	public void Decrease ()
	{
		GlobalVariables.Instance.GamesCount--;
		input.text = GlobalVariables.Instance.GamesCount.ToString ();

		if (GlobalVariables.Instance.GamesCount <= 1)
			decreaseButton.SetActive (false);

		if(increaseButton.activeSelf == false)
			increaseButton.SetActive (false);
	}
}
