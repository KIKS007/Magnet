using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AIAimeZone : MonoBehaviour 
{
	public LayerMask raycastLayer;

	private AIGameplay AIScript;

	// Use this for initialization
	void Awake () 
	{
		AIScript = transform.GetComponentInParent <AIGameplay> ();
	}

	void OnTriggerEnter(Collider collider){
		if(collider.tag == "Player" || collider.tag == "Movable"){
			AIScript.objectives.Add (collider.gameObject);
			Refresh ();
		}
	}

	void OnTriggerExit(Collider collider){
		if(collider.tag == "Player" || collider.tag == "Movable"){
			AIScript.objectives.Remove (collider.gameObject);
			Refresh ();
		}
	}

	void Refresh()
	{
		AIScript.isAimingPlayer = false;
		AIScript.isAimingCube = false;

		if (AIScript.objectives.Count == 0)
			return;

		AIScript.objectives = AIScript.objectives.OrderBy (x => Vector3.Distance (transform.parent.position, x.transform.position)).ToList ();

		if(AIScript.objectives[0].tag == "Movable")
			AIScript.isAimingCube = true;
		
		else if(AIScript.objectives[0].tag == "Player")
			AIScript.isAimingPlayer = true;

		AIScript.aiAnimator.SetBool ("isAimingPlayer", AIScript.isAimingPlayer);
		AIScript.aiAnimator.SetBool ("isAimingCube", AIScript.isAimingCube);
	}
}
