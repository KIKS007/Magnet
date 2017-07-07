using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDash_Away : AIComponent 
{
	public float awayThreshold = 10f;

	[Header ("Chances")]
	[Range (0, 100)]
	public float [] awayChances = new float[3] { 100, 100, 100 };

	[Header ("Random")]
	public AIRandomAngle[] randomAngles = new AIRandomAngle[3];

	protected Quaternion randomAngle;

	protected override void Enable ()
	{
		if (!AIScript.dashLayerEnabled)
			return;

		if (!CanPlay ())
			return;

		base.Enable ();

		if (Random.Range (0, 101) > awayChances [(int)AIScript.aiLevel])
			return;

		if (AIScript.closerPlayers.Count == 0)
			return;

		if (AIScript.dashState != DashState.CanDash)
			return;

		Vector3 movement = new Vector3 ();

		foreach (GameObject g in AIScript.dangerousCubes)
			if (Vector3.Distance (g.transform.position, transform.position) < awayThreshold)
				movement += (transform.position - g.transform.position);

		int triesCount = 0;

		do 
		{
			if(triesCount == 20)
				return;

			randomAngle = Quaternion.AngleAxis (Mathf.Sign (Random.Range (-1f, -1f)) * Random.Range (randomAngles [(int)AIScript.aiLevel].randomAngleMin, randomAngles [(int)AIScript.aiLevel].randomAngleMax), Vector3.up);
			movement = randomAngle * movement;

			triesCount++;
		} 
		while (DangerousCubes (movement));

		AIScript.dashState = DashState.Dashing;


		AIScript.dashMovement = movement;

		AIScript.dashMovement.Normalize ();

		AIScript.StartCoroutine ("Dash");
	}
}
