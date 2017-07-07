using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIMovement_Towards_Push : AIMovement_Towards 
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

		List<GameObject> targetsTemp = new List<GameObject> (GlobalVariables.Instance.AllMovables);
		List<GameObject> players = new List<GameObject> (GlobalVariables.Instance.AlivePlayersList);
		players.Remove (gameObject);

		GameObject player = players [Random.Range (0, players.Count)];

		foreach(GameObject g in AIScript.dangerousCubes)
			targetsTemp.Remove (g);

		targetsTemp = targetsTemp.OrderBy (x => Vector3.Distance (player.transform.position, x.transform.position)).ToList ();

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

		if(target == null)
		{
			List<GameObject> targetsTemp = new List<GameObject> (GlobalVariables.Instance.AllMovables);
			List<GameObject> players = new List<GameObject> (GlobalVariables.Instance.AlivePlayersList);
			players.Remove (gameObject);

			GameObject player = players [Random.Range (0, players.Count)];

			foreach(GameObject g in AIScript.dangerousCubes)
				targetsTemp.Remove (g);

			targetsTemp = targetsTemp.OrderBy (x => Vector3.Distance (player.transform.position, x.transform.position)).ToList ();

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
