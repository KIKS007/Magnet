using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RepulseModeManager : MonoBehaviour 
{
	[Header ("Zones")]
	public RepulseZones[] zones = new RepulseZones[4];
	public List<ZoneAndIntType> mostMovablesZones = new List<ZoneAndIntType> ();

	public int[] numberDebug = new int[4];
	public RepulseTriggerZones[] zonesDebug = new RepulseTriggerZones[4];


	[Header ("Players Settings")]
	public GameObject[] players = new GameObject[0];
	public Transform[] playersPos = new Transform[6];


	void Start ()
	{
		players = GameObject.FindGameObjectsWithTag ("Player");

		FindPlayersPosition ();

		GlobalMethods.Instance.StartCoroutine ("RandomPositionMovables", false);

		SetMovablesColor ();

		for(int i = 0; i < 4; i++)
			mostMovablesZones.Add (new ZoneAndIntType());

		mostMovablesZones [0].zone = RepulseTriggerZones.Zone1;
		mostMovablesZones [1].zone = RepulseTriggerZones.Zone2;
		mostMovablesZones [2].zone = RepulseTriggerZones.Zone3;
		mostMovablesZones [3].zone = RepulseTriggerZones.Zone4;
	}

	void Update ()
	{
		if(!GlobalVariables.Instance.GamePaused && !GlobalVariables.Instance.GameOver)
		{
			GetMovablesNumbers ();

			SortLists ();
		}

		for(int i = 0; i < mostMovablesZones.Count; i++)
		{
			numberDebug [i] = mostMovablesZones [i].movableInZone;
			zonesDebug [i] = mostMovablesZones [i].zone;
		}
	}

	void FindPlayersPosition ()
	{
		if (GlobalVariables.Instance.NumberOfPlayers == 2)
		{
			int randomInt = Random.Range(0, 1 + 1);

			players [0].transform.position = playersPos [randomInt].position;

			SetPlayerZone (randomInt, players [0]);
			SetZoneColor (randomInt, players [0]);

			if(randomInt == 0)
			{
				players [1].transform.position = playersPos [1].position;
				SetPlayerZone (1, players [1]);
				SetZoneColor (1, players [1]);
			}

			else
			{
				players [1].transform.position = playersPos [0].position;
				SetPlayerZone (0, players [1]);
				SetZoneColor (0, players [1]);
			}

		}

		else if(GlobalVariables.Instance.NumberOfPlayers > 2)
		{
			for(int i = 0; i < players.Length; i++)
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
				SetZoneColor (randomInt, players [i]);
			}
		}
	}

	void SetPlayerZone (int number, GameObject player)
	{
		switch(number)
		{
		case 0:
			player.GetComponent<PlayerRepulse> ().playerZone = RepulseTriggerZones.Zone1;
			break;
		case 1:
			player.GetComponent<PlayerRepulse> ().playerZone = RepulseTriggerZones.Zone2;
			break;
		case 2:
			player.GetComponent<PlayerRepulse> ().playerZone = RepulseTriggerZones.Zone1;
			break;
		case 3:
			player.GetComponent<PlayerRepulse> ().playerZone = RepulseTriggerZones.Zone2;
			break;
		case 4:
			player.GetComponent<PlayerRepulse> ().playerZone = RepulseTriggerZones.Zone3;
			break;
		case 5:
			player.GetComponent<PlayerRepulse> ().playerZone = RepulseTriggerZones.Zone4;
			break;
		}
	}

	void SetZoneColor (int zoneNumber, GameObject player)
	{
		switch(zoneNumber)
		{
		case 0:
			zones [0].zoneColor = player.GetComponent<Renderer> ().material.color;
			break;
		case 1:
			zones [1].zoneColor = player.GetComponent<Renderer> ().material.color;
			break;
		case 2:
			zones [0].zoneColor = player.GetComponent<Renderer> ().material.color;
			break;
		case 3:
			zones [1].zoneColor = player.GetComponent<Renderer> ().material.color;
			break;
		case 4:
			zones [2].zoneColor = player.GetComponent<Renderer> ().material.color;
			break;
		case 5:
			zones [3].zoneColor = player.GetComponent<Renderer> ().material.color;
			break;
		}
	}

	void SetMovablesColor ()
	{
		GameObject[] allMovables = GameObject.FindGameObjectsWithTag ("Movable");

		for(int i = 0; i < allMovables.Length; i++)
		{
			allMovables [i].GetComponent<MovableRepulse> ().zonesColors [0] = zones [0].zoneColor;
			allMovables [i].GetComponent<MovableRepulse> ().zonesColors [1] = zones [1].zoneColor;
			allMovables [i].GetComponent<MovableRepulse> ().zonesColors [2] = zones [2].zoneColor;
			allMovables [i].GetComponent<MovableRepulse> ().zonesColors [3] = zones [3].zoneColor;
		}
	}

	void GetMovablesNumbers ()
	{
		for(int i = 0; i < mostMovablesZones.Count; i++)
		{
			switch(mostMovablesZones[i].zone)
			{
			case RepulseTriggerZones.Zone1:
				mostMovablesZones [i].movableInZone = zones [0].movablesNumber;
				break;
			case RepulseTriggerZones.Zone2:
				mostMovablesZones [i].movableInZone = zones [1].movablesNumber;
				break;
			case RepulseTriggerZones.Zone3:
				mostMovablesZones [i].movableInZone = zones [2].movablesNumber;
				break;
			case RepulseTriggerZones.Zone4:
				mostMovablesZones [i].movableInZone = zones [3].movablesNumber;
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
}
