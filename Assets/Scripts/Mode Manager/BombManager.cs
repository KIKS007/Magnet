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
	public float timeBeforeFirstSpawn = 1;
	public float timeBetweenSpawn = 2;

	[Header ("Timer")]
	public Text[] timerTexts = new Text[0];
	public float timer;
	public string timerClock;
	public float timeBeforeEndGame = 2;

	private bool firstSpawn = true;

	// Use this for initialization
	void Start () 
	{
		bomb = GameObject.FindGameObjectWithTag ("Movable").gameObject;
		bomb.SetActive (false);

		/*for(int i = 0; i < timerTexts.Length; i++)
			timerTexts[i].text = "0";*/

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
		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing || bomb.GetComponent<MovableBomb>() == false);

		StartCoroutine (Timer ());
	}

	IEnumerator Timer ()
	{
		timer -= Time.deltaTime;

		string seconds = Mathf.Floor(timer % 60).ToString("00");

		//timerClock = minutes + ":" + seconds;
		timerClock = seconds;

		for(int i = 0; i < timerTexts.Length; i++)
			timerTexts[i].text = timerClock;

		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing || bomb.activeSelf == false);

		if(timer > 0)
		{
			StartCoroutine (Timer ());
		}

		else
		{
			for(int i = 0; i < timerTexts.Length; i++)
				timerTexts[i].text = "00";
			
			bomb.GetComponent<MovableBomb> ().StartCoroutine ("Explode");
	
			yield return new WaitWhile (()=> bomb.activeSelf == true);

			playersNumber--;

			switch(playersNumber)
			{
			case 4:
				timer = firstBombTimer;
				StartCoroutine (SpawnBomb ());
				yield return new WaitWhile (() => bomb.GetComponent<MovableBomb> ().playerHolding == null);

				StartCoroutine (Timer ());
				break;
			case 3:
				timer = secondBombTimer;
				StartCoroutine (SpawnBomb ());
				yield return new WaitWhile (() => bomb.GetComponent<MovableBomb> ().playerHolding == null);

				StartCoroutine (Timer ());
				break;
			case 2:
				timer = thirdBombTimer;
				StartCoroutine (SpawnBomb ());
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
		float timeBeforeSpawn = firstSpawn ? timeBeforeFirstSpawn : timeBetweenSpawn;
		firstSpawn = false;

		yield return new WaitForSeconds (timeBeforeSpawn);

		bomb.GetComponent<MovableBomb> ().ResetColor ();
		GlobalMethods.Instance.SpawnExistingMovableVoid (bomb, new Vector3(0, 2, 0));

		yield return new WaitUntil(() => bomb.activeSelf == true);

		yield return new WaitForSeconds (1.5f);

		if(bomb.GetComponent<MovableBomb>().playerHolding == null)
			playersList [Random.Range (0, playersList.Length)].GetComponent<PlayersBomb> ().GetBomb (bomb.GetComponent<Collider>());

	}

	IEnumerator GameEnd ()
	{
		playersList = GameObject.FindGameObjectsWithTag("Player");

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

		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().StartEndGameSlowMotion();
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking();

		yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(timeBeforeEndGame));

		GameObject.FindGameObjectWithTag("MainMenuManager").GetComponent<MainMenuManagerScript>().GameOverMenuVoid ();
	}
}
