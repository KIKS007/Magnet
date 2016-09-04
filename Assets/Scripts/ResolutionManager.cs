using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ResolutionManager : Singleton<ResolutionManager>
{
	public Vector2 currentScreenRes = new Vector2();

	public GameObject[] toggleScreenText = new GameObject[2];

    // Fixed aspect ratio parameters
    static public bool FixedAspectRatio = true;
    static public float TargetAspectRatio = 16f / 9f;

    // List of horizontal resolutions to include
    int[] resolutions = new int[] {1024, 1152, 1280, 1366, 1600, 1920};

    public Resolution DisplayResolution;
	public List<Vector2> ScreenResolutions;

	public Toggle[] resToggles = new Toggle[6];

    int screenResIndex;

    void Start()
    {
		InitResolutions();

		Setup();

		SetCurrentRes ();

		SetCorrectToggle ();

		//printResolution ();
    }

	void Setup()
	{
		if(Screen.resolutions.Length > 1)
		{
			DisplayResolution = Screen.resolutions[Screen.resolutions.Length - 1];

			Screen.SetResolution(DisplayResolution.width, DisplayResolution.height, true);

			FindCorrectResolution ();
		}
		else
		{
			DisplayResolution.width = 1920;
			DisplayResolution.height = 1080;

			screenResIndex = 5;

			Screen.SetResolution(DisplayResolution.width, DisplayResolution.height, true);
		}	


		if(Screen.fullScreen)
		{
			toggleScreenText [0].SetActive (false);
			toggleScreenText [1].SetActive (true);
		}
		else
		{
			toggleScreenText [0].SetActive (true);
			toggleScreenText [1].SetActive (false);
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

	void FindCorrectResolution ()
	{
		screenResIndex = ScreenResolutions.Count - 1;

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

	}


	void SetCurrentRes ()
	{
		currentScreenRes = new Vector2 (ScreenResolutions [screenResIndex].x, ScreenResolutions [screenResIndex].y);
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
			resToggles [screenResIndex].isOn = false;
		}
			
		resToggles [screenResIndex].isOn = true;
	}

	public void SetResolution(int index)
    {
        Vector2 r = new Vector2();
     
        screenResIndex = index;
        r = ScreenResolutions[screenResIndex];

        Debug.Log("Setting resolution to " + (int)r.x + "x" + (int)r.y);
		Screen.SetResolution((int)r.x, (int)r.y, Screen.fullScreen);

		SetCurrentRes ();
    }

    public void ToggleFullscreen()
    {
		if(Screen.fullScreen)
		{
			Screen.SetResolution((int)ScreenResolutions[screenResIndex].x, (int)ScreenResolutions[screenResIndex].y, false);

			toggleScreenText [0].SetActive (false);
			toggleScreenText [1].SetActive (true);
		}
		else
		{
			Screen.SetResolution((int)ScreenResolutions[screenResIndex].x, (int)ScreenResolutions[screenResIndex].y, true);

			toggleScreenText [0].SetActive (true);
			toggleScreenText [1].SetActive (false);
		}

    }
}