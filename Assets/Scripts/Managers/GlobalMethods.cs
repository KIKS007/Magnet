using UnityEngine;
using System.Collections;
using DG.Tweening;
using DarkTonic.MasterAudio;

public class GlobalMethods : Singleton<GlobalMethods> 
{
	public LayerMask gameplayLayer = (1 << 9) | (1 << 12);
	public LayerMask explosionMask = (1 << 9) | (1 << 12);

	public Vector2 xLimits;
	public Vector2 zLimits;

	private float checkSphereRadius = 5f;

	private float cubeYPosition = 3f;

	private const float defaultScaleDuration = 0.8f;

	private const float defaultDurationBetweenSpawn = 0.1f;

	void Awake ()
	{
		GlobalVariables.Instance.OnEndMode += () => StopAllCoroutines ();
		GlobalVariables.Instance.OnMenu += () => StopAllCoroutines ();
	}

	public void SetLimits ()
	{
		xLimits.y = GameObject.FindGameObjectWithTag ("CubesSpawnLimits").transform.GetChild (0).transform.position.x;
		zLimits.y = GameObject.FindGameObjectWithTag ("CubesSpawnLimits").transform.GetChild (1).transform.position.z;

		xLimits.x = GlobalVariables.Instance.currentModePosition.x - (xLimits.y - GlobalVariables.Instance.currentModePosition.x);
		zLimits.x = GlobalVariables.Instance.currentModePosition.z - (zLimits.y - GlobalVariables.Instance.currentModePosition.z);
	}

	public void SpawnExistingPlayerRandomVoid (GameObject player, float timeBeforeSpawn = 0)
	{
		StartCoroutine (SpawnExistingPlayerRandom (player, timeBeforeSpawn));
	}

	IEnumerator SpawnExistingPlayerRandom (GameObject player, float timeBeforeSpawn = 0)
	{
		Vector3 newPos = new Vector3();

		player.SetActive (false);

		yield return new WaitForSeconds (timeBeforeSpawn);

		do
		{
			newPos = new Vector3 (Random.Range(xLimits.x, xLimits.y), player.transform.position.y, Random.Range(zLimits.x, zLimits.y));
			yield return null;	
		}
		while(Physics.CheckSphere(newPos, checkSphereRadius, gameplayLayer));

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

	public void SpawnPlayerDeadCubeVoid (PlayerName playerName, int controllerNumber, string tag)
	{
		StartCoroutine (SpawnPlayerDeadCube (playerName, controllerNumber, tag));
	}

	IEnumerator SpawnPlayerDeadCube (PlayerName playerName, int controllerNumber, string tag, float scaleDuration = defaultScaleDuration)
	{
		Vector3 newPos = new Vector3();
		int randomCube = Random.Range (0, GlobalVariables.Instance.deadCubesPrefabs.Length);

		if (GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			yield return new WaitForSeconds (1f);
			
			do
			{
				newPos = new Vector3 (Random.Range(xLimits.x, xLimits.y), cubeYPosition, Random.Range(zLimits.x, zLimits.y));
				yield return null;	
			}
			while(Physics.CheckSphere(newPos, checkSphereRadius, gameplayLayer));			
			
			GameObject deadCube = Instantiate (GlobalVariables.Instance.deadCubesPrefabs [randomCube], newPos, GlobalVariables.Instance.deadCubesPrefabs [randomCube].transform.rotation, GameObject.FindGameObjectWithTag("MovableParent").transform) as GameObject;
			
			deadCube.GetComponent<PlayersDeadCube> ().controllerNumber = controllerNumber;
			deadCube.GetComponent<PlayersDeadCube> ().playerName = playerName;
			
			Vector3 scale = deadCube.transform.lossyScale;
			deadCube.transform.localScale = Vector3.zero;
			
			deadCube.tag = "Untagged";

			if (tag != "Movable")
				deadCube.GetComponent<MovablePlayer> ().basicMovable = false;

			deadCube.transform.DOScale (scale, scaleDuration).SetEase (Ease.OutElastic);
			StartCoroutine (ChangeMovableTag (deadCube, tag, scaleDuration));


			GameObject instantiatedParticles = Instantiate(GlobalVariables.Instance.PlayerSpawnParticles, deadCube.transform.position, GlobalVariables.Instance.PlayerSpawnParticles.transform.rotation) as GameObject;

			instantiatedParticles.transform.SetParent (GlobalVariables.Instance.ParticulesClonesParent);
			instantiatedParticles.GetComponent<ParticleSystemRenderer> ().material.color = GlobalVariables.Instance.Players [(int)playerName].gameObject.GetComponent<Renderer> ().material.color;

			GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<DynamicCamera> ().otherTargetsList.Add (deadCube);

			MasterAudio.PlaySound3DAtTransformAndForget (SoundsManager.Instance.cubeSpawnSound, deadCube.transform);
		}
	}

	public void RandomPositionMovablesVoid (GameObject[] allMovables = null, float durationBetweenSpawn = defaultDurationBetweenSpawn, float scaleDuration = defaultScaleDuration)
	{
		StartCoroutine (RandomPositionMovables (allMovables, durationBetweenSpawn, scaleDuration));
	}

	public IEnumerator RandomPositionMovables (GameObject[] allMovables = null, float durationBetweenSpawn = defaultDurationBetweenSpawn, float scaleDuration = defaultScaleDuration)
	{
		Vector3[] allScales = new Vector3[allMovables.Length];
		string[] allTags = new string[allMovables.Length];

		for(int i = 0; i < allMovables.Length; i++)
		{
			allMovables [i].SetActive (false);
			allScales [i] = allMovables [i].transform.lossyScale;
			allMovables [i].transform.localScale = new Vector3 (0, 0, 0);
			allTags [i] = allMovables [i].tag;
		}

		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		for(int i = 0; i < allMovables.Length; i++)
		{
			if(allMovables[i] != null)
				allMovables [i].tag = "Untagged";
			
			Vector3 newPos = new Vector3 ();

			do
			{
				newPos = new Vector3(Random.Range(xLimits.x, xLimits.y), cubeYPosition, Random.Range(zLimits.x, zLimits.y));
			}
			while(Physics.CheckSphere(newPos, checkSphereRadius, gameplayLayer));

			yield return new WaitForSeconds (durationBetweenSpawn);

			if(allMovables[i] != null)
			{
				EnableGameObject (allMovables [i], newPos);
				ScaleGameObect (allMovables [i], allTags [i], allScales [i], scaleDuration);
			}
				
			yield return null;
		}
	}

	public void SpawnExistingMovableVoid (GameObject movable, Vector3 position, float scaleDuration = defaultScaleDuration)
	{
		StartCoroutine (SpawnExistingMovable (movable, position, scaleDuration));
	}

	IEnumerator SpawnExistingMovable (GameObject movable, Vector3 position, float scaleDuration = defaultScaleDuration)
	{
		Vector3 movableScale = movable.transform.lossyScale;
		movable.gameObject.SetActive(false);
		string tagTemp = movable.tag;
		movable.tag = "Untagged";

		yield return new WaitWhile (()=> Physics.CheckSphere (position, checkSphereRadius, gameplayLayer));

		EnableGameObject (movable, position);
		ScaleGameObect (movable, tagTemp, movableScale, scaleDuration);
	}
		
	public void SpawnExistingMovableRandom (GameObject movable, float scaleDuration = defaultScaleDuration)
	{
		Vector3 movableScale = movable.transform.lossyScale;
		Vector3 newPos = new Vector3 ();
		string tagTemp = movable.tag;
		movable.tag = "Untagged";

		do
		{
			newPos = new Vector3(Random.Range(xLimits.x, xLimits.y), cubeYPosition, Random.Range(zLimits.x, zLimits.y));
		}
		while(Physics.CheckSphere(newPos, checkSphereRadius, gameplayLayer));

		EnableGameObject (movable, newPos);
		ScaleGameObect (movable, tagTemp, movableScale, scaleDuration);
	}

	public void SpawnNewMovableRandomVoid (GameObject movable = null, float delay = 0, float scaleDuration = defaultScaleDuration)
	{
		StartCoroutine (SpawnNewMovableRandom (movable, delay, scaleDuration));
	}

	IEnumerator SpawnNewMovableRandom (GameObject movable = null, float delay = 0, float scaleDuration = defaultScaleDuration)
	{
		if (movable == null)
			movable = GlobalVariables.Instance.cubesPrefabs [Random.Range (0, GlobalVariables.Instance.cubesPrefabs.Length)];

		Vector3 movableScale = movable.transform.lossyScale;
		Vector3 newPos = new Vector3 ();
		string tagTemp = movable.tag;

		GameObject clone = Instantiate (movable, newPos, Quaternion.Euler (Vector3.zero), movable.transform.parent) as GameObject;
		clone.gameObject.SetActive(false);

		yield return new WaitForSeconds (delay);

		do
		{
			newPos = new Vector3(Random.Range(xLimits.x, xLimits.y), cubeYPosition, Random.Range(zLimits.x, zLimits.y));
		}
		while(Physics.CheckSphere(newPos, checkSphereRadius, gameplayLayer));

		clone.tag = "Untagged";

		EnableGameObject (clone, newPos);
		ScaleGameObect (clone, tagTemp, movableScale, scaleDuration);
	}

	void EnableGameObject (GameObject target, Vector3 position)
	{
		target.transform.position = position;

		target.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		target.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;

		target.gameObject.SetActive(true);
	}

	void ScaleGameObect (GameObject target, string tag, Vector3 scale, float scaleDuration)
	{
		target.transform.localScale = Vector3.zero;
		target.transform.DOScale (scale, scaleDuration).SetEase (Ease.OutElastic);
		StartCoroutine (ChangeMovableTag (target, tag, scaleDuration));

		MasterAudio.PlaySound3DAtTransformAndForget (SoundsManager.Instance.cubeSpawnSound, target.transform);
	}

	public void Explosion (Vector3 explosionPosition, float explosionForce, float explosionRadius)
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

	public GameObject ExplosionFX (GameObject player, Vector3 position)
	{
		int playerNumber = (int)player.GetComponent<PlayersGameplay> ().playerName;

		GameObject instance = Instantiate (GlobalVariables.Instance.explosionFX [playerNumber], position, GlobalVariables.Instance.explosionFX [playerNumber].transform.rotation) as GameObject;
		instance.transform.parent = GlobalVariables.Instance.ParticulesClonesParent.transform;

		MasterAudio.PlaySound3DAtVector3AndForget (SoundsManager.Instance.explosionSound, position);

		return instance;
	}

	IEnumerator ChangeMovableTag (GameObject movable, string tagTemp, float timeTween)
	{
		yield return new WaitForSeconds (0.3f * timeTween);

		if(movable != null)
			movable.tag = tagTemp;
	}
}
