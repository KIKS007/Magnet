using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;

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
	[PropertyOrder (-1)]
	[ButtonAttribute] 
	void NextStep ()
	{
		StopAllCoroutines ();

		switch (tutorialState)
		{
		case TutorialState.Movement:
			StartCoroutine (DashStep ());
			break;
		case TutorialState.DashStep:
			StartCoroutine (DashHitStep ());
			break;
		case TutorialState.DashHitStep:
			StartCoroutine (AttractRepelStep ());
			break;
		case TutorialState.AttractRepelStep:
			StartCoroutine (ShootStep ());
			break;
		case TutorialState.ShootStep:
			StartCoroutine (DeadlyWallStep ());
			break;
		}
	}

	public TutorialState tutorialState = TutorialState.DeadlyWall;

	[Header ("Players")]
	public List<PlayerTutorialStats> PlayerStats = new List<PlayerTutorialStats> ();

	[Header ("Infos")]
	public float tweenDuration = 0.5f;
	public float delayDuration = 0.2f;
	public List<TutorialInfos> tutorialInfos = new List<TutorialInfos> ();

	[Header ("Intro")]
	public float introWaitDuration;

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
	public ArenaDeadzones arena;
	public int livesCount = 4;

	[Header ("DEAD CUBES")]
	public float timeBeforePlayerRespawn = 2;
	public bool oneDeadCube = false;
	public MovableScript movableExampleScript;

	private List<PlayersTutorial> playersScript = new List<PlayersTutorial> ();
	private List<PlayersFXAnimations> playersFX = new List<PlayersFXAnimations> ();

	private ZoomCamera zoomCamera;
	public int tutorialInfosIndex = 0;
	private Transform previousPanel = null;

	// Use this for initialization
	void Start () 
	{
		arena = FindObjectOfType<ArenaDeadzones> ();

		zoomCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<ZoomCamera> ();

		foreach(var t in tutorialInfos)
		{
			foreach(var p in t.panels)
			{
				p.localScale = Vector3.zero;
				p.gameObject.SetActive (true);
			}
		}

		StartCoroutine (WaitPlaying ());
	}

	void Update ()
	{
		for(int i = 0; i < playersScript.Count; i++)
		{
			PlayerStats [i].movingTime = playersScript [i].movingTime;
			PlayerStats [i].dashCount = playersScript [i].dashCount;
			PlayerStats [i].dashHitCount = playersScript [i].dashHitCount;
			PlayerStats [i].attractTime = playersScript [i].attractTime;
			PlayerStats [i].repelTime = playersScript [i].repelTime;
			PlayerStats [i].shootsCount = playersScript [i].shootsCount;
			PlayerStats [i].hitsCount = playersScript [i].hitsCount;
			PlayerStats [i].deathCount = playersScript [i].deathCount;
		}
	}

	IEnumerator ShowInfos ()
	{
		if(previousPanel != null && previousPanel.localScale == Vector3.one)
		{
			previousPanel.DOScale (0, tweenDuration).SetEase (MenuManager.Instance.easeMenu).SetUpdate (false);
			yield return new WaitForSeconds (tweenDuration);
		}

		for(int i = 0; i < tutorialInfos [tutorialInfosIndex].panels.Count; i++)
		{
			tutorialInfos [tutorialInfosIndex].panels [i].localScale = Vector3.zero;
			tutorialInfos [tutorialInfosIndex].panels [i].DOScale (1, tweenDuration).SetEase (MenuManager.Instance.easeMenu).SetUpdate (false);

			yield return new WaitForSeconds (MenuManager.Instance.animationDuration + tutorialInfos [tutorialInfosIndex].durations [i]);

			if(i != tutorialInfos [tutorialInfosIndex].panels.Count - 1)
			{
				tutorialInfos [tutorialInfosIndex].panels [i].DOScale (0, tweenDuration).SetEase (MenuManager.Instance.easeMenu).SetUpdate (false);
				
				yield return new WaitForSeconds (delayDuration);
			}
			else
			{
				previousPanel = tutorialInfos [tutorialInfosIndex].panels [i];
			}
		}

		/*if(tutorialInfosButton.localScale != Vector3.zero)
		{
			tutorialInfosButton.DOScale (0, MenuManager.Instance.animationDuration).SetEase (MenuManager.Instance.easeMenu).SetUpdate (false).OnComplete (()=> {
				
				tutorialInfosButton.GetChild (0).GetComponent<Text> ().text = tutorialInfos [tutorialInfosIndex].title;
				tutorialInfosButton.GetChild (1).GetComponent<Text> ().text = tutorialInfos [tutorialInfosIndex].description;

				tutorialInfosButton.DOScale (1, MenuManager.Instance.animationDuration).SetEase (MenuManager.Instance.easeMenu).SetUpdate (false);
			});
			
		}
		else
		{
			tutorialInfosButton.GetChild (0).GetComponent<Text> ().text = tutorialInfos [tutorialInfosIndex].title;
			tutorialInfosButton.GetChild (1).GetComponent<Text> ().text = tutorialInfos [tutorialInfosIndex].description;

			tutorialInfosButton.DOScale (1, MenuManager.Instance.animationDuration).SetEase (MenuManager.Instance.easeMenu).SetUpdate (false);
		}
*/
		tutorialInfosIndex++;

		yield return 0;
	}

	IEnumerator WaitPlaying ()
	{
		yield return new WaitUntil (() => GlobalVariables.Instance.GameState == GameStateEnum.Playing);

		DOVirtual.DelayedCall (0.01f, ()=> {
			arena.StopAllCoroutines ();
			arena.Reset ();
		});

		playersScript.Clear ();

		foreach (GameObject g in GlobalVariables.Instance.EnabledPlayersList)
		{
			playersScript.Add (g.GetComponent<PlayersTutorial> ());
			playersFX.Add (g.GetComponent<PlayersFXAnimations> ());
			PlayerStats.Add (new PlayerTutorialStats ());
		}

		foreach (PlayersTutorial p in playersScript)
			p.livesCount = livesCount;

		yield return StartCoroutine (ShowInfos ());

		yield return new WaitForSeconds (introWaitDuration);

		StartCoroutine (MovementStep ());
	}

	IEnumerator MovementStep ()
	{
		StartCoroutine (ShowInfos ());

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
		StartCoroutine (ShowInfos ());

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
		if(playersScript.Count == 1)
		{
			tutorialState |= TutorialState.DashHit;
			tutorialInfosIndex++;

			StartCoroutine (AttractRepelStep ());

			yield break;
		}

		StartCoroutine (ShowInfos ());

		tutorialState |= TutorialState.DashHit;

		zoomCamera.Zoom (FeedbackType.Startup);
		Waves ();

		yield return new WaitUntil (()=>
			{ 
				bool pass = true;

				foreach(PlayersTutorial p in playersScript)
					if(p.dashHitCount < dashHitCount)
						pass = false;

				if(playersScript.Count == 1)
					pass = true;

				return pass;
			});
		
		StartCoroutine (AttractRepelStep ());
	}

	IEnumerator AttractRepelStep ()
	{
		StartCoroutine (ShowInfos ());

		tutorialState |= TutorialState.AttractRepel;

		if(GlobalVariables.Instance.AllMovables.Count > 0)
			GlobalMethods.Instance.RandomPositionMovablesVoid (GlobalVariables.Instance.AllMovables.ToArray ());

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
		StartCoroutine (ShowInfos ());

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
		StartCoroutine (ShowInfos ());

		arena.Setup ();

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

	public virtual void PlayerDeath (PlayerName playerName, GameObject player)
	{
		PlayersGameplay playerScript = player.GetComponent<PlayersGameplay> ();

		playerScript.livesCount--;

		GlobalVariables.Instance.ListPlayers ();

		//Spawn Play if has lives left
		if(playerScript.livesCount != 0)
		{
			GlobalMethods.Instance.SpawnDeathText (playerName, player, playerScript.livesCount);
			GlobalMethods.Instance.SpawnExistingPlayerRandomVoid (player, timeBeforePlayerRespawn, true);
		}
		else 
		{
			GlobalMethods.Instance.SpawnPlayerDeadCubeVoid (playerScript.playerName, playerScript.controllerNumber, movableExampleScript);

			if(!oneDeadCube)
			{
				oneDeadCube = true;
			}
		}

		StartCoroutine (ShowInfos ());
	}

	[System.Serializable]
	public class TutorialInfos
	{
		public List<float> durations = new List<float> ();
		public List<Transform> panels = new List<Transform> ();
	}
	
	[System.Serializable]
	public class PlayerTutorialStats
	{
		[Header ("MOVEMENT")]
		public float movingTime = 0;
		
		[Header ("DASH")]
		public int dashCount = 0;
		
		[Header ("DASH Hit")]
		public int dashHitCount = 0;
		
		[Header ("ATTRACT / REPEL")]
		public float attractTime = 0;
		public float repelTime = 0;
		
		[Header ("SHOOTS")]
		public int shootsCount = 0;
		public int hitsCount = 0;
		
		[Header ("DEADLY WALLS")]
		public int deathCount = 0;
	}
}
