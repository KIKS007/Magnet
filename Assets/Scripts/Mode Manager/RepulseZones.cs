using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RepulseZones : MonoBehaviour 
{
	public RepulseTriggerZones zone;

	public Color zoneColor;

	public int movablesNumber = 0;

	public List<GameObject> touchedMovables = new List<GameObject>();

	public List<Collider> movablesList = new List<Collider>();

	public LayerMask checkBoxLayer;

	private Vector3 checkBoxScale;


	void Start ()
	{
		StartCoroutine (MovablesForcedCheck ());

		checkBoxScale = new Vector3 (transform.lossyScale.x / 2, transform.lossyScale.y / 2, transform.lossyScale.z / 2);
	}

	void Update ()
	{
		movablesNumber = touchedMovables.Count;
	}

	void OnTriggerStay (Collider other)
	{
		if(other.tag == "Movable")
		{
			StartCoroutine (EnableBooleans (other.gameObject));

			if (!touchedMovables.Contains (other.gameObject))
				touchedMovables.Add (other.gameObject);
		}

		if(other.tag == "Player")
		{
			if(other.GetComponent<PlayersGameplay> ().playerState == PlayerState.Holding)
			{
				StartCoroutine (EnableBooleans (other.GetComponent<PlayersGameplay> ().holdMovableTransform.gameObject));

				if (!touchedMovables.Contains (other.GetComponent<PlayersGameplay> ().holdMovableTransform.gameObject))
					touchedMovables.Add (other.GetComponent<PlayersGameplay> ().holdMovableTransform.gameObject);
			}
		}
	}

	void OnTriggerExit (Collider other)
	{
		if(other.tag == "Movable")
		{
			StartCoroutine (DisableBooleans (other.gameObject));

			touchedMovables.Remove (other.gameObject);
		}

		if(other.tag == "Player")
		{
			if(other.GetComponent<PlayersGameplay> ().playerState == PlayerState.Holding)
			{
				StartCoroutine (DisableBooleans (other.GetComponent<PlayersGameplay> ().holdMovableTransform.gameObject));

				touchedMovables.Remove (other.GetComponent<PlayersGameplay> ().holdMovableTransform.gameObject);
			}

		}
	}

	IEnumerator EnableBooleans (GameObject other)
	{
		switch(zone)
		{
		case RepulseTriggerZones.Zone1:
			other.GetComponent<MovableRepulse> ().inZone1 = true;
			break;
		case RepulseTriggerZones.Zone2:
			other.GetComponent<MovableRepulse> ().inZone2 = true;
			break;
		case RepulseTriggerZones.Zone3:
			other.GetComponent<MovableRepulse> ().inZone3 = true;
			break;
		case RepulseTriggerZones.Zone4:
			other.GetComponent<MovableRepulse> ().inZone4 = true;
			break;
		case RepulseTriggerZones.None:
			break;
		}

		yield return null;
	}

	IEnumerator DisableBooleans (GameObject other)
	{
		switch(zone)
		{
		case RepulseTriggerZones.Zone1:
			other.GetComponent<MovableRepulse> ().inZone1 = false;
			break;
		case RepulseTriggerZones.Zone2:
			other.GetComponent<MovableRepulse> ().inZone2 = false;
			break;
		case RepulseTriggerZones.Zone3:
			other.GetComponent<MovableRepulse> ().inZone3 = false;
			break;
		case RepulseTriggerZones.Zone4:
			other.GetComponent<MovableRepulse> ().inZone4 = false;
			break;
		case RepulseTriggerZones.None:
			break;
		}

		yield return null;
	}

	IEnumerator MovablesForcedCheck ()
	{
		Collider[] movablesCollider = Physics.OverlapBox (transform.position, checkBoxScale, transform.rotation, checkBoxLayer);

		movablesList.Clear ();
		movablesList = movablesCollider.ToList ();

		for(int i = 0; i < movablesList.Count; i++)
		{
			if (movablesList [i].tag == "Player")
				movablesList.Remove (movablesList [i]);
		}


		for(int i = 0; i < touchedMovables.Count; i++)
		{
			if(!movablesList.Contains(touchedMovables[i].GetComponent<Collider>()))
			{
				StartCoroutine (DisableBooleans (touchedMovables[i]));
			
				touchedMovables.Remove (touchedMovables [i]);
			}
		}

		yield return new WaitForSeconds (0.1f);

		StartCoroutine (MovablesForcedCheck ());
	}
}
