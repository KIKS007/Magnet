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
	public float timeTween;
	public float slowMotionDuration;
	public float bloomIntensity = 0.7f;
	public float timeTweenEffect;

	[Header ("SlowMotion Pause")]
	public float timeTweenPause = 0.05f;


	[Header ("Debug Test")]
	public bool slowMotion;

	private float timeScaleTemp;
	private float fixedDeltaTemp;
	private float maximumDeltaTemp;

	void Awake ()
	{
		Time.timeScale = 1f;
	}

	// Called when this script starts
	void Start()
	{
		timeScaleTemp = Time.timeScale;
		fixedDeltaTemp = Time.fixedDeltaTime;
		maximumDeltaTemp = Time.maximumDeltaTime;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(slowMotion)
		{
			slowMotion = false;
			StartSlowMotion ();
		}
	}

	public void StartSlowMotion ()
	{
		DOTween.To(()=> Time.timeScale, x=> Time.timeScale =x, Time.timeScale/slowFactor, timeTween).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> Time.fixedDeltaTime, x=> Time.fixedDeltaTime =x, Time.fixedDeltaTime/slowFactor, timeTween).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> Time.maximumDeltaTime, x=> Time.maximumDeltaTime =x, Time.maximumDeltaTime/slowFactor, timeTween).SetEase(easetype).SetId("SlowMotion");

		DOTween.To(()=> gameObject.GetComponent<Bloom>().bloomIntensity, x=> gameObject.GetComponent<Bloom>().bloomIntensity =x, bloomIntensity, timeTweenEffect).SetEase(easetype).SetId("SlowMotion");

		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity =x, 5f, timeTween/2).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration =x, 10f, timeTween/2).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur =x, 0.5f, timeTween/2).SetEase(easetype).SetId("SlowMotion");

		StartCoroutine("SlowMotionDuration");
	}

	public void StopSlowMotion ()
	{
		//Debug.Log("Undo Slomo !");

		DOTween.To(()=> Time.timeScale, x=> Time.timeScale =x, 1, timeTween).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> Time.fixedDeltaTime, x=> Time.fixedDeltaTime =x, fixedDeltaTemp, timeTween).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> Time.maximumDeltaTime, x=> Time.maximumDeltaTime =x, maximumDeltaTemp, timeTween).SetEase(easetype).SetId("SlowMotion");

		DOTween.To(()=> gameObject.GetComponent<Bloom>().bloomIntensity, x=> gameObject.GetComponent<Bloom>().bloomIntensity =x, 0f, timeTweenEffect).SetEase(easetype).SetId("SlowMotion");

		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity =x, 0f, timeTween/2).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration =x, 0f, timeTween/2).SetEase(easetype).SetId("SlowMotion");
		DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur =x, 0f, timeTween/2).SetEase(easetype).SetId("SlowMotion");
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

	IEnumerator SlowMotionDuration ()
	{
		yield return new WaitForSeconds(slowMotionDuration);
		StopSlowMotion ();

		slowMotion = false;
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


