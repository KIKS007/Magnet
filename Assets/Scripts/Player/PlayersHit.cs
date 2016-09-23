using UnityEngine;
using System.Collections;

public class PlayersHit : PlayersGameplay 
{
	[Header ("Hit")]
	public LayerMask checkSphereLayer;

	private float timeBetweenSpawn;

	protected override void Start ()
	{
		base.Start ();

		timeBetweenSpawn = GameObject.Find ("HitModeManager").GetComponent<HitModeManager> ().timeBetweenSpawn;
	}

	public void HitVoid (Collision other)
	{
		DeathParticles (other);

		Hit ();
	}

	void DeathParticles (Collision other)
	{
		Vector3 pos = other.contacts[0].point;
		//Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
		Quaternion rot = Quaternion.FromToRotation(Vector3.forward, new Vector3(0, 0, 0));

		GameObject instantiatedParticles = Instantiate(GlobalVariables.Instance.DeadParticles, pos, rot) as GameObject;
		instantiatedParticles.transform.SetParent (GlobalVariables.Instance.ParticulesClonesParent);
		instantiatedParticles.transform.position = new Vector3(instantiatedParticles.transform.position.x, 2f, instantiatedParticles.transform.position.z);
		instantiatedParticles.transform.LookAt(new Vector3(0, 0, 0));
		instantiatedParticles.GetComponent<Renderer>().material.color = gameObject.GetComponent<Renderer>().material.color;
	}

	void Hit ()
	{
		if(playerState == PlayerState.Holding)
		{
			Shoot ();
		}

		playerState = PlayerState.Dead;

		OnDeathVoid ();

		GlobalMethods.Instance.SpawnExistingPlayerRandomVoid (gameObject, timeBetweenSpawn);

		playerState = PlayerState.None;
		speed = originalSpeed;
	}
}
