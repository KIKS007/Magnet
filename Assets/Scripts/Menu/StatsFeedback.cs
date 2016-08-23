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
	private Text childText;

	// Use this for initialization
	void Start () 
	{
		stats = StatsManager.Instance;
		parentText = GetComponent<Text> ();
		childText = transform.GetChild(0).GetComponent<Text> ();
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
		switch(whichStat)
		{
		case WhichStat.Frags:
			childText.text = stats.playerStatsList [(int)whichPlayer].frags.ToString ();
			break;
		case WhichStat.Hits:
			childText.text = stats.playerStatsList [(int)whichPlayer].hits.ToString ();
			break;
		case WhichStat.Death:
			childText.text = stats.playerStatsList [(int)whichPlayer].death.ToString ();
			break;
		case WhichStat.Dash:
			childText.text = stats.playerStatsList [(int)whichPlayer].dash.ToString ();
			break;
		case WhichStat.Shots:
			childText.text = stats.playerStatsList [(int)whichPlayer].shots.ToString ();
			break;
		}
	}

	void MostStat ()
	{
		parentText.text = stats.mostStatsList [(int)whichStat].whichPlayer.ToString ();
		childText.text = stats.mostStatsList [(int)whichStat].statNumber.ToString ();

		switch (stats.mostStatsList [(int)whichStat].whichPlayer)
		{
		case WhichPlayer.Player1:
			childText.color = GlobalVariables.Instance.Player1.GetComponent<Renderer> ().material.color;
			break;
		case WhichPlayer.Player2:
			childText.color = GlobalVariables.Instance.Player2.GetComponent<Renderer> ().material.color;
			break;
		case WhichPlayer.Player3:
			childText.color = GlobalVariables.Instance.Player3.GetComponent<Renderer> ().material.color;
			break;
		case WhichPlayer.Player4:
			childText.color = GlobalVariables.Instance.Player4.GetComponent<Renderer> ().material.color;
			break;
		}
	}

	void TotalStat ()
	{
		switch(whichStat)
		{
		case WhichStat.Frags:
			childText.text = stats.totalFrags.ToString ();
			break;
		case WhichStat.Hits:
			childText.text = stats.totalHits.ToString ();
			break;
		case WhichStat.Death:
			childText.text = stats.totalDeath.ToString ();
			break;
		case WhichStat.Dash:
			childText.text = stats.totalDash.ToString ();
			break;
		case WhichStat.Shots:
			childText.text = stats.totalShots.ToString ();
			break;
		}
	}

	void WinnerStat ()
	{
		if(stats.winner != "")
		childText.text = stats.winner;

		switch (stats.winner)
		{
		case "Player 1":
			childText.color = GlobalVariables.Instance.Player1.GetComponent<Renderer> ().material.color;
			break;
		case "Player 2":
			childText.color = GlobalVariables.Instance.Player2.GetComponent<Renderer> ().material.color;
			break;
		case "Player 3":
			childText.color = GlobalVariables.Instance.Player3.GetComponent<Renderer> ().material.color;
			break;
		case "Player 4":
			childText.color = GlobalVariables.Instance.Player4.GetComponent<Renderer> ().material.color;
			break;
		}
	}
}
