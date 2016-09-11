using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class StatsFeedback : MonoBehaviour 
{
	public enum WhichStatType {Player, Most, Total, Winner, Wins, WinsInARow, EachPlayerWin};

	public WhichStatType whichStatType;
	public WhichStat whichStat;
	public WhichPlayer whichPlayer = WhichPlayer.None;

	[Header ("Text")]
	public bool playerNameAtFirst = true;
	public string beforeNumberText;
	public string afterNumberText;

	[Header ("Display Condition")]
	public bool displayCondition = false;
	public int minimumNumber = 0;

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
				UpdateStats ();
			}
		}
		else
			EditorUpdateStats ();

		if (Application.isPlaying && displayCondition && stats != null)
		{
			if (CanDisplay() && !textComponent.enabled)
				textComponent.enabled = true;

			if (!CanDisplay() && textComponent.enabled)
				textComponent.enabled = false;
		}
	}
	
	void OnEnable ()
	{
		if (Application.isPlaying)
			UpdateStats ();
		else
			EditorUpdateStats ();

		if (Application.isPlaying && displayCondition && stats != null)
		{
			if (CanDisplay() && !textComponent.enabled)
				textComponent.enabled = true;

			if (!CanDisplay() && textComponent.enabled)
				textComponent.enabled = false;
		}
	}

	bool CanDisplay ()
	{
		switch (whichStatType)
		{
		case WhichStatType.Player:
			
			switch(whichStat)
			{
			case WhichStat.Frags:
				if (stats.playerStatsList [(int)whichPlayer].frags >= minimumNumber)
					return true;
				else
					return false;
			case WhichStat.Hits:
				if (stats.playerStatsList [(int)whichPlayer].hits >= minimumNumber)
					return true;
				else
					return false;
			case WhichStat.Death:
				if (stats.playerStatsList [(int)whichPlayer].death >= minimumNumber)
					return true;
				else
					return false;
			case WhichStat.Dash:
				if (stats.playerStatsList [(int)whichPlayer].dash >= minimumNumber)
					return true;
				else
					return false;
			case WhichStat.Shots:
				if (stats.playerStatsList [(int)whichPlayer].shots >= minimumNumber)
					return true;
				else
					return false;
			default:
				return true;
			}

		case WhichStatType.Most:
			if (stats.mostStatsList [(int)whichStat].statNumber >= minimumNumber)
				return true;
			else
				return false;

		case WhichStatType.Total:
			
			switch(whichStat)
			{
			case WhichStat.Frags:
				if (stats.totalFrags >= minimumNumber)
					return true;
				else
					return false;
			case WhichStat.Hits:
				if (stats.totalHits >= minimumNumber)
					return true;
				else
					return false;
			case WhichStat.Death:
				if (stats.totalDeath >= minimumNumber)
					return true;
				else
					return false;
			case WhichStat.Dash:
				if (stats.totalDash >= minimumNumber)
					return true;
				else
					return false;
			case WhichStat.Shots:
				if (stats.totalShots >= minimumNumber)
					return true;
				else
					return false;
			default:
				return true;
			}

		case WhichStatType.Winner:
			return true;

		case WhichStatType.Wins:
			if (stats.mostStatsList [6].statNumber >= minimumNumber)
				return true;
			else
				return false;
			
		case WhichStatType.WinsInARow:
			if (stats.winsInARowNumber >= minimumNumber)
				return true;
			else
				return false;

		default:
			return true;
		}
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
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.cubeColorplayer1) + ">";
			secondText = beforeNumberText + " " + color + "0" + "</color> " + afterNumberText;
			textComponent.text = "Player 1";
			textComponent.text = playerNameAtFirst ? color + textComponent.text + "</color> " + secondText : secondText + " " + color + textComponent.text + "</color> ";
			break;
		case WhichStatType.Most:
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.cubeColorplayer1) + ">";
			secondText = beforeNumberText + " " + color + "0" + "</color> " + afterNumberText;
			textComponent.text = "Player 1";
			textComponent.text = playerNameAtFirst ? color + textComponent.text + "</color> " + secondText : secondText + " " + color + textComponent.text + "</color> ";
			break;
		case WhichStatType.Total:
			textComponent.text = beforeNumberText + " " + "0" + " " + afterNumberText;
			break;
		case WhichStatType.Winner:
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.cubeColorplayer1) + ">";
			textComponent.text = beforeNumberText + " " + color + "Player 1" + "</color> " + afterNumberText;
			break;
		case WhichStatType.Wins:
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.cubeColorplayer1) + ">";
			textComponent.text = beforeNumberText + " " + color + "Player 1" + "</color> " + afterNumberText;
			break;
		case WhichStatType.WinsInARow:
			color = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.cubeColorplayer1) + ">";
			textComponent.text = "Player 1";
			secondText = beforeNumberText + " " + color + "2" + "</color> " + afterNumberText;
			textComponent.text = playerNameAtFirst ? color + textComponent.text + "</color> " + secondText : secondText + " " + color + textComponent.text + "</color> ";
			break;
		case WhichStatType.EachPlayerWin:
			string color1 = "";
			string color2 = "";
			string color3 = "";
			string color4 = "";
			color1 = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.cubeColorplayer1) + ">";
			color2 = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.cubeColorplayer2) + ">";
			color3 = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.cubeColorplayer3) + ">";
			color4 = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.cubeColorplayer4) + ">";
			textComponent.text = beforeNumberText + " " + color1 + "0" + "</color>, " + color2 + "0" + "</color>, " + color3 + "0" + "</color>, " + color4 + "0" + "</color>, " + afterNumberText;
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
			case WhichStatType.Wins:
				Wins ();
				break;
			case WhichStatType.WinsInARow:
				WinsInARow ();
				break;
			case WhichStatType.EachPlayerWin:
				EachPlayerWin ();
				break;
			}
		}
	}

	void PlayerStat ()
	{
		int number = 0;
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
			number = stats.playerStatsList [(int)whichPlayer].frags;
			break;
		case WhichStat.Hits:
			secondText = beforeNumberText + " " + color +  stats.playerStatsList [(int)whichPlayer].hits.ToString () + "</color> " + afterNumberText;
			textComponent.text = playerNameAtFirst ? color + textComponent.text + "</color> " + secondText : secondText + " " + color + textComponent.text + "</color> ";
			number = stats.playerStatsList [(int)whichPlayer].hits;
			break;
		case WhichStat.Death:
			secondText = beforeNumberText + " " + color + stats.playerStatsList [(int)whichPlayer].death.ToString () + "</color> " + afterNumberText;
			textComponent.text = playerNameAtFirst ? color + textComponent.text + "</color> " + secondText : secondText + " " + color + textComponent.text + "</color> ";
			number = stats.playerStatsList [(int)whichPlayer].death;
			break;
		case WhichStat.Dash:
			secondText = beforeNumberText + " " + color + stats.playerStatsList [(int)whichPlayer].dash.ToString () + "</color> " + afterNumberText;
			textComponent.text = playerNameAtFirst ? color + textComponent.text + "</color> " + secondText : secondText + " " + color + textComponent.text + "</color> ";
			number = stats.playerStatsList [(int)whichPlayer].dash;
			break;
		case WhichStat.Shots:
			secondText = beforeNumberText + " " + color + stats.playerStatsList [(int)whichPlayer].shots.ToString () + "</color> " + afterNumberText;
			textComponent.text = playerNameAtFirst ? color + textComponent.text + "</color> " + secondText : secondText + " " + color + textComponent.text + "</color> ";
			number = stats.playerStatsList [(int)whichPlayer].shots;
			break;
		}

		if(number != 0)
			textComponent.enabled = true;
		else
			textComponent.enabled = false;
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

		if(stats.mostStatsList [(int)whichStat].statNumber != 0)
			textComponent.enabled = true;
		else
			textComponent.enabled = false;
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

		if(stats.winner != "")
			textComponent.enabled = true;
		else
			textComponent.enabled = false;
	}

	void Wins ()
	{
		string color = "";
		string number = stats.mostStatsList [6].statNumber.ToString ();
		string secondText;

		switch (stats.mostStatsList [6].whichPlayer)
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

		secondText = beforeNumberText + " " + color + number + "</color> " + afterNumberText;
		textComponent.text = playerNameAtFirst ? color + textComponent.text + "</color> " + secondText : secondText + " " + color + textComponent.text + "</color> ";

		if(stats.mostStatsList [6].statNumber != 0)
			textComponent.enabled = true;
		else
			textComponent.enabled = false;
	}

	void WinsInARow ()
	{
		string color = "";
		string number = stats.winsInARowNumber.ToString ();
		string secondText;

		switch (stats.mostWinsInARow)
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

		secondText = beforeNumberText + " " + color + number + "</color> " + afterNumberText;
		textComponent.text = playerNameAtFirst ? color + textComponent.text + "</color> " + secondText : secondText + " " + color + textComponent.text + "</color> ";

	}

	void EachPlayerWin ()
	{
		string color1 = "";
		string color2 = "";
		string color3 = "";
		string color4 = "";
		string number1 = stats.playerStatsList[0].wins.ToString ();
		string number2 = stats.playerStatsList[1].wins.ToString ();
		string number3 = stats.playerStatsList[2].wins.ToString ();
		string number4 = stats.playerStatsList[3].wins.ToString ();

		color1 = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player1.GetComponent<Renderer> ().material.color) + ">";
		color2 = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player2.GetComponent<Renderer> ().material.color) + ">";
		color3 = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player3.GetComponent<Renderer> ().material.color) + ">";
		color4 = "<color=#" + ColorUtility.ToHtmlStringRGBA (GlobalVariables.Instance.Player4.GetComponent<Renderer> ().material.color) + ">";

		textComponent.text = beforeNumberText + " " + color1 + number1 + "</color>, " + color2 + number2 + "</color>, " + color3 + number3 + "</color>, " + color4 + number4 + "</color>, " + afterNumberText;
	}
}