﻿using UnityEngine;
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

	private GameObject[] playersList;	 

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		playersList = GameObject.FindGameObjectsWithTag("Player");

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
		if(playersList.Length > 1)
		{
			float distanceTemp = 0;

			if (Vector3.Distance (playersList [0].transform.position, playersList [1].transform.position) > distanceTemp)
				distanceTemp = Vector3.Distance (playersList [0].transform.position, playersList [1].transform.position);

			if(playersList.Length > 2)
			{
				if (Vector3.Distance (playersList [0].transform.position, playersList [2].transform.position) > distanceTemp)
					distanceTemp = Vector3.Distance (playersList [0].transform.position, playersList [2].transform.position);

				if (Vector3.Distance (playersList [1].transform.position, playersList [2].transform.position) > distanceTemp)
					distanceTemp = Vector3.Distance (playersList [1].transform.position, playersList [2].transform.position);
			}

			if(playersList.Length > 3)
			{
				if (Vector3.Distance (playersList [0].transform.position, playersList [3].transform.position) > distanceTemp)
					distanceTemp = Vector3.Distance (playersList [0].transform.position, playersList [3].transform.position);

				if (Vector3.Distance (playersList [1].transform.position, playersList [3].transform.position) > distanceTemp)
					distanceTemp = Vector3.Distance (playersList [1].transform.position, playersList [3].transform.position);

				if (Vector3.Distance (playersList [2].transform.position, playersList [3].transform.position) > distanceTemp)
					distanceTemp = Vector3.Distance (playersList [2].transform.position, playersList [3].transform.position);
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

		for (int i = 0; i < playersList.Length; i++)
			centerPosTemp += playersList [i].transform.position;

		centerPosTemp = centerPosTemp / playersList.Length;
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