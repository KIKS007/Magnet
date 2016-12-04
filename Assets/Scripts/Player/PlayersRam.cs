using UnityEngine;
using System.Collections;

public class PlayersRam : PlayersGameplay 
{
	[Header("RAM")]
	public float currentRamVelocity;
	public float maxRamVelocity;

	protected override void FixedUpdate ()
	{
		if (playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			if (dashState != DashState.Dashing)
			{
				float speedTemp = playerState != PlayerState.Stunned ? speed : stunnedSpeed;
				//playerRigidbody.MovePosition(transform.position + movement * speedTemp * Time.fixedDeltaTime);
				playerRigidbody.AddForce(movement * speedTemp);		

				if(playerState != PlayerState.Stunned)
				{
					currentRamVelocity = playerRigidbody.velocity.magnitude;
					
					if (playerRigidbody.velocity.magnitude > maxRamVelocity)
						playerRigidbody.velocity = playerRigidbody.velocity.normalized * maxRamVelocity;					
				}
			}


			if (playerState == PlayerState.Holding)
			{
				holdMovableTransform.position = Vector3.Lerp(holdMovableTransform.position, magnetPoint.transform.position, lerpHold);
				holdMovableTransform.transform.rotation = Quaternion.Lerp(holdMovableTransform.rotation, transform.rotation, lerpHold);

				OnHoldingVoid ();
			}

			playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x * decelerationAmount, playerRigidbody.velocity.y, playerRigidbody.velocity.z * decelerationAmount);

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
