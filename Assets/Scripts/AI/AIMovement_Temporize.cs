using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AIMovement_Temporize : AIComponent 
{
	private LayerMask walls = 1 << 8 | 1 << 9;

	private int sign;

	private Vector2 movementDuration = new Vector2 (0.5f, 2);

	protected override void OnEnable ()
	{
		if (!AIScript.movementLayerEnabled)
			return;
		
		base.OnEnable ();

		sign = (int)Mathf.Sign (Random.Range (-1f, 1f));

		DOVirtual.DelayedCall (Random.Range (movementDuration.x, movementDuration.y), ()=> ToggleSign ()).SetId ("Sign" + gameObject.GetInstanceID ());
	}

	void Update ()
	{
		if (!AIScript.movementLayerEnabled)
			return;
		
		if (AIScript.dashState == DashState.Dashing || AIScript.playerState == PlayerState.Stunned)
			return;

		Vector3 direction = (Vector3.zero - transform.position).normalized;
		direction = Quaternion.Euler (new Vector3 (0, sign * 90f, 0)) * direction;

		Debug.DrawRay (transform.position, direction * 10000f, Color.cyan);

		if (Physics.Raycast (transform.position, direction, 6f, walls))
			ToggleSign ();

		AIScript.movement = direction;
	}

	void ToggleSign ()
	{
		DOTween.Kill ("Sign" + gameObject.GetInstanceID ());

		DOVirtual.DelayedCall (Random.Range (movementDuration.x, movementDuration.y), ()=> ToggleSign ()).SetId ("Sign" + gameObject.GetInstanceID ());

		sign = -sign;
	}
}
