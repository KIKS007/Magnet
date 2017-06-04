using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAim : AIComponent 
{
	public Transform target = null;
	[Range (0, 1)]
	public float[] lerps = new float[3] { 0.2f, 0.2f, 0.2f };


	protected virtual void Update ()
	{
		if (!AIScript.aimLayerEnabled)
			return;

		if (AIScript.playerState == PlayerState.Dead || AIScript.playerState == PlayerState.Startup || AIScript.playerState == PlayerState.Stunned)
			return;

		Quaternion targetRotation = new Quaternion ();

		if(target != null)
		{
			targetRotation = Quaternion.LookRotation (target.position - transform.position);
			
			transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, lerps [(int)AIScript.aiLevel]);
		}
		
		else if(AIScript.movement != Vector3.zero)
		{
			targetRotation = Quaternion.LookRotation (AIScript.movement);
			
			transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, lerps [(int)AIScript.aiLevel]);
		}
	}
}
