﻿using UnityEngine;
using System.Collections;

public class SpawnSingletonManager : MonoBehaviour 
{
	void Awake ()
	{
		Debug.Log(LoadModeManager.Instance.gameObject);
		Debug.Log(GlobalVariables.Instance.gameObject);
		Debug.Log(GamepadsManager.Instance.gameObject);
		Debug.Log(VibrationManager.Instance.gameObject);
		Debug.Log(StatsManager.Instance.gameObject);
	}

}
