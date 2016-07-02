using UnityEngine;
using System.Collections;
using XboxCtrlrInput;
using Rewired;

public class MagnetZoneScript : MonoBehaviour 
{
	public float rayLength;

	private Transform character;

	private RaycastHit objectHit;

	private Player player;

	// Use this for initialization
	void Start () 
	{
		character = gameObject.transform.parent;
	}

	void Update ()
	{
		player = character.GetComponent<PlayersGameplay> ().player;
	}

	void OnTriggerStay (Collider other)
	{
		if(other.tag == "Movable" && character.GetComponent<PlayersGameplay>().holdingMovable == false && character.GetComponent<PlayersGameplay>().bumped == false || other.tag == "Fluff" && character.GetComponent<PlayersGameplay>().holdingMovable == false && character.GetComponent<PlayersGameplay>().bumped == false)
		{
			if(Physics.Raycast(character.transform.position, other.transform.position - character.transform.position, out objectHit, rayLength))
			{
				Debug.DrawRay(character.transform.position, other.transform.position - character.transform.position, Color.red);


				if(StaticVariables.GamePaused == false && player != null)
				{
					if(objectHit.transform.tag == "Movable" && player.GetButton("Attract"))
					{
						character.GetComponent<PlayersGameplay>().Attraction (objectHit.collider.gameObject);
					}

					if(objectHit.transform.tag == "Movable" && player.GetButton("Repulse"))
					{
						character.GetComponent<PlayersGameplay>().Repulsion (objectHit.collider.gameObject);
					}

					if(objectHit.transform.tag == "Fluff" && player.GetButton("Repulse"))
					{
						character.GetComponent<PlayersGameplay>().Repulsion (objectHit.collider.gameObject);
					}
				}
			}
		}

	}
}
