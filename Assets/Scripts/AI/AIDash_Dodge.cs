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

	protected override void OnEnable ()
	{
		if (!AIScript.dashLayerEnabled)
			return;
		
		base.OnEnable ();
		
		if (Random.Range (0, 100) > dodgeChances [(int)AIScript.aiLevel])
			return;

		if (AIScript.thrownDangerousCubes.Count == 0)
			return;
		
		if (AIScript.dashState != DashState.CanDash)
			return;
		
		AIScript.dashState = DashState.Dashing;
		
		Vector3 direction = transform.position - AIScript.thrownDangerousCubes [0].transform.position;
		
		direction = Quaternion.Euler (new Vector3 (0, Mathf.Sign (Random.Range (-1, 1f)) * 90f, 0)) * direction;

		AIScript.movement = Quaternion.AngleAxis (Mathf.Sign (Random.Range (-1f, -1f)) * Random.Range (randomAngles [(int)AIScript.aiLevel].randomAngleMin, randomAngles [(int)AIScript.aiLevel].randomAngleMax), Vector3.up) * AIScript.movement;

		direction.Normalize ();
		
		DOVirtual.DelayedCall (Random.Range (randomDelay.x, randomDelay.y), ()=> AIScript.StartCoroutine ("Dash"));
	}

	void Update ()
	{
//		if (!AIScript.dashLayerEnabled)
//			return;
//		
//		if(AIScript.thrownDangerousCubes.Count != 0)
//		{
//			if (AIScript.dashState != DashState.CanDash)
//				return;
//
//			AIScript.dashState = DashState.Dashing;
//
//			Vector3 direction = transform.position - AIScript.thrownDangerousCubes [0].transform.position;
//
//			direction = Quaternion.Euler (new Vector3 (0, Mathf.Sign (Random.Range (-1, 1f)) * 90f, 0)) * direction;
//			direction.Normalize ();
//
//			Vector3 random = new Vector3 ();
//			random.x = Mathf.Sign (Random.Range (-1, 1)) * Random.Range (0.5f, 1);
//			random.z = Mathf.Sign (Random.Range (-1, 1)) * Random.Range (0.5f, 1);
//
//			AIScript.movement = direction;
//
//			direction.Normalize ();
//
//			AIScript.StartCoroutine ("Dash");
//		}
	}
}
