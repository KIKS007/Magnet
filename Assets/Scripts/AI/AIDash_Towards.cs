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
		if (Random.Range (0, 101) > towardsChances [(int)AIScript.aiLevel])
			yield break;

		yield return new WaitForSecondsRealtime (Random.Range (randomDelay.x, randomDelay.y));

		if (AIScript.dashState != DashState.CanDash)
			yield break;

		if (AIScript.holdTarget != null && AIScript.shootTarget != null)
			yield break;

		if (AIScript.holdTarget == null && AIScript.shootTarget == null)
			yield break;

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
