using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuEndMode : MonoBehaviour 
{
	[Header ("Scoreboard")]
	public List<List<Text>> scoreboard = new List<List<Text>> ();
	public List<float> scoreScales = new List<float> ();

	[Header ("Scoreboard Tween")]
	public float scoreTextDuration;
	public Ease scoreTweenEase;

	// Use this for initialization
	void Start () 
	{
		
	}

	void OnEnable ()
	{
		ScoreBoard ();
	}

	void ScoreBoard ()
	{
		foreach(List<Text> list in scoreboard)
			list [0].transform.parent.gameObject.SetActive (false);

		int players = GlobalVariables.Instance.NumberOfPlayers - 2;

		scoreboard [players] [0].transform.parent.gameObject.SetActive (true);

		for(int i = 0; i < scoreboard [players].Count; i++)
		{
			scoreboard [players] [i].transform.localScale = Vector3.one;
			scoreboard [players] [i].transform.DOScale ();
		}
	}
}
