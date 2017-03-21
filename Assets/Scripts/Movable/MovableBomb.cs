using UnityEngine;
using System.Collections;
using DG.Tweening;
using DarkTonic.MasterAudio;

public class MovableBomb : MovableScript 
{
	[Header ("BOMB")]
	public GameObject playerHolding;
	public float trackSpeed = 1.2f;
	public float trackSpeedAdded = 0.001f;

	public bool trackingPlayer = false;

	[Header ("Explosion")]
	public float explosionForce = 50;
	public float explosionRadius = 50;

	private float speedAddedCooldown = 0.5f;
	private float trackSpeedTemp;

	protected override void OnEnable ()
	{
		hold = false;

		rigidbodyMovable = GetComponent<Rigidbody>();
		movableRenderer = GetComponent<Renderer> ();
		cubeMeshFilter = transform.GetChild (2).GetComponent<MeshFilter> ();
		cubeMaterial = transform.GetChild (1).GetComponent<Renderer> ().material;

		if(playerHolding == null)
		{
			cubeMaterial.SetFloat ("_Lerp", 0);
			cubeMaterial.SetColor ("_Color", GlobalVariables.Instance.cubePlayersColor[4]);
		}

		attracedBy.Clear ();
		repulsedBy.Clear ();
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
			else if(currentVelocity < limitVelocity && gameObject.tag == "ThrownMovable")
			{
				gameObject.tag = "Movable";
				playerThatThrew = null;
			}
		}
	}

	protected void SetCubeColor ()
	{
		int whichPlayer = (int)player.GetComponent<PlayersGameplay> ().playerName;
		Color cubeCorrectColor = (GlobalVariables.Instance.cubePlayersColor[whichPlayer]);

		/*	switch(player.GetComponent <PlayersGameplay>().playerName)
		{
		case PlayerName.Player1:
			cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer1;
			break;
		case PlayerName.Player2:
			cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer2;
			break;
		case PlayerName.Player3:
			cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer3;
			break;
		case PlayerName.Player4:
			cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer4;
			break;
		}*/

		if (DOTween.IsTweening ("CubeNeutralTween" + gameObject.GetInstanceID ()))
		{
			DOTween.Kill ("CubeNeutralTween" + gameObject.GetInstanceID ());
		}

		Color cubeColorTemp = cubeMaterial.GetColor("_Color");
		float cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");
		
		DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, cubeCorrectColor, toColorDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp)).SetId("CubeColorTween" + gameObject.GetInstanceID ()).OnComplete (()=> GetComponent<Renderer> ().material.color = cubeMaterial.GetColor ("_Color"));
		DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 1, toColorDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp)).SetId("CubeColorTween" + gameObject.GetInstanceID ());

	}

	protected override void HitPlayer (Collision other)
	{
		if(tag == "Movable" && other.gameObject.tag == "Player" 
			|| tag == "ThrownMovable" && other.gameObject.tag == "Player" && !trackingPlayer)
		{
			if(playerThatThrew == null)
			{
				if(!trackingPlayer && playerThatThrew != null)
					StatsManager.Instance.PlayersFragsAndHits (playerThatThrew, other.gameObject);

				other.gameObject.GetComponent<PlayersGameplay> ().OnHoldMovable (gameObject);
				playerHolding = other.gameObject;

				playerHit = other.gameObject;

				mainCamera.GetComponent<ScreenShakeCamera>().CameraShaking(FeedbackType.Stun);
				mainCamera.GetComponent<ZoomCamera>().Zoom(FeedbackType.Stun);

				InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);
			}

			else if(other.collider.GetComponent<PlayersGameplay>().playerState == PlayerState.Stunned && playerThatThrew != other.gameObject)
			{
				if(!trackingPlayer && playerThatThrew != null)
					StatsManager.Instance.PlayersFragsAndHits (playerThatThrew, other.gameObject);

				other.gameObject.GetComponent<PlayersGameplay> ().OnHoldMovable (gameObject);
				playerHolding = other.gameObject;

				playerHit = other.gameObject;

				mainCamera.GetComponent<ScreenShakeCamera>().CameraShaking(FeedbackType.Stun);
				mainCamera.GetComponent<ZoomCamera>().Zoom(FeedbackType.Stun);

				InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);
			}

			else if(playerThatThrew != other.gameObject)
			{
				if(!trackingPlayer && playerThatThrew != null)
					StatsManager.Instance.PlayersFragsAndHits (playerThatThrew, other.gameObject);

				other.gameObject.GetComponent<PlayersGameplay> ().OnHoldMovable (gameObject);
				playerHolding = other.gameObject;

				playerHit = other.gameObject;

				mainCamera.GetComponent<ScreenShakeCamera>().CameraShaking(FeedbackType.Stun);
				mainCamera.GetComponent<ZoomCamera>().Zoom(FeedbackType.Stun);

				InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);
			}
		}

		if(tag == "ThrownMovable" && other.gameObject.tag == "Player" && trackingPlayer && other.collider.GetComponent<PlayersGameplay>().playerState != PlayerState.Dead)
		{
			hold = true;
			playerHolding = other.gameObject;

			playerHit = other.gameObject;

			other.collider.GetComponent<PlayersGameplay> ().Death (DeathFX.All, other.contacts [0].point);

			mainCamera.GetComponent<ScreenShakeCamera>().CameraShaking(FeedbackType.Stun);
			mainCamera.GetComponent<ZoomCamera>().Zoom(FeedbackType.Stun);

			InstantiateParticles (other.contacts [0], GlobalVariables.Instance.HitParticles, other.gameObject.GetComponent<Renderer>().material.color);
		}

	}

	public override void OnHold ()
	{
		base.OnHold ();

		playerHolding = player.gameObject;

		SetCubeColor ();
	}

	public override void OnRelease ()
	{
		OnReleaseEventVoid ();
	}

	public void ResetColor ()
	{
		if(playerHolding == null && hold == false)
		{
			if(cubeMaterial == null)
				cubeMaterial = transform.GetChild (1).GetComponent<Renderer> ().material;
			
			Color cubeColorTemp = cubeMaterial.GetColor("_Color");
			float cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");
			
			DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, GlobalVariables.Instance.cubePlayersColor[4], toNeutralDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp)).SetId("CubeNeutralTween" + gameObject.GetInstanceID ());
			DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 0, toNeutralDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp)).SetId("CubeNeutralTween" + gameObject.GetInstanceID ());
		}

	}

	public IEnumerator Explode ()
	{
		if (!hold)
		{
			trackingPlayer = true;
			tag = "ThrownMovable";

			yield return StartCoroutine (GetToPlayerPosition ());
		}

		mainCamera.GetComponent<SlowMotionCamera>().StartSlowMotion();

		GlobalMethods.Instance.Explosion (transform.position, explosionForce, explosionRadius);

		MasterAudio.StopAllOfSound(SoundsManager.Instance.lastSecondsSound);
		MasterAudio.StopAllOfSound(SoundsManager.Instance.cubeTrackingSound);

		Vector3 explosionPos = Vector3.Lerp (playerHolding.transform.position, transform.position, 0.5f);

		playerHolding.GetComponent<PlayersGameplay> ().Death (DeathFX.All, explosionPos);

		gameObject.SetActive (false);

		playerHolding = null;
		trackingPlayer = false;
		hold = false;
	}

	IEnumerator GetToPlayerPosition ()
	{
		transform.DORotate (Vector3.zero, 0.5f);
		transform.DOLocalMoveY (1.5f, 0.5f);

		rigidbodyMovable.velocity = Vector3.zero;
		rigidbodyMovable.angularVelocity = Vector3.zero;

		trackSpeedTemp = trackSpeed;

		StartCoroutine (AddSpeed ());

		while(Vector3.Distance(playerHolding.transform.position, transform.position) > 0.5f)
		{
			if (!hold)
			{
				Vector3 direction = (playerHolding.transform.position - transform.position);
				direction.Normalize ();

				//float distance = Vector3.Distance (playerHolding.transform.position, transform.position) + distanceFactor;
				//rigidbodyMovable.MovePosition (transform.position + direction * distance * getToPlayerForce * Time.deltaTime);
				rigidbodyMovable.AddForce(direction * trackSpeedTemp, ForceMode.Impulse);

				yield return new WaitForFixedUpdate();
			}
			else
			{
				trackingPlayer = false;
				break;
			}
		}

		StopCoroutine (AddSpeed ());
	}

	IEnumerator AddSpeed ()
	{
		yield return new WaitForSeconds (speedAddedCooldown);

		trackSpeedTemp += trackSpeedAdded;

		StartCoroutine (AddSpeed ());
	}
}
