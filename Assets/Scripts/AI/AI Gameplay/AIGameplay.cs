using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public enum AILevel { Easy, Normal , Hard};

public class AIGameplay : PlayersGameplay 
{
	[Header ("AI")]
	public AILevel aiLevel;
	public LayerMask playerLayer = 1 << 12;
	public GameObject[] aiZones = new GameObject[3];

	[Header ("AI Elements")]
	public List<GameObject> closerPlayers = new List<GameObject> ();
	public List<GameObject> closerCubes = new List<GameObject> ();
	public List<GameObject> thrownDangerousCubes = new List<GameObject> ();
	public List<GameObject> dangerousCubes = new List<GameObject> ();
	public List<GameObject> objectives = new List<GameObject> ();

	[Header ("AI Cubes Velocity")]
	public float dangerousCubeVelocity = 50f;

	[Header ("AI Target")]
	public Transform holdTarget;
	public Transform shootTarget;

	[Header ("AI States")]
	public bool isAimingShootTarget;
	public bool isAimingHoldTarget;
	public bool isAttracting;
	public bool isRepelling;

	[Header ("AI Delay")]
	public List<AIComponentsDelay> aiComponentsDelay = new List<AIComponentsDelay> ();

	[Header ("AI Debug")]
	public bool dashLayerEnabled = true;
	public bool movementLayerEnabled = true;
	public bool shootLayerEnabled = true;
	public bool aimLayerEnabled = true;

	[HideInInspector]
	public Animator aiAnimator;
	[HideInInspector]
	public Vector3 dashMovement;

	protected ArenaDeadzones arenaDeadzones;

	protected override void Awake ()
	{
		controllerNumber = -2;
		base.Awake ();
	}

	protected override void Start ()
	{
		base.Start ();

		//Setup (playerName, aiLevel);

		arenaDeadzones = FindObjectOfType<ArenaDeadzones> ();

		GlobalVariables.Instance.ListPlayers ();
	}

	public void Setup (PlayerName playerName, AILevel level)
	{
		this.playerName = playerName;
		aiLevel = level;

		name += " " + ((int)playerName + 1).ToString ();

		SetupZones ();

		aiAnimator = GetComponent<Animator> ();

		if (!GlobalVariables.Instance.dynamicCamera.targetsList.Contains (gameObject) && SceneManager.GetActiveScene ().name == "Scene Testing")
			GlobalVariables.Instance.dynamicCamera.otherTargetsList.Add (gameObject);

		GetComponent<AIFXAnimations> ().AISetup ();
	}

	protected virtual void SetupZones ()
	{
		for (int i = 0; i < aiZones.Length; i++)
		{
			if (i != (int)aiLevel)
				aiZones [i].SetActive (false);
			else
				aiZones [i].SetActive (true);
		}
	}

	protected override IEnumerator Startup ()
	{
		playerState = PlayerState.Startup;

		yield return new WaitUntil (() => GlobalVariables.Instance.GameState == GameStateEnum.Playing);

		switch (GlobalVariables.Instance.Startup)
		{
		case StartupType.Delayed:
			yield return new WaitForSeconds (GlobalVariables.Instance.delayedStartupDuration);
			break;
		case StartupType.Wave:
			yield return new WaitForSeconds (0.25f);
			playerFX.WaveFX ();
			yield return new WaitForSeconds (GlobalVariables.Instance.delayBetweenWavesFX * 4);
			break;
		}

		GlobalVariables.Instance.Startup = StartupType.Done;
		playerState = PlayerState.None;
	}

	protected override void OnEnable ()
	{
		movement = Vector3.zero;

		closerPlayers.Clear ();
		closerCubes.Clear ();

		base.OnEnable ();
	}

	protected override void Update ()
	{
		FindCloserElements ();

		FindDangerousCubes ();

		SetAnimatorParameters ();

		if (playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing && playerState != PlayerState.Startup)
		{
			//Stunned Rotation
			if (playerState == PlayerState.Stunned)
				transform.Rotate(0, stunnedRotation * Time.deltaTime, 0, Space.World);

			//Reset Attraction - Repulsion State
			if (playerState == PlayerState.Attracting && !isAttracting)
				playerState = PlayerState.None;

			if (playerState == PlayerState.Repulsing && !isRepelling)
				playerState = PlayerState.None;

			//On Attracted - On Repulsed Events
			OnAttractedOnRepulsed();
		}
	}

	protected virtual void FindCloserElements ()
	{
		if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
			return;

		closerPlayers.Clear ();

		foreach (var item in GlobalVariables.Instance.AlivePlayersList) 
		{
			if(item.activeSelf)
				closerPlayers.Add (item);	
		}

		closerPlayers = closerPlayers.OrderBy (x => Vector3.Distance (transform.position, x.transform.position)).ToList ();

		closerPlayers.Remove (gameObject);

		closerCubes.Clear ();

		foreach (GameObject g in GlobalVariables.Instance.AllMovables)
			if (g.tag != "DeadCube" && g.tag != "Suggestible")
				closerCubes.Add (g);

		closerCubes = closerCubes.OrderBy (x => Vector3.Distance (transform.position, x.transform.position)).ToList ();

		if(holdMovableTransform != null && closerCubes.Contains (holdMovableTransform.gameObject))
			closerCubes.Remove (holdMovableTransform.gameObject);
	}

	protected virtual void FindDangerousCubes ()
	{
		if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
			return;

		thrownDangerousCubes.Clear ();
		dangerousCubes.Clear ();

		//dangerousCubes.AddRange (arenaDeadzones.deadlyColumns);

		foreach(GameObject cube in GlobalVariables.Instance.AllMovables)
		{
			if(cube.tag == "DeadCube" || cube.tag == "Suggestible")
			{
				if (holdMovableTransform && cube == holdMovableTransform.gameObject)
					continue;

				dangerousCubes.Add (cube);

				dangerousCubes = dangerousCubes.OrderBy (x => Vector3.Distance (transform.position, x.transform.position)).ToList ();
			}

			RaycastHit hitInfo;
			Rigidbody rigidbody = cube.GetComponent<Rigidbody> ();

			if (rigidbody == null)
				continue;

			if(rigidbody.velocity.magnitude > dangerousCubeVelocity)
			{
				if(Physics.Raycast (cube.transform.position, rigidbody.velocity, out hitInfo, 2000f, playerLayer))
				{
					if(hitInfo.collider.gameObject == gameObject)
						thrownDangerousCubes.Add (cube);
				}
			}
		}
	}

	protected virtual void SetAnimatorParameters ()
	{
		if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
			return;
		
		aiAnimator.SetBool ("canDash", dashState == DashState.CanDash);
		aiAnimator.SetBool ("isDashing", dashState != DashState.CanDash);

		aiAnimator.SetBool ("isStunned", playerState == PlayerState.Stunned);

		aiAnimator.SetBool ("canHold", holdState == HoldState.CanHold);
		aiAnimator.SetBool ("isHolding", holdState == HoldState.Holding);

		aiAnimator.SetBool ("hasShootTarget", shootTarget != null);
		aiAnimator.SetBool ("hasHoldTarget", holdTarget != null);

		aiAnimator.SetInteger ("thrownDangerousCubes", thrownDangerousCubes.Count);

		aiAnimator.SetInteger ("attractedCubes", cubesAttracted.Count);
		aiAnimator.SetInteger ("repulsedCubes", cubesRepulsed.Count);

		aiAnimator.SetInteger ("closerPlayersCount", closerPlayers.Count);
		aiAnimator.SetInteger ("closerCubesCount", closerCubes.Count);

		if(closerPlayers.Count > 0)
			aiAnimator.SetFloat ("closerPlayerDistance", Vector3.Distance (transform.position, closerPlayers [0].transform.position));
		else
			aiAnimator.SetFloat ("closerPlayerDistance", 666);

		if(closerCubes.Count > 0)
			aiAnimator.SetFloat ("closerCubeDistance", Vector3.Distance (transform.position, closerCubes [0].transform.position));
		else
			aiAnimator.SetFloat ("closerCubeDistance", 666);

		aiAnimator.SetFloat ("distanceFromCenter", Vector3.Distance (Vector3.zero, transform.position));

		if(dangerousCubes.Count != 0)
			aiAnimator.SetFloat ("closerDangerousCubeDistance", Vector3.Distance (dangerousCubes [0].transform.position, transform.position));
		else
			aiAnimator.SetFloat ("closerDangerousCubeDistance", 666);

		if(shootTarget != null)
			aiAnimator.SetFloat ("playerTargetDistance", Vector3.Distance (shootTarget.position, transform.position));
		else
			aiAnimator.SetFloat ("playerTargetDistance", 666);

		if(holdTarget != null)
			aiAnimator.SetFloat ("cubeTargetDistance", Vector3.Distance (holdTarget.position, transform.position));
		else
			aiAnimator.SetFloat ("cubeTargetDistance", 666);
	}

	protected override void FixedUpdate ()
	{
		if (playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing && playerState != PlayerState.Startup)
		{
			//Movement
			if (dashState != DashState.Dashing)
			{
				movement.Normalize ();

				float speedTemp = playerState != PlayerState.Stunned ? speed : stunnedSpeed;
				playerRigidbody.MovePosition(transform.position + movement * speedTemp * Time.fixedDeltaTime);
			}

			//No Forces
			velocity = playerRigidbody.velocity.magnitude;

			if (velocity < noForcesThreshold && playerThatHit != null && playerState != PlayerState.Stunned)
				playerThatHit = null;
			
			//Hold Movable
			if (holdState == HoldState.Holding)
			{
				holdMovableTransform.position = Vector3.Lerp(holdMovableTransform.position, magnetPoint.transform.position, lerpHold);
				holdMovableTransform.transform.rotation = Quaternion.Lerp(holdMovableTransform.rotation, transform.rotation, lerpHold);

				OnHoldingVoid ();
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
	}

	public override void SetupController ()
	{
		controllerNumber = GlobalVariables.Instance.PlayersControllerNumber [(int)playerName];

		if (controllerNumber == -1)
		{
			gameObject.SetActive(false);
		}
		else
			SetupSkin ();
	}

	protected virtual void SetupSkin ()
	{
		
	}

	public override IEnumerator Dash ()
	{
		dashState = DashState.Dashing;

		OnDashVoid ();

		dashMovement.Normalize ();

		float dashSpeedTemp = dashSpeed;
		float futureTime = Time.time + dashDuration;
		float start = futureTime - Time.time;

		StartCoroutine(DashEnd());

		while (Time.time <= futureTime)
		{
			dashSpeedTemp = dashEase.Evaluate((futureTime - Time.time) / start) * dashSpeed;
			playerRigidbody.velocity = dashMovement * dashSpeedTemp * Time.fixedDeltaTime * 200 * 1 / Time.timeScale;

			yield return new WaitForFixedUpdate();
		}

		dashMovement = Vector3.zero;
	}

	protected override IEnumerator DeathCoroutine ()
	{
		playerState = PlayerState.Dead;

		GlobalVariables.Instance.screenShakeCamera.CameraShaking(FeedbackType.Death);
		GlobalVariables.Instance.zoomCamera.Zoom(FeedbackType.Death);

		PlayerStats (playerThatHit);

		if(gettingMovable || holdState == HoldState.Holding)
		{
			yield return new WaitWhile(() => gettingMovable == true);

			holdMovableTransform.gameObject.GetComponent<MovableScript>().hold = false;
			holdMovableTransform.tag = "Movable";
			holdMovableTransform.SetParent(null);
			holdMovableTransform.SetParent(movableParent);
			holdMovableTransform.GetComponent<MovableScript>().AddRigidbody();
			holdMovableTransform.GetComponent<MovableScript>().OnRelease();	
		}

		holdState = HoldState.CannotHold;

		gameObject.SetActive(false);

		RemoveFromAIObjectives ();

		if(GlobalVariables.Instance != null)
			GlobalVariables.Instance.ListPlayers ();

		GlobalVariables.Instance.lastManManager.PlayerDeath (playerName, gameObject);
		
		OnDeathVoid ();
	}

	protected override void OnCollisionStay (Collision other)
	{
		if(playerState == PlayerState.Startup)
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

	protected override void OnCollisionEnter (Collision other)
	{
		if(playerState == PlayerState.Startup)
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

		if (other.collider.tag == "HoldMovable" && dashState == DashState.Dashing)
		{
			other.collider.GetComponent<MovableScript> ().player.GetComponent<PlayersGameplay> ().playerThatHit = this;
			//other.gameObject.GetComponent<PlayersGameplay> ().playerThatHit = this;
		}

		if(other.collider.gameObject.layer == LayerMask.NameToLayer ("Movables"))
		{
			MovableScript script = other.collider.gameObject.GetComponent<MovableScript> ();

			if (script != null && script.playerThatThrew != null)
				playerThatHit = script.playerThatThrew.GetComponent<PlayersGameplay> ();
		}

		if(other.collider.gameObject.layer == LayerMask.NameToLayer ("Walls"))
			aiAnimator.SetTrigger ("touchedWall");
	}

	public override void OnHoldMovable (GameObject movable, bool forceHold = false)
	{
		base.OnHoldMovable (movable, forceHold);

		if(closerCubes.Contains (movable))
			closerCubes.Remove (movable);
	}
}

[System.Serializable]
public class AIRandomAngle
{
	[Range (0, 45)]
	public float randomAngleMin;
	[Range (0, 45)]
	public float randomAngleMax;
}

[System.Serializable]
public class AIComponentsDelay
{
	[MinMaxSliderAttribute (0, 2)]
	public Vector2[] delays = new Vector2[3];
	public List<AIComponents> components = new List<AIComponents> ();
}