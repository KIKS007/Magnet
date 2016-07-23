using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HitModeManager : MonoBehaviour 
{
	[Header ("Hit Settings")]
	public int timerDuration = 300;
	public float timeBetweenSpawn = 2;

	[Header ("Timer")]
	public float timer;
	public string timerClock;

	// Use this for initialization
	void Start () 
	{
		timer = timerDuration;

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

		string minutes = Mathf.Floor(timer / 60).ToString("0");
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
		GlobalVariables.Instance.GameOver = true;
	}
}
