using UnityEngine;
using System.Collections;

public class SlowMotionTriggerScript : MonoBehaviour 
{
	public LayerMask playerLayerMask;
	public bool triggerEnabled = false;

	[HideInInspector]
	public GameObject playerThatThrew;

	void OnTriggerEnter (Collider other)
	{
		if(triggerEnabled && (1<<other.gameObject.layer & playerLayerMask) != 0)
		{
			playerThatThrew = transform.parent.GetComponent<MovableScript>().playerThatThrew;
			
			if(triggerEnabled && other.tag == "Player" && other.gameObject != playerThatThrew)
			{
				triggerEnabled = false;
				
				GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().StartSlowMotion ();
				GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().ContrastVignette(transform.position);
			}			
		}
	}
}
