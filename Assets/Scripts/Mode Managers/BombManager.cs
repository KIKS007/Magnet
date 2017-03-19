using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DarkTonic.MasterAudio;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BombManager : LastManManager 
{
	[Header ("Bomb Settings")]
	public GameObject bomb;
	public int playersNumber;
	public int firstBombTimer = 300;
	public int secondBombTimer = 300;
	public int thirdBombTimer = 300;
	public float timeBeforeFirstSpawn = 1;
	public float timeBetweenSpawn = 2;

	[Header ("Timer")]
	public Text timerText;
	public float timer;
	public string timerClock;

	protected bool firstSpawn = true;
	protected bool lastSeconds = false;

	protected MovableBomb bombScript;

	protected int textInitialSize;
	protected Vector3 textLocalPosition;

	// Use this for initialization
	protected override void OnEnable () 
	{
		if (GlobalVariables.Instance.modeObjective != ModeObjective.LastMan)
			return;
		
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

		StopCoroutine (WaitForBeginning ());
		StartCoroutine (WaitForBeginning ());
	}

	protected override IEnumerator WaitForBeginning ()
	{
		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		GlobalVariables.Instance.AllMovables.Remove (bomb);

		if(GlobalVariables.Instance.AllMovables.Count > 0)
			GlobalMethods.Instance.RandomPositionMovablesVoid (GlobalVariables.Instance.AllMovables.ToArray (), durationBetweenSpawn);
	}

	protected virtual IEnumerator Setup ()
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
	protected override void Update () 
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

		Vector3 bombPosition = GlobalVariables.Instance.currentModePosition;
		bombPosition.y = 2;

		GlobalMethods.Instance.SpawnExistingMovableVoid (bomb, bombPosition);

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
				GlobalVariables.Instance.AlivePlayersList [Random.Range (0, GlobalVariables.Instance.AlivePlayersList.Count)].GetComponent<PlayersBomb> ().GetBomb (bomb.GetComponent<Collider>());
			
			else if(bomb.GetComponent<MovableScript>().attracedBy.Count > 0)
				bomb.GetComponent<MovableScript>().attracedBy[0].GetComponent<PlayersBomb> ().GetBomb (bomb.GetComponent<Collider>());			
		}
	}
}
