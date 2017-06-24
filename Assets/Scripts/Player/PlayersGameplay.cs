using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DarkTonic.MasterAudio;
using Rewired;
using GameAnalyticsSDK;

public enum PlayerState
{
	None,
	Attracting,
	Repulsing,
	Stunned,
	Dead,
	Startup
}

public enum DashState
{
	CanDash,
	Dashing,
	Cooldown
}

public enum HoldState 
{
	CanHold, 
	Holding, 
	CannotHold
}

public enum PlayerName
{
	Player1,
	Player2,
	Player3,
	Player4
}

public enum DeathFX 
{
	None, 
	Explosion, 
	Particles, 
	All
};

public delegate void EventHandler();

public class PlayersGameplay : MonoBehaviour
{
	#region Variables
    [Header("States")]
	public int livesCount = 0;
	public PlayerName playerName;
	public PlayerState playerState = PlayerState.None;
    public DashState dashState = DashState.CanDash;
	public HoldState holdState = HoldState.CanHold;

	[Header("Life Duration")]
	public int lifeDuration = 0;

    [Header("Controller Number")]
    public int controllerNumber = -1;
    [HideInInspector]
    public Player rewiredPlayer; // The Rewired Player

    [Header("Movement")]
    public float speed = 18;
    public float stunnedSpeed = 8;
    public float gravity = 100;

    protected float rightJoystickDeadzone = 0.5f;

    [Header("Forces")]
    public float attractionForce = 10;
    public float shootForce = 200;
    public float repulsionForce = 10;

    [Header("Stun")]
    public float stunnedRotation = 400;
    public float stunnedDuration = 0.6f;

    [Header("Dash")]
    public float dashSpeed = 80;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
	public AnimationCurve dashEase;

	[Header("Hits Taken")]
	public PlayersGameplay playerThatHit;

	protected string playerDeadCubeTag;

	public List<MovableScript> cubesAttracted = new List<MovableScript>();
	public List<MovableScript> cubesRepulsed = new List<MovableScript>();

    protected Transform movableParent;
    [HideInInspector]
    public Transform magnetPoint;

    protected float lerpHold = 0.05f;

    [HideInInspector]
    public Rigidbody playerRigidbody;
	[HideInInspector]
	public Vector3 movement;

    protected int triggerMask;
    protected float camRayLength = 200f;

    [HideInInspector]
    public Rigidbody holdMovableRB;
    [HideInInspector]
    public Transform holdMovableTransform;
	[HideInInspector]
	public bool gettingMovable = false;

    protected bool hasAttracted;
    protected bool hasRepulsed;

	protected float stunnedRotationTemp;

	protected PlayersFXAnimations playerFX;

	#endregion

	#region Setup
	protected virtual void Awake()
	{
		SetPlayerName ();		
	}

    // Use this for initialization
    protected virtual void Start()
    {
        triggerMask = LayerMask.GetMask("FloorMask");
        playerRigidbody = GetComponent<Rigidbody>();

		lifeDuration = 0;

		if(GameObject.FindGameObjectWithTag("MovableParent") != null)
		{
			movableParent = GameObject.FindGameObjectWithTag("MovableParent").transform;
			if (movableParent.childCount != 0)
				playerDeadCubeTag = movableParent.GetChild (0).tag;
			else
				Debug.LogWarning ("No Movables!");
		}
		
        magnetPoint = transform.GetChild(0).transform;
		transform.GetComponentInChildren<MagnetTriggerScript>().magnetPoint = magnetPoint;
		playerFX = GetComponent<PlayersFXAnimations> ();
    }

	protected void SetPlayerName()
	{
		switch (gameObject.name)
		{
		case "Player 1":
			playerName = PlayerName.Player1;
			break;
		case "Player 2":
			playerName = PlayerName.Player2;
			break;
		case "Player 3":
			playerName = PlayerName.Player3;
			break;
		case "Player 4":
			playerName = PlayerName.Player4;
			break;
		}
	}

    protected virtual void OnEnable()
    {
        StartCoroutine(WaitTillPlayerEnabled());

		if(GlobalVariables.Instance.GameState != GameStateEnum.Playing)
			StartCoroutine (Startup ());

		cubesAttracted.Clear();
		cubesRepulsed.Clear();

		if(playerRigidbody != null)
			playerRigidbody.velocity = Vector3.zero;

		if(playerState != PlayerState.Startup)
			playerState = PlayerState.None;
	
		StartCoroutine (LifeDuration ());
        
		dashState = DashState.CanDash;
		holdState = HoldState.CanHold;

		playerThatHit = null;

		if(GlobalVariables.Instance != null)
			GlobalVariables.Instance.ListPlayers ();

		if (controllerNumber == 0)
			GlobalVariables.Instance.SetPlayerMouseCursor ();

		if(gameObject.layer == LayerMask.NameToLayer ("Safe"))
		if (OnSafe != null)
			OnSafe();
    }

	protected virtual IEnumerator Startup ()
	{
		playerState = PlayerState.Startup;

		yield return new WaitUntil (() => GlobalVariables.Instance.GameState == GameStateEnum.Playing);

		if (controllerNumber == -1 && this.GetType () != typeof (AIGameplay))
			yield break;
		
		switch (GlobalVariables.Instance.Startup)
		{
		case StartupType.Delayed:
			yield return new WaitForSeconds (GlobalVariables.Instance.delayedStartupDuration);
			break;
		case StartupType.Wave:
			yield return new WaitForSeconds (0.25f);
			playerFX.WaveFX ();

			yield return new WaitForSeconds (GlobalVariables.Instance.delayedStartupDuration - 0.25f);
			break;
		}

		GlobalVariables.Instance.Startup = StartupType.Done;
		playerState = PlayerState.None;
	}

	protected IEnumerator WaitTillPlayerEnabled()
	{
		yield return new WaitUntil(() => gameObject.activeSelf == true);
				
		StartCoroutine(OnPlayerStateChange());
		StartCoroutine(OnDashAvailableEvent());
	}
	#endregion

	#region Update / FixedUpdate
    // Update is called once per frame
    protected virtual void Update()
    {
		if (rewiredPlayer == null)
			return;

		if (playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing && playerState != PlayerState.Startup)
        {
			//Movement Vector
			movement = new Vector3(rewiredPlayer.GetAxisRaw("Move Horizontal"), 0f, rewiredPlayer.GetAxisRaw("Move Vertical"));
			movement.Normalize();

			//Turning Player
			if (controllerNumber == 0 && playerState != PlayerState.Stunned)
				TurningMouse();

			else if (controllerNumber > 0 && playerState != PlayerState.Stunned)
				TurningGamepad();

			//Shoot
			if (holdState == HoldState.Holding && rewiredPlayer.GetButtonUp("Attract"))
				Shoot();

			//Dash
			if (rewiredPlayer.GetButtonDown("Dash") && dashState == DashState.CanDash && movement != Vector3.zero)
				StartCoroutine(Dash());

			//Taunt
			if (rewiredPlayer.GetButtonDown ("Taunt") && playerState != PlayerState.Stunned)
			{
				playerFX.WaveFX (true);

				if (OnTaunt != null)
					OnTaunt();
			}

			//Stunned Rotation
            if (playerState == PlayerState.Stunned)
				transform.Rotate(0, stunnedRotationTemp * Time.deltaTime, 0, Space.World);


			//Reset Attraction - Repulsion State
			if (playerState == PlayerState.Attracting && !rewiredPlayer.GetButton("Attract"))
			{
				playerState = PlayerState.None;
				
				if (OnAttractingEnd != null)
					OnAttractingEnd ();
			}

			if (playerState == PlayerState.Repulsing && !rewiredPlayer.GetButton("Repulse"))
			{
				playerState = PlayerState.None;
				
				if (OnRepulsingEnd != null)
					OnRepulsingEnd ();
			}


			//Attraction - Repulsion State
			if (playerState == PlayerState.None)
			{
				if (rewiredPlayer.GetButton("Attract"))
					playerState = PlayerState.Attracting;

				if (rewiredPlayer.GetButton("Repulse"))
					playerState = PlayerState.Repulsing;
			}

			//On Attracted - On Repulsed Events
            OnAttractedOnRepulsed();
        }
    }

    protected virtual void FixedUpdate()
    {
		if (rewiredPlayer == null)
			return;

		if (playerState == PlayerState.Dead || playerState == PlayerState.Stunned || GlobalVariables.Instance.GameState != GameStateEnum.Playing || playerState == PlayerState.Startup)
			return;

		//Movement
		if (dashState != DashState.Dashing)
		{
			float speedTemp = playerState != PlayerState.Stunned ? speed : stunnedSpeed;
			playerRigidbody.MovePosition(transform.position + movement * speedTemp * Time.fixedDeltaTime);
		}
		
		//Hold Movable
		if (holdState == HoldState.Holding)
		{
			holdMovableTransform.position = Vector3.Lerp(holdMovableTransform.position, magnetPoint.transform.position, lerpHold);
			holdMovableTransform.transform.rotation = Quaternion.Lerp(holdMovableTransform.rotation, transform.rotation, lerpHold);
			
			if (OnHolding != null)
				OnHolding();
		}
		
		//Gravity
		playerRigidbody.AddForce(-Vector3.up * gravity, ForceMode.Acceleration);
		
		//Attraction
		if (cubesAttracted.Count > 0)
		{
			for (int i = 0; i < cubesAttracted.Count; i++)
				Attraction(cubesAttracted[i]);
		}
		
		//Repulse
		if (cubesRepulsed.Count > 0)
		{
			for (int i = 0; i < cubesRepulsed.Count; i++)
				Repulsion(cubesRepulsed[i]);
		}
    }
	#endregion

	#region Rewired Controller
	public virtual void SetupController()
    {
		controllerNumber = GlobalVariables.Instance.PlayersControllerNumber [(int)playerName];

        if (controllerNumber == -1)
        {
            gameObject.SetActive(false);
			livesCount = 0;
        }

        if (controllerNumber != -1)
		{
			livesCount = GlobalVariables.Instance.LivesCount;
			rewiredPlayer = ReInput.players.GetPlayer(controllerNumber);
		}

		GlobalVariables.Instance.ListPlayers ();
    }
	#endregion

	#region Cubes Methods
    public virtual void Shoot()
    {
		holdState = HoldState.CanHold;
		playerState = PlayerState.None;

		MovableScript movableScript = holdMovableTransform.gameObject.GetComponent<MovableScript> ();

		movableScript.slowMoTrigger.triggerEnabled = true;

		movableScript.hold = false;
        holdMovableTransform.transform.SetParent(null);
        holdMovableTransform.transform.SetParent(movableParent);
		movableScript.playerThatThrew = gameObject;
		movableScript.AddRigidbody();
		holdMovableRB = movableScript.rigidbodyMovable;
        holdMovableTransform.gameObject.tag = "ThrownMovable";
		movableScript.OnRelease();


		movableScript.currentVelocity = 250;
        holdMovableRB.AddForce(transform.forward * shootForce, ForceMode.VelocityChange);

		playerRigidbody.AddForce(transform.forward * -holdMovableRB.mass * 5, ForceMode.VelocityChange);

        if (OnShoot != null)
            OnShoot();

    }

	public virtual void Attraction(MovableScript movable)
	{
		if (movable.tag == "HoldMovable" || movable.rigidbodyMovable == null)
			cubesAttracted.Remove (movable);

		playerState = PlayerState.Attracting;

		Vector3 movableAttraction = transform.position - movable.transform.position;

		movableAttraction.Normalize ();

		if(movable.rigidbodyMovable != null)
			movable.rigidbodyMovable.AddForce(movableAttraction * attractionForce * 10, ForceMode.Force);

		if (OnAttracting != null)
			OnAttracting();
	}

	public virtual void Repulsion(MovableScript movable)
    {
		if (movable.tag == "HoldMovable" || movable.rigidbodyMovable == null)
		{
			Debug.Log ("Here");
			cubesAttracted.Remove (movable);
		}
		
		playerState = PlayerState.Repulsing;
		
		Vector3 movableRepulsion = movable.transform.position - transform.position;
		movableRepulsion.Normalize ();
		movable.rigidbodyMovable.AddForce(movableRepulsion * repulsionForce * 10, ForceMode.Force);
		
		if (OnRepulsing != null)
			OnRepulsing();		
    }

    public virtual void OnHoldMovable(GameObject movable)
    {
		if (playerState == PlayerState.Dead || playerState == PlayerState.Stunned || holdState == HoldState.CannotHold)
			return;

		gettingMovable = true;
        
		holdState = HoldState.Holding;

		SetMagnetPointPosition (movable);

		MovableScript movableScript = movable.GetComponent<MovableScript> ();

		movable.tag = "HoldMovable";
		movableScript.player = transform;
		movableScript.DestroyRigibody();
		movableScript.OnHold();

		holdMovableTransform = movable.transform;
		holdMovableTransform.SetParent (transform);

        for (int i = 0; i < GlobalVariables.Instance.EnabledPlayersList.Count; i++)
        {
			PlayersGameplay playerScript = GlobalVariables.Instance.EnabledPlayersList [i].GetComponent<PlayersGameplay> ();

			if (playerScript.cubesAttracted.Contains(movableScript))
				playerScript.cubesAttracted.Remove(movableScript);

			if (playerScript.cubesRepulsed.Contains(movableScript))
				playerScript.cubesRepulsed.Remove(movableScript);
        }

		cubesAttracted.Clear();
		cubesRepulsed.Clear();

        if (OnHold != null)
            OnHold();

		gettingMovable = false;
    }

	protected void SetMagnetPointPosition (GameObject movable)
	{
		Vector3 scaleTemp = movable.GetComponent<MovableScript> ().initialScale;

		Vector3 v3 = magnetPoint.localPosition;
		v3.x = 0f;
		v3.y = (scaleTemp.y /2 + 0.1f) - 1;
		v3.z = 0.5f * scaleTemp.z + 1.2f;
		magnetPoint.localPosition = v3;
	}

	public void RemoveCubeAttractionRepulsion (MovableScript script)
	{
		if (cubesAttracted.Contains (script))
			cubesAttracted.Remove (script);

		if (cubesRepulsed.Contains (script))
			cubesRepulsed.Remove (script);
	}
	#endregion

	#region Collisions
	protected List<GameObject> playersHit = new List<GameObject>();

    protected virtual void OnCollisionStay(Collision other)
    {
		if(playerState == PlayerState.Startup || rewiredPlayer == null)
			return;

		if(other.gameObject.tag == "DeadZone" && gameObject.layer != LayerMask.NameToLayer ("Safe"))
		if (playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
			Death(DeathFX.All, other.contacts[0].point);

		if (other.collider.tag != "HoldMovable" && other.gameObject.tag == "Player")
		{
			PlayersGameplay playerScript = other.gameObject.GetComponent<PlayersGameplay> ();

			if (playerScript.playerState != PlayerState.Stunned && dashState == DashState.Dashing && !playersHit.Contains(other.gameObject))
			{
				playersHit.Add(other.gameObject);
				playerScript.StunVoid(false);
				playerScript.playerThatHit = this;
				
				GlobalVariables.Instance.screenShakeCamera.CameraShaking(FeedbackType.DashStun);
				GlobalVariables.Instance.zoomCamera.Zoom(FeedbackType.DashStun);
			}
		}
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
		if(playerState == PlayerState.Startup || rewiredPlayer == null)
			return;

		if(other.gameObject.tag == "DeadZone" && gameObject.layer != LayerMask.NameToLayer ("Safe"))
		if (playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
			Death(DeathFX.All, other.contacts[0].point);

		if (other.collider.tag != "HoldMovable" && other.gameObject.tag == "Player")
		{
			PlayersGameplay playerScript = other.gameObject.GetComponent<PlayersGameplay> ();

			if (playerScript.playerState != PlayerState.Stunned && dashState == DashState.Dashing && !playersHit.Contains(other.gameObject))
			{
				playersHit.Add(other.gameObject);
				playerScript.StunVoid(false);
				playerScript.playerThatHit = this;

				GlobalVariables.Instance.screenShakeCamera.CameraShaking(FeedbackType.DashStun);
				GlobalVariables.Instance.zoomCamera.Zoom(FeedbackType.DashStun);
			}
		}

		if(other.collider.gameObject.layer == LayerMask.NameToLayer ("Movables"))
		{
			MovableScript script = other.collider.gameObject.GetComponent<MovableScript> ();

			if (script != null && script.playerThatThrew != null)
				playerThatHit = script.playerThatThrew.GetComponent<PlayersGameplay> ();
		}
    }
	#endregion

	#region Turning
    protected virtual void TurningMouse()
    {
        // Create a ray from the mouse cursor on screen in the direction of the camera.
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Create a RaycastHit variable to store information about what was hit by the ray.
        RaycastHit floorHit;

        // Perform the raycast and if it hits something on the floor layer...
        if (Physics.Raycast(camRay, out floorHit, camRayLength, triggerMask))
        {
            // Create a vector from the player to the point on the floor the raycast from the mouse hit.
            Vector3 playerToMouse = floorHit.point - transform.position;

            // Ensure the vector is entirely along the floor plane.
            playerToMouse.y = 0f;

            // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
            Quaternion newRotatation = Quaternion.LookRotation(playerToMouse);

            // Set the player's rotation to this new rotation.
            playerRigidbody.MoveRotation(newRotatation);
        }
    }

    protected virtual void TurningGamepad()
    {
        Vector3 aim = new Vector3(rewiredPlayer.GetAxis("Aim Horizontal"), 0, rewiredPlayer.GetAxis("Aim Vertical"));

        if (aim.magnitude > rightJoystickDeadzone)
            playerRigidbody.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, Mathf.Atan2(rewiredPlayer.GetAxisRaw("Aim Horizontal"), rewiredPlayer.GetAxisRaw("Aim Vertical")) * Mathf.Rad2Deg, transform.eulerAngles.z));

		/*else if(rewiredPlayer.GetAxisRaw("Move Horizontal") != 0 || rewiredPlayer.GetAxisRaw("Move Vertical") != 0)
			playerRigidbody.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, Mathf.Atan2(rewiredPlayer.GetAxisRaw("Move Horizontal"), rewiredPlayer.GetAxisRaw("Move Vertical")) * Mathf.Rad2Deg, transform.eulerAngles.z));*/
    }
	#endregion

	#region Stun
    public virtual void StunVoid(bool cubeHit)
    {
		if (playerState == PlayerState.Stunned)
			return;

        StartCoroutine(Stun(cubeHit));
    }

    protected virtual IEnumerator Stun(bool cubeHit)
    {
		playerState = PlayerState.Stunned;

		stunnedRotationTemp = stunnedRotation;

		DOTween.To (()=> stunnedRotationTemp, x=> stunnedRotationTemp = x, stunnedRotationTemp * 0.3f, stunnedDuration * 0.5f).SetEase (Ease.OutQuint).SetDelay (stunnedDuration * 0.5f).SetUpdate (false);

		if(gettingMovable || holdState == HoldState.Holding)
		{
			holdState = HoldState.CannotHold;

			yield return new WaitWhile(() => gettingMovable == true);
			
			Shoot();
		}

		holdState = HoldState.CannotHold;

		GlobalVariables.Instance.screenShakeCamera.CameraShaking(FeedbackType.Stun);
		GlobalVariables.Instance.zoomCamera.Zoom(FeedbackType.Stun);

		if (OnStun != null)
            OnStun();

        if (cubeHit && OnCubeHit != null)
            OnCubeHit();

        if (!cubeHit && OnDashHit != null)
            OnDashHit();

        yield return new WaitForSeconds(stunnedDuration);

		playerRigidbody.velocity = Vector3.zero;

		playerThatHit = null;

        if (playerState == PlayerState.Stunned)
            playerState = PlayerState.None;

		if(holdState == HoldState.CannotHold)
			holdState = HoldState.CanHold;
    }
	#endregion

	#region Dash
	public virtual IEnumerator Dash()
    {
        dashState = DashState.Dashing;

        if (OnDash != null)
            OnDash();

        Vector3 movementTemp = new Vector3(rewiredPlayer.GetAxisRaw("Move Horizontal"), 0f, rewiredPlayer.GetAxisRaw("Move Vertical"));
        movementTemp = movementTemp.normalized;

        float dashSpeedTemp = dashSpeed;
        float futureTime = Time.time + dashDuration;
        float start = futureTime - Time.time;

        StartCoroutine(DashEnd());

        while (Time.time <= futureTime)
        {
            dashSpeedTemp = dashEase.Evaluate((futureTime - Time.time) / start) * dashSpeed;
            playerRigidbody.velocity = movementTemp * dashSpeedTemp * Time.fixedDeltaTime * 200 * 1 / Time.timeScale;

            yield return new WaitForFixedUpdate();
        }
    }

	public virtual IEnumerator DashEnd()
    {
        yield return new WaitForSeconds(dashDuration);

		playersHit.Clear();

        dashState = DashState.Cooldown;

        yield return new WaitForSeconds(dashCooldown);

        dashState = DashState.CanDash;
    }
	#endregion

	#region Death
	public virtual void Death(DeathFX deathFX, Vector3 deathPosition, GameObject killingPlayer = null)
	{
		if (playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			if(deathFX == DeathFX.Explosion || deathFX == DeathFX.All)
				playerFX.DeathExplosionFX (deathPosition);

			if(deathFX == DeathFX.Particles || deathFX == DeathFX.All)
				playerFX.DeathParticles(deathPosition);

			if (killingPlayer)
				playerThatHit = killingPlayer.GetComponent<PlayersGameplay> ();

			StartCoroutine (DeathCoroutine ());        
		}
	}

	protected virtual IEnumerator DeathCoroutine ()
	{
		playerState = PlayerState.Dead;

		GameAnalytics.NewDesignEvent("Player:" + name + ":" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":LifeDuration", 
			StatsManager.Instance.playersStats [playerName.ToString ()].playersStats [WhichStat.LifeDuration.ToString ()]);

		GlobalVariables.Instance.screenShakeCamera.CameraShaking(FeedbackType.Death);
		GlobalVariables.Instance.zoomCamera.Zoom(FeedbackType.Death);

		PlayerStats (playerThatHit);

		if(gettingMovable || holdState == HoldState.Holding)
		{
			yield return new WaitWhile(() => gettingMovable == true);

			MovableScript movableScript = holdMovableTransform.gameObject.GetComponent<MovableScript> ();

			movableScript.hold = false;
			holdMovableTransform.tag = "Movable";
			holdMovableTransform.SetParent(null);
			holdMovableTransform.SetParent(movableParent);

			if(movableScript.rigidbodyMovable == null)
				movableScript.AddRigidbody();

			movableScript.OnRelease();	
		}

		holdState = HoldState.CannotHold;

		gameObject.SetActive(false);

		RemoveFromAIObjectives ();

		GlobalVariables.Instance.lastManManager.PlayerDeath (playerName, gameObject);
	}

	protected virtual void RemoveFromAIObjectives ()
	{
		foreach(var player in GlobalVariables.Instance.AlivePlayersList)
		{
			if (player == gameObject)
				continue;

			AIGameplay aiScript = player.GetComponent<AIGameplay> ();

			if (aiScript == null)
				continue;

			if (aiScript.objectives.Contains (gameObject))
				aiScript.objectives.Remove (gameObject);

			if (aiScript.shootTarget == transform)
				aiScript.shootTarget = null;
		}
	}

	protected virtual void PlayerStats (PlayersGameplay playerThatHit = null)
	{
		if (playerThatHit == null)
			return;

		if (playerThatHit != this)
			StatsManager.Instance.PlayerKills (playerThatHit);
		else
			StatsManager.Instance.PlayerSuicides (this);
	}
		
    protected virtual void OnDestroy()
    {
        if (controllerNumber > 0 && VibrationManager.Instance != null)
			VibrationManager.Instance.StopVibration(controllerNumber);
    }

    protected virtual void OnDisable()
    {
        if (playerState == PlayerState.Dead && OnDeath != null)
            OnDeath();

		if(GlobalVariables.Instance != null)
			GlobalVariables.Instance.ListPlayers ();

		if (controllerNumber == 0 && GlobalVariables.Instance != null && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
			GlobalVariables.Instance.SetMouseVisibility (true);

        StopCoroutine(OnPlayerStateChange());
        StopCoroutine(OnDashAvailableEvent());
    }

	protected virtual IEnumerator LifeDuration ()
	{
		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		yield return new WaitForSecondsRealtime (1);

		if (playerState == PlayerState.Dead)
			yield break;

		lifeDuration += 1;

		StartCoroutine (LifeDuration ());
	}
	#endregion

	#region Events
	public event EventHandler OnAttracting;
	public event EventHandler OnAttracted;
	public event EventHandler OnAttractingEnd;
	public event EventHandler OnRepulsingEnd;
	public event EventHandler OnRepulsing;
	public event EventHandler OnRepulsed;
	public event EventHandler OnHolding;
	public event EventHandler OnHold;
	public event EventHandler OnShoot;
	public event EventHandler OnStun;
	public event EventHandler OnCubeHit;
	public event EventHandler OnDashHit;
	public event EventHandler OnDash;
	public event EventHandler OnDashAvailable;
	public event EventHandler OnDeath;
	public event EventHandler OnTaunt;
	public event EventHandler OnSafe;

	public event EventHandler OnPlayerstateChange;

	protected IEnumerator OnPlayerStateChange()
	{
		PlayerState playerStateTemp = playerState;

		yield return null;

		if (playerState != playerStateTemp)
		{
			if (OnPlayerstateChange != null)
				OnPlayerstateChange();
		}

		StartCoroutine(OnPlayerStateChange());
	}

    protected IEnumerator OnDashAvailableEvent()
    {
        yield return new WaitWhile(() => dashState != DashState.Cooldown);

        yield return new WaitWhile(() => dashState == DashState.Cooldown);

        if (OnDashAvailable != null)
            OnDashAvailable();

        StartCoroutine(OnDashAvailableEvent());
    }

	protected virtual void OnAttractedOnRepulsed()
	{
		if (playerState != PlayerState.Attracting)
			hasAttracted = false;

		if (playerState != PlayerState.Repulsing)
			hasRepulsed = false;


		if (playerState == PlayerState.Attracting && !hasAttracted)
		{
			hasAttracted = true;

			if (OnAttracted != null)
				OnAttracted();
		}

		if (playerState == PlayerState.Repulsing && !hasRepulsed)
		{
			hasRepulsed = true;

			if (OnRepulsed != null)
				OnRepulsed();
		}
	}

    protected void OnDeathVoid()
    {
        if (OnDeath != null)
            OnDeath();
    }

    protected void OnStunVoid()
    {
        if (OnStun != null)
            OnStun();
    }

	protected void OnHoldingVoid ()
	{
		if (OnHolding != null)
			OnHolding();
	}

	protected void OnTauntVoid ()
	{
		if (OnTaunt != null)
			OnTaunt();
	}

	protected void OnDashVoid()
	{
		if (OnDash != null)
			OnDash();
	}
	#endregion
}
