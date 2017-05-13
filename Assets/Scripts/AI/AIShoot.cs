using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShoot : AIComponent
{
	protected override void OnEnable ()
	{
		if (!AIScript.shootLayerEnabled)
			return;
			
		base.OnEnable ();
		
		AIScript.Shoot ();
	}
}
