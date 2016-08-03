using UnityEngine;
using System.Collections;

public class DynamicCamera : MonoBehaviour 
{
	public bool enabled = true;

	[Header ("Camera Lerp")]
	public float cameraZoomLerp = 0.5f;
	public float cameraMovementLerp = 0.5f;

	[Header ("Zoom Settings")]
	public float cameraYMin = 30;
	public float distanceMin = 30;
	public float distanceRatio = 1;

	[Header ("Infos")]
	public float largestDistance;
	public float yPos;

	private Vector3 centerPos = new Vector3(0, 0, 0);

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		if(GlobalVariables.Instance.GameOver == false && GlobalVariables.Instance.GamePaused == false && enabled)
		{
			FindLargestDistance ();
			FindCenterPosition ();
			FindYPosition ();
		}
	}

	void FixedUpdate ()
	{
		if(GlobalVariables.Instance.GameOver == false && GlobalVariables.Instance.GamePaused == false && enabled)
		{
			SetCameraPosition ();
		}
	}

	void FindLargestDistance ()
	{
		if(GlobalVariables.Instance.EnabledPlayersList.Count > 1)
		{
			float distanceTemp = 0;

			if (Vector3.Distance (GlobalVariables.Instance.EnabledPlayersList [0].transform.position, GlobalVariables.Instance.EnabledPlayersList [1].transform.position) > distanceTemp)
				distanceTemp = Vector3.Distance (GlobalVariables.Instance.EnabledPlayersList [0].transform.position, GlobalVariables.Instance.EnabledPlayersList [1].transform.position);

			if(GlobalVariables.Instance.EnabledPlayersList.Count > 2)
			{
				if (Vector3.Distance (GlobalVariables.Instance.EnabledPlayersList [0].transform.position, GlobalVariables.Instance.EnabledPlayersList [2].transform.position) > distanceTemp)
					distanceTemp = Vector3.Distance (GlobalVariables.Instance.EnabledPlayersList [0].transform.position, GlobalVariables.Instance.EnabledPlayersList [2].transform.position);

				if (Vector3.Distance (GlobalVariables.Instance.EnabledPlayersList [1].transform.position, GlobalVariables.Instance.EnabledPlayersList [2].transform.position) > distanceTemp)
					distanceTemp = Vector3.Distance (GlobalVariables.Instance.EnabledPlayersList [1].transform.position, GlobalVariables.Instance.EnabledPlayersList [2].transform.position);
			}

			if(GlobalVariables.Instance.EnabledPlayersList.Count > 3)
			{
				if (Vector3.Distance (GlobalVariables.Instance.EnabledPlayersList [0].transform.position, GlobalVariables.Instance.EnabledPlayersList [3].transform.position) > distanceTemp)
					distanceTemp = Vector3.Distance (GlobalVariables.Instance.EnabledPlayersList [0].transform.position, GlobalVariables.Instance.EnabledPlayersList [3].transform.position);

				if (Vector3.Distance (GlobalVariables.Instance.EnabledPlayersList [1].transform.position, GlobalVariables.Instance.EnabledPlayersList [3].transform.position) > distanceTemp)
					distanceTemp = Vector3.Distance (GlobalVariables.Instance.EnabledPlayersList [1].transform.position, GlobalVariables.Instance.EnabledPlayersList [3].transform.position);

				if (Vector3.Distance (GlobalVariables.Instance.EnabledPlayersList [2].transform.position, GlobalVariables.Instance.EnabledPlayersList [3].transform.position) > distanceTemp)
					distanceTemp = Vector3.Distance (GlobalVariables.Instance.EnabledPlayersList [2].transform.position, GlobalVariables.Instance.EnabledPlayersList [3].transform.position);
			}

			largestDistance = distanceTemp;
		}			
	}

	void FindYPosition ()
	{
		if (largestDistance > distanceMin)
			yPos = (largestDistance * distanceRatio) - distanceMin;
		else
			yPos = 0;

		if(yPos < 0)
			yPos = 0;
	}

	void FindCenterPosition ()
	{
		Vector3 centerPosTemp = new Vector3 ();

		for (int i = 0; i < GlobalVariables.Instance.EnabledPlayersList.Count; i++)
			centerPosTemp += GlobalVariables.Instance.EnabledPlayersList [i].transform.position;

		centerPosTemp = centerPosTemp / GlobalVariables.Instance.EnabledPlayersList.Count;
		centerPosTemp.y = transform.position.y;

		centerPos = centerPosTemp;
	}

	void SetCameraPosition ()
	{
		if(centerPos != Vector3.zero)
		{
			//Movements Lerp
			transform.position = Vector3.Lerp(transform.position, centerPos, cameraMovementLerp);


			Vector3 newPos = transform.position;
			newPos.y = cameraYMin + yPos;

			//Zoom Lerp
			transform.position = Vector3.Lerp(transform.position, newPos, cameraZoomLerp);
		}


	}
}
