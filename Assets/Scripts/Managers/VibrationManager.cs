using UnityEngine;
using System.Collections;
using Rewired;
using DG.Tweening;
using System;
using System.Collections.Generic;

public class VibrationManager : Singleton<VibrationManager>
{
    public bool vibrationEnabled = true;
    public List<VibrationSettings> vibrationList = new List<VibrationSettings>();

    [Header("Vibration Debug")]
    public float[] playersLeftMotor = new float[5];
    public float[] playersRightMotor = new float[5];
    public int[] playersVibrationCount = new int[5];

    private Player gamepad1;
    private Player gamepad2;
    private Player gamepad3;
    private Player gamepad4;

    [Header("Test")]
    public bool test = false;
    public FeedbackType whichFeedbackTest = FeedbackType.Default;

    private float timeToStopVibration = 1;

    // Use this for initialization
    void Start()
    {
        gamepad1 = ReInput.players.GetPlayer(1);
        gamepad2 = ReInput.players.GetPlayer(2);
        gamepad3 = ReInput.players.GetPlayer(3);
        gamepad4 = ReInput.players.GetPlayer(4);

        GlobalVariables.Instance.OnEndMode += SlowlyStopVibration;
        GlobalVariables.Instance.OnMenu += SlowlyStopVibration;
        GlobalVariables.Instance.OnPause += SlowlyStopVibration;
    }
	
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < GlobalVariables.Instance.rewiredPlayers.Length; i++)
        {
            if (!vibrationEnabled)
                return;

            if (GlobalVariables.Instance.rewiredPlayers[i] == null)
                continue;

            if (GlobalVariables.Instance.rewiredPlayers[i].controllers.joystickCount != 0 && GlobalVariables.Instance.rewiredPlayers[i].controllers.Joysticks[0].supportsVibration)
                GlobalVariables.Instance.rewiredPlayers[i].controllers.Joysticks[0].SetVibration(playersLeftMotor[i], playersRightMotor[i]);
        }

        if (test)
        {
            test = false;
            Vibrate(1, whichFeedbackTest);
        }
    }

    public void Vibrate(int whichPlayer, FeedbackType whichVibration)
    {
        if (!vibrationEnabled)
            return;
		
        float leftMotorForce = 0;
        float rightMotorForce = 0;
        float vibrationDuration = 0;

        bool exactType = true;

        for (int i = 0; i < vibrationList.Count; i++)
        {
            if (vibrationList[i].whichVibration == whichVibration)
            {
                leftMotorForce = vibrationList[i].leftMotorForce;
                rightMotorForce = vibrationList[i].rightMotorForce;
                vibrationDuration = vibrationList[i].vibrationDuration;
                exactType = true;
                break;
            }
        }

        if (!exactType)
        {
            leftMotorForce = vibrationList[0].leftMotorForce;
            rightMotorForce = vibrationList[0].rightMotorForce;
            vibrationDuration = vibrationList[0].vibrationDuration;
        }

        Vibrate(whichPlayer, leftMotorForce, rightMotorForce, vibrationDuration);
    }

    public void Vibrate(int whichPlayer, float leftMotor, float rightMotor, float duration)
    {
        if (!vibrationEnabled)
            return;
		
        StartCoroutine(Vibration(whichPlayer, leftMotor, rightMotor, duration));
    }

    public void Vibrate(int whichPlayer, float leftMotor, float rightMotor, float duration, float startDuration, float stopDuration, Ease easeType = Ease.Linear)
    {
        if (!vibrationEnabled)
            return;
		
        StartCoroutine(Vibration(whichPlayer, leftMotor, rightMotor, duration, startDuration, stopDuration, easeType));
    }

    public void VibrateBurst(int whichPlayer, int burstNumber, float leftMotor, float rightMotor, float burstDuration, float durationBetweenBurst)
    {
        if (!vibrationEnabled)
            return;
		
        StartCoroutine(VibrationBurst(whichPlayer, burstNumber, leftMotor, rightMotor, burstDuration, durationBetweenBurst));
    }

    IEnumerator Vibration(int whichPlayer, float leftMotor, float rightMotor, float duration)
    {
        DOTween.Kill("Vibration" + whichPlayer);

        playersLeftMotor[whichPlayer] = leftMotor;
        playersRightMotor[whichPlayer] = rightMotor;

        playersVibrationCount[whichPlayer]++;

        if (duration > 0)
            yield return new WaitForSecondsRealtime(duration);

        if (playersVibrationCount[whichPlayer] == 1)
            StopVibration(whichPlayer);

        playersVibrationCount[whichPlayer]--;

        yield break;
    }

    IEnumerator Vibration(int whichPlayer, float leftMotor, float rightMotor, float duration, float startDuration, float stopDuration, Ease easeType = Ease.Linear)
    {
        DOTween.Kill("Vibration" + whichPlayer);

        Tween myTween = DOTween.To(() => playersLeftMotor[whichPlayer], x => playersLeftMotor[whichPlayer] = x, leftMotor, startDuration).SetEase(easeType).SetId("Vibration" + whichPlayer).SetUpdate(false);
        DOTween.To(() => playersRightMotor[whichPlayer], x => playersRightMotor[whichPlayer] = x, rightMotor, startDuration).SetEase(easeType).SetId("Vibration" + whichPlayer).SetUpdate(false);

        playersVibrationCount[whichPlayer]++;

        yield return myTween.WaitForCompletion();

        if (duration > 0)
            yield return new WaitForSecondsRealtime(duration);

        myTween = DOTween.To(() => playersLeftMotor[whichPlayer], x => playersLeftMotor[whichPlayer] = x, 0, stopDuration).SetEase(easeType).SetId("Vibration" + whichPlayer).SetUpdate(false);
        DOTween.To(() => playersRightMotor[whichPlayer], x => playersRightMotor[whichPlayer] = x, 0, stopDuration).SetEase(easeType).SetId("Vibration" + whichPlayer).SetUpdate(false);

        yield return myTween.WaitForCompletion();

        if (playersVibrationCount[whichPlayer] == 1)
            StopVibration(whichPlayer);

        playersVibrationCount[whichPlayer]--;
    }

    IEnumerator VibrationBurst(int whichPlayer, int burstNumber, float leftMotor, float rightMotor, float burstDuration, float durationBetweenBurst)
    {
        DOTween.Kill("Vibration" + whichPlayer);

        for (int i = 0; i < burstNumber; i++)
        {
            playersLeftMotor[whichPlayer] = leftMotor;
            playersRightMotor[whichPlayer] = rightMotor;

            playersVibrationCount[whichPlayer]++;

            if (burstDuration > 0)
                yield return new WaitForSecondsRealtime(burstDuration);

            if (playersVibrationCount[whichPlayer] == 1)
                StopVibration(whichPlayer);

            playersVibrationCount[whichPlayer]--;

            if (durationBetweenBurst > 0)
                yield return new WaitForSecondsRealtime(durationBetweenBurst);
        }

        yield break;
    }

    public void StopVibration(int whichPlayer)
    {
        if (!GlobalVariables.applicationIsQuitting)
        {
            playersLeftMotor[whichPlayer] = 0;
            playersRightMotor[whichPlayer] = 0;
			
            switch (whichPlayer)
            {
                case 0:
                    if (gamepad1 != null)
                    {
                        foreach (Joystick j in gamepad1.controllers.Joysticks)
                        {
                            if (!j.supportsVibration && j != null)
                                continue;
                            j.SetVibration(0, 0);
                        }
                        
                    }

                    break;
                case 1:

                    if (gamepad2 != null)
                    {
                        
                        foreach (Joystick j in gamepad2.controllers.Joysticks)
                        {
                            if (!j.supportsVibration && j != null)
                                continue;
                            j.SetVibration(0, 0);
                        }
                    }

                    break;
                case 2:

                    if (gamepad3 != null)
                    {
                        foreach (Joystick j in gamepad3.controllers.Joysticks)
                        {
                            if (!j.supportsVibration && j != null)
                                continue;
                            j.SetVibration(0, 0);
                        }
                    }

                    break;
                case 3:
                    
                    if (gamepad4 != null)
                    {
                        foreach (Joystick j in gamepad4.controllers.Joysticks)
                        {
                            if (!j.supportsVibration && j != null)
                                continue;
                            j.SetVibration(0, 0);
                        }
                    }

                    break;
            }
        }
    }

    public void StopAllVibration()
    {
        for (int i = 0; i < 4; i++)
            StopVibration(i);

        DOTween.Kill("Vibration0");
        DOTween.Kill("Vibration1");
        DOTween.Kill("Vibration2");
        DOTween.Kill("Vibration3");
    }

    void SlowlyStopVibration()
    {
        DOTween.Kill("Vibration0");
        DOTween.Kill("Vibration1");
        DOTween.Kill("Vibration2");
        DOTween.Kill("Vibration3");

        DOTween.To(() => playersLeftMotor[0], x => playersLeftMotor[0] = x, 0, timeToStopVibration).SetId("Vibration" + 0);
        DOTween.To(() => playersRightMotor[0], x => playersRightMotor[0] = x, 0, timeToStopVibration).SetId("Vibration" + 0);

        DOTween.To(() => playersLeftMotor[1], x => playersLeftMotor[1] = x, 0, timeToStopVibration).SetId("Vibration" + 1);
        DOTween.To(() => playersRightMotor[1], x => playersRightMotor[1] = x, 0, timeToStopVibration).SetId("Vibration" + 1);

        DOTween.To(() => playersLeftMotor[2], x => playersLeftMotor[2] = x, 0, timeToStopVibration).SetId("Vibration" + 2);
        DOTween.To(() => playersRightMotor[2], x => playersRightMotor[2] = x, 0, timeToStopVibration).SetId("Vibration" + 2);

        DOTween.To(() => playersLeftMotor[3], x => playersLeftMotor[3] = x, 0, timeToStopVibration).SetId("Vibration" + 3);
        DOTween.To(() => playersRightMotor[3], x => playersRightMotor[3] = x, 0, timeToStopVibration).SetId("Vibration" + 3);

    }

    void OnApplicationQuit()
    {
        StopAllVibration();
    }
}

[Serializable]
public class VibrationSettings
{
    public FeedbackType whichVibration = FeedbackType.Default;

    [Range(0, 1)]
    public float leftMotorForce;
    [Range(0, 1)]
    public float rightMotorForce;
    public float vibrationDuration;
}
