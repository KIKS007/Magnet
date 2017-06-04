using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMagnetTrigger : MagnetTriggerScript
{
	private AIGameplay AIScript;

	protected override void Start ()
	{
		base.Start ();

		AIScript = transform.GetComponentInParent <AIGameplay> ();
	}

	protected override void OnTriggerStay (Collider other)
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing && other.tag == "Movable")
		{
			if(playerScript.playerState != PlayerState.Dead && playerScript.playerState != PlayerState.Stunned && playerScript.holdState == HoldState.CanHold)
			{
				if (AIScript.isAttracting && !AIScript.isRepelling)
					GetMovable (other);
			}
		}
	}

	public override void GetMovable (Collider other)
	{
		AIScript.OnHoldMovable (other.gameObject);
	}
}
