using UnityEngine;
using System.Collections;

public class MovableWrap : MovableScript 
{
	[Header ("Wrap Mode")]
	public bool hasWrapped = false;
	public float minX;
	public float maxX;
	public float minZ;
	public float maxZ;

	private SlowMotionTriggerScript slowMo;

	protected override void Start ()
	{
		base.Start ();

		slowMo = transform.GetChild (0).GetComponent<SlowMotionTriggerScript> ();
	}

	protected override void Update ()
	{
		base.Update ();

		if (hasWrapped && tag == "Movable")
			hasWrapped = false;

		if (hasWrapped && hold)
			hasWrapped = false;

		if (!hasWrapped && slowMo.triggerEnabled)
			slowMo.triggerEnabled = false;
			
		if(hasWrapped && tag == "ThrownMovable" && !slowMo.triggerEnabled)
		{
			slowMo.triggerEnabled = true;
		}

		if(transform.position.x < minX)
		{
			Vector3 temp = transform.position;
			temp.x = maxX;

			hasWrapped = true;
			transform.position = temp;
		}

		if(transform.position.x > maxX)
		{
			Vector3 temp = transform.position;
			temp.x = minX;

			hasWrapped = true;
			transform.position = temp;
		}

		if(transform.position.z < minZ)
		{
			Vector3 temp = transform.position;
			temp.z = maxZ;

			hasWrapped = true;
			transform.position = temp;
		}

		if(transform.position.z > maxZ)
		{
			Vector3 temp = transform.position;
			temp.z = minZ;

			hasWrapped = true;
			transform.position = temp;
		}
	}

	protected override void HitPlayer (Collision other)
	{
		if(hasWrapped)
		{
			if(other.collider.tag == "Player" 
				&& other.collider.GetComponent<PlayersGameplay>().playerState != PlayerState.Stunned 
				&& gameObject.tag == "ThrownMovable")
			{
				other.gameObject.GetComponent<PlayersGameplay>().StunVoid(true);
				hasWrapped = false;

				playerHit = other.gameObject;
				GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking(SlowMotionType.Stun);

				InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);
			}
		}
	}
}
