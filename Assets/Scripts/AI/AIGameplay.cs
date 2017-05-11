using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGameplay : PlayersGameplay 
{
	protected override void Update ()
	{
		if (rewiredPlayer == null)
			return;

		if (playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing && playerState != PlayerState.Startup)
		{
			//Stunned Rotation
			if (playerState == PlayerState.Stunned)
				transform.Rotate(0, stunnedRotation * Time.deltaTime, 0, Space.World);

			//Reset Attraction - Repulsion State
			if (playerState == PlayerState.Attracting && !rewiredPlayer.GetButton("Attract"))
				playerState = PlayerState.None;

			if (playerState == PlayerState.Repulsing && !rewiredPlayer.GetButton("Repulse"))
				playerState = PlayerState.None;

			//Attraction - Repulsion State
			if (playerState == PlayerState.None)
			{
				if (rewiredPlayer.GetButton("Attract"))
					playerState = PlayerState.Attracting;

				if (rewiredPlayer.GetButton("Repulse"))
					playerState = PlayerState.Repulsing;
			}

			//On Attracted - On Repulsed Events
			OnAttractedOnRepulsed();
		}
	}

	protected override void FixedUpdate ()
	{
		if (rewiredPlayer == null)
			return;

		if (playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing && playerState != PlayerState.Startup)
		{
			//Movement
			if (dashState != DashState.Dashing)
			{
				movement.Normalize ();

				float speedTemp = playerState != PlayerState.Stunned ? speed : stunnedSpeed;
				playerRigidbody.MovePosition(transform.position + movement * speedTemp * Time.fixedDeltaTime);
			}

			//Hold Movable
			if (holdState == HoldState.Holding)
			{
				holdMovableTransform.position = Vector3.Lerp(holdMovableTransform.position, magnetPoint.transform.position, lerpHold);
				holdMovableTransform.transform.rotation = Quaternion.Lerp(holdMovableTransform.rotation, transform.rotation, lerpHold);

				OnHoldingVoid ();
			}

			//Deceleration
			playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x * decelerationAmount, playerRigidbody.velocity.y, playerRigidbody.velocity.z * decelerationAmount);

			//Gravity
			playerRigidbody.AddForce(-Vector3.up * gravity, ForceMode.Acceleration);

			//Attraction
			if (cubesAttracted.Count > 0)
			{
				for (int i = 0; i < cubesAttracted.Count; i++)
					Attraction(cubesAttracted[i]);
			}

			//Repulse
			if (cubesRepulsed.Count > 0)
			{
				for (int i = 0; i < cubesRepulsed.Count; i++)
					Repulsion(cubesRepulsed[i]);
			}
		}
	}

	public override void SetupController ()
	{
		controllerNumber = GlobalVariables.Instance.PlayersControllerNumber [(int)playerName];

		if (controllerNumber == -1)
		{
			gameObject.SetActive(false);
		}
	}

	public override void Shoot ()
	{
		if(holdState == HoldState.Holding)
			base.Shoot ();
	}

	protected override IEnumerator Dash ()
	{
		if (rewiredPlayer.GetButtonDown ("Dash") && dashState == DashState.CanDash && movement != Vector3.zero)
			return base.Dash ();
		else
			return null;
	}

	protected override IEnumerator DeathCoroutine ()
	{
		playerState = PlayerState.Dead;

		mainCamera.GetComponent<ScreenShakeCamera>().CameraShaking(FeedbackType.Death);
		mainCamera.GetComponent<ZoomCamera>().Zoom(FeedbackType.Death);

		if(gettingMovable || holdState == HoldState.Holding)
		{
			yield return new WaitWhile(() => gettingMovable == true);

			holdMovableTransform.gameObject.GetComponent<MovableScript>().hold = false;
			holdMovableTransform.tag = "Movable";
			holdMovableTransform.SetParent(null);
			holdMovableTransform.SetParent(movableParent);
			holdMovableTransform.GetComponent<MovableScript>().AddRigidbody();
			holdMovableTransform.GetComponent<MovableScript>().OnRelease();	
		}

		holdState = HoldState.CannotHold;

		gameObject.SetActive(false);

		if(GlobalVariables.Instance.modeObjective == ModeObjective.LeastDeath)
			GlobalVariables.Instance.leastDeathManager.PlayerDeath (playerName, gameObject);
	}
}
