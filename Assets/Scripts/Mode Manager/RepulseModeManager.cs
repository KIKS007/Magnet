using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RepulseModeManager : MonoBehaviour 
{
	public float timeBeforeEndGame = 2;

	[Header ("Timer")]
	public int timerDuration = 300;
	public float timer;
	public string timerClock;

	[Header ("Zones")]
	public RepulseZones[] zones = new RepulseZones[6];
	public Text[] zonesScore = new Text[6];
	public List<ZoneAndIntType> leastMovablesZones = new List<ZoneAndIntType> ();

	[Header ("Players Settings")]
	public Transform[] playersPos = new Transform[6];
	public List<GameObject> players = new List<GameObject>();

	private GameObject[] allMovables = new GameObject[0];

	void Start ()
	{
		allMovables = GameObject.FindGameObjectsWithTag ("Movable");

		for (int i = 0; i < allMovables.Length; i++)
			allMovables [i].GetComponent<Renderer> ().enabled = false;

		for (int i = 0; i < zones.Length; i++)
		{
			zones [i].gameObject.SetActive (false);
			zonesScore [i].gameObject.SetActive (false);
		}

		StartCoroutine (WaitForBeginning ());
	}

	IEnumerator WaitForBeginning ()
	{
		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		SetupArrayList ();

		SetupPlayersAndZones ();

		GlobalMethods.Instance.StartCoroutine ("RandomPositionMovables", 0.1f);

		SetMovablesColor ();

		timer = timerDuration;
		StartCoroutine (StartTimer ());
	}

	void Update ()
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			GetMovablesNumbers ();

			SortLists ();

			for(int i = 0; i < zonesScore.Length; i++)
			{
				zonesScore [i].text = zones [i].movablesNumber.ToString ();
			}
		}
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

		else if(GlobalVariables.Instance.GameState != GameStateEnum.Over)
		{
			StartCoroutine (GameEnded ());
			transform.GetChild (0).GetChild (0).GetComponent<Text> ().text = "00:00";
		}

	}

	IEnumerator GameEnded ()
	{
		switch (leastMovablesZones[0].zonesPlayer.name)
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

		Debug.Log ("The winner is " + leastMovablesZones [0].zonesPlayer.name + " with " + leastMovablesZones [0].movableInZone + " points");
	}


	void SetupArrayList ()
	{
		players.Clear ();

		players = GlobalVariables.Instance.EnabledPlayersList;

		for (int i = 0; i < players.Count; i++)
		{
			leastMovablesZones.Add (new ZoneAndIntType ());
		}

		for (int i = 0; i < leastMovablesZones.Count; i++)
		{
			leastMovablesZones [i].zone = RepulseTriggerZones.None;
		}
	}

	void SetupPlayersAndZones ()
	{
		for (int i = 0; i < zones.Length; i++)
		{
			zones [i].gameObject.SetActive (false);
			zonesScore [i].gameObject.SetActive (false);
		}
			
		if (GlobalVariables.Instance.NumberOfPlayers == 2)
		{
			int randomInt = Random.Range(0, 1 + 1);

			players [0].transform.position = playersPos [randomInt].position;

			SetPlayerZone (randomInt, players [0]);
			EnableZonesAndScore (randomInt, players [0]);

			if(randomInt == 0)
			{
				players [1].transform.position = playersPos [1].position;
				SetPlayerZone (1, players [1]);
				EnableZonesAndScore (1, players [1]);
			}

			else
			{
				players [1].transform.position = playersPos [0].position;
				SetPlayerZone (0, players [1]);
				EnableZonesAndScore (0, players [1]);
			}

			for (int i = 0; i < GlobalVariables.Instance.EnabledPlayersList.Count; i++)
				GlobalVariables.Instance.EnabledPlayersList [i].transform.LookAt (new Vector3 (0, 0, 0));
		}

		else if(GlobalVariables.Instance.NumberOfPlayers > 2)
		{
			for(int i = 0; i < players.Count; i++)
			{
				int randomInt;
				LayerMask layer = (1 << 12);

				do
				{
					randomInt = Random.Range(2, playersPos.Length);
				}
				while (Physics.CheckSphere(playersPos[randomInt].position, 3, layer));

				players [i].transform.position = playersPos [randomInt].position;

				SetPlayerZone (randomInt, players [i]);
				EnableZonesAndScore (randomInt, players [i]);
			}
		}
	}

	void SetPlayerZone (int number, GameObject player)
	{
		switch(number)
		{
		case 0:
			for(int i = 0; i < leastMovablesZones.Count; i++)
			{
				if(leastMovablesZones [i].zone == RepulseTriggerZones.None)
				{
					player.GetComponent<PlayerRepulse> ().playerZone = RepulseTriggerZones.Zone1;
					leastMovablesZones [i].zone = RepulseTriggerZones.Zone1;
					leastMovablesZones [i].zonesPlayer = player;
					break;
				}
			}
			break;
		case 1:
			for(int i = 0; i < leastMovablesZones.Count; i++)
			{
				if(leastMovablesZones [i].zone == RepulseTriggerZones.None)
				{
				
					player.GetComponent<PlayerRepulse> ().playerZone = RepulseTriggerZones.Zone2;
					leastMovablesZones [i].zone = RepulseTriggerZones.Zone2;
					leastMovablesZones [i].zonesPlayer = player;
					break;
				}
			}
			break;
		case 2:
			for(int i = 0; i < leastMovablesZones.Count; i++)
			{
				if(leastMovablesZones [i].zone == RepulseTriggerZones.None)
				{

					player.GetComponent<PlayerRepulse> ().playerZone = RepulseTriggerZones.Zone1;
					leastMovablesZones [i].zone = RepulseTriggerZones.Zone1;
					leastMovablesZones [i].zonesPlayer = player;
					break;
				}
			}
			break;
		case 3:
			for(int i = 0; i < leastMovablesZones.Count; i++)
			{
				if(leastMovablesZones [i].zone == RepulseTriggerZones.None)
				{

					player.GetComponent<PlayerRepulse> ().playerZone = RepulseTriggerZones.Zone2;
					leastMovablesZones [i].zone = RepulseTriggerZones.Zone2;
					leastMovablesZones [i].zonesPlayer = player;
					break;
				}
			}
			break;
		case 4:
			for(int i = 0; i < leastMovablesZones.Count; i++)
			{
				if(leastMovablesZones [i].zone == RepulseTriggerZones.None)
				{

					player.GetComponent<PlayerRepulse> ().playerZone = RepulseTriggerZones.Zone3;
					leastMovablesZones [i].zone = RepulseTriggerZones.Zone3;
					leastMovablesZones [i].zonesPlayer = player;
					break;
				}
			}
			break;
		case 5:
			for(int i = 0; i < leastMovablesZones.Count; i++)
			{
				if(leastMovablesZones [i].zone == RepulseTriggerZones.None)
				{

					player.GetComponent<PlayerRepulse> ().playerZone = RepulseTriggerZones.Zone4;
					leastMovablesZones [i].zone = RepulseTriggerZones.Zone4;
					leastMovablesZones [i].zonesPlayer = player;
					break;
				}
			}
			break;
		}

	}

	void EnableZonesAndScore (int zoneNumber, GameObject player)
	{
		switch(zoneNumber)
		{
		case 0:
			zones [0].zoneColor = player.GetComponent<Renderer> ().material.color;
			zones [0].gameObject.SetActive (true);
			zonesScore [0].gameObject.SetActive (true);
			zonesScore [0].color = player.GetComponent<Renderer> ().material.color;
			break;
		case 1:
			zones [1].zoneColor = player.GetComponent<Renderer> ().material.color;
			zones [1].gameObject.SetActive (true);
			zonesScore [1].gameObject.SetActive (true);
			zonesScore [1].color = player.GetComponent<Renderer> ().material.color;
			break;
		case 2:
			zones [2].zoneColor = player.GetComponent<Renderer> ().material.color;
			zones [2].gameObject.SetActive (true);
			zonesScore [2].gameObject.SetActive (true);
			zonesScore [2].color = player.GetComponent<Renderer> ().material.color;
			break;
		case 3:
			zones [3].zoneColor = player.GetComponent<Renderer> ().material.color;
			zones [3].gameObject.SetActive (true);
			zonesScore [3].gameObject.SetActive (true);
			zonesScore [3].color = player.GetComponent<Renderer> ().material.color;
			break;
		case 4:
			zones [4].zoneColor = player.GetComponent<Renderer> ().material.color;
			zones [4].gameObject.SetActive (true);
			zonesScore [4].gameObject.SetActive (true);
			zonesScore [4].color = player.GetComponent<Renderer> ().material.color;
			break;
		case 5:
			zones [5].zoneColor = player.GetComponent<Renderer> ().material.color;
			zones [5].gameObject.SetActive (true);
			zonesScore [5].gameObject.SetActive (true);
			zonesScore [5].color = player.GetComponent<Renderer> ().material.color;
			break;
		}
	}

	void SetMovablesColor ()
	{
		for(int i = 0; i < allMovables.Length; i++)
		{
			if(zones [0].gameObject.activeSelf == true)
				allMovables [i].GetComponent<MovableRepulse> ().zonesColors [0] = zones [0].zoneColor;

			if(zones [1].gameObject.activeSelf == true)
				allMovables [i].GetComponent<MovableRepulse> ().zonesColors [1] = zones [1].zoneColor;

			if(zones [2].gameObject.activeSelf == true)
				allMovables [i].GetComponent<MovableRepulse> ().zonesColors [0] = zones [2].zoneColor;

			if(zones [3].gameObject.activeSelf == true)
				allMovables [i].GetComponent<MovableRepulse> ().zonesColors [1] = zones [3].zoneColor;

			if(zones [4].gameObject.activeSelf == true)
				allMovables [i].GetComponent<MovableRepulse> ().zonesColors [2] = zones [4].zoneColor;

			if(zones [5].gameObject.activeSelf == true)
				allMovables [i].GetComponent<MovableRepulse> ().zonesColors [3] = zones [5].zoneColor;
		}
	}

	void GetMovablesNumbers ()
	{
		for(int i = 0; i < leastMovablesZones.Count; i++)
		{
			switch(leastMovablesZones[i].zone)
			{
			case RepulseTriggerZones.Zone1:
				if(players.Count == 2)
					leastMovablesZones [i].movableInZone = zones [0].movablesNumber;
				else
					leastMovablesZones [i].movableInZone = zones [2].movablesNumber;
				break;
			case RepulseTriggerZones.Zone2:
				if(players.Count == 2)
					leastMovablesZones [i].movableInZone = zones [1].movablesNumber;
				else
					leastMovablesZones [i].movableInZone = zones [3].movablesNumber;
				break;
			case RepulseTriggerZones.Zone3:
				leastMovablesZones [i].movableInZone = zones [4].movablesNumber;
				break;
			case RepulseTriggerZones.Zone4:
				leastMovablesZones [i].movableInZone = zones [5].movablesNumber;
				break;
			}
		}
	}

	void SortLists ()
	{
		leastMovablesZones.Sort(delegate(ZoneAndIntType x, ZoneAndIntType y) 
		{
				return 1*(x.movableInZone).CompareTo(y.movableInZone);
		});
	}
}

[System.Serializable]
public class ZoneAndIntType
{
	public int movableInZone;
	public RepulseTriggerZones zone;
	public GameObject zonesPlayer;
}
