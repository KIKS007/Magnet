using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GlobalMethods : Singleton<GlobalMethods> 
{
	public IEnumerator RandomPositionMovables (float durationBetweenSpawn = 0)
	{
		GameObject[] allMovables = GameObject.FindGameObjectsWithTag ("Movable");
		Vector3[] allScales = new Vector3[allMovables.Length];
		LayerMask layer = (1 << 9) | (1 << 12) | (1 << 13) | (1 << 14);

		for(int i = 0; i < allMovables.Length; i++)
		{
			allScales [i] = allMovables [i].transform.lossyScale;
			allMovables [i].transform.localScale = new Vector3 (0, 0, 0);
		}
			

		for(int i = 0; i < allMovables.Length; i++)
		{
			Vector3 newPos = new Vector3 ();

			do
			{
				newPos = new Vector3(Random.Range(-24, 24 + 1), 3, Random.Range(-14, 14 + 1));
			}
			while(Physics.CheckSphere(newPos, 3, layer));

			yield return new WaitForSeconds (durationBetweenSpawn);

			allMovables [i].gameObject.SetActive(true);

			allMovables [i].transform.DOScale (allScales[i], 0.8f).SetEase (Ease.OutElastic);

			allMovables [i].transform.position = newPos;

			yield return null;
		}
	}

	public void SetPlayersPositions2Team (Transform[] team1Positions, Transform[] team2Positions)
	{
		if (GlobalVariables.Instance.Team1.Count == 1)
			GlobalVariables.Instance.Team1 [0].transform.position = team1Positions [0].position;

		if (GlobalVariables.Instance.Team2.Count == 1)
			GlobalVariables.Instance.Team2 [0].transform.position = team2Positions [0].position;

		if (GlobalVariables.Instance.Team1.Count == 2)
		{
			int randomInt = Random.Range(1, 2 + 1);

			GlobalVariables.Instance.Team1 [0].transform.position = team1Positions [randomInt].position;;

			if(randomInt == 1)
				GlobalVariables.Instance.Team1 [1].transform.position = team1Positions [2].position;
			else
				GlobalVariables.Instance.Team1 [1].transform.position = team1Positions [1].position;
		}

		if (GlobalVariables.Instance.Team2.Count == 2)
		{
			int randomInt = Random.Range(1, 2 + 1);

			GlobalVariables.Instance.Team2 [0].transform.position = team2Positions [randomInt].position;;

			if(randomInt == 1)
				GlobalVariables.Instance.Team2 [1].transform.position = team2Positions [2].position;
			else
				GlobalVariables.Instance.Team2 [1].transform.position = team2Positions [1].position;
		}

		for (int i = 0; i < GlobalVariables.Instance.EnabledPlayersList.Count; i++)
			GlobalVariables.Instance.EnabledPlayersList [i].transform.LookAt (new Vector3 (0, 0, 0));
	}

	public void SpawnExistingMovable (GameObject movable, Vector3 position)
	{
		LayerMask layer = (1 << 9) | (1 << 12) | (1 << 13) | (1 << 14);
		Vector3 movableScale = movable.transform.lossyScale;
		Vector3 newPos = new Vector3 ();

		do
		{
			newPos = position;
		}
		while (Physics.CheckSphere (position, 3, layer));

		movable.transform.localScale = Vector3.zero;

		movable.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		movable.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;

		movable.gameObject.SetActive(true);

		movable.transform.DOScale (movableScale, 0.8f).SetEase (Ease.OutElastic);

		movable.transform.position = newPos;
	}

	public void SpawnExistingMovableRandom (GameObject movable)
	{
		LayerMask layer = (1 << 9) | (1 << 12) | (1 << 13) | (1 << 14);
		Vector3 movableScale = movable.transform.lossyScale;
		Vector3 newPos = new Vector3 ();

		movable.transform.localScale = Vector3.zero;

		do
		{
			newPos = new Vector3(Random.Range(-24, 24 + 1), 3, Random.Range(-14, 14 + 1));
		}
		while(Physics.CheckSphere(newPos, 3, layer));

		movable.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		movable.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;

		movable.gameObject.SetActive(true);

		movable.transform.DOScale (movableScale, 0.8f).SetEase (Ease.OutElastic);

		movable.transform.position = newPos;
	}

	public void Explosion (Vector3 explosionPosition, float explosionForce, float explosionRadius, LayerMask explosionMask)
	{
		foreach(Collider other in Physics.OverlapSphere(explosionPosition, explosionRadius, explosionMask))
		{
			Vector3 repulseDirection = other.transform.position - explosionPosition;
			repulseDirection.Normalize ();

			float explosionImpactZone = 1 - (Vector3.Distance (explosionPosition, other.transform.position) / explosionRadius);

			if(explosionImpactZone > 0)
			{
				if(other.GetComponent<Rigidbody>() != null)
					other.GetComponent<Rigidbody> ().AddForce (repulseDirection * explosionImpactZone * explosionForce, ForceMode.Impulse);
			}
		}
	}

}
