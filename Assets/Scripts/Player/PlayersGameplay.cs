using UnityEngine;
using System.Collections;
using XInputDotNetPure;
using DG.Tweening;
using DarkTonic.MasterAudio;
using Rewired;

public enum Team
{
	Team1,
	Team2
}

public enum PlayerState
{
	None,
	Attracting,
	Repulsing,
	Holding,
	Stunned,
	Dead
}

public enum DashState
{
	CanDash,
	Dashing,
	Cooldown
}

public class PlayersGameplay : MonoBehaviour 
{
	public delegate void EventHandler();

	public event EventHandler OnAttracting;
	public event EventHandler OnRepulsing;
	public event EventHandler OnHolding;
	public event EventHandler OnHold;
	public event EventHandler OnShoot;
	public event EventHandler OnStun;
	public event EventHandler OnDash;
	public event EventHandler OnDeath;

	[Header ("States")]
	public Team team;
	public PlayerState playerState = PlayerState.None;
	public DashState dashState = DashState.CanDash;

	[Header ("Controller Number")]
	public int controllerNumber;
	[HideInInspector]
	public Player player; // The Rewired Player

	[Header ("Movement")]
	public float speed = 15;
	public float stunnedSpeed = 8;
	public float velocity;

	[Header ("Forces")]
	public float attractionForce = 10;
	public float shootForce = 200;
	public float repulsionForce = 10;

	[Header ("Bump")]
	public float stunnedRotation = 400;
	public float stunnedDuration = 1;

	[Header ("Dash")]
	public float dashSpeed = 30;
	public float dashDuration = 0.2f;
	public float dashCooldown = 2;
	public Ease dashEase;

	[Header ("Vibration")]
	[RangeAttribute(0, 1f)]
	public float motorVibration = 0.5f;
	public float durationVibration = 0.3f;

	[Header ("Player Sounds")]
	[SoundGroupAttribute]
	public string hitSound;
	[SoundGroupAttribute]
	public string shootSound;
	[SoundGroupAttribute]
	public string repulsionSound;
	[SoundGroupAttribute]
	public string attractionSound;
	public float fadeDuration;


	private float attractionSoundVolume;
	private float repulsionSoundVolume;

	private Transform movableParent;
	private Transform magnetPoint;

	private TrailRenderer trail;

	private float lerpHold = 0.2f;

	private Rigidbody playerRigidbody;
	private Vector3 movement;

	private int triggerMask;
	private float camRayLength = 200f;

	private float originalSpeed;

	private GameObject deadParticlesPrefab;
	//private GameObject playerDestroyParticles;

	[HideInInspector]
	public Rigidbody holdMovableRB;
	[HideInInspector]
	public Transform holdMovableTransform;

	private MainMenuManagerScript mainMenuScript;


	// Use this for initialization
	void Start () 
	{
		DOTween.Init ();

		GetControllerNumber ();

		Controller ();

		if (GameObject.FindGameObjectWithTag ("MainMenuManager") != null)
			mainMenuScript = GameObject.FindGameObjectWithTag ("MainMenuManager").GetComponent<MainMenuManagerScript> ();
		else
			Debug.Log ("No MainMenuManager Found");

		deadParticlesPrefab = GameObject.FindGameObjectWithTag("DeadParticles") as GameObject;

		//playerDestroyParticles = GameObject.FindGameObjectWithTag("PlayerDestroy") as GameObject;

		triggerMask = LayerMask.GetMask ("FloorMask");
		playerRigidbody = GetComponent<Rigidbody>();
		originalSpeed = speed;

		movableParent = GameObject.FindGameObjectWithTag ("MovableParent").transform;
		magnetPoint = transform.GetChild (1).transform;
		trail = transform.GetChild (4).GetComponent<TrailRenderer>();

		StartSounds ();
	}

	void OnEnable ()
	{
		playerState = PlayerState.None;
		dashState = DashState.CanDash;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(playerRigidbody.velocity.magnitude > velocity)
			velocity = playerRigidbody.velocity.magnitude;


		if (StaticVariables.Instance.GamePaused == false)
			ActivateFunctions ();

		if(playerState == PlayerState.Stunned)
		{
			transform.Rotate(0, stunnedRotation * Time.deltaTime, 0, Space.World);
		}

		if (playerState == PlayerState.Attracting && !player.GetButton ("Attract"))
			playerState = PlayerState.None;
			
		if (playerState == PlayerState.Repulsing && !player.GetButton ("Repulse"))
			playerState = PlayerState.None;
		
		TrailLength ();

		StatsButton ();

		Sounds ();
	}

	void ActivateFunctions ()
	{
		movement = new Vector3(player.GetAxisRaw("Move Horizontal"), 0f, player.GetAxisRaw("Move Vertical"));
		movement = movement.normalized * speed * Time.deltaTime;


		if(controllerNumber == 0 && playerState != PlayerState.Stunned)
			TurningMouse ();

		else if(controllerNumber > 0 && playerState != PlayerState.Stunned)
			TurningGamepad ();


		if(playerState == PlayerState.Holding && player.GetButtonUp("Attract"))
		{
			Shoot ();
		}
			

		if(playerState == PlayerState.None)
		{
			if (player.GetButton ("Attract"))
				playerState = PlayerState.Attracting;

			if (player.GetButton ("Repulse"))
				playerState = PlayerState.Repulsing;
		}

		if(player.GetButtonDown("Dash") && dashState == DashState.CanDash)
		{
			StartCoroutine(Dash ());
		}
			
		Pause ();
	}
	
	void FixedUpdate ()
	{
		if(dashState != DashState.Dashing)
			playerRigidbody.MovePosition(transform.position + movement);

		if(playerState == PlayerState.Holding)
		{
			holdMovableTransform.position = Vector3.Lerp(holdMovableTransform.position, magnetPoint.transform.position, lerpHold);
			holdMovableTransform.transform.rotation = Quaternion.Lerp(holdMovableTransform.rotation, transform.rotation, lerpHold);

			if (OnHolding != null)
				OnHolding ();
		}
	}

	public void GetControllerNumber ()
	{
		switch (gameObject.name)
		{
		case "Player 1":
			controllerNumber = StaticVariables.Instance.ControllerNumberPlayer1;
			break;
		case "Player 2":
			controllerNumber = StaticVariables.Instance.ControllerNumberPlayer2;
			break;
		case "Player 3":
			controllerNumber = StaticVariables.Instance.ControllerNumberPlayer3;
			break;
		case "Player 4":
			controllerNumber = StaticVariables.Instance.ControllerNumberPlayer4;
			break;
		}
	}

	public void Controller ()
	{
		if(controllerNumber == -1)
		{
			gameObject.SetActive(false);
		}

		if(gameObject.transform.GetChild(1).gameObject.activeSelf == false)
		{
			gameObject.transform.GetChild(1).gameObject.SetActive(true);
			gameObject.transform.GetChild(2).gameObject.SetActive(true);
		}

		if(controllerNumber != -1)
		{
			switch (controllerNumber)
			{
			case 0:
				player = ReInput.players.GetPlayer(4);
				break;
			case 1:
				player = ReInput.players.GetPlayer(0);
				break;
			case 2:
				player = ReInput.players.GetPlayer(1);
				break;
			case 3:
				player = ReInput.players.GetPlayer(2);
				break;
			case 4:
				player = ReInput.players.GetPlayer(3);
				break;
			}

		}
	}

	public void Attraction (GameObject movable)
	{
		playerState = PlayerState.Attracting;

		Vector3 movableAttraction = transform.position - movable.transform.position;
		movable.GetComponent<Rigidbody>().AddForce(movableAttraction * attractionForce, ForceMode.Force);

		if (OnAttracting != null)
			OnAttracting ();
	}
		
	public void Shoot ()
	{
		playerState = PlayerState.None;

		holdMovableTransform.GetChild(0).GetComponent<SlowMotionTriggerScript>().triggerEnabled = true;

		holdMovableTransform.gameObject.GetComponent<MovableScript>().hold = false;

		holdMovableTransform.transform.parent = null;
		holdMovableTransform.transform.parent = movableParent;
		holdMovableTransform.GetComponent<MovableScript>().AddRigidbody();
		holdMovableRB = holdMovableTransform.GetComponent<Rigidbody>();
		holdMovableTransform.gameObject.tag = "ThrownMovable";
		holdMovableTransform.GetComponent<MovableScript> ().currentVelocity = 200;
		holdMovableRB.AddForce(transform.forward * shootForce, ForceMode.VelocityChange);

		playerRigidbody.AddForce(transform.forward * -holdMovableRB.mass * 5, ForceMode.VelocityChange);

		MasterAudio.PlaySound3DAtTransformAndForget (shootSound, transform);

		if (OnShoot != null)
			OnShoot ();
	}

	public void Repulsion (GameObject movable)
	{
		playerState = PlayerState.Repulsing;

		Vector3 movableRepulsion = movable.transform.position - transform.position;
		movable.GetComponent<Rigidbody>().AddForce(movableRepulsion * repulsionForce, ForceMode.Force);

		if (OnRepulsing != null)
			OnRepulsing ();
	}

	public void OnHoldMovable (GameObject movable)
	{
		playerState = PlayerState.Holding;
		holdMovableRB = movable.GetComponent<Rigidbody>();
		holdMovableTransform = movable.GetComponent<Transform>();

		if (OnHold != null)
			OnHold ();
	}

	void StartSounds ()
	{
		attractionSoundVolume = MasterAudio.GetGroupVolume (attractionSound);
		repulsionSoundVolume = MasterAudio.GetGroupVolume (repulsionSound);

		MasterAudio.PlaySound3DFollowTransformAndForget (attractionSound, transform);
		MasterAudio.FadeSoundGroupToVolume (attractionSound, 0, 0);

		MasterAudio.PlaySound3DFollowTransformAndForget (repulsionSound, transform);
		MasterAudio.FadeSoundGroupToVolume (repulsionSound, 0, 0);
	}

	void Sounds ()
	{
		if(!StaticVariables.Instance.GamePaused)
		{
			if(playerState == PlayerState.Stunned || playerState == PlayerState.Holding)
			{
				if(MasterAudio.GetGroupVolume(attractionSound) != 0)
					MasterAudio.FadeSoundGroupToVolume (attractionSound, 0, fadeDuration);

				if(MasterAudio.GetGroupVolume(repulsionSound) != 0)
					MasterAudio.FadeSoundGroupToVolume (repulsionSound, 0, fadeDuration);
			}


			if(playerState != PlayerState.Stunned && playerState != PlayerState.Holding)
			{
				
				if(MasterAudio.GetGroupVolume(attractionSound) == 0)
				{
					if(player.GetButton("Attract"))
						MasterAudio.FadeSoundGroupToVolume (attractionSound, attractionSoundVolume, fadeDuration);
				}

				else if(MasterAudio.GetGroupVolume(attractionSound) != 0)
				{
					if(!player.GetButton("Attract"))
						MasterAudio.FadeSoundGroupToVolume (attractionSound, 0, fadeDuration);
				}

				if(MasterAudio.GetGroupVolume(repulsionSound) == 0)
				{
					if(player.GetButton("Repulse"))
						MasterAudio.FadeSoundGroupToVolume (repulsionSound, repulsionSoundVolume, fadeDuration);
				}

				else if(MasterAudio.GetGroupVolume(repulsionSound) != 0)
				{
					if(!player.GetButton("Repulse"))
						MasterAudio.FadeSoundGroupToVolume (repulsionSound, 0, fadeDuration);
				}
			}
		}

	}

	void TrailLength ()
	{
		if(playerRigidbody.velocity.magnitude > 1)
		{
			trail.time = 0.1f;
			trail.startWidth = 1;
		}
		else if (playerRigidbody.velocity.magnitude < 1)
		{
			trail.time = 0.15f;
			trail.startWidth = 0.5f;
		}
	}

	void Pause ()
	{
		if(player.GetButtonDown("Start"))
		{
			mainMenuScript.GamePauseResumeVoid ();
		}
	}

	void StatsButton ()
	{
		
	}

	void TurningGamepad ()
	{
		if(player.GetAxis("Aim Horizontal") != 0 || player.GetAxis("Aim Vertical") != 0)
		{
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Atan2(player.GetAxisRaw("Aim Horizontal"), player.GetAxisRaw("Aim Vertical")) * Mathf.Rad2Deg, transform.eulerAngles.z);
		}
	}

	void OnCollisionEnter (Collision other)
	{
		if(other.gameObject.tag == "DeadZone" && playerState != PlayerState.Dead)
		{
			playerState = PlayerState.Dead;

			Vector3 pos = other.contacts[0].point;
			//Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
			Quaternion rot = Quaternion.FromToRotation(Vector3.forward, new Vector3(0, 0, 0));

			GameObject instantiatedParticles = Instantiate(deadParticlesPrefab, pos, rot) as GameObject;
			instantiatedParticles.transform.SetParent (StaticVariables.Instance.ParticulesClonesParent);
			instantiatedParticles.transform.position = new Vector3(instantiatedParticles.transform.position.x, 2f, instantiatedParticles.transform.position.z);
			instantiatedParticles.transform.LookAt(new Vector3(0, 0, 0));
			instantiatedParticles.GetComponent<Renderer>().material.color = gameObject.GetComponent<Renderer>().material.color;
			instantiatedParticles.AddComponent<ParticlesAutoDestroy>();

			if(playerState == PlayerState.Holding)
			{
				for(int i = 0; i < transform.childCount; i++)
				{
					if(transform.GetChild(i).tag == "Movable" || transform.GetChild(i).tag == "HoldMovable")
					{
						transform.GetChild(i).transform.SetParent(null);
					}
				}
			}

			gameObject.SetActive (false);
		}

	}

	void TurningMouse ()
	{
		// Create a ray from the mouse cursor on screen in the direction of the camera.
		Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		
		// Create a RaycastHit variable to store information about what was hit by the ray.
		RaycastHit floorHit;
		
		// Perform the raycast and if it hits something on the floor layer...
		if (Physics.Raycast (camRay, out floorHit, camRayLength, triggerMask)) 
		{
			// Create a vector from the player to the point on the floor the raycast from the mouse hit.
			Vector3 playerToMouse = floorHit.point - transform.position;
			
			// Ensure the vector is entirely along the floor plane.
			playerToMouse.y = 0f;
			
			// Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
			Quaternion newRotatation = Quaternion.LookRotation (playerToMouse);
			
			// Set the player's rotation to this new rotation.
			playerRigidbody.MoveRotation (newRotatation);
		}
	}
		
	public void StunVoid ()
	{
		StartCoroutine(Stun ());
	}

	IEnumerator Stun ()
	{
		MasterAudio.PlaySound3DAtTransformAndForget (hitSound, transform);

		switch (controllerNumber)
		{
		case 0:
			break;

		case 1:
			GamePad.SetVibration (PlayerIndex.One, motorVibration, motorVibration);
			StartCoroutine (StunnedVibration (PlayerIndex.One));
			break;

		case 2:
			GamePad.SetVibration (PlayerIndex.Two, motorVibration, motorVibration);
			StartCoroutine (StunnedVibration (PlayerIndex.Two));
			break;

		case 3:
			GamePad.SetVibration (PlayerIndex.Three, motorVibration, motorVibration);
			StartCoroutine (StunnedVibration (PlayerIndex.Three));
			break;

		case 4:
			GamePad.SetVibration (PlayerIndex.Four, motorVibration, motorVibration);
			StartCoroutine (StunnedVibration (PlayerIndex.Four));
			break;

		default :
			break;
		}

		if(playerState == PlayerState.Holding)
		{
			Shoot ();
		}

		playerState = PlayerState.Stunned;

		speed = stunnedSpeed;

		if (OnStun != null)
			OnStun ();

		yield return new WaitForSeconds(stunnedDuration);

		playerState = PlayerState.None;
		speed = originalSpeed;
	}

	IEnumerator StunnedVibration (PlayerIndex whichController)
	{
		yield return new WaitForSeconds (durationVibration);

		GamePad.SetVibration (whichController, 0f, 0f);
	}

	IEnumerator Dash ()
	{
		dashState = DashState.Dashing;

		if (OnDash != null)
			OnDash ();
		
		Vector3 movementTemp = new Vector3(player.GetAxisRaw("Move Horizontal"), 0f, player.GetAxisRaw("Move Vertical"));

		float dashSpeedTemp = dashSpeed;

		DOTween.To (() => dashSpeedTemp, x => dashSpeedTemp = x, 0, dashDuration).SetEase (dashEase).SetId("Dash").OnUpdate(
			()=> playerRigidbody.velocity = movementTemp * dashSpeedTemp);
		

		yield return new WaitForSeconds (dashDuration - 0.05f);

		dashState = DashState.Cooldown;

		yield return new WaitForSeconds (dashCooldown);

		dashState = DashState.CanDash;
	}

	void OnApplicationQuit ()
	{
		GamePad.SetVibration (PlayerIndex.One, 0, 0);
		GamePad.SetVibration (PlayerIndex.Two, 0, 0);
		GamePad.SetVibration (PlayerIndex.Three, 0, 0);
		GamePad.SetVibration (PlayerIndex.Four, 0, 0);
	}

	void OnDestroy ()
	{
		GamePad.SetVibration (PlayerIndex.One, 0, 0);
		GamePad.SetVibration (PlayerIndex.Two, 0, 0);
		GamePad.SetVibration (PlayerIndex.Three, 0, 0);
		GamePad.SetVibration (PlayerIndex.Four, 0, 0);
	}

	void OnDisable ()
	{
		GamePad.SetVibration (PlayerIndex.One, 0, 0);
		GamePad.SetVibration (PlayerIndex.Two, 0, 0);
		GamePad.SetVibration (PlayerIndex.Three, 0, 0);
		GamePad.SetVibration (PlayerIndex.Four, 0, 0);
	}
}
