using UnityEngine;
using System.Collections;
using XboxCtrlrInput;
using XInputDotNetPure;
using DG.Tweening;
using DarkTonic.MasterAudio;
using Rewired;

public class PlayersGameplay : MonoBehaviour 
{
	[Header ("Controller Number")]
	public int controllerNumber;
	[HideInInspector]
	public Player player; // The Rewired Player

	[Header ("Movement")]
	public float speed = 15;
	public float maxVelocityChange = 10f;
	public float bumpedSpeed = 8;
	public float gamepadDeadZone;
	public float velocity;

	[Header ("Forces")]
	public float attractionForce = 10;
	public float shootForce = 200;
	public float repulsionForce = 10;

	[Header ("Bump")]
	public float bumpedRotation = 400;
	public float bumpedDuration = 1;

	[Header ("Dash")]
	public float dashSpeed = 30;
	public float dashDuration = 0.2f;
	public float dashCoolDown = 2;

	[Header ("Vibration")]
	[RangeAttribute(0, 1f)]
	public float motorVibration = 0.5f;
	public float durationVibration = 0.3f;

	[Header ("Game Paused")]
	public bool gamePaused = true;
	public bool gamePausedStats = false;

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

	[Space(10)]
	public GameObject slowMotionDetection;

	private float attractionSoundVolume;
	private float repulsionSoundVolume;

	private GameObject mainCamera;
	private Transform movableParent;
	private Transform magnetPoint;

	private TrailRenderer trail;

	private float lerpHold = 0.2f;

	private Rigidbody rigidbodyPlayer;
	private Vector3 movement;
	private Vector3 oldRotation;

	private int triggerMask;
	private float camRayLength = 200f;

	private bool bumpCoroutineRunning;
	private bool dashCoroutineRunning;

	private float originalSpeed;
	private float currentTimeScale;

	private GameObject deadParticlesPrefab;
	//private GameObject playerDestroyParticles;

	[HideInInspector]
	public Rigidbody holdMovable;
	[HideInInspector]
	public Transform holdMovableTransform;
	[HideInInspector]
	public bool holdingMovable = false;
	[HideInInspector]
	public RaycastHit objectHit;
	[HideInInspector]
	public RaycastHit playerHit;
	[HideInInspector]
	public bool mouseControl = false;
	[HideInInspector]
	public bool bumped;
	[HideInInspector]
	public bool dead;
	[HideInInspector]
	public Transform boxThatHitPlayer;

	//private float lerpRotation = 0.9f;
	//private Vector3 playerWorldToScreen;
	//private Camera cameraComponent;

	private float screenWidthEdge = 8.5f;
	private float screenHeightEdge = 7f;

	public bool rightBlocked;
	public bool leftBlocked;
	public bool topBlocked;
	public bool bottomBlocked;

	private MainMenuManagerScript mainMenuScript;

	// Use this for initialization
	void Start () 
	{
		DOTween.Init ();

		GetControllerNumber ();

		Controller ();

		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");

		mainMenuScript = GameObject.FindGameObjectWithTag ("MainMenuManager").GetComponent<MainMenuManagerScript> ();

		deadParticlesPrefab = GameObject.FindGameObjectWithTag("DeadParticles") as GameObject;

		//playerDestroyParticles = GameObject.FindGameObjectWithTag("PlayerDestroy") as GameObject;

		triggerMask = LayerMask.GetMask ("FloorMask");
		rigidbodyPlayer = GetComponent<Rigidbody>();
		originalSpeed = speed;

		movableParent = GameObject.FindGameObjectWithTag ("MovableParent").transform;
		magnetPoint = transform.GetChild (1).transform;
		trail = transform.GetChild (4).GetComponent<TrailRenderer>();

		//cameraComponent = mainCamera.GetComponent<Camera>();

		screenWidthEdge = screenWidthEdge / 100 * Screen.width;
		screenHeightEdge = screenHeightEdge / 100 * Screen.height;

		StartSounds ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(rigidbodyPlayer.velocity.magnitude > velocity)
			velocity = rigidbodyPlayer.velocity.magnitude;


		if (StaticVariables.GamePaused == false)
			WhichControllerFunctions ();
		

		if(bumped == true)
		{
			if(bumpCoroutineRunning == false)
			StartCoroutine("BumpedDuration");

			transform.Rotate(0, bumpedRotation * Time.deltaTime, 0, Space.World);
		}

		//SlowMotionDebug ();

		TrailLength ();

		StatsButton ();

		Pause ();

		Sounds ();
	}

	void WhichControllerFunctions ()
	{
		if(mouseControl == true && bumped == false)
			TurningMouse ();

		else if(mouseControl == false && bumped == false)
			TurningGamepad ();


		if(holdingMovable == true)
		{
			if(mouseControl == true && Input.GetMouseButtonUp(1))
			{
				Shoot ();
			}
			else if(mouseControl == false && XCI.GetAxisRaw(XboxAxis.RightTrigger, controllerNumber) == 0)
			{
				Shoot ();
			}
		}


		if(mouseControl == true)
		{
			movement = new Vector3(Input.GetAxisRaw("HorizontalPlayer"), 0f, Input.GetAxisRaw("VerticalPlayer"));
			movement = movement.normalized * speed * Time.deltaTime;
		}

		else
		{
			movement = new Vector3(XCI.GetAxisRaw(XboxAxis.LeftStickX, controllerNumber), 0f, XCI.GetAxisRaw(XboxAxis.LeftStickY, controllerNumber));
			movement = movement.normalized * speed * Time.deltaTime;
		}


		if(mouseControl == true && Input.GetKeyDown(KeyCode.Space))
		{
			if(dashCoroutineRunning == false)
			{
				StartCoroutine("Dash");
			}
		}

		else if(mouseControl == false && XCI.GetButtonDown(XboxButton.LeftStick, controllerNumber))
		{
			if(dashCoroutineRunning == false)
			{
				StartCoroutine("Dash");
			}
		}
	}
	
	void FixedUpdate ()
	{
		if (leftBlocked && movement.x < 0)
			movement.x = 0;

		if(rightBlocked && movement.x > 0)
			movement.x = 0;

		if(topBlocked && movement.z > 0)
			movement.z = 0;

		if(bottomBlocked && movement.z < 0)
			movement.z = 0;

		rigidbodyPlayer.MovePosition(transform.position + movement);

		//rigidbodyPlayer.velocity = movement;

		//PhysicsMovement ();

		if(holdingMovable)
		{
			holdMovableTransform.position = Vector3.Lerp(holdMovableTransform.position, magnetPoint.transform.position, lerpHold);
			holdMovableTransform.transform.rotation = Quaternion.Lerp(holdMovableTransform.rotation, transform.rotation, lerpHold);
		}
	}

	void PhysicsMovement ()
	{
		// Apply a force that attempts to reach our target velocity
		Vector3 velocity = GetComponent<Rigidbody>().velocity;
		Vector3 velocityChange = (movement - velocity);

		velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
		velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
		velocityChange.y = 0;

		if (leftBlocked && velocityChange.x < 0)
			velocityChange.x = 0;

		if(rightBlocked && velocityChange.x > 0)
			velocityChange.x = 0;

		if(topBlocked && velocityChange.z > 0)
			velocityChange.x = 0;

		if(bottomBlocked && velocityChange.z < 0)
			velocityChange.x = 0;


		rigidbodyPlayer.AddForce(velocityChange, ForceMode.VelocityChange);
	}

	public void GetControllerNumber ()
	{
		switch (gameObject.name)
		{
		case "Player 1":
			controllerNumber = StaticVariables.ControllerNumberPlayer1;
			break;
		case "Player 2":
			controllerNumber = StaticVariables.ControllerNumberPlayer2;
			break;
		case "Player 3":
			controllerNumber = StaticVariables.ControllerNumberPlayer3;
			break;
		case "Player 4":
			controllerNumber = StaticVariables.ControllerNumberPlayer4;
			break;
		}
	}

	public void Controller ()
	{
		if(controllerNumber == -1)
		{
			//Debug.Log(name + " : Deactivate");
			mouseControl = false;

			gameObject.SetActive(false);
		}

		if(controllerNumber == 0)
		{
			//Debug.Log(name + " : Keyboard");
			mouseControl = true;
		}

		if(controllerNumber > 0)
		{
			//Debug.Log(name + " : Gamepad");
			mouseControl = false;
		}

		if(gameObject.transform.GetChild(1).gameObject.activeSelf == false)
		{
			gameObject.transform.GetChild(1).gameObject.SetActive(true);
			gameObject.transform.GetChild(2).gameObject.SetActive(true);
		}

		if(controllerNumber != -1)
			player = ReInput.players.GetPlayer(controllerNumber);
	}

	public void Attraction ()
	{
		Vector3 movableAttraction = transform.position - objectHit.collider.gameObject.transform.position;
		Rigidbody movable = objectHit.collider.gameObject.GetComponent<Rigidbody>();
		movable.AddForce(movableAttraction * attractionForce, ForceMode.Force);
		//Debug.Log(movable.velocity.magnitude);
	}
		
	public void Shoot ()
	{
		holdMovableTransform.gameObject.GetComponent<MovableScript>().hold = false;
		holdingMovable = false;
		holdMovableTransform.transform.parent = null;
		holdMovableTransform.transform.parent = movableParent;
		holdMovableTransform.GetComponent<MovableScript>().AddRigidbody();
		holdMovable = holdMovableTransform.GetComponent<Rigidbody>();
		holdMovableTransform.gameObject.tag = "ThrownMovable";
		holdMovableTransform.GetComponent<MovableScript> ().currentVelocity = 200;
		holdMovable.AddForce(transform.forward * shootForce, ForceMode.VelocityChange);

		rigidbodyPlayer.AddForce(transform.forward * -holdMovable.mass * 5, ForceMode.VelocityChange);

		slowMotionDetection.GetComponent<SlowMotionDetectionScript>().detectingSlowMotion = true;
		slowMotionDetection.GetComponent<SlowMotionDetectionScript>().holdMovableTransform = holdMovableTransform;

		MasterAudio.PlaySound3DAtTransformAndForget (shootSound, transform);
	}

	public void Repulsion ()
	{
		Vector3 movableRepulsion = objectHit.collider.gameObject.transform.position - transform.position;
		Rigidbody movable = objectHit.collider.gameObject.GetComponent<Rigidbody>();
		movable.AddForce(movableRepulsion * repulsionForce, ForceMode.Force);
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
		if(!StaticVariables.GamePaused)
		{
			if(bumped || holdingMovable)
			{
				if(MasterAudio.GetGroupVolume(attractionSound) != 0)
					MasterAudio.FadeSoundGroupToVolume (attractionSound, 0, fadeDuration);

				if(MasterAudio.GetGroupVolume(repulsionSound) != 0)
					MasterAudio.FadeSoundGroupToVolume (repulsionSound, 0, fadeDuration);
			}


			if(!bumped && !holdingMovable)
			{
				
				if(MasterAudio.GetGroupVolume(attractionSound) == 0)
				{
					if(mouseControl == true && Input.GetMouseButton (1))
						MasterAudio.FadeSoundGroupToVolume (attractionSound, attractionSoundVolume, fadeDuration);


					if(controllerNumber > 0 && XCI.GetAxisRaw(XboxAxis.RightTrigger, controllerNumber) != 0)
						MasterAudio.FadeSoundGroupToVolume (attractionSound, attractionSoundVolume, fadeDuration);
				}

				else if(MasterAudio.GetGroupVolume(attractionSound) != 0)
				{
					if(mouseControl == true && !Input.GetMouseButton (1))
						MasterAudio.FadeSoundGroupToVolume (attractionSound, 0, fadeDuration);
					
					if(controllerNumber > 0 && XCI.GetAxisRaw(XboxAxis.RightTrigger, controllerNumber) == 0)
						MasterAudio.FadeSoundGroupToVolume (attractionSound, 0, fadeDuration);
				}

				if(MasterAudio.GetGroupVolume(repulsionSound) == 0)
				{
					if(mouseControl == true && Input.GetMouseButton (0))
						MasterAudio.FadeSoundGroupToVolume (repulsionSound, repulsionSoundVolume, fadeDuration);

					if(controllerNumber > 0 && XCI.GetAxisRaw(XboxAxis.LeftTrigger, controllerNumber) != 0)
						MasterAudio.FadeSoundGroupToVolume (repulsionSound, repulsionSoundVolume, fadeDuration);
				}

				else if(MasterAudio.GetGroupVolume(repulsionSound) != 0)
				{
					if(mouseControl == true && !Input.GetMouseButton (0))
						MasterAudio.FadeSoundGroupToVolume (repulsionSound, 0, fadeDuration);

					if(controllerNumber > 0 && XCI.GetAxisRaw(XboxAxis.LeftTrigger, controllerNumber) == 0)
						MasterAudio.FadeSoundGroupToVolume (repulsionSound, 0, fadeDuration);
				}
			}
		}

	}

	void TrailLength ()
	{
		if(rigidbodyPlayer.velocity.magnitude > 1)
		{
			trail.time = 0.1f;
			trail.startWidth = 1;
		}
		else if (rigidbodyPlayer.velocity.magnitude < 1)
		{
			trail.time = 0.15f;
			trail.startWidth = 0.5f;
		}
	}

	void SlowMotionDebug ()
	{
		if(mouseControl)
		{
			if(Input.GetKeyDown(KeyCode.E) && mainCamera.GetComponent<SlowMotionCamera>().slowMotion == false)
			{
				mainCamera.GetComponent<SlowMotionCamera>().slowMotion = true;
			}
			else if(Input.GetKeyDown(KeyCode.E) && mainCamera.GetComponent<SlowMotionCamera>().slowMotion == true)
			{
				mainCamera.GetComponent<SlowMotionCamera>().slowMotion = false;
			}
		}
		else
		{
			if(XCI.GetButtonDown(XboxButton.A) && mainCamera.GetComponent<SlowMotionCamera>().slowMotion == false)
			{
				mainCamera.GetComponent<SlowMotionCamera>().slowMotion = true;
			}
			else if(XCI.GetButtonDown(XboxButton.A)  && mainCamera.GetComponent<SlowMotionCamera>().slowMotion == true)
			{
				mainCamera.GetComponent<SlowMotionCamera>().slowMotion = false;
			}
		}
	}
		
	void Pause ()
	{
		if(controllerNumber == 0 && Input.GetKeyDown (KeyCode.Escape) || controllerNumber != 0 && XCI.GetButtonDown (XboxButton.Start, controllerNumber))
		{
			mainMenuScript.GamePauseResumeVoid ();
		}
	}

	void StatsButton ()
	{
		
	}

	void TurningGamepad ()
	{

		if(XCI.GetAxisRaw(XboxAxis.RightStickX, controllerNumber) > gamepadDeadZone || XCI.GetAxisRaw(XboxAxis.RightStickX, controllerNumber) < -gamepadDeadZone || 
			XCI.GetAxisRaw(XboxAxis.RightStickY, controllerNumber) > gamepadDeadZone || XCI.GetAxisRaw(XboxAxis.RightStickY, controllerNumber) < -gamepadDeadZone)
		{
			oldRotation = new Vector3(transform.eulerAngles.x, Mathf.Atan2(XCI.GetAxisRaw(XboxAxis.RightStickX, controllerNumber), XCI.GetAxisRaw(XboxAxis.RightStickY, controllerNumber)) * Mathf.Rad2Deg, transform.eulerAngles.z);
			//transform.eulerAngles =	new Vector3(transform.eulerAngles.x, Mathf.Atan2(XCI.GetAxisRaw(XboxAxis.RightStickX, controllerNumber), XCI.GetAxisRaw(XboxAxis.RightStickY, controllerNumber)) * Mathf.Rad2Deg, transform.eulerAngles.z);

			transform.eulerAngles = oldRotation;

		}
		else
		{
			transform.eulerAngles = oldRotation;
		}
	}

	void OnCollisionEnter (Collision other)
	{
		if(other.gameObject.tag == "DeadZone" && !dead)
		{
			dead = true;

			Vector3 pos = other.contacts[0].point;
			//Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
			Quaternion rot = Quaternion.FromToRotation(Vector3.forward, new Vector3(0, 0, 0));

			GameObject instantiatedParticles = Instantiate(deadParticlesPrefab, pos, rot) as GameObject;
			instantiatedParticles.transform.SetParent (StaticVariables.ParticulesClonesParent);
			instantiatedParticles.transform.position = new Vector3(instantiatedParticles.transform.position.x, 2f, instantiatedParticles.transform.position.z);
			instantiatedParticles.transform.LookAt(new Vector3(0, 0, 0));
			instantiatedParticles.GetComponent<Renderer>().material.color = gameObject.GetComponent<Renderer>().material.color;
			instantiatedParticles.AddComponent<ParticlesAutoDestroy>();

			if(holdingMovable)
			{
				for(int i = 0; i < transform.childCount; i++)
				{
					if(transform.GetChild(i).tag == "Movable" || transform.GetChild(i).tag == "HoldMovable")
					{
						transform.GetChild(i).transform.SetParent(null);
					}
				}
			}

			Destroy(gameObject);
		}

	}

	void OnCollisionStay (Collision other)
	{	
		if (other.gameObject.tag == "RightWall")
			rightBlocked = true;
		else
			rightBlocked = false;

		if (other.gameObject.tag == "LeftWall")
			leftBlocked = true;
		else
			leftBlocked = false;
		
		if (other.gameObject.tag == "UpWall")
			topBlocked = true;
		else
			topBlocked = false;
		
		if (other.gameObject.tag == "DownWall")
			bottomBlocked = true;
		else
			bottomBlocked = false;
	}

	void OnCollisionExit (Collision other)
	{
		/*if (other.gameObject.tag == "RightWall" || other.gameObject.tag == "LeftWall" || other.gameObject.tag == "UpWall" || other.gameObject.tag == "DownWall")
		{
			rightBlocked = false;
			leftBlocked = false;
			topBlocked = false;
			bottomBlocked = false;
		}	*/
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
			rigidbodyPlayer.MoveRotation (newRotatation);
		}
	
	}
		
	IEnumerator BumpedDuration ()
	{
		bumpCoroutineRunning = true;

		MasterAudio.PlaySound3DAtTransformAndForget (hitSound, transform);

		switch (controllerNumber)
		{
		case 0:
			break;

		case 1:
			GamePad.SetVibration (PlayerIndex.One, motorVibration, motorVibration);
			StartCoroutine (BumpedVibration (PlayerIndex.One));
			break;

		case 2:
			GamePad.SetVibration (PlayerIndex.Two, motorVibration, motorVibration);
			StartCoroutine (BumpedVibration (PlayerIndex.Two));
			break;

		case 3:
			GamePad.SetVibration (PlayerIndex.Three, motorVibration, motorVibration);
			StartCoroutine (BumpedVibration (PlayerIndex.Three));
			break;

		case 4:
			GamePad.SetVibration (PlayerIndex.Four, motorVibration, motorVibration);
			StartCoroutine (BumpedVibration (PlayerIndex.Four));
			break;

		default :
			break;
		}

		if(holdingMovable == true)
		{
			Shoot ();
		}

		speed = bumpedSpeed;
		
		yield return new WaitForSeconds(bumpedDuration);

		speed = originalSpeed;
		bumped = false;
		bumpCoroutineRunning = false;
	}

	IEnumerator BumpedVibration (PlayerIndex whichController)
	{
		yield return new WaitForSeconds (durationVibration);

		GamePad.SetVibration (whichController, 0f, 0f);
	}

	IEnumerator Dash ()
	{
		dashCoroutineRunning = true;
		speed = dashSpeed;

		yield return new WaitForSeconds(dashDuration);

		speed = originalSpeed;

		yield return new WaitForSeconds(dashCoolDown);

		dashCoroutineRunning = false;
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
