using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement_TowardsPlayer : AIMovement_Towards 
{
	private AIAimPlayer AIAimPlayer;

	protected override void Awake ()
	{
		AIAimPlayer = GetComponent<AIAimPlayer> ();

		base.Awake ();
	}
	protected override void Enable ()
	{
		if (!AIScript.movementLayerEnabled)
			return;
		
		base.Enable ();

		if (AIScript.closerPlayers.Count == 0)
			return;

//		if(AIAimPlayer.target != null)
//			AIScript.currentMovementTarget = target = AIAimPlayer.target;
//		else
//			AIScript.currentMovementTarget = target = AIScript.closerPlayers [Random.Range (0, 2)].transform;

		AIScript.cubeTarget = null;

		AIScript.playerTarget = target = AIScript.closerPlayers [Random.Range (0, 2)].transform;
	}

	protected override void Update ()
	{
		if (!AIScript.movementLayerEnabled)
			return;
		
		if(target == null || target.tag != "Player")
			AIScript.playerTarget = target = AIScript.closerPlayers [Random.Range (0, 2)].transform;

		base.Update ();
	}

	protected override void OnDisable ()
	{
		//AIScript.playerTarget = null;
		target = null;

		base.OnDisable ();
	}
}
