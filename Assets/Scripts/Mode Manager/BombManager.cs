using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BombManager : MonoBehaviour 
{
	[Header ("Bomb Settings")]
	public GameObject[] playersList;
	public GameObject bomb;
	public int playersNumber;
	public int firstBombTimer = 300;
	public int secondBombTimer = 300;
	public int thirdBombTimer = 300;
	public float timeBetweenSpawn = 2;

	[Header ("Timer")]
	public float timer;
	public string timerClock;
	public float timeBeforeEndGame = 2;

	// Use this for initialization
	void Start () 
	{
		bomb = GameObject.FindGameObjectWithTag ("Movable").gameObject;
		bomb.SetActive (false);

		transform.GetChild (0).GetChild (0).GetComponent<Text> ().text = "0:00";

		StartCoroutine (Setup ());
	}

	IEnumerator Setup ()
	{
		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		while (GlobalVariables.Instance.NumberOfPlayers == 0)
			yield return null;

		playersNumber = GlobalVariables.Instance.NumberOfPlayers;

		switch(playersNumber)
		{
		case 4:
			timer = firstBombTimer;
			break;
		case 3:
			timer = secondBombTimer;
			break;
		case 2:
			timer = thirdBombTimer;
			break;
		}

		StartCoroutine (SpawnBomb ());

		yield return new WaitWhile (() => bomb.GetComponent<MovableBomb> ().playerHolding == null);

		StartCoroutine (StartTimer ());
	}
	
	// Update is called once per frame
	void Update () 
	{
		playersList = GameObject.FindGameObjectsWithTag("Player");
	}

	IEnumerator StartTimer ()
	{
		while(GlobalVariables.Instance.GameState != GameStateEnum.Playing || bomb.activeSelf == false)
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

		while(GlobalVariables.Instance.GameState != GameStateEnum.Playing || bomb.activeSelf == false)
		{
			yield return null;
		}

		if(timer > 0.01f)
		{
			yield return new WaitWhile (() => bomb.GetComponent<MovableBomb> ().playerHolding == null);

			StartCoroutine (Timer ());
		}

		else
		{
			transform.GetChild (0).GetChild (0).GetComponent<Text> ().text = "0:00";
			bomb.GetComponent<MovableBomb> ().StartCoroutine ("Explode");
	
			yield return new WaitWhile (()=> bomb.activeSelf == true);

			playersNumber--;

			switch(playersNumber)
			{
			case 4:
				timer = firstBombTimer;
				StartCoroutine (SpawnBomb ());
				yield return new WaitForSeconds (timeBetweenSpawn);

				yield return new WaitWhile (() => bomb.GetComponent<MovableBomb> ().playerHolding == null);

				StartCoroutine (Timer ());
				break;
			case 3:
				timer = secondBombTimer;
				StartCoroutine (SpawnBomb ());
				yield return new WaitForSeconds (timeBetweenSpawn);

				yield return new WaitWhile (() => bomb.GetComponent<MovableBomb> ().playerHolding == null);

				StartCoroutine (Timer ());
				break;
			case 2:
				timer = thirdBombTimer;
				StartCoroutine (SpawnBomb ());
				yield return new WaitForSeconds (timeBetweenSpawn);

				yield return new WaitWhile (() => bomb.GetComponent<MovableBomb> ().playerHolding == null);

				StartCoroutine (Timer ());
				break;
			case 1:
				StartCoroutine(GameEnd ());
				break;
			}
		}

	}

	IEnumerator SpawnBomb ()
	{
		yield return new WaitForSeconds (timeBetweenSpawn - 0.5f);

		bomb.GetComponent<MovableBomb> ().ResetColor ();
		GlobalMethods.Instance.SpawnExistingMovableVoid (bomb, new Vector3(0, 2, 0));

		yield return new WaitUntil(() => bomb.activeSelf == true);

		yield return new WaitForSeconds (1.5f);

		if(bomb.GetComponent<MovableBomb>().playerHolding == null)
			playersList [Random.Range (0, playersList.Length)].GetComponent<PlayersBomb> ().GetBomb (bomb.GetComponent<Collider>());

	}

	IEnumerator GameEnd ()
	{
		switch (playersList [0].name)
		{
		case "Player 1":
			StatsManager.Instance.Winner(WhichPlayer.Player1);
			break;
		case "Player 2":
			StatsManager.Instance.Winner(WhichPlayer.Player2);
			break;
		case "Player 3":
			StatsManager.Instance.Winner(WhichPlayer.Player3);
			break;
		case "Player 4":
			StatsManager.Instance.Winner(WhichPlayer.Player4);
			break;
		}
			
		GlobalVariables.Instance.GameState = GameStateEnum.Over;

		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().StartPauseSlowMotion();
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking();

		yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(timeBeforeEndGame));

		GameObject.FindGameObjectWithTag("MainMenuManager").GetComponent<MainMenuManagerScript>().GameOverMenuVoid ();
	}
}
