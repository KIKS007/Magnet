using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using GameAnalyticsSDK;

public class ResolutionManager : Singleton<ResolutionManager>
{
	public Vector2 currentScreenRes = new Vector2();

    // Fixed aspect ratio parameters
    static public bool FixedAspectRatio = true;
    static public float TargetAspectRatio = 16f / 9f;

    // List of horizontal resolutions to include
    private int[] resolutions = new int[] {1024, 1152, 1280, 1366, 1600, 1920};

    public Resolution DisplayResolution;
	public List<Vector2> ScreenResolutions;

	public Toggle[] resToggles = new Toggle[6];
	public Toggle windowedToggle;

	private int screenResIndex;

    void Start()
    {
		InitResolutions();

		//Setup();

		if(!PlayerPrefs.HasKey ("ScreenResIndex"))
		{
			if (Screen.resolutions.Length > 1)
				FindCorrectResolution ();
			else
				SetupEditorResolution ();
		}
		else
		{
			screenResIndex = PlayerPrefs.GetInt ("ScreenResIndex");

			if(PlayerPrefs.GetInt ("Fullscreen") == 1)
			{
				windowedToggle.isOn = false;
				SetResolution (screenResIndex, true);
			}
			else
			{
				windowedToggle.isOn = true;
				SetResolution (screenResIndex, false);
			}
		}

		SetCurrentRes ();

		SetCorrectToggle ();

		//printResolution ();
    }

	void Update ()
	{
		if(Screen.fullScreen == true && PlayerPrefs.GetInt ("Fullscreen") == 0 && windowedToggle.isOn == true)
		{
			SetResolution (screenResIndex, true);
			windowedToggle.isOn = false;
		}

		if(Screen.fullScreen == false && PlayerPrefs.GetInt ("Fullscreen") == 1 && windowedToggle.isOn == false)
		{
			SetResolution (screenResIndex, false);
			windowedToggle.isOn = true;
		}
	}

	void InitResolutions()
	{
		float screenAspect = TargetAspectRatio;

		ScreenResolutions = new List<Vector2>();

		foreach (int w in resolutions)
		{
			ScreenResolutions.Add(new Vector2(w, Mathf.Round(w / screenAspect)));
		}

		ScreenResolutions = ScreenResolutions.OrderBy(resolution => resolution.x).ToList();
	}

	void Setup()
	{
		if(Screen.resolutions.Length > 1)
		{
			DisplayResolution = Screen.resolutions[Screen.resolutions.Length - 1];

			SetResolution (5, true);

			FindCorrectResolution ();
		}
		else
		{
			DisplayResolution.width = 1920;
			DisplayResolution.height = 1080;

			screenResIndex = 5;
			PlayerPrefs.SetInt ("ScreenResIndex", 5);

			if(PlayerPrefs.HasKey ("Fullscreen") && PlayerPrefs.GetInt ("Fullscreen") == 0)
			{
				SetResolution (5, false);
				PlayerPrefs.SetInt ("Fullscreen", 0);
			}

			else
			{
				SetResolution (5, true);
				PlayerPrefs.SetInt ("Fullscreen", 1);
			}
		}	
	}

	void SetupEditorResolution ()
	{
		DisplayResolution.width = 1920;
		DisplayResolution.height = 1080;

		screenResIndex = 5;
		PlayerPrefs.SetInt ("ScreenResIndex", 5);

		if(PlayerPrefs.HasKey ("Fullscreen") && PlayerPrefs.GetInt ("Fullscreen") == 0)
		{
			SetResolution (5, false);
			PlayerPrefs.SetInt ("Fullscreen", 0);
		}

		else
		{
			SetResolution (5, true);
			PlayerPrefs.SetInt ("Fullscreen", 1);
		}
	}

	void FindCorrectResolution ()
	{
		DisplayResolution = Screen.resolutions[Screen.resolutions.Length - 1];
		screenResIndex = ScreenResolutions.Count - 1;
		//SetResolution (5, true);

		bool exactRes = false;
		float screenResDifference = 5000;

		for (int i = 0; i < ScreenResolutions.Count; i++)
		{
			if(DisplayResolution.width == ScreenResolutions[i].x)
			{
				screenResIndex = i;

				exactRes = true;
				Debug.Log ("Exact Res with " + ScreenResolutions[i].x + " width");
				break;
			}

			if (Mathf.Abs(DisplayResolution.width - ScreenResolutions[i].x) < screenResDifference)
			{
				screenResDifference = Mathf.Abs (DisplayResolution.width - ScreenResolutions [i].x);
				screenResIndex = i;
			}
		}

		if(!exactRes)
			Debug.Log("Closest Res with " + ScreenResolutions[screenResIndex].x + " width");

		GameAnalytics.NewDesignEvent ("Menu:" + "Resolution:" + ScreenResolutions[screenResIndex].x.ToString());
		Debug.Log("Menu:" + "Resolution:" + ScreenResolutions[screenResIndex].x.ToString());


		if(PlayerPrefs.HasKey ("Fullscreen") && PlayerPrefs.GetInt ("Fullscreen") == 1)
			SetResolution (screenResIndex, false);

		else
			SetResolution (screenResIndex, true);
	}

	void SetCurrentRes ()
	{
		currentScreenRes = new Vector2 (ScreenResolutions [screenResIndex].x, ScreenResolutions [screenResIndex].y);
		PlayerPrefs.SetInt ("ScreenResIndex", screenResIndex);
	}

    void printResolution()
    {
		Debug.Log("Screen res: " + DisplayResolution.width + "x" + DisplayResolution.height);
		Debug.Log ("Current res: " + ScreenResolutions[screenResIndex].x + "x" + ScreenResolutions[screenResIndex].y);
    }

	void SetCorrectToggle ()
	{
		for(int i = 0; i < resToggles.Length; i++)
		{
			resToggles [i].isOn = false;
		}
			
		resToggles [screenResIndex].isOn = true;
	}

	void SetResolution(int index, bool fullScreen)
    {
        Vector2 r = new Vector2();
     
        screenResIndex = index;
        r = ScreenResolutions[screenResIndex];

		PlayerPrefs.SetInt ("ScreenResIndex", screenResIndex);

       // Debug.Log("Setting resolution to " + (int)r.x + "x" + (int)r.y);

		Screen.SetResolution((int)r.x, (int)r.y, fullScreen);

		if(fullScreen)
			PlayerPrefs.SetInt ("Fullscreen", 1);
		else
			PlayerPrefs.SetInt ("Fullscreen", 0);

		SetCurrentRes ();
    }

	public void SetResolution(int index)
	{
		Vector2 r = new Vector2();

		screenResIndex = index;
		r = ScreenResolutions[screenResIndex];

		PlayerPrefs.SetInt ("ScreenResIndex", screenResIndex);

		// Debug.Log("Setting resolution to " + (int)r.x + "x" + (int)r.y);

		if(PlayerPrefs.GetInt ("Fullscreen") == 1)
		{
			Screen.SetResolution((int)r.x, (int)r.y, true);
			PlayerPrefs.SetInt ("Fullscreen", 1);
		}
		else
		{
			Screen.SetResolution((int)r.x, (int)r.y, false);
			PlayerPrefs.SetInt ("Fullscreen", 0);
		}

		SetCurrentRes ();
	}

    public void ToggleFullscreen()
    {
		if(Screen.fullScreen)
		{
			Screen.SetResolution((int)ScreenResolutions[screenResIndex].x, (int)ScreenResolutions[screenResIndex].y, false);
			PlayerPrefs.SetInt ("Fullscreen", 0);
		}
		else
		{
			Screen.SetResolution((int)ScreenResolutions[screenResIndex].x, (int)ScreenResolutions[screenResIndex].y, true);
			PlayerPrefs.SetInt ("Fullscreen", 1);
		}
    }
}