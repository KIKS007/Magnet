using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDash_Hit : AIComponent 
{
	[Header ("Levels")]
	[Range (0, 100)]
	public float [] dashLevels = new float[3];

	protected override void OnEnable ()
	{
		base.OnEnable ();

		if (Random.Range (0, 100) > dashLevels [(int)AIScript.aiLevel])
			return;

		if (AIScript.closerPlayers.Count == 0)
			return;

		if (AIScript.dashState != DashState.CanDash)
			return;

		AIScript.dashState = DashState.Dashing;

		AIScript.movement = (AIScript.closerPlayers [0].transform.position - transform.position).normalized;

		AIScript.movement += new Vector3 (Random.Range (-1f, 1f), 0, Random.Range (-1f, 1f)) * 0.5f;

		AIScript.StartCoroutine ("Dash");
	}
}
