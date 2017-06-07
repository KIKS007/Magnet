using UnityEngine;
using System.Collections;
using DG.Tweening;
using DarkTonic.MasterAudio;

public class MovablePlague : MovableScript 
{
	[Header ("PLAGUE")]
	public float deadlyCubeTransitionDuration = 0.5f;
	public float deadlyCubeMass = 50;
	public float deadlyCubeMaxVelocity = 2;
	[Range (0, 1)]
	public float deadlyCubeDeceleration = 0.97f;

	[Header ("Explosion")]
	public float explosionForce = 50;
	public float explosionRadius = 50;

	protected override void Start ()
	{
		base.Start ();

		ToNeutralColor ();
	}

	protected override void Update ()
	{
		if(hold == false && rigidbodyMovable != null)
			currentVelocity = rigidbodyMovable.velocity.magnitude;


		if(hold == false && currentVelocity > 0)
		{
			if(currentVelocity > higherVelocity)
				higherVelocity = currentVelocity;

			if(tag != "DeadCube")
			{
				if(currentVelocity >= limitVelocity)
					gameObject.tag = "ThrownMovable";
				
				else if(currentVelocity < limitVelocity && gameObject.tag == "ThrownMovable")
				{
					slowMoTrigger.triggerEnabled = false;
					gameObject.tag = "Movable";
					playerThatThrew = null;
				}				
			}
		}
	}

	protected override void HitPlayer (Collision other)
	{
		if(other.collider.tag == "Player" && other.collider.GetComponent<PlayersGameplay>().playerState != PlayerState.Stunned)
		{
			if(tag == "ThrownMovable")
			{
				if(playerThatThrew == null || other.gameObject.name != playerThatThrew.name)
				{
					StartCoroutine (DeadlyTransition ());

					other.gameObject.GetComponent<PlayersGameplay>().StunVoid(true);
					
					playerHit = other.gameObject;

					InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);	
					
					if(playerThatThrew != null)
						StatsManager.Instance.PlayersHits (playerThatThrew, playerHit);

				}				
			}
		}

		if(other.collider.tag == "Player" 
			&& other.collider.GetComponent<PlayersGameplay>().playerState != PlayerState.Dead && tag == "DeadCube")
		{
			other.collider.GetComponent<PlayersGameplay> ().Death (DeathFX.All, other.contacts [0].point);
			InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);

			GlobalMethods.Instance.Explosion (transform.position, explosionForce, explosionRadius);
		}
	}

	IEnumerator DeadlyTransition ()
	{
		cubeColor = CubeColor.Neutral;

		GlobalMethods.Instance.SpawnNewMovableRandomVoid (gameObject, 2);

		tag = "Untagged";

		ToDeadlyColor ();

		while (rigidbodyMovable.velocity.magnitude > deadlyCubeMaxVelocity)
		{
			rigidbodyMovable.velocity = rigidbodyMovable.velocity.normalized * deadlyCubeDeceleration;

			yield return new WaitForFixedUpdate ();
		}

		yield return new WaitForSeconds (deadlyCubeTransitionDuration);

		tag = "DeadCube";

		rigidbodyMovable.mass = deadlyCubeMass;
	}
}
