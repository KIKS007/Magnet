using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRepel : AIComponent
{
	protected override void OnEnable ()
	{
		if (!AIScript.shootLayerEnabled)
			return;
		
		base.OnEnable ();
		
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
