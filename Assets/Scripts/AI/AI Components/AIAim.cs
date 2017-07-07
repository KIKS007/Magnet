using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAim : AIComponent 
{
	public Transform target = null;
	[Range (0, 1)]
	public float[] lerps = new float[3] { 0.2f, 0.2f, 0.2f };

	protected override void Update ()
	{
		if (!AIScript.aimLayerEnabled)
			return;

		base.Update ();
		
		if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
			return;

		Quaternion targetRotation = new Quaternion ();

		if(target != null)
		{
			Vector3 lookat = target.position - transform.position;
			lookat.y = 0;

			targetRotation = Quaternion.LookRotation (lookat);
			
			transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, lerps [(int)AIScript.aiLevel]);
		}
		
		else if(AIScript.movement != Vector3.zero)
		{
			Vector3 lookat = AIScript.movement;
			lookat.y = 0;

			targetRotation = Quaternion.LookRotation (lookat);
			
			transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, lerps [(int)AIScript.aiLevel]);
		}
	}
}
