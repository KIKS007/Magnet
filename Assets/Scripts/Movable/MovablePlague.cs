using UnityEngine;
using System.Collections;
using DG.Tweening;
using DarkTonic.MasterAudio;

public class MovablePlague : MovableScript 
{
	[Header ("PLAGUE")]
	public float deadlyCubeTransitionDuration = 0.5f;
	public float deadlyCubeMass = 50;
	public float deadlyCubeMaxVelocity = 2;
	[Range (0, 1)]
	public float deadlyCubeDeceleration = 0.97f;

	[Header ("Explosion")]
	[SoundGroupAttribute]
	public string explosionSound;
	public float explosionForce = 50;
	public float explosionRadius = 50;
	public LayerMask explosionMask;

	protected override void Update ()
	{
		if(hold == false && rigidbodyMovable != null)
			currentVelocity = rigidbodyMovable.velocity.magnitude;


		if(hold == false && currentVelocity > 0)
		{
			if(currentVelocity > higherVelocity)
				higherVelocity = currentVelocity;

			if(tag != "DeadCube")
			{
				if(currentVelocity >= limitVelocity)
					gameObject.tag = "ThrownMovable";
				
				else if(currentVelocity < limitVelocity && gameObject.tag == "ThrownMovable")
				{
					slowMoTrigger.triggerEnabled = false;
					gameObject.tag = "Movable";
					playerThatThrew = null;
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
					StartCoroutine (DeadlyTransition ());

					other.gameObject.GetComponent<PlayersGameplay>().StunVoid(true);
					
					playerHit = other.gameObject;
					GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking(SlowMotionType.Stun);
					
					InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);	
					
					if(playerThatThrew != null && other.gameObject.name != playerThatThrew.name)
						StatsManager.Instance.PlayersFragsAndHits (playerThatThrew, playerHit);

				}				
			}
			else if(tag == "DeadCube")
			{
				GlobalMethods.Instance.Explosion (transform.position, explosionForce, explosionRadius, explosionMask);
				MasterAudio.PlaySound3DAtTransformAndForget (explosionSound, transform);
				ExplosionFX (other);
			}
		}
	}

	IEnumerator DeadlyTransition ()
	{
		SetDeadColor ();

		GlobalMethods.Instance.SpawnNewMovableRandomVoid (gameObject, 2);

		while (rigidbodyMovable.velocity.magnitude > deadlyCubeMaxVelocity)
		{
			rigidbodyMovable.velocity = rigidbodyMovable.velocity.normalized * deadlyCubeDeceleration;

			yield return new WaitForFixedUpdate ();
		}

		yield return new WaitForSeconds (deadlyCubeTransitionDuration);

		rigidbodyMovable.mass = deadlyCubeMass;
		tag = "DeadCube";
	}

	void SetDeadColor ()
	{
		if (DOTween.IsTweening ("CubeNeutralTween" + gameObject.GetInstanceID ()))
			DOTween.Kill ("CubeNeutralTween" + gameObject.GetInstanceID ());

		Color cubeColorTemp = cubeMaterial.GetColor("_Color");
		float cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");

		DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, Color.black, toColorDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp)).SetId("CubeColorTween" + gameObject.GetInstanceID ());
		DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 1, toColorDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp)).SetId("CubeColorTween" + gameObject.GetInstanceID ());
	}

	void ExplosionFX (Collision other)
	{
		int playerNumber = (int)other.gameObject.GetComponent<PlayersGameplay> ().playerName;

		GameObject instance = Instantiate (GlobalVariables.Instance.explosionFX [playerNumber], transform.position, GlobalVariables.Instance.explosionFX [playerNumber].transform.rotation) as GameObject;
		instance.transform.parent = GlobalVariables.Instance.ParticulesClonesParent.transform;
	}
}
