using UnityEngine;
using System.Collections;

public class PlayersVibration : MonoBehaviour 
{
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
		VibrationManager.Instance.Vibrate (controllerNumber, FeedbackType.Dash);
	}

	void Stun ()
	{
		VibrationManager.Instance.Vibrate (controllerNumber, FeedbackType.Stun);
	}

	void Shoot ()
	{
		VibrationManager.Instance.Vibrate (controllerNumber, FeedbackType.Shoot);
	}

	void Hold ()
	{
		VibrationManager.Instance.Vibrate (controllerNumber, FeedbackType.Hold);
	}

	void Death ()
	{
		VibrationManager.Instance.Vibrate (controllerNumber, FeedbackType.Death);
	}

	public void Wave ()
	{
		VibrationManager.Instance.Vibrate (controllerNumber, FeedbackType.Wave);
	}
}
