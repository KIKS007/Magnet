using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using GameAnalyticsSDK;
using System.IO;
using System.Linq;
using UnityEngine.Audio;

public class SoundsManager : Singleton<SoundsManager> 
{
	public PlaylistController playlistCont;

	[Header ("Loaded Musics")]
	public bool playLoadedMusics;
	public List<AudioClip> loadedMusics = new List<AudioClip> ();

	[Header ("Low Pass")]
	public float lowPassTweenDuration;
	[Range (10, 22000)]
	public float startScreenLowPass;
	[Range (10, 22000)]
	public float pauseLowPass;
	[Range (10, 22000)]
	public float gameOverLowPass;

	[Header ("High Pass")]
	public float highPassTweenDuration;
	[Range (10, 22000)]
	public float sloMoHighPass;

	[Header ("Distortion")]
	[Range (0, 1)]
	public float distortion = 0.08f;

	[Header ("Menu Sounds")]
	[SoundGroupAttribute]
	public string menuSubmit;
	[SoundGroupAttribute]
	public string menuCancel;
	[SoundGroupAttribute]
	public string menuNavigation;
	[SoundGroupAttribute]
	public string gameStartSound;
	[SoundGroupAttribute]
	public string openMenuSound;
	[SoundGroupAttribute]
	public string closeMenuSound;
	[SoundGroupAttribute]
	public string gamepadDisconnectionSound;
	[SoundGroupAttribute]
	public string winSound;

	[Header ("Player Sounds")]
	[SoundGroupAttribute]
	public string onHoldSound;
	[SoundGroupAttribute]
	public string shootSound;
	[SoundGroupAttribute]
	public string cubeHitSound;
	[SoundGroupAttribute]
	public string dashHitSound;
	[SoundGroupAttribute]
	public string stunONSound;
	[SoundGroupAttribute]
	public string stunOFFSound;
	[SoundGroupAttribute]
	public string stunENDSound;
	[SoundGroupAttribute]
	public string dashSound;
	[SoundGroupAttribute]
	public string deathSound;

	[Header ("Cube Sounds")]
	[SoundGroupAttribute]
	public string wallHitSound;
	[SoundGroupAttribute]
	public string cubeSpawnSound;
	[SoundGroupAttribute]
	public string lastSecondsSound;
	[SoundGroupAttribute]
	public string cubeTrackingSound;

	[Header ("Explosion Sound")]
	[SoundGroupAttribute]
	public string explosionSound;

	[Header ("Sounds Options")]
	public Scrollbar soundsBar;
	public Scrollbar playlistBar;
	public bool soundsMute = false;
	public Toggle soundsMuteToggle;
	public bool musicMute = false;
	public Toggle musicMuteToggle;
	[SoundGroupAttribute]
	public string soundsVolumeTest;

	[Header ("Slow Mo Effect")]
	public bool slowMoEffectMusicEnabled = false;
	public Toggle toggleEnableSloMoEffect;
	public GameObject playListSource;
	public float timeScaleRatio = 1;
	public float slowMotweenDuration;

	private float initialSoundsVolume;
	private float initialPlaylistVolume;

	private bool canPlaySoundTest = true;

	private float previousVolumeSounds;
	private float previousVolumePlaylist;

	private bool loading = false;

	private FileInfo[] musicsFiles;
	private List<string> validExtensions = new List<string> { ".ogg", ".wav", ".mp3" };
	private string loadedMusicsPath = "/Musics";
	private string editorLoadedMusicsPath = "./Assets/SOUNDS/Loaded Musics";

	private float initialLowPassFrequency;
	private AudioLowPassFilter lowPass;
	private float initialHighPassFrequency;
	private AudioHighPassFilter highPass;

	private SlowMotionCamera slowMo;

	public event EventHandler OnMusicVolumeChange;
	public event EventHandler OnSoundsVolumeChange;

	void Start ()
	{
		LoadMusics ();

		lowPass = playlistCont.gameObject.GetComponent<AudioLowPassFilter> ();
		highPass = playlistCont.gameObject.GetComponent<AudioHighPassFilter> ();
		initialLowPassFrequency = lowPass.cutoffFrequency;
		initialHighPassFrequency = highPass.cutoffFrequency;
			
		initialSoundsVolume = MasterAudio.MasterVolumeLevel;
		initialPlaylistVolume = MasterAudio.PlaylistMasterVolume;

		slowMo = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<SlowMotionCamera> ();

		StartCoroutine (MusicVolumeChange ());
		StartCoroutine (SoundsVolumeChange ());

		if (PlayerPrefs.HasKey ("SlowMotionEffectEnable") && SceneManager.GetActiveScene().name != "Scene Testing")
			LoadPlayersPrefs ();

		else if(SceneManager.GetActiveScene().name != "Scene Testing")
		{
			toggleEnableSloMoEffect.isOn = false;
		}

		GlobalVariables.Instance.OnStartMode += SetGamePlaylist;
		GlobalVariables.Instance.OnMenu += SetMenuPlaylist;
		GlobalVariables.Instance.OnEndMode += () => MasterAudio.PlaySound(winSound, 1, 1, 1.5f);

		GlobalVariables.Instance.OnMenu += ()=> { if(!MenuManager.Instance.startScreen) ResetLowPass(); };
		GlobalVariables.Instance.OnPlaying += ResetLowPass;
		GlobalVariables.Instance.OnPause += ()=> LowPass (pauseLowPass);
		GlobalVariables.Instance.OnEndMode += ()=> LowPass (gameOverLowPass);

		slowMo.OnSlowMotionStart += () => HighPass (sloMoHighPass);
		slowMo.OnSlowMotionStop += ResetHighPass;

		OnMusicVolumeChange += UpdateAudioSettings;
		OnSoundsVolumeChange += UpdateAudioSettings;

		UpdateAudioSettings ();

		LowPass (startScreenLowPass, 0.5f);

		//MasterAudio.StartPlaylist ("Game");
		//MasterAudio.TriggerRandomPlaylistClip ();
	}

	void Update ()
	{
		for(int i = 0; i < 2; i++)
		{
			if (GlobalVariables.Instance.rewiredPlayers [i].GetButtonDown ("Random Music"))
				RandomMusic ();
			
			if (GlobalVariables.Instance.rewiredPlayers [i].GetButtonDown ("Next Music"))
				NextMusic ();
			
			if (GlobalVariables.Instance.rewiredPlayers [i].GetButton ("Volume Up"))
				MusicVolumeUp ();
			
			if (GlobalVariables.Instance.rewiredPlayers [i].GetButton ("Volume Down"))
				MusicVolumeDown ();
		}
	}

	void LoadMusics ()
	{
		if (Application.isEditor)
			loadedMusicsPath = editorLoadedMusicsPath;
		else
			loadedMusicsPath = Application.dataPath + loadedMusicsPath;

		if(!Directory.Exists (loadedMusicsPath))
		{
			Directory.CreateDirectory (loadedMusicsPath);
			Debug.LogWarning ("Musics folder don't exists!");
			return;
		}

		loadedMusics.Clear ();

		var info = new DirectoryInfo (loadedMusicsPath);

		musicsFiles = info.GetFiles ()
			.Where (f => IsValidFileType (f.Name))
			.ToArray ();

		if(musicsFiles.Length == 0)
		{
			Debug.LogWarning ("No (valid) Musics in folder!");
			return;
		}

		foreach (var s in musicsFiles)
		{
			if(Path.GetExtension (s.Name).Contains (".mp3"))
				StartCoroutine (LoadMP3File (s.FullName));
			else
				StartCoroutine (LoadFile (s.FullName));
		}
	}
	
	bool IsValidFileType (string fileName)
	{
		return validExtensions.Contains (Path.GetExtension (fileName));
	}

	IEnumerator LoadFile (string path)
	{
		WWW www = new WWW ("file://" + path);
		AudioClip clip = www.GetAudioClip();

		//Debug.Log ("loading " + path);
		yield return www;

		if (clip.loadState == AudioDataLoadState.Unloaded)
			yield break;

		if (www.error != null)
		{
			Debug.Log (www.error);
			yield break;
		}
		
		clip = www.GetAudioClip (false, false);
		clip.LoadAudioData ();

		if (clip.loadState == AudioDataLoadState.Failed)
			Debug.LogError ("Unable to load file: " + path);
		
		else
		{
			//Debug.Log ("done loading " + path);
			clip.name = Path.GetFileName (path);

			loadedMusics.Add (clip);

			MasterAudio.AddSongToPlaylist ("Loaded Musics", clip);
		}
	}

	IEnumerator LoadMP3File (string path)
	{
		WWW www = new WWW ("file://" + path);

		//Debug.Log ("loading " + path);
		yield return www;

		if (www.error != null)
		{
			Debug.Log (www.error);
			yield break;
		}

		AudioClip clip = NAudioPlayer.FromMp3Data (www.bytes);
		clip.LoadAudioData ();

		if (clip.loadState == AudioDataLoadState.Failed)
			Debug.LogError ("Unable to load file: " + path);

		else
		{
			//Debug.Log ("done loading " + path);
			clip.name = Path.GetFileName (path);

			loadedMusics.Add (clip);

			MasterAudio.AddSongToPlaylist ("Loaded Musics", clip);
		}
	}

	void RandomMusic ()
	{
		MasterAudio.TriggerRandomPlaylistClip ();
	}

	void NextMusic ()
	{
		MasterAudio.TriggerNextPlaylistClip ();
	}

	void MusicVolumeUp ()
	{
		if(MasterAudio.PlaylistMasterVolume < initialPlaylistVolume)
			MasterAudio.PlaylistMasterVolume += 0.02f;
		else
			MasterAudio.PlaylistMasterVolume = initialPlaylistVolume;

		UpdateAudioSettings ();
	}

	void MusicVolumeDown ()
	{
		if(MasterAudio.PlaylistMasterVolume > 0)
			MasterAudio.PlaylistMasterVolume -= 0.02f;
		
		else
			MasterAudio.PlaylistMasterVolume = 0;

		UpdateAudioSettings ();
	}

	public void SetGamePlaylist ()
	{
		string gamePlaylist = "Game";

		if(playLoadedMusics && loadedMusics.Count != 0)
			gamePlaylist = "Loaded Musics";

		if(playlistCont.PlaylistName != gamePlaylist)
		{
			MasterAudio.ChangePlaylistByName (gamePlaylist, false);
			playlistCont.PlayRandomSong ();
		}
	}

	public void SetMenuPlaylist ()
	{
		return;

		if(playlistCont.PlaylistName != "Menu Ambient")
		{
			MasterAudio.ChangePlaylistByName ("Menu Ambient", true);
		}
	}

	public void MenuSubmit ()
	{
		MasterAudio.PlaySound (menuSubmit);
	}

	public void MenuCancel ()
	{
		MasterAudio.PlaySound (menuCancel);
	}

	public void MenuNavigation ()
	{
		MasterAudio.PlaySound (menuNavigation);
	}

	public void SetSoundsVolume ()
	{
		if(!loading)
		{			
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

	void UpdateAudioSettings ()
	{
		if (MasterAudio.MasterVolumeLevel == 0)
			soundsMuteToggle.isOn = true;
		else
			soundsMuteToggle.isOn = false;

		if (MasterAudio.PlaylistMasterVolume == 0)
			musicMuteToggle.isOn = true;
		else
			musicMuteToggle.isOn = false;

		playlistBar.value = MasterAudio.PlaylistMasterVolume * initialPlaylistVolume;

		soundsBar.value = MasterAudio.MasterVolumeLevel * initialPlaylistVolume;
	}

	public void SetPlaylistVolume ()
	{
		if(!loading)
		{
			float volume = playlistBar.value * initialPlaylistVolume;
			
			MasterAudio.PlaylistMasterVolume = volume;		
		}
	}

	public void ToggleMuteSounds ()
	{
		if(MasterAudio.MasterVolumeLevel != 0)
		{
			previousVolumeSounds = MasterAudio.MasterVolumeLevel;
			DOTween.To(()=> MasterAudio.MasterVolumeLevel, x=> MasterAudio.MasterVolumeLevel =x, 0, 0.2f);
		}
		else
		{
			if(previousVolumeSounds != 0)
				DOTween.To(()=> MasterAudio.MasterVolumeLevel, x=> MasterAudio.MasterVolumeLevel =x, previousVolumeSounds, 0.2f);
			else
				DOTween.To(()=> MasterAudio.MasterVolumeLevel, x=> MasterAudio.MasterVolumeLevel =x, initialSoundsVolume, 0.2f);
		}
	}

	public void ToggleMuteMusic ()
	{
		if(MasterAudio.PlaylistMasterVolume != 0)
		{
			previousVolumePlaylist = MasterAudio.PlaylistMasterVolume;
			DOTween.To(()=> MasterAudio.PlaylistMasterVolume, x=> MasterAudio.PlaylistMasterVolume =x, 0, 0.2f);
		}
		else
		{
			if(previousVolumePlaylist != 0)
				DOTween.To(()=> MasterAudio.PlaylistMasterVolume, x=> MasterAudio.PlaylistMasterVolume =x, previousVolumePlaylist, 0.2f);
			else
				DOTween.To(()=> MasterAudio.PlaylistMasterVolume, x=> MasterAudio.PlaylistMasterVolume =x, initialPlaylistVolume, 0.2f);
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

	public void LowPass (float lowPassFrquency, float duration)
	{
		if (!slowMoEffectMusicEnabled)
			return;

		playlistCont.mixerChannel.audioMixer.DOSetFloat ("LowPass", lowPassFrquency, duration).SetEase (Ease.OutQuad).SetId ("LowPass");
		playlistCont.mixerChannel.audioMixer.DOSetFloat ("Distortion", distortion, duration).SetEase (Ease.OutQuad).SetId ("LowPass");
	}

	public void LowPass (float lowPassFrquency)
	{
		if (!slowMoEffectMusicEnabled)
			return;

		playlistCont.mixerChannel.audioMixer.DOSetFloat ("LowPass", lowPassFrquency, lowPassTweenDuration).SetEase (Ease.OutQuad).SetId ("LowPass");
		playlistCont.mixerChannel.audioMixer.DOSetFloat ("Distortion", distortion, lowPassTweenDuration).SetEase (Ease.OutQuad).SetId ("LowPass");
	}

	public void ResetLowPass ()
	{
		playlistCont.mixerChannel.audioMixer.DOSetFloat ("LowPass", initialLowPassFrequency, lowPassTweenDuration).SetEase (Ease.OutQuad).SetId ("LowPass");
		playlistCont.mixerChannel.audioMixer.DOSetFloat ("Distortion", 0, lowPassTweenDuration).SetEase (Ease.OutQuad).SetId ("LowPass");
	}

	public void ResetLowPass (float duration)
	{
		playlistCont.mixerChannel.audioMixer.DOSetFloat ("LowPass", initialLowPassFrequency, duration).SetEase (Ease.OutQuad).SetId ("LowPass");
		playlistCont.mixerChannel.audioMixer.DOSetFloat ("Distortion", 0, duration).SetEase (Ease.OutQuad).SetId ("LowPass");
	}

	public void HighPass (float frequency)
	{
		if (!slowMoEffectMusicEnabled)
			return;

		playlistCont.mixerChannel.audioMixer.DOSetFloat ("HighPass", frequency, highPassTweenDuration).SetEase (Ease.OutQuad).SetId ("HighPass");
		playlistCont.mixerChannel.audioMixer.DOSetFloat ("Distortion", distortion, highPassTweenDuration).SetEase (Ease.OutQuad).SetId ("HighPass");
	}

	public void ResetHighPass ()
	{
		playlistCont.mixerChannel.audioMixer.DOSetFloat ("HighPass", initialHighPassFrequency, highPassTweenDuration).SetEase (Ease.OutQuad).SetId ("HighPass");
		playlistCont.mixerChannel.audioMixer.DOSetFloat ("Distortion", 0, highPassTweenDuration).SetEase (Ease.OutQuad).SetId ("HighPass");
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
		Debug.Log ("Data Saved");

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
		Debug.Log ("Data Loaded");
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
			soundsBar.value = 0;
			MasterAudio.MasterVolumeLevel = 0;
			soundsMuteToggle.isOn = true;
		}
		else
		{
			soundsMute = false;
			soundsBar.value = PlayerPrefs.GetFloat("SoundsVolume");
			MasterAudio.MasterVolumeLevel = PlayerPrefs.GetFloat("SoundsVolume");
			soundsMuteToggle.isOn = false;
		}
	
		if(PlayerPrefs.GetInt ("MusicMute") == 1)
		{
			musicMute = true;
			previousVolumePlaylist = PlayerPrefs.GetFloat("PreviousVolumePlaylist");
			playlistBar.value = 0;
			MasterAudio.PlaylistMasterVolume = 0;
			musicMuteToggle.isOn = true;
		}
		else
		{
			playlistBar.value = PlayerPrefs.GetFloat("MusicVolume");
			MasterAudio.PlaylistMasterVolume = PlayerPrefs.GetFloat("MusicVolume");
			musicMuteToggle.isOn = false;
		}

		loading = false;
	}

	void OnApplicationQuit ()
	{
		float soundsMuteTemp = soundsMute ? 1f : 0f;
		float musicMuteTemp = musicMute ? 1f : 0f;

		GameAnalytics.NewDesignEvent ("Menu:" + "Options:" + "Sounds:" + "SoundsMute", soundsMuteTemp);
		GameAnalytics.NewDesignEvent ("Menu:" + "Options:" + "Sounds:" + "MuteMusic", musicMuteTemp);
	}


	IEnumerator MusicVolumeChange ()
	{
		if (OnMusicVolumeChange != null)
			OnMusicVolumeChange ();

		float volume = MasterAudio.PlaylistMasterVolume;

		yield return new WaitUntil (() => volume != MasterAudio.PlaylistMasterVolume);

		StartCoroutine (MusicVolumeChange ());
	}

	IEnumerator SoundsVolumeChange ()
	{
		if (OnSoundsVolumeChange != null)
			OnSoundsVolumeChange ();

		float volume = MasterAudio.MasterVolumeLevel;

		yield return new WaitUntil (() => volume != MasterAudio.MasterVolumeLevel);

		StartCoroutine (SoundsVolumeChange ());
	}
}
