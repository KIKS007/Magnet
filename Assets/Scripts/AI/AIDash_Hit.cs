using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AIDash_Hit : AIComponent 
{
	[Header ("Chances")]
	[Range (0, 100)]
	public float [] hitChances = new float[3] { 100, 100, 100 };

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
		yield return new WaitForSecondsRealtime (Random.Range (randomDelay.x, randomDelay.y));

		if (Random.Range (0, 100) > hitChances [(int)AIScript.aiLevel])
			yield break;

		if (AIScript.closerPlayers.Count == 0)
			yield break;

		if (AIScript.dashState != DashState.CanDash)
			yield break;

		AIScript.dashState = DashState.Dashing;

		AIScript.movement = (AIScript.closerPlayers [0].transform.position - transform.position).normalized;

		AIScript.movement = Quaternion.AngleAxis (Mathf.Sign (Random.Range (-1f, -1f)) * Random.Range (randomAngles [(int)AIScript.aiLevel].randomAngleMin, randomAngles [(int)AIScript.aiLevel].randomAngleMax), Vector3.up) * AIScript.movement;

		AIScript.movement.Normalize ();

		AIScript.StartCoroutine ("Dash");
	}
}
