using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovablesArea : MonoBehaviour 
{
	public List<GameObject> movablesIn = new List<GameObject>();

	void OnTriggerExit (Collider other)
	{
		if(other.tag == "Movable" || other.tag == "ThrownMovable")
		{
			movablesIn.Remove (other.gameObject);

			StartCoroutine (WaitBeforeDestroy (other.gameObject));
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if(other.tag == "Movable" || other.tag == "ThrownMovable")
		{
			movablesIn.Add (other.gameObject);
		}
	}

	IEnumerator WaitBeforeDestroy (GameObject movable)
	{
		yield return new WaitForSeconds (0.1f);

		if(!movablesIn.Contains(movable))
		{
			GlobalMethods.Instance.SpawnExistingMovableRandom (movable);
		}
	}
}
