using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CameraScreenShake : MonoBehaviour 
{
	public float shakeDuration = 0.5f;
	public Vector3 shakeStrenth = new Vector3 (1, 1, 0);
	public int shakeVibrato = 100;
	public float shakeRandomness = 45;

	public bool shake;

	private bool shaking;
	private Vector3 initialRotation;

	// Use this for initialization
	void Start () 
	{
		initialRotation = new Vector3 (90, 0, 0);
		//initialRotation = transform.rotation.eulerAngles;

		LoadModeManager.Instance.OnLevelLoaded += ResetCameraRotation;
	}

	// Update is called once per frame
	void Update () 
	{
		if(shake)
		{
			shake = false;
			CameraShaking();
		}
	}

	public void CameraShaking ()
	{
		shaking = true;
		shake = false;
		//print("Shaking");
		transform.DOShakeRotation (shakeDuration, shakeStrenth, shakeVibrato, shakeRandomness).OnComplete (EndOfShake).SetId("ScreenShake");
	}

	void EndOfShake ()
	{
		if(!DOTween.IsTweening("ScreenShake"))
		{
			shaking = false;
			ResetCameraRotation ();
		}
	}

	void ResetCameraRotation ()
	{
		transform.DORotate(initialRotation, 0.5f);
	}
	
}
