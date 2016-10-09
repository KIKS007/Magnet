using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WrapModeManager : MonoBehaviour 
{
	[Header ("Wrap Settings")]
	public int timerDuration = 300;
	public float timeBetweenSpawn = 2;
	public float timeBeforeEndGame = 2;

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
		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		StartCoroutine (Timer ());
	}

	IEnumerator Timer ()
	{
		timer -= Time.deltaTime;

		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		if(timer > 0)
		{
			string minutes = Mathf.Floor(timer / 60).ToString("0");
			string seconds = Mathf.Floor(timer % 60).ToString("00");
			
			timerClock = minutes + ":" + seconds;
			
			transform.GetChild (0).GetChild (0).GetComponent<Text> ().text = timerClock;
			
			StartCoroutine (Timer ());
		}

		else
		{
			StartCoroutine (GameEnded ());
			transform.GetChild (0).GetChild (0).GetComponent<Text> ().text = "0:00";
		}
	}

	IEnumerator GameEnded ()
	{
		GlobalVariables.Instance.GameState = GameStateEnum.Over;

		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().StartEndGameSlowMotion();
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking(SlowMotionType.ModeEnd);

		yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(timeBeforeEndGame));

		GameObject.FindGameObjectWithTag("MainMenuManager").GetComponent<MainMenuManagerScript>().GameOverMenuVoid ();
	}
}
