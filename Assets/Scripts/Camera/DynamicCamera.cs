using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class DynamicCamera : MonoBehaviour 
{
	public bool dynamicEnabled = true;

	public WhichMode whichMode = WhichMode.Default;

	public List<DynamicCameraSettings> modesSettingsList;


	[Header ("Current Camera Lerp")]
	public List<GameObject> otherTargetsList = new List<GameObject>();	
	public float cameraZoomLerp = 0.1f;
	public float cameraMovementLerp = 0.1f;

	[Header ("Current Zoom Settings")]
	public Vector3 cameraOffset = new Vector3(0, 0, 0);
	public float cameraYMin = 15;
	public float distanceMin = 0;
	public float distanceRatio = 1;

	[Header ("Infos")]
	public float largestDistance;
	public float yPos;

	private Vector3 centerPos = new Vector3(0, 0, 0);

	public List<GameObject> targetsList = new List<GameObject>();	

	void Awake ()
	{
		LoadModeManager.Instance.OnLevelLoaded += GetNewSettings;
	}

	// Update is called once per frame
	void Update () 
	{
		targetsList.Clear ();

		targetsList.AddRange (GlobalVariables.Instance.AlivePlayersList);
//		targetsList.AddRange (GlobalVariables.Instance.EnabledPlayersList);

		for (int i = 0; i < otherTargetsList.Count; i++)
			if(otherTargetsList[i] != null && otherTargetsList[i].activeSelf == true)
				targetsList.Add (otherTargetsList [i]);
		

		if(dynamicEnabled)
		{
			if (targetsList.Count == 0)
				return;
			
			FindLargestDistance ();
			FindCenterPosition ();
			FindYPosition ();
		}
	}

	void FixedUpdate () 
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing && dynamicEnabled)
		{
			if (targetsList.Count == 0)
				return;
			
			SetCameraPosition ();
		}
	}

	void FindLargestDistance ()
	{
		if(targetsList.Count > 1)
		{
			float distanceTemp = 0;

			for(int i = 0; i < targetsList.Count (); i++)
			{
				if (targetsList [i] == null)
					return;
				
				for(int j = 0; j < targetsList.Count (); j++)
				{

					if (Vector3.Distance (targetsList [i].transform.position, targetsList [j].transform.position) > distanceTemp)
						distanceTemp = Vector3.Distance (targetsList [i].transform.position, targetsList [j].transform.position);
				}				
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

		for (int i = 0; i < targetsList.Count (); i++)
		{
			if (targetsList [i] == null)
				return;
			
			centerPosTemp += targetsList [i].transform.position;
		}

		centerPosTemp = centerPosTemp / targetsList.Count ();
		centerPosTemp.y = transform.position.y;

		centerPosTemp.x += cameraOffset.x;
		centerPosTemp.z += cameraOffset.z;

		centerPos = centerPosTemp;
	}

	void SetCameraPosition ()
	{
		if(centerPos != Vector3.zero)
		{
			//Movements Lerp
			transform.position = Vector3.Lerp(transform.position, centerPos, cameraMovementLerp);


			Vector3 newPos = transform.position;
			newPos.y = cameraYMin + yPos + cameraOffset.y;

			//Zoom Lerp
			transform.position = Vector3.Lerp(transform.position, newPos, cameraZoomLerp);
		}
	}

	public void GetNewSettings ()
	{
		bool exactSettings = false;

		for(int i = 0; i < modesSettingsList.Count; i++)
		{
			if(modesSettingsList[i].whichMode == GlobalVariables.Instance.CurrentModeLoaded)
			{
				cameraZoomLerp = modesSettingsList [i].cameraZoomLerp;
				cameraMovementLerp = modesSettingsList [i].cameraMovementLerp;

				cameraYMin = modesSettingsList [i].cameraYMin;
				distanceMin = modesSettingsList [i].distanceMin;
				distanceRatio = modesSettingsList [i].distanceRatio;
				exactSettings = true;
				break;
			}
		}

		if(!exactSettings)
		{
			cameraZoomLerp = modesSettingsList [0].cameraZoomLerp;
			cameraMovementLerp = modesSettingsList [0].cameraMovementLerp;

			cameraYMin = modesSettingsList [0].cameraYMin;
			distanceMin = modesSettingsList [0].distanceMin;
			distanceRatio = modesSettingsList [0].distanceRatio;
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