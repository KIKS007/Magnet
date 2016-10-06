using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class LogoScript : MonoBehaviour 
{
	[Header ("Cube Effect")]
	public Image logoCube;
	public bool enableCubeEffect = true;
	public Color cubeInitialColor;
	public Color cubeModifiedColor;
	public float durationCube;
	public float durationCube2;

	[Header ("Font Effect")]
	public Image logoPolice;
	public bool enableFontEffect = true;
	public Color fontInitialColor;
	public Color fontModifiedColor;
	public float durationPolice;
	public float durationPolice2;


	// Use this for initialization
	void Start () 
	{
		CubeNewColor ();
		FontNewColor ();
	}

	void CubeNewColor ()
	{
		if(enableCubeEffect)
			logoCube.DOColor(cubeModifiedColor, durationCube).OnComplete(CubeInitialColor);
	}
	
	void CubeInitialColor ()
	{
		if(enableCubeEffect)
			logoCube.DOColor(cubeInitialColor, durationCube2).OnComplete(CubeNewColor);
	}


	void FontNewColor ()
	{
		if(enableFontEffect)
			logoPolice.DOColor(fontModifiedColor, durationPolice).OnComplete(FontInitialColor);
	}

	void FontInitialColor ()
	{
		if(enableFontEffect)
			logoPolice.DOColor(fontInitialColor, durationPolice2).OnComplete(FontNewColor);
	}
}
