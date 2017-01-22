using UnityEngine;
using System.Collections;

public class PlayersManikin : PlayersGameplay 
{
	protected override void Start ()
	{
		playerRigidbody = GetComponent<Rigidbody>();
	}

	protected override void Update ()
	{
		if(playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			if(playerState == PlayerState.Stunned)
			{
				transform.Rotate(0, stunnedRotation * Time.deltaTime, 0, Space.World);
			}
		}
	}

	protected override void FixedUpdate ()
	{
		if(playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x * decelerationAmount, playerRigidbody.velocity.y, playerRigidbody.velocity.z * decelerationAmount);

			playerRigidbody.AddForce (-Vector3.up * gravity, ForceMode.Acceleration);
		}
	}

	public override void OnHoldMovable (GameObject movable)
	{
		
	}

	protected override void OnCollisionStay (Collision other)
	{
		if(other.gameObject.tag == "DeadZone" && playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			Death ();

			DeathParticles (other.contacts[0], GlobalVariables.Instance.DeadParticles, GetComponent <Renderer>().material.color);
		}
	}

	protected override void OnCollisionEnter (Collision other)
	{
		if(other.gameObject.tag == "DeadZone" && playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			Death ();

			DeathExplosionFX (other.contacts[0]);

			DeathParticles (other.contacts[0], GlobalVariables.Instance.DeadParticles, GetComponent<Renderer> ().material.color);
		}
	}

	protected override IEnumerator Stun (bool cubeHit)
	{
		playerState = PlayerState.Stunned;

		OnStunVoid ();

		yield return new WaitForSeconds(stunnedDuration);

		playerState = PlayerState.None;
	}

	public override void Death ()
	{
		if(playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			playerState = PlayerState.Dead;

			OnDeathVoid ();

			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking(SlowMotionType.Death);

			gameObject.SetActive (false);
		}
	}

	public void DeathExplosionFX (ContactPoint contact)
	{
		Vector3 pos = contact.point;

		GameObject instance = Instantiate (GlobalVariables.Instance.explosionFX [4], pos, GlobalVariables.Instance.explosionFX [4].transform.rotation) as GameObject;
		instance.transform.parent = GlobalVariables.Instance.ParticulesClonesParent.transform;
	}

	public GameObject DeathParticles (ContactPoint contact, GameObject prefab, Color color)
	{
		Vector3 pos = contact.point;
		Quaternion rot = Quaternion.FromToRotation(Vector3.forward, Vector3.up);
		GameObject instantiatedParticles = Instantiate(prefab, pos, rot) as GameObject;

		instantiatedParticles.transform.SetParent (GlobalVariables.Instance.ParticulesClonesParent);
		instantiatedParticles.GetComponent<ParticleSystemRenderer>().material.color = color;

		return instantiatedParticles;
	}

	protected override void OnDestroy ()
	{
		
	}

	protected override void OnDisable ()
	{
		
	}
}
