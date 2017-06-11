using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovableBounce : MovableScript 
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
					ToNeutralColor ();
					
					slowMoTrigger.triggerEnabled = false;
					gameObject.tag = "Movable";
				}

				else if(gameObject.tag == "ThrownMovable")
				{
					ToNeutralColor ();

					slowMoTrigger.triggerEnabled = false;
					gameObject.tag = "Movable";
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

	protected override void HitWall (Collision other)
	{
		if(other.gameObject.tag == "Wall" && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			if (tag != "ThrownMovable")
				return;

			/*if(currentVelocity > (limitVelocity * 0.5f))
				InstantiateImpactFX (other.contacts [0]);*/

			if(canPlaySound)
				StartCoroutine(HitSound ());

			if(currentVelocity > limitVelocity)
				StartCoroutine (DeadlyTransition ());
		}
	}

	IEnumerator DeadlyTransition ()
	{
		ToDeadlyColor (0.15f);

		yield return new WaitForSecondsRealtime (0);

		tag = "DeadCube";
	}

	public override void OnRelease ()
	{
		ToNeutralColor();

		OnReleaseEventVoid ();
	}
}
