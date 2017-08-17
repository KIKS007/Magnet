using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityStandardAssets.ImageEffects;
using Colorful;

public class SlowMotionCamera : MonoBehaviour 
{
	public event EventHandler OnAllSlowMotionStart;
	public event EventHandler OnAllSlowMotionStop;

	public event EventHandler OnSlowMotionStart;
	public event EventHandler OnSlowMotionStop;

	public event EventHandler OnPauseSlowMotionStart;
	public event EventHandler OnPauseSlowMotionStop;

	public event EventHandler OnEndGameSlowMotionStart;
	public event EventHandler OnEndGameSlowMotionStop;

	public Ease easetype;
	public bool slowMoEnabled = true;

	[Header ("SlowMotion InGame")]
	public int slowMoNumber = 0;
	public float[] slowFactors = new float[4];
	public float[] slowMotionDurations = new float[4];
	public float timeTween;
	public float timeTweenEffect;

	[Header ("Pause SlowMo")]
	public float timeTweenPause;
	public float timeTweenEffectPause;

	[Header ("Bloom")]
	public bool unityBloomEnabled = true;
	public float bloomIntensity = 0.7f;

	[Header ("Unity Vignetting")]
	public bool unityVignettingEnable = true;
	public float vignettingIntensity;
	public float vignettingBlur;
	public float vignettingChromaticAberration;

	[Header ("RGB Split")]
	public bool rgbSplitEnabled = false;
	public float rgbAmount = 12;

	[Header ("Lens Distortion Blur")]
	public bool lensDistortionEnabled = false;
	public float distortion = 0.2f;
	public float cubicDistortion = 0.6f;
	public float scaleZoom = 0.9f;

	[Header ("Contrast Vignette")]
	public bool contrastVignetteEnabled = false;
	public bool focusVignetteEnabled = false;
	public float darkness;

	[Header ("Mirror Effect")]
	public bool mirrorEffectEnabled = false;
	public float mirrorTweenDuration = 0.5f;
	public float initialOffset = 0.5f;
	public float modifiedOffset;

	[Header ("Debug Test")]
	public bool slowMotion;
	public bool pauseSlowMotion;
	public float timeScaleDebug;

	[HideInInspector]
	public float initialTimeScale;
	[HideInInspector]
	public float initialMaximumDelta;

	public float bloomInitialIntensity;

	[HideInInspector]
	public MirrorReflection mirrorScript;

	void Awake ()
	{
		if (slowMoEnabled)
			Time.timeScale = 1f;

		initialTimeScale = Time.timeScale;
		initialMaximumDelta = Time.maximumDeltaTime;

		bloomInitialIntensity = gameObject.GetComponent<Bloom> ().bloomIntensity;

		GlobalVariables.Instance.OnEndMode += () => slowMoNumber = 0;
		GlobalVariables.Instance.OnMenu += () => slowMoNumber = 0;
		LoadModeManager.Instance.OnLevelLoaded += StopSlowMotion;

		LoadModeManager.Instance.OnLevelLoaded += () => mirrorScript = GameObject.FindGameObjectWithTag("Environment").transform.GetComponentInChildren<MirrorReflection>();
	}

	// Update is called once per frame
	void Update () 
	{
		if(slowMotion)
		{
			slowMotion = false;
			StartSlowMotion ();
		}

		if(pauseSlowMotion)
		{
			pauseSlowMotion = false;
			StartPauseSlowMotion ();
		}

		timeScaleDebug = Time.timeScale;
	}

	public void StartSlowMotion ()
	{
		if (!slowMoEnabled)
			return;

		if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
			return;

		StopCoroutine (SlowMotionDuration (slowMoNumber));
		DOTween.Kill ("StopSlowMotion");

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

		if (OnSlowMotionStart != null)
			OnSlowMotionStart ();

		if (OnAllSlowMotionStart != null)
			OnAllSlowMotionStart ();

		DOTween.To(()=> Time.timeScale, x=> Time.timeScale =x, initialTimeScale/slowFactorTemp, timeTween).SetEase(easetype).SetId("StartSlowMotion");
		DOTween.To(()=> Time.fixedDeltaTime, x=> Time.fixedDeltaTime =x, GlobalVariables.Instance.fixedDeltaTime/slowFactorTemp, timeTween).SetEase(easetype).SetId("StartSlowMotion");
		//DOTween.To(()=> Time.maximumDeltaTime, x=> Time.maximumDeltaTime =x, initialMaximumDelta/slowFactorTemp, timeTween).SetEase(easetype).SetId("StartSlowMotion");

		if(unityBloomEnabled)
			DOTween.To(()=> gameObject.GetComponent<Bloom>().bloomIntensity, x=> gameObject.GetComponent<Bloom>().bloomIntensity =x, bloomIntensity, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");

		if(unityVignettingEnable)
			UnityVignetting (true);

		if (rgbSplitEnabled)
			RGBSplit (true);

		if (lensDistortionEnabled)
			LensDistorsionBlur (true);

		if (contrastVignetteEnabled)
			ContrastVignette (true);

		if(mirrorEffectEnabled && mirrorScript != null)
			DOTween.To(()=> mirrorScript.m_ClipPlaneOffset, x=> mirrorScript.m_ClipPlaneOffset =x, modifiedOffset, mirrorTweenDuration).SetEase(easetype).SetId("StartSlowMotion");

		StartCoroutine(SlowMotionDuration (slowMoNumber));
	}

	public void StopSlowMotion ()
	{
		if (!slowMoEnabled)
			return;

		if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
			return;

		//Debug.Log("Undo Slomo !");

		if (OnSlowMotionStop != null)
			OnSlowMotionStop ();

		if (OnAllSlowMotionStop != null)
			OnAllSlowMotionStop ();

		DOTween.To(()=> Time.timeScale, x=> Time.timeScale =x, 1, timeTween).SetEase(easetype).SetId("StopSlowMotion");
		DOTween.To(()=> Time.fixedDeltaTime, x=> Time.fixedDeltaTime =x, GlobalVariables.Instance.fixedDeltaTime, timeTween).SetEase(easetype).SetId("StopSlowMotion");
		//DOTween.To(()=> Time.maximumDeltaTime, x=> Time.maximumDeltaTime =x, initialMaximumDelta, timeTween).SetEase(easetype).SetId("StopSlowMotion");

		if(unityBloomEnabled)
			DOTween.To(()=> gameObject.GetComponent<Bloom>().bloomIntensity, x=> gameObject.GetComponent<Bloom>().bloomIntensity =x, bloomInitialIntensity, timeTweenEffect).SetEase(easetype).SetId("StopSlowMotion");

		if(unityVignettingEnable)
			UnityVignetting (false);

		if (rgbSplitEnabled)
			RGBSplit (false);

		if (lensDistortionEnabled)
			LensDistorsionBlur (false);

		if (contrastVignetteEnabled)
			ContrastVignette (false);

		if(mirrorEffectEnabled && mirrorScript != null)
			DOTween.To(()=> mirrorScript.m_ClipPlaneOffset, x=> mirrorScript.m_ClipPlaneOffset =x, initialOffset, mirrorTweenDuration).SetEase(easetype).SetId("StartSlowMotion");
	}

	public void StartPauseSlowMotion ()
	{
		if (!slowMoEnabled)
			return;

		DOTween.Kill ("StopSlowMotion");

		if (OnPauseSlowMotionStart != null)
			OnPauseSlowMotionStart ();

		if (OnAllSlowMotionStart != null)
			OnAllSlowMotionStart ();

		DOTween.To(()=> Time.timeScale, x=> Time.timeScale =x, 0, timeTweenPause).SetEase(easetype).SetId("StartSlowMotion");
		DOTween.To(()=> Time.fixedDeltaTime, x=> Time.fixedDeltaTime =x, 0, timeTweenPause).SetEase(easetype).SetId("StartSlowMotion");
		DOTween.To(()=> Time.maximumDeltaTime, x=> Time.maximumDeltaTime =x, 0, timeTweenPause).SetEase(easetype).SetId("StartSlowMotion");
	}

	public void StopPauseSlowMotion ()
	{
		if (!slowMoEnabled)
			return;

		if (OnPauseSlowMotionStop != null)
			OnPauseSlowMotionStop ();

		if (OnAllSlowMotionStop != null)
			OnAllSlowMotionStop ();

		DOTween.To(()=> Time.timeScale, x=> Time.timeScale =x, 1, timeTweenPause).SetEase(easetype).SetId("StopSlowMotion");
		DOTween.To(()=> Time.fixedDeltaTime, x=> Time.fixedDeltaTime =x, GlobalVariables.Instance.fixedDeltaTime, timeTweenPause).SetEase(easetype).SetId("StopSlowMotion");
		DOTween.To(()=> Time.maximumDeltaTime, x=> Time.maximumDeltaTime =x, initialMaximumDelta, timeTweenPause).SetEase(easetype).SetId("StopSlowMotion");

		if(unityBloomEnabled)
			DOTween.To(()=> gameObject.GetComponent<Bloom>().bloomIntensity, x=> gameObject.GetComponent<Bloom>().bloomIntensity =x, bloomInitialIntensity, timeTweenEffectPause).SetEase(easetype).SetId("StopSlowMotion");

		if(unityVignettingEnable)
			UnityVignetting (false);

		if (rgbSplitEnabled)
			RGBSplit (false);

		if (lensDistortionEnabled)
			LensDistorsionBlur (false);

		if (contrastVignetteEnabled)
			ContrastVignette (false);
	}

	public void StartEndGameSlowMotion ()
	{
		if (!slowMoEnabled)
			return;

		StopAllCoroutines ();
		DOTween.Kill ("StopSlowMotion");

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

		if (OnEndGameSlowMotionStart != null)
			OnEndGameSlowMotionStart ();

		if (OnAllSlowMotionStart != null)
			OnAllSlowMotionStart ();
		
		DOTween.To(()=> Time.timeScale, x=> Time.timeScale =x, initialTimeScale/slowFactorTemp, timeTween).SetEase(easetype).SetId("StartSlowMotion");
		DOTween.To(()=> Time.fixedDeltaTime, x=> Time.fixedDeltaTime =x, GlobalVariables.Instance.fixedDeltaTime/slowFactorTemp, timeTween).SetEase(easetype).SetId("StartSlowMotion");
		DOTween.To(()=> Time.maximumDeltaTime, x=> Time.maximumDeltaTime =x, initialMaximumDelta/slowFactorTemp, timeTween).SetEase(easetype).SetId("StartSlowMotion");

		if(unityBloomEnabled)
			DOTween.To(()=> gameObject.GetComponent<Bloom>().bloomIntensity, x=> gameObject.GetComponent<Bloom>().bloomIntensity =x, bloomIntensity, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");

		if(unityVignettingEnable)
			UnityVignetting (true);

		if (rgbSplitEnabled)
			RGBSplit (true);

		if (lensDistortionEnabled)
			LensDistorsionBlur (true);

		if (contrastVignetteEnabled)
			ContrastVignette (true);

		if(mirrorEffectEnabled && mirrorScript != null)
			DOTween.To(()=> mirrorScript.m_ClipPlaneOffset, x=> mirrorScript.m_ClipPlaneOffset =x, modifiedOffset, mirrorTweenDuration).SetEase(easetype).SetId("StartSlowMotion");
	}

	public void StopEndGameSlowMotion ()
	{
		if (!slowMoEnabled)
			return;

		//Debug.Log("Undo Slomo !");

		if (OnEndGameSlowMotionStop != null)
			OnEndGameSlowMotionStop ();

		if (OnAllSlowMotionStop != null)
			OnAllSlowMotionStop ();
		
		DOTween.To(()=> Time.timeScale, x=> Time.timeScale =x, 1, timeTween).SetEase(easetype).SetId("StopSlowMotion");
		DOTween.To(()=> Time.fixedDeltaTime, x=> Time.fixedDeltaTime =x, GlobalVariables.Instance.fixedDeltaTime, timeTween).SetEase(easetype).SetId("StopSlowMotion");
		DOTween.To(()=> Time.maximumDeltaTime, x=> Time.maximumDeltaTime =x, initialMaximumDelta, timeTween).SetEase(easetype).SetId("StopSlowMotion");

		if(unityBloomEnabled)
			DOTween.To(()=> gameObject.GetComponent<Bloom>().bloomIntensity, x=> gameObject.GetComponent<Bloom>().bloomIntensity =x, bloomInitialIntensity, timeTweenEffect).SetEase(easetype).SetId("StopSlowMotion");

		if(unityVignettingEnable)
			UnityVignetting (false);

		if (rgbSplitEnabled)
			RGBSplit (false);

		if (lensDistortionEnabled)
			LensDistorsionBlur (false);

		if (contrastVignetteEnabled)
			ContrastVignette (false);

		if(mirrorEffectEnabled && mirrorScript != null)
			DOTween.To(()=> mirrorScript.m_ClipPlaneOffset, x=> mirrorScript.m_ClipPlaneOffset =x, initialOffset, mirrorTweenDuration).SetEase(easetype).SetId("StartSlowMotion");
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
		default:
			slowMotionDurationTemp = slowMotionDurations[3];
			break;
		}
	
		yield return new WaitForSecondsRealtime (slowMotionDurationTemp + timeTween);

		if(slowMoNumberTest == slowMoNumber && GlobalVariables.Instance.GameState != GameStateEnum.Paused)
		{
			StopSlowMotion ();

			slowMoNumber = 0;
		}
	}

	void UnityVignetting (bool enable)
	{
		if (!slowMoEnabled)
			return;
		
		if(enable)
		{
			DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity =x, vignettingIntensity, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");
			DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration =x, vignettingChromaticAberration, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");
			DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur =x, vignettingIntensity, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");
		}
		else
		{
			DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().intensity =x, 0, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");
			DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().chromaticAberration =x, 0, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");
			DOTween.To(()=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur, x=> gameObject.GetComponent<VignetteAndChromaticAberration>().blur =x, 0, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");
		}
	}

	void RGBSplit (bool enable)
	{
		if (!slowMoEnabled)
			return;
		
		if(enable)
			DOTween.To(()=> gameObject.GetComponent<RGBSplit>().Amount, x=> gameObject.GetComponent<RGBSplit>().Amount =x, rgbAmount, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");

		else
			DOTween.To(()=> gameObject.GetComponent<RGBSplit>().Amount, x=> gameObject.GetComponent<RGBSplit>().Amount =x, 0, timeTweenEffect).SetEase(easetype).SetId("StopSlowMotion");
		
	}

	void LensDistorsionBlur (bool enable)
	{
		if (!slowMoEnabled)
			return;
		
		if(enable)
		{
			DOTween.To(()=> gameObject.GetComponent<LensDistortionBlur>().Distortion, x=> gameObject.GetComponent<LensDistortionBlur>().Distortion =x, distortion, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");
			DOTween.To(()=> gameObject.GetComponent<LensDistortionBlur>().CubicDistortion, x=> gameObject.GetComponent<LensDistortionBlur>().CubicDistortion =x, cubicDistortion, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");
			DOTween.To(()=> gameObject.GetComponent<LensDistortionBlur>().Scale, x=> gameObject.GetComponent<LensDistortionBlur>().Scale =x, scaleZoom, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");
		}
		else
		{
			DOTween.To(()=> gameObject.GetComponent<LensDistortionBlur>().Distortion, x=> gameObject.GetComponent<LensDistortionBlur>().Distortion =x, 0, timeTweenEffect).SetEase(easetype).SetId("StopSlowMotion");
			DOTween.To(()=> gameObject.GetComponent<LensDistortionBlur>().CubicDistortion, x=> gameObject.GetComponent<LensDistortionBlur>().CubicDistortion =x, 0, timeTweenEffect).SetEase(easetype).SetId("StopSlowMotion");
			DOTween.To(()=> gameObject.GetComponent<LensDistortionBlur>().Scale, x=> gameObject.GetComponent<LensDistortionBlur>().Scale =x, 1, timeTweenEffect).SetEase(easetype).SetId("StopSlowMotion");
		}

	}

	void ContrastVignette (bool enable)
	{
		if (!slowMoEnabled)
			return;
		
		if(enable)
			DOTween.To(()=> gameObject.GetComponent<ContrastVignette>().Darkness, x=> gameObject.GetComponent<ContrastVignette>().Darkness =x, darkness, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");

		else
			DOTween.To(()=> gameObject.GetComponent<ContrastVignette>().Darkness, x=> gameObject.GetComponent<ContrastVignette>().Darkness =x, 0, timeTweenEffect).SetEase(easetype).SetId("StopSlowMotion");
	}

	public void ContrastVignette (Vector3 worldPosition)
	{
		if (!slowMoEnabled)
			return;

		if(focusVignetteEnabled)
		{
			Vector2 viewportPosition = GetComponent<Camera> ().WorldToViewportPoint (worldPosition);

			DOTween.To(()=> gameObject.GetComponent<ContrastVignette>().Darkness, x=> gameObject.GetComponent<ContrastVignette>().Darkness =x, darkness, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");
			DOTween.To(()=> gameObject.GetComponent<ContrastVignette>().Center, x=> gameObject.GetComponent<ContrastVignette>().Center =x, viewportPosition, timeTweenEffect).SetEase(easetype).SetId("StartSlowMotion");
		}
	}

	public void StopEffects ()
	{
		if(unityBloomEnabled)
			DOTween.To(()=> gameObject.GetComponent<Bloom>().bloomIntensity, x=> gameObject.GetComponent<Bloom>().bloomIntensity =x, bloomInitialIntensity, timeTweenEffect).SetEase(easetype).SetId("StopSlowMotion");

		if(unityVignettingEnable)
			UnityVignetting (false);

		if (rgbSplitEnabled)
			RGBSplit (false);

		if (lensDistortionEnabled)
			LensDistorsionBlur (false);

		if (contrastVignetteEnabled)
			ContrastVignette (false);

		if(mirrorEffectEnabled && mirrorScript != null)
			DOTween.To(()=> mirrorScript.m_ClipPlaneOffset, x=> mirrorScript.m_ClipPlaneOffset =x, initialOffset, mirrorTweenDuration).SetEase(easetype).SetId("StartSlowMotion");
	}
}


