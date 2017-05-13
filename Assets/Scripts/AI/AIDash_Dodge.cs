using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDash_Dodge : AIComponent 
{
	[Header ("Levels")]
	[Range (0, 100)]
	public float [] dodgeLevels = new float[3] { 100, 100, 100 };

	[Header ("Random")]
	public Vector2 randomBounds = new Vector2 (0.1f, 0.5f);

	protected override void OnEnable ()
	{
		if (!AIScript.dashLayerEnabled)
			return;
		
		base.OnEnable ();
		
		if (Random.Range (0, 100) > dodgeLevels [(int)AIScript.aiLevel])
			return;

		if (AIScript.dangerousCubes.Count == 0)
			return;
		
		if (AIScript.dashState != DashState.CanDash)
			return;
		
		AIScript.dashState = DashState.Dashing;
		
		Vector3 direction = transform.position - AIScript.dangerousCubes [0].transform.position;
		
		direction = Quaternion.Euler (new Vector3 (0, Mathf.Sign (Random.Range (-1, 1f)) * 90f, 0)) * direction;
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
		if (!AIScript.dashLayerEnabled)
			return;
		
		if(AIScript.dangerousCubes.Count != 0)
		{
			if (AIScript.dashState != DashState.CanDash)
				return;

			AIScript.dashState = DashState.Dashing;

			Vector3 direction = transform.position - AIScript.dangerousCubes [0].transform.position;

			direction = Quaternion.Euler (new Vector3 (0, Mathf.Sign (Random.Range (-1, 1f)) * 90f, 0)) * direction;
			direction.Normalize ();

			Vector3 random = new Vector3 ();
			random.x = Mathf.Sign (Random.Range (-1, 1)) * Random.Range (0.5f, 1);
			random.z = Mathf.Sign (Random.Range (-1, 1)) * Random.Range (0.5f, 1);

			AIScript.movement = direction;

			direction.Normalize ();

			AIScript.StartCoroutine ("Dash");
		}
	}
}
