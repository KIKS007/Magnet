using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XboxCtrlrInput;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class CrushManager : MonoBehaviour 
{
	public GameObject[] playersList;

	public float timeBeforeEndGame;

	private bool gameEndLoopRunning = false;


	// Update is called once per frame
	void Update () 
	{
		if(GlobalVariables.Instance.GamePaused == false)
			FindPlayers ();

	}

	void FindPlayers ()
	{
		playersList = GameObject.FindGameObjectsWithTag("Player");

		if(playersList.Length == 1 && gameEndLoopRunning == false)
		{
			gameEndLoopRunning = true;

			StartCoroutine("GameEnd");
		}
	}

	IEnumerator GameEnd ()
	{
		GlobalVariables.Instance.GameOver = true;
		GlobalVariables.Instance.GamePaused = true;

		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().StartPauseSlowMotion();
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking();

		yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(timeBeforeEndGame));

		GameObject.FindGameObjectWithTag("MainMenuManager").GetComponent<MainMenuManagerScript>().GameOverMenuVoid ();
	}
	
}
