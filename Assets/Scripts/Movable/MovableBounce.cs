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
					slowMoTrigger.triggerEnabled = false;
					gameObject.tag = "Movable";
				}
			}
		}
	}

	protected override void HitPlayer (Collision other)
	{
		if(other.collider.tag == "Player" && tag == "ThrownMovable")
		{
			PlayersGameplay playerScript = other.collider.GetComponent<PlayersGameplay> ();

			if (playerScript.playerState == PlayerState.Stunned)
				return;

			if(playerThatThrew == null || other.gameObject.name != playerThatThrew.name)
			{
				playerScript.StunVoid(true);
				
				InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, GlobalVariables.Instance.playersColors [(int)playerScript.playerName]);	
				
				if(playerThatThrew != null)
					StatsManager.Instance.PlayersHits (playerThatThrew, other.gameObject);
			}				
		}

		if(other.collider.tag == "Player" && tag == "DeadCube")
		{
			PlayersGameplay playerScript = other.collider.GetComponent<PlayersGameplay> ();

			if (playerScript.playerState == PlayerState.Dead)
				return;

			playerScript.Death (DeathFX.All, other.contacts [0].point, playerThatThrew);

			PlayerKilled ();

			if (playerThatThrew != null)
				StatsManager.Instance.PlayersHits (playerThatThrew, other.gameObject);

			InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, GlobalVariables.Instance.playersColors [(int)playerScript.playerName]);

			GlobalMethods.Instance.Explosion (transform.position);
		}
	}

	protected override void HitWall (Collision other)
	{
		if(canPlaySound)
			StartCoroutine(HitSound ());
		
		if (tag != "ThrownMovable")
			return;
		DeadlyTransition ();
	}

	void DeadlyTransition ()
	{
		ToDeadlyColor (0.1f);

		tag = "DeadCube";
	}

	public override void OnRelease ()
	{
		ToNeutralColor();

		OnReleaseEventVoid ();
	}
}
