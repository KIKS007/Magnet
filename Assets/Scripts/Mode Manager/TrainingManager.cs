using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrainingManager : MonoBehaviour 
{
	public WhichMode whichMode;

	public List<GameObject> playersList = new List<GameObject>();

	public float timeBeforeEndGame = 2;

	private bool gameEndLoopRunning = false;

	void Start ()
	{
		StartCoroutine (WaitForBeginning ());
	}

	IEnumerator WaitForBeginning ()
	{
		GameObject[] allMovables = GameObject.FindGameObjectsWithTag ("Movable");

		for (int i = 0; i < allMovables.Length; i++)
			allMovables [i].SetActive (false);

		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		GlobalMethods.Instance.RandomPositionMovablesVoid (allMovables);
	}

	// Update is called once per frame
	void Update () 
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing)
			FindManikin ();

	}

	void FindManikin ()
	{
		GameObject[] manikinTemp = GameObject.FindGameObjectsWithTag("Player");
		playersList.Clear ();

		for(int i = 0; i < manikinTemp.Length; i++)
		{
			if (manikinTemp [i].GetComponent<PlayersManikin> () != null)
				playersList.Add (manikinTemp [i].gameObject);
		}

		if(playersList.Count == 0 && gameEndLoopRunning == false)
		{
			gameEndLoopRunning = true;

			StartCoroutine (GameEnd ());
		}
	}

	IEnumerator GameEnd ()
	{
		GlobalVariables.Instance.GameState = GameStateEnum.Over;

		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().StartEndGameSlowMotion();
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking(SlowMotionType.ModeEnd);

		yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(timeBeforeEndGame));

		MenuManager.Instance.endModeMenu.EndMode (whichMode);
	}	
}
