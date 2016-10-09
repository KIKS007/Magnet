using UnityEngine;
using System.Collections;
using DG.Tweening;
using DarkTonic.MasterAudio;

public class GlobalMethods : Singleton<GlobalMethods> 
{
	public void SpawnExistingPlayerRandomVoid (GameObject player, float timeBeforeSpawn = 0)
	{
		StartCoroutine (SpawnExistingPlayerRandom (player, timeBeforeSpawn));
	}

	IEnumerator SpawnExistingPlayerRandom (GameObject player, float timeBeforeSpawn = 0)
	{
		LayerMask layer = (1 << 9) | (1 << 12) | (1 << 13) | (1 << 14);
		Vector3 newPos = new Vector3();

		player.SetActive (false);

		yield return new WaitForSeconds (timeBeforeSpawn);

		do
		{
			newPos = new Vector3 (Random.Range (-20f, 20f), player.transform.position.y, Random.Range (-10f, 10f));
			yield return null;	
		}
		while(Physics.CheckSphere(newPos, 3, layer));

		player.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		player.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;


		player.transform.position = newPos;
		SpawnParticles (player);

		player.SetActive (true);
	}

	void SpawnParticles (GameObject player)
	{
		GameObject instantiatedParticles = Instantiate(GlobalVariables.Instance.PlayerSpawnParticles, player.transform.position, GlobalVariables.Instance.PlayerSpawnParticles.transform.rotation) as GameObject;
	
		instantiatedParticles.transform.SetParent (GlobalVariables.Instance.ParticulesClonesParent);
		instantiatedParticles.GetComponent<ParticleSystemRenderer>().material.color = player.gameObject.GetComponent<Renderer>().material.color;
	}

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

		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		for(int i = 0; i < allMovables.Length; i++)
		{
			Vector3 newPos = new Vector3 ();

			do
			{
				newPos = new Vector3(Random.Range(-20f, 20f), 3, Random.Range(-10f, 10f));
			}
			while(Physics.CheckSphere(newPos, 3, layer));

			yield return new WaitForSeconds (durationBetweenSpawn);

			if(allMovables[i] != null)
			{
				allMovables [i].gameObject.SetActive (true);

				allMovables [i].transform.DOScale (allScales [i], 0.8f).SetEase (Ease.OutElastic);

				allMovables [i].transform.position = newPos;
				allMovables [i].transform.rotation = Quaternion.Euler (Vector3.zero);
				allMovables [i].GetComponent<Rigidbody> ().velocity = Vector3.zero;
				allMovables [i].GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;

				allMovables [i].transform.GetChild (1).GetComponent<Renderer> ().enabled = true;
				allMovables [i].transform.GetChild (2).GetComponent<Renderer> ().enabled = true;}

			MasterAudio.PlaySound3DAtTransformAndForget (GameSoundsManager.Instance.cubeSpawnSound, allMovables [i].transform);

			yield return null;
		}
	}

	public void SpawnExistingMovableVoid (GameObject movable, Vector3 position)
	{
		StartCoroutine (SpawnExistingMovable (movable, position));
	}

	IEnumerator SpawnExistingMovable (GameObject movable, Vector3 position)
	{
		LayerMask layer = (1 << 9) | (1 << 12) | (1 << 13) | (1 << 14);
		Vector3 movableScale = movable.transform.lossyScale;
		Vector3 newPos = new Vector3 ();

		do
		{
			newPos = position;
			yield return null;
		}
		while (Physics.CheckSphere (position, 3, layer));

		movable.transform.localScale = Vector3.zero;

		movable.gameObject.SetActive(true);

		movable.transform.rotation = Quaternion.Euler(Vector3.zero);
		movable.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		movable.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;

		movable.transform.DOScale (movableScale, 0.8f).SetEase (Ease.OutElastic);

		movable.transform.position = newPos;

		MasterAudio.PlaySound3DAtTransformAndForget (GameSoundsManager.Instance.cubeSpawnSound, movable.transform);

		yield return null;
	}
		
	public void SpawnExistingMovableRandom (GameObject movable)
	{
		LayerMask layer = (1 << 9) | (1 << 12) | (1 << 13) | (1 << 14);
		Vector3 movableScale = movable.transform.lossyScale;
		Vector3 newPos = new Vector3 ();

		movable.transform.localScale = Vector3.zero;

		do
		{
			newPos = new Vector3(Random.Range(-20f, 20f), 3, Random.Range(-10f, 10f));
		}
		while(Physics.CheckSphere(newPos, 3, layer));

		movable.transform.rotation = Quaternion.Euler(Vector3.zero);
		movable.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		movable.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;

		movable.gameObject.SetActive(true);

		movable.transform.DOScale (movableScale, 0.8f).SetEase (Ease.OutElastic);

		movable.transform.position = newPos;

		MasterAudio.PlaySound3DAtTransformAndForget (GameSoundsManager.Instance.cubeSpawnSound, movable.transform);
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
