using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;

public enum SlowMotionType {Default, Death, Stun, DashStun, ModeEnd};

public class ScreenShakeCamera : MonoBehaviour 
{

	public List<SlowMotionSettings> slowMotionList = new List<SlowMotionSettings> ();

	[Header ("Common Settings")]
	public int shakeVibrato = 100;
	public float shakeRandomness = 45;

	[Header ("Test")]
	public SlowMotionType whichSlowMoTest = SlowMotionType.Default;
	public bool shake;
	public bool resetShake;

	private Vector3 initialRotation;

	// Use this for initialization
	void Start () 
	{
		initialRotation = new Vector3 (90, 45, 45);
		//initialRotation = transform.rotation.eulerAngles;

		LoadModeManager.Instance.OnLevelLoaded += ResetCameraRotation;
		ResetCameraRotation ();
	}

	// Update is called once per frame
	void Update () 
	{
		if(shake)
		{
			shake = false;
			CameraShaking(whichSlowMoTest);
		}

		if(resetShake)
		{
			resetShake = false;
			ResetCameraRotation();
		}
	}

	public void CameraShaking (SlowMotionType whichSlowMo = SlowMotionType.Default)
	{
		float shakeDuration = 0;
		Vector3 shakeStrenth = Vector3.zero;
		bool exactType = true;

		for(int i = 0; i < slowMotionList.Count; i++)
		{
			if(slowMotionList[i].whichSlowMo == whichSlowMo)
			{
				shakeDuration = slowMotionList [i].shakeDuration;
				shakeStrenth = slowMotionList [i].shakeStrenth;
				exactType = true;
				break;
			}
		}

		if(!exactType)
		{
			shakeDuration = slowMotionList [0].shakeDuration;
			shakeStrenth = slowMotionList [0].shakeStrenth;
		}

		shake = false;
		transform.DOShakeRotation (shakeDuration, shakeStrenth, shakeVibrato, shakeRandomness).SetId("ScreenShake").OnComplete (EndOfShake);
	}

	void EndOfShake ()
	{
		if(!DOTween.IsTweening("ScreenShake"))
		{
			ResetCameraRotation ();
		}
	}

	void ResetCameraRotation ()
	{
//		Debug.Log ("Rotation : " + transform.rotation.eulerAngles);
		transform.DORotate(initialRotation, 0.5f);
	}
	
}

[Serializable]
public class SlowMotionSettings
{
	public SlowMotionType whichSlowMo = SlowMotionType.Default;

	public float shakeDuration = 0.5f;
	public Vector3 shakeStrenth = new Vector3 (1, 0, 1);
}
