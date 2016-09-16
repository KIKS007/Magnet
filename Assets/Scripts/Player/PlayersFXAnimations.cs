using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

	[Header ("Dash Available FX")]
	public ParticleSystem dashAvailableFX;

	[Header ("Attraction FX")]


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

	public List<GameObject> attractionRepulsionFX = new List<GameObject> ();

	public IEnumerator AttractionFX (GameObject whichCube)
	{
		GameObject fx = Instantiate (GlobalVariables.Instance.attractFX [PlayerNumber], whichCube.transform.position, transform.rotation) as GameObject;
		attractionRepulsionFX.Add (fx);
		ParticleSystem ps = fx.GetComponent<ParticleSystem> ();
		fx.transform.SetParent (GlobalVariables.Instance.ParticulesClonesParent);
		ps.startSize = 2 + whichCube.transform.lossyScale.x;

		StartCoroutine (SetAttractionParticles (whichCube, fx, ps));

		yield return new WaitWhile(() => playerScript.cubesAttracted.Contains (whichCube));

		ps.Stop ();

		yield return new WaitWhile(() => ps.IsAlive());

		attractionRepulsionFX.Remove (fx);

		Destroy (fx);
	}

	IEnumerator SetAttractionParticles (GameObject whichCube, GameObject fx, ParticleSystem ps)
	{
		while(ps.IsAlive())
		{
			fx.transform.position = whichCube.transform.position;

			Vector3 lookPos = new Vector3 (transform.position.x, fx.transform.position.y, transform.position.z);
			fx.transform.LookAt(lookPos);

			float dist = Vector3.Distance (transform.position, whichCube.transform.position);
			float lifeTime = 0.12222222222222f * dist - 0.25555555555556f;

			ps.startLifetime = lifeTime;

			ParticleSystem.Particle[] particlesList = new ParticleSystem.Particle[ps.particleCount];
			ps.GetParticles (particlesList);

			for (int i = 0; i < ps.particleCount; i++)
			{
				dist = Vector3.Distance (transform.position, ps.transform.TransformPoint(particlesList [i].position));
				lifeTime = 0.12222222222222f * dist - 0.25555555555556f;
				particlesList [i].lifetime = lifeTime;
			}

			ps.SetParticles (particlesList, particlesList.Length);

			yield return null;
		}
	}

	public IEnumerator RepulsionFX (GameObject whichCube)
	{
		GameObject fx = Instantiate (GlobalVariables.Instance.repulseFX [PlayerNumber], whichCube.transform.position, transform.rotation) as GameObject;
		attractionRepulsionFX.Add (fx);
		ParticleSystem ps = fx.GetComponent<ParticleSystem> ();
		fx.transform.SetParent (GlobalVariables.Instance.ParticulesClonesParent);
		ps.startSize = 2 + whichCube.transform.lossyScale.x;

		StartCoroutine (SetRepulsionParticles (whichCube, fx, ps));

		yield return new WaitWhile(() => playerScript.cubesRepulsed.Contains (whichCube));

		ps.Stop ();

		ParticleSystem.Particle[] particlesList = new ParticleSystem.Particle[ps.particleCount];
		ps.GetParticles (particlesList);

		for (int i = 0; i < ps.particleCount; i++)
		{
			particlesList [i].lifetime = -1f;
			yield return null;
		}

		ps.SetParticles (particlesList, particlesList.Length);

		yield return new WaitWhile(() => ps.IsAlive());

		attractionRepulsionFX.Remove (fx);

		Destroy (fx);
	}

	IEnumerator SetRepulsionParticles (GameObject whichCube, GameObject fx, ParticleSystem ps)
	{
		while(ps.IsAlive())
		{
			fx.transform.position = transform.position;

			Vector3 lookPos = new Vector3 (whichCube.transform.position.x, fx.transform.position.y, whichCube.transform.position.z);
			fx.transform.LookAt(lookPos);

			float dist = Vector3.Distance (transform.position, whichCube.transform.position);
			float lifeTime = 0.12222222222222f * dist - 0.25555555555556f;

			ps.startLifetime = lifeTime;

			ParticleSystem.Particle[] particlesList = new ParticleSystem.Particle[ps.particleCount];
			ps.GetParticles (particlesList);

			for (int i = 0; i < ps.particleCount; i++)
			{
				dist = Vector3.Distance (whichCube.transform.position, ps.transform.TransformPoint(particlesList [i].position));
				lifeTime = 0.12222222222222f * dist - 0.25555555555556f;
				particlesList [i].lifetime = lifeTime;
			}

			ps.SetParticles (particlesList, particlesList.Length);

			yield return null;
		}
	}
}
