using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HitModeManager : MonoBehaviour 
{
	public float timer;
	public string timerClock;
	public bool gameEnded;

	// Use this for initialization
	void Start () 
	{
		timer = StaticVariables.Instance.timerDuration;

		StartCoroutine (Timer ());
	}

	IEnumerator Timer ()
	{
		timer -= Time.deltaTime;

		string minutes = Mathf.Floor(timer / 60).ToString("00");
		string seconds = Mathf.Floor(timer % 60).ToString("00");

		timerClock = minutes + ":" + seconds;

		transform.GetChild (0).GetChild (0).GetComponent<Text> ().text = timerClock;

		yield return null;

		if(timer > 0.01f)
			StartCoroutine (Timer ());

		else
		{
			GameEnded ();
			transform.GetChild (0).GetChild (0).GetComponent<Text> ().text = "00:00";
		}
			
	}

	void GameEnded ()
	{
		gameEnded = true;

		StaticVariables.Instance.GameOver = true;
	}
}
