using UnityEngine;
using System.Collections;
using DG.Tweening;
using Rewired;

public class MagnetTriggerScript : MonoBehaviour 
{	
	[HideInInspector]
	public Transform magnetPoint;

	protected PlayersGameplay playerScript;

	protected Player rewiredPlayer;

	// Use this for initialization
	protected virtual void Start () 
	{
		playerScript = gameObject.transform.parent.GetComponent<PlayersGameplay> ();
	}

	protected virtual void OnTriggerStay (Collider other)
	{
		if (playerScript.rewiredPlayer == null)
			return;
		
		rewiredPlayer = ReInput.players.GetPlayer (playerScript.rewiredPlayer.id);

		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing && rewiredPlayer != null && other.tag == "Movable")
		{
			if(playerScript.playerState != PlayerState.Dead && playerScript.playerState != PlayerState.Stunned && playerScript.holdState == HoldState.CanHold)
			{
				if (rewiredPlayer.GetButton ("Attract") && !rewiredPlayer.GetButton ("Repulse"))
					GetMovable (other);
			}
		}
	}

	public virtual void GetMovable (Collider other)
	{
		playerScript.OnHoldMovable (other.gameObject);
	}
}
