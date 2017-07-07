using UnityEngine;
using System.Collections;
using DarkTonic.MasterAudio;
using DG.Tweening;

public class MovableDeadCube : MovableScript
{
	public override void OnEnable ()
	{
		tag = "DeadCube";

		hold = false;

		rigidbodyMovable = GetComponent<Rigidbody>();
		movableRenderer = GetComponent<Renderer> ();
		cubeMeshFilter = transform.GetChild (2).GetComponent<MeshFilter> ();
		cubeMaterial = transform.GetChild (1).GetComponent<Renderer> ().material;
		slowMoTrigger = transform.GetComponentInChildren<SlowMotionTriggerScript> ();

		//cubeMaterial.SetFloat ("_Lerp", 1);
		//cubeMaterial.SetColor ("_Color", Color.black);

		cubeMeshFilter.mesh = GlobalVariables.Instance.cubesStripes [Random.Range (0, GlobalVariables.Instance.cubesStripes.Length)];
		attracedBy.Clear ();
		repulsedBy.Clear ();

		ToDeadlyColor (0.01f);
	}

	public override void Start ()
	{
		ToDeadlyColor (0.01f);
	}

	protected override void HitPlayer (Collision other)
	{
		if(other.collider.tag == "Player")
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
}
