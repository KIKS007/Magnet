using UnityEngine;
using System.Collections;

public class SlowMotionTriggerScript : MonoBehaviour 
{
	public bool triggerEnabled = false;

	[HideInInspector]
	public GameObject playerThatThrew;

	void OnTriggerStay (Collider other)
	{
		playerThatThrew = transform.parent.GetComponent<MovableScript>().playerThatThrew;

		if(triggerEnabled && other.tag == "Player" && other.gameObject != playerThatThrew)
		{
			triggerEnabled = false;

			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().StartSlowMotion ();
		}

		if (triggerEnabled && transform.parent.tag != "ThrownMovable")
			triggerEnabled = false;
	}
}
