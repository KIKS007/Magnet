using UnityEngine;
using System.Collections;

public class SlowMotionTriggerScript : MonoBehaviour 
{
	public float timeTween = 0.1f;

	public bool triggerEnabled;

	[HideInInspector]
	public Rigidbody holdMovable;
	[HideInInspector]
	public GameObject playerThatThrew;


	void OnTriggerStay (Collider other)
	{
		playerThatThrew = transform.parent.GetComponent<MovableScript>().playerThatThrew;

		if(other.tag == "Player" && triggerEnabled && other.name != playerThatThrew.name)
		{
			triggerEnabled = false;

			//GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().slowMotion = true;
			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().StartSlowMotion ();


			//Debug.Log("Touching" + other.name);
		}
	}
}
