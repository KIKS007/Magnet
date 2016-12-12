using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MovableDeadCube : MovableScript 
{
	public override void OnHold ()
	{
		hold = true;

		attracedBy.Clear ();
		repulsedBy.Clear ();

		OnHoldEventVoid ();
	}

	public override void OnRelease ()
	{
		OnReleaseEventVoid ();
	}

	public void ToColor (PlayerName playerName)
	{
		int whichPlayer = (int)playerName;

		CubeColor cubeColorTest = (CubeColor)whichPlayer + 1;
		Color cubeCorrectColor = (GlobalVariables.Instance.cubePlayersColor[whichPlayer]);

		if(cubeMaterial.GetColor("_Color") != cubeCorrectColor)
		{
			if (DOTween.IsTweening ("CubeNeutralTween" + gameObject.GetInstanceID ()))
				DOTween.Kill ("CubeNeutralTween" + gameObject.GetInstanceID ());

			//Debug.Log ("New Color : " + cubeCorrectColor);

			Color cubeColorTemp = cubeMaterial.GetColor("_Color");
			float cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");

			DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, cubeCorrectColor, toColorDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp)).SetId("CubeColorTween" + gameObject.GetInstanceID ());
			DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 1, toColorDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp)).SetId("CubeColorTween" + gameObject.GetInstanceID ());

			StartCoroutine (WaitToChangeColorEnum (cubeColorTest, toColorDuration));
		}
	}

	IEnumerator WaitToChangeColorEnum (CubeColor whichColor, float waitTime)
	{
		yield return new WaitForSeconds (waitTime * 0.5f);		

		if(hold)
			cubeColor = whichColor;

	}
}
