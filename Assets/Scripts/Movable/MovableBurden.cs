using UnityEngine;
using System.Collections;
using DarkTonic.MasterAudio;
using System.Collections.Generic;

public class MovableBurden : MovableScript 
{
	[Header ("BURDEN")]
	public bool targetFinder = false;
	public GameObject targetPlayer = null;
	public float trackSpeed = 1.2f;
	public float trackSpeedAdded = 0.001f;

	private float speedAddedCooldown = 0.5f;

	[Header ("Explosion")]
	public float explosionForce = 50;
	public float explosionRadius = 50;
	public LayerMask explosionMask;

	private List<MovableBurden> otherMovables = new List<MovableBurden>();

	private bool firstStart = true;

	private GameObject[] allMovables = new GameObject[0];

	protected override void Awake ()
	{
		allMovables = GameObject.FindGameObjectsWithTag ("DeadCube");
		GlobalVariables.Instance.OnEndMode += ()=> targetPlayer = null;

		for (int i = 0; i < transform.childCount; i++)
			transform.GetChild (i).gameObject.SetActive (false);
	}

	protected override void OnEnable ()
	{
		rigidbodyMovable = GetComponent<Rigidbody>();
		movableRenderer = GetComponent<Renderer> ();
		cubeMeshFilter = transform.GetChild (2).GetComponent<MeshFilter> ();
		cubeMaterial = transform.GetChild (1).GetComponent<Renderer> ().material;
		slowMoTrigger = transform.GetComponentInChildren<SlowMotionTriggerScript> ();

		cubeMeshFilter.mesh = GlobalVariables.Instance.cubesStripes [Random.Range (0, GlobalVariables.Instance.cubesStripes.Length)];
		attracedBy.Clear ();
		repulsedBy.Clear ();

		if (targetFinder && !firstStart)
			FindTarget ();

		firstStart = false;
	}

	void FindTarget ()
	{
		List<MovableBurden> otherMovables = new List<MovableBurden>();
		List<GameObject> playersList = GlobalVariables.Instance.AlivePlayersList;

		int randomPlayer = Random.Range (0, playersList.Count);
		targetPlayer = playersList [randomPlayer];

		playersList.RemoveAt (randomPlayer);

		for (int i = 0; i < allMovables.Length; i++)
			if (allMovables [i] != gameObject)
				otherMovables.Add (allMovables [i].GetComponent<MovableBurden> ());

		for (int i = 0; i < otherMovables.Count; i++)
		{
			if(playersList.Count > 0)
			{
				randomPlayer = Random.Range (0, playersList.Count);
				otherMovables [i].targetPlayer = playersList [randomPlayer];

				otherMovables [i].ToColor (otherMovables [i].targetPlayer);
				otherMovables [i].GetToPlayerVoid ();
				otherMovables [i].DisplayCube ();
				otherMovables [i].StartCoroutine ("AddSpeed");

				playersList.RemoveAt (randomPlayer);
			}
			else
			{
				Destroy (otherMovables [i].gameObject);
			}
		}

		ToColor (targetPlayer);
		GetToPlayerVoid ();
		DisplayCube ();
		StartCoroutine (AddSpeed ());
	}

	void DisplayCube ()
	{
		for (int i = 0; i < transform.childCount; i++)
			transform.GetChild (i).gameObject.SetActive (true);
	}

	void GetToPlayerVoid ()
	{
		StartCoroutine (GetToPlayerPosition ());
	}

	IEnumerator GetToPlayerPosition ()
	{
		rigidbodyMovable.velocity = Vector3.zero;
		rigidbodyMovable.angularVelocity = Vector3.zero;

		while(targetPlayer != null && Vector3.Distance(targetPlayer.transform.position, transform.position) > 0.5f)
		{
			Vector3 direction = (targetPlayer.transform.position - transform.position);
			direction.Normalize ();
			
			rigidbodyMovable.AddForce(direction * trackSpeed, ForceMode.Impulse);
			
			yield return new WaitForFixedUpdate();
		}
	}

	IEnumerator AddSpeed ()
	{
		yield return new WaitForSeconds (speedAddedCooldown);

		trackSpeed += trackSpeedAdded;

		StartCoroutine (AddSpeed ());
	}

	protected override void HitPlayer (Collision other)
	{
		if(other.gameObject == targetPlayer)
		{
			StopTrackingPlayer ();
			Explode ();
		}

		else
		{
			for (int i = 0; i < otherMovables.Count; i++)
				if (otherMovables [i].targetPlayer == other.gameObject)
				{
					Explode ();
					otherMovables [i].StopTrackingPlayer ();
				}
		}
	}

	void Explode ()
	{
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().StartSlowMotion();

		GlobalMethods.Instance.Explosion (transform.position, explosionForce, explosionRadius, explosionMask);
		GlobalMethods.Instance.ExplosionFX (targetPlayer, transform.position);
		MasterAudio.PlaySound3DAtTransformAndForget (SoundsManager.Instance.explosionSound, transform);
		//gameObject.SetActive (false);	
	}

	public void StopTrackingPlayer ()
	{
		StopCoroutine (GetToPlayerPosition ());
	}
}
