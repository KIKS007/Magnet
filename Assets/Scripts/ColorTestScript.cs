using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ColorTestScript : MonoBehaviour 
{
	public enum CubeColor {Neutral, Blue, Pink, Green, Yellow};

	public bool test = false;
	public Color theCUBECOLOR;
	public string player;
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
		cubeMaterial.SetColor ("_Color", GlobalVariables.Instance.cubeNeutralColor);
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

			Color cubeCorrectColor = new Color ();
			CubeColor cubeColorTest = CubeColor.Neutral;

			switch(player)
			{
			case "Player 1":
				cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer1;
				cubeColorTest = CubeColor.Blue;
				break;
			case "Player 2":
				cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer2;
				cubeColorTest = CubeColor.Pink;
				break;
			case "Player 3":
				cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer3;
				cubeColorTest = CubeColor.Green;
				break;
			case "Player 4":
				cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer4;
				cubeColorTest = CubeColor.Yellow;
				break;
			}

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

			DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, GlobalVariables.Instance.cubeNeutralColor, toNeutralDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp)).SetId("CubeNeutralTween");
			DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 0, toNeutralDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp)).OnComplete(()=> cubeColor = CubeColor.Neutral);
		}

		GetComponent<Renderer> ().material.color = cubeMaterial.GetColor ("_Color");
	}
}
