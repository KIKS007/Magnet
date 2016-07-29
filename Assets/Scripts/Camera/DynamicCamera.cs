using UnityEngine;
using System.Collections;

public class DynamicCamera : MonoBehaviour 
{
	public float cameraZoomLerp = 0.5f;
	public float cameraMovementLerp = 0.5f;

	public float distanceMin = 30;

	public float largestDistance;

	public float yPos;

	private Vector3 centerPos = new Vector3(0, 0, 0);

	private Vector3 cameraOffset = new Vector3(0, 30, 0);

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		FindLargestDistance ();
		FindCenterPosition ();
		FindCameraPosition ();
		FindYPosition ();
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
		if(largestDistance > distanceMin)
			yPos = (largestDistance - distanceMin);
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

	void FindCameraPosition ()
	{
		if(centerPos != Vector3.zero)
		{
			//Movements Lerp
			transform.position = Vector3.Lerp(transform.position, centerPos, cameraMovementLerp);


			Vector3 newPos = transform.position;
			newPos.y = cameraOffset.y + yPos;

			//Zoom Lerp
			transform.position = Vector3.Lerp(transform.position, newPos, cameraZoomLerp);
		}


	}
}
