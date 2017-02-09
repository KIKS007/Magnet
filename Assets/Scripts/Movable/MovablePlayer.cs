using UnityEngine;
using System.Collections;
using DG.Tweening;
using DarkTonic.MasterAudio;

public class MovablePlayer : MovableScript 
{
	[Header ("Explosion")]
	public float explosionForce = 20;
	public float explosionRadius = 50;
	public LayerMask explosionMask;

	[HideInInspector]
	public bool basicMovable = true;

	protected override void Update ()
	{
		if(hold == false && rigidbodyMovable != null)
			currentVelocity = rigidbodyMovable.velocity.magnitude;

		if(hold == false && currentVelocity > 0)
		{
			if(currentVelocity > higherVelocity)
				higherVelocity = currentVelocity;
		}	

		if(basicMovable)
		{
			if(hold == false && currentVelocity > 0)
			{
				if(currentVelocity >= limitVelocity)
					gameObject.tag = "ThrownMovable";
				
				else if(currentVelocity < limitVelocity && gameObject.tag == "ThrownMovable")
				{
					slowMoTrigger.triggerEnabled = false;
					gameObject.tag = "Movable";
					playerThatThrew = null;
				}
			}			
		}
	}

	protected override void HitPlayer (Collision other)
	{
		if(tag != "Suggestible")
			base.HitPlayer (other);

		else
		{
			if(other.collider.tag == "Player" 
				&& other.collider.GetComponent<PlayersGameplay>().playerState != PlayerState.Dead)
			{
				InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);

				GlobalMethods.Instance.Explosion (transform.position, explosionForce, explosionRadius, explosionMask);
				GlobalMethods.Instance.ExplosionFX (other.gameObject, transform.position);

				other.gameObject.GetComponent<PlayersFXAnimations> ().DeathParticles (other.contacts [0].point);
				other.gameObject.GetComponent<PlayersGameplay> ().Death ();
			}
		}
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
