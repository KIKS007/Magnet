using UnityEngine;
using System.Collections;

public class ProbesPlacement : MonoBehaviour 
{
	public Transform forwardWall;
	public Transform backwardWall;
	public Transform rightWall;
	public Transform leftWall;

	public Transform forwardProbe;
	public Transform backwardProbe;
	public Transform rightProbe;
	public Transform leftProbe;

	public bool followCamera = false;

	private GameObject mainCamera;

	private Vector3 forwardProbePosition;
	private Vector3 backwardProbePosition;
	private Vector3 rightProbePosition;
	private Vector3 leftProbePosition;

	// Use this for initialization
	void Start () 
	{
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

		GetReflectionGameObjects ();

		if(LoadModeManager.mirrorForward != null)
		{
			forwardProbePosition = mainCamera.transform.position;
			backwardProbePosition = mainCamera.transform.position;
			rightProbePosition = mainCamera.transform.position;
			leftProbePosition = mainCamera.transform.position;

			forwardProbePosition.z = forwardWall.position.z + (forwardWall.position.z - mainCamera.transform.position.z);
			backwardProbePosition.z = backwardWall.position.z + (backwardWall.position.z - mainCamera.transform.position.z);
			rightProbePosition.x = rightWall.position.x + (rightWall.position.x - mainCamera.transform.position.x);
			leftProbePosition.x = leftWall.position.x + (leftWall.position.x - mainCamera.transform.position.x);

			forwardProbe.transform.position = forwardProbePosition;
			backwardProbe.transform.position = backwardProbePosition;
			rightProbe.transform.position = rightProbePosition;
			leftProbe.transform.position = leftProbePosition;
		}
	}

	public void GetReflectionGameObjects ()
	{
		if(LoadModeManager.mirrorForward != null)
		{
			forwardWall = LoadModeManager.mirrorForward.transform;
			backwardWall = LoadModeManager.mirrorBackward.transform;
			rightWall = LoadModeManager.mirrorRight.transform;
			leftWall = LoadModeManager.mirrorLeft.transform;

			forwardProbe = LoadModeManager.probeForward.transform;
			backwardProbe = LoadModeManager.probeBackward.transform;
			rightProbe = LoadModeManager.probeRight.transform;
			leftProbe = LoadModeManager.probeLeft.transform;
		}


	}
	
	// Update is called once per frame
	void Update () 
	{
		if(followCamera && LoadModeManager.mirrorForward != null)
		{
			forwardProbePosition = mainCamera.transform.position;
			backwardProbePosition = mainCamera.transform.position;
			rightProbePosition = mainCamera.transform.position;
			leftProbePosition = mainCamera.transform.position;
			
			forwardProbePosition.z = forwardWall.position.z + (forwardWall.position.z - mainCamera.transform.position.z);
			backwardProbePosition.z = backwardWall.position.z + (backwardWall.position.z - mainCamera.transform.position.z);
			rightProbePosition.x = rightWall.position.x + (rightWall.position.x - mainCamera.transform.position.x);
			leftProbePosition.x = leftWall.position.x + (leftWall.position.x - mainCamera.transform.position.x);
			
			forwardProbe.transform.position = forwardProbePosition;
			backwardProbe.transform.position = backwardProbePosition;
			rightProbe.transform.position = rightProbePosition;
			leftProbe.transform.position = leftProbePosition;
		}
	}
}
