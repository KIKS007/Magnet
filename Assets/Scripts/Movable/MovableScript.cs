﻿#pragma warning disable 0618

using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using DarkTonic.MasterAudio;

public enum CubeColor {Neutral, Blue, Pink, Green, Yellow, Deadly};

public class MovableScript : MonoBehaviour 
{
	public event EventHandler OnHoldEvent;
	public event EventHandler OnReleaseEvent;

	#region Variables
	[Header ("Informations")]
	public CubeColor cubeColor;
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

	protected bool canPlaySound = true;

	protected float toColorDuration = 0.5f;
	protected float toNeutralDuration = 1.5f;
	protected float toDeadlyDuration = 1f;

	protected Rigidbody rigidbodyMovable;

	protected float massRb;
	protected float drag;
	protected CollisionDetectionMode collisionDetectionModeRb;

	protected Renderer movableRenderer;

	protected SlowMotionTriggerScript slowMoTrigger;

	protected GameObject mainCamera;

	protected ParticleSystem deadlyParticle;

	[HideInInspector]
	public Transform player;
	[HideInInspector]
	public GameObject playerThatThrew;
	[HideInInspector]
	public GameObject playerHit;
	[HideInInspector]
	public MeshFilter cubeMeshFilter;
	[HideInInspector]
	public Material cubeMaterial;
	[HideInInspector]
	public Vector3 initialScale;
	#endregion

	#region Setup
	protected virtual void Awake () 
	{
		initialScale = transform.localScale;
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
	}

	protected virtual void Start () 
	{
		ToNeutralColor ();
	}

	protected virtual void OnEnable ()
	{
		hold = false;

		rigidbodyMovable = GetComponent<Rigidbody>();
		movableRenderer = GetComponent<Renderer> ();
		cubeMeshFilter = transform.GetChild (2).GetComponent<MeshFilter> ();
		cubeMaterial = transform.GetChild (1).GetComponent<Renderer> ().material;
		deadlyParticle = transform.GetChild (3).GetComponent<ParticleSystem> ();
		slowMoTrigger = transform.GetComponentInChildren<SlowMotionTriggerScript> ();

		deadlyParticle.Stop ();
		cubeMeshFilter.mesh = GlobalVariables.Instance.cubesStripes [Random.Range (0, GlobalVariables.Instance.cubesStripes.Length)];
		attracedBy.Clear ();
		repulsedBy.Clear ();
	}
	#endregion

	#region Update / FixedUpdate
	protected virtual void Update () 
	{
		if(hold == false && rigidbodyMovable != null)
			currentVelocity = rigidbodyMovable.velocity.magnitude;


		if(hold == false && currentVelocity > 0)
		{
			if(currentVelocity > higherVelocity)
				higherVelocity = currentVelocity;

			if(currentVelocity >= limitVelocity)
				gameObject.tag = "ThrownMovable";
			
			else if(currentVelocity < limitVelocity && gameObject.tag == "ThrownMovable")
			{
				if(slowMoTrigger == null)
					slowMoTrigger = transform.GetComponentInChildren<SlowMotionTriggerScript> ();

				slowMoTrigger.triggerEnabled = false;
				gameObject.tag = "Movable";
				playerThatThrew = null;
			}
		}
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
	#endregion

	#region Color
	public virtual void ToColor (GameObject otherPlayer = null, float overrideDuration = -1)
	{
		int whichPlayer = otherPlayer != null ? (int)otherPlayer.GetComponent<PlayersGameplay> ().playerName : (int)player.GetComponent<PlayersGameplay> ().playerName;

		CubeColor whichColor = (CubeColor)whichPlayer + 1;

		float duration = overrideDuration != -1 ? overrideDuration : toColorDuration;

		if (DOTween.IsTweening ("CubeColorTween" + gameObject.GetInstanceID ()))
			DOTween.Kill ("CubeColorTween" + gameObject.GetInstanceID ());
		
		DisableAllColor (duration);

		deadlyParticle.Stop ();

		switch(whichColor)
		{
		case CubeColor.Blue:
			cubeMaterial.DOFloat (1f, "_LerpBLUE", duration).SetId("CubeColorTween" + gameObject.GetInstanceID ());
			break;
		case CubeColor.Pink:
			cubeMaterial.DOFloat (1f, "_LerpPINK", duration).SetId("CubeColorTween" + gameObject.GetInstanceID ());
			break;
		case CubeColor.Green:
			cubeMaterial.DOFloat (1f, "_LerpGREEN", duration).SetId("CubeColorTween" + gameObject.GetInstanceID ());
			break;
		case CubeColor.Yellow:
			cubeMaterial.DOFloat (1f, "_LerpYELLOW", duration).SetId("CubeColorTween" + gameObject.GetInstanceID ());
			break;
		}

		StartCoroutine (WaitToChangeColorEnum (whichColor, duration));
	}

	public virtual void ToNeutralColor (float overrideDuration = -1)
	{
		if(!hold)
		{
			float duration = overrideDuration != -1 ? overrideDuration : toNeutralDuration;

			if(deadlyParticle == null)
				deadlyParticle = transform.GetChild (3).GetComponent<ParticleSystem> ();
			
			deadlyParticle.Stop ();

			DisableAllColor (duration);

			StartCoroutine (WaitToChangeColorEnum (CubeColor.Neutral, duration));
		}
	}

	public virtual void ToDeadlyColor (float overrideDuration = -1)
	{
		float duration = overrideDuration != -1 ? overrideDuration : toColorDuration;

		DisableAllColor (duration);

		if(deadlyParticle == null)
			deadlyParticle = transform.GetChild (3).GetComponent<ParticleSystem> ();

		deadlyParticle.Play ();

		cubeMaterial.DOFloat (1f, "_LerpRED", duration).SetId("CubeColorTween" + gameObject.GetInstanceID ());

		cubeColor = CubeColor.Deadly;
	}

	public virtual void DisableAllColor (float duration)
	{
		if (DOTween.IsTweening ("CubeColorTween" + gameObject.GetInstanceID ()))
			DOTween.Kill ("CubeColorTween" + gameObject.GetInstanceID ());
		
		cubeMaterial.DOFloat (0f, "_LerpBLUE", duration).SetId("CubeColorTween" + gameObject.GetInstanceID ());
		
		cubeMaterial.DOFloat (0f, "_LerpPINK", duration).SetId("CubeColorTween" + gameObject.GetInstanceID ());
		
		cubeMaterial.DOFloat (0f, "_LerpGREEN", duration).SetId("CubeColorTween" + gameObject.GetInstanceID ());
		
		cubeMaterial.DOFloat (0f, "_LerpYELLOW", duration).SetId("CubeColorTween" + gameObject.GetInstanceID ());
		
		cubeMaterial.DOFloat (0f, "_LerpRED", duration).SetId("CubeColorTween" + gameObject.GetInstanceID ());
	}

	IEnumerator WaitToChangeColorEnum (CubeColor whichColor, float waitTime)
	{
		yield return new WaitForSeconds (waitTime * 0.5f);		

		if(hold)
			cubeColor = whichColor;
		
	}
	#endregion

	#region Collisions
	protected virtual void OnCollisionEnter (Collision other)
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			if(other.collider.tag != "HoldMovable")
				HitPlayer (other);			
			
			if(other.gameObject.tag == "Movable")
				HitOtherMovable (other);	
			
			if(other.gameObject.layer == 16)
				HitWall (other);			
		}
	}

	protected virtual void HitPlayer (Collision other)
	{
		if(other.collider.tag == "Player" && other.collider.GetComponent<PlayersGameplay>().playerState != PlayerState.Stunned && gameObject.tag == "ThrownMovable")
		{
			if(playerThatThrew == null || other.gameObject.name != playerThatThrew.name)
			{
				other.gameObject.GetComponent<PlayersGameplay>().StunVoid(true);
				
				playerHit = other.gameObject;

				InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);	

				if(playerThatThrew != null && other.gameObject.name != playerThatThrew.name)
					StatsManager.Instance.PlayersFragsAndHits (playerThatThrew, playerHit);
			}
		}
	}

	protected virtual void HitOtherMovable (Collision other)
	{
		float numberOfParticlesFloat = (0.2f * rigidbodyMovable.velocity.magnitude);
		int numberOfParticles = (int) numberOfParticlesFloat;

		GameObject instantiatedParticles = InstantiateParticles (other.contacts [0], GlobalVariables.Instance.WallHitParticles, gameObject.GetComponent<Renderer> ().material.color);

		instantiatedParticles.GetComponent<ParticleSystem>().startSize += (gameObject.transform.lossyScale.x * 0.1f);
		instantiatedParticles.GetComponent<ParticleSystem>().Emit(numberOfParticles);

		if(canPlaySound && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
			StartCoroutine(HitSound ());
	}

	protected virtual void HitWall (Collision other)
	{
		if(other.gameObject.tag == "Wall" && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			if(currentVelocity > (limitVelocity * 0.5f))
				InstantiateImpactFX (other.contacts [0]);

			if(canPlaySound)
				StartCoroutine(HitSound ());
		}
		
	}

	protected IEnumerator HitSound ()
	{
		canPlaySound = false;

		if(currentVelocity >= limitVelocity)
			MasterAudio.PlaySound3DFollowTransformAndForget (SoundsManager.Instance.wallHitSound, transform);

		else
		{
			float soundVolume = (currentVelocity * 100) / limitVelocity / 100;
			MasterAudio.PlaySound3DFollowTransformAndForget (SoundsManager.Instance.wallHitSound, transform, soundVolume);	
		}

		yield return new WaitForSeconds (0.05f);

		canPlaySound = true;
	}
	#endregion

	#region Particles / FX
	public virtual GameObject InstantiateImpactFX (ContactPoint contact)
	{
		GameObject prefab = GlobalVariables.Instance.wallImpactFX [(int)cubeColor];

		Vector3 pos = contact.point;
		Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
		GameObject instance = Instantiate(prefab, pos, rot) as GameObject;

		instance.transform.SetParent (GlobalVariables.Instance.ParticulesClonesParent);

		return instance;
	}

	public virtual GameObject InstantiateParticles (ContactPoint contact, GameObject prefab, Color color)
	{
		Vector3 pos = contact.point;
		Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
		GameObject instance = Instantiate(prefab, pos, rot) as GameObject;

		instance.transform.SetParent (GlobalVariables.Instance.ParticulesClonesParent);
		instance.GetComponent<ParticleSystemRenderer>().material.color = color;

		return instance;
	}
	#endregion

	#region Hold / Release
	public virtual void DestroyRigibody ()
	{
		massRb = rigidbodyMovable.mass;
		collisionDetectionModeRb = rigidbodyMovable.collisionDetectionMode;
		drag = rigidbodyMovable.drag;

		Destroy (rigidbodyMovable);
	}

	public virtual void AddRigidbody ()
	{
		gameObject.AddComponent<Rigidbody>();
		rigidbodyMovable = gameObject.GetComponent<Rigidbody>();
		rigidbodyMovable.mass = massRb;
		rigidbodyMovable.collisionDetectionMode = collisionDetectionModeRb;
		rigidbodyMovable.drag = drag;
		player.GetComponent<PlayersGameplay>().holdMovableRB = rigidbodyMovable;
	}

	public virtual void OnHold ()
	{
		hold = true;

		attracedBy.Clear ();
		repulsedBy.Clear ();

		ToColor();

		OnHoldEventVoid ();
	}

	public virtual void OnRelease ()
	{
		ToNeutralColor();

		OnReleaseEventVoid ();
	}

	public virtual void OnHoldEventVoid ()
	{
		if (OnHoldEvent != null)
			OnHoldEvent ();
	}

	public virtual void OnReleaseEventVoid ()
	{
		if (OnReleaseEvent != null)
			OnReleaseEvent ();
	}
	#endregion
}
