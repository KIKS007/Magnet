using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AIShoot : AIComponent
{
	protected override void Enable ()
	{
		if (!AIScript.shootLayerEnabled)
			return;
			
		if (!CanPlay ())
			return;

		base.Enable ();
		
		AIScript.Shoot ();
	}

	protected override void Update ()
	{
		if (!AIScript.shootLayerEnabled)
			return;

		if (!CanPlay ())
			return;

		base.Enable ();

		if(AIScript.holdState == HoldState.Holding)
			AIScript.Shoot ();
	}
}
