using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDash_Hit : AIComponent 
{
	[Header ("Levels")]
	[Range (0, 100)]
	public float [] hitLevels = new float[3] { 100, 100, 100 };

	[Header ("Random")]
	public Vector2 randomBounds = new Vector2 (0.1f, 0.5f);

	protected override void OnEnable ()
	{
		if (!AIScript.dashLayerEnabled)
			return;
		
		base.OnEnable ();

		if (Random.Range (0, 100) > hitLevels [(int)AIScript.aiLevel])
			return;

		if (AIScript.closerPlayers.Count == 0)
			return;

		if (AIScript.dashState != DashState.CanDash)
			return;

		AIScript.dashState = DashState.Dashing;

		AIScript.movement = (AIScript.closerPlayers [0].transform.position - transform.position).normalized;

		Vector3 random = new Vector3 ();
		random.x = Mathf.Sign (Random.Range (-1, 1)) * Random.Range (randomBounds.x, randomBounds.y);
		random.z = Mathf.Sign (Random.Range (-1, 1)) * Random.Range (randomBounds.x, randomBounds.y);
	
		AIScript.movement += random;

		AIScript.movement.Normalize ();

		AIScript.StartCoroutine ("Dash");
	}
}
