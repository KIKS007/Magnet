using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement_TowardsCube : AIMovement_Towards 
{
	private AIAimCube AIAimCube;

	protected override void Awake ()
	{
		AIAimCube = GetComponent<AIAimCube> ();

		base.Awake ();
	}

	protected override void OnEnable ()
	{
		if (!AIScript.movementLayerEnabled)
			return;
		
		base.OnEnable ();

		if (AIScript.closerCubes.Count == 0)
			return;
			
//		if(AIAimCube.target != null)
//			AIScript.currentMovementTarget = target = AIAimCube.target;
//		else
//			AIScript.currentMovementTarget = target = AIScript.closerCubes [Random.Range (0, 2)].transform;

		AIScript.playerTarget = null;

		AIScript.cubeTarget = target = AIScript.closerCubes [Random.Range (0, 4)].transform;
	}

	protected override void Update ()
	{
		if (!AIScript.movementLayerEnabled)
			return;
		
		if(target == null)
			AIScript.cubeTarget = target = AIScript.closerCubes [Random.Range (0, 4)].transform;

		base.Update ();
	}

	protected override void OnDisable ()
	{
		//AIScript.cubeTarget = null;
		target = null;

		base.OnDisable ();
	}
}
