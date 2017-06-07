using UnityEngine;
using System.Collections;
using DarkTonic.MasterAudio;

public class MovableFlow : MovableScript 
{
	[Header ("FLOW")]
	public float explosionForce = 20;
	public float explosionRadius = 50;

	protected override void Start ()
	{
		gameObject.tag = "Suggestible";
		slowMoTrigger = transform.GetComponentInChildren<SlowMotionTriggerScript> ();

		ToDeadlyColor ();
	}

	protected override void LowVelocity ()
	{
		if (currentVelocity >= limitVelocity && !slowMoTrigger.triggerEnabled)
			slowMoTrigger.triggerEnabled = true;

		if(currentVelocity < limitVelocity)
		{
			if(gameObject.tag == "ThrownMovable")
			{
				if(slowMoTrigger == null)
					slowMoTrigger = transform.GetComponentInChildren<SlowMotionTriggerScript> ();

				slowMoTrigger.triggerEnabled = false;

				gameObject.tag = "Movable";
			}
		}
	}

	protected override void HitPlayer (Collision other)
	{
		if(other.collider.tag == "Player" 
			&& other.collider.GetComponent<PlayersGameplay>().playerState != PlayerState.Dead)
		{
			other.collider.GetComponent<PlayersGameplay> ().Death (DeathFX.All, other.contacts [0].point);
			InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);

			GlobalMethods.Instance.Explosion (transform.position, explosionForce, explosionRadius);
		}
	}
}
