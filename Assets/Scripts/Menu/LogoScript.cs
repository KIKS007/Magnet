using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class LogoScript : MonoBehaviour 
{
	public float durationCube;
	public float durationCube2;

	public float durationPolice;
	public float durationPolice2;

	public Image logoCube;
	public Image logoPolice;

	// Use this for initialization
	void Start () 
	{
		RoseToBlue ();
		LightToDark ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	/*void RoseToBlue ()
	{
		logoCube.DOColor(new Color (255, 197, 233, 255) / 255, durationCube).OnComplete(BlueToRose);
	}

	void BlueToRose ()
	{
		logoCube.DOColor(new Color (239, 99, 255, 255) / 255, durationCube2).OnComplete(RoseToBlue);
	}*/



	void RoseToBlue ()
	{
		logoCube.DOColor(new Color (255, 159, 223, 255) / 255, durationCube).OnComplete(BlueToRose);
	}
	
	void BlueToRose ()
	{
		logoCube.DOColor(new Color (255, 197, 233, 255) / 255, durationCube2).OnComplete(RoseToBlue);
	}


	void LightToDark ()
	{
		logoPolice.DOColor(new Color (140, 140, 140, 255) / 255, durationPolice).OnComplete(DarkToLight);
	}

	void DarkToLight ()
	{
		logoPolice.DOColor(new Color (255, 255, 255, 255) / 255, durationPolice2).OnComplete(LightToDark);
	}
}
