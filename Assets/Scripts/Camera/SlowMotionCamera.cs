using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityStandardAssets.ImageEffects;

public class SlowMotionCamera : MonoBehaviour 
{
	public Ease easetype;

	[Header ("SlowMotion InGame")]
	//the factor used to slow down time
	public float slowFactor = 4f;
	public float slowFactor2 = 6f;
	public float slowFactor3 = 8f;
	public float slowFactor4 = 10f;
	public float slowMotionDuration;
	public float slowMotionDuration2;
	public float slowMotionDuration3;
	public float slowMotionDuration4;
	public float timeTween;
	public float bloomIntensity = 0.7f;
	public float timeTweenEffect;

	[Header ("SlowMotion Pause")]
	public float timeTweenPause = 0.05f;

	[Header ("Debug Test")]
	public bool slowMotion;
	public float timeScaleDebug;

	private float timeScaleTemp;
	private float fixedDeltaTemp;
	private float maximumDeltaTemp;

	public int slowMoNumber = 0;

	void Awake ()
	{
		Time.timeScale = 1f;
		timeScaleTemp = Time.timeScale;
		fixedDeltaTemp = Time.fixedDeltaTime;
		maximumDeltaTemp = Time.maximumDeltaTime;
	}

	// Called when this script starts
	void Start()
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(slowMotion)
		{
			slowMotion = false;
			StartSlowMotion ();
		}

		timeScaleDebug = Time.timeScale;
	}

	public void StartSlowMotion ()
	{
		StopCoroutine (SlowMotionDuration (slowMoNumber));
		DOTween.Pause ("StopSlowMotion");

		slowMoNumber++;

		float slowFactorTemp = 0;

		switch (slowMoNumber)
		{
		case 1:
			slowFactorTemp = slowFactor;
			break;
		case 2:
			slowFactorTemp = slowFactor2;
			break;
		case 3:
			slowFactorTemp = slowFactor3;
			break;
		case 4:
			slowFactorTemp = slowFactor4;
			break;
		default:
			slowFactorTemp = slowFactor4;
			break;
		}

		DOTween.To(()=> Time.timeScale, x=> Time.timeScale =x, timeScaleTemp/slowFactorTemp, timeTween).SetEase(easetype).SetId("StartSlowMotion");
		DOTween.To(()=> Time.fixedDeltaTime, x=> Time.fixedDeltaTime =x, fixedDeltaTemp/slowFactorTemp, timeTween).SetEase(easetype).SetId("StartSlowMotion");
		DOTween.To(()=> Time.maximumDeltaTime, x=> Time.maximumDeltaTime =x, maximumDeltaTemp/slowFactorTemp, timeTween).SetEase(easetype).SetId("StartSlowMotion");

		DOTween.To(()=> gameObject.GetComponent<Bloom>().bloomIntensity, x=> gameObject.GetComponent<Bloom>().bloomIntensity =x, bloomIntensity, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");

		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity =x, 5f, timeTween/2).SetEase(easetype).SetId("StartSlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration =x, 10f, timeTween/2).SetEase(easetype).SetId("StartSlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur =x, 0.5f, timeTween/2).SetEase(easetype).SetId("StartSlowMotion");

		StartCoroutine(SlowMotionDuration (slowMoNumber));
	}

	public void StopSlowMotion ()
	{
		//Debug.Log("Undo Slomo !");

		DOTween.To(()=> Time.timeScale, x=> Time.timeScale =x, 1, timeTween).SetEase(easetype).SetId("StopSlowMotion");
		DOTween.To(()=> Time.fixedDeltaTime, x=> Time.fixedDeltaTime =x, fixedDeltaTemp, timeTween).SetEase(easetype).SetId("StopSlowMotion");
		DOTween.To(()=> Time.maximumDeltaTime, x=> Time.maximumDeltaTime =x, maximumDeltaTemp, timeTween).SetEase(easetype).SetId("StopSlowMotion");

		DOTween.To(()=> gameObject.GetComponent<Bloom>().bloomIntensity, x=> gameObject.GetComponent<Bloom>().bloomIntensity =x, 0f, timeTweenEffect).SetEase(easetype).SetId("StopSlowMotion");

		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity =x, 0f, timeTween/2).SetEase(easetype).SetId("StopSlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration =x, 0f, timeTween/2).SetEase(easetype).SetId("StopSlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur =x, 0f, timeTween/2).SetEase(easetype).SetId("StopSlowMotion");	
	}

	public void StartPauseSlowMotion ()
	{
		//DOTween.Pause ("SlowMotion");
		//StopCoroutine (SlowMotionDuration ());


		DOTween.To(()=> Time.timeScale, x=> Time.timeScale =x, 0, timeTweenPause).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> Time.fixedDeltaTime, x=> Time.fixedDeltaTime =x, 0, timeTweenPause).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> Time.maximumDeltaTime, x=> Time.maximumDeltaTime =x, 0, timeTweenPause).SetEase(easetype).SetId("SlowMotion");

		DOTween.To(()=> gameObject.GetComponent<Bloom>().bloomIntensity, x=> gameObject.GetComponent<Bloom>().bloomIntensity =x, bloomIntensity, timeTweenEffect).SetEase(easetype).SetId("SlowMotion");

		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity =x, 5f, timeTweenPause/2).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration =x, 10f, timeTweenPause/2).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur =x, 0.5f, timeTweenPause/2).SetEase(easetype).SetId("SlowMotion");
	}

	public void StopPauseSlowMotion ()
	{
		//Debug.Log("Undo Slomo !");

		DOTween.To(()=> Time.timeScale, x=> Time.timeScale =x, timeScaleTemp, timeTweenPause).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> Time.fixedDeltaTime, x=> Time.fixedDeltaTime =x, fixedDeltaTemp, timeTweenPause).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> Time.maximumDeltaTime, x=> Time.maximumDeltaTime =x, maximumDeltaTemp, timeTweenPause).SetEase(easetype).SetId("SlowMotion");

		DOTween.To(()=> gameObject.GetComponent<Bloom>().bloomIntensity, x=> gameObject.GetComponent<Bloom>().bloomIntensity =x, 0f, timeTweenEffect).SetEase(easetype).SetId("SlowMotion");

		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity =x, 0f, timeTweenPause/2).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration =x, 0f, timeTweenPause/2).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur =x, 0f, timeTweenPause/2).SetEase(easetype).SetId("SlowMotion");
	}

	IEnumerator SlowMotionDuration (int slowMoNumberTest)
	{
		float slowMotionDurationTemp = 0;

		switch (slowMoNumber)
		{
		case 1:
			slowMotionDurationTemp = slowMotionDuration;
			break;
		case 2:
			slowMotionDurationTemp = slowMotionDuration2;
			break;
		case 3:
			slowMotionDurationTemp = slowMotionDuration3;
			break;
		case 4:
			slowMotionDurationTemp = slowMotionDuration4;
			break;
		}
	
		yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(slowMotionDurationTemp + timeTween));

		if(slowMoNumberTest == slowMoNumber)
		{
			StopSlowMotion ();

			slowMoNumber = 0;
		}
	}

	public void SlowMotionImageEffects ()
	{
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity =x, 5f, timeTween/2).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration =x, 10f, timeTween/2).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur =x, 0.5f, timeTween/2).SetEase(easetype).SetId("SlowMotion");
	}

	public void SlowMotionStopImageEffects ()
	{
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity =x, 0f, timeTween/2).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration =x, 0f, timeTween/2).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur =x, 0f, timeTween/2).SetEase(easetype).SetId("SlowMotion");
	}
}


