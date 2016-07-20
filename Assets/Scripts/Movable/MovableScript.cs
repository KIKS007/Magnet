using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MovableScript : MonoBehaviour 
{
	public float higherVelocity;
	public float currentVelocity;
	public float limitVelocity = 170f;

	public bool hold;

	public Ease easetype;
	public float timeTween;

	protected Rigidbody rigibodyMovable;

	protected float massRb;
	protected CollisionDetectionMode collisionDetectionModeRb;


	[HideInInspector]
	public Transform player;
	[HideInInspector]
	public GameObject playerThatThrew;
	[HideInInspector]
	public GameObject playerHit;

	// Use this for initialization
	protected virtual void Start () 
	{
		rigibodyMovable = GetComponent<Rigidbody>();

		massRb = rigibodyMovable.mass;
		collisionDetectionModeRb = rigibodyMovable.collisionDetectionMode;
	}
	
	// Update is called once per frame
	protected virtual void Update () 
	{
		if(hold == false)
			currentVelocity = rigibodyMovable.velocity.magnitude;


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
			}
		}

		if(hold == true && gameObject.GetComponent<Renderer>().material.color == Color.white)
		{
			DOTween.To(()=> gameObject.GetComponent<Renderer>().material.color, x=> gameObject.GetComponent<Renderer>().material.color =x, transform.parent.GetComponent<Renderer>().material.color, timeTween).SetEase(easetype);
		}

		if(hold == false && gameObject.GetComponent<Renderer>().material.color != Color.white && gameObject.tag == "Movable")
		{
			DOTween.To(()=> gameObject.GetComponent<Renderer>().material.color, x=> gameObject.GetComponent<Renderer>().material.color =x, Color.white, timeTween).SetEase(easetype);
		}



	}

	protected virtual void OnCollisionEnter (Collision other)
	{
		if(other.collider.tag != "HoldMovable")
		{
			HitPlayer (other);			
		}			
		

		if(other.gameObject.tag == "Movable")
		{
			HitOtherMovable (other);
		}

		//Touched Wall
		if(other.gameObject.layer == 16)
		{
			HitWall (other);
		}
	}

	protected virtual void HitPlayer (Collision other)
	{
		if(other.collider.tag == "Player" 
			&& other.collider.GetComponent<PlayersGameplay>().playerState != PlayerState.Stunned 
			&& gameObject.tag == "ThrownMovable" 
			&& other.gameObject.name != playerThatThrew.name)
		{
			other.gameObject.GetComponent<PlayersGameplay>().StunVoid();

			playerHit = other.gameObject;
			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking();

			InstantiateParticles (other.contacts [0], StaticVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);

			StatsManager.Instance.PlayersFragsAndHits (playerThatThrew, playerHit);
		}
	}

	protected virtual void HitOtherMovable (Collision other)
	{
		float numberOfParticlesFloat = (0.2f * rigibodyMovable.velocity.magnitude);
		int numberOfParticles = (int) numberOfParticlesFloat;

		GameObject instantiatedParticles = InstantiateParticles (other.contacts [0], StaticVariables.Instance.WallHitParticles, gameObject.GetComponent<Renderer> ().material.color);

		instantiatedParticles.GetComponent<ParticleSystem>().startSize += (gameObject.transform.lossyScale.x * 0.1f);
		instantiatedParticles.GetComponent<ParticleSystem>().Emit(numberOfParticles);
	}

	protected virtual void HitWall (Collision other)
	{
		float numberOfParticlesFloat = (0.2f * rigibodyMovable.velocity.magnitude);
		int numberOfParticles = (int) numberOfParticlesFloat;

		GameObject instantiatedParticles = InstantiateParticles (other.contacts [0], StaticVariables.Instance.WallHitParticles, gameObject.GetComponent<Renderer> ().material.color);

		instantiatedParticles.GetComponent<ParticleSystem>().startSize += (gameObject.transform.lossyScale.x * 0.1f);
		instantiatedParticles.GetComponent<ParticleSystem>().Emit(numberOfParticles);
	}

	public virtual GameObject InstantiateParticles (ContactPoint contact, GameObject prefab, Color color)
	{
		Vector3 pos = contact.point;
		Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
		GameObject instantiatedParticles = Instantiate(prefab, pos, rot) as GameObject;

		instantiatedParticles.transform.SetParent (StaticVariables.Instance.ParticulesClonesParent);
		instantiatedParticles.GetComponent<Renderer>().material.color = color;

		return instantiatedParticles;
	}

	public virtual void AddRigidbody ()
	{
		gameObject.AddComponent<Rigidbody>();
		rigibodyMovable = gameObject.GetComponent<Rigidbody>();
		rigibodyMovable.mass = massRb;
		rigibodyMovable.collisionDetectionMode = collisionDetectionModeRb;
		player.GetComponent<PlayersGameplay>().holdMovableRB = rigibodyMovable;

		currentVelocity = player.GetComponent<PlayersGameplay>().shootForce;
		gameObject.tag = "ThrownMovable";
	}
}
