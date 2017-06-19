using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AIShoot : AIComponent
{
	[Header ("Delay")]
	[MinMaxSliderAttribute (0, 3)]
	public Vector2[] delayLimits = new Vector2 [3];

	protected override void Enable ()
	{
		if (!AIScript.shootLayerEnabled)
			return;
			
		if (!CanPlay ())
			return;

		base.Enable ();
		
		StartCoroutine (Delay ());
	}

	IEnumerator Delay ()
	{
		yield return new WaitForSecondsRealtime (Random.Range (delayLimits[(int)AIScript.aiLevel].x, delayLimits[(int)AIScript.aiLevel].y));

		AIScript.Shoot ();

		yield return 0;
	}
}
