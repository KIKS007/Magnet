using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityStandardAssets.ImageEffects;

public class SlowMotionCamera : MonoBehaviour 
{
	public Ease easetype;

	[Header ("SlowMotion InGame")]
	//the factor used to slow down time
	public float[] slowFactors = new float[4];
	public float[] slowMotionDurations = new float[4];
	public float timeTween;
	public float timeTweenEffect;
	public float bloomIntensity = 0.7f;

	[Header ("Vignetting")]
	public float vignettingIntensity;
	public float vignettingBlur;
	public float vignettingChromaticAberration;

	[Header ("Pause SlowMo")]
	public float timeTweenPause;
	public float timeTweenEffectPause;

	[Header ("Debug Test")]
	public bool slowMotion;
	public float timeScaleDebug;

	private float timeScaleTemp;
	private float fixedDeltaTemp;
	private float maximumDeltaTemp;

	public int slowMoNumber = 0;

	private float bloomInitialIntensity;

	void Awake ()
	{
		Time.timeScale = 1f;
		timeScaleTemp = Time.timeScale;
		fixedDeltaTemp = Time.fixedDeltaTime;
		maximumDeltaTemp = Time.maximumDeltaTime;

		bloomInitialIntensity = gameObject.GetComponent<Bloom> ().bloomIntensity;
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
			slowFactorTemp = slowFactors[0];
			break;
		case 2:
			slowFactorTemp = slowFactors[1];
			break;
		case 3:
			slowFactorTemp = slowFactors[2];
			break;
		case 4:
			slowFactorTemp = slowFactors[3];
			break;
		default:
			slowFactorTemp = slowFactors[3];
			break;
		}

		DOTween.To(()=> Time.timeScale, x=> Time.timeScale =x, timeScaleTemp/slowFactorTemp, timeTween).SetEase(easetype).SetId("StartSlowMotion");
		DOTween.To(()=> Time.fixedDeltaTime, x=> Time.fixedDeltaTime =x, fixedDeltaTemp/slowFactorTemp, timeTween).SetEase(easetype).SetId("StartSlowMotion");
		DOTween.To(()=> Time.maximumDeltaTime, x=> Time.maximumDeltaTime =x, maximumDeltaTemp/slowFactorTemp, timeTween).SetEase(easetype).SetId("StartSlowMotion");

		DOTween.To(()=> gameObject.GetComponent<Bloom>().bloomIntensity, x=> gameObject.GetComponent<Bloom>().bloomIntensity =x, bloomIntensity, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");

		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity =x, vignettingIntensity, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration =x, vignettingChromaticAberration, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur =x, vignettingIntensity, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");

		StartCoroutine(SlowMotionDuration (slowMoNumber));
	}

	public void StopSlowMotion ()
	{
		//Debug.Log("Undo Slomo !");

		DOTween.To(()=> Time.timeScale, x=> Time.timeScale =x, 1, timeTween).SetEase(easetype).SetId("StopSlowMotion");
		DOTween.To(()=> Time.fixedDeltaTime, x=> Time.fixedDeltaTime =x, fixedDeltaTemp, timeTween).SetEase(easetype).SetId("StopSlowMotion");
		DOTween.To(()=> Time.maximumDeltaTime, x=> Time.maximumDeltaTime =x, maximumDeltaTemp, timeTween).SetEase(easetype).SetId("StopSlowMotion");

		DOTween.To(()=> gameObject.GetComponent<Bloom>().bloomIntensity, x=> gameObject.GetComponent<Bloom>().bloomIntensity =x, bloomInitialIntensity, timeTweenEffect).SetEase(easetype).SetId("StopSlowMotion");

		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity =x, 0f, timeTweenEffect).SetEase(easetype).SetId("StopSlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration =x, 0f, timeTweenEffect).SetEase(easetype).SetId("StopSlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur =x, 0f, timeTweenEffect).SetEase(easetype).SetId("StopSlowMotion");	
	}

	public void StartPauseSlowMotion ()
	{
		StopCoroutine (SlowMotionDuration (slowMoNumber));
		DOTween.Pause ("StopSlowMotion");

		DOTween.To(()=> Time.timeScale, x=> Time.timeScale =x, 0, timeTweenPause).SetEase(easetype).SetId("StartSlowMotion");
		DOTween.To(()=> Time.fixedDeltaTime, x=> Time.fixedDeltaTime =x, 0, timeTweenPause).SetEase(easetype).SetId("StartSlowMotion");
		DOTween.To(()=> Time.maximumDeltaTime, x=> Time.maximumDeltaTime =x, 0, timeTweenPause).SetEase(easetype).SetId("StartSlowMotion");

		DOTween.To(()=> gameObject.GetComponent<Bloom>().bloomIntensity, x=> gameObject.GetComponent<Bloom>().bloomIntensity =x, bloomIntensity, timeTweenEffectPause).SetEase(easetype).SetId("StartSlowMotion");

		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity =x, vignettingIntensity, timeTweenEffectPause).SetEase(easetype).SetId("StartSlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration =x, vignettingChromaticAberration, timeTweenEffectPause).SetEase(easetype).SetId("StartSlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur =x, vignettingIntensity, timeTweenEffectPause).SetEase(easetype).SetId("StartSlowMotion");
	}

	public void StopPauseSlowMotion ()
	{
		//Debug.Log("Undo Slomo !");

		DOTween.To(()=> Time.timeScale, x=> Time.timeScale =x, 1, timeTweenPause).SetEase(easetype).SetId("StopSlowMotion");
		DOTween.To(()=> Time.fixedDeltaTime, x=> Time.fixedDeltaTime =x, fixedDeltaTemp, timeTweenPause).SetEase(easetype).SetId("StopSlowMotion");
		DOTween.To(()=> Time.maximumDeltaTime, x=> Time.maximumDeltaTime =x, maximumDeltaTemp, timeTweenPause).SetEase(easetype).SetId("StopSlowMotion");

		DOTween.To(()=> gameObject.GetComponent<Bloom>().bloomIntensity, x=> gameObject.GetComponent<Bloom>().bloomIntensity =x, bloomInitialIntensity, timeTweenEffectPause).SetEase(easetype).SetId("StopSlowMotion");

		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity =x, 0f, timeTweenEffectPause).SetEase(easetype).SetId("StopSlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration =x, 0f, timeTweenEffectPause).SetEase(easetype).SetId("StopSlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur =x, 0f, timeTweenEffectPause).SetEase(easetype).SetId("StopSlowMotion");
	}

	public void StartEndGameSlowMotion ()
	{
		StopCoroutine (SlowMotionDuration (slowMoNumber));
		DOTween.Pause ("StopSlowMotion");

		slowMoNumber++;

		float slowFactorTemp = 0;

		switch (slowMoNumber)
		{
		case 1:
			slowFactorTemp = slowFactors[0];
			break;
		case 2:
			slowFactorTemp = slowFactors[1];
			break;
		case 3:
			slowFactorTemp = slowFactors[2];
			break;
		case 4:
			slowFactorTemp = slowFactors[3];
			break;
		default:
			slowFactorTemp = slowFactors[3];
			break;
		}

		DOTween.To(()=> Time.timeScale, x=> Time.timeScale =x, timeScaleTemp/slowFactorTemp, timeTween).SetEase(easetype).SetId("StartSlowMotion");
		DOTween.To(()=> Time.fixedDeltaTime, x=> Time.fixedDeltaTime =x, fixedDeltaTemp/slowFactorTemp, timeTween).SetEase(easetype).SetId("StartSlowMotion");
		DOTween.To(()=> Time.maximumDeltaTime, x=> Time.maximumDeltaTime =x, maximumDeltaTemp/slowFactorTemp, timeTween).SetEase(easetype).SetId("StartSlowMotion");

		DOTween.To(()=> gameObject.GetComponent<Bloom>().bloomIntensity, x=> gameObject.GetComponent<Bloom>().bloomIntensity =x, bloomIntensity, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");

		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity =x, vignettingIntensity, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration =x, vignettingChromaticAberration, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur =x, vignettingIntensity, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");
	}

	public void StopEndGameSlowMotion ()
	{
		//Debug.Log("Undo Slomo !");

		DOTween.To(()=> Time.timeScale, x=> Time.timeScale =x, 1, timeTween).SetEase(easetype).SetId("StopSlowMotion");
		DOTween.To(()=> Time.fixedDeltaTime, x=> Time.fixedDeltaTime =x, fixedDeltaTemp, timeTween).SetEase(easetype).SetId("StopSlowMotion");
		DOTween.To(()=> Time.maximumDeltaTime, x=> Time.maximumDeltaTime =x, maximumDeltaTemp, timeTween).SetEase(easetype).SetId("StopSlowMotion");

		DOTween.To(()=> gameObject.GetComponent<Bloom>().bloomIntensity, x=> gameObject.GetComponent<Bloom>().bloomIntensity =x, bloomInitialIntensity, timeTweenEffect).SetEase(easetype).SetId("StopSlowMotion");

		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity =x, 0f, timeTweenEffect).SetEase(easetype).SetId("StopSlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration =x, 0f, timeTweenEffect).SetEase(easetype).SetId("StopSlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur =x, 0f, timeTweenEffect).SetEase(easetype).SetId("StopSlowMotion");
	}

	IEnumerator SlowMotionDuration (int slowMoNumberTest)
	{
		float slowMotionDurationTemp = 0;

		switch (slowMoNumber)
		{
		case 1:
			slowMotionDurationTemp = slowMotionDurations[0];
			break;
		case 2:
			slowMotionDurationTemp = slowMotionDurations[1];
			break;
		case 3:
			slowMotionDurationTemp = slowMotionDurations[2];
			break;
		case 4:
			slowMotionDurationTemp = slowMotionDurations[3];
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
		DOTween.To(()=> gameObject.GetComponent<Bloom>().bloomIntensity, x=> gameObject.GetComponent<Bloom>().bloomIntensity =x, bloomIntensity, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");

		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity =x, vignettingIntensity, timeTweenEffect).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration =x, vignettingChromaticAberration, timeTweenEffect).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur =x, vignettingIntensity, timeTweenEffect).SetEase(easetype).SetId("SlowMotion");
	}

	public void SlowMotionStopImageEffects ()
	{
		DOTween.To(()=> gameObject.GetComponent<Bloom>().bloomIntensity, x=> gameObject.GetComponent<Bloom>().bloomIntensity =x, bloomInitialIntensity, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");

		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity =x, 0f, timeTweenEffect).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration =x, 0f, timeTweenEffect).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur =x, 0f, timeTweenEffect).SetEase(easetype).SetId("SlowMotion");
	}
}


