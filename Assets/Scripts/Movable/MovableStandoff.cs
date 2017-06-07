using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovableStandoff : MovableScript 
{
	protected override void LowVelocity () 
	{
		if(hold == false && currentVelocity > 0)
		{
			if(currentVelocity > higherVelocity)
				higherVelocity = currentVelocity;

			else if(currentVelocity < limitVelocity)
			{
				if(gameObject.tag == "DeadCube")
				{
					if(slowMoTrigger == null)
						slowMoTrigger = transform.GetComponentInChildren<SlowMotionTriggerScript> ();

					slowMoTrigger.triggerEnabled = false;

					gameObject.tag = "Movable";

					ToNeutralColor ();
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
					other.gameObject.GetComponent<PlayersGameplay>().StunVoid(true);

					playerHit = other.gameObject;

					InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);	

					if(playerThatThrew != null)
						StatsManager.Instance.PlayersHits (playerThatThrew, playerHit);

				}				
			}
		}

		if(other.collider.tag == "Player" && other.collider.GetComponent<PlayersGameplay>().playerState != PlayerState.Dead)
		{
			if(tag == "DeadCube")
			{
				other.collider.GetComponent<PlayersGameplay> ().Death (DeathFX.All, other.contacts [0].point);
				GlobalMethods.Instance.Explosion (transform.position);
			}
		}
	}

	public override void OnHold ()
	{
		hold = true;

		attracedBy.Clear ();
		repulsedBy.Clear ();

		StartCoroutine (DeadlyTransition ());

		OnHoldEventVoid ();
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

		tag = "DeadCube";
	}
}
