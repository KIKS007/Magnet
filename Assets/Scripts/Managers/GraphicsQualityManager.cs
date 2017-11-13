using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Colorful;
using UnityEngine.PostProcessing;
using UnityStandardAssets.ImageEffects;
using UnityEngine.SceneManagement;
using Klak.Motion;
using System;

public class GraphicsQualityManager : Singleton<GraphicsQualityManager>
{
    public Action<float> OnFixedDeltaTimeChange;

    [Header("Fixed Delta Times")]
    public float[] fixedDeltaTimes = new float[] { 0.02f, 0.01f, 0.005f };
    public float currentFixedDeltaTime;

    [Header("Quality")]
    public Toggle[] qualityToggles = new Toggle[3];

    [Header("Sliders")]
    public Slider antialiasingSlider;
    public Slider shadowsSlider;

    [Header("Advanced Sliders")]
    public Slider brightnessSlider;
    public Slider contrastSlider;
    public Slider gammaSlider;
    public Slider bloomSlider;
    public Slider shakeSlider;

    [Header("Advanced Toggles")]
    public Toggle ambiantOcclusionToggle;
    public Toggle blurToggle;
    public Toggle analogTVToggle;
    public Toggle vignettingToggle;
    public Toggle cameraMotionToggle;
    public Toggle distorsionToggle;

    [Header("Apply")]
    public bool changes = false;
    public GameObject applyButton;

    private SlowMotionCamera slowmoScript;
    private BrownianMotion browianMotion;
    private PostProcessingProfile postProcessProfile;
    private Bloom bloom;
    private LensDistortionBlur distorsion;
    private BrightnessContrastGamma bcgScript;
    private AnalogTV analogTV;

    private bool setup = true;

    [HideInInspector]
    public bool browianMotionEnabled = true;

    // Use this for initialization
    void Start()
    {
        var camera = GameObject.FindGameObjectWithTag("MainCamera");

        postProcessProfile = camera.GetComponent<PostProcessingBehaviour>().profile;
        bloom = camera.GetComponent<Bloom>();
        distorsion = camera.GetComponent<LensDistortionBlur>();
        bcgScript = FindObjectOfType<BrightnessContrastGamma>();
        slowmoScript = camera.GetComponent<SlowMotionCamera>();
        browianMotion = camera.GetComponent<BrownianMotion>();
        analogTV = camera.GetComponent<AnalogTV>();

        //Video
        brightnessSlider.onValueChanged.AddListener(Brightness);
        contrastSlider.onValueChanged.AddListener(Contrast);
        gammaSlider.onValueChanged.AddListener(Gamma);

        //Advanced
        antialiasingSlider.onValueChanged.AddListener(AntiAliasing);
        bloomSlider.onValueChanged.AddListener(Bloom);
        shadowsSlider.onValueChanged.AddListener(Shadows);
        shakeSlider.onValueChanged.AddListener(Shake);

        ambiantOcclusionToggle.onValueChanged.AddListener((bool arg0) =>
            {
                if (QualitySettings.GetQualityLevel() == 0)
                {
                    ambiantOcclusionToggle.isOn = false;
                    postProcessProfile.ambientOcclusion.enabled = false;
                }
                else
                {
                    postProcessProfile.ambientOcclusion.enabled = arg0;
                    EnableApplyButton();
                }
            });
        blurToggle.onValueChanged.AddListener((bool arg0) =>
            {
                postProcessProfile.motionBlur.enabled = arg0;
                EnableApplyButton();
            });
        analogTVToggle.onValueChanged.AddListener((bool arg0) =>
            {
                analogTV.enabled = arg0;
                EnableApplyButton();
            });
        vignettingToggle.onValueChanged.AddListener((bool arg0) =>
            {
                postProcessProfile.vignette.enabled = arg0;
                EnableApplyButton();
            });

        cameraMotionToggle.onValueChanged.AddListener((bool arg0) =>
            {
                browianMotion.enabled = arg0;
                browianMotionEnabled = arg0;

                if (!arg0 && MenuManager.Instance.currentMenu != null)
                    GlobalVariables.Instance.menuCameraMovement.MenuPositionGraphics();

                if (arg0)
                    GlobalVariables.Instance.menuCameraMovement.EnableBrowianMotion();
                
                EnableApplyButton();
            });
        distorsionToggle.onValueChanged.AddListener((bool arg0) =>
            {
                distorsion.enabled = arg0;
                EnableApplyButton();
            });


        Reset();
        ResetAdvanced();

        if (PlayerPrefs.HasKey("QualityLevel"))
            LoadData();

        SaveData();

        applyButton.SetActive(false);
        changes = false;
        setup = false;
    }

    void LoadData()
    {
//		if(PlayerPrefs.HasKey("QualityLevel"))
//			Debug.Log ("Graphics Data Loaded");

        if (PlayerPrefs.HasKey("QualityLevel"))
        {
            qualityToggles[0].isOn = false;
            qualityToggles[1].isOn = false;
            qualityToggles[2].isOn = false;

            switch (PlayerPrefs.GetInt("QualityLevel"))
            {
                case 2:
                    qualityToggles[2].isOn = true;
                    HighQuality();
                    break;
                case 1:
                    qualityToggles[1].isOn = true;
                    MediumQuality();
                    break;
                case 0:
                    qualityToggles[0].isOn = true;
                    LowQuality();
                    break;
            }
        }

        if (PlayerPrefs.HasKey("Brightness"))
            brightnessSlider.value = PlayerPrefs.GetFloat("Brightness");

        if (PlayerPrefs.HasKey("Contrast"))
            contrastSlider.value = PlayerPrefs.GetFloat("Contrast");

        if (PlayerPrefs.HasKey("Gamma"))
            gammaSlider.value = PlayerPrefs.GetFloat("Gamma");

        if (PlayerPrefs.HasKey("Shake"))
            shakeSlider.value = PlayerPrefs.GetFloat("Shake");

        //Advanced
        if (PlayerPrefs.HasKey("Antialiasing"))
        {
            AntiAliasing(PlayerPrefs.GetInt("Antialiasing"));
            antialiasingSlider.value = PlayerPrefs.GetInt("Antialiasing");
        }
			
        if (PlayerPrefs.HasKey("Bloom"))
        {
            Bloom(PlayerPrefs.GetFloat("Bloom"));
            bloomSlider.value = PlayerPrefs.GetFloat("Bloom");
        }

        if (PlayerPrefs.HasKey("Shadows"))
        {
            Shadows(PlayerPrefs.GetInt("Shadows"));
            shadowsSlider.value = PlayerPrefs.GetInt("Shadows");
        }


        if (PlayerPrefs.HasKey("AmbientOcclusion"))
            ambiantOcclusionToggle.isOn = PlayerPrefs.GetInt("AmbientOcclusion") == 1 ? true : false;

        if (PlayerPrefs.HasKey("Blur"))
            blurToggle.isOn = PlayerPrefs.GetInt("Blur") == 1 ? true : false;

        if (PlayerPrefs.HasKey("Analog"))
            analogTVToggle.isOn = PlayerPrefs.GetInt("Analog") == 1 ? true : false;

        if (PlayerPrefs.HasKey("Vignetting"))
            vignettingToggle.isOn = PlayerPrefs.GetInt("Vignetting") == 1 ? true : false;


        if (PlayerPrefs.HasKey("CameraMotion"))
            cameraMotionToggle.isOn = PlayerPrefs.GetInt("CameraMotion") == 1 ? true : false;

        if (PlayerPrefs.HasKey("Distorsion"))
            distorsionToggle.isOn = PlayerPrefs.GetInt("Distorsion") == 1 ? true : false;
    }

    void SaveData()
    {
//		Debug.Log ("Graphics Data Saved");

        PlayerPrefs.SetInt("AmbientOcclusion", ambiantOcclusionToggle.isOn == true ? 1 : 0);
        PlayerPrefs.SetInt("Blur", blurToggle.isOn == true ? 1 : 0);
        PlayerPrefs.SetInt("Analog", analogTVToggle.isOn == true ? 1 : 0);
        PlayerPrefs.SetInt("Vignetting", vignettingToggle.isOn == true ? 1 : 0);

        PlayerPrefs.SetInt("CameraMotion", cameraMotionToggle.isOn == true ? 1 : 0);
        PlayerPrefs.SetInt("Distorsion", distorsionToggle.isOn == true ? 1 : 0);

        PlayerPrefs.SetInt("QualityLevel", QualitySettings.GetQualityLevel());

        PlayerPrefs.SetFloat("Brightness", (float)brightnessSlider.value);
        PlayerPrefs.SetFloat("Contrast", (float)contrastSlider.value);
        PlayerPrefs.SetFloat("Gamma", (float)gammaSlider.value);
        PlayerPrefs.SetFloat("Shake", (float)shakeSlider.value);

        PlayerPrefs.SetFloat("Bloom", slowmoScript.bloomInitialIntensity);
    }

    void OnEnable()
    {
        if (GlobalVariables.Instance.GameState == GameStateEnum.Paused)
        {
            slowmoScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>();
            slowmoScript.StopPauseSlowMotion();
        }
    }

    public void OnShow()
    {
        //applyButton.SetActive(false);
        Revert();
        changes = false;
    }

    public void OnHide()
    {
        /* if (!apply)
            LoadData();
        else
            SaveData();*/
    }

    public void EnableApplyButton()
    {
        changes = true;
        //applyButton.SetActive(true);
    }

    public void HighQuality()
    {
        bool vSync = QualitySettings.vSyncCount == 0;

        QualitySettings.SetQualityLevel(2, true);
        //Shadows(shadowsSlider.value);

        QualitySettings.vSyncCount = vSync ? 0 : 1;

        ambiantOcclusionToggle.isOn = true;
        ambiantOcclusionToggle.onValueChanged.Invoke(true);

        /*Time.fixedDeltaTime = fixedDeltaTimes[2];
        currentFixedDeltaTime = fixedDeltaTimes[2];

        if (OnFixedDeltaTimeChange != null)
            OnFixedDeltaTimeChange(currentFixedDeltaTime);*/

        if (GlobalVariables.Instance.slowMotionCamera.mirrorScript == null)
            GlobalVariables.Instance.slowMotionCamera.mirrorScript = GameObject.FindGameObjectWithTag("ArenaGround").GetComponent<MirrorReflection>();

        GlobalVariables.Instance.slowMotionCamera.mirrorScript.enabled = false;
        GlobalVariables.Instance.slowMotionCamera.mirrorScript.enabled = true;

        EnableApplyButton();
    }

    public void MediumQuality()
    {
        bool vSync = QualitySettings.vSyncCount == 0;

        QualitySettings.SetQualityLevel(1, true);
        //Shadows(shadowsSlider.value);

        QualitySettings.vSyncCount = vSync ? 0 : 1;

        ambiantOcclusionToggle.isOn = true;
        ambiantOcclusionToggle.onValueChanged.Invoke(true);

        /* Time.fixedDeltaTime = fixedDeltaTimes[1];
        currentFixedDeltaTime = fixedDeltaTimes[1];

        if (OnFixedDeltaTimeChange != null)
            OnFixedDeltaTimeChange(currentFixedDeltaTime);*/
   
        if (GlobalVariables.Instance.slowMotionCamera.mirrorScript == null)
            GlobalVariables.Instance.slowMotionCamera.mirrorScript = GameObject.FindGameObjectWithTag("ArenaGround").GetComponent<MirrorReflection>();
        
        GlobalVariables.Instance.slowMotionCamera.mirrorScript.enabled = true;
        GlobalVariables.Instance.slowMotionCamera.mirrorScript.enabled = false;

        EnableApplyButton();
    }

    public void LowQuality()
    {
        int vSync = QualitySettings.vSyncCount;

        QualitySettings.SetQualityLevel(0, true);
        // Shadows(shadowsSlider.value);

        QualitySettings.vSyncCount = vSync;

        ambiantOcclusionToggle.isOn = false;
        ambiantOcclusionToggle.onValueChanged.Invoke(false);

        /*Time.fixedDeltaTime = fixedDeltaTimes[0];
        currentFixedDeltaTime = fixedDeltaTimes[0];

        if (OnFixedDeltaTimeChange != null)
            OnFixedDeltaTimeChange(currentFixedDeltaTime);*/

        if (GlobalVariables.Instance.slowMotionCamera.mirrorScript == null)
            GlobalVariables.Instance.slowMotionCamera.mirrorScript = GameObject.FindGameObjectWithTag("ArenaGround").GetComponent<MirrorReflection>();
        
        GlobalVariables.Instance.slowMotionCamera.mirrorScript.enabled = true;
        GlobalVariables.Instance.slowMotionCamera.mirrorScript.enabled = false;

        EnableApplyButton();
    }

    void Shake(float value)
    {
        GlobalVariables.Instance.screenShakeCamera.screenShakeFactor = (int)value * 10;
        EnableApplyButton();
    }

    void Brightness(float value)
    {
        bcgScript.Brightness = (float)brightnessSlider.value * 10;
        EnableApplyButton();
    }

    void Contrast(float value)
    {
        bcgScript.Contrast = (float)contrastSlider.value * 10;
        EnableApplyButton();
    }

    void Gamma(float value)
    {
        bcgScript.Gamma = (float)gammaSlider.value;
        EnableApplyButton();
    }

    public void Bloom(float value)
    {
        bloom.bloomIntensity = value;
        slowmoScript.bloomInitialIntensity = value;
        EnableApplyButton();
    }

    public void Reset()
    {
        antialiasingSlider.value = 2;
        shadowsSlider.value = 3;

        qualityToggles[2].isOn = true;
        HighQuality();

        applyButton.SetActive(false);
        changes = false;

        if (!setup)
            SaveData();
    }

    //Advanced Settings
    public void ResetAdvanced()
    {
        //Reset Settings
        brightnessSlider.value = 0f;
        contrastSlider.value = 0f;
        gammaSlider.value = 1f;
        bloomSlider.value = 1f;
        shakeSlider.value = 20f;

        //Invoke Changes
        brightnessSlider.onValueChanged.Invoke(0f);
        contrastSlider.onValueChanged.Invoke(0f);
        gammaSlider.onValueChanged.Invoke(1f);
        shakeSlider.onValueChanged.Invoke(20f);

        //Reset Settings
        ambiantOcclusionToggle.isOn = true;
        blurToggle.isOn = true;
        analogTVToggle.isOn = true;
        vignettingToggle.isOn = true;
        cameraMotionToggle.isOn = true;
        distorsionToggle.isOn = true;

        //Invoke Changes
        ambiantOcclusionToggle.onValueChanged.Invoke(true);
        blurToggle.onValueChanged.Invoke(true);
        analogTVToggle.onValueChanged.Invoke(true);
        vignettingToggle.onValueChanged.Invoke(true);
        if (!setup)
            cameraMotionToggle.onValueChanged.Invoke(true);
        distorsionToggle.onValueChanged.Invoke(true);

        applyButton.SetActive(false);
        changes = false;

        if (!setup)
            SaveData();
    }

    public void Apply()
    {
        changes = false;
        SaveData();
        ResolutionManager.Instance.SaveData();
        //applyButton.SetActive(false);
    }

    public void Revert()
    {
        LoadData();
        ResolutionManager.Instance.LoadData();
        changes = false;
    }

    public void AntiAliasing(float value)
    {
        AntialiasingModel.Settings settings = postProcessProfile.antialiasing.settings;
        string quality = "";

        switch ((int)value)
        {
            case 0:
                settings.fxaaSettings.preset = AntialiasingModel.FxaaPreset.ExtremePerformance;
                quality = "Extreme Performance";
                break;
            case 1:
                settings.fxaaSettings.preset = AntialiasingModel.FxaaPreset.Performance;
                quality = "Performance";
                break;
            case 2:
                settings.fxaaSettings.preset = AntialiasingModel.FxaaPreset.Default;
                quality = "Default";
                break;
            case 3:
                settings.fxaaSettings.preset = AntialiasingModel.FxaaPreset.Quality;
                quality = "Quality";
                break;
            case 4:
                settings.fxaaSettings.preset = AntialiasingModel.FxaaPreset.ExtremeQuality;
                quality = "Extreme Quality";
                break;
        }

        antialiasingSlider.transform.GetChild(4).GetComponent<Text>().text = quality;

        postProcessProfile.antialiasing.settings = settings;

        PlayerPrefs.SetInt("Antialiasing", (int)value);
    }

    public void Shadows(float value)
    {
        string quality = "";

        switch ((int)value)
        {
            case 0:
                QualitySettings.shadowResolution = ShadowResolution.Low;
                quality = "Low";
                break;
            case 1:
                QualitySettings.shadowResolution = ShadowResolution.Medium;
                quality = "Medium";
                break;
            case 2:
                QualitySettings.shadowResolution = ShadowResolution.High;
                quality = "High";
                break;
            case 3:
                QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                quality = "Very High";
                break;
        }

        shadowsSlider.transform.GetChild(4).GetComponent<Text>().text = quality;

        PlayerPrefs.SetInt("Shadows", (int)value);
    }
}
