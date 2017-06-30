using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBounce : AIGameplay
{
	[Header ("AI Bounce Layer")]
	public LayerMask bounceLayer;

	protected override void Update ()
	{
		base.Update ();

		if(holdState == HoldState.Holding && IsAimingPlayer ())
			aiAnimator.SetBool ("shoot", true);
		else
			aiAnimator.SetBool ("shoot", false);
	}

	bool IsAimingPlayer ()
	{
		RaycastHit hit;
		RaycastHit hit2;

		Vector3 forward = transform.forward;
		forward.y = 0;

		Physics.Raycast (transform.position + forward, forward, out hit, 100f, bounceLayer, QueryTriggerInteraction.Ignore);

//		Debug.DrawLine (transform.position, hit.point, Color.red);
//		Debug.Log (hit.collider);

		if (hit.collider.gameObject.tag == "Player" && hit.collider.gameObject != gameObject)
			return true;

		else if(hit.collider.gameObject.tag == "Wall")
		{
			Physics.Raycast (hit.point, Vector3.Reflect (forward, hit.normal), out hit2, 100f, bounceLayer, QueryTriggerInteraction.Ignore);
//			Debug.DrawRay (hit.point, Vector3.Reflect (forward, hit.normal) * 20, Color.red);

			if (hit2.collider.gameObject.tag == "Player" && hit2.collider.gameObject != gameObject)
				return true;
			else
				return false;
		}
		else
			return false;
	}
}
