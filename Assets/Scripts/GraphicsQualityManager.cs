using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GraphicsQualityManager : MonoBehaviour 
{
	public Toggle[] qualityToggles = new Toggle[3];

	private SlowMotionCamera slowmoScript;

	// Use this for initialization
	void Start () 
	{
		slowmoScript = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<SlowMotionCamera> ();

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
		Debug.Log ("high");
		//slowmoScript.initialFixedDelta = 0.005f;
		//Time.fixedDeltaTime = 0.005f;
		QualitySettings.SetQualityLevel (5, true);
		PlayerPrefs.SetInt ("QualityLevel", 2);
	}

	public void MediumQuality ()
	{
		Debug.Log ("medium");
		//slowmoScript.initialFixedDelta = 0.0075f;
		//Time.fixedDeltaTime = 0.0075f;
		QualitySettings.SetQualityLevel (4, true);
		PlayerPrefs.SetInt ("QualityLevel", 1);
	}

	public void LowQuality ()
	{
		Debug.Log ("low");
		//slowmoScript.initialFixedDelta = 0.01f;
		//Time.fixedDeltaTime = 0.01f;
		QualitySettings.SetQualityLevel (3, true);
		PlayerPrefs.SetInt ("QualityLevel", 0);
	}
}
