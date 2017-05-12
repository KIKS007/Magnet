using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDash_Counter : AIComponent 
{
	[Header ("Levels")]
	[Range (0, 100)]
	public float [] counterLevels = new float[3] { 100, 100, 100 };

	[Header ("Random")]
	public Vector2 randomBounds = new Vector2 (0.1f, 0.5f);

	protected override void OnEnable ()
	{
		base.OnEnable ();

		if (Random.Range (0, 100) > counterLevels [(int)AIScript.aiLevel])
			return;

		if (AIScript.playerState != PlayerState.Stunned)
			return;

		if (AIScript.dashState != DashState.CanDash)
			return;

		AIScript.dashState = DashState.Dashing;

		Vector3 direction = Vector3.zero - transform.position;
		direction.Normalize ();

		Vector3 random = new Vector3 ();
		random.x = Mathf.Sign (Random.Range (-1, 1)) * Random.Range (0.5f, 1);
		random.z = Mathf.Sign (Random.Range (-1, 1)) * Random.Range (0.5f, 1);

		AIScript.movement = direction + random;

		direction.Normalize ();

		AIScript.StartCoroutine ("Dash");
	}

	void Update ()
	{
//		if(AIScript.playerState == PlayerState.Stunned)
//		{
//			if (AIScript.dashState != DashState.CanDash)
//				return;
//
//			AIScript.dashState = DashState.Dashing;
//
//			Vector3 direction = Vector3.zero - transform.position;
//			direction.Normalize ();
//
//			Vector3 random = new Vector3 ();
//			random.x = Mathf.Sign (Random.Range (-1, 1)) * Random.Range (0.5f, 1);
//			random.z = Mathf.Sign (Random.Range (-1, 1)) * Random.Range (0.5f, 1);
//
//			AIScript.movement = direction + random;
//
//			direction.Normalize ();
//
//			AIScript.StartCoroutine ("Dash");
//		}
	}
}
