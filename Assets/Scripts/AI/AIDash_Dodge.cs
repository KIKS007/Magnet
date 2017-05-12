using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDash_Dodge : AIComponent 
{
	[Header ("Levels")]
	[Range (0, 100)]
	public float [] dashLevels = new float[3];

	[Header ("Random")]
	public Vector2 randomBounds = new Vector2 (0.5f, 1);

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

		Vector3 random = new Vector3 ();
		random.x = Mathf.Sign (Random.Range (-1, 1)) * Random.Range (randomBounds.x, randomBounds.y);
		random.z = Mathf.Sign (Random.Range (-1, 1)) * Random.Range (randomBounds.x, randomBounds.y);

		AIScript.movement += random;

		AIScript.movement.Normalize ();

		AIScript.StartCoroutine ("Dash");
	}

	void Update ()
	{
		if(AIScript.dangerousCubes.Count != 0)
		{
			if (AIScript.dashState != DashState.CanDash)
				return;

			Debug.Log ("Bite");

			AIScript.dashState = DashState.Dashing;

			Vector3 direction = transform.position - AIScript.dangerousCubes [0].transform.position;
			direction = Quaternion.AngleAxis (90f, Vector3.up) * direction;
			direction.Normalize ();

			Debug.DrawRay (transform.position, direction * 50f, Color.cyan, 2);

			Vector3 random = new Vector3 ();
			random.x = Mathf.Sign (Random.Range (-1, 1)) * Random.Range (0.5f, 1);
			random.z = Mathf.Sign (Random.Range (-1, 1)) * Random.Range (0.5f, 1);

			AIScript.movement = random;

			AIScript.StartCoroutine ("Dash");
		}
	}
}
