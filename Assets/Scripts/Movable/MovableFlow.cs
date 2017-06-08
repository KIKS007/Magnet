using UnityEngine;
using System.Collections;
using DarkTonic.MasterAudio;

public class MovableFlow : MovableScript 
{
	public override void Start ()
	{
		gameObject.tag = "Suggestible";
		slowMoTrigger = transform.GetComponentInChildren<SlowMotionTriggerScript> ();

		ToDeadlyColor ();
	}

	protected override void HitPlayer (Collision other)
	{
		if(other.collider.tag == "Player" 
			&& other.collider.GetComponent<PlayersGameplay>().playerState != PlayerState.Dead)
		{
			other.collider.GetComponent<PlayersGameplay> ().Death (DeathFX.All, other.contacts [0].point);
			InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);

			GlobalMethods.Instance.Explosion (transform.position);
		}
	}
}
