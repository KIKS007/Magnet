using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityStandardAssets.ImageEffects;
using Colorful;
using UnityEngine.PostProcessing;

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

    [Header("SlowMotion InGame")]
    public int slowMoNumber = 0;
    public float[] slowFactors = new float[4];
    public float[] slowMotionDurations = new float[4];
    public float timeTween;
    public float timeTweenEffect;
    public float pauseStopTimeTweenEffect = 0.5f;

    [Header("Pause SlowMo")]
    public float timeTweenPause;
    public float timeTweenEffectPause;

    [Header("Bloom")]
    public bool unityBloomEnabled = true;
    public float bloomIntensity = 0.7f;

    [Header("Chromatic Aberration")]
    public bool chromaticAberrationEnabled = true;
    public float chromaticAberrationIntensity = 0.2f;

    [Header("Vignette")]
    public bool vignetteEnabled = false;
    public float vignetteIntensity = 0.6f;

    [Header("Lens Distortion Blur")]
    public bool lensDistortionEnabled = false;
    public float distortion = 0.2f;
    public float cubicDistortion = 0.6f;
    public float scaleZoom = 0.9f;

    [Header("Mirror Effect")]
    public bool mirrorEffectEnabled = false;
    public float mirrorTweenDuration = 0.5f;
    public float initialOffset = 0.5f;
    public float modifiedOffset;

    [Header("Debug Test")]
    public bool slowMotion;
    public bool pauseSlowMotion;
    public float timeScaleDebug;

    [HideInInspector]
    public float initialTimeScale;
    [HideInInspector]
    public float initialMaximumDelta;
    [HideInInspector]
    public float bloomInitialIntensity = 1;
    [HideInInspector]
    public MirrorReflection mirrorScript;

    private PostProcessingProfile postProcess;
    private Bloom bloom;
    private LensDistortionBlur lensDistorsion;

    private float vignetteInitialIntensity;
    private float chromaticAberrationInitialIntensity;

    void Awake()
    {
        if (slowMoEnabled)
            Time.timeScale = 1f;

        initialTimeScale = Time.timeScale;
        initialMaximumDelta = Time.maximumDeltaTime;

        postProcess = GetComponent<PostProcessingBehaviour>().profile;

        bloom = gameObject.GetComponent<Bloom>();
        lensDistorsion = gameObject.GetComponent<LensDistortionBlur>();
        bloomInitialIntensity = bloom.bloomIntensity;

        vignetteInitialIntensity = postProcess.vignette.settings.intensity;
        chromaticAberrationInitialIntensity = postProcess.chromaticAberration.settings.intensity;

        GlobalVariables.Instance.OnEndMode += () => slowMoNumber = 0;
        GlobalVariables.Instance.OnMenu += () => slowMoNumber = 0;
        LoadModeManager.Instance.OnLevelLoaded += StopSlowMotion;
    }

    // Update is called once per frame
    void Update()
    {
        if (slowMotion)
        {
            slowMotion = false;
            StartSlowMotion();
        }

        if (pauseSlowMotion)
        {
            pauseSlowMotion = false;
            StartPauseSlowMotion();
        }

        timeScaleDebug = Time.timeScale;
    }

    public void StartEffects()
    {
        BloomEffect(true);

        LensDistorsionBlur(true);

        VignetteEffect(true);

        ChromaticAberration(true);

        MirrorEffect(true);
    }

    public void StopEffects(bool endMode = false)
    {
        BloomEffect(false, endMode);

        LensDistorsionBlur(false, endMode);

        VignetteEffect(false);

        ChromaticAberration(false);

        MirrorEffect(false, endMode);
    }

    public void StartSlowMotion()
    {
        if (!slowMoEnabled)
            return;

        if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
            return;

        StopCoroutine(SlowMotionDuration(slowMoNumber));
        DOTween.Kill("StopSlowMotion");

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
            OnSlowMotionStart();

        if (OnAllSlowMotionStart != null)
            OnAllSlowMotionStart();

        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, initialTimeScale / slowFactorTemp, timeTween).SetEase(easetype).SetId("StartSlowMotion");
        DOTween.To(() => Time.fixedDeltaTime, x => Time.fixedDeltaTime = x, GlobalVariables.Instance.fixedDeltaTime / slowFactorTemp, timeTween).SetEase(easetype).SetId("StartSlowMotion");

        StartEffects();

        StartCoroutine(SlowMotionDuration(slowMoNumber));
    }

    public void StopSlowMotion()
    {
        if (!slowMoEnabled)
            return;

        if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
            return;

        //Debug.Log("Undo Slomo !");

        if (OnSlowMotionStop != null)
            OnSlowMotionStop();

        if (OnAllSlowMotionStop != null)
            OnAllSlowMotionStop();

        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, timeTween).SetEase(easetype).SetId("StopSlowMotion");
        DOTween.To(() => Time.fixedDeltaTime, x => Time.fixedDeltaTime = x, GlobalVariables.Instance.fixedDeltaTime, timeTween).SetEase(easetype).SetId("StopSlowMotion");

        StopEffects();
    }

    public void StartPauseSlowMotion()
    {
        if (!slowMoEnabled)
            return;

        if (OnPauseSlowMotionStart != null)
            OnPauseSlowMotionStart();

        if (OnAllSlowMotionStart != null)
            OnAllSlowMotionStart();

        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0, timeTweenPause).SetEase(easetype).SetId("StartSlowMotion");
        DOTween.To(() => Time.fixedDeltaTime, x => Time.fixedDeltaTime = x, 0, timeTweenPause).SetEase(easetype).SetId("StartSlowMotion");

        StopEffects();
    }

    public void StopPauseSlowMotion()
    {
        if (!slowMoEnabled)
            return;

        if (OnPauseSlowMotionStop != null)
            OnPauseSlowMotionStop();

        if (OnAllSlowMotionStop != null)
            OnAllSlowMotionStop();

        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, timeTweenPause).SetEase(easetype).SetId("StopSlowMotion");
        DOTween.To(() => Time.fixedDeltaTime, x => Time.fixedDeltaTime = x, GlobalVariables.Instance.fixedDeltaTime, timeTweenPause).SetEase(easetype).SetId("StopSlowMotion");

        StopEffects();
    }

    public void StartEndGameSlowMotion()
    {
        if (!slowMoEnabled)
            return;

        StopAllCoroutines();
        DOTween.Kill("StopSlowMotion");

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
            OnEndGameSlowMotionStart();

        if (OnAllSlowMotionStart != null)
            OnAllSlowMotionStart();
		
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, initialTimeScale / slowFactorTemp, timeTween).SetEase(easetype).SetId("StartSlowMotion");
        DOTween.To(() => Time.fixedDeltaTime, x => Time.fixedDeltaTime = x, GlobalVariables.Instance.fixedDeltaTime / slowFactorTemp, timeTween).SetEase(easetype).SetId("StartSlowMotion");

        StartEffects();
    }

    public void StopEndGameSlowMotion()
    {
        if (!slowMoEnabled)
            return;

        //Debug.Log("Undo Slomo !");

        if (OnEndGameSlowMotionStop != null)
            OnEndGameSlowMotionStop();

        if (OnAllSlowMotionStop != null)
            OnAllSlowMotionStop();
		
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, timeTween).SetEase(easetype).SetId("StopSlowMotion");
        DOTween.To(() => Time.fixedDeltaTime, x => Time.fixedDeltaTime = x, GlobalVariables.Instance.fixedDeltaTime, timeTween).SetEase(easetype).SetId("StopSlowMotion");

        StopEffects();
    }

    IEnumerator SlowMotionDuration(int slowMoNumberTest)
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
	
        yield return new WaitForSecondsRealtime(slowMotionDurationTemp + timeTween);

        if (slowMoNumberTest == slowMoNumber && GlobalVariables.Instance.GameState != GameStateEnum.Paused)
        {
            StopSlowMotion();

            slowMoNumber = 0;
        }
    }

    void BloomEffect(bool enable, bool endMode = false)
    {
        if (!slowMoEnabled)
            return;

        if (!unityBloomEnabled)
            return;

        float duration = endMode ? pauseStopTimeTweenEffect : timeTweenEffect;

        if (enable)
        {
            DOTween.To(() => bloom.bloomIntensity, x => bloom.bloomIntensity = x, bloomIntensity, duration).SetEase(easetype).SetId("StartSlowMotion");
        }
        else
        {
            DOTween.To(() => bloom.bloomIntensity, x => bloom.bloomIntensity = x, bloomInitialIntensity, duration).SetEase(easetype).SetId("StopSlowMotion");
        }
    }

    void ChromaticAberration(bool enable, bool endMode = false)
    {
        if (!slowMoEnabled)
            return;

        if (!chromaticAberrationEnabled)
            return;

        float duration = endMode ? pauseStopTimeTweenEffect : timeTweenEffect;

        var chormatic = postProcess.chromaticAberration.settings;

        if (enable)
        {
            DOTween.To(() => chormatic.intensity, x => chormatic.intensity = x, chromaticAberrationIntensity, duration).SetEase(easetype).SetId("StartSlowMotion").OnUpdate(() =>
                {
                    postProcess.chromaticAberration.settings = chormatic;
                });
        }
        else
        {
            DOTween.To(() => chormatic.intensity, x => chormatic.intensity = x, chromaticAberrationInitialIntensity, duration).SetEase(easetype).SetId("StopSlowMotion").OnUpdate(() =>
                {
                    postProcess.chromaticAberration.settings = chormatic;
                });   
        }
    }

    void VignetteEffect(bool enable, bool endMode = false)
    {
        if (!slowMoEnabled)
            return;

        if (!vignetteEnabled)
            return;

        float duration = endMode ? pauseStopTimeTweenEffect : timeTweenEffect;

        var vignette = postProcess.vignette.settings;

        if (enable)
        {
            DOTween.To(() => vignette.intensity, x => vignette.intensity = x, vignetteIntensity, duration).SetEase(easetype).SetId("StartSlowMotion").OnUpdate(() =>
                {
                    postProcess.vignette.settings = vignette;
                });
        }
        else
        {
            DOTween.To(() => vignette.intensity, x => vignette.intensity = x, vignetteInitialIntensity, duration).SetEase(easetype).SetId("StopSlowMotion").OnUpdate(() =>
                {
                    postProcess.vignette.settings = vignette;
                });   
        }
    }

    void MirrorEffect(bool enable, bool endMode = false)
    {
        if (!slowMoEnabled)
            return;

        if (!mirrorEffectEnabled)
            return;

        float duration = endMode ? pauseStopTimeTweenEffect : timeTweenEffect;

        if (mirrorScript == null)
            mirrorScript = GameObject.FindGameObjectWithTag("ArenaGround").GetComponent<MirrorReflection>();

        if (enable)
        {
            if (mirrorScript != null)
                DOTween.To(() => mirrorScript.m_ClipPlaneOffset, x => mirrorScript.m_ClipPlaneOffset = x, modifiedOffset, mirrorTweenDuration).SetEase(easetype).SetId("StartSlowMotion");
        }
        else
        {
            if (mirrorScript != null)
                DOTween.To(() => mirrorScript.m_ClipPlaneOffset, x => mirrorScript.m_ClipPlaneOffset = x, initialOffset, mirrorTweenDuration).SetEase(easetype).SetId("StopSlowMotion");
        }
    }

    void LensDistorsionBlur(bool enable, bool endMode = false)
    {
        if (!lensDistortionEnabled)
            return;
        
        if (!slowMoEnabled)
            return;

        float duration = endMode ? pauseStopTimeTweenEffect : timeTweenEffect;

        if (enable)
        {
            DOTween.To(() => lensDistorsion.Distortion, x => lensDistorsion.Distortion = x, distortion, duration).SetEase(easetype).SetId("StartSlowMotion");
            DOTween.To(() => lensDistorsion.CubicDistortion, x => lensDistorsion.CubicDistortion = x, cubicDistortion, duration).SetEase(easetype).SetId("StartSlowMotion");
            DOTween.To(() => lensDistorsion.Scale, x => lensDistorsion.Scale = x, scaleZoom, duration).SetEase(easetype).SetId("StartSlowMotion");
        }
        else
        {
            DOTween.To(() => lensDistorsion.Distortion, x => lensDistorsion.Distortion = x, 0, duration).SetEase(easetype).SetId("StopSlowMotion");
            DOTween.To(() => lensDistorsion.CubicDistortion, x => lensDistorsion.CubicDistortion = x, 0, duration).SetEase(easetype).SetId("StopSlowMotion");
            DOTween.To(() => lensDistorsion.Scale, x => lensDistorsion.Scale = x, 1, duration).SetEase(easetype).SetId("StopSlowMotion");
        }

    }
}


