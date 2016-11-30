using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DarkTonic.MasterAudio;
using DG.Tweening;

public class BombManager : MonoBehaviour 
{
	public WhichMode whichMode;

	[Header ("Bomb Settings")]
	public GameObject[] playersList;
	public GameObject bomb;
	public int playersNumber;
	public int firstBombTimer = 300;
	public int secondBombTimer = 300;
	public int thirdBombTimer = 300;
	public float timeBeforeFirstSpawn = 1;
	public float timeBetweenSpawn = 2;

	[Header ("Timer Sounds")]
	[SoundGroupAttribute]
	public string lastSecondsSound;
	[SoundGroupAttribute]
	public string cubeTrackingSound;

	[Header ("Timer")]
	public Text timerText;
	public float timer;
	public string timerClock;
	public float timeBeforeEndGame = 1;

	private bool firstSpawn = true;

	private bool lastSeconds = false;

	private MovableBomb bombScript;

	private int textInitialSize;
	private Vector3 textLocalPosition;

	// Use this for initialization
	void Start () 
	{
		bomb = GameObject.FindGameObjectWithTag ("Movable").gameObject;
		bomb.gameObject.SetActive(false);
		bombScript = bomb.GetComponent<MovableBomb> ();
		textInitialSize = timerText.fontSize;
		timerText.fontSize = 0;
		textLocalPosition = timerText.transform.parent.transform.localPosition;
		timerText.transform.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(90, 0, 0));

		timerText.transform.parent.SetParent (GameObject.FindGameObjectWithTag("MovableParent").transform);

		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<DynamicCamera> ().otherTargetsList.Clear ();
		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<DynamicCamera> ().otherTargetsList.Add (bomb);

		StartCoroutine (Setup ());
	}

	IEnumerator Setup ()
	{
		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

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

		string seconds = Mathf.Floor(timer % 60).ToString("00");
		timerClock = seconds;

		timerText.text = timerClock;

		StartCoroutine (SpawnBomb ());

		yield return new WaitWhile (() => bombScript.playerHolding == null);

		StartCoroutine (Timer ());
	}
	
	// Update is called once per frame
	void Update () 
	{
		playersList = GameObject.FindGameObjectsWithTag("Player");

		if(bomb.activeSelf == true && !lastSeconds && timer < 4)
		{
			lastSeconds = true;
			MasterAudio.PlaySound3DAtTransformAndForget (lastSecondsSound, bomb.transform);
		}
	}

	IEnumerator Timer ()
	{
		timer -= Time.deltaTime;

		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing || bomb.activeSelf == false);

		if(timer > 0)
		{
			string seconds = Mathf.Floor(timer % 60).ToString("00");
			timerClock = seconds;

			timerText.text = timerClock;
		

			StartCoroutine (Timer ());
		}

		else
		{
			timerText.text = "00";
			
			MasterAudio.PlaySound3DAtTransformAndForget (cubeTrackingSound, bomb.transform);
			bombScript.StartCoroutine ("Explode");
	

			yield return new WaitWhile (()=> bomb.activeSelf == true);

			playersNumber--;

			switch(playersNumber)
			{
			case 4:
				timer = firstBombTimer;
				StartCoroutine (SpawnBomb ());
				yield return new WaitWhile (() => bombScript.playerHolding == null);

				StartCoroutine (Timer ());
				break;
			case 3:
				timer = secondBombTimer;
				StartCoroutine (SpawnBomb ());
				yield return new WaitWhile (() => bombScript.playerHolding == null);

				StartCoroutine (Timer ());
				break;
			case 2:
				timer = thirdBombTimer;
				StartCoroutine (SpawnBomb ());
				yield return new WaitWhile (() => bombScript.playerHolding == null);

				StartCoroutine (Timer ());
				break;
			case 1:
				StartCoroutine(GameEnd ());
				break;
			}

			string seconds = Mathf.Floor(timer % 60).ToString("00");
			timerClock = seconds;

			timerText.text = timerClock;

			lastSeconds = false;
		}

	}

	IEnumerator SpawnBomb ()
	{
		float timeBeforeSpawn = firstSpawn ? timeBeforeFirstSpawn : timeBetweenSpawn;
		firstSpawn = false;

		yield return new WaitForSeconds (timeBeforeSpawn);

		bombScript.ResetColor ();
		bomb.tag = "Movable";

		timerText.transform.parent.SetParent (GameObject.FindGameObjectWithTag("MovableParent").transform);
		timerText.fontSize = 0;

		GlobalMethods.Instance.SpawnExistingMovableVoid (bomb, new Vector3(0, 2, 0));

		yield return new WaitWhile (()=> bomb.activeSelf == false);

		yield return new WaitForSeconds (0.5f);

		timerText.transform.parent.SetParent (bomb.transform);
		timerText.transform.parent.transform.localPosition = textLocalPosition;
		timerText.transform.parent.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
		timerText.transform.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

		DOTween.To(()=> timerText.fontSize, x=> timerText.fontSize =x, textInitialSize, 0.2f);

		yield return new WaitForSeconds (1f);

		if(bomb.GetComponent<MovableBomb>().playerHolding == null && bomb.GetComponent<MovableScript>().hold == false)
		{
			if(bomb.GetComponent<MovableScript>().attracedBy.Count == 0)
				playersList [Random.Range (0, playersList.Length)].GetComponent<PlayersBomb> ().GetBomb (bomb.GetComponent<Collider>());
			
			else if(bomb.GetComponent<MovableScript>().attracedBy.Count > 0)
				bomb.GetComponent<MovableScript>().attracedBy[0].GetComponent<PlayersBomb> ().GetBomb (bomb.GetComponent<Collider>());			
		}
	}
		
	IEnumerator GameEnd ()
	{
		playersList = GameObject.FindGameObjectsWithTag("Player");

		StatsManager.Instance.Winner(playersList [0].GetComponent<PlayersGameplay> ().playerName);
			
		GlobalVariables.Instance.GameState = GameStateEnum.Over;

		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().StartEndGameSlowMotion();
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking(SlowMotionType.ModeEnd);

		yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(timeBeforeEndGame));

		//GameObject.FindGameObjectWithTag("MainMenuManager").GetComponent<MainMenuManagerScript>().GameOverMenuVoid ();
		MenuManager.Instance.endModeMenu.EndMode (whichMode);
	}
}
