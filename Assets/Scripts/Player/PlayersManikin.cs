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
			if(playerRigidbody.velocity.magnitude > maxVelocity)
				maxVelocity = playerRigidbody.velocity.magnitude;

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

			DeathParticles ();
		}
	}

	protected override void OnCollisionEnter (Collision other)
	{
		if(other.gameObject.tag == "DeadZone" && playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			Death ();

			DeathParticles ();

		}
	}

	protected override IEnumerator Stun (bool cubeHit)
	{
		playerState = PlayerState.Stunned;

		OnStunVoid ();

		speed = stunnedSpeed;

		yield return new WaitForSeconds(stunnedDuration);

		playerState = PlayerState.None;
		speed = originalSpeed;
	}

	public override void Death ()
	{
		if(playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			playerState = PlayerState.Dead;

			OnDeathVoid ();

			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking();

			gameObject.SetActive (false);
		}
	}

	public override void DeathParticles ()
	{
		GameObject instance = Instantiate (GlobalVariables.Instance.explosionFX [4], transform.position, GlobalVariables.Instance.explosionFX [4].transform.rotation) as GameObject;
		instance.transform.parent = GlobalVariables.Instance.ParticulesClonesParent.transform;
	}

	protected override void OnDestroy ()
	{
		
	}

	protected override void OnDisable ()
	{
		
	}
}
