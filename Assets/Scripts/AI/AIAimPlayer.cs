using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAimPlayer : AIAim
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
		if (AIScript.playerTarget != null)
		{
			if (AIScript.playerTarget != target || target == null)
			{
				target = AIScript.playerTarget;
				return;
			}
		}
			
//		if (AIScript.currentMovementTarget != null && AIScript.currentMovementTarget.tag == "Player")
//		{
//			target = AIScript.currentMovementTarget;
//			return;
//		}
//
//		if(target == null && AIScript.closerPlayers.Count > 0)
//		{
//			if(AIScript.closerPlayers.Count > 1)
//				target = AIScript.closerPlayers [Random.Range (0, 2)].transform;
//			else
//				target = AIScript.closerPlayers [0].transform;
//
//			StartCoroutine (ChangeTarget (target));
//		}
	}

	IEnumerator ChangeTarget (Transform previousTarget)
	{
		float randomDuration = Random.Range (0.5f, 2f);

		yield return new WaitForSeconds (randomDuration);

		if (AIScript.isAimingPlayer)
			yield break;
		
		do
		{
			target = AIScript.closerPlayers [Random.Range (0, 2)].transform;
		}
		while (target == previousTarget && AIScript.closerPlayers.Count > 1);

		Debug.Log ("PlayerChange");
	}

	protected override void OnDisable ()
	{
		StopAllCoroutines ();

		target = null;

		base.OnDisable ();
	}
}
