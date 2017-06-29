using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement_Away : AIComponent 
{
	public float awayThreshold = 10f;

	[Header ("Random")]
	public AIRandomAngle[] randomAngles = new AIRandomAngle[3];

	protected Transform target;

	protected Quaternion randomAngle;

	protected override void Enable ()
	{
		if (AIScript.dangerousCubes.Count == 0)
			return;

		if (!CanPlay ())
			return;
		
		base.Enable ();

		target = AIScript.dangerousCubes [0].transform;

		randomAngle = Quaternion.AngleAxis (Mathf.Sign (Random.Range (-1f, -1f)) * Random.Range (randomAngles [(int)AIScript.aiLevel].randomAngleMin, randomAngles [(int)AIScript.aiLevel].randomAngleMax), Vector3.up);
	}

	protected override void Update ()
	{
		if (!AIScript.movementLayerEnabled)
			return;

		if (!CanPlay ())
			return;
		
		base.Update ();

		if (AIScript.playerState == PlayerState.Dead || AIScript.playerState == PlayerState.Startup || AIScript.playerState == PlayerState.Stunned)
			return;

		if (AIScript.dangerousCubes.Count == 0)
			return;

		Vector3 movement = new Vector3 ();

		foreach (GameObject g in AIScript.dangerousCubes)
			if (Vector3.Distance (g.transform.position, transform.position) < awayThreshold)
				movement += (transform.position - g.transform.position);

		movement.Normalize ();

		movement = randomAngle * movement;

		AIScript.movement = movement;
	}
}
