using UnityEngine;
using System.Collections;
using DG.Tweening;
using DarkTonic.MasterAudio;

public class MovablePlague : MovableScript 
{
	[Header ("PLAGUE")]
	public float deadlyCubeTransitionDuration = 0.5f;
	public float deadlyCubeMass = 50;
	public float deadlyCubeMaxVelocity = 2;
	[Range (0, 1)]
	public float deadlyCubeDeceleration = 0.97f;

	public override void Start ()
	{
		base.Start ();

		ToNeutralColor (0);
	}

	protected override void HitPlayer (Collision other)
	{
		if(other.collider.tag == "Player" && tag == "ThrownMovable")
		{
			PlayersGameplay playerScript = other.collider.GetComponent<PlayersGameplay> ();

			if (playerScript.playerState == PlayerState.Stunned)
				return;
			
			if(playerThatThrew == null || other.gameObject.name != playerThatThrew.name)
			{
				StartCoroutine (DeadlyTransition ());
				
				playerScript.StunVoid(true);
				
				InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, GlobalVariables.Instance.playersColors [(int)playerScript.playerName]);	
				
				if(playerThatThrew != null)
					StatsManager.Instance.PlayersHits (playerThatThrew, other.gameObject);
				
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

	IEnumerator DeadlyTransition ()
	{
		cubeColor = CubeColor.Neutral;

		if(GetComponent<PlayersDeadCube> () == null)
			GlobalMethods.Instance.SpawnNewMovableRandomVoid (gameObject, 2);

		tag = "Untagged";

		ToDeadlyColor ();

		while (rigidbodyMovable.velocity.magnitude > deadlyCubeMaxVelocity)
		{
			rigidbodyMovable.velocity = rigidbodyMovable.velocity.normalized * deadlyCubeDeceleration;

			yield return new WaitForFixedUpdate ();
		}

		yield return new WaitForSeconds (deadlyCubeTransitionDuration);

		tag = "DeadCube";

		rigidbodyMovable.mass = deadlyCubeMass;
	}
}
