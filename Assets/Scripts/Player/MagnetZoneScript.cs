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
		player = characterScript.player;

		if (player.GetButtonUp ("Attract"))
			characterScript.cubesAttracted.Clear();

		if (player.GetButtonUp ("Repulse"))
			characterScript.cubesRepulsed.Clear ();

		if (player.GetButton ("Repulse") && player.GetButton ("Attract"))
		{
			characterScript.cubesAttracted.Clear ();
			characterScript.cubesRepulsed.Clear ();
		}
	}

	void OnTriggerStay (Collider other)
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing && player != null)
		{
			if(other.tag == "Movable" && characterScript.playerState != PlayerState.Holding && characterScript.playerState != PlayerState.Stunned && characterScript.playerState != PlayerState.Dead)
			{
				RaycastHit hit;

				if(Physics.Raycast(character.transform.position, other.transform.position - character.transform.position, out hit, rayLength) && hit.collider.gameObject.tag == "Movable")
				{
					Debug.DrawRay(character.transform.position, other.transform.position - character.transform.position, Color.red);

					if (player.GetButton ("Attract") && !player.GetButton ("Repulse"))
						Attract (other);
					
					if (player.GetButton ("Repulse") && !player.GetButton ("Attract"))
						Repulse (other);

				}
			}
		}
	}

	void Attract (Collider other)
	{
		if (!other.GetComponent<MovableScript> ().attracedBy.Contains (character.gameObject))
			other.GetComponent<MovableScript> ().attracedBy.Add (character.gameObject);

		if (!characterScript.cubesAttracted.Contains (other.gameObject))
		{
			characterScript.cubesAttracted.Add (other.gameObject);
			fxAnimationsScript.StartCoroutine ("AttractionFX", other.gameObject);
		}
	}

	void Repulse (Collider other)
	{
		if (!other.GetComponent<MovableScript> ().repulsedBy.Contains (character.gameObject))
			other.GetComponent<MovableScript> ().repulsedBy.Add (character.gameObject);

		if (!characterScript.cubesRepulsed.Contains (other.gameObject))
		{
			characterScript.cubesRepulsed.Add (other.gameObject);
			fxAnimationsScript.StartCoroutine ("RepulsionFX", other.gameObject);
		}
	}

	void OnTriggerExit (Collider other)
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			if(other.tag == "Movable" || other.tag =="ThrownMovable")
			{
				if(other.GetComponent<MovableScript> ().attracedBy.Contains (character.gameObject))
					other.GetComponent<MovableScript> ().attracedBy.Remove (character.gameObject);
				
				if (other.GetComponent<MovableScript> ().repulsedBy.Contains (character.gameObject))
					other.GetComponent<MovableScript> ().repulsedBy.Remove (character.gameObject);


				if(characterScript.cubesAttracted.Contains(other.gameObject))
					characterScript.cubesAttracted.Remove (other.gameObject);

				if(characterScript.cubesRepulsed.Contains(other.gameObject))
					characterScript.cubesRepulsed.Remove (other.gameObject);
			}
		}
	}
}
