using UnityEngine;
using System.Collections;
using DarkTonic.MasterAudio;

public class PlayersSounds : MonoBehaviour 
{
	public float fadeDuration = 1;

	[SoundGroupAttribute]
	public string attractingSound;
	[SoundGroupAttribute]
	public string repulsingSound;

	private PlayersGameplay playerScript;

	static float initialAttractingVolume = -1;
	static float initialRepulsingVolume = -1;

	// Use this for initialization
	void Start () 
	{
		playerScript = GetComponent<PlayersGameplay> ();

		if(initialAttractingVolume == -1)
		{
			initialAttractingVolume = MasterAudio.GetGroupVolume (attractingSound);
			initialRepulsingVolume = MasterAudio.GetGroupVolume (repulsingSound);
		}
			
		MasterAudio.SetGroupVolume (attractingSound, 0);
		MasterAudio.SetGroupVolume (repulsingSound, 0);

		playerScript.OnPlayerstateChange += Attracting;
		playerScript.OnPlayerstateChange += Repulsing;
		playerScript.OnAttracting += Attracting;
		playerScript.OnRepulsing += Repulsing;

		playerScript.OnHold += OnHold;
		playerScript.OnShoot += Shoot;
		playerScript.OnDash += Dash;
		playerScript.OnDeath += Death;
		playerScript.OnCubeHit += CubeHit;
		playerScript.OnDashHit += DashHit;

		GlobalVariables.Instance.OnEndMode += FadeSounds;
		GlobalVariables.Instance.OnMenu += FadeSounds;
		GlobalVariables.Instance.OnPause += FadeSounds;
	}

	private bool fadeInAttraction = false;
	private bool fadeOutAttraction = false;

	void Attracting ()
	{
		if(playerScript.cubesAttracted.Count > 0 && MasterAudio.GetGroupVolume(attractingSound) != initialAttractingVolume && !fadeInAttraction)
		{
			MasterAudio.PlaySound3DFollowTransformAndForget (attractingSound, transform);
			MasterAudio.FadeSoundGroupToVolume (attractingSound, initialAttractingVolume, fadeDuration, ()=> fadeInAttraction = false);
		}

		if(playerScript.cubesAttracted.Count == 0 && MasterAudio.GetGroupVolume(attractingSound) != 0 && !fadeOutAttraction)
			MasterAudio.FadeSoundGroupToVolume (attractingSound, 0, fadeDuration, ()=> fadeOutAttraction = false);		
	}

	private bool fadeInRepulsion = false;
	private bool fadeOutRepulsion = false;

	void Repulsing ()
	{
		if(playerScript.cubesRepulsed.Count > 0 && MasterAudio.GetGroupVolume(repulsingSound) != initialRepulsingVolume && !fadeInRepulsion)
		{
			MasterAudio.PlaySound3DFollowTransformAndForget (repulsingSound, transform);
			MasterAudio.FadeSoundGroupToVolume (repulsingSound, initialRepulsingVolume, fadeDuration, ()=> fadeInRepulsion = false);
		}

		if(playerScript.cubesRepulsed.Count == 0 && MasterAudio.GetGroupVolume(repulsingSound) != 0 && !fadeOutRepulsion)
			MasterAudio.FadeSoundGroupToVolume (repulsingSound, 0, fadeDuration, ()=> fadeOutRepulsion = false);
	}

	void OnHold ()
	{
		MasterAudio.PlaySound3DFollowTransformAndForget (SoundsManager.Instance.onHoldSound, transform);
	}

	void Shoot ()
	{
		MasterAudio.PlaySound3DFollowTransformAndForget (SoundsManager.Instance.shootSound, transform);
	}

	void CubeHit ()
	{
		MasterAudio.PlaySound3DFollowTransformAndForget (SoundsManager.Instance.cubeHitSound, transform);
	}

	void DashHit ()
	{
		MasterAudio.PlaySound3DFollowTransformAndForget (SoundsManager.Instance.dashHitSound, transform);
	}

	public void StunON ()
	{
		MasterAudio.PlaySound3DFollowTransformAndForget (SoundsManager.Instance.stunONSound, transform);
	}

	public void StunOFF ()
	{
		MasterAudio.PlaySound3DFollowTransformAndForget (SoundsManager.Instance.stunOFFSound, transform);
	}

	public void StunEND ()
	{
		MasterAudio.PlaySound3DFollowTransformAndForget (SoundsManager.Instance.stunENDSound, transform);
	}

	void Dash ()
	{
		MasterAudio.PlaySound3DFollowTransformAndForget (SoundsManager.Instance.dashSound, transform);
	}

	void Death ()
	{
		MasterAudio.PlaySound3DAtVector3AndForget (SoundsManager.Instance.deathSound, transform.position);
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
