using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Colorful;

public class GraphicsQualityManager : MonoBehaviour 
{
	[Header ("Quality")]
	public Toggle[] qualityToggles = new Toggle[3];

	[Header ("Sliders")]
	public Slider brightnessSlider;
	public Slider contrastSlider;
	public Slider gammaSlider;

	private SlowMotionCamera slowmoScript;
	private BrightnessContrastGamma bcgScript;

	// Use this for initialization
	void Start () 
	{
		bcgScript = FindObjectOfType<BrightnessContrastGamma> ();
		slowmoScript = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<SlowMotionCamera> ();

		brightnessSlider.onValueChanged.AddListener (Brightness);
		contrastSlider.onValueChanged.AddListener (Contrast);
		gammaSlider.onValueChanged.AddListener (Gamma);

		if(PlayerPrefs.HasKey("QualityLevel"))
		{
			qualityToggles [0].isOn = false;
			qualityToggles [1].isOn = false;
			qualityToggles [2].isOn = false;

			switch(PlayerPrefs.GetInt("QualityLevel"))
			{
			case 2:
				qualityToggles [2].isOn = true;
				HighQuality ();
				break;
			case 1:
				qualityToggles [1].isOn = true;
				MediumQuality ();
				break;
			case 0:
				qualityToggles [0].isOn = true;
				LowQuality ();
				break;
			}
		}

		if (PlayerPrefs.HasKey ("Brightness"))
			brightnessSlider.value = PlayerPrefs.GetFloat ("Brightness");

		if (PlayerPrefs.HasKey ("Contrast"))
			contrastSlider.value = PlayerPrefs.GetFloat ("Contrast");

		if (PlayerPrefs.HasKey ("Gamma"))
			gammaSlider.value = PlayerPrefs.GetFloat ("Gamma");
	}

	void OnEnable ()
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Paused)
		{
			slowmoScript = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<SlowMotionCamera> ();
			slowmoScript.StopPauseSlowMotion ();
		}
	}
	
	public void HighQuality ()
	{
		QualitySettings.SetQualityLevel (5, true);
		PlayerPrefs.SetInt ("QualityLevel", 2);
	}

	public void MediumQuality ()
	{
		QualitySettings.SetQualityLevel (4, true);
		PlayerPrefs.SetInt ("QualityLevel", 1);
	}

	public void LowQuality ()
	{
		QualitySettings.SetQualityLevel (3, true);
		PlayerPrefs.SetInt ("QualityLevel", 0);
	}

	void Brightness (float value)
	{
		PlayerPrefs.SetFloat ("Brightness", (float)brightnessSlider.value);
		bcgScript.Brightness = (float)brightnessSlider.value;
	}

	void Contrast (float value)
	{
		PlayerPrefs.SetFloat ("Contrast", (float)contrastSlider.value);
		bcgScript.Contrast = (float)contrastSlider.value;
	}

	void Gamma (float value)
	{
		PlayerPrefs.SetFloat ("Gamma", (float)gammaSlider.value);
		bcgScript.Gamma = (float)gammaSlider.value;
	}

	public void Reset ()
	{
		brightnessSlider.value = 0;
		contrastSlider.value = 0;
		gammaSlider.value = 1;

		qualityToggles [2].isOn = true;
		HighQuality ();
	}
}
