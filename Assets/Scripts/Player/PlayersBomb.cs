﻿using UnityEngine;
using System.Collections;

public class PlayersBomb : PlayersGameplay 
{
	public void GetBomb (Collider other)
	{
		if(playerState != PlayerState.Holding && playerState != PlayerState.Dead)
		{
			transform.GetChild (2).GetComponent<MagnetTriggerScript> ().GetMovable (other);
		}
	}
}
