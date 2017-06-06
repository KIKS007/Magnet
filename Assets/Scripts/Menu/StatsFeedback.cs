using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[ExecuteInEditMode]
public class StatsFeedback : MonoBehaviour 
{
	public enum WhichStatType {Player, Most, Total, Winner, GameDuration};

	public WhichStatType whichStatType;
	[ShowIfAttribute("ClassicStat")]
	public WhichStat whichStat;
	bool ClassicStat()
	{
		if (whichStatType != WhichStatType.Winner && whichStatType != WhichStatType.GameDuration)
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
		
	[ShowIfAttribute("PlayerText")]
	public Text playerText;
	bool PlayerText ()
	{
		if (whichStatType == WhichStatType.Most || whichStatType == WhichStatType.Player)
			return true;
		else
			return false;
	}

	private StatsManager stats;
	private Text textComponent;

	// Use this for initialization
	void Start () 
	{
		stats = StatsManager.Instance;
		textComponent = GetComponent<Text> ();

		UpdateText ();
	}

	void OnEnable ()
	{
		textComponent = GetComponent<Text> ();

		UpdateText ();	
	}

	void UpdateText ()
	{
		switch (whichStatType)
		{
		case WhichStatType.Player:
			PlayerStats ();
			break;
		case WhichStatType.Most:
			MostStats ();
			break;
		case WhichStatType.Total:
			TotalStats ();
			break;
		case WhichStatType.Winner:
			Winner ();
			break;
		case WhichStatType.GameDuration:
			GameDuration ();
			break;
		}

		if(textComponent.text == "")
			textComponent.text = "0";
	}

	void PlayerStats ()
	{
		if (StatsManager.Instance.playersStats.Count == 0 || StatsManager.Instance.playersStats [whichPlayer.ToString ()].playersStats.Count == 0)
			return;

		textComponent.color = GlobalVariables.Instance.playersColor [(int)whichPlayer];
		textComponent.text = StatsManager.Instance.playersStats [whichPlayer.ToString ()].playersStats [whichStat.ToString ()].ToString ();
	}

	void MostStats ()
	{	
		if (StatsManager.Instance.mostStats.Count == 0)
			return;

		if (StatsManager.Instance.mostStats [whichStat.ToString ()].whichPlayer == WhichPlayer.None)
			return;

		textComponent.color = GlobalVariables.Instance.playersColor [(int)StatsManager.Instance.mostStats [whichStat.ToString ()].whichPlayer];
		playerText.color = GlobalVariables.Instance.playersColor [(int)StatsManager.Instance.mostStats [whichStat.ToString ()].whichPlayer];

		textComponent.text = StatsManager.Instance.mostStats [whichStat.ToString ()].value.ToString ();
		playerText.text = StatsManager.Instance.mostStats [whichStat.ToString ()].whichPlayer.ToString ();
	}

	void TotalStats ()
	{
		if (StatsManager.Instance.totalStats.Count == 0)
			return;

		textComponent.text = StatsManager.Instance.totalStats [whichStat.ToString ()].ToString ();
	}

	void Winner ()
	{
		textComponent.color = GlobalVariables.Instance.playersColor [(int)StatsManager.Instance.winnerName];
		textComponent.text = StatsManager.Instance.winner;
	}

	void GameDuration ()
	{
		textComponent.text = StatsManager.Instance.gameDuration;
	}
}