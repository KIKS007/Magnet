using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAimForwards : AIAim
{
	protected override void OnEnable ()
	{
		if (!AIScript.aimLayerEnabled)
			return;

		base.OnEnable ();

		target = null;
	}

	protected override void Update ()
	{
		base.Update ();

		if(target != null)
			target = null;
	}
}
