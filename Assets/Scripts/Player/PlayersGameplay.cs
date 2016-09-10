using UnityEngine;
using System.Collections;
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

public enum RepulsionWaveState
{
	CanRepulse,
	Repulsing,
	Cooldown
}

public delegate void EventHandler();


public class PlayersGameplay : MonoBehaviour 
{
	public event EventHandler OnAttracting;
	public event EventHandler OnAttracted;
	public event EventHandler OnRepulsing;
	public event EventHandler OnRepulsed;
	public event EventHandler OnHolding;
	public event EventHandler OnHold;
	public event EventHandler OnShoot;
	public event EventHandler OnStun;
	public event EventHandler OnDash;
	public event EventHandler OnDeath;

	public event EventHandler OnPlayerstateChange;

	[Header ("States")]
	public Team team;
	public PlayerState playerState = PlayerState.None;
	public DashState dashState = DashState.CanDash;
	public RepulsionWaveState repulsionState = RepulsionWaveState.CanRepulse;

	[Header ("Controller Number")]
	public int controllerNumber = -1;
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

	[Header ("Deceleration")]
	[Range (0, 1)]
	public float decelerationAmount = 1;

	[Header ("Repulsion Wave")]
	public bool enableRepulsionWave = false;
	public float repulsionWaveForce = 10;
	public float repulsionWaveRadius = 3;
	public LayerMask repulsionWaveMask = (1 << 9) | (1 << 13);
	public float repulsionCoolDown = 2;

	[Header ("Stun")]
	public float stunnedRotation = 400;
	public float stunnedDuration = 1;

	[Header ("Dash")]
	public float dashSpeed = 70;
	public float dashDuration = 0.3f;
	public float dashCooldown = 1.2f;
	public Ease dashEase = Ease.OutQuad;


	protected Transform movableParent;
	protected Transform magnetPoint;

	protected TrailRenderer trail;

	protected float lerpHold = 0.2f;

	protected Rigidbody playerRigidbody;
	protected Vector3 movement;

	protected int triggerMask;
	protected float camRayLength = 200f;

	protected float originalSpeed;


	[HideInInspector]
	public Rigidbody holdMovableRB;
	[HideInInspector]
	public Transform holdMovableTransform;

	protected MainMenuManagerScript mainMenuScript;

	protected bool hasAttracted;
	protected bool hasRepulsed;

	// Use this for initialization
	protected virtual void Start () 
	{
		DOTween.Init ();

		GetControllerNumber ();

		Controller ();

		if (GameObject.FindGameObjectWithTag ("MainMenuManager") != null)
			mainMenuScript = GameObject.FindGameObjectWithTag ("MainMenuManager").GetComponent<MainMenuManagerScript> ();

		triggerMask = LayerMask.GetMask ("FloorMask");
		playerRigidbody = GetComponent<Rigidbody>();
		originalSpeed = speed;

		movableParent = GameObject.FindGameObjectWithTag ("MovableParent").transform;
		magnetPoint = transform.GetChild (1).transform;
		trail = transform.GetChild (4).GetComponent<TrailRenderer>();

		if(gameObject.activeSelf == true)
			StartCoroutine (OnPlayerStateChange ());
	}

	protected void OnEnable ()
	{
		playerState = PlayerState.None;
		dashState = DashState.CanDash;

		GlobalVariables.Instance.ListPlayers ();
	}
	
	// Update is called once per frame
	protected virtual void Update () 
	{
		if(playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			if(playerRigidbody.velocity.magnitude > velocity)
				velocity = playerRigidbody.velocity.magnitude;


			ActivateFunctions ();

			if(playerState == PlayerState.Stunned)
			{
				transform.Rotate(0, stunnedRotation * Time.deltaTime, 0, Space.World);
			}

			if (playerState == PlayerState.Attracting && !player.GetButton ("Attract"))
				playerState = PlayerState.None;

			if (playerState == PlayerState.Repulsing && !player.GetButton ("Repulse"))
				playerState = PlayerState.None;

			StatsButton ();

			OnAttractedOnRepusled ();
		}

		Pause ();

		TrailLength ();
	}

	protected virtual void ActivateFunctions ()
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

			playerState = PlayerState.None;
		}
			

		if(playerState == PlayerState.None)
		{
			if (player.GetButton ("Attract"))
				playerState = PlayerState.Attracting;

			if (player.GetButton ("Repulse"))
				playerState = PlayerState.Repulsing;
		}

		if(player.GetButtonDown("Dash") && dashState == DashState.CanDash && movement != Vector3.zero)
		{
			StartCoroutine(Dash ());
		}

		if(player.GetButtonDown("Repulse") && repulsionState == RepulsionWaveState.CanRepulse && enableRepulsionWave && playerState != PlayerState.Holding)
		{
			StartCoroutine(RepulsionWave ());
		}
	}
	
	protected virtual void FixedUpdate ()
	{
		if(playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
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

			playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x * decelerationAmount, playerRigidbody.velocity.y, playerRigidbody.velocity.z * decelerationAmount);
		}
	}

	public void GetControllerNumber ()
	{
		switch (gameObject.name)
		{
		case "Player 1":
			controllerNumber = GlobalVariables.Instance.ControllerNumberPlayer1;
			break;
		case "Player 2":
			controllerNumber = GlobalVariables.Instance.ControllerNumberPlayer2;
			break;
		case "Player 3":
			controllerNumber = GlobalVariables.Instance.ControllerNumberPlayer3;
			break;
		case "Player 4":
			controllerNumber = GlobalVariables.Instance.ControllerNumberPlayer4;
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
			player = ReInput.players.GetPlayer(controllerNumber);

			switch (controllerNumber)
			{
			case 1:
				foreach(Joystick j in ReInput.controllers.Joysticks)
				{
					if(j.name == "XInput Gamepad 1")
						player.controllers.AddController(ControllerType.Joystick, j.id, true);
				}
				break;
			case 2:
				foreach(Joystick j in ReInput.controllers.Joysticks)
				{
					if(j.name == "XInput Gamepad 2")
						player.controllers.AddController(ControllerType.Joystick, j.id, true);
				}
				break;
			case 3:
				foreach(Joystick j in ReInput.controllers.Joysticks)
				{
					if(j.name == "XInput Gamepad 3")
						player.controllers.AddController(ControllerType.Joystick, j.id, true);
				}
				break;
			case 4:
				foreach(Joystick j in ReInput.controllers.Joysticks)
				{
					if(j.name == "XInput Gamepad 4")
						player.controllers.AddController(ControllerType.Joystick, j.id, true);
				}
				break;
			}
		}
	}

	public virtual void Attraction (GameObject movable)
	{
		playerState = PlayerState.Attracting;

		Vector3 movableAttraction = transform.position - movable.transform.position;
		movable.GetComponent<Rigidbody>().AddForce(movableAttraction * attractionForce, ForceMode.Force);

		if (OnAttracting != null)
			OnAttracting ();
	}
		
	public virtual void Shoot ()
	{
		holdMovableTransform.GetChild(0).GetComponent<SlowMotionTriggerScript>().triggerEnabled = true;

		holdMovableTransform.gameObject.GetComponent<MovableScript>().hold = false;

		holdMovableTransform.transform.SetParent(null);
		holdMovableTransform.transform.SetParent(movableParent);
		holdMovableTransform.GetComponent<MovableScript>().playerThatThrew = gameObject;
		holdMovableTransform.GetComponent<MovableScript>().AddRigidbody();
		holdMovableRB = holdMovableTransform.GetComponent<Rigidbody>();
		holdMovableTransform.gameObject.tag = "ThrownMovable";
		holdMovableTransform.GetComponent<MovableScript> ().currentVelocity = 200;
		holdMovableRB.AddForce(transform.forward * shootForce, ForceMode.VelocityChange);

		playerRigidbody.AddForce(transform.forward * -holdMovableRB.mass * 5, ForceMode.VelocityChange);

		if (OnShoot != null)
			OnShoot ();
	}

	public virtual void Repulsion (GameObject movable)
	{
		if(!enableRepulsionWave)
		{
			playerState = PlayerState.Repulsing;

			Vector3 movableRepulsion = movable.transform.position - transform.position;
			movable.GetComponent<Rigidbody>().AddForce(movableRepulsion * repulsionForce, ForceMode.Force);

			if (OnRepulsing != null)
				OnRepulsing ();
		}
	}

	protected IEnumerator RepulsionWave ()
	{
		playerState = PlayerState.Repulsing;
		repulsionState = RepulsionWaveState.Repulsing;

		foreach(Collider other in Physics.OverlapSphere(transform.position, repulsionWaveRadius, repulsionWaveMask))
		{
			Vector3 repulseDirection = other.transform.position - transform.position;
			repulseDirection.Normalize ();

			float explosionImpactZone = 1 - (Vector3.Distance (transform.position, other.transform.position) / repulsionWaveRadius);

			if(explosionImpactZone > 0)
			{
				if(other.GetComponent<Rigidbody>() != null)
					other.GetComponent<Rigidbody> ().AddForce (repulseDirection * explosionImpactZone * repulsionWaveForce, ForceMode.Impulse);
			}
		}

		repulsionState = RepulsionWaveState.Cooldown;

		yield return new WaitForSeconds (repulsionCoolDown);

		repulsionState = RepulsionWaveState.CanRepulse;
	}

	public virtual void OnHoldMovable (GameObject movable)
	{
		playerState = PlayerState.Holding;
		holdMovableRB = movable.GetComponent<Rigidbody>();
		holdMovableTransform = movable.GetComponent<Transform>();
		movable.GetComponent<MovableScript> ().OnHold ();

		if (OnHold != null)
			OnHold ();
	}
		
	protected virtual void OnAttractedOnRepusled ()
	{
		if(playerState != PlayerState.Attracting)
			hasAttracted = false;
	
		if(playerState != PlayerState.Repulsing)
			hasRepulsed = false;
		

		if(playerState == PlayerState.Attracting && !hasAttracted)
		{
			hasAttracted = true;

			if (OnAttracted != null)
				OnAttracted ();
		}

		if(playerState == PlayerState.Repulsing && !hasRepulsed)
		{
			hasRepulsed = true;

			if (OnRepulsed != null)
				OnRepulsed ();
		}
	}

	protected void TrailLength ()
	{
		if(playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
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

		else
		{
			trail.time = 0f;
			trail.startWidth = 0f;
		}
	}

	protected void Pause ()
	{
		if(player.GetButtonDown("Start"))
		{
			mainMenuScript.GamePauseResumeVoid ();
		}
	}

	protected void StatsButton ()
	{
		
	}

	protected virtual void OnCollisionStay (Collision other)
	{
		if(other.gameObject.tag == "DeadZone" && playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			DeathParticles ();
		
			Death ();
		}

		if(other.gameObject.tag == "Player" && dashState == DashState.Dashing)
		{
			if (other.gameObject.GetComponent<PlayersGameplay> ().playerState != PlayerState.Stunned)
			{
				other.gameObject.GetComponent<PlayersGameplay> ().StunVoid ();

				GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking();
			}
		}
	}

	protected virtual void OnCollisionEnter (Collision other)
	{
		if(other.gameObject.tag == "DeadZone" && playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			DeathParticles ();

			Death ();
		}

		if(other.gameObject.tag == "Player" && dashState == DashState.Dashing)
		{
			if (other.gameObject.GetComponent<PlayersGameplay> ().playerState != PlayerState.Stunned)
			{
				other.gameObject.GetComponent<PlayersGameplay> ().StunVoid ();

				GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking();
			}
		}
	}

	protected virtual void TurningMouse ()
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
		
	protected virtual void TurningGamepad ()
	{
		if(player.GetAxis("Aim Horizontal") != 0 || player.GetAxis("Aim Vertical") != 0)
		{
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Atan2(player.GetAxisRaw("Aim Horizontal"), player.GetAxisRaw("Aim Vertical")) * Mathf.Rad2Deg, transform.eulerAngles.z);
		}
	}

	public virtual void StunVoid ()
	{
		StartCoroutine(Stun ());
	}

	protected IEnumerator Stun ()
	{
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

	protected IEnumerator Dash ()
	{
		dashState = DashState.Dashing;

		if (OnDash != null)
			OnDash ();
		
		Vector3 movementTemp = new Vector3(player.GetAxisRaw("Move Horizontal"), 0f, player.GetAxisRaw("Move Vertical"));
		movementTemp = movementTemp.normalized;

		float dashSpeedTemp = dashSpeed;

		DOTween.To (() => dashSpeedTemp, x => dashSpeedTemp = x, 0, dashDuration).SetEase (dashEase).SetId("Dash").OnUpdate(
			()=> playerRigidbody.velocity = movementTemp * dashSpeedTemp);
		

		yield return new WaitForSeconds (dashDuration - 0.05f);

		dashState = DashState.Cooldown;

		yield return new WaitForSeconds (dashCooldown);

		dashState = DashState.CanDash;
	}
		
	protected IEnumerator OnPlayerStateChange ()
	{
		PlayerState playerStateTemp = playerState;

		yield return null;

		if (playerState != playerStateTemp)
		{
			if (OnPlayerstateChange != null)
				OnPlayerstateChange ();
		}

		StartCoroutine (OnPlayerStateChange ());
	}

	public virtual void DeathParticles ()
	{
		//Vector3 pos = other.contacts[0].point;
		//Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
		Quaternion rot = Quaternion.FromToRotation(Vector3.forward, new Vector3(0, 0, 0));

		GameObject instantiatedParticles = Instantiate(GlobalVariables.Instance.DeadParticles, transform.position, rot) as GameObject;
		instantiatedParticles.transform.SetParent (GlobalVariables.Instance.ParticulesClonesParent);
		instantiatedParticles.transform.position = new Vector3(instantiatedParticles.transform.position.x, 2f, instantiatedParticles.transform.position.z);
		instantiatedParticles.transform.LookAt(new Vector3(0, 0, 0));
		instantiatedParticles.GetComponent<Renderer>().material.color = gameObject.GetComponent<Renderer>().material.color;
	}

	public virtual void Death ()
	{
		if(playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			if (OnDeath != null)
				OnDeath ();


			if(playerState == PlayerState.Holding)
			{
				Transform holdMovableTemp = null;

				for(int i = 0; i < transform.childCount; i++)
				{
					if(transform.GetChild(i).tag == "Movable" || transform.GetChild(i).tag == "HoldMovable")
					{
						holdMovableTemp = transform.GetChild (i);

						holdMovableTemp.gameObject.GetComponent<MovableScript>().hold = false;

						holdMovableTemp.transform.SetParent(null);
						holdMovableTemp.transform.SetParent(movableParent);
						holdMovableTemp.GetComponent<MovableScript>().AddRigidbody();
					}
				}
			}

			playerState = PlayerState.Dead;

			gameObject.SetActive (false);
		}
	}

	protected void OnDestroy ()
	{
		if(controllerNumber != -1 && controllerNumber != 0 && VibrationManager.Instance != null)
			VibrationManager.Instance.StopVibration (controllerNumber);
	}

	protected void OnDisable ()
	{
		if(controllerNumber != -1 && controllerNumber != 0 && VibrationManager.Instance != null)
			VibrationManager.Instance.StopVibration (controllerNumber);

		GlobalVariables.Instance.ListPlayers ();
	}


	protected void OnDeathVoid ()
	{
		if (OnDeath != null)
			OnDeath ();
	}
}
