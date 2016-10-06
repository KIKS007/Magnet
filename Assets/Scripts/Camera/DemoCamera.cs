using UnityEngine;
using System.Collections;

public class DemoCamera : MonoBehaviour 
{
	public KeyCode keyToActivate;

	public float rotationSpeed = 1;
	public Vector3 startPosition;
	public KeyCode keySloMo;

	private Transform target;
	private GameObject parent;

	private bool sloMo = false;
	private bool demoEnabled = false;

	// Use this for initialization
	void Start () 
	{
		
	}

	void StartDemo ()
	{
		target = GlobalVariables.Instance.Player1.GetComponent<Transform> ();

		parent = new GameObject ();
		parent.transform.position = target.position;
		transform.SetParent (parent.transform);
		transform.position = startPosition;
	}
	
	// Update is called once per frame
	void Update () 
	{
			if (!sloMo && Input.GetKeyDown (keySloMo))
			{
				sloMo = true;
				GetComponent<SlowMotionCamera> ().StartSlowMotion ();
				GetComponent<SlowMotionCamera> ().slowMoNumber = 5;

			}

			else if (sloMo && Input.GetKeyDown (keySloMo))
			{
				sloMo = false;
				GetComponent<SlowMotionCamera> ().StopSlowMotion ();
				GetComponent<SlowMotionCamera> ().slowMoNumber = 0;
			}


		if(Input.GetKeyDown (keyToActivate) && !demoEnabled)
		{
			GetComponent<DynamicCamera> ().enabled = false;
			StartDemo ();
			demoEnabled = true;
		}

		else if(Input.GetKeyDown (keyToActivate) && demoEnabled)
		{
			transform.rotation = Quaternion.Euler(new Vector3 (90, 0, 0));
			GetComponent<DynamicCamera> ().enabled = true;
			demoEnabled = false;
		}

	}

	void FixedUpdate ()
	{
		if(demoEnabled)
		{
			parent.transform.position = target.position;
			
			transform.LookAt (target);
			parent.transform.Rotate(Vector3.up * rotationSpeed);
		}
	}
}
