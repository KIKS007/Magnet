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
		timer = GlobalVariables.Instance.timerDuration;

		StartCoroutine (StartTimer ());
	}

	IEnumerator StartTimer ()
	{
		while(GlobalVariables.Instance.GameOver == true || GlobalVariables.Instance.GamePaused == true)
		{
			yield return null;
		}

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

		while(GlobalVariables.Instance.GameOver == true || GlobalVariables.Instance.GamePaused == true)
		{
			yield return null;
		}

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

		GlobalVariables.Instance.GameOver = true;
	}
}
