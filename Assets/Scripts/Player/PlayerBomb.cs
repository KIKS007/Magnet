using UnityEngine;
using System.Collections;

public class PlayerBomb : PlayersGameplay 
{
	public void GetBomb (Collider other)
	{
		if(playerState != PlayerState.Holding && playerState != PlayerState.Stunned && playerState != PlayerState.Dead)
		{
			transform.GetChild (3).GetComponent<MagnetTriggerScript> ().GetMovable (other);
		}
	}
}
