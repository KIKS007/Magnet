using UnityEngine;
using System.Collections;
using DG.Tweening;
using DarkTonic.MasterAudio;

public class MovableSuggestible : MovableScript 
{
	public override void Start ()
	{
		gameObject.tag = "Suggestible";
		slowMoTrigger = transform.GetComponentInChildren<SlowMotionTriggerScript> ();
		ToDeadlyColor ();
	}

	protected override void HitPlayer (Collision other)
	{
		if(other.collider.tag == "Player")
		{
			PlayersGameplay playerScript = other.collider.GetComponent<PlayersGameplay> ();

			if (playerScript.playerState == PlayerState.Dead)
				return;

			playerScript.Death (DeathFX.All, other.contacts [0].point);

			PlayerKilled ();

			foreach (GameObject g in attracedBy)
				StatsManager.Instance.PlayerKills (g.GetComponent<PlayersGameplay> ());

			foreach (GameObject g in repulsedBy)
				StatsManager.Instance.PlayerKills (g.GetComponent<PlayersGameplay> ());

			InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, GlobalVariables.Instance.playersColors [(int)playerScript.playerName]);

			GlobalMethods.Instance.Explosion (transform.position);
		}
	}
}
