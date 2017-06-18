using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement_TowardsCenter : AIMovement_Towards 
{
	protected override void Enable ()
	{
		if (!AIScript.movementLayerEnabled)
			return;

		if (!CanPlay ())
			return;

		base.Enable ();

		target = null;
	}
}
