using UnityEngine;
using System.Collections;

public class TestDeadCube : MonoBehaviour 
{
	public PlayersGameplay player1;
	public Collider movableTest;

	public bool test = false;
	public float timeBeforeStun = 0;
	
	// Update is called once per frame
	void Update () 
	{
		if(test)
		{
			test = false;
			StartCoroutine (Test ());
		}
	}

	IEnumerator Test ()
	{
		player1.transform.GetChild (2).GetComponent<MagnetTriggerScript> ().GetMovable (movableTest);

		yield return new WaitForSeconds (timeBeforeStun);

		player1.StunVoid (true);
	}
}
