using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovablePush : MovableScript 
{
	private float deadlyDelay = 0.02f;

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
		base.HitPlayer (other);

		if(other.collider.tag == "Player" && tag != "DeadCube")
		{
			PlayersGameplay playerScript = other.collider.GetComponent<PlayersGameplay> ();

			if (playerScript.playerState == PlayerState.Stunned)
				return;

			if (playerScript.dashState != DashState.Dashing)
				return;

			if(playerThatThrew == null || other.gameObject.name != playerThatThrew.name)
			{
				DeadlyTransition ();

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

			if (playerThatThrew != null)
				StatsManager.Instance.PlayersHits (playerThatThrew, other.gameObject);

			InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, GlobalVariables.Instance.playersColors [(int)playerScript.playerName]);

			GlobalMethods.Instance.Explosion (transform.position);
		}
	}

	void DeadlyTransition ()
	{
		ToDeadlyColor (0.1f);

		DOVirtual.DelayedCall (deadlyDelay, ()=> tag = "DeadCube").SetUpdate (true);
	}
}
