using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DynamicCamera : MonoBehaviour 
{
	public bool dynamicEnabled = true;

	public WhichMode whichMode = WhichMode.Default;

	public List<DynamicCameraSettings> modesSettingsList;


	[Header ("Current Camera Lerp")]
	public float cameraZoomLerp = 0.1f;
	public float cameraMovementLerp = 0.1f;

	[Header ("Current Zoom Settings")]
	public float cameraYMin = 15;
	public float distanceMin = 0;
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
		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing && dynamicEnabled)
		{
			FindLargestDistance ();
			FindCenterPosition ();
			FindYPosition ();
		}
	}

	void FixedUpdate ()
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing && dynamicEnabled)
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

	public void GetNewSettings ()
	{
		for(int i = 0; i < modesSettingsList.Count; i++)
		{
			if(modesSettingsList[i].whichMode == GlobalVariables.Instance.WhichModeLoaded)
			{
				cameraZoomLerp = modesSettingsList [i].cameraZoomLerp;
				cameraMovementLerp = modesSettingsList [i].cameraMovementLerp;

				cameraYMin = modesSettingsList [i].cameraYMin;
				distanceMin = modesSettingsList [i].distanceMin;
				distanceRatio = modesSettingsList [i].distanceRatio;

				break;
			}

			else
			{
				for(int y = 0; i < modesSettingsList.Count; i++)
				{
					if(modesSettingsList[y].whichMode == WhichMode.Default)
					{
						cameraZoomLerp = modesSettingsList [y].cameraZoomLerp;
						cameraMovementLerp = modesSettingsList [y].cameraMovementLerp;

						cameraYMin = modesSettingsList [y].cameraYMin;
						distanceMin = modesSettingsList [y].distanceMin;
						distanceRatio = modesSettingsList [y].distanceRatio;

						break;
					}
				}
			}
		}
	}
}

[Serializable]
public class DynamicCameraSettings
{
	public WhichMode whichMode = WhichMode.Default;

	[Header ("Camera Lerp")]
	public float cameraZoomLerp = 0.1f;
	public float cameraMovementLerp = 0.1f;

	[Header ("Zoom Settings")]
	public float cameraYMin = 15;
	public float distanceMin = 0;
	public float distanceRatio = 1;
}