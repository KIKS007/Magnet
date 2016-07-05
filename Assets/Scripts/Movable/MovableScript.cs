using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MovableScript : MonoBehaviour 
{
	public float higherVelocity;
	public float currentVelocity;
	public float limitVelocity;

	public bool hold;
	public bool fastMovable;

	public Ease easetype;
	public float timeTween;

	private Rigidbody rigibodyMovable;
	private GameObject mainCamera;

	private float massRb;
	private CollisionDetectionMode collisionDetectionModeRb;

	private GameObject wallHitParticlesPrefab;
	private GameObject hitParticlesPrefab;
	private GameObject movableExplosionPrefab;

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
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

		wallHitParticlesPrefab = GameObject.FindGameObjectWithTag("WallHitParticles") as GameObject;
		hitParticlesPrefab = GameObject.FindGameObjectWithTag("HitParticles") as GameObject;
		movableExplosionPrefab = GameObject.FindGameObjectWithTag("MovableExplosion") as GameObject;

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

				ContactPoint contact = other.contacts[0];
				Vector3 pos = contact.point;
				Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
				GameObject instantiatedParticles = Instantiate(hitParticlesPrefab, pos, rot) as GameObject;
				instantiatedParticles.transform.SetParent (StaticVariables.ParticulesClonesParent);
				instantiatedParticles.GetComponent<Renderer>().material.color = other.gameObject.GetComponent<Renderer>().material.color;
				instantiatedParticles.AddComponent<ParticlesAutoDestroy>();

				FragsAndHits ();
			}
		}

		if(other.gameObject.tag == "Movable")
		{
			//Debug.Log("Touched Movable!");

			ContactPoint contact = other.contacts[0];
			Vector3 pos = contact.point;
			Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);

			float numberOfParticlesFloat = (0.2f * rigibodyMovable.velocity.magnitude);
			int numberOfParticles = (int) numberOfParticlesFloat;

			GameObject instantiatedParticles = Instantiate(wallHitParticlesPrefab, pos, rot) as GameObject;
			instantiatedParticles.transform.SetParent (StaticVariables.ParticulesClonesParent);

			//instantiatedParticles.GetComponent<Renderer>().material.color = other.gameObject.GetComponent<Renderer>().material.color;
			instantiatedParticles.GetComponent<Renderer>().material.color = gameObject.GetComponent<Renderer>().material.color;

			instantiatedParticles.GetComponent<ParticleSystem>().startSize += (gameObject.transform.lossyScale.x * 0.1f);
			instantiatedParticles.GetComponent<ParticleSystem>().Emit(numberOfParticles);
			instantiatedParticles.AddComponent<ParticlesAutoDestroy>();
		}

		if(other.gameObject.layer == 16)
		{
			//Debug.Log("Touched Wall!");

			ContactPoint contact = other.contacts[0];
			Vector3 pos = contact.point;
			Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);

			float numberOfParticlesFloat = (0.2f * rigibodyMovable.velocity.magnitude);
			int numberOfParticles = (int) numberOfParticlesFloat;

			GameObject instantiatedParticles = Instantiate(wallHitParticlesPrefab, pos, rot) as GameObject;
			instantiatedParticles.transform.SetParent (StaticVariables.ParticulesClonesParent);

			//instantiatedParticles.GetComponent<Renderer>().material.color = other.gameObject.GetComponent<Renderer>().material.color;
			instantiatedParticles.GetComponent<Renderer>().material.color = gameObject.GetComponent<Renderer>().material.color;

			instantiatedParticles.GetComponent<ParticleSystem>().startSize += (gameObject.transform.lossyScale.x * 0.1f);
			instantiatedParticles.GetComponent<ParticleSystem>().Emit(numberOfParticles);
			instantiatedParticles.AddComponent<ParticlesAutoDestroy>();
		}

		if(other.gameObject.tag == "GoalTrigger")
		{
			other.gameObject.GetComponent<GoalTriggerScore>().ScoreUpdate();
			gameObject.SetActive(false);

			mainCamera.GetComponent<SlowMotionCamera>().slowMotion = true;

			//Debug.Log("Touched Trigger!");
			
			ContactPoint contact = other.contacts[0];
			Vector3 pos = contact.point;
			Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
			
			GameObject instantiatedParticles = Instantiate(movableExplosionPrefab, pos, rot) as GameObject;
			instantiatedParticles.transform.SetParent (StaticVariables.ParticulesClonesParent);
			instantiatedParticles.GetComponent<Renderer>().material.color = gameObject.GetComponent<Renderer>().material.color;
			instantiatedParticles.AddComponent<ParticlesAutoDestroy>();
		}
	}

	void FragsAndHits ()
	{
		if(playerThatThrew.name == "Player 1" && playerTouched.name == "Player 2")
		{
			mainCamera.GetComponent<StatsScript>().Player_2HitByPlayer_1 += 1;
		}

		if(playerThatThrew.name == "Player 1" && playerTouched.name == "Player 3")
		{
			mainCamera.GetComponent<StatsScript>().Player_3HitByPlayer_1 += 1;
		}

		if(playerThatThrew.name == "Player 1" && playerTouched.name == "Player 4")
		{
			mainCamera.GetComponent<StatsScript>().Player_4HitByPlayer_1 += 1;
		}


		if(playerThatThrew.name == "Player 2" && playerTouched.name == "Player 1")
		{
			mainCamera.GetComponent<StatsScript>().Player_1HitByPlayer_2 += 1;
		}

		if(playerThatThrew.name == "Player 2" && playerTouched.name == "Player 3")
		{
			mainCamera.GetComponent<StatsScript>().Player_3HitByPlayer_2 += 1;
		}

		if(playerThatThrew.name == "Player 2" && playerTouched.name == "Player 4")
		{
			mainCamera.GetComponent<StatsScript>().Player_4HitByPlayer_2 += 1;
		}


		if(playerThatThrew.name == "Player 3" && playerTouched.name == "Player 1")
		{
			mainCamera.GetComponent<StatsScript>().Player_1HitByPlayer_3 += 1;
		}
		
		if(playerThatThrew.name == "Player 3" && playerTouched.name == "Player 2")
		{
			mainCamera.GetComponent<StatsScript>().Player_2HitByPlayer_3 += 1;
		}
		
		if(playerThatThrew.name == "Player 3" && playerTouched.name == "Player 4")
		{
			mainCamera.GetComponent<StatsScript>().Player_2HitByPlayer_3 += 1;
		}


		if(playerThatThrew.name == "Player 4" && playerTouched.name == "Player 1")
		{
			mainCamera.GetComponent<StatsScript>().Player_4HitByPlayer_1 += 1;
		}
		
		if(playerThatThrew.name == "Player 4" && playerTouched.name == "Player 2")
		{
			mainCamera.GetComponent<StatsScript>().Player_4HitByPlayer_2 += 1;
		}
		
		if(playerThatThrew.name == "Player 4" && playerTouched.name == "Player 3")
		{
			mainCamera.GetComponent<StatsScript>().Player_4HitByPlayer_3 += 1;
		}

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
