#pragma warning disable 0618

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

	[Header ("Stun FX Settings")]
	public MeshRenderer[] playerMaterials = new MeshRenderer[0];
	public float[] stunFXDurations = new float[6];

	[Header ("Dash FX")]
	public ParticleSystem dashFX;

	[Header ("Dash Available FX")]
	public ParticleSystem dashAvailableFX;

	[Header ("Attraction FX")]


	private PlayersGameplay playerScript;
	private PlayersSounds playerSoundsScript;

	private TrailRenderer trail;

	private int playerNumber = -1;

	// Use this for initialization
	void Start () 
	{
		playerScript = GetComponent<PlayersGameplay> ();
		playerSoundsScript = GetComponent<PlayersSounds> ();
		trail = transform.GetChild (4).GetComponent<TrailRenderer>();

		playerScript.OnShoot += ShootFX;
		playerScript.OnDashAvailable += DashAvailableFX;
		playerScript.OnDash += StopDashAvailable;
		playerScript.OnStun += ()=> StartCoroutine (StunFX ());
		playerScript.OnDash += EnableDashFX;

		playerNumber = (int)playerScript.playerName;
	}

	void OnEnable ()
	{
		for (int i = 0; i < playerMaterials.Length; i++)
			if(playerMaterials[i] != null)
				playerMaterials [i].material.EnableKeyword ("_EMISSION");
	}
	
	// Update is called once per frame
	void Update () 
	{
		//TrailLength ();

		if(dashAvailableFX.isPlaying)
		{
			ParticleSystem.Particle[] particlesList = new ParticleSystem.Particle[dashAvailableFX.particleCount];
			dashAvailableFX.GetParticles (particlesList);

			for (int i = 0; i < particlesList.Length; i++)
				particlesList [i].rotation = transform.rotation.eulerAngles.y;

			dashAvailableFX.SetParticles (particlesList, particlesList.Length);
			dashAvailableFX.startRotation = transform.rotation.eulerAngles.y;
		}

		if (dashFX != null && playerScript.dashState != DashState.Dashing)
			DisableDashFX ();		
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

	void EnableDashFX ()
	{
		if(dashFX != null)
			dashFX.Play ();
	}

	void DisableDashFX ()
	{
		if(dashFX != null)
			dashFX.Stop ();
	}

	void ShootFX ()
	{
		Instantiate (GlobalVariables.Instance.shootFX [playerNumber], transform.position + shootPosOffset, transform.rotation);
	}

	IEnumerator StunFX ()
	{
		float[] stunFXDurationsTemp = stunFXDurations;
		stunFXDurationsTemp [0] = stunFXDurations [0] + Random.Range (-0.005f, 0.005f);
		stunFXDurationsTemp [1] = stunFXDurationsTemp [0];

		stunFXDurationsTemp [2] = stunFXDurations [2] + Random.Range (-0.005f, 0.005f);
		stunFXDurationsTemp [3] = stunFXDurationsTemp [2];

		stunFXDurationsTemp [4] = stunFXDurations [4] + Random.Range (-0.005f, 0.005f);
		stunFXDurationsTemp [5] = stunFXDurations [5] + Random.Range (-0.005f, 0.005f);

		for (int i = 0; i < playerMaterials.Length; i++)
			playerMaterials [i].material.DisableKeyword ("_EMISSION");

		playerSoundsScript.StunOFF ();

		yield return new WaitForSeconds (stunFXDurationsTemp[0]);

		for (int i = 0; i < playerMaterials.Length; i++)
			playerMaterials [i].material.EnableKeyword ("_EMISSION");

		playerSoundsScript.StunON ();

		yield return new WaitForSeconds (stunFXDurationsTemp[1]);

		for (int i = 0; i < playerMaterials.Length; i++)
			playerMaterials [i].material.DisableKeyword ("_EMISSION");

		playerSoundsScript.StunOFF ();

		yield return new WaitForSeconds (stunFXDurationsTemp[2]);

		for (int i = 0; i < playerMaterials.Length; i++)
			playerMaterials [i].material.EnableKeyword ("_EMISSION");

		playerSoundsScript.StunON ();

		yield return new WaitForSeconds (stunFXDurationsTemp[3]);

		for (int i = 0; i < playerMaterials.Length; i++)
			playerMaterials [i].material.DisableKeyword ("_EMISSION");

		playerSoundsScript.StunOFF ();

		yield return new WaitForSeconds (stunFXDurationsTemp[4]);

		for (int i = 0; i < playerMaterials.Length; i++)
			playerMaterials [i].material.EnableKeyword ("_EMISSION");

		playerSoundsScript.StunON ();

		yield return new WaitForSeconds (stunFXDurationsTemp[5]);

		for (int i = 0; i < playerMaterials.Length; i++)
			playerMaterials [i].material.DisableKeyword ("_EMISSION");

		playerSoundsScript.StunOFF ();

		yield return new WaitUntil(()=> playerScript.playerState != PlayerState.Stunned);

		for (int i = 0; i < playerMaterials.Length; i++)
			playerMaterials [i].material.EnableKeyword ("_EMISSION");

		playerSoundsScript.StunEND ();
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
		GameObject fx = Instantiate (GlobalVariables.Instance.attractFX [playerNumber], whichCube.transform.position, transform.rotation) as GameObject;
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
				particlesList [i].startLifetime = lifeTime;
			}

			ps.SetParticles (particlesList, particlesList.Length);

			yield return null;
		}
	}

	public IEnumerator RepulsionFX (GameObject whichCube)
	{
		GameObject fx = Instantiate (GlobalVariables.Instance.repulseFX [playerNumber], whichCube.transform.position, transform.rotation) as GameObject;
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
			particlesList [i].startLifetime = -1f;
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
				particlesList [i].startLifetime = lifeTime;
			}

			ps.SetParticles (particlesList, particlesList.Length);

			yield return null;
		}
	}
}
