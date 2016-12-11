using UnityEngine;
using System.Collections;
using DarkTonic.MasterAudio;

public class MovableFlow : MovableScript 
{
	[Header ("FLOW")]
	[SoundGroupAttribute]
	public string explosionSound;
	public float explosionForce = 50;
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

			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking(SlowMotionType.Death);

			GlobalMethods.Instance.Explosion (transform.position, explosionForce, explosionRadius, explosionMask);
			MasterAudio.PlaySound3DAtTransformAndForget (explosionSound, transform);
			ExplosionFX (other);

			other.gameObject.GetComponent<PlayersGameplay> ().DeathParticles (other.contacts [0], GlobalVariables.Instance.DeadParticles, other.gameObject.GetComponent<Renderer>().material.color);

			other.gameObject.GetComponent<PlayersGameplay> ().Death ();
		}
	}

	void ExplosionFX (Collision other)
	{
		int playerNumber = (int)other.gameObject.GetComponent<PlayersGameplay> ().playerName;

		GameObject instance = Instantiate (GlobalVariables.Instance.explosionFX [playerNumber], transform.position, GlobalVariables.Instance.explosionFX [playerNumber].transform.rotation) as GameObject;
		instance.transform.parent = GlobalVariables.Instance.ParticulesClonesParent.transform;
	}
}
