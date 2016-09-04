using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StatsFeedback : MonoBehaviour 
{
	public enum WhichStatType {Player, Most, Total, Winner};

	public WhichStatType whichStatType;
	public WhichStat whichStat;
	public WhichPlayer whichPlayer = WhichPlayer.None;

	private StatsManager stats;
	private Text parentText;
	private Text firstChildText;

	// Use this for initialization
	void Start () 
	{
		stats = StatsManager.Instance;
		parentText = GetComponent<Text> ();
		firstChildText = transform.GetChild(0).GetComponent<Text> ();

		if(stats != null)
		{
			switch (whichStatType)
			{
			case WhichStatType.Player:
				PlayerStat ();
				break;
			case WhichStatType.Most:
				MostStat ();
				break;
			case WhichStatType.Total:
				TotalStat ();
				break;
			case WhichStatType.Winner:
				WinnerStat ();
				break;
			}
		}
	}
	
	void OnEnable ()
	{
		if(stats != null)
		{
			switch (whichStatType)
			{
			case WhichStatType.Player:
				PlayerStat ();
				break;
			case WhichStatType.Most:
				MostStat ();
				break;
			case WhichStatType.Total:
				TotalStat ();
				break;
			case WhichStatType.Winner:
				WinnerStat ();
				break;
			}
		}
	}

	void PlayerStat ()
	{
		string color = "";

		switch (whichPlayer)
		{
		case WhichPlayer.Player1:
			parentText.text = "Player 1";
			parentText.color = GlobalVariables.Instance.Player1.GetComponent<Renderer> ().material.color;
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player1.GetComponent<Renderer> ().material.color) + ">";
			break;
		case WhichPlayer.Player2:
			parentText.text = "Player 2";
			parentText.color = GlobalVariables.Instance.Player2.GetComponent<Renderer> ().material.color;
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player2.GetComponent<Renderer> ().material.color) + ">";
			break;
		case WhichPlayer.Player3:
			parentText.text = "Player 3";
			parentText.color = GlobalVariables.Instance.Player3.GetComponent<Renderer> ().material.color;
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player3.GetComponent<Renderer> ().material.color) + ">";
			break;
		case WhichPlayer.Player4:
			parentText.text = "Player 4";
			parentText.color = GlobalVariables.Instance.Player4.GetComponent<Renderer> ().material.color;
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player4.GetComponent<Renderer> ().material.color) + ">";
			break;
		}

		switch(whichStat)
		{
		case WhichStat.Frags:
			firstChildText.text = stats.playerStatsList [(int)whichPlayer].frags.ToString ();
			firstChildText.text = "made " + color + stats.playerStatsList [(int)whichPlayer].frags.ToString () + "</color> frags";
			break;
		case WhichStat.Hits:
			firstChildText.text = stats.playerStatsList [(int)whichPlayer].hits.ToString ();
			firstChildText.text = "has been hit " + color +  stats.playerStatsList [(int)whichPlayer].hits.ToString () + "</color> times";
			break;
		case WhichStat.Death:
			firstChildText.text = stats.playerStatsList [(int)whichPlayer].death.ToString ();
			firstChildText.text = "died " + color + stats.playerStatsList [(int)whichPlayer].death.ToString () + "</color> times";
			break;
		case WhichStat.Dash:
			firstChildText.text = stats.playerStatsList [(int)whichPlayer].dash.ToString ();
			firstChildText.text = "made " + color + stats.playerStatsList [(int)whichPlayer].dash.ToString () + "</color> dashs";
			break;
		case WhichStat.Shots:
			firstChildText.text = stats.playerStatsList [(int)whichPlayer].shots.ToString ();
			firstChildText.text = "made " + color + stats.playerStatsList [(int)whichPlayer].shots.ToString () + "</color> shots";
			break;
		}
	}

	void MostStat ()
	{
		//parentText.text = stats.mostStatsList [(int)whichStat].whichPlayer.ToString ();

		string color = "";
		string number = stats.mostStatsList [(int)whichStat].statNumber.ToString ();

		switch (stats.mostStatsList [(int)whichStat].whichPlayer)
		{
		case WhichPlayer.Player1:
			parentText.text = "Player 1";
			parentText.color = GlobalVariables.Instance.Player1.GetComponent<Renderer> ().material.color;
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player1.GetComponent<Renderer> ().material.color) + ">";
			break;
		case WhichPlayer.Player2:
			parentText.text = "Player 2";
			parentText.color = GlobalVariables.Instance.Player2.GetComponent<Renderer> ().material.color;
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player2.GetComponent<Renderer> ().material.color) + ">";
			break;
		case WhichPlayer.Player3:
			parentText.text = "Player 3";
			parentText.color = GlobalVariables.Instance.Player3.GetComponent<Renderer> ().material.color;
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player3.GetComponent<Renderer> ().material.color) + ">";
			break;
		case WhichPlayer.Player4:
			parentText.text = "Player 4";
			parentText.color = GlobalVariables.Instance.Player4.GetComponent<Renderer> ().material.color;
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player4.GetComponent<Renderer> ().material.color) + ">";
			break;
		}

		switch(whichStat)
		{
		case WhichStat.Frags:
			firstChildText.text = "made " + color + number + "</color> frags";
			break;
		case WhichStat.Hits:
			firstChildText.text = "has been hit " + color +  number + "</color> times";
			break;
		case WhichStat.Death:
			firstChildText.text = "died " + color + number + "</color> times";
			break;
		case WhichStat.Dash:
			firstChildText.text = "made " + color + number + "</color> dashs";
			break;
		case WhichStat.Shots:
			firstChildText.text = "made " + color + number + "</color> shots";
			break;
		}
	}

	void TotalStat ()
	{
		switch(whichStat)
		{
		case WhichStat.Frags:
			firstChildText.text = stats.totalFrags.ToString ();
			break;
		case WhichStat.Hits:
			firstChildText.text = stats.totalHits.ToString ();
			break;
		case WhichStat.Death:
			firstChildText.text = stats.totalDeath.ToString ();
			break;
		case WhichStat.Dash:
			firstChildText.text = stats.totalDash.ToString ();
			break;
		case WhichStat.Shots:
			firstChildText.text = stats.totalShots.ToString ();
			break;
		}
	}

	void WinnerStat ()
	{
		parentText.text = "The winner is ";

		if(stats.winner != "")
			firstChildText.text = stats.winner;

		switch (stats.winner)
		{
		case "Player 1":
			firstChildText.color = GlobalVariables.Instance.Player1.GetComponent<Renderer> ().material.color;
			break;
		case "Player 2":
			firstChildText.color = GlobalVariables.Instance.Player2.GetComponent<Renderer> ().material.color;
			break;
		case "Player 3":
			firstChildText.color = GlobalVariables.Instance.Player3.GetComponent<Renderer> ().material.color;
			break;
		case "Player 4":
			firstChildText.color = GlobalVariables.Instance.Player4.GetComponent<Renderer> ().material.color;
			break;
		}
	}
}
