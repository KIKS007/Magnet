using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement_TowardsPlayer : AIMovement_Towards 
{
	protected override void OnEnable ()
	{
		if (!AIScript.movementLayerEnabled)
			return;
		
		base.OnEnable ();

		if (AIScript.closerPlayers.Count == 0)
			return;

		AIScript.currentMovementTarget = target = AIScript.closerPlayers [Random.Range (0, 2)].transform;
	}
}
