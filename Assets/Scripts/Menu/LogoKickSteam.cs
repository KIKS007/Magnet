using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

public class LogoKickSteam : EventTrigger 
{
	public void GetToURL (string url)
	{
		if(url != "")
		Application.OpenURL (url);
	}
}
