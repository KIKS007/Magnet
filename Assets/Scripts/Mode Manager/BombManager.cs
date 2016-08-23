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

		StartCoroutine (StartTimer ());
	}
	
	// Update is called once per frame
	void Update () 
	{
		playersList = GameObject.FindGameObjectsWithTag("Player");
	}

	IEnumerator StartTimer ()
	{
		while(GlobalVariables.Instance.GameState != GameStateEnum.Playing)
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

		while(GlobalVariables.Instance.GameState != GameStateEnum.Playing)
		{
			yield return null;
		}

		if(timer > 0.01f)
			StartCoroutine (Timer ());

		else
		{
			transform.GetChild (0).GetChild (0).GetComponent<Text> ().text = "00:00";
			bomb.GetComponent<MovableBomb> ().StartCoroutine ("Explode");
	
			yield return new WaitWhile (()=> bomb.activeSelf == true);

			playersNumber--;

			switch(playersNumber)
			{
			case 4:
				timer = firstBombTimer;
				StartCoroutine (SpawnBomb ());
				yield return new WaitForSeconds (timeBetweenSpawn);
				StartCoroutine (Timer ());
				break;
			case 3:
				timer = secondBombTimer;
				StartCoroutine (SpawnBomb ());
				yield return new WaitForSeconds (timeBetweenSpawn);
				StartCoroutine (Timer ());
				break;
			case 2:
				timer = thirdBombTimer;
				StartCoroutine (SpawnBomb ());
				yield return new WaitForSeconds (timeBetweenSpawn);
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
		GlobalMethods.Instance.SpawnExistingMovable (bomb, new Vector3(0, 2, 0));

		yield return new WaitForSeconds (0.5f);

		playersList [Random.Range (0, playersList.Length)].GetComponent<PlayerBomb> ().GetBomb (bomb.GetComponent<Collider>());
	}

	IEnumerator GameEnd ()
	{
		GlobalVariables.Instance.GameState = GameStateEnum.Over;

		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().StartPauseSlowMotion();
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking();

		yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(timeBeforeEndGame));

		GameObject.FindGameObjectWithTag("MainMenuManager").GetComponent<MainMenuManagerScript>().GameOverMenuVoid ();
	}
}
