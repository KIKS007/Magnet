using UnityEngine;
using System.Collections;

public class PlayersBomb : PlayersGameplay 
{
	public void GetBomb (Collider other)
	{
		if(playerState != PlayerState.Holding && playerState != PlayerState.Dead)
		{
			transform.GetChild (2).GetComponent<MagnetTriggerScript> ().GetMovable (other);
		}
	}

	public void DeathParticles ()
	{
		Quaternion rot = Quaternion.FromToRotation(Vector3.forward, Vector3.up);
		GameObject instantiatedParticles = Instantiate(GlobalVariables.Instance.DeadParticles, transform.position, rot) as GameObject;

		instantiatedParticles.transform.SetParent (GlobalVariables.Instance.ParticulesClonesParent);
		instantiatedParticles.GetComponent<ParticleSystemRenderer>().material.color = GetComponent<Renderer> ().material.color;	
	}
}
