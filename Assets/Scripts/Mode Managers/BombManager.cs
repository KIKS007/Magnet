using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DarkTonic.MasterAudio;
using DG.Tweening;

public class BombManager : LastManManager 
{
	[Header ("Bomb Settings")]
	public GameObject bomb;
	public float timeBeforeFirstSpawn = 1;
	public float timeBetweenSpawn = 2;

	[Header ("Timer Settings")]
	public float currentTimer;
	public Vector2 timerLimits = new Vector2 (20, 5);
	public int eachBombReducedTime = 1;

	[Header ("Timer")]
	public Text timerText;
	public float timer;
	public string timerClock;

	protected bool firstSpawn = true;
	protected bool lastSeconds = false;

	protected MovableBomb bombScript;

	public int textInitialSize;
	public Vector3 textLocalPosition;

	protected void Awake ()
	{
		textInitialSize = timerText.fontSize;
		textLocalPosition = timerText.transform.parent.transform.localPosition;
	}

	// Use this for initialization
	protected override void OnEnable () 
	{
		bomb.gameObject.SetActive(false);
		bombScript = bomb.GetComponent<MovableBomb> ();

		StartCoroutine (Setup ());

		StopCoroutine (WaitForBeginning ());
		StartCoroutine (WaitForBeginning ());
	}

	protected override IEnumerator WaitForBeginning ()
	{
		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		GlobalVariables.Instance.AllMovables.Remove (bomb);

		livesCount = new int[GlobalVariables.Instance.NumberOfAlivePlayers];

		for (int i = 0; i < livesCount.Length; i++)
			livesCount [i] = GlobalVariables.Instance.LivesCount;

		if(GlobalVariables.Instance.AllMovables.Count > 0)
			GlobalMethods.Instance.RandomPositionMovablesVoid (GlobalVariables.Instance.AllMovables.ToArray (), durationBetweenSpawn);
	}

	protected IEnumerator Setup ()
	{
		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		bombScript = bomb.GetComponent<MovableBomb> ();
		timerText.fontSize = 0;
		timerText.transform.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(90, 0, 0));

		timerText.transform.parent.SetParent (GameObject.FindGameObjectWithTag("MovableParent").transform);

		timer = timerLimits.x;
		currentTimer = timerLimits.x;

		string seconds = Mathf.Floor(timer % 60).ToString("00");
		timerClock = seconds;

		timerText.text = timerClock;

		StartCoroutine (SpawnBomb ());

		yield return new WaitWhile (() => bombScript.playerHolding == null);

		StartCoroutine (Timer ());
	}

	// Update is called once per frame
	protected void Update () 
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing && bomb.activeSelf == true && !lastSeconds && timer < 4)
		{
			lastSeconds = true;
			MasterAudio.PlaySound3DAtTransformAndForget (SoundsManager.Instance.lastSecondsSound, bomb.transform);
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

			MasterAudio.PlaySound3DAtTransformAndForget (SoundsManager.Instance.cubeTrackingSound, bomb.transform);
			bombScript.StartCoroutine ("Explode");

			yield return new WaitWhile (()=> bomb.activeSelf == true);

			if (gameEndLoopRunning)
				yield break;

			currentTimer -= eachBombReducedTime;

			if(currentTimer < timerLimits.y)
				currentTimer = timerLimits.y;

			timer = currentTimer;

			StartCoroutine (SpawnBomb ());

			yield return new WaitWhile (() => bombScript.playerHolding == null);

			StartCoroutine (Timer ());

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
		bombScript.ResetColor ();

		yield return new WaitForSeconds (timeBeforeSpawn);

		bomb.tag = "Movable";

		bomb.transform.rotation = Quaternion.Euler (Vector3.zero);

		timerText.transform.parent.SetParent (GameObject.FindGameObjectWithTag("MovableParent").transform);
		timerText.fontSize = 0;

		Vector3 bombPosition = GlobalVariables.Instance.currentModePosition;
		bombPosition.y = 2;

		if(!Physics.CheckSphere(bombPosition, 5f, GlobalMethods.Instance.gameplayLayer))
			GlobalMethods.Instance.SpawnExistingMovableVoid (bomb, bombPosition);
		else
			GlobalMethods.Instance.SpawnExistingMovableRandom (new Vector2(0, 0), new Vector2 (-8, 8), bomb);

		yield return new WaitWhile (()=> bomb.activeSelf == false);

		yield return new WaitForSeconds (0.5f);

		if (gameEndLoopRunning)
			yield break;

		timerText.transform.parent.SetParent (bomb.transform);
		timerText.transform.parent.transform.localPosition = textLocalPosition;
		timerText.transform.parent.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
		timerText.transform.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

		DOTween.To(()=> timerText.fontSize, x=> timerText.fontSize =x, textInitialSize, 0.2f);

		yield return new WaitForSeconds (1f);

		if (gameEndLoopRunning)
			yield break;

		if(bomb.GetComponent<MovableBomb>().playerHolding == null && bomb.GetComponent<MovableScript>().hold == false)
		{
			if(bomb.GetComponent<MovableScript>().attracedBy.Count == 0)
				GlobalVariables.Instance.AlivePlayersList [Random.Range (0, GlobalVariables.Instance.AlivePlayersList.Count)].GetComponent<PlayersGameplay> ().OnHoldMovable (bomb);

			else if(bomb.GetComponent<MovableScript>().attracedBy.Count > 0)
				bomb.GetComponent<MovableScript>().attracedBy[0].GetComponent<PlayersGameplay> ().OnHoldMovable (bomb);			
		}
	}

	protected override IEnumerator GameEnd ()
	{
		StopCoroutine (Timer ());

		return base.GameEnd ();
	}
}
