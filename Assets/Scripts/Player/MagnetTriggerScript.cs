using UnityEngine;
using System.Collections;
using DG.Tweening;
using Rewired;

public class MagnetTriggerScript : MonoBehaviour 
{	
	[HideInInspector]
	public Transform magnetPoint;

	private PlayersGameplay playerScript;

	private Player rewiredPlayer;

	// Use this for initialization
	void Start () 
	{
		playerScript = gameObject.transform.parent.GetComponent<PlayersGameplay> ();
	}

	void OnTriggerStay (Collider other)
	{
		if (playerScript.rewiredPlayer == null)
			return;
		
		rewiredPlayer = ReInput.players.GetPlayer (playerScript.rewiredPlayer.id);

		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing && rewiredPlayer != null && other.tag == "Movable")
		{
			if(playerScript.holdState == HoldState.CanHold)
			{
				if (rewiredPlayer.GetButton ("Attract") && !rewiredPlayer.GetButton ("Repulse"))
					GetMovable (other);
			}
		}
	}

	public void GetMovable (Collider other)
	{
		playerScript.OnHoldMovable (other.gameObject);
	}

}
