using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LogoKickSteam : MonoBehaviour 
{
	public string url;

	public void GetToURL ()
	{
		Application.OpenURL (url);
	}
}
