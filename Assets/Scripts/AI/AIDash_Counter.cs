using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AIDash_Counter : AIComponent 
{
	[Header ("Chances")]
	[Range (0, 100)]
	public float [] counterChances = new float[3] { 100, 100, 100 };

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

		if (Random.Range (0, 100) > counterChances [(int)AIScript.aiLevel])
			return;

		if (AIScript.playerState != PlayerState.Stunned)
			return;

		if (AIScript.dashState != DashState.CanDash)
			return;

		AIScript.dashState = DashState.Dashing;

		Vector3 direction = Vector3.zero - transform.position;

		AIScript.movement = direction.normalized;
			
		AIScript.movement = Quaternion.AngleAxis (Mathf.Sign (Random.Range (-1f, -1f)) * Random.Range (randomAngles [(int)AIScript.aiLevel].randomAngleMin, randomAngles [(int)AIScript.aiLevel].randomAngleMax), Vector3.up) * AIScript.movement;

		DOVirtual.DelayedCall (Random.Range (randomDelay.x, randomDelay.y), ()=> AIScript.StartCoroutine ("Dash")).SetUpdate (false);
	}

	protected override void Update ()
	{
		if (!AIScript.dashLayerEnabled)
			return;

		if (!CanPlay ())
			return;
		
		base.Update ();
		

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
