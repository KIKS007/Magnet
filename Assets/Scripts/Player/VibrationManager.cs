using UnityEngine;
using System.Collections;
using Rewired;
using DG.Tweening;

public class VibrationManager : Singleton<VibrationManager> 
{
	public float[] playersLeftMotor = new float[4];
	public float[] playersRightMotor = new float[4];

	private Player gamepad1;
	private Player gamepad2;
	private Player gamepad3;
	private Player gamepad4;

	[Header ("Debug Test")]
	public bool test = false;
	[Range (0, 1)]
	public float leftMotorTest;
	[Range (0, 1)]
	public float rightMotorTest;
	public float durationTest;
	public float startDurationTest;
	public float stopDurationTest;
	public Ease easeTypeTest;
	public int burstNumberTest;
	public float burstDurationTest;
	public float durationBetweenBurstTest;

	// Use this for initialization
	void Start () 
	{
		gamepad1 = ReInput.players.GetPlayer (0);
		gamepad2 = ReInput.players.GetPlayer (1);
		gamepad3 = ReInput.players.GetPlayer (2);
		gamepad4 = ReInput.players.GetPlayer (3);
	}
	
	// Update is called once per frame
	void Update () 
	{
		foreach(Joystick j in gamepad1.controllers.Joysticks) 
		{
			if(!j.supportsVibration) continue;
			j.SetVibration(playersLeftMotor[0], playersRightMotor[0]);
		}

		foreach(Joystick j in gamepad2.controllers.Joysticks) 
		{
			if(!j.supportsVibration) continue;
			j.SetVibration(playersLeftMotor[1], playersRightMotor[1]);
		}

		foreach(Joystick j in gamepad3.controllers.Joysticks) 
		{
			if(!j.supportsVibration) continue;
			j.SetVibration(playersLeftMotor[2], playersRightMotor[2]);
		}

		foreach(Joystick j in gamepad4.controllers.Joysticks) 
		{
			if(!j.supportsVibration) continue;
			j.SetVibration(playersLeftMotor[3], playersRightMotor[3]);
		}

		if(test)
		{
			test = false;

			if(stopDurationTest == 0 && startDurationTest == 0 && burstNumberTest == 0)
				StartCoroutine (Vibration (0, leftMotorTest, rightMotorTest, durationTest));
			
			else if(burstNumberTest == 0)
				StartCoroutine (Vibration (0, leftMotorTest, rightMotorTest, durationTest, startDurationTest, stopDurationTest, easeTypeTest));

			else
				StartCoroutine (VibrationBurst (0, burstNumberTest, leftMotorTest, rightMotorTest, burstDurationTest, durationBetweenBurstTest));

		}
	}

	public void Vibrate (int whichPlayer, float leftMotor, float rightMotor, float duration)
	{
		StartCoroutine (Vibration (whichPlayer, leftMotor, rightMotor, duration));
	}

	public void Vibrate (int whichPlayer, float leftMotor, float rightMotor, float duration, float startDuration, float stopDuration, Ease easeType = Ease.Linear)
	{
		StartCoroutine (Vibration (whichPlayer, leftMotor, rightMotor, duration, startDuration, stopDuration, easeType));
	}

	public void VibrateBurst (int whichPlayer, int burstNumber, float leftMotor, float rightMotor, float burstDuration, float durationBetweenBurst)
	{
		StartCoroutine (VibrationBurst (whichPlayer, burstNumber, leftMotor, rightMotor, burstDuration, durationBetweenBurst));
	}

	IEnumerator Vibration (int whichPlayer, float leftMotor, float rightMotor, float duration)
	{
		playersLeftMotor [whichPlayer] = leftMotor;
		playersRightMotor [whichPlayer] = rightMotor;

		yield return StartCoroutine (CoroutineUtil.WaitForRealSeconds (duration));

		StopVibration (whichPlayer);

		yield return null;
	}

	IEnumerator Vibration (int whichPlayer, float leftMotor, float rightMotor, float duration, float startDuration, float stopDuration, Ease easeType = Ease.Linear)
	{
		Tween myTween = DOTween.To(()=> playersLeftMotor [whichPlayer], x=> playersLeftMotor [whichPlayer] = x, leftMotor, startDuration).SetEase(easeType).SetId("Vibration" + whichPlayer);
		DOTween.To(()=> playersRightMotor [whichPlayer], x=> playersRightMotor [whichPlayer] = x, rightMotor, startDuration).SetEase(easeType).SetId("Vibration" + whichPlayer);

		yield return myTween.WaitForCompletion ();
		yield return StartCoroutine (CoroutineUtil.WaitForRealSeconds (duration));

		myTween = DOTween.To(()=> playersLeftMotor [whichPlayer], x=> playersLeftMotor [whichPlayer] = x, 0, stopDuration).SetEase(easeType).SetId("Vibration" + whichPlayer);
		DOTween.To(()=> playersRightMotor [whichPlayer], x=> playersRightMotor [whichPlayer] = x, 0, stopDuration).SetEase(easeType).SetId("Vibration" + whichPlayer);

		yield return myTween.WaitForCompletion ();

		StopVibration (whichPlayer);

		yield return null;
	}

	IEnumerator VibrationBurst (int whichPlayer, int burstNumber, float leftMotor, float rightMotor, float burstDuration, float durationBetweenBurst)
	{
		for (int i = 0; i < burstNumber; i++)
		{
			playersLeftMotor [whichPlayer] = leftMotor;
			playersRightMotor [whichPlayer] = rightMotor;

			yield return StartCoroutine (CoroutineUtil.WaitForRealSeconds (burstDuration));

			StopVibration (whichPlayer);

			yield return StartCoroutine (CoroutineUtil.WaitForRealSeconds (durationBetweenBurst));
		}

		yield return null;
	}
		
	void StopVibration (int whichPlayer)
	{
		playersLeftMotor [whichPlayer] = 0;
		playersRightMotor [whichPlayer] = 0;

		switch (whichPlayer)
		{
		case 0:
			foreach(Joystick j in gamepad1.controllers.Joysticks) 
			{
				if(!j.supportsVibration) continue;
				j.SetVibration(0, 0);
			}
			break;
		case 1:
			foreach(Joystick j in gamepad2.controllers.Joysticks) 
			{
				if(!j.supportsVibration) continue;
				j.SetVibration(0, 0);
			}
			break;
		case 2:
			foreach(Joystick j in gamepad3.controllers.Joysticks) 
			{
				if(!j.supportsVibration) continue;
				j.SetVibration(0, 0);
			}
			break;
		case 3:
			foreach(Joystick j in gamepad4.controllers.Joysticks) 
			{
				if(!j.supportsVibration) continue;
				j.SetVibration(0, 0);
			}
			break;
		}
	}

	void StopAllVibration ()
	{
		foreach(Joystick j in gamepad1.controllers.Joysticks) 
		{
			if(!j.supportsVibration) continue;
			j.SetVibration(0, 0);
		}

		foreach(Joystick j in gamepad2.controllers.Joysticks) 
		{
			if(!j.supportsVibration) continue;
			j.SetVibration(0, 0);
		}

		foreach(Joystick j in gamepad3.controllers.Joysticks) 
		{
			if(!j.supportsVibration) continue;
			j.SetVibration(0, 0);
		}

		foreach(Joystick j in gamepad4.controllers.Joysticks) 
		{
			if(!j.supportsVibration) continue;
			j.SetVibration(0, 0);
		}

		DOTween.Pause ("Vibration0");
		DOTween.Pause ("Vibration1");
		DOTween.Pause ("Vibration2");
		DOTween.Pause ("Vibration3");
	}

	void OnLevelWasLoaded ()
	{
		StopAllVibration ();
	}

	void OnApplicationQuit ()
	{
		StopAllVibration ();
	}
}
