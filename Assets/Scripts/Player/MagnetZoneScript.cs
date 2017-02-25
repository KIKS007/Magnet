using UnityEngine;
using System.Collections;
using Rewired;

public class MagnetZoneScript : MonoBehaviour 
{
	public float rayLength;

	private Transform player;
	private PlayersGameplay playerScript;
	private PlayersFXAnimations fxAnimationsScript;

	private RaycastHit objectHit;

	public Player rewiredPlayer;


	// Use this for initialization
	void Start () 
	{
		player = gameObject.transform.parent;
		playerScript = player.GetComponent<PlayersGameplay> ();
		fxAnimationsScript = player.GetComponent<PlayersFXAnimations> ();
	}

	void Update ()
	{
		if(rewiredPlayer != null)
		{
			if (rewiredPlayer.GetButtonUp ("Attract"))
				playerScript.cubesAttracted.Clear();
			
			if (rewiredPlayer.GetButtonUp ("Repulse"))
				playerScript.cubesRepulsed.Clear ();
			
			if (rewiredPlayer.GetButton ("Repulse") && rewiredPlayer.GetButton ("Attract"))
			{
				playerScript.cubesAttracted.Clear ();
				playerScript.cubesRepulsed.Clear ();
			}			
		}
	}

	void OnTriggerStay (Collider other)
	{
		if (playerScript.rewiredPlayer == null)
			return;
	
		rewiredPlayer = ReInput.players.GetPlayer (playerScript.rewiredPlayer.id);

		if (playerScript.playerState == PlayerState.Startup)
			return;

		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing && rewiredPlayer != null && playerScript.holdState == HoldState.CanHold)
		{
			if(other.tag == "Movable" || other.tag == "Suggestible")
			{
				RaycastHit hit;

				if(Physics.Raycast(player.transform.position, other.transform.position - player.transform.position, out hit, rayLength))
				{
					if(hit.collider.gameObject.tag == "Movable" || hit.collider.gameObject.tag == "Suggestible")
					{
						Debug.DrawRay(player.transform.position, other.transform.position - player.transform.position, Color.red);

						if (rewiredPlayer.GetButton ("Attract") && !rewiredPlayer.GetButton ("Repulse"))
							Attract (other);

						if (rewiredPlayer.GetButton ("Repulse") && !rewiredPlayer.GetButton ("Attract"))
							Repulse (other);						
					}
				}
			}
		}
	}

	void Attract (Collider other)
	{
		if (!other.GetComponent<MovableScript> ().attracedBy.Contains (player.gameObject))
			other.GetComponent<MovableScript> ().attracedBy.Add (player.gameObject);

		if (!playerScript.cubesAttracted.Contains (other.gameObject))
		{
			playerScript.cubesAttracted.Add (other.gameObject);
			fxAnimationsScript.StartCoroutine ("AttractionFX", other.gameObject);
		}
	}

	void Repulse (Collider other)
	{
		if (!other.GetComponent<MovableScript> ().repulsedBy.Contains (player.gameObject))
			other.GetComponent<MovableScript> ().repulsedBy.Add (player.gameObject);

		if (!playerScript.cubesRepulsed.Contains (other.gameObject))
		{
			playerScript.cubesRepulsed.Add (other.gameObject);
			fxAnimationsScript.StartCoroutine ("RepulsionFX", other.gameObject);
		}
	}

	void OnTriggerExit (Collider other)
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			if(other.tag == "Movable" || other.tag =="ThrownMovable" || other.tag == "Suggestible")
			{
				if(other.GetComponent<MovableScript> ().attracedBy.Contains (player.gameObject))
					other.GetComponent<MovableScript> ().attracedBy.Remove (player.gameObject);
				
				if (other.GetComponent<MovableScript> ().repulsedBy.Contains (player.gameObject))
					other.GetComponent<MovableScript> ().repulsedBy.Remove (player.gameObject);


				if(playerScript.cubesAttracted.Contains(other.gameObject))
					playerScript.cubesAttracted.Remove (other.gameObject);

				if(playerScript.cubesRepulsed.Contains(other.gameObject))
					playerScript.cubesRepulsed.Remove (other.gameObject);
			}
		}
	}
}
