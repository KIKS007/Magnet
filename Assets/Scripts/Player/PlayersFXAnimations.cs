using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayersFXAnimations : MonoBehaviour 
{
	[Header ("Trail FX Settings")]
	public float trailTweenDuration = 0.5f;
	public float lowSpeedtime = 0.1f;
	public float lowSpeedstartWidth = 1;
	public float highSpeedtime = 0.1f;
	public float highSpeedstartWidth = 1;
	public float dashingTime = 0.5f;
	public float dashingStartWidth = 2;

	[Header ("Shoot FX Settings")]
	public Vector3 shootPosOffset;

	public ParticleSystem dashAvailableFX;

	private PlayersGameplay playerScript;

	private TrailRenderer trail;

	private int PlayerNumber = -1;

	// Use this for initialization
	void Start () 
	{
		playerScript = GetComponent<PlayersGameplay> ();
		trail = transform.GetChild (4).GetComponent<TrailRenderer>();

		playerScript.OnShoot += ShootFX;
		playerScript.OnDashAvailable += DashAvailableFX;
		playerScript.OnDash += StopDashAvailable;

		switch(gameObject.name)
		{
		case "Player 1":
			PlayerNumber = 0;
			break;
		case "Player 2":
			PlayerNumber = 1;
			break;
		case "Player 3":
			PlayerNumber = 2;
			break;
		case "Player 4":
			PlayerNumber = 3;
			break;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//TrailLength ();
	}

	void TrailLength ()
	{
		if(playerScript.playerState != PlayerState.Dead && playerScript.dashState != DashState.Dashing && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			if(playerScript.playerRigidbody.velocity.magnitude > 1 && !DOTween.IsTweening("Trail"))
			{
				Debug.Log ("High");

				DOTween.To(()=> trail.time, x=> trail.time =x, highSpeedtime, trailTweenDuration).SetId("Trail");
				DOTween.To(()=> trail.startWidth, x=> trail.startWidth =x, highSpeedstartWidth, trailTweenDuration).SetId("Trail");

			}
			else if (playerScript.playerRigidbody.velocity.magnitude < 1 && !DOTween.IsTweening("Trail"))
			{
				Debug.Log ("Low");

				DOTween.To(()=> trail.time, x=> trail.time =x, lowSpeedtime, trailTweenDuration).SetId("Trail");
				DOTween.To(()=> trail.startWidth, x=> trail.startWidth =x, lowSpeedstartWidth, trailTweenDuration).SetId("Trail");
			}
		}

		else if(playerScript.playerState != PlayerState.Dead && playerScript.dashState == DashState.Dashing && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			Debug.Log ("Dash");

			trail.time = dashingTime;
			trail.startWidth = dashingStartWidth;
		}

		else
		{
			trail.time = 0f;
			trail.startWidth = 0f;
		}
	}

	void ShootFX ()
	{
		Instantiate (GlobalVariables.Instance.shootFX [PlayerNumber], transform.position + shootPosOffset, transform.rotation);
	}

	void DashAvailableFX ()
	{
		dashAvailableFX.Play ();
	}

	void StopDashAvailable ()
	{
		if (dashAvailableFX.isPlaying)
			dashAvailableFX.Stop ();
	}
}
