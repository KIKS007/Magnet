using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DarkTonic.MasterAudio;
using Rewired;
using GameAnalyticsSDK;

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
    public event EventHandler OnCubeHit;
    public event EventHandler OnDashHit;
    public event EventHandler OnDash;
    public event EventHandler OnDashAvailable;
    public event EventHandler OnDeath;

    public event EventHandler OnPlayerstateChange;

    [Header("States")]
    public Team team;
    public PlayerState playerState = PlayerState.None;
    public DashState dashState = DashState.CanDash;
    public RepulsionWaveState repulsionState = RepulsionWaveState.CanRepulse;

    [Header("Controller Number")]
    public int controllerNumber = -1;
    [HideInInspector]
    public Player player; // The Rewired Player

    [Header("Movement")]
    public float speed = 18;
    public float stunnedSpeed = 8;
    public float maxVelocity;
    public float gravity = 100;

    protected float rightJoystickDeadzone = 0.85f;

    [Header("Forces")]
    public float attractionForce = 10;
    public float shootForce = 200;
    public float repulsionForce = 10;

    [Header("Deceleration")]
    [Range(0, 1)]
    public float decelerationAmount = 1;

    [Header("Repulsion Wave")]
    public bool enableRepulsionWave = false;
    public float repulsionWaveForce = 10;
    public float repulsionWaveRadius = 3;
    public LayerMask repulsionWaveMask = (1 << 9) | (1 << 13);
    public float repulsionCoolDown = 2;

    [Header("Stun")]
    public float stunnedRotation = 400;
    public float stunnedDuration = 1;

    [Header("Dash")]
    public float dashSpeed = 100;
    public float dashDuration = 0.12f;
    public float dashCooldown = 1f;
    public AnimationCurve dashEase;

    public List<GameObject> cubesAttracted = new List<GameObject>();
    public List<GameObject> cubesRepulsed = new List<GameObject>();

    protected Transform movableParent;
    [HideInInspector]
    public Transform magnetPoint;

    protected float lerpHold = 0.05f;

    [HideInInspector]
    public Rigidbody playerRigidbody;
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

    protected float startModeTime;

    // Use this for initialization
    protected virtual void Start()
    {
        GlobalVariables.Instance.OnModeStarted += StartModeTime;

        GetControllerNumber();

        Controller();

        if (GameObject.FindGameObjectWithTag("MainMenuManager") != null)
            mainMenuScript = GameObject.FindGameObjectWithTag("MainMenuManager").GetComponent<MainMenuManagerScript>();

        triggerMask = LayerMask.GetMask("FloorMask");
        playerRigidbody = GetComponent<Rigidbody>();
        originalSpeed = speed;

        movableParent = GameObject.FindGameObjectWithTag("MovableParent").transform;
        magnetPoint = transform.GetChild(0).transform;
        transform.GetChild(2).GetComponent<MagnetTriggerScript>().magnetPoint = magnetPoint;
    }

    protected IEnumerator WaitTillPlayerEnabled()
    {
        yield return new WaitUntil(() => gameObject.activeSelf == true);

        StartCoroutine(OnPlayerStateChange());
        StartCoroutine(OnDashAvailableEvent());
    }

    protected void StartModeTime()
    {
        startModeTime = Time.unscaledTime;
    }

    protected void OnEnable()
    {
        StartCoroutine(WaitTillPlayerEnabled());

        playerState = PlayerState.None;
        dashState = DashState.CanDash;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
        {
            if (playerRigidbody.velocity.magnitude > maxVelocity)
                maxVelocity = playerRigidbody.velocity.magnitude;

            ActivateFunctions();

            if (playerState == PlayerState.Stunned)
            {
                transform.Rotate(0, stunnedRotation * Time.deltaTime, 0, Space.World);
            }

            if (playerState == PlayerState.Attracting && !player.GetButton("Attract"))
                playerState = PlayerState.None;

            if (playerState == PlayerState.Repulsing && !player.GetButton("Repulse"))
                playerState = PlayerState.None;

            if (dashState != DashState.Dashing && playersHit.Count > 0)
                playersHit.Clear();

            OnAttractedOnRepulsed();
        }
    }

    protected virtual void ActivateFunctions()
    {
        movement = new Vector3(player.GetAxisRaw("Move Horizontal"), 0f, player.GetAxisRaw("Move Vertical"));
        movement.Normalize();

        if (controllerNumber == 0 && playerState != PlayerState.Stunned)
            TurningMouse();

        else if (controllerNumber > 0 && playerState != PlayerState.Stunned)
            TurningGamepad();


        if (playerState == PlayerState.Holding && player.GetButtonUp("Attract"))
        {
            Shoot();

            playerState = PlayerState.None;
        }


        if (playerState == PlayerState.None)
        {
            if (player.GetButton("Attract"))
                playerState = PlayerState.Attracting;

            if (player.GetButton("Repulse"))
                playerState = PlayerState.Repulsing;
        }

        if (player.GetButtonDown("Dash") && dashState == DashState.CanDash && movement != Vector3.zero)
        {
            StartCoroutine(Dash());
        }

        if (player.GetButtonDown("Repulse") && repulsionState == RepulsionWaveState.CanRepulse && enableRepulsionWave && playerState != PlayerState.Holding)
        {
            StartCoroutine(RepulsionWave());
        }
    }

    protected virtual void FixedUpdate()
    {
        if (playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
        {
            if (dashState != DashState.Dashing)
                playerRigidbody.MovePosition(transform.position + movement * speed * Time.deltaTime);


            if (playerState == PlayerState.Holding)
            {
                holdMovableTransform.position = Vector3.Lerp(holdMovableTransform.position, magnetPoint.transform.position, lerpHold);
                holdMovableTransform.transform.rotation = Quaternion.Lerp(holdMovableTransform.rotation, transform.rotation, lerpHold);

                if (OnHolding != null)
                    OnHolding();
            }

            playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x * decelerationAmount, playerRigidbody.velocity.y, playerRigidbody.velocity.z * decelerationAmount);

            playerRigidbody.AddForce(-Vector3.up * gravity, ForceMode.Acceleration);

            if (cubesAttracted.Count > 0)
            {
                for (int i = 0; i < cubesAttracted.Count; i++)
                    Attraction(cubesAttracted[i]);
            }

            if (cubesRepulsed.Count > 0)
            {
                for (int i = 0; i < cubesRepulsed.Count; i++)
                    Repulsion(cubesRepulsed[i]);
            }
        }
    }

    public void GetControllerNumber()
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

    public void Controller()
    {
        if (controllerNumber == -1)
        {
            gameObject.SetActive(false);
        }

        if (controllerNumber != -1)
        {
            player = ReInput.players.GetPlayer(controllerNumber);

			if(controllerNumber > 0)
			{
				for(int i = 0; i < GamepadsManager.Instance.gamepadsList.Count; i++)
				{
					if(GamepadsManager.Instance.gamepadsList[i].GamepadId == controllerNumber)
						player.controllers.AddController(GamepadsManager.Instance.gamepadsList[i].GamepadController, true);
				}
			}
        }
    }

    public virtual void Attraction(GameObject movable)
    {
        playerState = PlayerState.Attracting;

        Vector3 movableAttraction = transform.position - movable.transform.position;

		movableAttraction.Normalize ();
        movable.GetComponent<Rigidbody>().AddForce(movableAttraction * attractionForce * 10, ForceMode.Force);

        if (OnAttracting != null)
            OnAttracting();
    }

    public virtual void Shoot()
    {
        holdMovableTransform.GetChild(0).GetComponent<SlowMotionTriggerScript>().triggerEnabled = true;

        holdMovableTransform.gameObject.GetComponent<MovableScript>().hold = false;
        holdMovableTransform.gameObject.GetComponent<MovableScript>().OnRelease();
        holdMovableTransform.transform.SetParent(null);
        holdMovableTransform.transform.SetParent(movableParent);
        holdMovableTransform.GetComponent<MovableScript>().playerThatThrew = gameObject;
        holdMovableTransform.GetComponent<MovableScript>().AddRigidbody();
        holdMovableRB = holdMovableTransform.GetComponent<Rigidbody>();
        holdMovableTransform.gameObject.tag = "ThrownMovable";


        holdMovableTransform.GetComponent<MovableScript>().currentVelocity = 200;
        holdMovableRB.AddForce(transform.forward * shootForce, ForceMode.VelocityChange);

		playerRigidbody.AddForce(transform.forward * -holdMovableRB.mass * 5, ForceMode.Force);

        if (OnShoot != null)
            OnShoot();

    }

    public virtual void Repulsion(GameObject movable)
    {
        if (!enableRepulsionWave)
        {
            playerState = PlayerState.Repulsing;

            Vector3 movableRepulsion = movable.transform.position - transform.position;
			movableRepulsion.Normalize ();
			movable.GetComponent<Rigidbody>().AddForce(movableRepulsion * repulsionForce * 10, ForceMode.Acceleration);

            if (OnRepulsing != null)
                OnRepulsing();
        }
    }

    protected IEnumerator RepulsionWave()
    {
        playerState = PlayerState.Repulsing;
        repulsionState = RepulsionWaveState.Repulsing;

        foreach (Collider other in Physics.OverlapSphere(transform.position, repulsionWaveRadius, repulsionWaveMask))
        {
            Vector3 repulseDirection = other.transform.position - transform.position;
            repulseDirection.Normalize();

            float explosionImpactZone = 1 - (Vector3.Distance(transform.position, other.transform.position) / repulsionWaveRadius);

            if (explosionImpactZone > 0)
            {
                if (other.GetComponent<Rigidbody>() != null)
                    other.GetComponent<Rigidbody>().AddForce(repulseDirection * explosionImpactZone * repulsionWaveForce, ForceMode.Impulse);
            }
        }

        repulsionState = RepulsionWaveState.Cooldown;

        yield return new WaitForSeconds(repulsionCoolDown);

        repulsionState = RepulsionWaveState.CanRepulse;
    }

    public virtual void OnHoldMovable(GameObject movable)
    {
        playerState = PlayerState.Holding;
        holdMovableTransform = movable.GetComponent<Transform>();
        movable.GetComponent<MovableScript>().OnHold();

        for (int i = 0; i < GlobalVariables.Instance.EnabledPlayersList.Count; i++)
        {
            if (GlobalVariables.Instance.EnabledPlayersList[i].GetComponent<PlayersGameplay>().cubesAttracted.Contains(movable))
                GlobalVariables.Instance.EnabledPlayersList[i].GetComponent<PlayersGameplay>().cubesAttracted.Remove(movable);

            if (GlobalVariables.Instance.EnabledPlayersList[i].GetComponent<PlayersGameplay>().cubesRepulsed.Contains(movable))
                GlobalVariables.Instance.EnabledPlayersList[i].GetComponent<PlayersGameplay>().cubesRepulsed.Remove(movable);
        }

        cubesAttracted.Clear();

        if (OnHold != null)
            OnHold();
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

    private List<GameObject> playersHit = new List<GameObject>();

    protected virtual void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "DeadZone" && playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
        {
            Death();

            DeathParticles();
        }

        if (other.collider.tag != "HoldMovable")
        {
            if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<PlayersGameplay>().playerState != PlayerState.Stunned && dashState == DashState.Dashing && !playersHit.Contains(other.gameObject))
            {
                playersHit.Add(other.gameObject);
                other.gameObject.GetComponent<PlayersGameplay>().StunVoid(false);

                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking(SlowMotionType.DashStun);
            }
        }
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "DeadZone" && playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
        {
            Death();

            DeathParticles();
        }

        if (other.collider.tag != "HoldMovable")
        {
            if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<PlayersGameplay>().playerState != PlayerState.Stunned && dashState == DashState.Dashing && !playersHit.Contains(other.gameObject))
            {
                playersHit.Add(other.gameObject);
                other.gameObject.GetComponent<PlayersGameplay>().StunVoid(false);

                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking(SlowMotionType.DashStun);
            }
        }
    }

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
        Vector3 aim = new Vector3(player.GetAxis("Aim Horizontal"), 0, player.GetAxis("Aim Vertical"));

        if (aim.magnitude > rightJoystickDeadzone)
        {
            //transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Atan2(player.GetAxisRaw("Aim Horizontal"), player.GetAxisRaw("Aim Vertical")) * Mathf.Rad2Deg, transform.eulerAngles.z);
            //playerRigidbody.MoveRotation(Quaternion.Euler (new Vector3 (transform.eulerAngles.x, Mathf.Atan2 (player.GetAxisRaw ("Aim Horizontal"), player.GetAxisRaw ("Aim Vertical")) * Mathf.Rad2Deg, transform.eulerAngles.z));
            playerRigidbody.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, Mathf.Atan2(player.GetAxisRaw("Aim Horizontal"), player.GetAxisRaw("Aim Vertical")) * Mathf.Rad2Deg, transform.eulerAngles.z));
        }
    }

    public virtual void StunVoid(bool cubeHit)
    {
        StartCoroutine(Stun(cubeHit));
    }

    protected virtual IEnumerator Stun(bool cubeHit)
    {
        MagnetTriggerScript magnetTrigger = transform.GetChild(2).GetComponent<MagnetTriggerScript>();

        yield return new WaitWhile(() => magnetTrigger.gettingMovable == true);

        if (playerState == PlayerState.Holding)
        {
            playerState = PlayerState.Stunned;
            Shoot();
        }

        if (OnStun != null)
            OnStun();

        if (cubeHit && OnCubeHit != null)
            OnCubeHit();

        if (!cubeHit && OnDashHit != null)
            OnDashHit();

        playerState = PlayerState.Stunned;

        speed = stunnedSpeed;

        yield return new WaitForSeconds(stunnedDuration);

        if (playerState == PlayerState.Stunned)
            playerState = PlayerState.None;

        speed = originalSpeed;
    }

    protected IEnumerator Dash()
    {
        dashState = DashState.Dashing;

        if (OnDash != null)
            OnDash();

        Vector3 movementTemp = new Vector3(player.GetAxisRaw("Move Horizontal"), 0f, player.GetAxisRaw("Move Vertical"));
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

    IEnumerator DashEnd()
    {
        yield return new WaitForSeconds(dashDuration);

        dashState = DashState.Cooldown;

        yield return new WaitForSeconds(dashCooldown);

        dashState = DashState.CanDash;
    }

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

    public virtual void DeathParticles()
    {
        int playerNumber = -1;

        switch (gameObject.name)
        {
            case "Player 1":
                playerNumber = 0;
                break;
            case "Player 2":
                playerNumber = 1;
                break;
            case "Player 3":
                playerNumber = 2;
                break;
            case "Player 4":
                playerNumber = 3;
                break;

        }

        GameObject instance = Instantiate(GlobalVariables.Instance.explosionFX[playerNumber], transform.position, GlobalVariables.Instance.explosionFX[playerNumber].transform.rotation) as GameObject;
        instance.transform.parent = GlobalVariables.Instance.ParticulesClonesParent.transform;
    }

    public virtual void Death()
    {
        if (playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
        {
            GameAnalytics.NewDesignEvent("Player:" + name + ":" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":LifeDuration", (int)(Time.unscaledTime - startModeTime));
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking(SlowMotionType.Death);

			if (playerState == PlayerState.Holding)
            {
				Transform holdMovableTemp = transform.GetComponentInChildren<MovableBomb> ().transform;

				holdMovableTemp.gameObject.GetComponent<MovableScript>().hold = false;
				
				holdMovableTemp.transform.SetParent(null);
				holdMovableTemp.transform.SetParent(movableParent);
				holdMovableTemp.GetComponent<MovableScript>().AddRigidbody();
            }

            playerState = PlayerState.Dead;

            for (int i = 0; i < GetComponent<PlayersFXAnimations>().attractionRepulsionFX.Count; i++)
            {
                Destroy(GetComponent<PlayersFXAnimations>().attractionRepulsionFX[i]);
            }

            gameObject.SetActive(false);
        }
    }

    protected virtual void OnDestroy()
    {
        if (controllerNumber != -1 && controllerNumber != 0 && VibrationManager.Instance != null)
            VibrationManager.Instance.StopVibration(controllerNumber);
    }

    protected virtual void OnDisable()
    {
		/* if (controllerNumber != -1 && controllerNumber != 0 && VibrationManager.Instance != null)
            VibrationManager.Instance.StopVibration(controllerNumber);*/

        if (playerState == PlayerState.Dead && OnDeath != null)
            OnDeath();

        StopCoroutine(OnPlayerStateChange());
        StopCoroutine(OnDashAvailableEvent());
    }

    protected IEnumerator OnDashAvailableEvent()
    {
        yield return new WaitWhile(() => dashState != DashState.Cooldown);

        yield return new WaitWhile(() => dashState == DashState.Cooldown);

        if (OnDashAvailable != null)
            OnDashAvailable();

        StartCoroutine(OnDashAvailableEvent());
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
}
