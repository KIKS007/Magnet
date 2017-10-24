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

    [Header("Advanced Toggles")]
    public Toggle ambiantOcclusionToggle;
    public Toggle blurToggle;
    public Toggle grainToggle;
    public Toggle vignettingToggle;
    public Toggle cameraMotionToggle;
    public Toggle rgbToggle;
    public Toggle distorsionToggle;
    public Toggle slowMoVignettingToggle;

    [Header("Apply")]
    public bool apply = false;
    public GameObject applyButton;

    private SlowMotionCamera slowmoScript;
    private BrightnessContrastGamma bcgScript;
    private PostProcessingProfile postProcessProfile;
    private Bloom bloom;
    private LensDistortionBlur distorsion;
    private RGBSplit rgbSplit;
    private ContrastVignette slowMoVignetting;
    private BrownianMotion browianMotion;


    // Use this for initialization
    void Start()
    {
        postProcessProfile = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PostProcessingBehaviour>().profile;
        bloom = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Bloom>();
        distorsion = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<LensDistortionBlur>();
        rgbSplit = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<RGBSplit>();
        slowMoVignetting = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ContrastVignette>();
        bcgScript = FindObjectOfType<BrightnessContrastGamma>();
        slowmoScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>();
        browianMotion = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BrownianMotion>();

        //Video
        brightnessSlider.onValueChanged.AddListener(Brightness);
        contrastSlider.onValueChanged.AddListener(Contrast);
        gammaSlider.onValueChanged.AddListener(Gamma);

        //Advanced
        antialiasingSlider.onValueChanged.AddListener(AntiAliasing);
        bloomSlider.onValueChanged.AddListener(Bloom);
        shadowsSlider.onValueChanged.AddListener(Shadows);

        ambiantOcclusionToggle.onValueChanged.AddListener((bool arg0) => postProcessProfile.ambientOcclusion.enabled = arg0);
        blurToggle.onValueChanged.AddListener((bool arg0) => postProcessProfile.motionBlur.enabled = arg0);
        grainToggle.onValueChanged.AddListener((bool arg0) => postProcessProfile.grain.enabled = arg0);
        vignettingToggle.onValueChanged.AddListener((bool arg0) => postProcessProfile.vignette.enabled = arg0);

        cameraMotionToggle.onValueChanged.AddListener((bool arg0) => browianMotion.enabled = arg0);
        rgbToggle.onValueChanged.AddListener((bool arg0) => rgbSplit.enabled = arg0);
        distorsionToggle.onValueChanged.AddListener((bool arg0) => distorsion.enabled = arg0);
        slowMoVignettingToggle.onValueChanged.AddListener((bool arg0) => slowMoVignetting.enabled = arg0);


        if (PlayerPrefs.HasKey("QualityLevel"))
            LoadData();
        else
        {
            Reset();
            ResetAdvanced();
        }

        SaveData();

        applyButton.SetActive(false);
        apply = true;
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

        if (PlayerPrefs.HasKey("Grain"))
            grainToggle.isOn = PlayerPrefs.GetInt("Grain") == 1 ? true : false;

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
        PlayerPrefs.SetInt("Grain", grainToggle.isOn == true ? 1 : 0);
        PlayerPrefs.SetInt("Vignetting", vignettingToggle.isOn == true ? 1 : 0);

        PlayerPrefs.SetInt("CameraMotion", cameraMotionToggle.isOn == true ? 1 : 0);
        PlayerPrefs.SetInt("RGBSplit", rgbToggle.isOn == true ? 1 : 0);
        PlayerPrefs.SetInt("Distorsion", distorsionToggle.isOn == true ? 1 : 0);
        PlayerPrefs.SetInt("SlowMoVignetting", slowMoVignettingToggle.isOn == true ? 1 : 0);

        PlayerPrefs.SetInt("QualityLevel", QualitySettings.GetQualityLevel());

        PlayerPrefs.SetFloat("Brightness", (float)brightnessSlider.value);
        PlayerPrefs.SetFloat("Contrast", (float)contrastSlider.value);
        PlayerPrefs.SetFloat("Gamma", (float)gammaSlider.value);

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
        applyButton.SetActive(false);
        apply = false;
    }

    public void OnHide()
    {
        if (!apply)
            LoadData();
        else
            SaveData();
    }

    void EnableApplyButton()
    {
        apply = false;
        applyButton.SetActive(true);
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

    void Brightness(float value)
    {
        bcgScript.Brightness = (float)brightnessSlider.value;
        EnableApplyButton();
    }

    void Contrast(float value)
    {
        bcgScript.Contrast = (float)contrastSlider.value;
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
        apply = true;

        SaveData();
    }

    //Advanced Settings
    public void ResetAdvanced()
    {
        brightnessSlider.value = 0;
        contrastSlider.value = 0;
        gammaSlider.value = 1;
        bloomSlider.value = 1f;

        ambiantOcclusionToggle.isOn = true;
        blurToggle.isOn = true;
        grainToggle.isOn = true;
        vignettingToggle.isOn = true;
        cameraMotionToggle.isOn = true;
        rgbToggle.isOn = true;
        distorsionToggle.isOn = true;
        slowMoVignettingToggle.isOn = true;

        applyButton.SetActive(false);
        apply = true;

        SaveData();
    }

    public void Apply()
    {
        apply = true;
        SaveData();
        applyButton.SetActive(false);
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
