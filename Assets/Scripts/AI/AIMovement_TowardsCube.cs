using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement_TowardsCube : AIMovement_Towards 
{
	protected override void OnEnable ()
	{
		if (!AIScript.movementLayerEnabled)
			return;
		
		base.OnEnable ();

		if (AIScript.closerCubes.Count == 0)
			return;
				
		AIScript.currentMovementTarget = target = AIScript.closerCubes [Random.Range (0, 2)].transform;
	}
}
