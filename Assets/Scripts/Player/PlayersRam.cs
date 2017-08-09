using UnityEngine;
using System.Collections;
using Replay;

public class PlayersRam : PlayersGameplay 
{
	protected override void FixedUpdate ()
	{
		if (ReplayManager.Instance.isReplaying)
			return;

		if (playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			//No Forces
			velocity = playerRigidbody.velocity.magnitude;

			if (velocity < noForcesThreshold && playerThatHit != null && playerState != PlayerState.Stunned)
				playerThatHit = null;

			//Hold Movable
			if (holdState == HoldState.Holding)
			{
				holdMovableTransform.position = Vector3.Lerp(holdMovableTransform.position, magnetPoint.transform.position, lerpHold * Time.fixedDeltaTime);
				holdMovableTransform.transform.rotation = Quaternion.Lerp(holdMovableTransform.rotation, transform.rotation, lerpHold * Time.fixedDeltaTime);

				OnHoldingVoid ();
			}

			if (dashState != DashState.Dashing)
			{
				float speedTemp = playerState != PlayerState.Stunned ? speed : stunnedSpeed;

				playerRigidbody.MovePosition(transform.position + movement * speedTemp * Time.fixedDeltaTime);
			}

			playerRigidbody.AddForce(-Vector3.up * gravity, ForceMode.Acceleration);

			if (cubesAttracted.Count > 0)
			{
				for (int i = 0; i < cubesAttracted.Count; i++)
					Attraction(cubesAttracted[i]);
			}

			if (cubesRepulsed.Count > 0)
			{
				for (int i = 0; i < cubesRepulsed.Count; i++)
					Repulsion(cubesRepulsed[i]);
			}
		}
	}
}
