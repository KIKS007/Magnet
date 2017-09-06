using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System;
using System.Linq;

public class StatsFeedback : MonoBehaviour 
{
	public enum WhichStatType {Player, Most, Least, Total, Winner, RoundDuration, AllRoundDuration };

	public WhichStatType whichStatType;

	[ShowIfAttribute("ClassicStat")]
	public WhichStat whichStat;
	bool ClassicStat()
	{
		if (whichStatType != WhichStatType.Winner && whichStatType != WhichStatType.RoundDuration && whichStatType != WhichStatType.AllRoundDuration)
			return true;
		else
			return false;
	}

	[ShowIfAttribute("PlayerStat")]
	public WhichPlayer whichPlayer = WhichPlayer.None;
	bool PlayerStat()
	{
		if (whichStatType == WhichStatType.Player)
			return true;
		else
			return false;
	}
		
	[Header ("Text")]
	public bool changeColor = true;
	public bool otherText = false;
	[ShowIfAttribute("otherText")]
	public Text textComponent;

	[ShowIfAttribute("PlayerText")]
	public Text playerText;
	bool PlayerText ()
	{
		if (whichStatType == WhichStatType.Most || whichStatType == WhichStatType.Player)
			return true;
		else
			return false;
	}

	[Header ("Show Conditions")]
	public bool disableParent = false;
	public bool showIfDifferent;
	[ShowIfAttribute("showIfDifferent")]
	public string differentValue;

	public bool showIfHigher;
	[ShowIfAttribute("showIfHigher")]
	public int higherValue;

	public bool showIfLower;
	[ShowIfAttribute("showIfLower")]
	public int lowerValue;

	private int value;
	private string valueText;

	private string initialString = "xxx";
	private MenuEndMode menuEndMode;
	private int modesStatsIndex = 0;

	void OnEnable ()
	{
		if (StatsManager.Instance == null)
			return;

		if (menuEndMode == null)
			menuEndMode = MenuManager.Instance.endModeMenu.GetComponent<MenuEndMode> ();

		if (!StatsManager.Instance.statsFeedback.Contains (this))
			StatsManager.Instance.statsFeedback.Add (this);
		
		if(textComponent == null)
			textComponent = GetComponent<Text> ();

		initialString = textComponent.text;

		UpdateText ();	
	}

	void OnDisable ()
	{
		if(initialString != "xxx")
			textComponent.text = initialString;
	}

	public void UpdateText ()
	{
		for(int i = 0; i < menuEndMode.modesStats.Count; i++)
		{
			foreach(WhichMode m in menuEndMode.modesStats [i].modes)
			{
				if (m == GlobalVariables.Instance.CurrentModeLoaded)
				{
					modesStatsIndex = i;
					break;
				}
			}
		}

		switch (whichStatType)
		{
		case WhichStatType.Player:
			PlayerStats ();
			break;
		case WhichStatType.Most:
			MostStats ();
			break;
		case WhichStatType.Least:
			LeastStats ();
			break;
		case WhichStatType.Total:
			TotalStats ();
			break;
		case WhichStatType.Winner:
			Winner ();
			break;
		case WhichStatType.RoundDuration:
			RoundDuration ();
			break;
		case WhichStatType.AllRoundDuration:
			AllRoundsDuration ();
			break;
		}
	}

	void PlayerStats ()
	{
		if (StatsManager.Instance.playersStats.Count == 0 || !StatsManager.Instance.playersStats.ContainsKey (whichPlayer.ToString ()) || StatsManager.Instance.playersStats [whichPlayer.ToString ()].playersStats.Count == 0)
			return;

		var statsDictionnary = StatsManager.Instance.overallStats ? StatsManager.Instance.playersTotalStats : StatsManager.Instance.playersStats;

		if(changeColor)
			textComponent.color = GlobalVariables.Instance.playersColors [(int)whichPlayer];

		if (whichStat != WhichStat.LifeDuration)
		{
			string text = StatsManager.Instance.statsText.FirstOrDefault (x=> x.Value == whichStat).Key;
			textComponent.text = text;

			GlobalMethods.Instance.ReplaceInText (textComponent, 
				statsDictionnary [((WhichPlayer)whichPlayer).ToString ()].playersStats [whichStat.ToString ()].ToString ());
		}
		else
			LifeDuration ();
		
	}

	void LifeDuration ()
	{
		var statsDictionnary = StatsManager.Instance.overallStats ? StatsManager.Instance.playersTotalStats : StatsManager.Instance.playersStats;

		float duration = statsDictionnary [whichPlayer.ToString ()].playerLifeDuration;

		string minutes = Mathf.Floor(duration / 60).ToString("00");
		string seconds = Mathf.Floor(duration % 60).ToString("00");
		string milliseconds = Mathf.Floor (duration * 1000f % 1000).ToString ("000");

		string durationText = minutes + ":" + seconds + ":" + milliseconds;

		GlobalMethods.Instance.ReplaceInText (textComponent, durationText);
	}

	void MostStats ()
	{	
		if (StatsManager.Instance.mostStats.Count == 0)
			return;

		if (StatsManager.Instance.mostStats [whichStat.ToString ()].whichPlayer == WhichPlayer.None)
			return;

		if(changeColor)
			textComponent.color = GlobalVariables.Instance.playersColors [(int)StatsManager.Instance.mostStats [whichStat.ToString ()].whichPlayer];

		if(playerText != null && changeColor)
			playerText.color = GlobalVariables.Instance.playersColors [(int)StatsManager.Instance.mostStats [whichStat.ToString ()].whichPlayer];


		value = StatsManager.Instance.mostStats [whichStat.ToString ()].value;

		GlobalMethods.Instance.ReplaceInText (textComponent, StatsManager.Instance.mostStats [whichStat.ToString ()].value.ToString ());
		valueText = StatsManager.Instance.mostStats [whichStat.ToString ()].value.ToString ();

		if(playerText != null)
			playerText.text = StatsManager.Instance.mostStats [whichStat.ToString ()].whichPlayer.ToString ();
	}

	void LeastStats ()
	{	
		if (StatsManager.Instance.leastStats.Count == 0)
			return;

		if (StatsManager.Instance.leastStats [whichStat.ToString ()].whichPlayer == WhichPlayer.None)
			return;

		if(changeColor)
			textComponent.color = GlobalVariables.Instance.playersColors [(int)StatsManager.Instance.leastStats [whichStat.ToString ()].whichPlayer];

		if(playerText != null && changeColor)
			playerText.color = GlobalVariables.Instance.playersColors [(int)StatsManager.Instance.leastStats [whichStat.ToString ()].whichPlayer];

		value = StatsManager.Instance.leastStats [whichStat.ToString ()].value;

		GlobalMethods.Instance.ReplaceInText (textComponent, StatsManager.Instance.leastStats [whichStat.ToString ()].value.ToString ());
		valueText = StatsManager.Instance.leastStats [whichStat.ToString ()].value.ToString ();

		if (playerText != null)
			playerText.text = StatsManager.Instance.leastStats [whichStat.ToString ()].whichPlayer.ToString ();
	}

	void TotalStats ()
	{
		if (StatsManager.Instance.totalStats.Count == 0)
			return;

		value = StatsManager.Instance.totalStats [whichStat.ToString ()];

		GlobalMethods.Instance.ReplaceInText (textComponent, StatsManager.Instance.totalStats [whichStat.ToString ()].ToString ());
		valueText = StatsManager.Instance.totalStats [whichStat.ToString ()].ToString ();
	}

	void Winner ()
	{
		if(changeColor)
			textComponent.color = GlobalVariables.Instance.playersColors [(int)StatsManager.Instance.winnerName];

		GlobalMethods.Instance.ReplaceInText (textComponent, StatsManager.Instance.winner);
		valueText = StatsManager.Instance.winner;
	}

	void RoundDuration ()
	{
		GlobalMethods.Instance.ReplaceInText (textComponent, StatsManager.Instance.roundDuration);
		valueText = StatsManager.Instance.roundDuration;
	}

	void AllRoundsDuration ()
	{
		GlobalMethods.Instance.ReplaceInText (textComponent, StatsManager.Instance.allRoundsDuration);
		valueText = StatsManager.Instance.allRoundsDuration;
	}

	void CheckVisibility ()
	{
		if(showIfDifferent)
		{
			if (valueText == differentValue)
				textComponent.enabled = false;
			else
				textComponent.enabled = true;
		}

		if(showIfHigher)
		{
			if (value <= higherValue)
				textComponent.enabled = false;
			else
				textComponent.enabled = true;
		}

		if(showIfLower)
		{
			if (value >= lowerValue)
				textComponent.enabled = false;
			else
				textComponent.enabled = true;

		}
	}

	void OnDestroy ()
	{
		if (StatsManager.Instance.statsFeedback.Contains (this))
			StatsManager.Instance.statsFeedback.Remove (this);
	}
}