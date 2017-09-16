using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialColliderEventTrigger : MonoBehaviour 
{
	public bool eventEnabled = true;

	[Header ("OnCollisionEnter")]
	public UnityEvent onCollisionEnterEvents;

	[Header ("OnTriggerEnter")]
	public UnityEvent onTriggerEnterEvents;

	[HideInInspector]
	public List<GameObject> touchingGameobjects = new List<GameObject> ();

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
					onCollisionEnterEvents.Invoke ();
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
					onTriggerEnterEvents.Invoke ();
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
