using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RepulseModeManager : MonoBehaviour 
{
	[Header ("Timer")]
	public int timerDuration = 300;
	public float timer;
	public string timerClock;

	[Header ("Zones")]
	public RepulseZones[] zones = new RepulseZones[6];
	public Text[] zonesScore = new Text[6];
	public List<ZoneAndIntType> mostMovablesZones = new List<ZoneAndIntType> ();

	[Header ("Debug")]
	public int[] numberDebug = new int[4];
	public RepulseTriggerZones[] zonesDebug = new RepulseTriggerZones[4];
	public GameObject[] playersDebug = new GameObject[4];

	[Header ("Players Settings")]
	public Transform[] playersPos = new Transform[6];
	public List<GameObject> players = new List<GameObject>();

	void Start ()
	{
		SetupArrayList ();

		FindPlayersPosition ();

		GlobalMethods.Instance.StartCoroutine ("RandomPositionMovables", false);

		SetMovablesColor ();


		timer = timerDuration;

		StartCoroutine (StartTimer ());
	}

	void Update ()
	{
		if(!GlobalVariables.Instance.GamePaused && !GlobalVariables.Instance.GameOver)
		{
			GetMovablesNumbers ();

			SortLists ();

			for(int i = 0; i < mostMovablesZones.Count; i++)
			{
				numberDebug [i] = mostMovablesZones [i].movableInZone;
				zonesDebug [i] = mostMovablesZones [i].zone;
				playersDebug [i] = mostMovablesZones [i].zonesPlayer;
			}

			for(int i = 0; i < zonesScore.Length; i++)
			{
				zonesScore [i].text = zones [i].movablesNumber.ToString ();
			}
		}
	}


	IEnumerator StartTimer ()
	{
		while(GlobalVariables.Instance.GameOver == true || GlobalVariables.Instance.GamePaused == true)
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

		while(GlobalVariables.Instance.GameOver == true || GlobalVariables.Instance.GamePaused == true)
		{
			yield return null;
		}

		if(timer > 0.01f)
			StartCoroutine (Timer ());

		else
		{
			GameEnded ();
			transform.GetChild (0).GetChild (0).GetComponent<Text> ().text = "00:00";
		}

	}

	void GameEnded ()
	{
		GlobalVariables.Instance.GameOver = true;
		GlobalVariables.Instance.GamePaused = true;

		Debug.Log ("The winner is " + mostMovablesZones [0].zonesPlayer.name + " with " + mostMovablesZones [0].movableInZone + " points");
	}


	void SetupArrayList ()
	{
		GameObject[] playersTemp = GameObject.FindGameObjectsWithTag ("Player");
		players.Clear ();

		for(int i = 0; i < playersTemp.Length; i++)
		{
			if (playersTemp [i].GetComponent<PlayersGameplay> ().controllerNumber != -1)
				players.Add (playersTemp [i]);
		}

		for (int i = 0; i < players.Count; i++)
			mostMovablesZones.Add (new ZoneAndIntType ());

		for (int i = 0; i < mostMovablesZones.Count; i++)
			mostMovablesZones [i].zone = RepulseTriggerZones.None;

		numberDebug = new int[players.Count];
		zonesDebug = new RepulseTriggerZones[players.Count];
		playersDebug = new GameObject[players.Count];
	}

	void FindPlayersPosition ()
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
			player.GetComponent<PlayerRepulse> ().playerZone = RepulseTriggerZones.Zone1;
			for(int i = 0; i < mostMovablesZones.Count; i++)
			{
				if(mostMovablesZones [i].zone == RepulseTriggerZones.None)
				{
					mostMovablesZones [i].zone = RepulseTriggerZones.Zone1;
					mostMovablesZones [i].zonesPlayer = player;
				}
			}
			break;
		case 1:
			player.GetComponent<PlayerRepulse> ().playerZone = RepulseTriggerZones.Zone2;
			for(int i = 0; i < mostMovablesZones.Count; i++)
			{
				if(mostMovablesZones [i].zone == RepulseTriggerZones.None)
				{
					mostMovablesZones [i].zone = RepulseTriggerZones.Zone1;
					mostMovablesZones [i].zonesPlayer = player;
				}
			}
			break;
		case 2:
			player.GetComponent<PlayerRepulse> ().playerZone = RepulseTriggerZones.Zone1;
			for(int i = 0; i < mostMovablesZones.Count; i++)
			{
				if(mostMovablesZones [i].zone == RepulseTriggerZones.None)
				{
					mostMovablesZones [i].zone = RepulseTriggerZones.Zone1;
					mostMovablesZones [i].zonesPlayer = player;
				}
			}
			break;
		case 3:
			player.GetComponent<PlayerRepulse> ().playerZone = RepulseTriggerZones.Zone2;
			for(int i = 0; i < mostMovablesZones.Count; i++)
			{
				if(mostMovablesZones [i].zone == RepulseTriggerZones.None)
				{
					mostMovablesZones [i].zone = RepulseTriggerZones.Zone1;
					mostMovablesZones [i].zonesPlayer = player;
				}
			}
			break;
		case 4:
			player.GetComponent<PlayerRepulse> ().playerZone = RepulseTriggerZones.Zone3;
			for(int i = 0; i < mostMovablesZones.Count; i++)
			{
				if(mostMovablesZones [i].zone == RepulseTriggerZones.None)
				{
					mostMovablesZones [i].zone = RepulseTriggerZones.Zone1;
					mostMovablesZones [i].zonesPlayer = player;
				}
			}
			break;
		case 5:
			player.GetComponent<PlayerRepulse> ().playerZone = RepulseTriggerZones.Zone4;
			for(int i = 0; i < mostMovablesZones.Count; i++)
			{
				if(mostMovablesZones [i].zone == RepulseTriggerZones.None)
				{
					mostMovablesZones [i].zone = RepulseTriggerZones.Zone1;
					mostMovablesZones [i].zonesPlayer = player;
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
		GameObject[] allMovables = GameObject.FindGameObjectsWithTag ("Movable");

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
		for(int i = 0; i < mostMovablesZones.Count; i++)
		{
			switch(mostMovablesZones[i].zone)
			{
			case RepulseTriggerZones.Zone1:
				if(players.Count == 2)
					mostMovablesZones [i].movableInZone = zones [0].movablesNumber;
				else
					mostMovablesZones [i].movableInZone = zones [2].movablesNumber;
				break;
			case RepulseTriggerZones.Zone2:
				if(players.Count == 2)
					mostMovablesZones [i].movableInZone = zones [1].movablesNumber;
				else
					mostMovablesZones [i].movableInZone = zones [3].movablesNumber;
				break;
			case RepulseTriggerZones.Zone3:
				mostMovablesZones [i].movableInZone = zones [4].movablesNumber;
				break;
			case RepulseTriggerZones.Zone4:
				mostMovablesZones [i].movableInZone = zones [5].movablesNumber;
				break;
			}
		}
	}

	void SortLists ()
	{
		mostMovablesZones.Sort(delegate(ZoneAndIntType x, ZoneAndIntType y) 
		{
				return -1*(x.movableInZone).CompareTo(y.movableInZone);
		});
	}
}

public class ZoneAndIntType
{
	public int movableInZone;
	public RepulseTriggerZones zone;
	public GameObject zonesPlayer;
}
