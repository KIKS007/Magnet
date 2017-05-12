using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement_TowardsCenter : AIMovement_Towards 
{
	protected override void OnEnable ()
	{
		base.OnEnable ();

		AIScript.currentMovementTarget = null;
	}
}
