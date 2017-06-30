using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIMovement_Towards_Pool : AIMovement_Towards 
{
	private float cubeCloseToPlayerDistance = 10;
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

		if (AIScript.closerPlayers.Count == 0)
			return;

		List<GameObject> targetsTemp = new List<GameObject> ();
		targetsTemp.AddRange (AIScript.closerPlayers);

		foreach(GameObject g in AIScript.dangerousCubes)
		{
			foreach (GameObject p in GlobalVariables.Instance.AlivePlayersList)
				if (Vector3.Distance (g.transform.position, p.transform.position) < cubeCloseToPlayerDistance)
					targetsTemp.Add (g);
		}

		targetsTemp = targetsTemp.OrderBy (x => Vector3.Distance (transform.position, x.transform.position)).ToList ();


		if(targetsTemp.Count >= randomPlayers)
		{
			do
			{
				AIScript.shootTarget = target = targetsTemp [Random.Range (0, randomPlayers)].transform;
			}
			while(!AIScript.shootTarget.gameObject.activeSelf);
		}
		else
		{
			if(targetsTemp [0].activeSelf)
				AIScript.shootTarget = target = targetsTemp [0].transform;
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
			List<GameObject> targetsTemp = new List<GameObject> ();
			targetsTemp.AddRange (AIScript.closerPlayers);

			foreach(GameObject g in AIScript.dangerousCubes)
			{
				foreach (GameObject p in GlobalVariables.Instance.AlivePlayersList)
					if (Vector3.Distance (g.transform.position, p.transform.position) < cubeCloseToPlayerDistance)
						targetsTemp.Add (g);
			}

			targetsTemp = targetsTemp.OrderBy (x => Vector3.Distance (transform.position, x.transform.position)).ToList ();


			if(targetsTemp.Count >= randomPlayers)
			{
				do
				{
					AIScript.shootTarget = target = targetsTemp [Random.Range (0, randomPlayers)].transform;
				}
				while(!AIScript.shootTarget.gameObject.activeSelf);
			}
			else
			{
				if(targetsTemp [0].activeSelf)
					AIScript.shootTarget = target = targetsTemp [0].transform;
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
