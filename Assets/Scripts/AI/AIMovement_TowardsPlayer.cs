using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement_TowardsPlayer : AIMovement_Towards 
{
	private int randomPlayers = 2;

	protected override void OnEnable ()
	{
		AIScript.holdTarget = null;

		base.OnEnable ();

		if (!CanPlay ())
			return;
	}

	protected override void Enable ()
	{
		if (!AIScript.movementLayerEnabled)
			return;

		if (!CanPlay ())
			return;

		base.Enable ();

		if (AIScript.closerPlayers.Count == 0)
			return;

		if(AIScript.closerPlayers.Count >= randomPlayers)
		{
			do
			{
				AIScript.shootTarget = target = AIScript.closerPlayers [Random.Range (0, randomPlayers)].transform;
			}
			while(!AIScript.shootTarget.gameObject.activeSelf);
		}
		else
		{
			if(AIScript.closerPlayers [0].activeSelf)
				AIScript.shootTarget = target = AIScript.closerPlayers [0].transform;
		}
	}

	protected override void Update ()
	{
		if (!AIScript.movementLayerEnabled)
			return;

		if (!CanPlay ())
			return;
		
		base.Update ();

		if(AIScript.shootTarget != null && !AIScript.shootTarget.gameObject.activeSelf)
		{
			AIScript.shootTarget = null;
			target = null;
		}

		if (AIScript.closerPlayers.Count == 0)
			return;

		if(target == null || target.tag != "Player")
		{
			if(AIScript.closerPlayers.Count >= randomPlayers)
			{
				do
				{
					AIScript.shootTarget = target = AIScript.closerPlayers [Random.Range (0, randomPlayers)].transform;
				}
				while(!AIScript.shootTarget.gameObject.activeSelf);
			}
			else
			{
				if(AIScript.closerPlayers [0].activeSelf)
					AIScript.shootTarget = target = AIScript.closerPlayers [0].transform;
			}
		}
	}

	protected override void OnDisable ()
	{
		//AIScript.playerTarget = null;
		target = null;

		base.OnDisable ();
	}
}
