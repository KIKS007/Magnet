using UnityEngine;
using System.Collections;
using DarkTonic.MasterAudio;
using System.Collections.Generic;
using DG.Tweening;

public class MovableBurden : MovableScript 
{
	[Header ("BURDEN")]
	public PlayerName targetPlayerName;
	public GameObject targetPlayer = null;
	public float trackSpeed = 1.2f;
	public float trackSpeedAdded = 0.001f;

	private float speedAddedCooldown = 0.5f;

	private List<MovableBurden> otherMovables = new List<MovableBurden>();

	private float initialTrackSpeed;

	public override void Awake ()
	{
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		initialTrackSpeed = trackSpeed;
		GlobalVariables.Instance.OnEndMode += ()=> targetPlayer = null;
	}

	public override void OnEnable ()
	{
		rigidbodyMovable = GetComponent<Rigidbody>();
		movableRenderer = GetComponent<Renderer> ();
		cubeMeshFilter = transform.GetChild (2).GetComponent<MeshFilter> ();
		cubeMaterial = transform.GetChild (1).GetComponent<Renderer> ().material;

		slowMoTrigger = transform.GetComponentInChildren<SlowMotionTriggerScript> ();

		cubeMeshFilter.mesh = GlobalVariables.Instance.cubesStripes [Random.Range (0, GlobalVariables.Instance.cubesStripes.Length)];
		attracedBy.Clear ();
		repulsedBy.Clear ();

		FindTarget ();
	}

	void FindTarget ()
	{
		if (GlobalVariables.Instance.Players [(int)targetPlayerName] == null)
			return;

		if (GlobalVariables.Instance.Players [(int)targetPlayerName].activeSelf == false)
		{
			gameObject.SetActive (false);
			return;
		}

		targetPlayer = GlobalVariables.Instance.Players [(int)targetPlayerName];

		StartCoroutine (ColorTransition ());
		StartCoroutine (AddSpeed ());
		GetToPlayerVoid ();
	}

	void GetToPlayerVoid ()
	{
		StartCoroutine (GetToPlayerPosition ());
	}

	IEnumerator ColorTransition ()
	{
		ToColor (targetPlayer);

		yield return new WaitForSeconds (1f);

		ToDeadlyColor ();
	}

	IEnumerator GetToPlayerPosition ()
	{
		rigidbodyMovable.velocity = Vector3.zero;
		rigidbodyMovable.angularVelocity = Vector3.zero;

		yield return new WaitWhile (()=> targetPlayer.GetComponent<PlayersGameplay> ().playerState == PlayerState.Startup);

		while(targetPlayer != null && Vector3.Distance(targetPlayer.transform.position, transform.position) > 0.5f)
		{
			Vector3 direction = (targetPlayer.transform.position - transform.position);
			direction.Normalize ();
			
			rigidbodyMovable.AddForce(direction * trackSpeed, ForceMode.Impulse);

			yield return new WaitWhile (()=> GlobalVariables.Instance.GameState != GameStateEnum.Playing);

			yield return new WaitForFixedUpdate();
		}
	}

	IEnumerator AddSpeed ()
	{
		yield return new WaitForSeconds (speedAddedCooldown);

		yield return new WaitWhile (()=> GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		trackSpeed += trackSpeedAdded;

		if (!hold)
			StartCoroutine (AddSpeed ());
	}

	protected override void HitPlayer (Collision other)
	{
		if(other.collider.tag == "Player" 
			&& other.collider.GetComponent<PlayersGameplay>().playerState != PlayerState.Dead)
		{
			if(other.gameObject == targetPlayer)
			{
				other.collider.GetComponent<PlayersGameplay> ().Death (DeathFX.All, other.contacts [0].point);
				InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);
				Explode ();
				StopTrackingPlayer ();
			}

			else
			{
				other.collider.GetComponent<PlayersGameplay> ().Death (DeathFX.All, other.contacts [0].point);
				InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);
				Explode ();

				for (int i = 0; i < otherMovables.Count; i++)
					if (otherMovables [i].targetPlayer == other.gameObject)
						otherMovables [i].StopTrackingPlayer ();
			}
		}
	}

	void Explode ()
	{
		mainCamera.GetComponent<SlowMotionCamera>().StartSlowMotion();

		GlobalMethods.Instance.Explosion (transform.position);
		MasterAudio.PlaySound3DAtTransformAndForget (SoundsManager.Instance.explosionSound, transform);
		//gameObject.SetActive (false);	
	}

	public void StopTrackingPlayer ()
	{
		StopCoroutine (GetToPlayerPosition ());
		StopCoroutine (AddSpeed ());

		StartCoroutine (WaitToTrackAgain ());
	}

	IEnumerator WaitToTrackAgain ()
	{
		rigidbodyMovable.velocity = Vector3.zero;

		yield return new WaitWhile (()=> GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		yield return new WaitWhile (() => targetPlayer == null || targetPlayer.activeSelf);

		if (targetPlayer == null)
			yield break;

		yield return new WaitForSeconds (1f);

		trackSpeed = initialTrackSpeed;
		GetToPlayerVoid ();
		StartCoroutine (AddSpeed ());
	}
}
