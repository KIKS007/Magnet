using System.Collections;
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
		yield return new WaitForSecondsRealtime (Random.Range (randomDelay.x, randomDelay.y));

		if (Random.Range (0, 100) > dodgeChances [(int)AIScript.aiLevel])
			yield break;

		if (AIScript.closerPlayers.Count == 0)
			yield break;

		if (AIScript.dashState != DashState.CanDash)
			yield break;

		AIScript.dashState = DashState.Dashing;

		Vector3 direction = transform.position - AIScript.thrownDangerousCubes [0].transform.position;

		direction = Quaternion.Euler (new Vector3 (0, Mathf.Sign (Random.Range (-1, 1f)) * 90f, 0)) * direction;

		AIScript.movement =  direction.normalized;

		AIScript.movement = Quaternion.AngleAxis (Mathf.Sign (Random.Range (-1f, -1f)) * Random.Range (randomAngles [(int)AIScript.aiLevel].randomAngleMin, randomAngles [(int)AIScript.aiLevel].randomAngleMax), Vector3.up) * AIScript.movement;

		AIScript.movement.Normalize ();

		AIScript.StartCoroutine ("Dash");
	}
}
