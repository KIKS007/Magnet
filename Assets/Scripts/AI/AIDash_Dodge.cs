﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AIDash_Dodge : AIComponent 
{
	[Header ("Chances")]
	[Range (0, 100)]
	public float [] dodgeChances = new float[3] { 100, 100, 100 };

	[Header ("Random")]
	public AIRandomAngle[] randomAngles = new AIRandomAngle[3];

	[Header ("Delay")]
	public Vector2 randomDelay = new Vector2 (0.05f, 0.5f);

	protected override void Enable ()
	{
		if (!AIScript.dashLayerEnabled)
			return;

		if (!CanPlay ())
			return;

		base.Enable ();
		
		StartCoroutine (Delay ());
	}

	IEnumerator Delay ()
	{
		if (Random.Range (0, 101) > dodgeChances [(int)AIScript.aiLevel])
			yield break;

		yield return new WaitForSecondsRealtime (Random.Range (randomDelay.x, randomDelay.y));
		
		if (AIScript.thrownDangerousCubes.Count == 0)
			yield break;

		if (AIScript.dashState != DashState.CanDash)
			yield break;
		
		AIScript.dashState = DashState.Dashing;

		Vector3 direction = transform.position - AIScript.thrownDangerousCubes [0].transform.position;

		direction = Quaternion.Euler (new Vector3 (0, Mathf.Sign (Random.Range (-1, 1f)) * 90f, 0)) * direction;

		AIScript.dashMovement =  direction.normalized;

		AIScript.dashMovement = Quaternion.AngleAxis (Mathf.Sign (Random.Range (-1f, -1f)) * Random.Range (randomAngles [(int)AIScript.aiLevel].randomAngleMin, randomAngles [(int)AIScript.aiLevel].randomAngleMax), Vector3.up) * AIScript.dashMovement;

		AIScript.dashMovement.Normalize ();

		AIScript.StartCoroutine ("Dash");
	}
}
