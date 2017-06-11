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

	public override void Start ()
	{
		base.Start ();

		ToNeutralColor (0);
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
					
					InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);	
					
					if(playerThatThrew != null)
						StatsManager.Instance.PlayersHits (playerThatThrew, other.gameObject);

				}				
			}
		}

		if(other.collider.tag == "Player" 
			&& other.collider.GetComponent<PlayersGameplay>().playerState != PlayerState.Dead && tag == "DeadCube")
		{
			other.collider.GetComponent<PlayersGameplay> ().Death (DeathFX.All, other.contacts [0].point, playerThatThrew);

			if (playerThatThrew != null)
				StatsManager.Instance.PlayersHits (playerThatThrew, other.gameObject);

			InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);

			GlobalMethods.Instance.Explosion (transform.position);
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
