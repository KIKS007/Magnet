using UnityEngine;
using System.Collections;

public class PlayersVibration : MonoBehaviour 
{
	[Header ("Hold")]
	[Range (0,1)]
	public float leftMotorHold;
	[Range (0,1)]
	public float rightMotorHold;
	public float durationHold;

	[Header ("Shoot")]
	[Range (0,1)]
	public float leftMotorShoot;
	[Range (0,1)]
	public float rightMotorShoot;
	public float durationShoot;

	[Header ("Stun")]
	[Range (0,1)]
	public float leftMotorStun;
	[Range (0,1)]
	public float rightMotorStun;
	public float durationStun;

	[Header ("Dash")]
	[Range (0,1)]
	public float leftMotorDash;
	[Range (0,1)]
	public float rightMotorDash;
	public float durationDash;

	[Header ("Death")]
	[Range (0,1)]
	public float leftMotorDeath;
	[Range (0,1)]
	public float rightMotorDeath;
	public float durationDeath;


	private PlayersGameplay playerScript;
	private int controllerNumber;

	// Use this for initialization
	void Start () 
	{
		playerScript = GetComponent<PlayersGameplay> ();

		playerScript.OnDash += Dash;
		playerScript.OnStun += Stun;
		playerScript.OnShoot += Shoot;
		playerScript.OnHold += Hold;
		playerScript.OnDeath += Death;
	}
	
	// Update is called once per frame
	void Update () 
	{
		controllerNumber = playerScript.controllerNumber;
	}

	void Dash ()
	{
		VibrationManager.Instance.Vibrate (controllerNumber, leftMotorDash, rightMotorDash, durationDash);
	}

	void Stun ()
	{
		VibrationManager.Instance.Vibrate (controllerNumber, leftMotorStun, rightMotorStun, durationStun);
	}

	void Shoot ()
	{
		VibrationManager.Instance.Vibrate (controllerNumber, leftMotorShoot, rightMotorShoot, durationShoot);
	}

	void Hold ()
	{
		VibrationManager.Instance.Vibrate (controllerNumber, leftMotorHold, rightMotorHold, durationHold);
	}

	void Death ()
	{
		VibrationManager.Instance.Vibrate (controllerNumber, leftMotorDeath, rightMotorDeath, durationDeath);
	}
}
