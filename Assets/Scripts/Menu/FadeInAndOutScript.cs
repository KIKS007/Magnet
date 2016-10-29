using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class FadeInAndOutScript : MonoBehaviour 
{
	public float durationFadeIn;
	public float durationFadeOut;

	[Range(0,255)]
	public float alphaMinimum;


	private Text textComponent;
	private Image imageComponent;
	private Color originalColor;

	public bool tweening = false;

	// Use this for initialization
	void Start () 
	{
		DOTween.Init ();

		if(GetComponent<Text>() != null)
		{
			textComponent = GetComponent<Text>();
			originalColor = textComponent.color;
		}

		if(GetComponent<Image>() != null)
		{
			imageComponent = GetComponent<Image>();
			originalColor = imageComponent.color;
		}

		FadeOut ();
		tweening = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (gameObject.activeInHierarchy == false && tweening)
			tweening = false;

		if (gameObject.activeInHierarchy == true && !tweening)
		{
			FadeOut ();
			tweening = true;
		}
	}

	void FadeOut ()
	{
		if (tweening == true)
		{
			if(GetComponent<Text>() != null)
			{
				textComponent.DOColor (new Color (originalColor.r, originalColor.g, originalColor.b, alphaMinimum / 255), durationFadeOut).OnComplete (FadeIn);
			}
			
			if(GetComponent<Image>() != null)
			{
				imageComponent.DOColor (new Color (originalColor.r, originalColor.g, originalColor.b, alphaMinimum / 255), durationFadeOut).OnComplete (FadeIn);
			}			
		}
	}

	void FadeIn ()
	{
		if (tweening == true)
		{
			if(GetComponent<Text>() != null)
			{
				textComponent.DOColor (new Color (originalColor.r, originalColor.g, originalColor.b, 255 / 255), durationFadeIn).OnComplete (FadeOut);
			}
			
			if(GetComponent<Image>() != null)
			{
				imageComponent.DOColor (new Color (originalColor.r, originalColor.g, originalColor.b, 255 / 255), durationFadeIn).OnComplete (FadeOut);
			}			
		}
	}	
}
