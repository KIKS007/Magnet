using UnityEngine;
using System.Collections;
using DG.Tweening;
using XInputDotNetPure;

public class VibrationTest : MonoBehaviour 
{

	[RangeAttribute(0, 1)]
	public float leftMotorVibration;
	[RangeAttribute(0, 1)]
	public float rightMotorVibration;

	public float riseDuration = 10;
	public float fallDuration = 5;

	// Use this for initialization
	void Start () 
	{
		leftMotorVibration = 0;
		rightMotorVibration = 0;

		StartCoroutine (TweeningVibration ());
	}

	IEnumerator TweeningVibration ()
	{
		DOTween.To (() => leftMotorVibration, x => leftMotorVibration = x, 1, riseDuration);
		yield return new WaitForSeconds (riseDuration);
		DOTween.To (() => leftMotorVibration, x => leftMotorVibration = x, 0, fallDuration);
		yield return new WaitForSeconds (fallDuration);

		DOTween.To (() => rightMotorVibration, x => rightMotorVibration = x, 1, riseDuration);
		yield return new WaitForSeconds (riseDuration);
		DOTween.To (() => rightMotorVibration, x => rightMotorVibration = x, 0, fallDuration);
		yield return new WaitForSeconds (fallDuration);

		yield return null;
	}
	
	// Update is called once per frame
	void Update () 
	{
		GamePad.SetVibration (PlayerIndex.One, leftMotorVibration, rightMotorVibration);
	}
}
