using UnityEngine;
using System.Collections;

public class SlowMotionTriggerScript : MonoBehaviour 
{
	public LayerMask playerLayerMask;
	public bool triggerEnabled = false;

	[HideInInspector]
	public GameObject playerThatThrew;

	private SlowMotionCamera slowMotionCamera;

	void Start ()
	{
		slowMotionCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<SlowMotionCamera>();
	}

	void OnTriggerEnter (Collider other)
	{
		if(triggerEnabled && (1<<other.gameObject.layer & playerLayerMask) != 0)
		{
			playerThatThrew = transform.parent.GetComponent<MovableScript>().playerThatThrew;
			
			if(triggerEnabled && other.tag == "Player" && other.gameObject != playerThatThrew)
			{
				triggerEnabled = false;
				
				slowMotionCamera.StartSlowMotion ();
				slowMotionCamera.ContrastVignette(transform.position);
			}			
		}
	}
}
