﻿using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using DarkTonic.MasterAudio;

public class MovableScript : MonoBehaviour 
{
	[Header ("Informations")]
	public float higherVelocity;
	public float currentVelocity;
	public float limitVelocity = 80f;

	[Header ("Cube States")]
	public bool hold;
	public List<GameObject> attracedBy = new List<GameObject> ();
	public List<GameObject> repulsedBy = new List<GameObject> ();

	[Header ("Deceleration")]
	public bool decelerationShotOnly = false;
	[Range (0, 1)]
	public float decelerationAmount = 1;

	[Header ("Gravity")]
	public float gravity = 0;

	[SoundGroupAttribute]
	static string wallHitSound = "Impact";
	private bool canPlaySound = true;

	protected float timeTween = 0.5f;

	protected Rigidbody rigidbodyMovable;

	protected float massRb;
	protected float drag;
	protected CollisionDetectionMode collisionDetectionModeRb;

	protected Renderer movableRenderer;

	[HideInInspector]
	public Transform player;
	[HideInInspector]
	public GameObject playerThatThrew;
	[HideInInspector]
	public GameObject playerHit;
	[HideInInspector]
	public MeshFilter cubeMeshFilter;
	public Material cubeMaterial;

	// Use this for initialization
	protected virtual void Start () 
	{
		tag = "Untagged";

		rigidbodyMovable = GetComponent<Rigidbody>();
		movableRenderer = GetComponent<Renderer> ();
		cubeMeshFilter = transform.GetChild (2).GetComponent<MeshFilter> ();
		cubeMaterial = transform.GetChild (1).GetComponent<Renderer> ().material;

		GetRigidbodySettings ();

		cubeMaterial.SetFloat ("_Lerp", 0);
		cubeMaterial.SetColor ("_Color", GlobalVariables.Instance.cubeNeutralColor);

		cubeMeshFilter.mesh = GlobalVariables.Instance.cubesStripes [Random.Range (0, GlobalVariables.Instance.cubesStripes.Length)];

		tag = "Movable";
	}
	
	// Update is called once per frame
	protected virtual void Update () 
	{
		if(hold == false && rigidbodyMovable != null)
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

		SetCubeColor ();
	}

	protected virtual void SetCubeColor ()
	{
		/*if(hold == true && movableRenderer.material.color == Color.white)
		{
			DOTween.To(()=> movableRenderer.material.color, x=> movableRenderer.material.color =x, transform.parent.GetComponent<Renderer>().material.color, timeTween);
		}

		if(hold == false && movableRenderer.material.color != Color.white && gameObject.tag == "Movable")
		{
			DOTween.To(()=> movableRenderer.material.color, x=> movableRenderer.material.color =x, Color.white, timeTween);
		}*/

		if(hold)
		{
			Color cubeCorrectColor = new Color ();

			switch(player.name)
			{
			case "Player 1":
				cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer1;
				break;
			case "Player 2":
				cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer2;
				break;
			case "Player 3":
				cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer3;
				break;
			case "Player 4":
				cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer4;
				break;
			}

			if (cubeMaterial.GetColor("_Color") != cubeCorrectColor)
			{
				Color cubeColorTemp = cubeMaterial.GetColor("_Color");
				float cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");

				DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, cubeCorrectColor, timeTween).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp));
				DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 1, timeTween).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp));
			}
				
		}

		if(!hold && tag != "ThrownMovable" && cubeMaterial.GetColor("_Color") != GlobalVariables.Instance.cubeNeutralColor)
		{
			Color cubeColorTemp = cubeMaterial.GetColor("_Color");
			float cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");

			DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, GlobalVariables.Instance.cubeNeutralColor, timeTween).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp));
			DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 0, timeTween).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp));
		}

		GetComponent<Renderer> ().material.color = cubeMaterial.GetColor ("_Color");
	}

	protected virtual void FixedUpdate () 
	{
		if(rigidbodyMovable != null)
		{
			rigidbodyMovable.AddForce (Vector3.down * gravity, ForceMode.Acceleration);
		}

		if(rigidbodyMovable != null && currentVelocity > 5)
		{
			if(!decelerationShotOnly)
				rigidbodyMovable.velocity = new Vector3(rigidbodyMovable.velocity.x * decelerationAmount, rigidbodyMovable.velocity.y, rigidbodyMovable.velocity.z * decelerationAmount);

			else if(decelerationShotOnly && attracedBy.Count == 0 && repulsedBy.Count == 0)
				rigidbodyMovable.velocity = new Vector3(rigidbodyMovable.velocity.x * decelerationAmount, rigidbodyMovable.velocity.y, rigidbodyMovable.velocity.z * decelerationAmount);
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
			&& playerThatThrew == null)
		{
			other.gameObject.GetComponent<PlayersGameplay>().StunVoid();

			playerHit = other.gameObject;
			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking();

			InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);
		}

		else if(other.collider.tag == "Player" 
			&& other.collider.GetComponent<PlayersGameplay>().playerState != PlayerState.Stunned 
			&& gameObject.tag == "ThrownMovable" 
			&& other.gameObject.name != playerThatThrew.name)
		{
			other.gameObject.GetComponent<PlayersGameplay>().StunVoid();

			playerHit = other.gameObject;
			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking();

			InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);

			StatsManager.Instance.PlayersFragsAndHits (playerThatThrew, playerHit);

			playerThatThrew = null;
		}
	}

	protected virtual void HitOtherMovable (Collision other)
	{
		float numberOfParticlesFloat = (0.2f * rigidbodyMovable.velocity.magnitude);
		int numberOfParticles = (int) numberOfParticlesFloat;

		GameObject instantiatedParticles = InstantiateParticles (other.contacts [0], GlobalVariables.Instance.WallHitParticles, gameObject.GetComponent<Renderer> ().material.color);

		instantiatedParticles.GetComponent<ParticleSystem>().startSize += (gameObject.transform.lossyScale.x * 0.1f);
		instantiatedParticles.GetComponent<ParticleSystem>().Emit(numberOfParticles);

		StartCoroutine(HitSound ());
	}

	protected virtual void HitWall (Collision other)
	{
		float numberOfParticlesFloat = (0.2f * rigidbodyMovable.velocity.magnitude);
		int numberOfParticles = (int) numberOfParticlesFloat;

		GameObject instantiatedParticles = InstantiateParticles (other.contacts [0], GlobalVariables.Instance.WallHitParticles, gameObject.GetComponent<Renderer> ().material.color);

		instantiatedParticles.GetComponent<ParticleSystem>().startSize += (gameObject.transform.lossyScale.x * 0.1f);
		instantiatedParticles.GetComponent<ParticleSystem>().Emit(numberOfParticles);

		if(other.gameObject.tag == "Wall" && canPlaySound)
		{
			StartCoroutine(HitSound ());
		}
	}

	IEnumerator HitSound ()
	{
		canPlaySound = false;

		if(currentVelocity >= limitVelocity)
			MasterAudio.PlaySound3DFollowTransformAndForget (wallHitSound, transform);

		else
		{
			float soundVolume = (currentVelocity * 100) / limitVelocity / 100;
			MasterAudio.PlaySound3DFollowTransformAndForget (wallHitSound, transform, soundVolume);	
		}

		yield return new WaitForSeconds (0.05f);

		canPlaySound = true;
	}

	public virtual GameObject InstantiateParticles (ContactPoint contact, GameObject prefab, Color color)
	{
		Vector3 pos = contact.point;
		Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
		GameObject instantiatedParticles = Instantiate(prefab, pos, rot) as GameObject;

		instantiatedParticles.transform.SetParent (GlobalVariables.Instance.ParticulesClonesParent);
		instantiatedParticles.GetComponent<Renderer>().material.color = color;

		return instantiatedParticles;
	}

	public virtual GameObject InstantiateExplosionParticles (GameObject prefab, Color color)
	{
		Vector3 pos = transform.position;
		Quaternion rot = Quaternion.FromToRotation(Vector3.forward, new Vector3(0, 0, 0));
		GameObject instantiatedParticles = Instantiate(prefab, pos, rot) as GameObject;

		instantiatedParticles.transform.SetParent (GlobalVariables.Instance.ParticulesClonesParent);
		instantiatedParticles.GetComponent<Renderer>().material.color = color;

		return instantiatedParticles;
	}

	public virtual void GetRigidbodySettings ()
	{
		massRb = rigidbodyMovable.mass;
		collisionDetectionModeRb = rigidbodyMovable.collisionDetectionMode;
		drag = rigidbodyMovable.drag;
	}

	public virtual void AddRigidbody ()
	{
		gameObject.AddComponent<Rigidbody>();
		rigidbodyMovable = gameObject.GetComponent<Rigidbody>();
		rigidbodyMovable.mass = massRb;
		rigidbodyMovable.collisionDetectionMode = collisionDetectionModeRb;
		rigidbodyMovable.drag = drag;
		player.GetComponent<PlayersGameplay>().holdMovableRB = rigidbodyMovable;

		currentVelocity = player.GetComponent<PlayersGameplay>().shootForce;
		gameObject.tag = "ThrownMovable";
	}

	public virtual void OnHold ()
	{
		attracedBy.Clear ();
		repulsedBy.Clear ();
		playerThatThrew = null;
	}
}
