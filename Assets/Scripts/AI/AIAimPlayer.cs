using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAimPlayer : AIAim
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
		if (AIScript.shootTarget != null)
		{
			if (AIScript.shootTarget != target || target == null)
			{
				target = AIScript.shootTarget;
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
