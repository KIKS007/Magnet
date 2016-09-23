using UnityEngine;
using System.Collections;
using DarkTonic.MasterAudio;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameSoundsManager : Singleton<GameSoundsManager> 
{
	[Header ("Slow Mo Effect")]
	public bool slowMoEffectMusicEnabled = true;
	public Toggle toggleEnableSloMoEffect;
	public GameObject playListSource;
	public float timeScaleRatio = 1;
	public float slowMotweenDuration;

	[Header ("Sounds Menu")]
	public Scrollbar soundsBar;
	public Scrollbar playlistBar;
	public GameObject[] toggleMuteSounds = new GameObject[2];
	public GameObject[] toggleMuteMusic = new GameObject[2];
	public bool soundsMute = false;
	public bool musicMute = false;

	[SoundGroupAttribute]
	public string soundsVolumeTest;

	private float initialSoundsVolume;
	private float initialPlaylistVolume;

	private bool canPlaySoundTest = true;

	private bool muting = false;
	private float previousVolumeSounds;
	private float previousVolumePlaylist;

	private bool loading = true;

	void Start ()
	{
		initialSoundsVolume = MasterAudio.MasterVolumeLevel;
		initialPlaylistVolume = MasterAudio.PlaylistMasterVolume;

		if (PlayerPrefs.HasKey ("SlowMotionEffectEnable") && SceneManager.GetActiveScene().name != "Scene Testing")
			LoadPlayersPrefs ();

		else if(SceneManager.GetActiveScene().name != "Scene Testing")
		{
			toggleMuteSounds [0].SetActive (true);
			toggleMuteSounds [1].SetActive (false);

			toggleMuteMusic [0].SetActive (true);
			toggleMuteMusic [1].SetActive (false);

			toggleEnableSloMoEffect.isOn = true;
		}


		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<SlowMotionCamera> ().OnAllSlowMotionStop += StopSlowMoEffect;

		MasterAudio.StartPlaylist ("Game");
		MasterAudio.TriggerRandomPlaylistClip ();
	}

	public void SetSoundsVolume ()
	{
		if(!loading)
		{
			if(toggleMuteSounds[0].activeSelf == false && !muting)
			{
				toggleMuteSounds [0].SetActive (true);
				toggleMuteSounds [1].SetActive (false);
			}
			
			float volume = soundsBar.value * initialSoundsVolume;
			
			MasterAudio.MasterVolumeLevel = volume;
			
			if(canPlaySoundTest)
			{
				MasterAudio.PlaySound3DAtVector3 (soundsVolumeTest, Vector3.zero);
				StartCoroutine (PlaySoundWait ());
			}
		}		
	}

	IEnumerator PlaySoundWait ()
	{
		canPlaySoundTest = false;

		yield return new WaitForSeconds (0.35f);

		canPlaySoundTest = true;
	}
		
	public void SetPlaylistVolume ()
	{
		if(!loading)
		{
			if(toggleMuteMusic[0].activeSelf == false && !muting)
			{
				toggleMuteMusic [0].SetActive (true);
				toggleMuteMusic [1].SetActive (false);
			}
			
			float volume = playlistBar.value * initialPlaylistVolume;
			
			MasterAudio.PlaylistMasterVolume = volume;		
		}
	}

	public void ToggleMuteSounds ()
	{
		if(toggleMuteSounds [0].activeSelf == true)
		{
			muting = true;
			soundsMute = true;
			previousVolumeSounds = soundsBar.value;
			toggleMuteSounds [0].SetActive (false);
			toggleMuteSounds [1].SetActive (true);
			DOTween.To(()=> soundsBar.value, x=> soundsBar.value =x, 0, 0.2f).OnComplete(()=> muting = false);
		}
		else
		{
			muting = true;
			soundsMute = false;
			toggleMuteSounds [0].SetActive (true);
			toggleMuteSounds [1].SetActive (false);
			DOTween.To(()=> soundsBar.value, x=> soundsBar.value =x, previousVolumeSounds, 0.2f).OnComplete(()=> muting = false);
		}
	}

	public void ToggleMuteMusic ()
	{
		if(toggleMuteMusic [0].activeSelf == true)
		{
			muting = true;
			musicMute = true;
			previousVolumePlaylist = playlistBar.value;
			toggleMuteMusic [0].SetActive (false);
			toggleMuteMusic [1].SetActive (true);
			DOTween.To(()=> playlistBar.value, x=> playlistBar.value =x, 0, 0.2f).OnComplete(()=> muting = false);
		}
		else
		{
			muting = true;
			musicMute = false;
			toggleMuteMusic [0].SetActive (true);
			toggleMuteMusic [1].SetActive (false);
			DOTween.To(()=> playlistBar.value, x=> playlistBar.value =x, previousVolumePlaylist, 0.2f).OnComplete(()=> muting = false);
		}
	}

	public void StartSlowMoEffect (float whichSlowFactor)
	{
		if(slowMoEffectMusicEnabled) 
		{
			AudioSource[] audioSourceComponents = playListSource.GetComponents<AudioSource> ();
			AudioSource playlistAudioSource = new AudioSource ();

			for (int i = 0; i < audioSourceComponents.Length; i++) {
				if (audioSourceComponents [i].volume != 0)
					playlistAudioSource = audioSourceComponents [i];
			}

			if(playlistAudioSource != null)
				playlistAudioSource.DOPitch ((playlistAudioSource.pitch / whichSlowFactor) * timeScaleRatio, slowMotweenDuration).SetEase (Ease.OutQuad);
		}
	}

	public void StopSlowMoEffect ()
	{
		if(slowMoEffectMusicEnabled) 
		{
			AudioSource[] audioSourceComponents = playListSource.GetComponents<AudioSource> ();
			AudioSource playlistAudioSource = new AudioSource ();

			for (int i = 0; i < audioSourceComponents.Length; i++) {
				if (audioSourceComponents [i].volume != 0)
					playlistAudioSource = audioSourceComponents [i];
			}

			playlistAudioSource.DOPitch (1, slowMotweenDuration).SetEase (Ease.OutQuad);
		}
	}

	public void ToggleSlowMoEffect ()
	{
		if(slowMoEffectMusicEnabled == true)
		{
			slowMoEffectMusicEnabled = false;
		}
		else
		{
			slowMoEffectMusicEnabled = true;
		}
	}

	public override void OnDestroy ()
	{
		if(SceneManager.GetActiveScene().name != "Scene Testing")
			SavePlayerPrefs ();
		
		//Debug.Log ("Data Saved");

		base.OnDestroy ();
	}

	void SavePlayerPrefs ()
	{
		PlayerPrefs.SetInt ("SlowMotionEffectEnable", slowMoEffectMusicEnabled ? 1 : 0);

		if(soundsMute)
		{
			PlayerPrefs.SetInt ("SoundsMute", 1);
			PlayerPrefs.SetFloat ("PreviousVolumeSounds", (float)previousVolumeSounds);
		}
		else
		{
			PlayerPrefs.SetInt ("SoundsMute", 0);
			PlayerPrefs.SetFloat ("SoundsVolume", (float)soundsBar.value);
		}

		if(musicMute)
		{
			PlayerPrefs.SetInt ("MusicMute", 1);
			PlayerPrefs.SetFloat ("PreviousVolumePlaylist", (float)previousVolumePlaylist);
		}
		else
		{
			PlayerPrefs.SetInt ("MusicMute", 0);
			PlayerPrefs.SetFloat ("MusicVolume", (float)playlistBar.value);
		}
	}

	void LoadPlayersPrefs ()
	{
		//Debug.Log ("Data Loaded");
		loading = true;

		if(PlayerPrefs.GetInt ("SlowMotionEffectEnable") == 1)
		{
			slowMoEffectMusicEnabled = true;
			toggleEnableSloMoEffect.isOn = true;
		}
		else
		{
			slowMoEffectMusicEnabled = false;
			toggleEnableSloMoEffect.isOn = false;
		}

		if(PlayerPrefs.GetInt ("SoundsMute") == 1)
		{
			soundsMute = true;
			previousVolumeSounds = PlayerPrefs.GetFloat("PreviousVolumeSounds");
			toggleMuteSounds [0].SetActive (false);
			toggleMuteSounds [1].SetActive (true);
			soundsBar.value = 0;
			MasterAudio.MasterVolumeLevel = 0;
		}
		else
		{
			soundsMute = false;
			toggleMuteSounds [0].SetActive (true);
			toggleMuteSounds [1].SetActive (false);
			soundsBar.value = PlayerPrefs.GetFloat("SoundsVolume");
			MasterAudio.MasterVolumeLevel = PlayerPrefs.GetFloat("SoundsVolume");
		}

		if(PlayerPrefs.GetInt ("MusicMute") == 1)
		{
			musicMute = true;
			previousVolumePlaylist = PlayerPrefs.GetFloat("PreviousVolumePlaylist");
			toggleMuteMusic [0].SetActive (false);
			toggleMuteMusic [1].SetActive (true);
			playlistBar.value = 0;
			MasterAudio.PlaylistMasterVolume = 0;
		}
		else
		{
			toggleMuteMusic [0].SetActive (true);
			toggleMuteMusic [1].SetActive (false);
			playlistBar.value = PlayerPrefs.GetFloat("MusicVolume");
			MasterAudio.PlaylistMasterVolume = PlayerPrefs.GetFloat("MusicVolume");
		}

		loading = false;
	}
}
