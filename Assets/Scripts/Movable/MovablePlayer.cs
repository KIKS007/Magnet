using UnityEngine;
using System.Collections;
using DG.Tweening;
using DarkTonic.MasterAudio;

public class MovablePlayer : MovableScript 
{
	[Header ("Explosion")]
	public float explosionForce = 20;
	public float explosionRadius = 50;

	[HideInInspector]
	public bool basicMovable = true;

	protected override void Start ()
	{
		
	}

	protected override void OnEnable ()
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

	protected override void Update ()
	{
		if(hold == false && rigidbodyMovable != null)
			currentVelocity = rigidbodyMovable.velocity.magnitude;

		if(hold == false && currentVelocity > 0)
		{
			if(currentVelocity > higherVelocity)
				higherVelocity = currentVelocity;
		}	

		if(basicMovable)
		{
			if(hold == false && currentVelocity > 0)
			{
				if(currentVelocity >= limitVelocity)
					gameObject.tag = "ThrownMovable";
				
				else if(currentVelocity < limitVelocity && gameObject.tag == "ThrownMovable")
				{
					slowMoTrigger.triggerEnabled = false;
					gameObject.tag = "Movable";
					playerThatThrew = null;
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

				GlobalMethods.Instance.Explosion (transform.position, explosionForce, explosionRadius);
			}
		}
	}

	public override void OnHold ()
	{
		hold = true;

		attracedBy.Clear ();
		repulsedBy.Clear ();

		ToColor();

		OnHoldEventVoid ();
	}

	protected override IEnumerator WaitToChangeColorEnum (CubeColor whichColor, float waitTime)
	{
		yield return new WaitForSeconds (waitTime * 0.5f);		

		if(hold)
			cubeColor = whichColor;

	}
}
