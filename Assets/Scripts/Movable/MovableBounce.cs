using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovableBounce : MovableScript 
{
	[Header ("Explosion")]
	public float explosionForce = 50;
	public float explosionRadius = 50;
	public LayerMask explosionMask = (1 << 9) | (1 << 12);

	protected override void Update () 
	{
		if(hold == false && rigidbodyMovable != null)
			currentVelocity = rigidbodyMovable.velocity.magnitude;


		if(hold == false && currentVelocity > 0)
		{
			if(currentVelocity > higherVelocity)
				higherVelocity = currentVelocity;

			else if(currentVelocity < limitVelocity && gameObject.tag == "DeadCube")
			{
				slowMoTrigger.triggerEnabled = false;
				gameObject.tag = "Movable";
				playerThatThrew = null;

				ToNeutralColor ();
			}
		}
	}

	protected override void HitWall (Collision other)
	{
		if(other.gameObject.tag == "Wall" && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			if(currentVelocity > (limitVelocity * 0.5f))
				InstantiateImpactFX (other.contacts [0]);

			if(canPlaySound)
				StartCoroutine(HitSound ());

			StartCoroutine (DeadlyTransition ());
		}
	}

	IEnumerator DeadlyTransition ()
	{
		if (DOTween.IsTweening ("CubeNeutralTween" + gameObject.GetInstanceID ()))
			DOTween.Kill ("CubeNeutralTween" + gameObject.GetInstanceID ());

		Color cubeColorTemp = cubeMaterial.GetColor("_Color");
		float cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");

		DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, Color.black, toColorDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp)).SetId("CubeColorTween" + gameObject.GetInstanceID ());
		DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 1, toColorDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp)).SetId("CubeColorTween" + gameObject.GetInstanceID ());

		yield return new WaitForSeconds (0.01f);

		tag = "DeadCube";
	}
}
