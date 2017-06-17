using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponent : MonoBehaviour 
{
	protected AIGameplay AIScript;

	// Use this for initialization
	protected virtual void Awake () 
	{
		AIScript = GetComponent<AIGameplay> ();
	}
	
	protected virtual void OnEnable ()
	{
		if (AIScript.playerState == PlayerState.Dead || AIScript.playerState == PlayerState.Startup || AIScript.playerState == PlayerState.Stunned)
			return;

		if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
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
