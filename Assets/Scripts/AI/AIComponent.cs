using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponent : MonoBehaviour 
{
	public float enableDelay = 0;

	protected AIGameplay AIScript;
	protected bool componentEnabled = true;

	// Use this for initialization
	protected virtual void Awake () 
	{
		AIScript = GetComponent<AIGameplay> ();
	}
	
	protected virtual void Enable ()
	{
		if (AIScript.playerState == PlayerState.Dead || AIScript.playerState == PlayerState.Startup || AIScript.playerState == PlayerState.Stunned)
			return;

		if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
			return;

	}

	IEnumerator OnEnableDelay ()
	{
		if(enableDelay > 0)
		{
			componentEnabled = false;
			yield return new WaitForSecondsRealtime (enableDelay);
		}

		componentEnabled = true;
	
		Enable ();

		yield return 0;
	}

	protected virtual void Update ()
	{
		if (AIScript.playerState == PlayerState.Dead || AIScript.playerState == PlayerState.Startup || AIScript.playerState == PlayerState.Stunned)
			return;

		if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
			return;

		if (!componentEnabled)
			return;

	}

	protected virtual void OnDisable ()
	{
		if (AIScript.playerState == PlayerState.Dead || AIScript.playerState == PlayerState.Startup || AIScript.playerState == PlayerState.Stunned)
			return;

		if (!GlobalVariables.Instance || GlobalVariables.Instance.GameState != GameStateEnum.Playing)
			return;
	}
}
