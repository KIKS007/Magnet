﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Replay;

public class DynamicCamera : MonoBehaviour
{
    public bool dynamicEnabled = true;

    [Header("Current Camera Lerp")]
    public List<GameObject> otherTargetsList = new List<GameObject>();
    public float cameraZoomLerp = 0.1f;
    public float cameraMovementLerp = 0.1f;

    [Header("Current Zoom Settings")]
    public Vector3 cameraOffset = new Vector3(0, 0, 0);
    public float cameraYMin = 15;
    public float distanceMin = 0;
    public float distanceRatio = 1;

    [Header("Infos")]
    public float largestDistance;
    public float yPos;

    private Vector3 centerPos = new Vector3(0, 0, 0);
    private float lerpFactor;

    public List<GameObject> targetsList = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        if (GlobalVariables.Instance.demoEnabled)
            return;

        if (ReplayManager.Instance.isReplaying)
            return;

        targetsList.Clear();

        targetsList.AddRange(GlobalVariables.Instance.AlivePlayersList);
//		targetsList.AddRange (GlobalVariables.Instance.EnabledPlayersList);

        for (int i = 0; i < otherTargetsList.Count; i++)
            if (otherTargetsList[i] != null && otherTargetsList[i].activeSelf == true)
                targetsList.Add(otherTargetsList[i]);
		

        if (dynamicEnabled)
        {
            if (targetsList.Count == 0)
                return;
			
            FindLargestDistance();
            FindCenterPosition();
            FindYPosition();
        }
    }

    void FixedUpdate()
    {
        if (GlobalVariables.Instance.demoEnabled)
            return;
        
        if (ReplayManager.Instance.isReplaying)
            return;
        
        if (GlobalVariables.Instance.GameState == GameStateEnum.Playing && dynamicEnabled)
        {
            if (targetsList.Count == 0)
                return;
			
            SetCameraPosition();
        }
    }

    void FindLargestDistance()
    {
        if (targetsList.Count > 1)
        {
            float distanceTemp = 0;

            for (int i = 0; i < targetsList.Count(); i++)
            {
                if (targetsList[i] == null)
                    return;
				
                for (int j = 0; j < targetsList.Count(); j++)
                {

                    if (Vector3.Distance(targetsList[i].transform.position, targetsList[j].transform.position) > distanceTemp)
                        distanceTemp = Vector3.Distance(targetsList[i].transform.position, targetsList[j].transform.position);
                }				
            }

            largestDistance = distanceTemp;
        }
    }

    void FindYPosition()
    {
        if (largestDistance > distanceMin)
            yPos = (largestDistance * distanceRatio) - distanceMin;
        else
            yPos = 0;

        if (yPos < 0)
            yPos = 0;
    }

    void FindCenterPosition()
    {
        Vector3 centerPosTemp = new Vector3();

        for (int i = 0; i < targetsList.Count(); i++)
        {
            if (targetsList[i] == null)
                return;
			
            centerPosTemp += targetsList[i].transform.position;
        }

        centerPosTemp = centerPosTemp / targetsList.Count();
        centerPosTemp.y = transform.position.y;

        centerPosTemp.x += cameraOffset.x;
        centerPosTemp.z += cameraOffset.z;

        centerPos = centerPosTemp;
    }

    void SetCameraPosition()
    {
        if (centerPos != Vector3.zero)
        {
            //Movements Lerp
            transform.position = Vector3.Lerp(transform.position, centerPos, cameraMovementLerp * Time.fixedDeltaTime);


            Vector3 newPos = transform.position;
            newPos.y = cameraYMin + yPos + cameraOffset.y;

            //Zoom Lerp
            transform.position = Vector3.Lerp(transform.position, newPos, cameraZoomLerp * Time.fixedDeltaTime);
        }
    }
}