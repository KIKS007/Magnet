using UnityEngine;
using System.Collections;

public class PlayersHit : PlayersGameplay 
{
	[Header ("Hit")]
	public LayerMask checkSphereLayer;

	public void HitVoid (Collision other)
	{
		DeathParticles (other);

		StartCoroutine (Hit ());
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

	void SpawnParticles ()
	{
		GameObject instantiatedParticles = Instantiate(GlobalVariables.Instance.PlayerSpawnParticles, transform.position, GlobalVariables.Instance.PlayerSpawnParticles.transform.rotation) as GameObject;

		instantiatedParticles.transform.SetParent (GlobalVariables.Instance.ParticulesClonesParent);
		//instantiatedParticles.transform.position = new Vector3(instantiatedParticles.transform.position.x, 2f, instantiatedParticles.transform.position.z);
		instantiatedParticles.GetComponent<Renderer>().material.color = gameObject.GetComponent<Renderer>().material.color;
	}

	IEnumerator Hit ()
	{
		if(playerState == PlayerState.Holding)
		{
			Shoot ();
		}

		playerState = PlayerState.Dead;

		OnDeathVoid ();

		DeactivatePlayer ();

		yield return new WaitForSeconds(GlobalVariables.Instance.timeBetweenSpawn);

		Vector3 newPos = new Vector3();

		do
		{
			newPos = new Vector3 (Random.Range (-24, 24 + 1), transform.position.y, Random.Range (-14, 14 + 1));
		}
		while(Physics.CheckSphere(newPos, 3, checkSphereLayer));

		transform.position = newPos;

		playerState = PlayerState.None;
		speed = originalSpeed;

		ActivatePlayer ();

		SpawnParticles ();
	}

	void ActivatePlayer ()
	{
		GetComponent<MeshRenderer> ().enabled = true;
		GetComponent<Collider> ().enabled = true;
		GetComponent<Rigidbody> ().useGravity = true;

		GetComponent<Rigidbody> ().velocity = Vector3.zero;
		GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;

		for(int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild (i).gameObject.SetActive (true);
		}
	}

	void DeactivatePlayer ()
	{
		GetComponent<MeshRenderer> ().enabled = false;
		GetComponent<Collider> ().enabled = false;
		GetComponent<Rigidbody> ().useGravity = false;

		GetComponent<Rigidbody> ().velocity = Vector3.zero;
		GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;

		for(int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild (i).gameObject.SetActive (false);
		}
	}
}
