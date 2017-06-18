using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAimCube : AIAim 
{
	protected override void Enable ()
	{
		if (!AIScript.aimLayerEnabled)
			return;
		
		base.Enable ();

		target = null;

		SetTarget ();
	}

	protected override void Update ()
	{
		base.Update ();

		SetTarget ();
	}

	void SetTarget ()
	{
		if (AIScript.cubeTarget != null)
		{
			if (AIScript.cubeTarget != target || target == null)
			{
				target = AIScript.cubeTarget;
				return;
			}
		}


//		if (AIScript.currentMovementTarget != null && AIScript.currentMovementTarget.tag == "Movable")
//		{
//			target = AIScript.currentMovementTarget;
//			return;
//		}
//
//		if(target == null  && AIScript.closerCubes.Count > 0)
//		{
//			if(AIScript.closerCubes.Count > 1)
//				target = AIScript.closerCubes [Random.Range (0, 4)].transform;
//			else
//				target = AIScript.closerCubes [0].transform;
//
//			StartCoroutine (ChangeTarget (target));
//		}
	}

	IEnumerator ChangeTarget (Transform previousTarget)
	{
		float randomDuration = Random.Range (0.5f, 2f);

		yield return new WaitForSeconds (randomDuration);

//		if (AIScript.cubesAttracted.Count > 0)
//			yield break;

		do
		{
			target = AIScript.closerCubes [Random.Range (0, 4)].transform;
		}
		while (target == previousTarget && AIScript.closerCubes.Count > 2);

		Debug.Log ("CubeChange");
	}

	protected override void OnDisable ()
	{
		StopAllCoroutines ();
		
		target = null;
		
		base.OnDisable ();
	}
}
