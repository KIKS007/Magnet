using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

	[Header ("AI Target")]
	public Transform holdTarget;
	public Transform shootTarget;

	[Header ("AI States")]
	public bool isAimingPlayer;
	public bool isAimingCube;
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

	protected override void Awake ()
	{
		controllerNumber = -2;
		base.Awake ();
	}

	protected override void Start ()
	{
		base.Start ();

		//Setup (playerName, aiLevel);

		GlobalVariables.Instance.ListPlayers ();
	}

	public void Setup (PlayerName playerName, AILevel level)
	{
		this.playerName = playerName;
		aiLevel = level;

		name += " " + ((int)playerName + 1).ToString ();

		SetupDelays ();

		SetupZones ();

		aiAnimator = GetComponent<Animator> ();

		if (!GlobalVariables.Instance.dynamicCamera.targetsList.Contains (gameObject))
			GlobalVariables.Instance.dynamicCamera.otherTargetsList.Add (gameObject);

		GetComponent<AIFXAnimations> ().AISetup ();
	}

	void SetupZones ()
	{
		for (int i = 0; i < aiZones.Length; i++)
		{
			if (i != (int)aiLevel)
				aiZones [i].SetActive (false);
			else
				aiZones [i].SetActive (true);
		}
	}

	void SetupDelays ()
	{
		foreach(AIComponentsDelay l in aiComponentsDelay)
		{
			foreach(AIComponents c in l.components)
			{
				if(gameObject.GetComponent (c.ToString ()))
				{
					AIComponent component = gameObject.GetComponent (c.ToString ()) as AIComponent;
					component.enableDelay = l.delays [(int)aiLevel];
				}
			}
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

	void FindCloserElements ()
	{
		if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
			return;

		closerPlayers.Clear ();
		closerPlayers.AddRange (GlobalVariables.Instance.AlivePlayersList);

		closerPlayers = closerPlayers.OrderBy (x => Vector3.Distance (transform.position, x.transform.position)).ToList ();

		closerPlayers.Remove (gameObject);

		closerCubes.Clear ();

		foreach (GameObject g in GlobalVariables.Instance.AllMovables)
			if (g.tag != "DeadCube")
				closerCubes.Add (g);

		closerCubes = closerCubes.OrderBy (x => Vector3.Distance (transform.position, x.transform.position)).ToList ();

		if(holdMovableTransform != null && closerCubes.Contains (holdMovableTransform.gameObject))
			closerCubes.Remove (holdMovableTransform.gameObject);
	}

	void FindDangerousCubes ()
	{
		if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
			return;

		thrownDangerousCubes.Clear ();
		dangerousCubes.Clear ();

		foreach(GameObject cube in GlobalVariables.Instance.AllMovables)
		{
			if(cube.tag == "ThrownMovable")
			{
				RaycastHit hitInfo;

				if(cube.GetComponent<Rigidbody> ().velocity.magnitude > 50)
				{
					if(Physics.Raycast (cube.transform.position, cube.GetComponent<Rigidbody> ().velocity, out hitInfo, 2000f, playerLayer))
					{
						if(hitInfo.collider.gameObject == gameObject)
							thrownDangerousCubes.Add (cube);
					}
				}
			}

			else if(cube.tag == "DeadCube")
			{
				dangerousCubes.Add (cube);

				dangerousCubes = dangerousCubes.OrderBy (x => Vector3.Distance (transform.position, x.transform.position)).ToList ();
			}
		}
	}

	void SetAnimatorParameters ()
	{
		if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
			return;
		
		aiAnimator.SetBool ("canDash", dashState == DashState.CanDash);
		aiAnimator.SetBool ("isDashing", dashState == DashState.Dashing);

		aiAnimator.SetBool ("isStunned", playerState == PlayerState.Stunned);

		aiAnimator.SetBool ("canHold", holdState == HoldState.CanHold);
		aiAnimator.SetBool ("isHolding", holdState == HoldState.Holding);

		aiAnimator.SetBool ("hasPlayerTarget", shootTarget != null);
		aiAnimator.SetBool ("hasCubeTarget", holdTarget != null);

		aiAnimator.SetInteger ("thrownDangerousCubes", thrownDangerousCubes.Count);

		aiAnimator.SetInteger ("attractedCubes", cubesAttracted.Count);
		aiAnimator.SetInteger ("repulsedCubes", cubesRepulsed.Count);

		if(closerPlayers.Count > 0)
			aiAnimator.SetFloat ("closerPlayerDistance", Vector3.Distance (transform.position, closerPlayers [0].transform.position));
		
		if(closerCubes.Count > 0)
			aiAnimator.SetFloat ("closerCubeDistance", Vector3.Distance (transform.position, closerCubes [0].transform.position));

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

	void SetupSkin ()
	{
		
	}

	public override IEnumerator Dash ()
	{
		dashState = DashState.Dashing;

		OnDashVoid ();

		movement.Normalize ();

		float dashSpeedTemp = dashSpeed;
		float futureTime = Time.time + dashDuration;
		float start = futureTime - Time.time;

		StartCoroutine(DashEnd());

		while (Time.time <= futureTime)
		{
			dashSpeedTemp = dashEase.Evaluate((futureTime - Time.time) / start) * dashSpeed;
			playerRigidbody.velocity = movement * dashSpeedTemp * Time.fixedDeltaTime * 200 * 1 / Time.timeScale;

			yield return new WaitForFixedUpdate();
		}

		movement = Vector3.zero;
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

		GlobalVariables.Instance.lastManManager.PlayerDeath (playerName, gameObject);
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

				GlobalVariables.Instance.screenShakeCamera.CameraShaking(FeedbackType.DashStun);
				GlobalVariables.Instance.zoomCamera.Zoom(FeedbackType.DashStun);
			}
		}
	}

	public override void OnHoldMovable (GameObject movable)
	{
		base.OnHoldMovable (movable);

		if(closerCubes.Contains (holdMovableTransform.gameObject))
			closerCubes.Remove (holdMovableTransform.gameObject);
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
	public float[] delays = new float[3];
	public List<AIComponents> components = new List<AIComponents> ();
}