using UnityEngine;
using System.Collections;
using Rewired;

public class PlayersDeadCube : MonoBehaviour 
{
	[Header("States")]
	public PlayerName playerName;
	public DashState dashState = DashState.CanDash;
	public bool hold = false;

	[Header("Controller Number")]
	public int controllerNumber = -1;
	[HideInInspector]
	public Player rewiredPlayer; // The Rewired Player

	[Header("Movement")]
	public float currentVelocity;
	public float maxVelocity;
	public float movementSpeed = 18;
	public float gravity = 100;

	[Header("Dash")]
	public float dashForce;
	public float dashCooldown;

	protected float rightJoystickDeadzone = 0.5f;

	[HideInInspector]
	public Rigidbody rigidBody;

	protected Vector3 movement;

	private MovablePlayer movableScript;

	private Vector3 rightMovePos;
	private Vector3 leftMovePos;
	private Vector3 forwardMovePos;
	private Vector3 backwardMovePos;

	private float halfScale;
	private ParticleSystem[] reincarnationFXs = new ParticleSystem[0];

	// Use this for initialization
	void Start () 
	{
		rigidBody = GetComponent<Rigidbody> ();
		movableScript = GetComponent<MovablePlayer> ();

		movableScript.OnReleaseEvent += () => rigidBody = GetComponent<Rigidbody> ();

		halfScale = transform.lossyScale.x * 0.5f;

		Controller ();
		SetReincarnationFX ();
		//movableScript.ToColor (playerName);
	}

	void SetReincarnationFX ()
	{
		GameObject FX = Instantiate (GlobalVariables.Instance.reincarnationFX [(int)playerName], transform.position, transform.rotation, transform) as GameObject;
		reincarnationFXs = FX.transform.GetComponentsInChildren<ParticleSystem> ();
	}

	public void Controller()
	{
		if (controllerNumber == -1)
		{
			Debug.LogWarning ("Controller Number Not Set! " + name);
			gameObject.SetActive(false);
		}

		if (controllerNumber != -1)
			rewiredPlayer = ReInput.players.GetPlayer(controllerNumber);
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		movement = new Vector3(rewiredPlayer.GetAxisRaw("Move Vertical"), 0f, -rewiredPlayer.GetAxisRaw("Move Horizontal"));
		movement.Normalize();

		hold = movableScript.hold;

		SetMovePositions ();

		if (!hold && rigidBody != null && dashState == DashState.CanDash && rewiredPlayer.GetButtonDown ("Dash"))
			StartCoroutine (Dash ());
			
		//Taunt
		if (rewiredPlayer.GetButtonDown ("Taunt"))
		{
			foreach(ParticleSystem particle in reincarnationFXs)
			{
				if (particle.isPlaying)
					particle.Stop ();
				else
					particle.Play ();
			}
		}


		if (!hold && rigidBody != null)
			currentVelocity = rigidBody.velocity.magnitude;
	}

	void SetMovePositions ()
	{
		rightMovePos = new Vector3 (transform.position.x + halfScale, transform.position.y + halfScale, transform.position.z);
		leftMovePos = new Vector3 (transform.position.x - halfScale, transform.position.y + halfScale, transform.position.z);

		forwardMovePos = new Vector3 (transform.position.x, transform.position.y + halfScale, transform.position.z + halfScale);
		backwardMovePos = new Vector3 (transform.position.x, transform.position.y + halfScale, transform.position.z - halfScale);
	}

	void FixedUpdate () 
	{
		if(!hold && rigidBody != null)
		{
			//Vector3 movement = new Vector3 (transform.forward * player.GetAxisRaw("Move Horizontal"), 0, transform.right * player.GetAxisRaw("Move Vertical"));

			//rigidBody.AddTorque (movement * torqueForce);
			//rigidBody.MoveRotation (rigidBody.rotation * Quaternion.Euler (movement * torqueForce));
			//rigidBody.AddForceAtPosition (Vector3.right * torqueForce * player.GetAxisRaw("Move Horizontal"), rightMovement.position);

			if(dashState != DashState.Dashing)
				Movement ();

			rigidBody.AddForce(-Vector3.up * gravity, ForceMode.Acceleration);
		}
	}

	IEnumerator Dash ()
	{
		dashState = DashState.Dashing;

		Vector3 movement = new Vector3 (rewiredPlayer.GetAxisRaw("Move Horizontal"), 0, rewiredPlayer.GetAxisRaw("Move Vertical"));
		movement.Normalize ();

		rigidBody.AddForce (movement * dashForce, ForceMode.VelocityChange);

		dashState = DashState.Cooldown;

		yield return new WaitForSeconds (dashCooldown);

		dashState = DashState.CanDash;
	}

	void Movement ()
	{
		if(currentVelocity < maxVelocity)
		{
			if(rewiredPlayer.GetAxisRaw("Move Horizontal") > 0)
				rigidBody.AddForceAtPosition (Vector3.right * movementSpeed * rewiredPlayer.GetAxisRaw("Move Horizontal"), rightMovePos, ForceMode.VelocityChange);
			
			if(rewiredPlayer.GetAxisRaw("Move Horizontal") < 0)
				rigidBody.AddForceAtPosition (Vector3.right * movementSpeed * rewiredPlayer.GetAxisRaw("Move Horizontal"), leftMovePos, ForceMode.VelocityChange);
			
			if(rewiredPlayer.GetAxisRaw("Move Vertical") > 0)
				rigidBody.AddForceAtPosition (Vector3.forward * movementSpeed * rewiredPlayer.GetAxisRaw("Move Vertical"), forwardMovePos, ForceMode.VelocityChange);
			
			if(rewiredPlayer.GetAxisRaw("Move Vertical") < 0)
				rigidBody.AddForceAtPosition (Vector3.forward * movementSpeed * rewiredPlayer.GetAxisRaw("Move Vertical"), backwardMovePos, ForceMode.VelocityChange);
		}
	}
}
