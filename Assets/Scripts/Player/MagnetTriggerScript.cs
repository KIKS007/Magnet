using UnityEngine;
using System.Collections;
using DG.Tweening;
using XboxCtrlrInput;
using Rewired;

public class MagnetTriggerScript : MonoBehaviour 
{	
	[HideInInspector]
	public Transform magnetPoint;

	private Transform character;
	private PlayersGameplay characterScript;

	private Player player;

	// Use this for initialization
	void Start () 
	{
		character = gameObject.transform.parent;
		characterScript = character.GetComponent<PlayersGameplay> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	void OnTriggerStay (Collider other)
	{
		player = characterScript.player;

		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing && player != null && other.tag == "Movable")
		{
			if(characterScript.playerState != PlayerState.Holding && characterScript.playerState != PlayerState.Stunned	&& characterScript.playerState != PlayerState.Dead)
			{
				if (player.GetButton ("Attract") && !player.GetButton ("Repulse"))
					GetMovable (other);
			}
		}
	}

	public void GetMovable (Collider other)
	{
		characterScript.OnHoldMovable (other.gameObject);
	}

}
