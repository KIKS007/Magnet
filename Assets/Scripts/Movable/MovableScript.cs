using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MovableScript : MonoBehaviour 
{
	public float higherVelocity;
	public float currentVelocity;
	public float limitVelocity;

	public bool hold;

	public Ease easetype;
	public float timeTween;

	private Rigidbody rigibodyMovable;

	private float massRb;
	private CollisionDetectionMode collisionDetectionModeRb;

	private GameObject wallHitParticlesPrefab;
	private GameObject hitParticlesPrefab;

	[HideInInspector]
	public Transform player;
	[HideInInspector]
	public GameObject playerThatThrew;
	[HideInInspector]
	public GameObject playerTouched;
	[HideInInspector]

	// Use this for initialization
	void Start () 
	{
		rigibodyMovable = GetComponent<Rigidbody>();

		wallHitParticlesPrefab = GameObject.FindGameObjectWithTag("WallHitParticles") as GameObject;
		hitParticlesPrefab = GameObject.FindGameObjectWithTag("HitParticles") as GameObject;

		massRb = rigibodyMovable.mass;
		collisionDetectionModeRb = rigibodyMovable.collisionDetectionMode;
	}
	
	// Update is called once per frame
	void Update () 
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

	void OnCollisionEnter (Collision other)
	{
		if(other.collider.tag != "HoldMovable")
		{
			if(other.collider.tag == "Player" 
				&& other.collider.GetComponent<PlayersGameplay>().playerState != PlayerState.Stunned 
				&& gameObject.tag == "ThrownMovable" 
				&& other.gameObject.name != playerThatThrew.name)
			{
				other.gameObject.GetComponent<PlayersGameplay>().StunVoid();

				playerTouched = other.gameObject;
				GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking();

				InstantiateParticles (other.contacts [0], hitParticlesPrefab, other.gameObject.GetComponent<Renderer>().material.color);
			}
		}

		if(other.gameObject.tag == "Movable")
		{
			float numberOfParticlesFloat = (0.2f * rigibodyMovable.velocity.magnitude);
			int numberOfParticles = (int) numberOfParticlesFloat;

			GameObject instantiatedParticles = InstantiateParticles (other.contacts [0], wallHitParticlesPrefab, gameObject.GetComponent<Renderer> ().material.color);

			instantiatedParticles.GetComponent<ParticleSystem>().startSize += (gameObject.transform.lossyScale.x * 0.1f);
			instantiatedParticles.GetComponent<ParticleSystem>().Emit(numberOfParticles);
		}

		//Touched Wall
		if(other.gameObject.layer == 16)
		{
			float numberOfParticlesFloat = (0.2f * rigibodyMovable.velocity.magnitude);
			int numberOfParticles = (int) numberOfParticlesFloat;

			GameObject instantiatedParticles = InstantiateParticles (other.contacts [0], wallHitParticlesPrefab, gameObject.GetComponent<Renderer> ().material.color);

			instantiatedParticles.GetComponent<ParticleSystem>().startSize += (gameObject.transform.lossyScale.x * 0.1f);
			instantiatedParticles.GetComponent<ParticleSystem>().Emit(numberOfParticles);
		}
	}

	GameObject InstantiateParticles (ContactPoint contact, GameObject prefab, Color color)
	{
		Vector3 pos = contact.point;
		Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
		GameObject instantiatedParticles = Instantiate(prefab, pos, rot) as GameObject;

		instantiatedParticles.transform.SetParent (StaticVariables.Instance.ParticulesClonesParent);
		instantiatedParticles.GetComponent<Renderer>().material.color = color;

		return instantiatedParticles;
	}

	public void AddRigidbody ()
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
