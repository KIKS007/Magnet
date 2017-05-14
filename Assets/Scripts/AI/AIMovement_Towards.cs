using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement_Towards : AIComponent 
{
	protected Transform target;

	protected virtual void Update ()
	{
		if (!AIScript.movementLayerEnabled)
			return;

		if (AIScript.playerState == PlayerState.Dead || AIScript.playerState == PlayerState.Startup || AIScript.playerState == PlayerState.Stunned)
			return;

		if(target == null)
		{
			if (Vector3.Distance (transform.position, Vector3.zero) > 2f)
				AIScript.movement = (Vector3.zero - transform.position).normalized;
			else
				AIScript.movement = Vector3.zero;
		}
		else
		{
			if(Vector3.Distance (transform.position, target.position) > 2f)
				AIScript.movement = (target.position - transform.position).normalized;
			else
				AIScript.movement = Vector3.zero;
		}
	}
}
