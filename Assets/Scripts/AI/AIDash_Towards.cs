using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AIDash_Towards : AIComponent
{
	[Header ("Chances")]
	[Range (0, 100)]
	public float [] towardsChances = new float[3] { 100, 100, 100 };

	[Header ("Random")]
	public AIRandomAngle[] randomAngles = new AIRandomAngle[3];

	protected override void Enable ()
	{
		if (!AIScript.dashLayerEnabled)
			return;

		if (!CanPlay ())
			return;

		base.Enable ();

		if (Random.Range (0, 101) > towardsChances [(int)AIScript.aiLevel])
			return;

		if (AIScript.dashState != DashState.CanDash)
			return;

		if (AIScript.holdTarget != null && AIScript.shootTarget != null)
			return;

		if (AIScript.holdTarget == null && AIScript.shootTarget == null)
			return;

		AIScript.dashState = DashState.Dashing;

		if (AIScript.shootTarget != null)
			AIScript.dashMovement = (AIScript.shootTarget.position - transform.position).normalized;
		else
			AIScript.dashMovement = (AIScript.holdTarget.position - transform.position).normalized;

		AIScript.dashMovement = Quaternion.AngleAxis (Mathf.Sign (Random.Range (-1f, -1f)) * Random.Range (randomAngles [(int)AIScript.aiLevel].randomAngleMin, randomAngles [(int)AIScript.aiLevel].randomAngleMax), Vector3.up) * AIScript.dashMovement;

		AIScript.dashMovement.Normalize ();

		AIScript.StartCoroutine ("Dash");
	}
}
