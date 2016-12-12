using UnityEngine;
using System.Collections;
using DG.Tweening;
using DarkTonic.MasterAudio;

public class GlobalMethods : Singleton<GlobalMethods> 
{
	void Start ()
	{
		GlobalVariables.Instance.OnEndMode += () => StopAllCoroutines ();
		GlobalVariables.Instance.OnMenu += () => StopAllCoroutines ();
	}

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
		while(Physics.CheckSphere(newPos, 5, layer));

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

	public void SpawnPlayerDeadCubeVoid (PlayerName playerName, int controllerNumber)
	{
		StartCoroutine (SpawnPlayerDeadCube (playerName, controllerNumber));
	}

	IEnumerator SpawnPlayerDeadCube (PlayerName playerName, int controllerNumber)
	{
		LayerMask layer = (1 << 9) | (1 << 12) | (1 << 13) | (1 << 14);
		Vector3 newPos = new Vector3();
		int randomCube = Random.Range (0, GlobalVariables.Instance.deadCubesPrefabs.Length);

		if (GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			yield return new WaitForSeconds (1f);
			
			do
			{
				newPos = new Vector3 (Random.Range (-20f, 20f), 3, Random.Range (-10f, 10f));
				yield return null;	
			}
			while(Physics.CheckSphere(newPos, 5, layer));			
			
			GameObject deadCube = Instantiate (GlobalVariables.Instance.deadCubesPrefabs [randomCube], newPos, GlobalVariables.Instance.deadCubesPrefabs [randomCube].transform.rotation, GameObject.FindGameObjectWithTag("MovableParent").transform) as GameObject;
			
			deadCube.GetComponent<PlayersDeadCube> ().controllerNumber = controllerNumber;
			deadCube.GetComponent<PlayersDeadCube> ().playerName = playerName;
			
			Vector3 scale = deadCube.transform.lossyScale;
			deadCube.transform.localScale = Vector3.zero;
			
			string tagTemp = deadCube.tag;
			deadCube.tag = "Untagged";
			
			deadCube.transform.DOScale (scale, 0.8f).SetEase (Ease.OutElastic);
			StartCoroutine (ChangeMovableTag (deadCube, tagTemp, 0.8f));


			GameObject instantiatedParticles = Instantiate(GlobalVariables.Instance.PlayerSpawnParticles, deadCube.transform.position, GlobalVariables.Instance.PlayerSpawnParticles.transform.rotation) as GameObject;

			instantiatedParticles.transform.SetParent (GlobalVariables.Instance.ParticulesClonesParent);
			instantiatedParticles.GetComponent<ParticleSystemRenderer> ().material.color = GlobalVariables.Instance.Players [(int)playerName].gameObject.GetComponent<Renderer> ().material.color;

			GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<DynamicCamera> ().otherTargetsList.Add (deadCube);

			MasterAudio.PlaySound3DAtTransformAndForget (GameSoundsManager.Instance.cubeSpawnSound, deadCube.transform);			
		}
	}

	public void RandomPositionMovablesVoid (GameObject[] allMovables = null, float durationBetweenSpawn = 0.1f)
	{
		StartCoroutine (RandomPositionMovables (allMovables, durationBetweenSpawn));
	}

	public IEnumerator RandomPositionMovables (GameObject[] allMovables = null, float durationBetweenSpawn = 0.1f)
	{
		Vector3[] allScales = new Vector3[allMovables.Length];
		LayerMask layer = (1 << 9) | (1 << 12) | (1 << 13) | (1 << 14);
		string tagTemp = allMovables [0].tag;

		for(int i = 0; i < allMovables.Length; i++)
		{
			allMovables [i].SetActive (false);
			allScales [i] = allMovables [i].transform.lossyScale;
			allMovables [i].transform.localScale = new Vector3 (0, 0, 0);
		}

		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		for(int i = 0; i < allMovables.Length; i++)
		{
			allMovables [i].tag = "Untagged";
			Vector3 newPos = new Vector3 ();

			do
			{
				newPos = new Vector3(Random.Range(-20f, 20f), 3, Random.Range(-10f, 10f));
			}
			while(Physics.CheckSphere(newPos, 5, layer));

			yield return new WaitForSeconds (durationBetweenSpawn);

			if(allMovables[i] != null)
			{
				allMovables [i].gameObject.SetActive (true);

				allMovables [i].transform.DOScale (allScales [i], 0.8f).SetEase (Ease.OutElastic);
				StartCoroutine (ChangeMovableTag (allMovables [i], tagTemp, 0.8f));

				allMovables [i].transform.position = newPos;
				allMovables [i].transform.rotation = Quaternion.Euler (Vector3.zero);
				allMovables [i].GetComponent<Rigidbody> ().velocity = Vector3.zero;
				allMovables [i].GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
			
				MasterAudio.PlaySound3DAtTransformAndForget (GameSoundsManager.Instance.cubeSpawnSound, allMovables [i].transform);
			}


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
		movable.gameObject.SetActive(false);
		string tagTemp = movable.tag;
		movable.tag = "Untagged";

		yield return new WaitWhile (()=> Physics.CheckSphere (position, 5, layer));

		movable.transform.localScale = Vector3.zero;

		movable.transform.rotation = Quaternion.Euler(Vector3.zero);
		movable.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		movable.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;

		movable.gameObject.SetActive(true);
		movable.transform.DOScale (movableScale, 0.8f).SetEase (Ease.OutElastic);
		StartCoroutine (ChangeMovableTag (movable, tagTemp, 0.8f));

		movable.transform.position = position;

		MasterAudio.PlaySound3DAtTransformAndForget (GameSoundsManager.Instance.cubeSpawnSound, movable.transform);

		yield return null;
	}
		
	public void SpawnExistingMovableRandom (GameObject movable)
	{
		LayerMask layer = (1 << 9) | (1 << 12) | (1 << 13) | (1 << 14);
		Vector3 movableScale = movable.transform.lossyScale;
		Vector3 newPos = new Vector3 ();
		string tagTemp = movable.tag;
		movable.tag = "Untagged";

		movable.transform.localScale = Vector3.zero;

		do
		{
			newPos = new Vector3(Random.Range(-20f, 20f), 3, Random.Range(-10f, 10f));
		}
		while(Physics.CheckSphere(newPos, 5, layer));

		movable.transform.rotation = Quaternion.Euler(Vector3.zero);
		movable.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		movable.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;

		movable.gameObject.SetActive(true);

		movable.transform.DOScale (movableScale, 0.8f).SetEase (Ease.OutElastic);
		StartCoroutine (ChangeMovableTag (movable, tagTemp, 0.8f));
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

	IEnumerator ChangeMovableTag (GameObject movable, string tagTemp, float timeTween)
	{
		yield return new WaitForSeconds (0.3f * timeTween);

		if(movable != null)
			movable.tag = tagTemp;
	}
}
