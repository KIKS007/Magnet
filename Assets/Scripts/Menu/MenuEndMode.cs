using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;

public class MenuEndMode : SerializedMonoBehaviour 
{
	[Header ("Scoreboard")]
	public List<ScoreboardPosition> scoreboardPosition = new List<ScoreboardPosition> ();
	public List<Text> scoreboardPlayers = new List<Text> ();
	public List<float> scoreScales = new List<float> ();

	[Header ("Scoreboard Tween")]
	public float scoreTextDuration;
	public Ease scoreTweenEase;

	private List<int> scores = new List<int> ();
	private Dictionary<int, int> previousScales = new Dictionary<int, int> ();

	// Use this for initialization
	void Start () 
	{
		GlobalVariables.Instance.OnEndMode += ()=> StartCoroutine (ScoreBoard ());

		GlobalVariables.Instance.OnMenu += ()=> 
		{
			foreach (Text text in scoreboardPlayers)
			{
				text.text = "0";
				text.transform.localScale = Vector3.one;
			}
		};

	}

	IEnumerator ScoreBoard ()
	{
		if(GlobalVariables.Instance.NumberOfPlayers == 0)
			yield break;

		foreach (Text text in scoreboardPlayers)
			text.gameObject.SetActive (false);

		List<Text> enabledText = new List<Text> ();

		foreach (GameObject g in GlobalVariables.Instance.EnabledPlayersList)
		{
			scoreboardPlayers [(int)g.GetComponent<PlayersGameplay> ().playerName].gameObject.SetActive (true);
			enabledText.Add (scoreboardPlayers [(int)g.GetComponent<PlayersGameplay> ().playerName]);
		}

		for(int i = 0; i < enabledText.Count; i++)
			enabledText [i].rectTransform.anchoredPosition = new Vector2 (scoreboardPosition [enabledText.Count - 2].positions [i], enabledText [i].rectTransform.anchoredPosition.y);


		var playersStats = StatsManager.Instance.playersStats.OrderByDescending (x => x.Value.playersStats [WhichStat.Wins.ToString ()]).ToDictionary (x => x.Key, x=> x.Value);

		yield return new WaitWhile (()=> MenuManager.Instance.currentMenu == null);

		yield return new WaitWhile (()=> DOTween.IsTweening ("MenuCamera"));

		yield return new WaitWhile (()=> MenuManager.Instance.isTweening);

		List<string> keys = playersStats.Keys.ToList ();

		scores.Clear ();
		previousScales.Clear ();

		for (int i = 0; i < keys.Count; i++)
			scores.Add (playersStats [keys [i]].playersStats [WhichStat.Wins.ToString ()]);

		for(int i = 0; i < keys.Count; i++)
		{
			PlayerName playerName = (PlayerName) Enum.Parse (typeof(PlayerName), keys [i]);
			int wins = playersStats [keys [i]].playersStats [WhichStat.Wins.ToString ()];

			if(wins != 0)
			{
				scoreboardPlayers [(int)playerName].transform.DOScale (scoreScales [ScoreScale (i, wins)], scoreTextDuration);

				if(!previousScales.ContainsKey (wins))
					previousScales.Add (wins, ScoreScale (i, wins));
			}
			else
				scoreboardPlayers [(int)playerName].transform.DOScale (scoreScales [3], scoreTextDuration);

			StartCoroutine (GradualScore (scoreboardPlayers [(int)playerName], playersStats [keys [i]].playersStats [WhichStat.Wins.ToString ()]));
		}
	}

	int ScoreScale (int scoreIndex, int score)
	{
		int sameScore = 0;
		int differentScore = 0;

		for(int i = 0; i < scores.Count; i++)
		{
			if (i + 1 == scores.Count)
				break;

			if (scores [i + 1] == scores [i])
				sameScore++;
			else
				differentScore++;
		}

		if(differentScore == scores.Count - 1 && scores.Count == 2)
		{
			if (scoreIndex == 0)
				return 0;
			else
				return 3;
		}

		if (previousScales.ContainsKey (score))
			return previousScales [score];
		
		if(scoreIndex == scores.Count - 1)
			return 3;

		if (sameScore == scores.Count - 1)
			return 1;

		if (differentScore == scores.Count - 1)
			return scoreIndex;


		else
		{
			if(scoreIndex == 0)
			{
				if (scores [0] == scores [1])
					return 1;
				else
					return 0;
			}

			if (scoreIndex != 0 && scores [scoreIndex] == scores [0])
				return 1;
		}

		return scoreIndex;
	}

	IEnumerator GradualScore (Text textComponent, int endScore)
	{
		int score = int.Parse (textComponent.text);

		DOTween.To (()=> score, x=> score =x, endScore, scoreTextDuration).SetEase (scoreTweenEase).OnUpdate (()=>
			{
				textComponent.text = score.ToString ();
			});

		yield return 0;
	}
}

[System.Serializable]
public class ScoreboardPosition
{
	public List<float> positions = new List<float> ();
}
