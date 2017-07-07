using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRepel_Flow : AIAim 
{
	protected override void Enable ()
	{
		if (!AIScript.shootLayerEnabled)
			return;

		if (!CanPlay ())
			return;

		base.Enable ();

	//	StartCoroutine (SetTargetCoroutine ());

		SetTarget ();

		AIScript.isRepelling = true;
	}

	protected override void Update ()
	{
		base.Update ();

		if (!AIScript.shootLayerEnabled)
			return;

		if(!AIScript.isRepelling)
			AIScript.isRepelling = true;

	//	SetTarget ();
	}

	void SetTarget ()
	{
		if (AIScript.dangerousCubes.Count == 0)
			return;

		target = AIScript.holdTarget = AIScript.dangerousCubes [0].transform;
	}

	IEnumerator SetTargetCoroutine ()
	{
		if (AIScript.dangerousCubes.Count == 0)
			target = AIScript.holdTarget = AIScript.dangerousCubes [0].transform;

		yield return new WaitForSecondsRealtime (1f);

		StartCoroutine (SetTargetCoroutine ());
	}

	protected override void OnDisable ()
	{
		if (!AIScript.shootLayerEnabled)
			return;

		StopAllCoroutines ();

		base.OnDisable ();

		target = null;
		AIScript.isRepelling = false;
	}
}
