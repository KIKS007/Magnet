using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class TutorialColliderEventTrigger : MonoBehaviour 
{
	public bool eventEnabled = true;

	[Header ("OnCollisionEnter")]
	public UnityEvent onCollisionEnterEvents;

	[Header ("OnTriggerEnter")]
	public UnityEvent onTriggerEnterEvents;

	[Header ("Button")]
	public MenuButtonAnimationsAndSounds menuButton;
	public float rotationSpeed;

	[HideInInspector]
	public List<GameObject> touchingGameobjects = new List<GameObject> ();

	void Start ()
	{
		if(rotationSpeed > 0)
			menuButton.transform.DOLocalRotate (Vector3.forward * 360, rotationSpeed).SetSpeedBased ().SetLoops (-1, LoopType.Incremental).SetRelative ();
		else
			menuButton.transform.DOLocalRotate (-Vector3.forward * 360, -rotationSpeed).SetSpeedBased ().SetLoops (-1, LoopType.Incremental).SetRelative ();

	}

	void OnCollisionEnter (Collision collision)
	{
		if(collision.gameObject.layer == LayerMask.NameToLayer ("Player") || collision.gameObject.layer == LayerMask.NameToLayer ("Movables") && collision.gameObject.GetComponent<PlayersDeadCube> () != null)
		{
			if (collision.gameObject.GetComponent<Rigidbody> () == null)
				return;

			if (!touchingGameobjects.Contains (collision.gameObject))
			{
				touchingGameobjects.Add (collision.gameObject);

				if(eventEnabled)
				{
					onCollisionEnterEvents.Invoke ();
					menuButton.ShaderClickDuration ();
				}
			}
		}
	}

	void OnCollisionExit (Collision collision)
	{
		if (collision.gameObject.GetComponent<Rigidbody> () == null)
			return;
		
		if (touchingGameobjects.Contains (collision.gameObject))
			touchingGameobjects.Remove (collision.gameObject);
	}

	void OnTriggerEnter (Collider collider)
	{
		if (collider.GetComponent<Rigidbody> () == null)
			return;
		
		if(collider.gameObject.layer == LayerMask.NameToLayer ("Player") || collider.gameObject.layer == LayerMask.NameToLayer ("Movables") && collider.gameObject.GetComponent<PlayersDeadCube> () != null)
		{
			if (!touchingGameobjects.Contains (collider.gameObject))
			{
				touchingGameobjects.Add (collider.gameObject);

				if(eventEnabled)
				{
					onTriggerEnterEvents.Invoke ();
					menuButton.ShaderClickDuration ();
				}
			}
		}
	}

	void OnTriggerExit (Collider collider)
	{
		if (collider.GetComponent<Rigidbody> () == null)
			return;
		
		if (touchingGameobjects.Contains (collider.gameObject))
			touchingGameobjects.Remove (collider.gameObject);
	}
}
