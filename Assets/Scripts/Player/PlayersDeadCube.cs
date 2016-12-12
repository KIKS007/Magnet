using UnityEngine;
using System.Collections;
using Rewired;

public class PlayersDeadCube : MonoBehaviour 
{
	[Header("States")]
	public PlayerName playerName;
	public bool hold = false;

	[Header("Controller Number")]
	public int controllerNumber = -1;
	[HideInInspector]
	public Player player; // The Rewired Player

	[Header("Movement")]
	public float currentVelocity;
	public float maxVelocity;
	public float movementSpeed = 18;
	public float gravity = 100;

	protected float rightJoystickDeadzone = 0.5f;

	[HideInInspector]
	public Rigidbody rigidBody;

	protected Vector3 movement;

	private MovableDeadCube movableScript;

	private Vector3 rightMovePos;
	private Vector3 leftMovePos;
	private Vector3 forwardMovePos;
	private Vector3 backwardMovePos;

	private float halfScale;

	// Use this for initialization
	void Start () 
	{
		rigidBody = GetComponent<Rigidbody> ();
		movableScript = GetComponent<MovableDeadCube> ();

		movableScript.OnReleaseEvent += () => rigidBody = GetComponent<Rigidbody> ();

		halfScale = transform.lossyScale.x * 0.5f;


		//TEST
		/*controllerNumber = 1;
		playerName = PlayerName.Player2;*/


		Controller ();
		movableScript.ToColor (playerName);
	}

	public void Controller()
	{
		if (controllerNumber == -1)
		{
			Debug.LogWarning ("Controller Number Not Set! " + name);
			gameObject.SetActive(false);
		}

		if (controllerNumber != -1)
		{
			player = ReInput.players.GetPlayer(controllerNumber);

			if(controllerNumber > 0)
			{
				for(int i = 0; i < GamepadsManager.Instance.gamepadsList.Count; i++)
				{
					if(GamepadsManager.Instance.gamepadsList[i].GamepadId == controllerNumber)
						player.controllers.AddController(GamepadsManager.Instance.gamepadsList[i].GamepadController, true);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		movement = new Vector3(player.GetAxisRaw("Move Vertical"), 0f, -player.GetAxisRaw("Move Horizontal"));
		movement.Normalize();

		hold = movableScript.hold;

		SetMovePositions ();

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

			Movement ();

			rigidBody.AddForce(-Vector3.up * gravity, ForceMode.Acceleration);
		}
	}

	void Movement ()
	{
		if(currentVelocity < maxVelocity)
		{
			if(player.GetAxisRaw("Move Horizontal") > 0)
				rigidBody.AddForceAtPosition (Vector3.right * movementSpeed * player.GetAxisRaw("Move Horizontal"), rightMovePos, ForceMode.VelocityChange);
			
			if(player.GetAxisRaw("Move Horizontal") < 0)
				rigidBody.AddForceAtPosition (Vector3.right * movementSpeed * player.GetAxisRaw("Move Horizontal"), leftMovePos, ForceMode.VelocityChange);
			
			if(player.GetAxisRaw("Move Vertical") > 0)
				rigidBody.AddForceAtPosition (Vector3.forward * movementSpeed * player.GetAxisRaw("Move Vertical"), forwardMovePos, ForceMode.VelocityChange);
			
			if(player.GetAxisRaw("Move Vertical") < 0)
				rigidBody.AddForceAtPosition (Vector3.forward * movementSpeed * player.GetAxisRaw("Move Vertical"), backwardMovePos, ForceMode.VelocityChange);
		}
	}
}
