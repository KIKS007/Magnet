using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MovableBomb : MovableScript 
{
	public GameObject playerHolding;

	[Header ("Explosion")]
	public float explosionForce = 10;
	public float explosionRadius = 3;
	public float explosionUpwardsMofifier;
	public LayerMask explosionMask;

	private Renderer playerRenderer;

	protected override void Update () 
	{
		if(hold == false)
			currentVelocity = rigidbodyMovable.velocity.magnitude;


		if(hold == false && currentVelocity > 0)
		{
			if(currentVelocity > higherVelocity)
			{
				higherVelocity = currentVelocity;
			}

			if(currentVelocity >= limitVelocity)
			{
				gameObject.tag = "ThrownMovable";
			}
			else if(currentVelocity < limitVelocity)
			{
				gameObject.tag = "Movable";
				playerThatThrew = null;
			}
		}

		if(hold == true && playerRenderer != null && movableRenderer.material.color != playerRenderer.material.color)
		{
			DOTween.To(()=> movableRenderer.material.color, x=> movableRenderer.material.color =x, playerRenderer.material.color, timeTween);
		}
	}

	protected override void HitPlayer (Collision other)
	{
		if(other.collider.tag == "Player" 
			&& other.collider.GetComponent<PlayersGameplay>().playerState != PlayerState.Stunned 
			&& gameObject.tag == "ThrownMovable" 
			&& playerThatThrew == null)
		{
			StatsManager.Instance.PlayersFragsAndHits (playerThatThrew, other.gameObject);

			//other.gameObject.GetComponent<PlayersGameplay>().StunVoid();
			other.gameObject.GetComponent<PlayerBomb>().GetBomb(GetComponent<Collider>());
			playerHolding = other.gameObject;

			playerHit = other.gameObject;


			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking();

			InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);
		}

		if(other.collider.tag == "Player" 
			&& other.collider.GetComponent<PlayersGameplay>().playerState != PlayerState.Stunned 
			&& gameObject.tag == "ThrownMovable" 
			&& other.gameObject.name != playerThatThrew.name)
		{
			StatsManager.Instance.PlayersFragsAndHits (playerThatThrew, other.gameObject);

			//other.gameObject.GetComponent<PlayersGameplay>().StunVoid();
			other.gameObject.GetComponent<PlayerBomb>().GetBomb(GetComponent<Collider>());
			playerHolding = other.gameObject;

			playerHit = other.gameObject;
		
			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking();

			InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);
		
			playerThatThrew = null;
		}
	}

	public override void OnHold ()
	{
		base.OnHold ();

		playerRenderer = player.GetComponent<Renderer> ();
		playerHolding = player.gameObject;
	}

	public void ResetColor ()
	{
		DOTween.To(()=> movableRenderer.material.color, x=> movableRenderer.material.color =x, Color.white, timeTween);
	}

	public void Explode ()
	{
		Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, explosionMask);

		for(int i = 0; i < colliders.Length; i++)
		{
			Rigidbody rb = colliders [i].GetComponent<Rigidbody> ();

			if (rb != null)
				rb.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpwardsMofifier, ForceMode.Impulse);
		}

		//playerHolding.GetComponent<PlayersGameplay> ().DeathParticles ();
		playerHolding.GetComponent<PlayersGameplay> ().Death ();
		InstantiateExplosionParticles (GlobalVariables.Instance.MovableExplosion, playerRenderer.material.color);
		gameObject.SetActive (false);
	}
}
