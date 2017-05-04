using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RamManager : MonoBehaviour 
{
	[Header ("Spawn")]
	public float durationBetweenSpawn = 4f;

	[Header ("Cubes")]
	public GameObject[] cubesPrefabs = new GameObject[3];

	// Use this for initialization
	void Start () 
	{
		StartCoroutine (SpawnCube ());
	}

	IEnumerator SpawnCube ()
	{
		yield return new WaitWhile (()=> GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		yield return new WaitForSeconds (durationBetweenSpawn);

		GlobalMethods.Instance.SpawnNewMovableRandomVoid (cubesPrefabs [Random.Range (0, cubesPrefabs.Length)], 0, 0.5f, 5);

		StartCoroutine (SpawnCube ());
	}
}
