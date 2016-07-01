using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CameraScreenShake : MonoBehaviour 
{
	public float shakeDuration = 0.5f;
	public Vector3 shakeStrenth = new Vector3 (1, 1, 0);
	public int shakeVibrato = 100;
	public float shakeRandomness = 45;

	public bool shaking;

	private Vector3 initialPosition;

	private ProbesPlacement probesScript;

	// Use this for initialization
	void Start () 
	{
		probesScript = GetComponent<ProbesPlacement> ();

		//initialPosition = transform.position;
		initialPosition = new Vector3 (0, 30, 0);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(shaking)
		{
			shaking = false;
			CameraShaking();
		}
	}

	public void CameraShaking ()
	{
		shaking = false;
		//print("Shaking");
		transform.DOShakePosition (shakeDuration, shakeStrenth, shakeVibrato, shakeRandomness).OnComplete (ResetCameraPosition);
	}

	void ResetCameraPosition ()
	{
		if(StaticVariables.GamePaused == false)
			transform.DOMove(initialPosition, 0.5f);
	}
	
}
