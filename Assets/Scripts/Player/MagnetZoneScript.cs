using UnityEngine;
using System.Collections;
using XboxCtrlrInput;
using Rewired;

public class MagnetZoneScript : MonoBehaviour 
{
	public float rayLength;

	private Transform character;
	private PlayersGameplay characterScript;

	private RaycastHit objectHit;

	public Player player;

	// Use this for initialization
	void Start () 
	{
		character = gameObject.transform.parent;
		characterScript = character.GetComponent<PlayersGameplay> ();
	}

	void Update ()
	{
		player = character.GetComponent<PlayersGameplay> ().player;
	}

	void OnTriggerStay (Collider other)
	{
		if(other.tag == "Movable" && character.GetComponent<PlayersGameplay>().playerState != PlayerState.Holding && character.GetComponent<PlayersGameplay>().playerState != PlayerState.Stunned 
			|| other.tag == "Fluff" && character.GetComponent<PlayersGameplay>().playerState != PlayerState.Holding && character.GetComponent<PlayersGameplay>().playerState != PlayerState.Stunned)
		{
			if(Physics.Raycast(character.transform.position, other.transform.position - character.transform.position, out objectHit, rayLength))
			{
				Debug.DrawRay(character.transform.position, other.transform.position - character.transform.position, Color.red);


				if(StaticVariables.Instance.GamePaused == false && player != null)
				{
					if(objectHit.transform.tag == "Movable" && player.GetButton("Attract"))
					{
						characterScript.Attraction (objectHit.collider.gameObject);
					}

					if(objectHit.transform.tag == "Movable" && player.GetButton("Repulse"))
					{
						characterScript.Repulsion (objectHit.collider.gameObject);
					}

					if(objectHit.transform.tag == "Fluff" && player.GetButton("Repulse"))
					{
						characterScript.Repulsion (objectHit.collider.gameObject);
					}
				}
			}
		}

	}
}
