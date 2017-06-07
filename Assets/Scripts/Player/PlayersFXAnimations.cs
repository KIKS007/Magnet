#pragma warning disable 0618

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DarkTonic.MasterAudio;

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

	[Header ("Player Mesh")]
	public Transform playerMesh;
	public float leanSpeed;
	public float leanLerp = 0.1f;
	public float leanMaxAngle;

	[Header ("Safe FX")]
	public float safeDurationBetween = 0.5f;

	private PlayersGameplay playerScript;
	private PlayersSounds playerSoundsScript;
	private PlayerName playerName;

	private int playerNumber = -1;

	private float spawnDuration = 0.2f;
	private Vector3 initialScale;

	private Color playerColor;

	[HideInInspector]
	public float distance;

	[HideInInspector]
	public List<GameObject> attractionRepulsionFX = new List<GameObject> ();

	void Awake ()
	{
		initialScale = transform.localScale;
	}

	// Use this for initialization
	void Start () 
	{
		playerScript = GetComponent<PlayersGameplay> ();
		playerSoundsScript = GetComponent<PlayersSounds> ();
		playerColor = GetComponent <Renderer> ().material.color;

		playerScript.OnShoot += ShootFX;
		playerScript.OnDashAvailable += DashAvailableFX;
		playerScript.OnDash += StopDashAvailable;
		playerScript.OnStun += ()=> StartCoroutine (StunFX ());
		playerScript.OnDash += EnableDashFX;
		playerScript.OnDeath += RemoveAttractionRepulsionFX;
		playerScript.OnSafe += () => StartCoroutine (SafeFX ());

		playerName = playerScript.playerName;
		playerNumber = (int)playerScript.playerName;
	}

	void OnEnable ()
	{
		transform.localScale = Vector3.zero;
		transform.DOScale (initialScale, spawnDuration).SetEase (Ease.InBack);

		for (int i = 0; i < playerMaterials.Length; i++)
			if(playerMaterials[i] != null)
				playerMaterials [i].material.EnableKeyword ("_EMISSION");
	}
	
	// Update is called once per frame
	void Update () 
	{
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

		LeanMesh ();
	}

	void LeanMesh ()
	{
		Vector3 movementDirection = transform.InverseTransformDirection (playerScript.movement);
		Vector3 newRotation = new Vector3 ();

		if (movementDirection.z > 0.5f || movementDirection.z < -0.5f)
			newRotation.x = movementDirection.z * leanSpeed;
		else
			newRotation.x = 0;

		if (movementDirection.x > 0.5f || movementDirection.x < -0.5f)
			newRotation.z = -movementDirection.x * leanSpeed;
		else
			newRotation.z = 0;

		if (playerScript.holdState == HoldState.Holding)
			newRotation = Vector3.zero;

		playerMesh.localRotation = Quaternion.Lerp (playerMesh.localRotation, Quaternion.Euler (newRotation), leanLerp);

		playerMesh.localEulerAngles = new Vector3 (ClampAngle (playerMesh.localEulerAngles.x, -leanMaxAngle, leanMaxAngle), playerMesh.localEulerAngles.y, ClampAngle (playerMesh.localEulerAngles.z, -leanMaxAngle, leanMaxAngle));
	}

	float ClampAngle(float angle, float min, float max) 
	{
		if(angle < 90 || angle > 270)
		{
			if (angle > 180)
				angle -= 360;

			if (max > 180)
				max -= 360;

			if (min > 180)
				min -= 360;
		}

		angle = Mathf.Clamp(angle, min, max);

		if(angle < 0) 
			angle += 360;

		return angle;
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

	IEnumerator SafeFX ()
	{
		while(gameObject.layer == LayerMask.NameToLayer ("Safe"))
		{
			for (int i = 0; i < playerMaterials.Length; i++)
				playerMaterials [i].material.DisableKeyword ("_EMISSION");

			playerSoundsScript.StunOFF ();

			if (gameObject.layer != LayerMask.NameToLayer ("Safe"))
				break;
			
			yield return new WaitForSeconds (safeDurationBetween);

			for (int i = 0; i < playerMaterials.Length; i++)
				playerMaterials [i].material.EnableKeyword ("_EMISSION");

			playerSoundsScript.StunON ();

			yield return new WaitForSeconds (safeDurationBetween);
		}

		for (int i = 0; i < playerMaterials.Length; i++)
			playerMaterials [i].material.EnableKeyword ("_EMISSION");

		playerSoundsScript.StunON ();
	}
		
	public IEnumerator AttractionFX (GameObject whichCube)
	{
		GameObject fx = Instantiate (GlobalVariables.Instance.attractFX [playerNumber], whichCube.transform.position, transform.rotation) as GameObject;
		attractionRepulsionFX.Add (fx);
		ParticleSystem ps = fx.GetComponent<ParticleSystem> ();
		fx.transform.SetParent (GlobalVariables.Instance.ParticulesClonesParent);

		ps.startSize = 3 + whichCube.transform.lossyScale.x;

		StartCoroutine (SetAttractionParticles (whichCube, fx, ps));

		MovableScript script = whichCube.GetComponent<MovableScript> ();

		yield return new WaitWhile(() => playerScript.cubesAttracted.Contains (script));

		ps.Stop ();

		yield return new WaitWhile(() => ps.IsAlive());

		attractionRepulsionFX.Remove (fx);

		Destroy (fx);
	}

	IEnumerator SetAttractionParticles (GameObject whichCube, GameObject fx, ParticleSystem ps)
	{
		while(ps != null && ps.IsAlive())
		{
			fx.transform.position = whichCube.transform.position;

			Vector3 lookPos = new Vector3 (transform.position.x, fx.transform.position.y, transform.position.z);
			fx.transform.LookAt(lookPos);

			float dist = Vector3.Distance (transform.position, whichCube.transform.position);
			float lifeTime = 0.027491408934708f * dist -0.036082474226804f;

			ps.startLifetime = lifeTime;

			ParticleSystem.Particle[] particlesList = new ParticleSystem.Particle[ps.particleCount];
			ps.GetParticles (particlesList);

			for (int i = 0; i < ps.particleCount; i++)
			{
				dist = Vector3.Distance (transform.position, ps.transform.TransformPoint(particlesList [i].position));
				lifeTime = 0.027491408934708f * dist -0.036082474226804f;
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

		ps.startSize = 3 + whichCube.transform.lossyScale.x;

		StartCoroutine (SetRepulsionParticles (whichCube, fx, ps));

		MovableScript script = whichCube.GetComponent<MovableScript> ();

		yield return new WaitWhile(() => playerScript.cubesRepulsed.Contains (script));

		/*ParticleSystem.Particle[] particlesList = new ParticleSystem.Particle[ps.particleCount];
		ps.GetParticles (particlesList);

		for (int i = 0; i < ps.particleCount; i++)
		{
			particlesList [i].startLifetime = -1f;
			yield return null;
		}

		ps.SetParticles (particlesList, particlesList.Length);*/

		ps.Stop ();

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

			distance = Vector3.Distance (transform.position, whichCube.transform.position);

			float dist = Vector3.Distance (transform.position, whichCube.transform.position);
			float lifeTime = 0.027491408934708f * dist -0.036082474226804f;

			ps.startLifetime = lifeTime;

			ParticleSystem.Particle[] particlesList = new ParticleSystem.Particle[ps.particleCount];
			ps.GetParticles (particlesList);

			for (int i = 0; i < ps.particleCount; i++)
			{
				dist = Vector3.Distance (whichCube.transform.position, ps.transform.TransformPoint(particlesList [i].position));
				lifeTime =  0.027491408934708f * dist -0.036082474226804f;
				particlesList [i].startLifetime = lifeTime;
			}

			ps.SetParticles (particlesList, particlesList.Length);

			yield return null;
		}
	}

	void RemoveAttractionRepulsionFX ()
	{
		for (int i = 0; i < attractionRepulsionFX.Count; i++)
		{
			Destroy(attractionRepulsionFX[i]);
		}
	}

	public virtual void DeathExplosionFX(Vector3 position)
	{
		int playerNumber = (int)playerName;

		GameObject instance = Instantiate(GlobalVariables.Instance.explosionFX[playerNumber], position, GlobalVariables.Instance.explosionFX[playerNumber].transform.rotation) as GameObject;
		instance.transform.parent = GlobalVariables.Instance.ParticulesClonesParent.transform;

		MasterAudio.PlaySound3DAtTransformAndForget (SoundsManager.Instance.explosionSound, transform);
	}

	public virtual void DeathExplosionFX()
	{
		int playerNumber = (int)playerName;

		GameObject instance = Instantiate(GlobalVariables.Instance.explosionFX[playerNumber], transform.position, GlobalVariables.Instance.explosionFX[playerNumber].transform.rotation) as GameObject;
		instance.transform.parent = GlobalVariables.Instance.ParticulesClonesParent.transform;

		MasterAudio.PlaySound3DAtTransformAndForget (SoundsManager.Instance.explosionSound, transform);
	}

	public virtual void WaveFX(bool singleWave = false)
	{
		int loopsCount = !singleWave ? (int)playerName + 1 : 1;

		for(int i = 0; i < loopsCount; i++)
		{
			DOVirtual.DelayedCall (GlobalVariables.Instance.delayBetweenWavesFX * i, ()=> 
			{
				int playerNumber = (int)playerName;
				Quaternion rotation = Quaternion.Euler (new Vector3 (90, 0, 0));

				Instantiate(GlobalVariables.Instance.waveFX[playerNumber], transform.position, rotation, transform);
				//instance.transform.parent = GlobalVariables.Instance.ParticulesClonesParent.transform;

				GetComponent<PlayersVibration> ().Wave ();
			});
		}


	}

	public virtual GameObject DeathParticles (Vector3 position)
	{
		Quaternion rot = Quaternion.FromToRotation(Vector3.forward, Vector3.up);
		GameObject instantiatedParticles = Instantiate(GlobalVariables.Instance.DeadParticles, position, rot) as GameObject;

		instantiatedParticles.transform.SetParent (GlobalVariables.Instance.ParticulesClonesParent);
		instantiatedParticles.GetComponent<ParticleSystemRenderer>().material.color = playerColor;

		return instantiatedParticles;
	}
}
