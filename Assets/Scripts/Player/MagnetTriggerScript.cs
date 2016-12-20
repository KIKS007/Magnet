using UnityEngine;
using System.Collections;
using DG.Tweening;
using XboxCtrlrInput;
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

		StartCoroutine (SetupRewiredPlayer ());
	}

	IEnumerator SetupRewiredPlayer ()
	{
		yield return new WaitUntil (() => playerScript.rewiredPlayer != null);

		rewiredPlayer = ReInput.players.GetPlayer (playerScript.rewiredPlayer.id);
	}

	void OnTriggerStay (Collider other)
	{
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
