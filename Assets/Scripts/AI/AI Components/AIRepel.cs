using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRepel : AIComponent
{
	protected override void Enable ()
	{
		if (!AIScript.shootLayerEnabled)
			return;

		if (!CanPlay ())
			return;
		
		base.Enable ();
		
		AIScript.isRepelling = true;
	}

	protected override void Update ()
	{
		base.Update ();

		if (!AIScript.shootLayerEnabled)
			return;
		
		if(!AIScript.isRepelling)
			AIScript.isRepelling = true;
	}

	protected override void OnDisable ()
	{
		if (!AIScript.shootLayerEnabled)
			return;
		
		base.OnDisable ();
		
		AIScript.isRepelling = false;
	}
}
