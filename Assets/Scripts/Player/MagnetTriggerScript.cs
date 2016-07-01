using UnityEngine;
using System.Collections;
using DG.Tweening;
using XboxCtrlrInput;

public class MagnetTriggerScript : MonoBehaviour 
{	
	public Transform magnetPoint;

	private Transform player;


	// Use this for initialization
	void Start () 
	{
		player = gameObject.transform.parent;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	void OnTriggerStay (Collider other)
	{
		if(other.tag == "Movable" && player.GetComponent<PlayersGameplay>().holdingMovable == false && player.GetComponent<PlayersGameplay>().bumped == false)
		{

			if(player.GetComponent<PlayersGameplay>().mouseControl == true && Input.GetMouseButton(1))
			{
				other.tag = "HoldMovable";
				other.gameObject.GetComponent<MovableScript>().hold = true;
				other.gameObject.GetComponent<MovableScript>().playerThatThrew = transform.parent.gameObject;
				other.gameObject.GetComponent<MovableScript>().player = transform.parent;
				player.GetComponent<PlayersGameplay>().holdingMovable = true;
				player.GetComponent<PlayersGameplay>().holdMovable = other.gameObject.GetComponent<Rigidbody>();
				player.GetComponent<PlayersGameplay>().holdMovableTransform = other.gameObject.GetComponent<Transform>();
				player.GetComponent<PlayersGameplay>().slowMotionDetection.transform.localScale = new Vector3(other.gameObject.GetComponent<Rigidbody>().transform.localScale.x, transform.localScale.y, transform.localScale.z);

				Destroy (other.GetComponent<Rigidbody>());
			
				other.transform.SetParent(transform.parent);
				
				Vector3 v3 = magnetPoint.localPosition;
				v3.x = 0f;
				v3.y = (other.transform.localScale.y /2 + 0.1f) - 1;
				v3.z = 0.5f * other.transform.localScale.z + 0.8f;
				magnetPoint.localPosition = v3;
			}

			else if(player.GetComponent<PlayersGameplay>().mouseControl == false && XCI.GetAxisRaw(XboxAxis.RightTrigger, player.GetComponent<PlayersGameplay>().controllerNumber) != 0)
			{
				other.tag = "HoldMovable";
				other.gameObject.GetComponent<MovableScript>().hold = true;
				other.gameObject.GetComponent<MovableScript>().playerThatThrew = transform.parent.gameObject;
				other.gameObject.GetComponent<MovableScript>().player = transform.parent;
				player.GetComponent<PlayersGameplay>().holdingMovable = true;
				player.GetComponent<PlayersGameplay>().holdMovable = other.gameObject.GetComponent<Rigidbody>();
				player.GetComponent<PlayersGameplay>().holdMovableTransform = other.gameObject.GetComponent<Transform>();
				player.GetComponent<PlayersGameplay>().slowMotionDetection.transform.localScale = new Vector3(other.gameObject.GetComponent<Rigidbody>().transform.localScale.x, transform.localScale.y, transform.localScale.z);

				Destroy (other.GetComponent<Rigidbody>());
				
				other.transform.SetParent(transform.parent);
				
				Vector3 v3 = magnetPoint.localPosition;
				v3.x = 0f;
				v3.y = (other.transform.localScale.y /2 + 0.1f) - 1;
				v3.z = 0.5f * other.transform.localScale.z + 0.8f;
				magnetPoint.localPosition = v3;
			}
		}
	}
}
