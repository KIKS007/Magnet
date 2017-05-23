using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using GameAnalyticsSDK;

public class ResolutionManager : MonoBehaviour
{
	[Header("Fullscreen")]
	public bool fullScreen = true;

	[Header("Scroll View")]
	public GameObject resolutionContentParent;
	public ToggleGroup resolutionsToggleGroup;
	public float initialYPos = -36f;
	public float gapHeight = 110f;

	[Header("Resolutions")]
	public Text currentResText;
	public GameObject resolutionLinePrefab;
	public List<Vector2> allScreenRes;

	[Header("Ratio")]
	public List<Vector2> allRatios;

	[Header("4:3")]
	public List<Vector2> screenRes1;
	[Header("5:4")]
	public List<Vector2> screenRes2;
	[Header("16:10")]
	public List<Vector2> screenRes3;
	[Header("16:09")]
	public List<Vector2> screenRes4;

	[Header("Settings")]
	public Vector2 currentScreenRes = new Vector2();

	[Header("Toggles")]
	public Toggle fullscreenToggle;
	public Toggle vsyncToggle;

	public List<GameObject> allToggles = new List<GameObject> ();

    void Start()
    {
		Setup();

		StartCoroutine (CheckFullScreenChange (Screen.fullScreen));

		CreateResolutionLines();

		if (!PlayerPrefs.HasKey ("ScreenWidth"))
			FindResolution ();

		SelectToggle ();
    }
		
	void Setup()
	{
		if (PlayerPrefs.HasKey ("Fullscreen"))
		{
			fullScreen = PlayerPrefs.GetInt ("Fullscreen") == 1 ? true : false;
			fullscreenToggle.isOn = fullScreen;
		}

		if (PlayerPrefs.HasKey ("Vsync"))
		{
			QualitySettings.vSyncCount = PlayerPrefs.GetInt ("Vsync");
			vsyncToggle.isOn = PlayerPrefs.GetInt ("Vsync") == 0 ? false : true;
		}

		if (PlayerPrefs.HasKey ("ScreenWidth"))
			SetResolution (new Vector2(PlayerPrefs.GetInt ("ScreenWidth"), PlayerPrefs.GetInt ("ScreenHeight")));

		if (PlayerPrefs.HasKey ("ScreenWidth"))
			Debug.Log ("Resolution Loaded : " + currentScreenRes.x + "x" + currentScreenRes.y);

	}

	void FindResolution ()
	{
		bool resolutionFound = false;
		Vector2 currentResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

		foreach(Vector2 v in allScreenRes)
		{
			if(Mathf.Approximately (currentResolution.x, v.x) && Mathf.Approximately (currentResolution.y, v.y))
			{
				currentScreenRes = v;
				SetResolution (v);
				resolutionFound = true;
				break;
			}
		}

		if(!resolutionFound)
		{
			currentScreenRes = new Vector2 (1920, 1080);
			SetResolution (currentScreenRes);
		}

		Debug.Log (Screen.currentResolution);
		Debug.Log ("Res found : " + currentScreenRes.x + "x" + currentScreenRes.y);

//		foreach (Resolution r in Screen.resolutions)
//			Debug.Log (r.width + " x " + r.height);
	}

	void SelectToggle ()
	{
		foreach(GameObject g in allToggles)
		{
			Text res = g.transform.GetChild (1).GetComponent<Text> ();

			if(res.text == currentScreenRes.x + "x" + currentScreenRes.y)
			{
				g.GetComponent<Toggle> ().isOn = true;
				break;
			}
		}	
	}

	void CreateResolutionLines ()
	{
		allScreenRes.AddRange (screenRes1);
		allScreenRes.AddRange (screenRes2);
		allScreenRes.AddRange (screenRes3);
		allScreenRes.AddRange (screenRes4);

		allToggles.Clear ();

		List<Vector2> allResTemp = new List<Vector2> (allScreenRes);
		int resCount = 0;

		resolutionContentParent.GetComponent<RectTransform> ().sizeDelta = new Vector2 (resolutionContentParent.GetComponent<RectTransform> ().sizeDelta.x, 50 + gapHeight * allScreenRes.Count);

		while(allResTemp.Count != 0)
		{
			Vector2 smallestRes = allResTemp [0];

			foreach(Vector2 v in allResTemp)
			{
				if (v.y < smallestRes.y)
					smallestRes = v;
			}

			allResTemp.Remove (smallestRes);

			Vector3 pos = Vector3.zero;
			pos.y = -gapHeight * resCount + initialYPos;
			pos.z = 0;

			GameObject resLine = Instantiate (resolutionLinePrefab, resolutionLinePrefab.transform.position, resolutionLinePrefab.transform.rotation, resolutionContentParent.transform) as GameObject;

			allToggles.Add (resLine);

			resLine.GetComponent<Toggle> ().group = resolutionsToggleGroup;

			resLine.GetComponent<Toggle> ().onValueChanged.AddListener ( (bool arg0) => { if(arg0) SetResolution (new Vector2 (smallestRes.x, smallestRes.y)); } );

			resLine.GetComponent<RectTransform> ().anchoredPosition3D = pos;
			resLine.transform.GetChild (1).GetComponent<Text> ().text = smallestRes.x + "x" + smallestRes.y;
			resLine.transform.GetChild (2).GetComponent<Text> ().text = FindRatio (smallestRes);

			resCount++;
		}
	}

	string FindRatio (Vector2 resolution)
	{
		float testedRatio = resolution.x / resolution.y;
		string ratioText = "";

		foreach(Vector2 ratio in allRatios)
		{
			if(Mathf.Abs (testedRatio - (ratio.x / ratio.y)) < 0.01f)
			{
				ratioText = ratio.x + ":" + ratio.y;
				break;
			}
		}

		return ratioText;
	}

	void SetResolution (Vector2 res)
	{
		Debug.Log ("New Resolution : " + (int)res.x + " x " + (int)res.y);

		currentScreenRes = res;
		Screen.SetResolution ((int)res.x, (int)res.y, fullScreen);

		currentResText.text = (int)res.x + " x " + (int)res.y;

		PlayerPrefs.SetInt ("ScreenWidth", (int)res.x);
		PlayerPrefs.SetInt ("ScreenHeight", (int)res.y);
	}

	public void ToggleFullscreen()
	{
		fullScreen = !fullScreen;
		Screen.SetResolution ((int)currentScreenRes.x, (int)currentScreenRes.y, fullScreen);

		PlayerPrefs.SetInt ("Fullscreen", fullScreen ? 1 : 0);
	}

	public void ToggleVsync ()
	{
		if (QualitySettings.vSyncCount == 0)
			QualitySettings.vSyncCount = 1;
		else
			QualitySettings.vSyncCount = 0;

		PlayerPrefs.SetInt ("Vsync", QualitySettings.vSyncCount);
	}

	IEnumerator CheckFullScreenChange (bool fullscreen)
	{
		yield return new WaitUntil (()=> Screen.fullScreen != fullscreen);

		if(Screen.fullScreen == true && PlayerPrefs.GetInt ("Fullscreen") == 0)
			fullscreenToggle.isOn = false;

		if(Screen.fullScreen == false && PlayerPrefs.GetInt ("Fullscreen") == 1)
			fullscreenToggle.isOn = true;

		StartCoroutine (CheckFullScreenChange (Screen.fullScreen));
	}
}