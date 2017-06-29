using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AIMovement_Temporize : AIComponent 
{
	public bool temporizeEnabled = true;

	private float rayLength = 6f;
	private LayerMask walls = 1 << 8;
	private int sign = -1;
	private Vector2 movementDuration = new Vector2 (0.7f, 3);

	protected override void OnEnable ()
	{
		if (!AIScript.movementLayerEnabled)
			return;
		
		if (!temporizeEnabled)
			return;

		base.OnEnable ();

		sign = -sign;

		StartCoroutine (Delay (Random.Range (movementDuration.x, movementDuration.y), ()=> ToggleSign ()));
	}

	protected override void Update ()
	{
		//return;

		if (!temporizeEnabled)
			return;
		
		if (!AIScript.movementLayerEnabled)
			return;
		
		if (AIScript.dashState == DashState.Dashing || AIScript.playerState == PlayerState.Stunned)
			return;

		Vector3 direction = (Vector3.zero - transform.position).normalized;
		direction.y = 0;

		direction = Quaternion.Euler (new Vector3 (0, sign * 90f, 0)) * direction;

		Debug.DrawRay (transform.position, direction * rayLength, Color.cyan);

		if (Physics.Raycast (transform.position, direction, rayLength, walls))
			ToggleSign ();

		AIScript.movement = direction;
	}

	void ToggleSign ()
	{
		sign = -sign;

		StartCoroutine (Delay (Random.Range (movementDuration.x, movementDuration.y), ()=> ToggleSign ()));
	}

	IEnumerator Delay (float delay, System.Action action)
	{
		yield return new WaitForSecondsRealtime (delay);

		action ();
	}

	protected override void OnDisable ()
	{
		AIScript.movement = Vector3.zero;

		base.OnDisable ();
	}
}
