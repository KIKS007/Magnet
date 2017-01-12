using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MovableTag : MovableScript
{
	[Header ("TAG")]
	public TagManager tagManager;
	public GameObject previousOwner;

	protected override void HitPlayer (Collision other)
	{
		if(other.collider.tag == "Player" && gameObject.tag == "ThrownMovable")
		{
			if(playerThatThrew == null || other.gameObject.name != playerThatThrew.name)
			{
				other.gameObject.GetComponent<PlayersGameplay>().StunVoid(true);
				
				playerHit = other.gameObject;
				GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking(SlowMotionType.Stun);
				
				InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);

				if(other.gameObject != previousOwner)
				{
					if (previousOwner != null)
						tagManager.UpdateScores (previousOwner.GetComponent<PlayersGameplay> ().playerName, other.gameObject.GetComponent<PlayersGameplay> ().playerName);
					else
						tagManager.UpdateScores (other.gameObject.GetComponent<PlayersGameplay> ().playerName);					
				}

				ToColor (other.gameObject);
				
				if(playerThatThrew != null && other.gameObject.name != playerThatThrew.name)
					StatsManager.Instance.PlayersFragsAndHits (playerThatThrew, playerHit);

				previousOwner = other.gameObject;
			}
		}
	}

	public override void ToNeutralColor ()
	{
		Color cubeColorTemp = cubeMaterial.GetColor("_Color");
		float cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");

		//Debug.Log ("Neutral Color");

		DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, GlobalVariables.Instance.cubePlayersColor[4], toNeutralDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp)).SetId("CubeNeutralTween" + gameObject.GetInstanceID ());
		DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 0, toNeutralDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp)).SetId("CubeNeutralTween" + gameObject.GetInstanceID ());

		StartCoroutine (WaitToChangeColorEnum (CubeColor.Neutral, toNeutralDuration));
	}

	IEnumerator WaitToChangeColorEnum (CubeColor whichColor, float waitTime)
	{
		yield return new WaitForSeconds (waitTime * 0.5f);		

		if(hold)
			cubeColor = whichColor;

	}

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
}
