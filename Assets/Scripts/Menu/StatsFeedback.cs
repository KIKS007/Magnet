using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum WhichStatType {Player, Most, Total, Winner};

[ExecuteInEditMode]
public class StatsFeedback : MonoBehaviour 
{
	public WhichStatType whichStatType;
	public WhichStat whichStat;
	public WhichPlayer whichPlayer = WhichPlayer.None;

	[Header ("Text")]
	public bool playerNameAtFirst = true;
	public string beforeNumberText;
	public string afterNumberText;

	private StatsManager stats;
	private Text textComponent;

	// Use this for initialization
	void Start () 
	{
		if(Application.isPlaying)
		{
			stats = StatsManager.Instance;
			textComponent = GetComponent<Text> ();

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
	}
	
	void OnEnable ()
	{
		if (Application.isPlaying)
			UpdateStats ();
		else
			EditorUpdateStats ();
	}

	void EditorUpdateStats ()
	{
		stats = StatsManager.Instance;
		textComponent = GetComponent<Text> ();

		textComponent.text = "";

		string color = "";
		string secondText;

		switch (whichStatType)
		{
		case WhichStatType.Player:
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (Color.red) + ">";
			secondText = beforeNumberText + " " + color + "0" + "</color> " + afterNumberText;
			textComponent.text = "Player 1";
			textComponent.text = playerNameAtFirst ? color + textComponent.text + "</color> " + secondText : secondText + " " + color + textComponent.text + "</color> ";
			break;
		case WhichStatType.Most:
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (Color.red) + ">";
			secondText = beforeNumberText + " " + color + "0" + "</color> " + afterNumberText;
			textComponent.text = "Player 1";
			textComponent.text = playerNameAtFirst ? color + textComponent.text + "</color> " + secondText : secondText + " " + color + textComponent.text + "</color> ";
			break;
		case WhichStatType.Total:
			textComponent.text = beforeNumberText + " " + stats.totalFrags.ToString () + " " + afterNumberText;
			break;
		case WhichStatType.Winner:
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (Color.red) + ">";
			textComponent.text = beforeNumberText + " " + color + "Player 1" + "</color> " + afterNumberText;
			break;
		}	

	}

	void UpdateStats ()
	{
		if(stats != null)
		{
			textComponent.text = "";

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
			textComponent.text = "Player 1";
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player1.GetComponent<Renderer> ().material.color) + ">";
			break;
		case WhichPlayer.Player2:
			textComponent.text = "Player 2";
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player2.GetComponent<Renderer> ().material.color) + ">";
			break;
		case WhichPlayer.Player3:
			textComponent.text = "Player 3";
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player3.GetComponent<Renderer> ().material.color) + ">";
			break;
		case WhichPlayer.Player4:
			textComponent.text = "Player 4";
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player4.GetComponent<Renderer> ().material.color) + ">";
			break;
		}

		string secondText;

		switch(whichStat)
		{
		case WhichStat.Frags:
			secondText = beforeNumberText + " " + color + stats.playerStatsList [(int)whichPlayer].frags.ToString () + "</color> " + afterNumberText;
			textComponent.text = playerNameAtFirst ? color + textComponent.text + "</color> " + secondText : secondText + " " + color + textComponent.text + "</color> ";
			break;
		case WhichStat.Hits:
			secondText = beforeNumberText + " " + color +  stats.playerStatsList [(int)whichPlayer].hits.ToString () + "</color> " + afterNumberText;
			textComponent.text = playerNameAtFirst ? color + textComponent.text + "</color> " + secondText : secondText + " " + color + textComponent.text + "</color> ";
			break;
		case WhichStat.Death:
			secondText = beforeNumberText + " " + color + stats.playerStatsList [(int)whichPlayer].death.ToString () + "</color> " + afterNumberText;
			textComponent.text = playerNameAtFirst ? color + textComponent.text + "</color> " + secondText : secondText + " " + color + textComponent.text + "</color> ";
			break;
		case WhichStat.Dash:
			secondText = beforeNumberText + " " + color + stats.playerStatsList [(int)whichPlayer].dash.ToString () + "</color> " + afterNumberText;
			textComponent.text = playerNameAtFirst ? color + textComponent.text + "</color> " + secondText : secondText + " " + color + textComponent.text + "</color> ";
			break;
		case WhichStat.Shots:
			secondText = beforeNumberText + " " + color + stats.playerStatsList [(int)whichPlayer].shots.ToString () + "</color> " + afterNumberText;
			textComponent.text = playerNameAtFirst ? color + textComponent.text + "</color> " + secondText : secondText + " " + color + textComponent.text + "</color> ";
			break;
		}
	}

	void MostStat ()
	{
		string color = "";
		string number = stats.mostStatsList [(int)whichStat].statNumber.ToString ();

		switch (stats.mostStatsList [(int)whichStat].whichPlayer)
		{
		case WhichPlayer.Player1:
			textComponent.text = "Player 1";
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player1.GetComponent<Renderer> ().material.color) + ">";
			break;
		case WhichPlayer.Player2:
			textComponent.text = "Player 2";
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player2.GetComponent<Renderer> ().material.color) + ">";
			break;
		case WhichPlayer.Player3:
			textComponent.text = "Player 3";
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player3.GetComponent<Renderer> ().material.color) + ">";
			break;
		case WhichPlayer.Player4:
			textComponent.text = "Player 4";
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player4.GetComponent<Renderer> ().material.color) + ">";
			break;
		}

		string secondText;

		switch(whichStat)
		{
		case WhichStat.Frags:
			secondText = beforeNumberText + " " + color + number + "</color> " + afterNumberText;
			textComponent.text = playerNameAtFirst ? color + textComponent.text + "</color> " + secondText : secondText + " " + color + textComponent.text + "</color> ";
			break;
		case WhichStat.Hits:
			secondText = beforeNumberText + " " + color +  number + "</color> " + afterNumberText;
			textComponent.text = playerNameAtFirst ? color + textComponent.text + "</color> " + secondText : secondText + " " + color + textComponent.text + "</color> ";
			break;
		case WhichStat.Death:
			secondText = beforeNumberText + " " + color + number + "</color> " + afterNumberText;
			textComponent.text = playerNameAtFirst ? color + textComponent.text + "</color> " + secondText : secondText + " " + color + textComponent.text + "</color> ";
			break;
		case WhichStat.Dash:
			secondText = beforeNumberText + " " + color + number + "</color> " + afterNumberText;
			textComponent.text = playerNameAtFirst ? color + textComponent.text + "</color> " + secondText : secondText + " " + color + textComponent.text + "</color> ";
			break;
		case WhichStat.Shots:
			secondText = beforeNumberText + " " + color + number + "</color> " + afterNumberText;
			textComponent.text = playerNameAtFirst ? color + textComponent.text + "</color> " + secondText : secondText + " " + color + textComponent.text + "</color> ";
			break;
		case WhichStat.AimAccuracy:
			secondText = beforeNumberText + " " + color + number + "</color> " + afterNumberText;
			textComponent.text = playerNameAtFirst ? color + textComponent.text + "</color> " + secondText : secondText + " " + color + textComponent.text + "</color> ";
			break;
		}
	}

	void TotalStat ()
	{
		switch(whichStat)
		{
		case WhichStat.Frags:
			textComponent.text = beforeNumberText + " " + stats.totalFrags.ToString () + " " + afterNumberText;
			break;
		case WhichStat.Hits:
			textComponent.text = beforeNumberText + " " + stats.totalHits.ToString () + " " + afterNumberText;
			break;
		case WhichStat.Death:
			textComponent.text = beforeNumberText + " " + stats.totalDeath.ToString () + " " + afterNumberText;
			break;
		case WhichStat.Dash:
			textComponent.text = beforeNumberText + " " + stats.totalDash.ToString () + " " + afterNumberText;
			break;
		case WhichStat.Shots:
			textComponent.text = beforeNumberText + " " + stats.totalShots.ToString () + " " + afterNumberText;
			break;
		}
	}

	void WinnerStat ()
	{
		string color = "";

		switch (stats.winner)
		{
		case "Player 1":
			textComponent.text = stats.winner;
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player1.GetComponent<Renderer> ().material.color) + ">";
			break;
		case "Player 2":
			textComponent.text = stats.winner;
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player2.GetComponent<Renderer> ().material.color) + ">";
			break;
		case "Player 3":
			textComponent.text = stats.winner;
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player3.GetComponent<Renderer> ().material.color) + ">";
			break;
		case "Player 4":
			textComponent.text = stats.winner;
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player4.GetComponent<Renderer> ().material.color) + ">";
			break;
		}

		textComponent.text = beforeNumberText + " " + color + textComponent.text + "</color> " + afterNumberText;
	}
}