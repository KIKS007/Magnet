using UnityEngine;
using System.Collections;
using DarkTonic.MasterAudio;
using UnityEngine.UI;
using DG.Tweening;

public class GameSoundsManager : Singleton<GameSoundsManager> 
{
	[Header ("Slow Mo Effect")]
	public GameObject playListSource;
	public float timeScaleRatio = 1;
	public float slowMotweenDuration;

	[Header ("Sounds Menu")]
	public Scrollbar soundsBar;
	public Scrollbar playlistBar;
	public GameObject[] toggleMuteSounds = new GameObject[2];
	public GameObject[] toggleMuteMusic = new GameObject[2];

	[SoundGroupAttribute]
	public string masterVolumeSound;
	[SoundGroupAttribute]
	public string soundsVolumeSound;

	public float initialSoundsVolume;
	public float initialPlaylistVolume;

	void Awake ()
	{
		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<SlowMotionCamera> ().OnAllSlowMotionStop += StopSlowMoEffect;

		initialSoundsVolume = MasterAudio.MasterVolumeLevel;
		initialPlaylistVolume = MasterAudio.PlaylistMasterVolume;
	}

	void Start ()
	{
		MasterAudio.StartPlaylist ("Game");
		MasterAudio.TriggerRandomPlaylistClip ();
	}

	public void SetSoundsVolume ()
	{
		float volume = soundsBar.value * initialSoundsVolume;
		
		MasterAudio.MasterVolumeLevel = volume;
		
	}
	
	public void SetPlaylistVolume ()
	{
		
		float volume = playlistBar.value * initialPlaylistVolume;
		
		MasterAudio.PlaylistMasterVolume = volume;
		
	}

	private float previousVolumeSounds;

	public void ToggleMuteSounds ()
	{
		if(MasterAudio.MasterVolumeLevel != 0)
		{
			previousVolumeSounds = soundsBar.value;
			MasterAudio.MasterVolumeLevel = 0;
		}
	}

	public void ToggleMuteMusic ()
	{

	}

	public void StartSlowMoEffect (float whichSlowFactor)
	{
		AudioSource[] audioSourceComponents = playListSource.GetComponents<AudioSource> ();
		AudioSource playlistAudioSource = new AudioSource();

		for(int i = 0; i < audioSourceComponents.Length; i++)
		{
			if (audioSourceComponents [i].volume != 0)
				playlistAudioSource = audioSourceComponents [i];
		}

		playlistAudioSource.DOPitch ((playlistAudioSource.pitch / whichSlowFactor) * timeScaleRatio, slowMotweenDuration).SetEase(Ease.OutQuad);
	}

	public void StopSlowMoEffect ()
	{
		AudioSource[] audioSourceComponents = playListSource.GetComponents<AudioSource> ();
		AudioSource playlistAudioSource = new AudioSource();

		for(int i = 0; i < audioSourceComponents.Length; i++)
		{
			if (audioSourceComponents [i].volume != 0)
				playlistAudioSource = audioSourceComponents [i];
		}

		playlistAudioSource.DOPitch (1, slowMotweenDuration).SetEase(Ease.OutQuad);
	}
}
