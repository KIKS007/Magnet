using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlague : AIGameplay
{
	[Header ("AI Plague Layer")]
	public LayerMask plagueLayer;
	public LayerMask plagueLayer2;
	public float shootNothingChance = 2f;

	private float shootNothingChanceTemp = 0;

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

		Vector3 forward = transform.forward * 4f;
		forward.y = 0;

		Physics.Raycast (transform.position + forward, forward, out hit, 100f, plagueLayer, QueryTriggerInteraction.Ignore);

		Debug.DrawLine (transform.position, hit.point, Color.red);
//		Debug.Log (hit.collider);

		if (hit.collider.gameObject.tag == "Player" && hit.collider.gameObject != gameObject)
			return true;

		else if(hit.collider.gameObject.tag == "DeadCube")
		{
			Physics.Raycast (hit.point, Vector3.Reflect (forward, hit.normal), out hit2, 100f, plagueLayer2, QueryTriggerInteraction.Ignore);
			Debug.DrawRay (hit.point, Vector3.Reflect (forward, hit.normal) * 20, Color.red);

			if (hit2.collider && hit2.collider.gameObject.tag == "Player" && hit2.collider.gameObject != gameObject)
				return true;
			else if(hit2.collider && hit2.collider.gameObject != gameObject)
			{
				if(shootNothingChanceTemp >= shootNothingChance)
				{
					shootNothingChanceTemp += 0;
					return true;
				}
				else
				{
					shootNothingChanceTemp += Time.unscaledDeltaTime;
					return false;
				}
			}
			else
				return false;
		}
		else
			return false;
	}
}
