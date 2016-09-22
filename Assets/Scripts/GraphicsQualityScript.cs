using UnityEngine;
using System.Collections;

public class GraphicsQualityScript : MonoBehaviour 
{
	private SlowMotionCamera slowmoScript;
	// Use this for initialization
	void Start () 
	{
		slowmoScript = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<SlowMotionCamera> ();
	}

	void OnEnable ()
	{
		slowmoScript = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<SlowMotionCamera> ();
		slowmoScript.StopPauseSlowMotion ();
	}
	
	public void HighQuality ()
	{
		slowmoScript.initialFixedDelta = 0.005f;
		Time.fixedDeltaTime = 0.005f;
	}

	public void MediumQuality ()
	{
		slowmoScript.initialFixedDelta = 0.0075f;
		Time.fixedDeltaTime = 0.0075f;
	}

	public void LowQuality ()
	{
		slowmoScript.initialFixedDelta = 0.01f;
		Time.fixedDeltaTime = 0.01f;
	}
}
