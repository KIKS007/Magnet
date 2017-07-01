using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIMovement_Towards_Plague : AIMovement_Towards 
{
	private int randomPlayers = 3;

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

		List<GameObject> targetsTemp = new List<GameObject> (AIScript.dangerousCubes);

		targetsTemp = targetsTemp.OrderBy (x => Vector3.Distance (transform.position, x.transform.position)).ToList ();

		if (targetsTemp.Count == 0)
			return;

		do
		{
			AIScript.shootTarget = target = targetsTemp [Random.Range (0, targetsTemp.Count)].transform;
		}
		while(!AIScript.shootTarget.gameObject.activeSelf);
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

		List<GameObject> targetsTemp = new List<GameObject> (AIScript.dangerousCubes);

		targetsTemp = targetsTemp.OrderBy (x => Vector3.Distance (transform.position, x.transform.position)).ToList ();

		if (targetsTemp.Count == 0)
			return;
		
		do
		{
			AIScript.shootTarget = target = targetsTemp [Random.Range (0, targetsTemp.Count)].transform;
		}
		while(!AIScript.shootTarget.gameObject.activeSelf);
	}

	protected override void OnDisable ()
	{
		//AIScript.playerTarget = null;
		target = null;

		base.OnDisable ();
	}
}
