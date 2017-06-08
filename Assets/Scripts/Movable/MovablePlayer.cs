using UnityEngine;
using System.Collections;
using DG.Tweening;
using DarkTonic.MasterAudio;

public class MovablePlayer : MovableScript 
{
	[HideInInspector]
	public bool basicMovable = true;

	public override void Start ()
	{
		
	}

	public override void OnEnable ()
	{
		hold = false;

		rigidbodyMovable = GetComponent<Rigidbody>();
		movableRenderer = GetComponent<Renderer> ();
		cubeMeshFilter = transform.GetChild (2).GetComponent<MeshFilter> ();
		cubeMaterial = transform.GetChild (1).GetComponent<Renderer> ().material;
		deadlyParticle = transform.GetChild (3).GetComponent<ParticleSystem> ();
		slowMoTrigger = transform.GetComponentInChildren<SlowMotionTriggerScript> ();

		deadlyParticle.Stop ();
		//cubeMeshFilter.mesh = GlobalVariables.Instance.deadCubesMeshFilter;
		cubeMeshFilter.mesh = GlobalVariables.Instance.cubesStripes [Random.Range (0, GlobalVariables.Instance.cubesStripes.Length)];
		attracedBy.Clear ();
		repulsedBy.Clear ();
	}

	public void CubeColor (string tag)
	{
		if (tag == "Suggestible" || tag == "DeadCube")
			ToDeadlyColor ();
		else
			ToNeutralColor ();
	}

	protected override void LowVelocity ()
	{
		if(hold == false && currentVelocity > 0)
		{
			if(currentVelocity > higherVelocity)
				higherVelocity = currentVelocity;
		}	

		if(basicMovable)
		{
			if(hold == false && currentVelocity > 0)
			{
				if(currentVelocity > limitVelocity)
				{
					if(slowMoTrigger == null)
						slowMoTrigger = transform.GetComponentInChildren<SlowMotionTriggerScript> ();

					slowMoTrigger.triggerEnabled = true;
				}
			}

			if(currentVelocity < limitVelocity)
			{
				if(gameObject.tag == "ThrownMovable")
				{
					if(slowMoTrigger == null)
						slowMoTrigger = transform.GetComponentInChildren<SlowMotionTriggerScript> ();

					slowMoTrigger.triggerEnabled = false;

					gameObject.tag = "Movable";
				}
			}
		}
	}

	protected override void HitPlayer (Collision other)
	{
		if(tag != "Suggestible" && tag != "DeadCube")
			base.HitPlayer (other);

		else
		{
			if(other.collider.tag == "Player" 
				&& other.collider.GetComponent<PlayersGameplay>().playerState != PlayerState.Dead)
			{
				other.collider.GetComponent<PlayersGameplay> ().Death (DeathFX.All, other.contacts [0].point);

				InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);

				GlobalMethods.Instance.Explosion (transform.position);
			}
		}
	}
}
