using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ColorTestScript : MonoBehaviour 
{
	public enum CubeColor {Neutral, Blue, Pink, Green, Yellow};

	public bool test = false;
	public Color theCUBECOLOR;
	public PlayerName player;
	public Material cubeMaterial;
	public CubeColor cubeColor;

	public float toColorDuration = 0.5f;
	public float toNeutralDuration = 1.5f;

	public bool pause = false;

	// Use this for initialization
	void Start () 
	{
		cubeMaterial = transform.GetChild (1).GetComponent<Renderer> ().material;

		cubeMaterial.SetFloat ("_Lerp", 0);
		cubeMaterial.SetColor ("_Color", GlobalVariables.Instance.playersColor[4]);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(test)
		{
			test = false;

			SetCubeColor ();
		}

		theCUBECOLOR = cubeMaterial.GetColor("_Color");
	}

	protected virtual void SetCubeColor ()
	{
		if(cubeColor == CubeColor.Neutral)
		{
			DOTween.Kill ("CubeNeutralTween");

			int whichPlayer = (int)player;
			CubeColor cubeColorTest = (CubeColor)whichPlayer + 1;
			Color cubeCorrectColor = (GlobalVariables.Instance.playersColor[whichPlayer]);

			Color cubeColorTemp = cubeMaterial.GetColor("_Color");
			float cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");

			DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, cubeCorrectColor, toColorDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp)).SetId("CubeColorTween");
			DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 1, toColorDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp)).OnComplete(()=> cubeColor = cubeColorTest);
		}

		else if(cubeColor != CubeColor.Neutral)
		{
			DOTween.Kill ("CubeColorTween");

			Color cubeColorTemp = cubeMaterial.GetColor("_Color");
			float cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");

			DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, GlobalVariables.Instance.playersColor[4], toNeutralDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp)).SetId("CubeNeutralTween");
			DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 0, toNeutralDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp)).OnComplete(()=> cubeColor = CubeColor.Neutral);
		}

		GetComponent<Renderer> ().material.color = cubeMaterial.GetColor ("_Color");
	}
}
