using UnityEngine;
using System.Collections;
using DarkTonic.MasterAudio;

public class PlayersSounds : MonoBehaviour
{
    public enum SoundState
    {
        NotPlaying,
        Playing,
        TransitionToPlaying,
        TransitionToNotPlaying

    }

    public float fadeDuration = 1;

    [Header("Attraction Sound")]
    public SoundState attractingSoundState = SoundState.NotPlaying;

    [Header("Repulsing Sound")]
    public SoundState repulsingSoundState = SoundState.NotPlaying;

    private PlayersGameplay playerScript;

    // Use this for initialization
    void Start()
    {
        playerScript = GetComponent<PlayersGameplay>();

        MasterAudio.PlaySound3DFollowTransformAndForget(SoundsManager.Instance.attractingSounds[(int)playerScript.playerName], transform);
        MasterAudio.PlaySound3DFollowTransformAndForget(SoundsManager.Instance.repulsingSounds[(int)playerScript.playerName], transform);

        /*playerScript.OnPlayerstateChange += Attracting;
		playerScript.OnPlayerstateChange += Repulsing;
		playerScript.OnAttracting += Attracting;
		playerScript.OnRepulsing += Repulsing;*/

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

    void Update()
    {
        if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
            return;

        Attracting();

        Repulsing();
    }

    void Attracting()
    {
        if (playerScript.cubesAttracted.Count != 0 && playerScript.playerState == PlayerState.Attracting)
        {
            if (attractingSoundState != SoundState.Playing && attractingSoundState != SoundState.TransitionToPlaying)
            {
                attractingSoundState = SoundState.TransitionToPlaying;
                MasterAudio.FadeSoundGroupToVolume(SoundsManager.Instance.attractingSounds[(int)playerScript.playerName], SoundsManager.Instance.initialAttractingVolume, fadeDuration, () => attractingSoundState = SoundState.Playing);
            }
        }
        else
        {
            if (attractingSoundState != SoundState.NotPlaying && attractingSoundState != SoundState.TransitionToNotPlaying)
            {
                attractingSoundState = SoundState.TransitionToNotPlaying;
                MasterAudio.FadeSoundGroupToVolume(SoundsManager.Instance.attractingSounds[(int)playerScript.playerName], 0, fadeDuration, () => attractingSoundState = SoundState.NotPlaying);
            }
        }
    }

    void Repulsing()
    {
        if (playerScript.cubesRepulsed.Count != 0 && playerScript.playerState == PlayerState.Repulsing)
        {
            if (repulsingSoundState != SoundState.Playing && repulsingSoundState != SoundState.TransitionToPlaying)
            {
                repulsingSoundState = SoundState.TransitionToPlaying;
                MasterAudio.FadeSoundGroupToVolume(SoundsManager.Instance.repulsingSounds[(int)playerScript.playerName], SoundsManager.Instance.initialRepulsingVolume, fadeDuration, () => repulsingSoundState = SoundState.Playing);
            }
        }
        else
        {
            if (repulsingSoundState != SoundState.NotPlaying && repulsingSoundState != SoundState.TransitionToNotPlaying)
            {
                repulsingSoundState = SoundState.TransitionToNotPlaying;
                MasterAudio.FadeSoundGroupToVolume(SoundsManager.Instance.repulsingSounds[(int)playerScript.playerName], 0, fadeDuration, () => repulsingSoundState = SoundState.NotPlaying);
            }	
        }
    }

    void OnHold()
    {
        MasterAudio.PlaySound3DFollowTransformAndForget(SoundsManager.Instance.onHoldSound, transform);
    }

    void Shoot()
    {
        MasterAudio.PlaySound3DFollowTransformAndForget(SoundsManager.Instance.shootSound, transform);
    }

    void CubeHit()
    {
        MasterAudio.PlaySound3DFollowTransformAndForget(SoundsManager.Instance.cubeHitSound, transform);
    }

    void DashHit()
    {
        MasterAudio.PlaySound3DFollowTransformAndForget(SoundsManager.Instance.dashHitSound, transform);
    }

    public void StunON()
    {
        MasterAudio.PlaySound3DFollowTransformAndForget(SoundsManager.Instance.stunONSound, transform);
    }

    public void StunOFF()
    {
        MasterAudio.PlaySound3DFollowTransformAndForget(SoundsManager.Instance.stunOFFSound, transform);
    }

    public void StunEND()
    {
        MasterAudio.PlaySound3DFollowTransformAndForget(SoundsManager.Instance.stunENDSound, transform);
    }

    void Dash()
    {
        MasterAudio.PlaySound3DFollowTransformAndForget(SoundsManager.Instance.dashSound, transform);
    }

    void Death()
    {
        MasterAudio.PlaySound3DAtVector3AndForget(SoundsManager.Instance.deathSound, transform.position);
    }

    void OnDisable()
    {
        FadeSounds();
    }

    void FadeSounds()
    {
        if (GlobalVariables.applicationIsQuitting || !playerScript)
            return;
		
        MasterAudio.FadeSoundGroupToVolume(SoundsManager.Instance.attractingSounds[(int)playerScript.playerName], 0, fadeDuration);
        MasterAudio.FadeSoundGroupToVolume(SoundsManager.Instance.repulsingSounds[(int)playerScript.playerName], 0, fadeDuration);
    }
}
