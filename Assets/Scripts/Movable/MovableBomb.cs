using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MovableBomb : MovableScript 
{
	public GameObject playerHolding;

	[Header ("Explosion")]
	public float explosionForce = 10;
	public float explosionRadius = 3;
	public LayerMask explosionMask;

	public float getToPlayerForce = 2;

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
			&& gameObject.tag == "Movable")
		{
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

	public IEnumerator Explode ()
	{
		if (!hold)
		{
			yield return StartCoroutine (GetToPlayerPosition ());
		}

		GlobalMethods.Instance.Explosion (transform.position, explosionForce, explosionRadius, explosionMask);

		//playerHolding.GetComponent<PlayersGameplay> ().DeathParticles ();
		playerHolding.GetComponent<PlayersGameplay> ().Death ();
		InstantiateExplosionParticles (GlobalVariables.Instance.MovableExplosion, playerRenderer.material.color);
		gameObject.SetActive (false);

		playerHolding = null;
	}

	IEnumerator GetToPlayerPosition ()
	{
		while(Vector3.Distance(playerHolding.transform.position, transform.position) > 2)
		{
			if (!hold)
			{
				//transform.position = Vector3.Lerp(transform.position, playerHolding.transform.position, 0.1f);
				Vector3 direction = playerHolding.transform.position - transform.position;

				rigidbodyMovable.AddForce(direction * getToPlayerForce, ForceMode.Acceleration);

				yield return null;
			}
			else
				break;
		}
	}
}
