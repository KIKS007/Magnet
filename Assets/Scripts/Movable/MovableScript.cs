﻿using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using DarkTonic.MasterAudio;

public class MovableScript : MonoBehaviour 
{
	public enum CubeColor {Neutral, Blue, Pink, Green, Yellow} ;

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

	[Header ("Sounds")]
	[SoundGroupAttribute]
	public string wallHitSound;

	protected bool canPlaySound = true;

	protected float toColorDuration = 0.5f;
	protected float toNeutralDuration = 1.5f;

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
	[HideInInspector]
	public Material cubeMaterial;
	[HideInInspector]
	public Vector3 initialScale;

	protected virtual void Awake () 
	{
		initialScale = transform.localScale;
	}

	// Use this for initialization
	protected virtual void Start () 
	{
		hold = false;

		rigidbodyMovable = GetComponent<Rigidbody>();
		movableRenderer = GetComponent<Renderer> ();
		cubeMeshFilter = transform.GetChild (2).GetComponent<MeshFilter> ();
		cubeMaterial = transform.GetChild (1).GetComponent<Renderer> ().material;

		GetRigidbodySettings ();

		cubeMaterial.SetFloat ("_Lerp", 0);
		cubeMaterial.SetColor ("_Color", GlobalVariables.Instance.cubeNeutralColor);

		cubeMeshFilter.mesh = GlobalVariables.Instance.cubesStripes [Random.Range (0, GlobalVariables.Instance.cubesStripes.Length)];
		attracedBy.Clear ();
		repulsedBy.Clear ();
	}

	protected virtual void OnEnable ()
	{
		hold = false;

		rigidbodyMovable = GetComponent<Rigidbody>();
		movableRenderer = GetComponent<Renderer> ();
		cubeMeshFilter = transform.GetChild (2).GetComponent<MeshFilter> ();
		cubeMaterial = transform.GetChild (1).GetComponent<Renderer> ().material;

		GetRigidbodySettings ();

		cubeMaterial.SetFloat ("_Lerp", 0);
		cubeMaterial.SetColor ("_Color", GlobalVariables.Instance.cubeNeutralColor);

		cubeMeshFilter.mesh = GlobalVariables.Instance.cubesStripes [Random.Range (0, GlobalVariables.Instance.cubesStripes.Length)];
		attracedBy.Clear ();
		repulsedBy.Clear ();
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
			else if(currentVelocity < limitVelocity && gameObject.tag == "ThrownMovable")
			{
				gameObject.tag = "Movable";
				playerThatThrew = null;
			}
		}

	}

	protected virtual void SetCubeColor ()
	{
		if(hold && !DOTween.IsTweening ("CubeColorTween"))
		{
			Color cubeCorrectColor = new Color ();
			CubeColor cubeColorTest = CubeColor.Neutral;

			switch(player.GetComponent <PlayersGameplay>().playerName)
			{
			case PlayerName.Player1:
				cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer1;
				cubeColorTest = CubeColor.Blue;
				break;
			case PlayerName.Player2:
				cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer2;
				cubeColorTest = CubeColor.Pink;
				break;
			case PlayerName.Player3:
				cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer3;
				cubeColorTest = CubeColor.Green;
				break;
			case PlayerName.Player4:
				cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer4;
				cubeColorTest = CubeColor.Yellow;
				break;
			}

			if(cubeMaterial.GetColor("_Color") != cubeCorrectColor && !DOTween.IsTweening ("CubeColorTween"))
			{
				if (DOTween.IsTweening ("CubeNeutralTween"))
				{
					Debug.Log ("CubeNeutralTween is Playing");
					DOTween.Kill ("CubeNeutralTween");
				}

				Debug.Log ("New Color : " + cubeCorrectColor);

				Color cubeColorTemp = cubeMaterial.GetColor("_Color");
				float cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");
				
				DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, cubeCorrectColor, toColorDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp)).SetId("CubeColorTween");
				DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 1, toColorDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp)).SetId("CubeColorTween");
				
				StartCoroutine (WaitToChangeColorEnum (cubeColorTest, toColorDuration));
			}
		}

		if(!hold && tag != "ThrownMovable" && !DOTween.IsTweening ("CubeNeutralTween") && cubeMaterial.GetColor("_Color") != GlobalVariables.Instance.cubeNeutralColor)
		{
			if (DOTween.IsTweening ("CubeColorTween"))
			{
				Debug.Log ("CubeColorTween is Playing");
				DOTween.Kill ("CubeColorTween");
			}

			Debug.Log ("Neutral Color");

			Color cubeColorTemp = cubeMaterial.GetColor("_Color");
			float cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");

			DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, GlobalVariables.Instance.cubeNeutralColor, toNeutralDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp)).SetId("CubeNeutralTween");
			DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 0, toNeutralDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp)).SetId("CubeNeutralTween");

			StartCoroutine (WaitToChangeColorEnum (CubeColor.Neutral, toNeutralDuration));
		}
	}

	void ToColor ()
	{
		Color cubeCorrectColor = new Color ();
		CubeColor cubeColorTest = CubeColor.Neutral;

		switch(player.GetComponent<PlayersGameplay> ().playerName)
		{
		case PlayerName.Player1:
			cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer1;
			cubeColorTest = CubeColor.Blue;
			break;
		case PlayerName.Player2:
			cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer2;
			cubeColorTest = CubeColor.Pink;
			break;
		case PlayerName.Player3:
			cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer3;
			cubeColorTest = CubeColor.Green;
			break;
		case PlayerName.Player4:
			cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer4;
			cubeColorTest = CubeColor.Yellow;
			break;
		}

		if(cubeMaterial.GetColor("_Color") != cubeCorrectColor)
		{
			if (DOTween.IsTweening ("CubeNeutralTween" + gameObject.GetInstanceID ()))
			{
				DOTween.Kill ("CubeNeutralTween" + gameObject.GetInstanceID ());
			}

			//Debug.Log ("New Color : " + cubeCorrectColor);

			Color cubeColorTemp = cubeMaterial.GetColor("_Color");
			float cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");

			DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, cubeCorrectColor, toColorDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp)).SetId("CubeColorTween" + gameObject.GetInstanceID ());
			DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 1, toColorDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp)).SetId("CubeColorTween" + gameObject.GetInstanceID ());

			StartCoroutine (WaitToChangeColorEnum (cubeColorTest, toColorDuration));
		}
	}

	IEnumerator ToNeutralColor ()
	{
		yield return new WaitWhile (() => tag != "ThrownMovable");

		if(!hold)
		{
			Color cubeColorTemp = cubeMaterial.GetColor("_Color");
			float cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");

			//Debug.Log ("Neutral Color");

			DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, GlobalVariables.Instance.cubeNeutralColor, toNeutralDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp)).SetId("CubeNeutralTween" + gameObject.GetInstanceID ());
			DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 0, toNeutralDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp)).SetId("CubeNeutralTween" + gameObject.GetInstanceID ());

			StartCoroutine (WaitToChangeColorEnum (CubeColor.Neutral, toNeutralDuration));
		}
	}

	IEnumerator WaitToChangeColorEnum (CubeColor whichColor, float waitTime)
	{
		yield return new WaitForSeconds (waitTime * 0.5f);

		if(!hold && whichColor == CubeColor.Neutral)
		{
			cubeColor = whichColor;
		}

		if(hold && whichColor != CubeColor.Neutral)
		{
			cubeColor = whichColor;
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
			other.gameObject.GetComponent<PlayersGameplay>().StunVoid(true);

			playerHit = other.gameObject;
			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking(SlowMotionType.Stun);

			InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);
		}

		else if(other.collider.tag == "Player" 
			&& other.collider.GetComponent<PlayersGameplay>().playerState != PlayerState.Stunned 
			&& gameObject.tag == "ThrownMovable" 
			&& other.gameObject.name != playerThatThrew.name)
		{
			other.gameObject.GetComponent<PlayersGameplay>().StunVoid(true);

			playerHit = other.gameObject;
			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking(SlowMotionType.Stun);

			InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);

			StatsManager.Instance.PlayersFragsAndHits (playerThatThrew, playerHit);
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
		/*float numberOfParticlesFloat = (0.2f * rigidbodyMovable.velocity.magnitude);
		int numberOfParticles = (int) numberOfParticlesFloat;

		GameObject instantiatedParticles = InstantiateParticles (other.contacts [0], GlobalVariables.Instance.WallHitParticles, gameObject.GetComponent<Renderer> ().material.color);

		instantiatedParticles.GetComponent<ParticleSystem>().startSize += (gameObject.transform.lossyScale.x * 0.1f);
		instantiatedParticles.GetComponent<ParticleSystem>().Emit(numberOfParticles);*/

		if(other.gameObject.tag == "Wall" && canPlaySound && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			if(currentVelocity > (limitVelocity * 0.5f))
				InstantiateImpactFX (other.contacts [0]);

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

	public virtual GameObject InstantiateImpactFX (ContactPoint contact)
	{
		GameObject prefab = GlobalVariables.Instance.wallImpactFX [(int)cubeColor];

		Vector3 pos = contact.point;
		Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
		GameObject instantiatedParticles = Instantiate(prefab, pos, rot) as GameObject;

		instantiatedParticles.transform.SetParent (GlobalVariables.Instance.ParticulesClonesParent);

		return instantiatedParticles;
	}

	public virtual GameObject InstantiateParticles (ContactPoint contact, GameObject prefab, Color color)
	{
		Vector3 pos = contact.point;
		Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
		GameObject instantiatedParticles = Instantiate(prefab, pos, rot) as GameObject;

		instantiatedParticles.transform.SetParent (GlobalVariables.Instance.ParticulesClonesParent);
		instantiatedParticles.GetComponent<ParticleSystemRenderer>().material.color = color;

		return instantiatedParticles;
	}

	public virtual GameObject InstantiateExplosionParticles (GameObject prefab, Color color)
	{
		Vector3 pos = transform.position;
		Quaternion rot = Quaternion.FromToRotation(Vector3.forward, new Vector3(0, 0, 0));
		GameObject instantiatedParticles = Instantiate(prefab, pos, rot) as GameObject;

		instantiatedParticles.transform.SetParent (GlobalVariables.Instance.ParticulesClonesParent);
		instantiatedParticles.GetComponent<ParticleSystemRenderer>().material.color = color;

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

		//SetCubeColor ();
		ToColor();
	}

	public virtual void OnRelease ()
	{
		//SetCubeColor ();
		StartCoroutine (ToNeutralColor());
	}

}
