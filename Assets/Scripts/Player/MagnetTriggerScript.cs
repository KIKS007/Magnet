using UnityEngine;
using System.Collections;
using DG.Tweening;
using XboxCtrlrInput;
using Rewired;

public class MagnetTriggerScript : MonoBehaviour 
{	
	public Transform magnetPoint;

	private Transform character;

	private Player player;

	// Use this for initialization
	void Start () 
	{
		character = gameObject.transform.parent;
	}
	
	// Update is called once per frame
	void Update () 
	{
		player = character.GetComponent<PlayersGameplay> ().player;
	}

	void OnTriggerStay (Collider other)
	{
		if(StaticVariables.Instance.GameOver == false)
		{
			if(other.tag == "Movable" && character.GetComponent<PlayersGameplay>().playerState != PlayerState.Holding 
				&& character.GetComponent<PlayersGameplay>().playerState != PlayerState.Stunned 
				&& character.GetComponent<PlayersGameplay>().playerState != PlayerState.Dead)
			{

				if(player.GetButton("Attract"))
				{
					other.tag = "HoldMovable";
					other.gameObject.GetComponent<MovableScript>().hold = true;
					other.gameObject.GetComponent<MovableScript>().playerThatThrew = transform.parent.gameObject;
					other.gameObject.GetComponent<MovableScript>().player = transform.parent;

					character.GetComponent<PlayersGameplay> ().OnHoldMovable (other.gameObject);

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
}
