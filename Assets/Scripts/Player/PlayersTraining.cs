using UnityEngine;
using System.Collections;

public class PlayersTraining : PlayersGameplay 
{
	[Header ("Training Settings")]
	public float timeBetweenSpawn = 1f;

	public override void Death ()
	{
		if(playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			OnDeathVoid ();


			if(playerState == PlayerState.Holding)
			{
				Transform holdMovableTemp = null;

				for(int i = 0; i < transform.childCount; i++)
				{
					if(transform.GetChild(i).tag == "Movable" || transform.GetChild(i).tag == "HoldMovable")
					{
						holdMovableTemp = transform.GetChild (i);

						holdMovableTemp.gameObject.GetComponent<MovableScript>().hold = false;

						holdMovableTemp.transform.SetParent(null);
						holdMovableTemp.transform.SetParent(movableParent);
						holdMovableTemp.GetComponent<MovableScript>().AddRigidbody();
					}
				}
			}

			for(int i = 0; i < GetComponent<PlayersFXAnimations>().attractionRepulsionFX.Count; i++)
			{
				Destroy (GetComponent<PlayersFXAnimations> ().attractionRepulsionFX [i]);
			}

			GlobalMethods.Instance.SpawnExistingPlayerRandomVoid (gameObject, timeBetweenSpawn);

			playerState = PlayerState.None;
			speed = originalSpeed;
		}
	}
}
