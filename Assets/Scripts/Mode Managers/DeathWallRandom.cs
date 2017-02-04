using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWallRandom : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		int random = Random.Range (0, transform.childCount);

		for(int i = 0; i < transform.childCount; i++)
			transform.GetChild (i).gameObject.SetActive (false);

		transform.GetChild (random).gameObject.SetActive (true);
	}
}
