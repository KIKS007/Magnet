using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AIComponent : MonoBehaviour 
{
	public bool stuckAvoidance = false;
	[ShowIf ("stuckAvoidance")]
	public float stuckDelay;
	[ShowIf ("stuckAvoidance")]
	public string animatorTrigger;

	[HideInInspector]
	public float enableDelay = 0;

	protected AIGameplay AIScript;
	protected bool componentEnabled = true;

	// Use this for initialization
	protected virtual void Awake () 
	{
		AIScript = GetComponent<AIGameplay> ();
	}

	protected virtual void OnEnable ()
	{
		StartCoroutine (OnEnableDelay ());
	}

	IEnumerator OnEnableDelay ()
	{
		componentEnabled = false;

		foreach (var l in AIScript.aiComponentsDelay) 
		{
			if(l.components.Contains ( (AIComponents) System.Enum.Parse (typeof(AIComponents), this.GetType ().ToString () )) )
			{
				enableDelay =  Random.Range (l.delays [(int)AIScript.aiLevel].x, l.delays [(int)AIScript.aiLevel].y);
				break;
			}
		}

		if(enableDelay > 0)
			yield return new WaitForSecondsRealtime (enableDelay);

		componentEnabled = true;

		if (stuckAvoidance)
			StartCoroutine (StuckDelay ());

		Enable ();

		yield return 0;
	}

	protected virtual bool CanPlay ()
	{
		if (AIScript.playerState == PlayerState.Dead || AIScript.playerState == PlayerState.Startup || AIScript.playerState == PlayerState.Stunned)
			return false;

		if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
			return false;

		if (!componentEnabled)
			return false;


		return true;
	}

	IEnumerator StuckDelay ()
	{
		yield return new WaitForSecondsRealtime (stuckDelay);

		AIScript.aiAnimator.SetTrigger (animatorTrigger);
	}

	protected virtual void Enable ()
	{
		
	}

	protected virtual void Update ()
	{
		
	}

	protected virtual void OnDisable ()
	{
		StopAllCoroutines ();
		componentEnabled = true;
	}

	protected virtual bool DangerousCubes (Vector3 movement)
	{
		int layer = 1 << LayerMask.NameToLayer ("Movables");
		RaycastHit hit;
		Physics.Raycast (transform.position, movement, out hit, 9f, layer);

		if (hit.collider == null)
			return false;

		if (hit.collider.gameObject.tag == "DeadCube" || hit.collider.gameObject.tag == "Suggestible")
			return true;

		else
			return false;
	}
}
