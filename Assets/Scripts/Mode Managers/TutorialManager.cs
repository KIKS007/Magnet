using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Flags]
public enum TutorialState 
{ 
	Movement = 1, 
	Dash = 2, 
	DashHit = 4, 
	AttractRepel = 8, 
	Shoot = 16, 
	DeadlyWall = 32,

	DashStep = Movement | Dash,
	DashHitStep = Movement | Dash | DashHit,
	AttractRepelStep = Movement | Dash | DashHit | AttractRepel,
	ShootStep = Movement | Dash | DashHit | AttractRepel | Shoot,
	DeadlyWallStep = Movement | Dash | DashHit | AttractRepel | Shoot | DeadlyWall
}

public class TutorialManager : MonoBehaviour 
{
	public TutorialState tutorialState = TutorialState.Movement;

	[Header ("MOVEMENT")]
	public float movingTime = 6;

	[Header ("DASH")]
	public int dashCount = 3;
	
	[Header ("DASH Hit")]
	public int dashHitCount = 3;

	[Header ("ATTRACT / REPEL")]
	public float attractTime = 4;
	public float repelTime = 3;

	[Header ("SHOOTS")]
	public int shootsCount = 5;
	public int hitsCount = 4;

	[Header ("DEADLY WALLS")]
	public int deathCount = 1;

	private List<PlayersTutorial> playersScript = new List<PlayersTutorial> ();
	private List<PlayersFXAnimations> playersFX = new List<PlayersFXAnimations> ();

	private ZoomCamera zoomCamera;

	// Use this for initialization
	void Start () 
	{
		zoomCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<ZoomCamera> ();
		StartCoroutine (WaitPlaying ());
	}

	IEnumerator WaitPlaying ()
	{
		yield return new WaitUntil (() => GlobalVariables.Instance.GameState == GameStateEnum.Playing);

		playersScript.Clear ();

		foreach (GameObject g in GlobalVariables.Instance.EnabledPlayersList)
		{
			playersScript.Add (g.GetComponent<PlayersTutorial> ());
			playersFX.Add (g.GetComponent<PlayersFXAnimations> ());
		}

		StartCoroutine (MovementStep ());
	}

	IEnumerator MovementStep ()
	{
		tutorialState = TutorialState.Movement;

		yield return new WaitUntil (()=>
			{ 
				bool pass = true;
				
				foreach(PlayersTutorial p in playersScript)
					if(p.movingTime < movingTime)
						pass = false;

				return pass;
			});

		StartCoroutine (DashStep ());
	}

	IEnumerator DashStep ()
	{
		tutorialState |= TutorialState.Dash;

		zoomCamera.Zoom (FeedbackType.Startup);
		Waves ();

		yield return new WaitUntil (()=>
			{ 
				bool pass = true;

				foreach(PlayersTutorial p in playersScript)
					if(p.dashCount < dashCount)
						pass = false;

				return pass;
			});

		StartCoroutine (DashHitStep ());
	}

	IEnumerator DashHitStep ()
	{
		tutorialState |= TutorialState.DashHit;

		zoomCamera.Zoom (FeedbackType.Startup);
		Waves ();

		yield return new WaitUntil (()=>
			{ 
				bool pass = true;

				foreach(PlayersTutorial p in playersScript)
					if(p.dashHitCount < dashHitCount)
						pass = false;

				return pass;
			});
		
		StartCoroutine (AttractRepelStep ());
	}

	IEnumerator AttractRepelStep ()
	{
		tutorialState |= TutorialState.AttractRepel;

		zoomCamera.Zoom (FeedbackType.Startup);
		Waves ();

		yield return new WaitUntil (()=>
			{ 
				bool pass = true;

				foreach(PlayersTutorial p in playersScript)
					if(p.attractTime < attractTime || p.repelTime < repelTime)
						pass = false;

				return pass;
			});

		StartCoroutine (ShootStep ());
	}

	IEnumerator ShootStep ()
	{
		tutorialState |= TutorialState.Shoot;

		zoomCamera.Zoom (FeedbackType.Startup);
		Waves ();

		yield return new WaitUntil (()=>
			{ 
				bool pass = true;

				foreach(PlayersTutorial p in playersScript)
					if(p.shootsCount < shootsCount)
						pass = false;

				return pass;
			});

		StartCoroutine (DeadlyWallStep ());
	}

	IEnumerator DeadlyWallStep ()
	{
		tutorialState |= TutorialState.DeadlyWall;

		zoomCamera.Zoom (FeedbackType.Startup);
		Waves ();

		yield return 0;

	}

	void Waves ()
	{
		foreach (PlayersFXAnimations f in playersFX)
			f.WaveFX (true);
	}
}
