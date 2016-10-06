﻿using UnityEngine;
using System.Collections;
using DG.Tweening;
using DarkTonic.MasterAudio;

public class MovableBomb : MovableScript 
{
	public GameObject playerHolding;

	[Header ("Explosion")]
	[SoundGroupAttribute]
	public string explosionSound;
	[SoundGroupAttribute]
	public string lastSecondsSound;
	[SoundGroupAttribute]
	public string cubeTrackingSound;

	public float explosionForce = 10;
	public float explosionRadius = 3;
	public LayerMask explosionMask;

	public float getToPlayerForce = 2;
	public float distanceFactor = 2;

	private bool trackingPlayer = false;

	protected override void Start ()
	{
		tag = "Untagged";
		hold = false;

		rigidbodyMovable = GetComponent<Rigidbody>();
		movableRenderer = GetComponent<Renderer> ();
		cubeMeshFilter = transform.GetChild (2).GetComponent<MeshFilter> ();
		cubeMaterial = transform.GetChild (1).GetComponent<Renderer> ().material;

		GetRigidbodySettings ();

		cubeMaterial.SetFloat ("_Lerp", 0);
		cubeMaterial.SetColor ("_Color", GlobalVariables.Instance.cubeNeutralColor);

		tag = "Movable";
	}

	protected override void OnEnable ()
	{
		tag = "Untagged";
		hold = false;

		rigidbodyMovable = GetComponent<Rigidbody>();
		movableRenderer = GetComponent<Renderer> ();
		cubeMeshFilter = transform.GetChild (2).GetComponent<MeshFilter> ();
		cubeMaterial = transform.GetChild (1).GetComponent<Renderer> ().material;

		GetRigidbodySettings ();

		cubeMaterial.SetFloat ("_Lerp", 0);
		cubeMaterial.SetColor ("_Color", GlobalVariables.Instance.cubeNeutralColor);

		tag = "Movable";
	}

	protected override void Update () 
	{
		if(hold == false)
			currentVelocity = rigidbodyMovable.velocity.magnitude;


		if(hold == false && currentVelocity > 0 && !trackingPlayer)
		{
			if(currentVelocity > higherVelocity)
			{
				higherVelocity = currentVelocity;
			}

			if(currentVelocity >= limitVelocity)
			{
				gameObject.tag = "ThrownMovable";
			}
			else if(currentVelocity < limitVelocity)
			{
				gameObject.tag = "Movable";
				playerThatThrew = null;
			}
		}

		if(hold)
		{
			Color cubeCorrectColor = new Color ();

			switch(player.name)
			{
			case "Player 1":
				cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer1;
				break;
			case "Player 2":
				cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer2;
				break;
			case "Player 3":
				cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer3;
				break;
			case "Player 4":
				cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer4;
				break;
			}

			if (cubeMaterial.GetColor("_Color") != cubeCorrectColor)
			{
				Color cubeColorTemp = cubeMaterial.GetColor("_Color");
				float cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");

				DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, cubeCorrectColor, toColorDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp));
				DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 1, toColorDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp));
			}
		}
	}

	protected override void HitPlayer (Collision other)
	{
		if(tag == "Movable" && other.gameObject.tag == "Player" || tag == "ThrownMovable" && other.gameObject.tag == "Player" && !trackingPlayer)
		{
			if(playerThatThrew == null)
			{
				if(!trackingPlayer && playerThatThrew != null)
					StatsManager.Instance.PlayersFragsAndHits (playerThatThrew, other.gameObject);

				other.gameObject.GetComponent<PlayersBomb>().GetBomb(GetComponent<Collider>());
				playerHolding = other.gameObject;

				playerHit = other.gameObject;

				GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking();

				InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);
			}

			else if(other.collider.GetComponent<PlayersGameplay>().playerState == PlayerState.Stunned && playerThatThrew != other.gameObject)
			{
				if(!trackingPlayer && playerThatThrew != null)
					StatsManager.Instance.PlayersFragsAndHits (playerThatThrew, other.gameObject);

				other.gameObject.GetComponent<PlayersBomb>().GetBomb(GetComponent<Collider>());
				playerHolding = other.gameObject;

				playerHit = other.gameObject;

				GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking();

				InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);
			}

			else if(playerThatThrew != other.gameObject)
			{
				if(!trackingPlayer && playerThatThrew != null)
					StatsManager.Instance.PlayersFragsAndHits (playerThatThrew, other.gameObject);

				other.gameObject.GetComponent<PlayersBomb>().GetBomb(GetComponent<Collider>());
				playerHolding = other.gameObject;

				playerHit = other.gameObject;

				GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking();

				InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);
			}
		}

		if(tag == "ThrownMovable" && other.gameObject.tag == "Player" && trackingPlayer)
		{
			Debug.Log ("Bomb Hit");
			hold = true;
			playerHolding = other.gameObject;

			playerHit = other.gameObject;

			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking();

			InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);
		}

	}

	public override void OnHold ()
	{
		base.OnHold ();

		playerHolding = player.gameObject;
	}

	public override void OnRelease ()
	{
		
	}

	public void ResetColor ()
	{
		if(cubeMaterial == null)
			cubeMaterial = transform.GetChild (1).GetComponent<Renderer> ().material;

		Color cubeColorTemp = cubeMaterial.GetColor("_Color");
		float cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");

		DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, GlobalVariables.Instance.cubeNeutralColor, toNeutralDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp));
		DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 0, toNeutralDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp));
	}

	public IEnumerator Explode ()
	{
		if (!hold)
		{
			trackingPlayer = true;
			tag = "ThrownMovable";

			yield return StartCoroutine (GetToPlayerPosition ());
		}

		GlobalMethods.Instance.Explosion (transform.position, explosionForce, explosionRadius, explosionMask);

		MasterAudio.StopAllOfSound(lastSecondsSound);
		MasterAudio.StopAllOfSound(cubeTrackingSound);
		MasterAudio.PlaySound3DAtTransformAndForget (explosionSound, transform);

		ExplosionFX ();

		playerHolding.GetComponent<PlayersGameplay> ().Death ();
		gameObject.SetActive (false);

		playerHolding = null;
		trackingPlayer = false;
		hold = false;
	}

	void ExplosionFX ()
	{
		int playerNumber = -1;

		switch(playerHolding.name)
		{
		case "Player 1":
			playerNumber = 0;
			break;
		case "Player 2":
			playerNumber = 1;
			break;
		case "Player 3":
			playerNumber = 2;
			break;
		case "Player 4":
			playerNumber = 3;
			break;
		}

		GameObject instance = Instantiate (GlobalVariables.Instance.explosionFX [playerNumber], transform.position, GlobalVariables.Instance.explosionFX [playerNumber].transform.rotation) as GameObject;
		instance.transform.parent = GlobalVariables.Instance.ParticulesClonesParent.transform;
	}

	IEnumerator GetToPlayerPosition ()
	{
		transform.DORotate (Vector3.zero, 0.5f);
		transform.DOLocalMoveY (1.5f, 0.5f);

		rigidbodyMovable.velocity = Vector3.zero;
		rigidbodyMovable.angularVelocity = Vector3.zero;

		while(Vector3.Distance(playerHolding.transform.position, transform.position) > 0.5f)
		{
			if (!hold)
			{
				Vector3 direction = (playerHolding.transform.position - transform.position);
				direction.Normalize ();

				//float distance = Vector3.Distance (playerHolding.transform.position, transform.position) + distanceFactor;
				//rigidbodyMovable.MovePosition (transform.position + direction * distance * getToPlayerForce * Time.deltaTime);
				rigidbodyMovable.AddForce(direction * getToPlayerForce, ForceMode.Impulse);

				yield return new WaitForFixedUpdate();
			}
			else
			{
				trackingPlayer = false;
				break;
			}
		}
	}
}
