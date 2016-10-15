using UnityEngine;
using System.Collections;
using Rewired;
using DG.Tweening;

public class VibrationManager : Singleton<VibrationManager> 
{
	public float[] playersLeftMotor = new float[5];
	public float[] playersRightMotor = new float[5];
	public int[] playersVibrationCount = new int[5];

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

	public float timeToStopVibration = 3;

	private bool applicationIsQuitting = false;

	// Use this for initialization
	void Start () 
	{
		gamepad1 = ReInput.players.GetPlayer (1);
		gamepad2 = ReInput.players.GetPlayer (2);
		gamepad3 = ReInput.players.GetPlayer (3);
		gamepad4 = ReInput.players.GetPlayer (4);

		GlobalVariables.Instance.OnGameOver += SlowlyStopVibration;
		GlobalVariables.Instance.OnPause += SlowlyStopVibration;
	}
	
	// Update is called once per frame
	void Update () 
	{
		foreach(Joystick j in gamepad1.controllers.Joysticks) 
		{
			if(!j.supportsVibration) continue;
			j.SetVibration(playersLeftMotor[1], playersRightMotor[1]);
		}

		foreach(Joystick j in gamepad2.controllers.Joysticks) 
		{
			if(!j.supportsVibration) continue;
			j.SetVibration(playersLeftMotor[2], playersRightMotor[2]);
		}

		foreach(Joystick j in gamepad3.controllers.Joysticks) 
		{
			if(!j.supportsVibration) continue;
			j.SetVibration(playersLeftMotor[3], playersRightMotor[3]);
		}

		foreach(Joystick j in gamepad4.controllers.Joysticks) 
		{
			if(!j.supportsVibration) continue;
			j.SetVibration(playersLeftMotor[4], playersRightMotor[4]);
		}

		if(test)
		{
			test = false;

			if(stopDurationTest == 0 && startDurationTest == 0 && burstNumberTest == 0)
				StartCoroutine (Vibration (1, leftMotorTest, rightMotorTest, durationTest));
			
			else if(burstNumberTest == 0)
				StartCoroutine (Vibration (1, leftMotorTest, rightMotorTest, durationTest, startDurationTest, stopDurationTest, easeTypeTest));

			else
				StartCoroutine (VibrationBurst (1, burstNumberTest, leftMotorTest, rightMotorTest, burstDurationTest, durationBetweenBurstTest));

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

		playersVibrationCount [whichPlayer]++;

		yield return new WaitForSeconds (duration);

		if(playersVibrationCount [whichPlayer] == 1)
			StopVibration (whichPlayer);

		playersVibrationCount [whichPlayer]--;

		yield return null;
	}

	IEnumerator Vibration (int whichPlayer, float leftMotor, float rightMotor, float duration, float startDuration, float stopDuration, Ease easeType = Ease.Linear)
	{
		Tween myTween = DOTween.To(()=> playersLeftMotor [whichPlayer], x=> playersLeftMotor [whichPlayer] = x, leftMotor, startDuration).SetEase(easeType).SetId("Vibration" + whichPlayer);
		DOTween.To(()=> playersRightMotor [whichPlayer], x=> playersRightMotor [whichPlayer] = x, rightMotor, startDuration).SetEase(easeType).SetId("Vibration" + whichPlayer);

		playersVibrationCount [whichPlayer]++;

		yield return myTween.WaitForCompletion ();
		yield return new WaitForSeconds (duration);

		myTween = DOTween.To(()=> playersLeftMotor [whichPlayer], x=> playersLeftMotor [whichPlayer] = x, 0, stopDuration).SetEase(easeType).SetId("Vibration" + whichPlayer);
		DOTween.To(()=> playersRightMotor [whichPlayer], x=> playersRightMotor [whichPlayer] = x, 0, stopDuration).SetEase(easeType).SetId("Vibration" + whichPlayer);

		yield return myTween.WaitForCompletion ();

		if(playersVibrationCount [whichPlayer] == 1)
			StopVibration (whichPlayer);

		playersVibrationCount [whichPlayer]--;

		yield return null;
	}

	IEnumerator VibrationBurst (int whichPlayer, int burstNumber, float leftMotor, float rightMotor, float burstDuration, float durationBetweenBurst)
	{
		for (int i = 0; i < burstNumber; i++)
		{
			playersLeftMotor [whichPlayer] = leftMotor;
			playersRightMotor [whichPlayer] = rightMotor;

			playersVibrationCount [whichPlayer]++;

			yield return new WaitForSeconds (burstDuration);

			if(playersVibrationCount [whichPlayer] == 1)
				StopVibration (whichPlayer);

			playersVibrationCount [whichPlayer]--;

			yield return new WaitForSeconds (durationBetweenBurst);
		}

		yield return null;
	}
		
	public void StopVibration (int whichPlayer)
	{
		if(!applicationIsQuitting)
		{
			playersLeftMotor [whichPlayer] = 0;
			playersRightMotor [whichPlayer] = 0;
			
			switch (whichPlayer)
			{
			case 0:
				foreach(Joystick j in gamepad1.controllers.Joysticks) 
				{
					if(!j.supportsVibration && j != null) continue;
					j.SetVibration(0, 0);
				}
				break;
			case 1:
				foreach(Joystick j in gamepad2.controllers.Joysticks) 
				{
					if(!j.supportsVibration && j != null) continue;
					j.SetVibration(0, 0);
				}
				break;
			case 2:
				foreach(Joystick j in gamepad3.controllers.Joysticks) 
				{
					if(!j.supportsVibration && j != null) continue;
					j.SetVibration(0, 0);
				}
				break;
			case 3:
				foreach(Joystick j in gamepad4.controllers.Joysticks) 
				{
					if(!j.supportsVibration && j != null) continue;
					j.SetVibration(0, 0);
				}
				break;
			}
		}
	}

	public void StopAllVibration ()
	{
		foreach(Joystick j in gamepad1.controllers.Joysticks) 
		{
			if(!j.supportsVibration && j != null) continue;
			j.SetVibration(0, 0);
		}

		foreach(Joystick j in gamepad2.controllers.Joysticks) 
		{
			if(!j.supportsVibration && j != null) continue;
			j.SetVibration(0, 0);
		}

		foreach(Joystick j in gamepad3.controllers.Joysticks) 
		{
			if(!j.supportsVibration && j != null) continue;
			j.SetVibration(0, 0);
		}

		foreach(Joystick j in gamepad4.controllers.Joysticks) 
		{
			if(!j.supportsVibration && j != null) continue;
			j.SetVibration(0, 0);
		}

		DOTween.Pause ("Vibration0");
		DOTween.Pause ("Vibration1");
		DOTween.Pause ("Vibration2");
		DOTween.Pause ("Vibration3");
	}

	void SlowlyStopVibration ()
	{
		DOTween.Pause ("Vibration0");
		DOTween.Pause ("Vibration1");
		DOTween.Pause ("Vibration2");
		DOTween.Pause ("Vibration3");

		DOTween.To(()=> playersLeftMotor [0], x=> playersLeftMotor [0] = x, 0, timeToStopVibration).SetId("Vibration" + 0);
		DOTween.To(()=> playersRightMotor [0], x=> playersRightMotor [0] = x, 0, timeToStopVibration).SetId("Vibration" + 0);

		DOTween.To(()=> playersLeftMotor [1], x=> playersLeftMotor [1] = x, 0, timeToStopVibration).SetId("Vibration" + 0);
		DOTween.To(()=> playersRightMotor [1], x=> playersRightMotor [1] = x, 0, timeToStopVibration).SetId("Vibration" + 0);

		DOTween.To(()=> playersLeftMotor [2], x=> playersLeftMotor [2] = x, 0, timeToStopVibration).SetId("Vibration" + 0);
		DOTween.To(()=> playersRightMotor [2], x=> playersRightMotor [2] = x, 0, timeToStopVibration).SetId("Vibration" + 0);

		DOTween.To(()=> playersLeftMotor [3], x=> playersLeftMotor [3] = x, 0, timeToStopVibration).SetId("Vibration" + 0);
		DOTween.To(()=> playersRightMotor [3], x=> playersRightMotor[3] = x, 0, timeToStopVibration).SetId("Vibration" + 0);

	}

	void OnApplicationQuit ()
	{
		applicationIsQuitting = true;

		StopAllVibration ();
	}
}
