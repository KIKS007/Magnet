using UnityEngine;
using System.Collections;
using XboxCtrlrInput;
using Rewired;

public class MagnetZoneScript : MonoBehaviour 
{
	public float rayLength;

	private Transform character;
	private PlayersGameplay characterScript;
	private PlayersFXAnimations fxAnimationsScript;

	private RaycastHit objectHit;

	public Player player;

	// Use this for initialization
	void Start () 
	{
		character = gameObject.transform.parent;
		characterScript = character.GetComponent<PlayersGameplay> ();
		fxAnimationsScript = character.GetComponent<PlayersFXAnimations> ();
	}

	void Update ()
	{
		player = character.GetComponent<PlayersGameplay> ().player;

		if (player.GetButtonUp ("Attract"))
			characterScript.cubesAttracted.Clear();

		if (player.GetButtonUp ("Repulse"))
			characterScript.cubesRepulsed.Clear ();
	}

	void OnTriggerStay (Collider other)
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			if(other.tag == "Movable" && character.GetComponent<PlayersGameplay>().playerState != PlayerState.Holding && character.GetComponent<PlayersGameplay>().playerState != PlayerState.Stunned && character.GetComponent<PlayersGameplay>().playerState != PlayerState.Dead
				|| other.tag == "Fluff" && character.GetComponent<PlayersGameplay>().playerState != PlayerState.Holding && character.GetComponent<PlayersGameplay>().playerState != PlayerState.Stunned && character.GetComponent<PlayersGameplay>().playerState != PlayerState.Dead)
			{
				if(Physics.Raycast(character.transform.position, other.transform.position - character.transform.position, out objectHit, rayLength))
				{
					Debug.DrawRay(character.transform.position, other.transform.position - character.transform.position, Color.red);


					if(player != null)
					{
						if(objectHit.transform.tag == "Movable" && player.GetButton("Attract"))
						{
							if (!other.GetComponent<MovableScript> ().attracedBy.Contains (character.gameObject))
								other.GetComponent<MovableScript> ().attracedBy.Add (character.gameObject);

							if (!characterScript.cubesAttracted.Contains (other.gameObject))
							{
								characterScript.cubesAttracted.Add (other.gameObject);
								fxAnimationsScript.StartCoroutine ("AttractionFX", other.gameObject);
							}
						
							//characterScript.Attraction (objectHit.collider.gameObject);
						}

						if(objectHit.transform.tag == "Movable" && player.GetButton("Repulse"))
						{
							if (!other.GetComponent<MovableScript> ().repulsedBy.Contains (character.gameObject))
								other.GetComponent<MovableScript> ().repulsedBy.Add (character.gameObject);

							if (!characterScript.cubesRepulsed.Contains (other.gameObject))
							{
								characterScript.cubesRepulsed.Add (other.gameObject);
								fxAnimationsScript.StartCoroutine ("RepulsionFX", other.gameObject);
							}

							//characterScript.Repulsion (objectHit.collider.gameObject);	
						}
					}
				}
			}
		}
	}

	void OnTriggerExit (Collider other)
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			if (other.tag =="Movable" && other.GetComponent<MovableScript> ().attracedBy.Contains (character.gameObject))
				other.GetComponent<MovableScript> ().attracedBy.Remove (character.gameObject);

			if (other.tag =="ThrownMovable" && other.GetComponent<MovableScript> ().attracedBy.Contains (character.gameObject))
				other.GetComponent<MovableScript> ().attracedBy.Remove (character.gameObject);


			if (other.tag =="Movable" && other.GetComponent<MovableScript> ().repulsedBy.Contains (character.gameObject))
				other.GetComponent<MovableScript> ().repulsedBy.Remove (character.gameObject);

			if (other.tag =="ThrownMovable" && other.GetComponent<MovableScript> ().repulsedBy.Contains (character.gameObject))
				other.GetComponent<MovableScript> ().repulsedBy.Remove (character.gameObject);

			if(other.tag == "Movable" || other.tag =="ThrownMovable")
			{
				if(characterScript.cubesAttracted.Contains(other.gameObject))
					characterScript.cubesAttracted.Remove (other.gameObject);

				if(characterScript.cubesRepulsed.Contains(other.gameObject))
					characterScript.cubesRepulsed.Remove (other.gameObject);
			}
		}
	}
}
