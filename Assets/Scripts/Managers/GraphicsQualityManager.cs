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
    public Toggle rgbToggle;
    public Toggle distorsionToggle;
    public Toggle slowMoVignettingToggle;

    [Header("Apply")]
    public bool changes = false;
    public GameObject applyButton;

    private SlowMotionCamera slowmoScript;
    private BrightnessContrastGamma bcgScript;
    private PostProcessingProfile postProcessProfile;
    private Bloom bloom;
    private LensDistortionBlur distorsion;
    private RGBSplit rgbSplit;
    private ContrastVignette slowMoVignetting;
    private BrownianMotion browianMotion;
    private AnalogTV analogTV;

    [HideInInspector]
    public bool browianMotionEnabled = true;

    // Use this for initialization
    void Start()
    {
        var camera = GameObject.FindGameObjectWithTag("MainCamera");

        postProcessProfile = camera.GetComponent<PostProcessingBehaviour>().profile;
        bloom = camera.GetComponent<Bloom>();
        distorsion = camera.GetComponent<LensDistortionBlur>();
        rgbSplit = camera.GetComponent<RGBSplit>();
        slowMoVignetting = camera.GetComponent<ContrastVignette>();
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
                postProcessProfile.ambientOcclusion.enabled = arg0;
                EnableApplyButton();
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
        rgbToggle.onValueChanged.AddListener((bool arg0) =>
            {
                rgbSplit.enabled = arg0;
                EnableApplyButton();
            });
        distorsionToggle.onValueChanged.AddListener((bool arg0) =>
            {
                distorsion.enabled = arg0;
                EnableApplyButton();
            });
        slowMoVignettingToggle.onValueChanged.AddListener((bool arg0) =>
            {
                slowMoVignetting.enabled = arg0;
                EnableApplyButton();
            });


        if (PlayerPrefs.HasKey("QualityLevel"))
            LoadData();
        else
        {
            Reset();
            ResetAdvanced();
        }

        SaveData();

        applyButton.SetActive(false);
        changes = false;
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

        if (PlayerPrefs.HasKey("RGBSplit"))
            rgbToggle.isOn = PlayerPrefs.GetInt("RGBSplit") == 1 ? true : false;

        if (PlayerPrefs.HasKey("Distorsion"))
            distorsionToggle.isOn = PlayerPrefs.GetInt("Distorsion") == 1 ? true : false;

        if (PlayerPrefs.HasKey("SlowMoVignetting"))
            slowMoVignettingToggle.isOn = PlayerPrefs.GetInt("SlowMoVignetting") == 1 ? true : false;
    }

    void SaveData()
    {
//		Debug.Log ("Graphics Data Saved");

        PlayerPrefs.SetInt("AmbientOcclusion", ambiantOcclusionToggle.isOn == true ? 1 : 0);
        PlayerPrefs.SetInt("Blur", blurToggle.isOn == true ? 1 : 0);
        PlayerPrefs.SetInt("Analog", analogTVToggle.isOn == true ? 1 : 0);
        PlayerPrefs.SetInt("Vignetting", vignettingToggle.isOn == true ? 1 : 0);

        PlayerPrefs.SetInt("CameraMotion", cameraMotionToggle.isOn == true ? 1 : 0);
        PlayerPrefs.SetInt("RGBSplit", rgbToggle.isOn == true ? 1 : 0);
        PlayerPrefs.SetInt("Distorsion", distorsionToggle.isOn == true ? 1 : 0);
        PlayerPrefs.SetInt("SlowMoVignetting", slowMoVignettingToggle.isOn == true ? 1 : 0);

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
        changes = false;
        LoadData();
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
        QualitySettings.SetQualityLevel(2, true);
        Shadows(shadowsSlider.value);

        Time.fixedDeltaTime = fixedDeltaTimes[2];
        currentFixedDeltaTime = fixedDeltaTimes[2];

        if (OnFixedDeltaTimeChange != null)
            OnFixedDeltaTimeChange(currentFixedDeltaTime);

        EnableApplyButton();
    }

    public void MediumQuality()
    {
        QualitySettings.SetQualityLevel(1, true);
        Shadows(shadowsSlider.value);

        Time.fixedDeltaTime = fixedDeltaTimes[1];
        currentFixedDeltaTime = fixedDeltaTimes[1];

        if (OnFixedDeltaTimeChange != null)
            OnFixedDeltaTimeChange(currentFixedDeltaTime);
   
        EnableApplyButton();
    }

    public void LowQuality()
    {
        QualitySettings.SetQualityLevel(0, true);
        Shadows(shadowsSlider.value);

        Time.fixedDeltaTime = fixedDeltaTimes[0];
        currentFixedDeltaTime = fixedDeltaTimes[0];

        if (OnFixedDeltaTimeChange != null)
            OnFixedDeltaTimeChange(currentFixedDeltaTime);
        
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

        SaveData();
    }

    //Advanced Settings
    public void ResetAdvanced()
    {
        brightnessSlider.value = 0;
        contrastSlider.value = 0;
        gammaSlider.value = 1;
        bloomSlider.value = 1f;
        shakeSlider.value = 20f;

        ambiantOcclusionToggle.isOn = true;
        blurToggle.isOn = true;
        analogTVToggle.isOn = true;
        vignettingToggle.isOn = true;
        cameraMotionToggle.isOn = true;
        rgbToggle.isOn = true;
        distorsionToggle.isOn = true;
        slowMoVignettingToggle.isOn = true;

        applyButton.SetActive(false);
        changes = false;

        SaveData();
    }

    public void Apply()
    {
        changes = false;
        SaveData();
        //applyButton.SetActive(false);
    }

    public void Revert()
    {
        changes = false;
        LoadData();
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
