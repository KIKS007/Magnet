using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAttract : AIComponent
{
	protected override void OnEnable ()
	{
		if (!AIScript.shootLayerEnabled)
			return;
		
		base.OnEnable ();
		
		AIScript.isAttracting = true;
	}

	protected override void OnDisable ()
	{
		if (!AIScript.shootLayerEnabled)
			return;
		
		base.OnDisable ();
		
		AIScript.isAttracting = false;
	}
}
