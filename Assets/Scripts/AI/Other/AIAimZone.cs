using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AIAimZone : MonoBehaviour 
{
	public bool removeHoldMovable = true;

	private int targetSearchCount = 2;
	private AIGameplay AIScript;

	// Use this for initialization
	void Awake () 
	{
		AIScript = transform.GetComponentInParent <AIGameplay> ();

		LoadModeManager.Instance.OnLevelLoaded += () => AIScript.objectives.Clear ();

		AIScript.OnStun += () => AIScript.objectives.Clear ();
		AIScript.OnDeath += () => AIScript.objectives.Clear ();

		if(removeHoldMovable)
		AIScript.OnHold += () => 
		{
			if(AIScript.objectives.Contains (AIScript.holdMovableTransform.gameObject))
			{
				AIScript.objectives.Remove (AIScript.holdMovableTransform.gameObject);
				Refresh ();
			}
		};
	}

	void OnTriggerStay (Collider collider)
	{
		if(collider.tag == "Player" || collider.tag == "Movable")
		{
			if(removeHoldMovable)
			if (collider.tag == "Movable" && collider.gameObject.transform == AIScript.holdMovableTransform)
				return;

			if(!collider.gameObject.activeSelf && AIScript.objectives.Contains (collider.gameObject))
				AIScript.objectives.Remove (collider.gameObject);

			if (!collider.gameObject.activeSelf)
				return;

			if(!AIScript.objectives.Contains (collider.gameObject))
				AIScript.objectives.Add (collider.gameObject);

			Refresh ();
		}
	}

	void OnTriggerExit(Collider collider)
	{
		if(collider.tag == "Player" || collider.tag == "Movable")
		{
			if(AIScript.objectives.Contains (collider.gameObject))
				AIScript.objectives.Remove (collider.gameObject);

			Refresh ();
		}
	}

	void Refresh()
	{
		AIScript.isAimingPlayer = false;
		AIScript.isAimingCube = false;

		if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
			return;
		
		if (AIScript.objectives.Count == 0)
			return;

		AIScript.objectives = AIScript.objectives.OrderBy (x => Vector3.Distance (transform.parent.position, x.transform.position)).ToList ();

		for(int i = 0; i < targetSearchCount + 1; i++)
		{
			if (i >= AIScript.objectives.Count)
				break;

			if(AIScript.objectives[i].tag == "Player")
				AIScript.isAimingPlayer = true;
		}

		if(AIScript.objectives[0].tag == "Movable")
			AIScript.isAimingCube = true;
		
		AIScript.aiAnimator.SetBool ("isAimingPlayer", AIScript.isAimingPlayer);
		AIScript.aiAnimator.SetBool ("isAimingCube", AIScript.isAimingCube);
	}
}
