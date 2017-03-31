using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovableStar : MovableScript 
{
	[Header ("Explosion")]
	public float explosionForce = 50;
	public float explosionRadius = 50;

	protected override void Update () 
	{
		if(hold == false && rigidbodyMovable != null)
			currentVelocity = rigidbodyMovable.velocity.magnitude;


		if(hold == false && currentVelocity > 0)
		{
			if(currentVelocity > higherVelocity)
				higherVelocity = currentVelocity;

			else if(currentVelocity < limitVelocity && gameObject.tag == "Suggestible")
			{
				slowMoTrigger.triggerEnabled = false;
				gameObject.tag = "Movable";
				playerThatThrew = null;

				ToNeutralColor ();
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
					other.gameObject.GetComponent<PlayersGameplay>().StunVoid(true);

					playerHit = other.gameObject;

					InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);	

					if(playerThatThrew != null && other.gameObject.name != playerThatThrew.name)
						StatsManager.Instance.PlayersFragsAndHits (playerThatThrew, playerHit);

				}				
			}
		}

		if(other.collider.tag == "Player" && other.collider.GetComponent<PlayersGameplay>().playerState != PlayerState.Dead)
		{
			if(tag == "Suggestible")
			{
				other.collider.GetComponent<PlayersGameplay> ().Death (DeathFX.All, other.contacts [0].point);
				GlobalMethods.Instance.Explosion (transform.position, explosionForce, explosionRadius);
			}
		}
	}

	public override void OnRelease ()
	{
		OnReleaseEventVoid ();

		StartCoroutine (DeadlyTransition ());
	}

	IEnumerator DeadlyTransition ()
	{
		ToDeadlyColor ();

		yield return new WaitForSeconds (0.01f);

		tag = "Suggestible";
	}
}
