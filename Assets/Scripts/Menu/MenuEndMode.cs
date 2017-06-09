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

	[Header ("Panels")]
	public List<RectTransform> playersPanels = new List<RectTransform> ();
	public List<ScoreboardPosition> playersPanelsPosition = new List<ScoreboardPosition> ();
	public List<float> playersPanelsYPos = new List<float> ();

	[Header ("Stats")]
	public GameObject statsPrefab;
	public float initialYPos;
	public float statsGapHeight;
	public List<ModeStats> modesStats = new List<ModeStats> ();

	[Header ("Tween")]
	public float scoreTextDuration;
	public Ease scoreTweenEase;
	public Ease panelTweenEase;

	private List<int> scores = new List<int> ();
	private Dictionary<int, int> previousScales = new Dictionary<int, int> ();
	private List<RectTransform> enabledPanels = new List<RectTransform> ();

	// Use this for initialization
	void Start () 
	{
		GlobalVariables.Instance.OnEndMode += ()=> StartCoroutine (PlayersPanels ());

		GlobalVariables.Instance.OnMenu += ()=> 
		{
			foreach (Text text in scoreboardPlayers)
			{
				text.text = "0";
				text.transform.localScale = Vector3.one;
			}
		};

	}

	IEnumerator PlayersPanels ()
	{
		if(GlobalVariables.Instance.NumberOfPlayers == 0)
			yield break;

		enabledPanels.Clear ();

		foreach (RectTransform panel in playersPanels)
		{
			panel.gameObject.SetActive (false);
			panel.anchoredPosition = new Vector2 (panel.anchoredPosition.x, playersPanelsYPos [2]);
		}

		foreach (Text text in scoreboardPlayers)
		{
			text.text = "0";
			text.transform.localScale = Vector3.one;
		}

		foreach (GameObject g in GlobalVariables.Instance.EnabledPlayersList)
		{
			playersPanels [(int)g.GetComponent<PlayersGameplay> ().playerName].gameObject.SetActive (true);
			enabledPanels.Add (playersPanels [(int)g.GetComponent<PlayersGameplay> ().playerName]);
		}

		CreateStats ();

		for(int i = 0; i < enabledPanels.Count; i++)
			enabledPanels [i].anchoredPosition = new Vector2 (playersPanelsPosition [enabledPanels.Count - 2].positions [i], enabledPanels [i].anchoredPosition.y);

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
				playersPanels [(int)playerName].DOAnchorPosY (playersPanelsYPos [ScoreOder (i, wins)], scoreTextDuration).SetEase (panelTweenEase);

				if(!previousScales.ContainsKey (wins))
					previousScales.Add (wins, ScoreOder (i, wins));
			}

			StartCoroutine (GradualScore (scoreboardPlayers [(int)playerName], playersStats [keys [i]].playersStats [WhichStat.Wins.ToString ()]));
		}
	}

	void CreateStats ()
	{
		int modesStatsIndex = 0;

		for(int i = 0; i < modesStats.Count; i++)
		{
			if (modesStats [i].mode == GlobalVariables.Instance.CurrentModeLoaded)
			{
				modesStatsIndex = i;
				break;
			}
		}

		foreach(RectTransform r in enabledPanels)
		{
			//Remove Previous Stats
			List<Transform> children = new List<Transform> ();

			foreach (Transform child in r.transform.GetChild (0))
				children.Add (child);
			
			children.ForEach (child => Destroy (child.gameObject));

			int playerIndex = playersPanels.FindIndex (x => x == r);

			for(int i = 0; i < modesStats [modesStatsIndex].modesStats.Count; i++)
			{
				Vector3 position = new Vector3 (statsPrefab.GetComponent<RectTransform> ().anchoredPosition.x, initialYPos - statsGapHeight * i, 0);

				GameObject statsClone = Instantiate (statsPrefab, statsPrefab.transform.position, statsPrefab.transform.rotation, r.transform.GetChild (0));
				statsClone.GetComponent<RectTransform> ().anchoredPosition3D = position;

				string text = StatsManager.Instance.statsText.FirstOrDefault (x=> x.Value == modesStats [modesStatsIndex].modesStats [i]).Key;
				statsClone.GetComponent<Text> ().text = text;

				GlobalMethods.Instance.ReplaceInText (statsClone.GetComponent<Text> (), 
					StatsManager.Instance.playersStats [((WhichPlayer)playerIndex).ToString ()].playersStats [modesStats [modesStatsIndex].modesStats [i].ToString ()].ToString ());
			}
		}
	}

	int ScoreOder (int scoreIndex, int score)
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
		int score = 0;

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

[System.Serializable]
public class ModeStats 
{
	public WhichMode mode = WhichMode.Default;
	public List<WhichStat> modesStats = new List<WhichStat> ();
}

[System.Serializable]
public class StatsPrefab 
{
	public WhichStat stats;
	public GameObject prefab;
}
