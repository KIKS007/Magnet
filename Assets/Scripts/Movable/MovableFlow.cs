using UnityEngine;
using System.Collections;
using DarkTonic.MasterAudio;

public class MovableFlow : MovableScript 
{
	[Header ("FLOW")]
	[SoundGroupAttribute]
	public string explosionSound;
	public float explosionForce = 20;
	public float explosionRadius = 50;
	public LayerMask explosionMask;

	protected override void Start ()
	{
		gameObject.tag = "Suggestible";
		slowMoTrigger = transform.GetComponentInChildren<SlowMotionTriggerScript> ();
	}

	protected override void Update ()
	{
		if(rigidbodyMovable != null)
			currentVelocity = rigidbodyMovable.velocity.magnitude;

		if (currentVelocity >= limitVelocity && !slowMoTrigger.triggerEnabled)
			slowMoTrigger.triggerEnabled = true;
	}

	protected override void HitPlayer (Collision other)
	{
		if(other.collider.tag == "Player" 
			&& other.collider.GetComponent<PlayersGameplay>().playerState != PlayerState.Dead)
		{
			InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);

			GlobalMethods.Instance.Explosion (transform.position, explosionForce, explosionRadius, explosionMask);
			MasterAudio.PlaySound3DAtTransformAndForget (explosionSound, transform);
		}
	}
}
