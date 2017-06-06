using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;

public class PlayersTutorial : PlayersGameplay 
{
	[Header ("MOVEMENT")]
	public float movingTime = 0;

	[Header ("DASH")]
	public int dashCount = 0;

	[Header ("DASH Hit")]
	public int dashHitCount = 0;

	[Header ("ATTRACT / REPEL")]
	public float attractTime = 0;
	public float repelTime = 0;

	[Header ("SHOOTS")]
	public int shootsCount = 0;
	public int hitsCount = 0;

	[Header ("DEADLY WALLS")]
	public int deathCount = 0;

	private TutorialManager tutorialManager;

	protected override void Start ()
	{
		tutorialManager = FindObjectOfType<TutorialManager> ();

		OnDash += () => dashCount++;
		OnDashHit += () => dashHitCount++;
		OnShoot += () => shootsCount++;
		OnDeath += () => 
		{
			if((tutorialManager.tutorialState & TutorialState.DeadlyWall) == TutorialState.DeadlyWall) deathCount++;
		};

		base.Start ();
	}

	protected override void Update ()
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
			if((tutorialManager.tutorialState & TutorialState.Shoot) == TutorialState.Shoot)
			if (holdState == HoldState.Holding && rewiredPlayer.GetButtonUp("Attract"))
				Shoot();

			//Dash
			if((tutorialManager.tutorialState & TutorialState.Dash) == TutorialState.Dash)
			if (rewiredPlayer.GetButtonDown("Dash") && dashState == DashState.CanDash && movement != Vector3.zero)
				StartCoroutine(Dash());

			//Taunt
			if (rewiredPlayer.GetButtonDown ("Taunt") && playerState != PlayerState.Stunned)
			{
				playerFX.WaveFX (true);

				OnTauntVoid ();
			}

			//Stunned Rotation
			if (playerState == PlayerState.Stunned)
				transform.Rotate(0, stunnedRotation * Time.deltaTime, 0, Space.World);

			//Reset Attraction - Repulsion State
			if (playerState == PlayerState.Attracting && !rewiredPlayer.GetButton("Attract"))
				playerState = PlayerState.None;

			if (playerState == PlayerState.Repulsing && !rewiredPlayer.GetButton("Repulse"))
				playerState = PlayerState.None;

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


		//////
		hitsCount = StatsManager.Instance.playersStats [playerName.ToString ()].playersStats [WhichStat.Frags.ToString ()];;

		if(playerState != PlayerState.Dead && playerState != PlayerState.Stunned && playerState != PlayerState.Startup)
		{
			if(tutorialManager.tutorialState == TutorialState.Movement)
			{
				if (movement.magnitude != 0)
					movingTime += Time.deltaTime;
			}

			if(tutorialManager.tutorialState == TutorialState.AttractRepelStep)
			{
				if (playerState == PlayerState.Attracting)
					attractTime += Time.deltaTime;

				if (playerState == PlayerState.Repulsing)
					repelTime += Time.deltaTime;
			}
		}
	}
	protected override void FixedUpdate ()
	{
		if (rewiredPlayer == null)
			return;

		if (playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing && playerState != PlayerState.Startup)
		{
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

				OnHoldingVoid ();
			}

			//Deceleration
			playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x * decelerationAmount, playerRigidbody.velocity.y, playerRigidbody.velocity.z * decelerationAmount);

			//Gravity
			playerRigidbody.AddForce(-Vector3.up * gravity, ForceMode.Acceleration);

			if((tutorialManager.tutorialState & TutorialState.AttractRepel) == TutorialState.AttractRepel)
			{
				if (transform.GetChild (1).gameObject.activeSelf == false)
					transform.GetChild (1).gameObject.SetActive (true);

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
			else
			{
				if (transform.GetChild (1).gameObject.activeSelf == true)
					transform.GetChild (1).gameObject.SetActive (false);
				
				cubesAttracted.Clear ();
				cubesRepulsed.Clear ();
			}
		}
	}

	protected override void OnCollisionStay(Collision other)
	{
		if(playerState == PlayerState.Startup || rewiredPlayer == null)
			return;

		if(other.gameObject.tag == "DeadZone" && gameObject.layer != LayerMask.NameToLayer ("Safe"))
		if (playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
			Death(DeathFX.All, other.contacts[0].point);

		if (other.collider.tag != "HoldMovable")
		if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<PlayersGameplay>().playerState != PlayerState.Stunned && dashState == DashState.Dashing && !playersHit.Contains(other.gameObject))
		{
			if ((tutorialManager.tutorialState & TutorialState.DashHit) != TutorialState.DashHit)
				return;
			
			playersHit.Add(other.gameObject);
			other.gameObject.GetComponent<PlayersGameplay>().StunVoid(false);

			mainCamera.GetComponent<ScreenShakeCamera>().CameraShaking(FeedbackType.DashStun);
			mainCamera.GetComponent<ZoomCamera>().Zoom(FeedbackType.DashStun);

		}
	}

	protected override void OnCollisionEnter(Collision other)
	{
		if(playerState == PlayerState.Startup || rewiredPlayer == null)
			return;

		if(other.gameObject.tag == "DeadZone" && gameObject.layer != LayerMask.NameToLayer ("Safe"))
		if (playerState != PlayerState.Dead && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
			Death(DeathFX.All, other.contacts[0].point);

		if (other.collider.tag != "HoldMovable" && other.gameObject.tag == "Player")
		if (other.gameObject.GetComponent<PlayersGameplay>().playerState != PlayerState.Stunned && dashState == DashState.Dashing && !playersHit.Contains(other.gameObject))
		{
			if ((tutorialManager.tutorialState & TutorialState.DashHit) != TutorialState.DashHit)
				return;

			playersHit.Add(other.gameObject);
			other.gameObject.GetComponent<PlayersGameplay>().StunVoid(false);

			mainCamera.GetComponent<ScreenShakeCamera>().CameraShaking(FeedbackType.DashStun);
			mainCamera.GetComponent<ZoomCamera>().Zoom(FeedbackType.DashStun);
		}
	}

	public override void OnHoldMovable (GameObject movable)
	{
		if((tutorialManager.tutorialState & TutorialState.Shoot) == TutorialState.Shoot)
			base.OnHoldMovable (movable);
	}

	protected override IEnumerator DeathCoroutine ()
	{
		playerState = PlayerState.Dead;

		GameAnalytics.NewDesignEvent("Player:" + name + ":" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":LifeDuration", (int)(Time.unscaledTime - startModeTime));
		mainCamera.GetComponent<ScreenShakeCamera>().CameraShaking(FeedbackType.Death);
		mainCamera.GetComponent<ZoomCamera>().Zoom(FeedbackType.Death);

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

		FindObjectOfType<TutorialManager> () .PlayerDeath (playerName, gameObject);

//		GlobalMethods.Instance.SpawnExistingPlayerRandomVoid (gameObject, 1f, true);
	}
}
