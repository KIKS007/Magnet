using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShoot : AIComponent
{
	protected override void Enable ()
	{
		if (!AIScript.shootLayerEnabled)
			return;
			
		base.Enable ();
		
		AIScript.Shoot ();
	}
}
