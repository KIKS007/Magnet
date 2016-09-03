using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ResolutionManager : Singleton<ResolutionManager>
{
	public Vector2 currentScreenRes = new Vector2();

	public GameObject[] toggleScreenText = new GameObject[2];

    static public ResolutionManager Instance;

    // Fixed aspect ratio parameters
    static public bool FixedAspectRatio = true;
    static public float TargetAspectRatio = 16f / 9f;

    // Windowed aspect ratio when FixedAspectRatio is false
    static public float WindowedAspectRatio = 16f / 9f;

    // List of horizontal resolutions to include
    int[] resolutions = new int[] {1024, 1152, 1280, 1366, 1600, 1920};

    public Resolution DisplayResolution;
    public List<Vector2> ScreenResolutions;

	public Toggle[] resToggles = new Toggle[6];

    int screenResIndex;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartRoutine();

		for(int i = 0; i < Screen.resolutions.Length; i++)
		{
			Debug.Log (Screen.resolutions[i]);
		}
    }

	void SetCurrentRes ()
	{
		currentScreenRes = new Vector2 (ScreenResolutions [screenResIndex].x, ScreenResolutions [screenResIndex].y);
	}

    private void printResolution()
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

	private void StartRoutine()
    {
        if (Application.platform == RuntimePlatform.OSXPlayer)
        {
            DisplayResolution = Screen.currentResolution;
        }
        else
        {
			if(Screen.resolutions.Length > 1)
			{
				DisplayResolution = Screen.resolutions[Screen.resolutions.Length - 1];

				Screen.SetResolution(DisplayResolution.width, DisplayResolution.height, true);
			}
			else
			{
				DisplayResolution.width = 1920;
				DisplayResolution.height = 1080;

				Screen.SetResolution(DisplayResolution.width, DisplayResolution.height, true);
			}
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

        InitResolutions();
    }

    private void InitResolutions()
    {
        float screenAspect = (float)DisplayResolution.width / DisplayResolution.height;

        ScreenResolutions = new List<Vector2>();

        foreach (int w in resolutions)
        {
            if (w < DisplayResolution.width)
            {
				// Adding resolution only if it's 20% smaller than the screen OLD
                // Adding resolution only if it's 10% smaller than the screen NEW
                if (w < DisplayResolution.width * 0.9f)
                {
                    ScreenResolutions.Add(new Vector2(w, Mathf.Round(w / screenAspect)));
                }
            }
        }

        // Adding fullscreen native resolution
        ScreenResolutions.Add(new Vector2(DisplayResolution.width, DisplayResolution.height));

        // Adding half fullscreen native resolution
        Vector2 halfNative = new Vector2(DisplayResolution.width * 0.5f, DisplayResolution.height * 0.5f);
        if (halfNative.x > resolutions[0] && ScreenResolutions.IndexOf(halfNative) == -1)
            ScreenResolutions.Add(halfNative);

        ScreenResolutions = ScreenResolutions.OrderBy(resolution => resolution.x).ToList();

        bool found = false;


		screenResIndex = ScreenResolutions.Count - 1;

		for (int i = 0; i < ScreenResolutions.Count; i++)
		{
			if (ScreenResolutions[i].x == Screen.width && ScreenResolutions[i].y == Screen.height)
			{
				screenResIndex = i;
				found = true;
				break;
			}
		}

		if (!found)
			SetResolution(ScreenResolutions.Count - 1);

		SetCurrentRes ();

		SetCorrectToggle ();

		printResolution ();
    }

	public void SetResolution(int index)
    {
		bool fullscreen = Screen.fullScreen;			

        Vector2 r = new Vector2();
     
        screenResIndex = index;
        r = ScreenResolutions[screenResIndex];

        bool fullscreen2windowed = Screen.fullScreen & !fullscreen;

        Debug.Log("Setting resolution to " + (int)r.x + "x" + (int)r.y);
        Screen.SetResolution((int)r.x, (int)r.y, fullscreen);

		SetCurrentRes ();

        // On OSX the application will pass from fullscreen to windowed with an animated transition of a couple of seconds.
        // After this transition, the first time you exit fullscreen you have to call SetResolution again to ensure that the window is resized correctly.
        if (Application.platform == RuntimePlatform.OSXPlayer)
        {
            // Ensure that there is no SetResolutionAfterResize coroutine running and waiting for screen size changes
            StopAllCoroutines();

            // Resize the window again after the end of the resize transition
            if (fullscreen2windowed) StartCoroutine(SetResolutionAfterResize(r));
        }

    }

    private IEnumerator SetResolutionAfterResize(Vector2 r)
    {
        int maxTime = 5; // Max wait for the end of the resize transition
        float time = Time.time;

        // Skipping a couple of frames during which the screen size will change
        yield return null;
        yield return null;

        int lastW = Screen.width;
        int lastH = Screen.height;

        // Waiting for another screen size change at the end of the transition animation
        while (Time.time - time < maxTime)
        {
            if (lastW != Screen.width || lastH != Screen.height)
            {
                Debug.Log("Resize! " + Screen.width + "x" + Screen.height);

                Screen.SetResolution((int)r.x, (int)r.y, Screen.fullScreen);
                yield break;
            }

            yield return null;
        }

        Debug.Log("End waiting");
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