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
		if (AIScript.playerTarget != null)
		{
			if (AIScript.playerTarget != target || target == null)
			{
				target = AIScript.playerTarget;
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
