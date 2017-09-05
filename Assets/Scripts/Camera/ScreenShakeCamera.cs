using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;

public enum FeedbackType {Default, Death, Stun, DashStun, ModeEnd, Startup, Hold, Shoot, Dash, Wave, ButtonClick};

public class ScreenShakeCamera : MonoBehaviour 
{
	public List<SlowMotionSettings> screenShakeList = new List<SlowMotionSettings> ();

	[Header ("Common Settings")]
	public int shakeVibrato = 100;
	public float shakeRandomness = 45;

	[Header ("Test")]
	public FeedbackType whichScreenShakeTest = FeedbackType.Default;
	public bool shake;
	public bool resetShake;

	// Update is called once per frame
	void Update () 
	{
		if(shake)
		{
			shake = false;
			CameraShaking(whichScreenShakeTest);
		}

		if(resetShake)
		{
			resetShake = false;
			ResetCameraRotation();
		}
	}

	public void CameraShaking (FeedbackType whichSlowMo = FeedbackType.Default)
	{
		if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
			return;

		float shakeDuration = 0;
		Vector3 shakeStrenth = Vector3.zero;
		bool exactType = true;

		for(int i = 0; i < screenShakeList.Count; i++)
		{
			if(screenShakeList[i].whichScreenShake == whichSlowMo)
			{
				shakeDuration = screenShakeList [i].shakeDuration;
				shakeStrenth = screenShakeList [i].shakeStrenth;
				exactType = true;
				break;
			}
		}

		if(!exactType)
		{
			shakeDuration = screenShakeList [0].shakeDuration;
			shakeStrenth = screenShakeList [0].shakeStrenth;
		}

		shake = false;

		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing)
			transform.DOShakeRotation (shakeDuration, shakeStrenth, shakeVibrato, shakeRandomness).SetId("ScreenShake").OnComplete (ResetCameraRotation).SetUpdate (false);
	}

	void ResetCameraRotation ()
	{
		//Debug.Log ("Rotation : " + transform.rotation.eulerAngles);
		if (DOTween.IsTweening ("ScreenShake"))
			return;

		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing)
			transform.DORotate(new Vector3 (90.0f, 0.0f, 0.0f), 1f, RotateMode.Fast).SetId("ScreenShake").SetUpdate (false);
	}
	
}

[Serializable]
public class SlowMotionSettings
{
	public FeedbackType whichScreenShake = FeedbackType.Default;

	public float shakeDuration = 0.5f;
	public Vector3 shakeStrenth = new Vector3 (1, 0, 1);
}
