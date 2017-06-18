using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement_TowardsCube : AIMovement_Towards 
{
	protected override void OnEnable ()
	{
		AIScript.playerTarget = null;

		if (!CanPlay ())
			return;
		
		base.OnEnable ();
	}


	protected override void Enable ()
	{
		if (!AIScript.movementLayerEnabled)
			return;

		if (!CanPlay ())
			return;
		
		base.Enable ();

		if (AIScript.closerCubes.Count == 0)
			return;

		AIScript.cubeTarget = target = AIScript.closerCubes [Random.Range (0, 4)].transform;
	}

	protected override void Update ()
	{
		if (!AIScript.movementLayerEnabled)
			return;

		if (!CanPlay ())
			return;

		base.Update ();

		if (AIScript.closerCubes.Count == 0)
			return;
		
		if(target == null || target.tag != "Movable")
			AIScript.cubeTarget = target = AIScript.closerCubes [Random.Range (0, 4)].transform;
	}

	protected override void OnDisable ()
	{
		//AIScript.cubeTarget = null;
		target = null;
		
		base.OnDisable ();
	}
}
