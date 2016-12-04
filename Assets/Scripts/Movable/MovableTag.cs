using UnityEngine;
using System.Collections;

public class MovableTag : MovableScript
{
	[Header ("TAG")]
	public TagManager tagManager;
	public GameObject previousOwner;

	protected override void HitPlayer (Collision other)
	{
		if(other.collider.tag == "Player" && other.collider.GetComponent<PlayersGameplay>().playerState != PlayerState.Stunned && gameObject.tag == "ThrownMovable")
		{
			if(playerThatThrew == null || other.gameObject.name != playerThatThrew.name)
			{
				other.gameObject.GetComponent<PlayersGameplay>().StunVoid(true);
				
				playerHit = other.gameObject;
				GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking(SlowMotionType.Stun);
				
				InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);

				if(other.gameObject != previousOwner)
				{
					if (previousOwner != null)
						tagManager.UpdateScores (previousOwner.GetComponent<PlayersGameplay> ().playerName, other.gameObject.GetComponent<PlayersGameplay> ().playerName);
					else
						tagManager.UpdateScores (other.gameObject.GetComponent<PlayersGameplay> ().playerName);					
				}

				ToColor (other.gameObject);
				
				if(playerThatThrew != null && other.gameObject.name != playerThatThrew.name)
					StatsManager.Instance.PlayersFragsAndHits (playerThatThrew, playerHit);

				previousOwner = other.gameObject;
			}
		}
	}

	public override void OnHold ()
	{
		hold = true;

		attracedBy.Clear ();
		repulsedBy.Clear ();
	}

	public override void OnRelease ()
	{
		
	}
}
