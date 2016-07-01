using UnityEngine;
using System.Collections;
using XboxCtrlrInput;

public class MagnetZoneScript : MonoBehaviour 
{
	public float rayLength;

	private Transform player;

	private RaycastHit objectHit;

	[HideInInspector]
	public bool mouseControl;

	private bool enableCollisions = false;

	// Use this for initialization
	void Start () 
	{
		player = gameObject.transform.parent;

		mouseControl = player.GetComponent<PlayersGameplay>().mouseControl;
	}

	void Update ()
	{
		if (!enableCollisions)
			enableCollisions = true;

		mouseControl = player.GetComponent<PlayersGameplay>().mouseControl;
	}

	void OnTriggerStay (Collider other)
	{
		if(enableCollisions)
		{
			if(other.tag == "Movable" && player.GetComponent<PlayersGameplay>().holdingMovable == false && player.GetComponent<PlayersGameplay>().bumped == false || other.tag == "Fluff" && player.GetComponent<PlayersGameplay>().holdingMovable == false && player.GetComponent<PlayersGameplay>().bumped == false)
			{


				if(Physics.Raycast(player.transform.position, other.transform.position - player.transform.position, out objectHit, rayLength))
				{
					Debug.DrawRay(player.transform.position, other.transform.position - player.transform.position, Color.red);

					player.GetComponent<PlayersGameplay>().objectHit = objectHit;



					if(mouseControl == true && StaticVariables.GamePaused == false)
					{
						if(objectHit.transform.tag == "Movable" && Input.GetMouseButton(1))
						{
							player.GetComponent<PlayersGameplay>().Attraction ();
						}

						if(objectHit.transform.tag == "Movable" && Input.GetMouseButton(0))
						{
							player.GetComponent<PlayersGameplay>().Repulsion ();
						}

						if(objectHit.transform.tag == "Fluff" && Input.GetMouseButton(0))
						{
							player.GetComponent<PlayersGameplay>().Repulsion ();
						}
					}



					else if(mouseControl == false && StaticVariables.GamePaused == false)
					{

						if(objectHit.transform.tag == "Movable" && XCI.GetAxisRaw(XboxAxis.RightTrigger, player.GetComponent<PlayersGameplay>().controllerNumber) != 0)
						{
							player.GetComponent<PlayersGameplay>().Attraction ();
						}

						if(objectHit.transform.tag == "Movable" && XCI.GetAxisRaw(XboxAxis.LeftTrigger, player.GetComponent<PlayersGameplay>().controllerNumber) != 0)
						{
							player.GetComponent<PlayersGameplay>().Repulsion ();
						}

						if(objectHit.transform.tag == "Fluff" && XCI.GetAxisRaw(XboxAxis.LeftTrigger, player.GetComponent<PlayersGameplay>().controllerNumber) != 0)
						{
							player.GetComponent<PlayersGameplay>().Repulsion ();
						}
					}

				}
			}
		}


	}
}
