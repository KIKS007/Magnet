﻿using UnityEngine;
using System.Collections;
using DarkTonic.MasterAudio;

public class PlayersSounds : MonoBehaviour 
{
	public float fadeDuration = 1;

	[SoundGroupAttribute]
	public string attractingSound;
	[SoundGroupAttribute]
	public string repulsingSound;
	[SoundGroupAttribute]
	public string onHoldSound;
	[SoundGroupAttribute]
	public string shootSound;
	[SoundGroupAttribute]
	public string stunSound;
	[SoundGroupAttribute]
	public string dashSound;
	[SoundGroupAttribute]
	public string deathSound;


	private PlayersGameplay playerScript;

	static float attractingVolume = -1;
	static float repulsingVolume = -1;

	// Use this for initialization
	void Start () 
	{
		playerScript = GetComponent<PlayersGameplay> ();

		if(attractingVolume == -1)
		{
			attractingVolume = MasterAudio.GetGroupVolume (attractingSound);
			repulsingVolume = MasterAudio.GetGroupVolume (repulsingSound);
		}
			
		MasterAudio.SetGroupVolume (attractingSound, 0);
		MasterAudio.SetGroupVolume (repulsingSound, 0);

		playerScript.OnPlayerstateChange += Attracting;
		playerScript.OnPlayerstateChange += Repulsing;
		playerScript.OnAttracting += Attracting;
		playerScript.OnRepulsing += Repulsing;

		playerScript.OnHold += OnHold;
		playerScript.OnShoot += Shoot;
		playerScript.OnStun += Stun;
		playerScript.OnDash += Dash;
		playerScript.OnDeath += Death;

		GlobalVariables.Instance.OnGameOver += FadeSounds;
		GlobalVariables.Instance.OnPause += FadeSounds;
	}

	private bool fadeInAttraction = false;
	private bool fadeOutAttraction = false;

	void Attracting ()
	{
		if(playerScript.cubesAttracted.Count > 0 && MasterAudio.GetGroupVolume(attractingSound) != attractingVolume && !fadeInAttraction)
		{
			MasterAudio.PlaySound3DFollowTransformAndForget (attractingSound, transform);
			MasterAudio.FadeSoundGroupToVolume (attractingSound, attractingVolume, fadeDuration, ()=> fadeInAttraction = false);
		}

		if(playerScript.cubesAttracted.Count == 0 && MasterAudio.GetGroupVolume(attractingSound) != 0 && !fadeOutAttraction)
			MasterAudio.FadeSoundGroupToVolume (attractingSound, 0, fadeDuration, ()=> fadeOutAttraction = false);			
	}

	private bool fadeInRepulsion = false;
	private bool fadeOutRepulsion = false;

	void Repulsing ()
	{
		if(playerScript.cubesRepulsed.Count > 0 && MasterAudio.GetGroupVolume(repulsingSound) != repulsingVolume && !fadeInRepulsion)
		{
			MasterAudio.PlaySound3DFollowTransformAndForget (repulsingSound, transform);
			MasterAudio.FadeSoundGroupToVolume (repulsingSound, repulsingVolume, fadeDuration, ()=> fadeInRepulsion = false);
		}

		if(playerScript.cubesRepulsed.Count == 0 && MasterAudio.GetGroupVolume(repulsingSound) != 0 && !fadeOutRepulsion)
			MasterAudio.FadeSoundGroupToVolume (repulsingSound, 0, fadeDuration, ()=> fadeOutRepulsion = false);
	}

	void OnHold ()
	{
		MasterAudio.PlaySound3DFollowTransformAndForget (onHoldSound, transform);
	}

	void Shoot ()
	{
		MasterAudio.PlaySound3DFollowTransformAndForget (shootSound, transform);
	}

	void Stun ()
	{
		MasterAudio.PlaySound3DFollowTransformAndForget (stunSound, transform);
	}

	void Dash ()
	{
		MasterAudio.PlaySound3DFollowTransformAndForget (dashSound, transform);
	}

	void Death ()
	{
		MasterAudio.PlaySound3DFollowTransformAndForget (deathSound, transform);
	}

	void OnDisable ()
	{
		//FadeSounds ();
	}

	void FadeSounds ()
	{
		MasterAudio.FadeSoundGroupToVolume (attractingSound, 0, fadeDuration);
		MasterAudio.FadeSoundGroupToVolume (repulsingSound, 0, fadeDuration);
	}
}
