using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovableStar : MovableScript 
{
	[Header ("Trail")]
	public Color trailNormalColor;
	public Color trailDeadlyColor;

	public override void Awake ()
	{
		base.Awake ();

		deadlyParticle2 = transform.GetChild (4).GetComponent<ParticleSystem> ();
		trailDeadlyColor = deadlyParticle2.main.startColor.color;
	}

	public override void OnEnable ()
	{
		hold = false;

		rigidbodyMovable = GetComponent<Rigidbody>();
		movableRenderer = GetComponent<Renderer> ();
		cubeMeshFilter = transform.GetChild (2).GetComponent<MeshFilter> ();
		cubeMaterial = transform.GetChild (1).GetComponent<Renderer> ().material;
		deadlyParticle = transform.GetChild (3).GetComponent<ParticleSystem> ();
		deadlyParticle2 = transform.GetChild (4).GetComponent<ParticleSystem> ();

		slowMoTrigger = transform.GetComponentInChildren<SlowMotionTriggerScript> ();

		deadlyParticle.Stop ();
		//deadlyParticle2.Stop ();

		var main = deadlyParticle2.main;
		main.startColor = trailNormalColor;

		cubeMaterial.DOColor (trailNormalColor, "_EmissionNEUTRAL", 0.1f).SetId("CubeColorTween" + gameObject.GetInstanceID ()).SetUpdate (false);

		cubeMeshFilter.mesh = GlobalVariables.Instance.cubesStripes [Random.Range (0, GlobalVariables.Instance.cubesStripes.Length)];
		attracedBy.Clear ();
		repulsedBy.Clear ();
	}

	protected override void LowVelocity () 
	{
		if(hold == false && currentVelocity > 0)
		{
			if(currentVelocity > higherVelocity)
				higherVelocity = currentVelocity;

			else if(currentVelocity < limitVelocity)
			{
				if(gameObject.tag == "DeadCube" || gameObject.tag == "ThrownMovable")
				{
					ToNeutralColor ();

					slowMoTrigger.triggerEnabled = false;
					gameObject.tag = "Movable";
				}
			}
		}
	}

	protected override void HitPlayer (Collision other)
	{
		if(other.collider.tag == "Player")
		{
			PlayersGameplay playerScript = other.collider.GetComponent<PlayersGameplay> ();

			if (playerScript.playerState == PlayerState.Stunned)
				return;

			if(tag == "ThrownMovable")
			{
				if(playerThatThrew == null || other.gameObject.name != playerThatThrew.name)
				{
					other.gameObject.GetComponent<PlayersGameplay>().StunVoid(true);

					InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, GlobalVariables.Instance.playersColors [(int)playerScript.playerName]);	

					if(playerThatThrew != null)
						StatsManager.Instance.PlayersHits (playerThatThrew, other.gameObject);
				}				
			}
		}

		if(other.collider.tag == "Player" && tag == "DeadCube")
		{
			PlayersGameplay playerScript = other.collider.GetComponent<PlayersGameplay> ();

			if (playerScript.playerState == PlayerState.Dead)
				return;

			playerScript.Death (DeathFX.All, other.contacts [0].point, playerThatThrew);

			if (playerThatThrew != null)
				StatsManager.Instance.PlayersHits (playerThatThrew, other.gameObject);

			InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, GlobalVariables.Instance.playersColors [(int)playerScript.playerName]);

			GlobalMethods.Instance.Explosion (transform.position);
		}
	}

	protected override void HitWall (Collision other)
	{
		if(other.gameObject.tag == "Wall" && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			/*if(currentVelocity > (limitVelocity * 0.5f))
				InstantiateImpactFX (other.contacts [0]);*/

			if(canPlaySound)
				StartCoroutine(HitSound ());

			DeadlyTransition ();
		}
	}

	public override void ToColor (GameObject otherPlayer = null, float overrideDuration = toColorDuration)
	{
		return;
	}

	public override void ToNeutralColor (float overrideDuration = toNeutralDuration)
	{
		if(!hold)
		{
			if(deadlyParticle == null)
				deadlyParticle = transform.GetChild (3).GetComponent<ParticleSystem> ();

			if(deadlyParticle2 == null)
				deadlyParticle2 = transform.GetChild (4).GetComponent<ParticleSystem> ();

			deadlyParticle.Stop ();
			//deadlyParticle2.main.startColor.color = trailNormalColor;

			var main = deadlyParticle2.main;
			main.startColor = trailNormalColor;

			//deadlyParticle2.Stop ();

			DisableAllColor (overrideDuration);

			StartCoroutine (WaitToChangeColorEnum (CubeColor.Neutral, overrideDuration));
		}
	}

	public override void ToDeadlyColor (float overrideDuration = toColorDuration)
	{
		DisableAllColor (overrideDuration);

		if(deadlyParticle == null)
			deadlyParticle = transform.GetChild (3).GetComponent<ParticleSystem> ();

		if(deadlyParticle2 == null)
			deadlyParticle2 = transform.GetChild (4).GetComponent<ParticleSystem> ();

		deadlyParticle.Play ();
		//deadlyParticle2.Play ();

		var main = deadlyParticle2.main;
		main.startColor = trailDeadlyColor;

		cubeMaterial.DOFloat (1f, "_LerpRED", overrideDuration).SetId("CubeColorTween" + gameObject.GetInstanceID ()).SetUpdate (false);

		cubeColor = CubeColor.Deadly;
	}

	void DeadlyTransition ()
	{
		ToDeadlyColor (0.1f);

		tag = "DeadCube";
	}

	public override void OnRelease ()
	{
		ToNeutralColor();

		OnReleaseEventVoid ();
	}
}
