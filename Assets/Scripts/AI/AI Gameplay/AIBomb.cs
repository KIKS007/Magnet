using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIBomb : AIGameplay
{
	private GameObject bomb;
	private MovableBomb bombScript;

	protected override void Start ()
	{
		base.Start ();

		bomb = ((BombManager)GlobalVariables.Instance.lastManManager).bomb;
		bombScript = bomb.GetComponent<MovableBomb> ();
	}

	protected override void FindCloserElements ()
	{
		if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
			return;

		closerPlayers.Clear ();

		foreach (var item in GlobalVariables.Instance.AlivePlayersList) 
		{
			if(item.activeSelf)
				closerPlayers.Add (item);	
		}

		closerPlayers = closerPlayers.OrderBy (x => Vector3.Distance (transform.position, x.transform.position)).ToList ();

		closerPlayers.Remove (gameObject);

		closerCubes.Clear ();

		if(bombScript.playerHolding == gameObject)
			closerCubes.Add (bomb);
	}

	protected override void FindDangerousCubes ()
	{
		base.FindDangerousCubes ();

		dangerousCubes.Add (bomb);
	}
}
