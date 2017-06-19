using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAimCube : AIAim 
{
	protected override void Enable ()
	{
		if (!AIScript.aimLayerEnabled)
			return;

		if (!CanPlay ())
			return;

		base.Enable ();

		target = null;

		SetTarget ();
	}

	protected override void Update ()
	{
		if (!CanPlay ())
			return;

		base.Update ();

		SetTarget ();
	}

	void SetTarget ()
	{
		if (AIScript.cubeTarget != null)
		{
			if (AIScript.cubeTarget != target || target == null)
			{
				target = AIScript.cubeTarget;
				return;
			}
		}
	}

	protected override void OnDisable ()
	{
		StopAllCoroutines ();
		
		target = null;
		
		base.OnDisable ();
	}
}
