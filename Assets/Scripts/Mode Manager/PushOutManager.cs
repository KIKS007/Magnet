using UnityEngine;
using System.Collections;

public class PushOutManager : MonoBehaviour 
{
	public GameObject[] playersList;

	public float timeBeforeEndGame;

	private bool gameEndLoopRunning = false;

	// Update is called once per frame
	void Update () 
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing)
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
		GlobalVariables.Instance.GameState = GameStateEnum.Over;

		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().StartEndGameSlowMotion();
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking(SlowMotionType.ModeEnd);

		yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(timeBeforeEndGame));

		GameObject.FindGameObjectWithTag("MainMenuManager").GetComponent<MainMenuManagerScript>().GameOverMenuVoid ();
	}
	
}
