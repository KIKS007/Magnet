using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement_TowardsCenter : AIMovement_Towards 
{
	protected override void OnEnable ()
	{
		if (!AIScript.movementLayerEnabled)
			return;
		
		base.OnEnable ();

		AIScript.currentMovementTarget = null;
	}
}
