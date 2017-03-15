using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamesCountInput : MonoBehaviour 
{
	public GameObject increaseButton;
	public GameObject decreaseButton;

	private InputField input;
	private Vector2 bounds = new Vector2 (1, 99);
	private Vector2 initialbounds = new Vector2 (1, 99);

	private ModeSequenceType previousModeSequence;

	// Use this for initialization
	void Start () 
	{
		input = GetComponent<InputField> ();
		previousModeSequence = GlobalVariables.Instance.ModeSequenceType;

		if (GlobalVariables.Instance.GamesCount == 0)
			GlobalVariables.Instance.GamesCount = 1;

		input.text = GlobalVariables.Instance.GamesCount.ToString ();

		GlobalVariables.Instance.OnCocktailModesChange += () =>
		{
			if (GlobalVariables.Instance.ModeSequenceType == ModeSequenceType.Cocktail)
				bounds.x = GlobalVariables.Instance.cocktailModes.Count;

			GetValue ();
		};
			
		GlobalVariables.Instance.OnSequenceChange += () => 
		{
			bounds = initialbounds;

			if (GlobalVariables.Instance.ModeSequenceType != ModeSequenceType.Cocktail && previousModeSequence == ModeSequenceType.Cocktail)
			{
				input.text = "1";

				GetValue ();
			}

			previousModeSequence = GlobalVariables.Instance.ModeSequenceType;
		};
	}

	void OnEnable ()
	{
		GlobalVariables.Instance.CurrentGamesCount = GlobalVariables.Instance.GamesCount;

		input = GetComponent<InputField> ();
		input.text = GlobalVariables.Instance.GamesCount.ToString ();

		if (GlobalVariables.Instance.ModeSequenceType == ModeSequenceType.Cocktail)
			bounds.x = GlobalVariables.Instance.cocktailModes.Count;

		GetValue ();
	}

	void OnDisable ()
	{
		if(GlobalVariables.Instance)
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

		if (GlobalVariables.Instance.ModeSequenceType == ModeSequenceType.Cocktail && value < GlobalVariables.Instance.cocktailModes.Count)
		{
			value = GlobalVariables.Instance.cocktailModes.Count;
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
		if (GlobalVariables.Instance.GamesCount <= bounds.x)
			decreaseButton.SetActive (false);
		else
			decreaseButton.SetActive (true);
		
		if (GlobalVariables.Instance.GamesCount >= bounds.y)
			increaseButton.SetActive (false);

		else
			increaseButton.SetActive (true);
	}
}
