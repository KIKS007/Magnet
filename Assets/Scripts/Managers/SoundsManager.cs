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

	[Header ("Master Volume")]
	public float masterVolume = 1f;
	public bool masterMute = false;
	public Scrollbar masterBar;
	public Toggle masterMuteToggle;

	[Header ("Sounds Options")]
	public Scrollbar soundsBar;
	public Scrollbar playlistBar;
	public bool soundsMute = false;
	public Toggle soundsMuteToggle;
	public bool musicMute = false;
	public Toggle musicMuteToggle;
	[SoundGroupAttribute]
	public string soundsVolumeTest;

	[Header ("Songs Titles")]
	public Text currentSong;
	public GameObject songTitlePrefab;
	public GameObject songTitleContentParent;
	public GameObject loadedSongTitleContentParent;
	public float initialYPos = -36f;
	public float gapHeight = 110f;

	[Header ("Loaded Musics")]
	public Text loadButtonText;
	public GameObject canTakeTime;
	public bool loadingMusics = false;
	public bool playLoadedMusics;
	public List<AudioClip> loadedMusics = new List<AudioClip> ();

	[Header ("Low Pass")]
	public bool lowPassEnabled = true;
	public Toggle lowPassToggle;
	public float lowPassTweenDuration;
	[Range (10, 22000)]
	public float pauseLowPass;
	[Range (10, 22000)]
	public float gameOverLowPass;

	[Header ("High Pass")]
	public bool highPassEnabled = true;
	public Toggle highPassToggle;
	public float highPassTweenDuration;
	[Range (10, 22000)]
	public float sloMoHighPass;

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

	private bool canPlaySoundTest = true;

	private float previousVolumeSounds;
	private float previousVolumePlaylist;
	private float previousMasterVolume;

	private bool loading = false;

	private FileInfo[] musicsFiles;
	private List<string> validExtensions = new List<string> { ".ogg", ".wav", ".mp3" };
	private string loadedMusicsPath = "/Musics";
	private string editorLoadedMusicsPath = "./Assets/SOUNDS/Loaded Musics";
	private List<string> loadingMusicsList = new List<string> ();

	private SlowMotionCamera slowMo;

	public event EventHandler OnMusicVolumeChange;
	public event EventHandler OnSoundsVolumeChange;
	public event EventHandler OnMasterVolumeChange;

	void Start ()
	{
		//LoadMusics ();
		SetGamePlaylist ();

		canTakeTime.SetActive (false);

		slowMo = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<SlowMotionCamera> ();

		if (PlayerPrefs.HasKey ("LowPassEnabled") && SceneManager.GetActiveScene().name != "Scene Testing")
			LoadPlayersPrefs ();
		
		StartCoroutine (MusicVolumeChange ());
		StartCoroutine (SoundsVolumeChange ());
		StartCoroutine (MasterVolumeChange ());

		GlobalVariables.Instance.OnStartMode += SetGamePlaylist;
		GlobalVariables.Instance.OnMenu += SetMenuPlaylist;
		GlobalVariables.Instance.OnEndMode += () => MasterAudio.PlaySound(winSound, 1, 1, 1.5f);

		GlobalVariables.Instance.OnMenu += ()=> { if(!MenuManager.Instance.startScreen) ResetLowPass(); };

		GlobalVariables.Instance.OnPlaying += ResetLowPass;
		GlobalVariables.Instance.OnPlaying += ResetHighPass;

		GlobalVariables.Instance.OnPause += ()=> LowPass (pauseLowPass);
		GlobalVariables.Instance.OnEndMode += ()=> LowPass (gameOverLowPass);

		slowMo.OnSlowMotionStart += () => HighPass (sloMoHighPass);
		slowMo.OnSlowMotionStop += ResetHighPass;

		OnMusicVolumeChange += UpdateAudioSettings;
		OnSoundsVolumeChange += UpdateAudioSettings;
		OnMasterVolumeChange += UpdateAudioSettings;

		playlistCont.SongChanged += (newSongName) => currentSong.text = newSongName;

		UpdateAudioSettings ();

		LowPass (pauseLowPass, 0.5f);

		CreateMusicsSongTitle ();

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

	public void LoadMusics ()
	{
		StartCoroutine (LoadMusicsCoroutine ());
	}

	IEnumerator LoadMusicsCoroutine ()
	{
		loadingMusics = true;

		loadButtonText.text = "Loading...";
		canTakeTime.SetActive (true);

		MasterAudio.GrabPlaylist ("Loaded Musics", false).MusicSettings.Clear ();

		if (Application.isEditor)
			loadedMusicsPath = editorLoadedMusicsPath;
		else
			loadedMusicsPath = Application.dataPath + loadedMusicsPath;

		if(!Directory.Exists (loadedMusicsPath))
		{
			Directory.CreateDirectory (loadedMusicsPath);
			Debug.LogWarning ("Musics folder don't exists!");
		}

		loadedMusics.Clear ();

		var info = new DirectoryInfo (loadedMusicsPath);

		musicsFiles = info.GetFiles ()
			.Where (f => IsValidFileType (f.Name))
			.ToArray ();

		if(musicsFiles.Length == 0)
		{
			Debug.LogWarning ("No (valid) Musics in folder!");
			SetGamePlaylist ();
			yield break;
		}

		for(int i = 0; i < musicsFiles.Length; i++)
		{
			loadingMusicsList.Add (musicsFiles[i].FullName);

			if(Path.GetExtension (musicsFiles[i].Name).Contains (".mp3"))
				StartCoroutine (LoadMP3File (musicsFiles[i].FullName));
			else
				StartCoroutine (LoadFile (musicsFiles[i].FullName));
		}

		yield return new WaitUntil (() => loadingMusicsList.Count == 0);

		loadingMusics = false;

		loadButtonText.text = "Load";
		canTakeTime.SetActive (false);

		SetGamePlaylist ();

		CreateLoadedMusicsSongTitle ();
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

		loadingMusicsList.Remove (path);
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

		loadingMusicsList.Remove (path);
	}

	void CreateMusicsSongTitle ()
	{
		foreach (Transform t in songTitleContentParent.transform)
			Destroy (t.gameObject, 0.05f);

		for(int i = 0; i < MasterAudio.GrabPlaylist ("Game", false).MusicSettings.Count; i++)
		{
			Vector3 pos = songTitlePrefab.GetComponent<RectTransform> ().anchoredPosition3D;
			pos.y = -gapHeight * i + initialYPos;
			pos.z = 0;
			
			GameObject songTitle = Instantiate (songTitlePrefab, songTitlePrefab.transform.position, songTitlePrefab.transform.rotation, songTitleContentParent.transform);
			songTitle.GetComponent<RectTransform> ().anchoredPosition3D = pos;

			songTitle.GetComponent<Button> ().onClick.AddListener (()=> PlayGameSong (songTitle.transform.GetChild (0).GetComponent<Text> ().text));

			songTitle.transform.GetChild (0).GetComponent<Text> ().text = MasterAudio.GrabPlaylist ("Game", false).MusicSettings [i].clip.name;
			songTitle.transform.GetChild (1).GetComponent<Text> ().text = (i + 1).ToString () + ".";
			songTitle.transform.GetChild (2).GetComponent<Text> ().text = 
				((int) (MasterAudio.GrabPlaylist ("Game", false).MusicSettings [i].clip.length / 60)).ToString ("D")
				+ ":" + 
				((int)(MasterAudio.GrabPlaylist ("Game", false).MusicSettings [i].clip.length % 60)).ToString ("D2");
		}

		songTitleContentParent.GetComponent<RectTransform> ().sizeDelta = new Vector2 (songTitleContentParent.GetComponent<RectTransform> ().sizeDelta.x, MasterAudio.GrabPlaylist ("Game", false).MusicSettings.Count * gapHeight);
	}

	void CreateLoadedMusicsSongTitle ()
	{
		foreach (Transform t in loadedSongTitleContentParent.transform)
			Destroy (t.gameObject, 0.05f);

		for(int i = 0; i < MasterAudio.GrabPlaylist ("Loaded Musics", false).MusicSettings.Count; i++)
		{
			Vector3 pos = songTitlePrefab.GetComponent<RectTransform> ().anchoredPosition3D;
			pos.y = -gapHeight * i + initialYPos;
			pos.z = 0;

			GameObject songTitle = Instantiate (songTitlePrefab, songTitlePrefab.transform.position, songTitlePrefab.transform.rotation, loadedSongTitleContentParent.transform);
			songTitle.GetComponent<RectTransform> ().anchoredPosition3D = pos;

			songTitle.GetComponent<Button> ().onClick.AddListener (()=> PlayLoadedSong (songTitle.transform.GetChild (0).GetComponent<Text> ().text));

			songTitle.transform.GetChild (0).GetComponent<Text> ().text = MasterAudio.GrabPlaylist ("Loaded Musics", false).MusicSettings [i].clip.name;
			songTitle.transform.GetChild (1).GetComponent<Text> ().text = (i + 1).ToString () + ".";
			songTitle.transform.GetChild (2).GetComponent<Text> ().text = 
				((int) (MasterAudio.GrabPlaylist ("Loaded Musics", false).MusicSettings [i].clip.length / 60)).ToString ("D")
				+ ":" + 
				((int)(MasterAudio.GrabPlaylist ("Loaded Musics", false).MusicSettings [i].clip.length % 60)).ToString ("D2");
		}

		loadedSongTitleContentParent.GetComponent<RectTransform> ().sizeDelta = new Vector2 (loadedSongTitleContentParent.GetComponent<RectTransform> ().sizeDelta.x, MasterAudio.GrabPlaylist ("Loaded Musics", false).MusicSettings.Count * gapHeight);
	}

	public void PlayPlaylist (string playlist)
	{
		if(playlist == "Game")
			playLoadedMusics = false;

		else if(playlist == "Loaded Musics")
			playLoadedMusics = true;

		SetGamePlaylist ();
	}

	public void PlayGameSong (string song)
	{
		PlayPlaylist ("Game");

		playlistCont.TriggerPlaylistClip (song);
	}

	public void PlayLoadedSong (string song)
	{
		PlayPlaylist ("Loaded Musics");

		playlistCont.TriggerPlaylistClip (song);
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
		MasterAudio.PlaylistMasterVolume += 0.02f;

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

		if(playlistCont.HasPlaylist && playlistCont.PlaylistName != gamePlaylist)
		{
			MasterAudio.ChangePlaylistByName (gamePlaylist, false);

			playlistCont.PlayRandomSong ();
		}
	}

	public void SetMenuPlaylist ()
	{
//		if(playlistCont.PlaylistName != "Menu Ambient")
//			MasterAudio.ChangePlaylistByName ("Menu Ambient", true);
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

	public void SetMasterVolume ()
	{
		masterVolume = masterBar.value;

		SetSoundsVolume ();
		SetPlaylistVolume ();
	}

	public void SetSoundsVolume ()
	{
		if(!loading)
		{			
			MasterAudio.MasterVolumeLevel = soundsBar.value * masterVolume;
			
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

		yield return new WaitForSeconds (0.8f);

		canPlaySoundTest = true;
	}

	void UpdateAudioSettings ()
	{
		if (SceneManager.GetActiveScene ().name == "Scene Testing")
			return;

		if (masterVolume == 0)
			masterMuteToggle.isOn = true;
		else
			masterMuteToggle.isOn = false;

		if (MasterAudio.MasterVolumeLevel == 0)
			soundsMuteToggle.isOn = true;
		else
			soundsMuteToggle.isOn = false;

		if (MasterAudio.PlaylistMasterVolume == 0)
			musicMuteToggle.isOn = true;
		else
			musicMuteToggle.isOn = false;

		masterBar.value = masterVolume;

		if(masterVolume != 0)
		{
			playlistBar.value = MasterAudio.PlaylistMasterVolume / masterVolume;
			
			soundsBar.value = MasterAudio.MasterVolumeLevel / masterVolume;
		}
	}

	public void SetPlaylistVolume ()
	{
		if(!loading)
			MasterAudio.PlaylistMasterVolume = playlistBar.value * masterVolume;		
	}

	public void ToggleMuteMaster ()
	{
		if(!masterMute)
		{
			masterMute = true;

			musicMuteToggle.interactable = false;
			soundsMuteToggle.interactable = false;

			previousMasterVolume = masterVolume;
			DOTween.To(()=> masterVolume, x=> masterVolume =x, 0, 0.2f);
		}
		else
		{
			masterMute = false;

			musicMuteToggle.interactable = true;
			soundsMuteToggle.interactable = true;

			if(previousVolumePlaylist != 0)
				DOTween.To(()=> masterVolume, x=> masterVolume =x, previousMasterVolume, 0.2f);
			else
				DOTween.To(()=> masterVolume, x=> masterVolume =x, 1, 0.2f);
		}
	}

	public void ToggleMuteSounds ()
	{
		if (masterMute)
			return;


		if(!soundsMute)
		{
			soundsMute = true;

			previousVolumeSounds = MasterAudio.MasterVolumeLevel;
			DOTween.To(()=> MasterAudio.MasterVolumeLevel, x=> MasterAudio.MasterVolumeLevel =x, 0, 0.2f);
		}
		else
		{
			soundsMute = false;

			if(previousVolumeSounds != 0)
				DOTween.To(()=> MasterAudio.MasterVolumeLevel, x=> MasterAudio.MasterVolumeLevel =x, previousVolumeSounds, 0.2f);
			else
				DOTween.To(()=> MasterAudio.MasterVolumeLevel, x=> MasterAudio.MasterVolumeLevel =x, MasterAudio.MasterVolumeLevel * masterVolume, 0.2f);
		}
	}

	public void ToggleMuteMusic ()
	{
		if (masterMute)
			return;
		
		if(!musicMute)
		{
			musicMute = true;

			previousVolumePlaylist = MasterAudio.PlaylistMasterVolume;
			DOTween.To(()=> MasterAudio.PlaylistMasterVolume, x=> MasterAudio.PlaylistMasterVolume =x, 0, 0.2f);
		}
		else
		{
			musicMute = false;

			if(previousVolumePlaylist != 0)
				DOTween.To(()=> MasterAudio.PlaylistMasterVolume, x=> MasterAudio.PlaylistMasterVolume =x, previousVolumePlaylist, 0.2f);
			else
				DOTween.To(()=> MasterAudio.PlaylistMasterVolume, x=> MasterAudio.PlaylistMasterVolume =x, MasterAudio.PlaylistMasterVolume * masterVolume, 0.2f);
		}
	}

	public void ToggleLowPass ()
	{
		if(lowPassEnabled == true)
		{
			lowPassEnabled = false;
			ResetLowPass ();
		}
		else
		{
			lowPassEnabled = true;
			LowPass (pauseLowPass);
		}
	}

	public void ToggleHighPass ()
	{
		if(highPassEnabled == true)
		{
			highPassEnabled = false;
			ResetLowPass ();
		}
		else
		{
			highPassEnabled = true;
		}
	}

	public void LowPass (float lowPassFrquency, float duration)
	{
		if (!lowPassEnabled)
			return;

		playlistCont.mixerChannel.audioMixer.DOSetFloat ("LowPass", lowPassFrquency, duration).SetEase (Ease.OutQuad).SetId ("LowPass");
	}

	public void LowPass (float lowPassFrquency)
	{
		if (!lowPassEnabled)
			return;

		playlistCont.mixerChannel.audioMixer.DOSetFloat ("LowPass", lowPassFrquency, lowPassTweenDuration).SetEase (Ease.OutQuad).SetId ("LowPass");
	}

	public void ResetLowPass ()
	{
		playlistCont.mixerChannel.audioMixer.DOSetFloat ("LowPass", 22000f, lowPassTweenDuration).SetEase (Ease.OutQuad).SetId ("LowPass");
	}

	public void ResetLowPass (float duration)
	{
		playlistCont.mixerChannel.audioMixer.DOSetFloat ("LowPass", 22000f, duration).SetEase (Ease.OutQuad).SetId ("LowPass");
	}

	public void HighPass (float frequency)
	{
		if (!highPassEnabled)
			return;

		playlistCont.mixerChannel.audioMixer.DOSetFloat ("HighPass", frequency, highPassTweenDuration).SetEase (Ease.OutQuad).SetId ("HighPass");
	}

	public void ResetHighPass ()
	{
		playlistCont.mixerChannel.audioMixer.DOSetFloat ("HighPass", 10f, highPassTweenDuration).SetEase (Ease.OutQuad).SetId ("HighPass");
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
//		Debug.Log ("Audio Data Saved");

		PlayerPrefs.SetInt ("LowPassEnabled", lowPassEnabled ? 1 : 0);
		PlayerPrefs.SetInt ("HighPassEnabled", highPassEnabled ? 1 : 0);

		if(soundsMute)
		{
			PlayerPrefs.SetInt ("SoundsMute", 1);
			PlayerPrefs.SetFloat ("PreviousVolumeSounds", (float)previousVolumeSounds);
		}
		else
		{
			PlayerPrefs.SetInt ("SoundsMute", 0);
			PlayerPrefs.SetFloat ("SoundsVolume", (float)soundsBar.value * masterVolume);
		}

		if(musicMute)
		{
			PlayerPrefs.SetInt ("MusicMute", 1);

			PlayerPrefs.SetFloat ("PreviousVolumePlaylist", (float)previousVolumePlaylist);
		}
		else
		{
			PlayerPrefs.SetInt ("MusicMute", 0);

			PlayerPrefs.SetFloat ("MusicVolume", (float)playlistBar.value * masterVolume);
		}

		if(masterMute)
		{
			PlayerPrefs.SetInt ("MasterMute", 1);
			PlayerPrefs.SetFloat ("PreviousMasterVolume", (float)previousMasterVolume);
		}
		else
		{
			PlayerPrefs.SetInt ("MasterMute", 0);
			PlayerPrefs.SetFloat ("MasterVolume", (float)masterVolume);
		}
	}

	void LoadPlayersPrefs ()
	{
//		Debug.Log ("Audio Data Loaded");

		loading = true;

		if(PlayerPrefs.GetInt ("LowPassEnabled") == 1)
		{
			lowPassEnabled = true;
			lowPassToggle.isOn = true;
		}
		else
		{
			lowPassEnabled = false;
			lowPassToggle.isOn = false;
		}

		if(PlayerPrefs.GetInt ("HighPassEnabled") == 1)
		{
			highPassEnabled = true;
			highPassToggle.isOn = true;
		}
		else
		{
			highPassEnabled = false;
			highPassToggle.isOn = false;
		}


		previousMasterVolume = PlayerPrefs.GetFloat("PreviousVolumePlaylist");
		masterBar.value = PlayerPrefs.GetFloat("MasterVolume");
		masterVolume = PlayerPrefs.GetFloat("MasterVolume");

		if(PlayerPrefs.GetInt ("MasterMute") == 1)
		{
			masterMute = true;
			masterBar.value = 0;
			masterVolume = 0;
			masterMuteToggle.isOn = true;

			musicMuteToggle.interactable = false;
			soundsMuteToggle.interactable = false;
		}
		else
			masterMuteToggle.isOn = false;

		previousVolumeSounds = PlayerPrefs.GetFloat("PreviousVolumeSounds");

		if(masterVolume != 0)
			soundsBar.value = PlayerPrefs.GetFloat("SoundsVolume") / masterVolume;
		else
			soundsBar.value = 0;

		MasterAudio.MasterVolumeLevel = PlayerPrefs.GetFloat("SoundsVolume");

		if(PlayerPrefs.GetInt ("SoundsMute") == 1)
		{
			soundsMute = true;
			soundsBar.value = 0;
			MasterAudio.MasterVolumeLevel = 0;
			soundsMuteToggle.isOn = true;
		}
		else
		{
			soundsMute = false;
			soundsMuteToggle.isOn = false;
		}

		previousVolumePlaylist = PlayerPrefs.GetFloat("PreviousVolumePlaylist");

		if(masterVolume != 0)
			playlistBar.value = PlayerPrefs.GetFloat("MusicVolume") / masterVolume;
		else
			playlistBar.value = 0;

		MasterAudio.PlaylistMasterVolume = PlayerPrefs.GetFloat("MusicVolume");

		if(PlayerPrefs.GetInt ("MusicMute") == 1)
		{
			musicMute = true;
			playlistBar.value = 0;
			MasterAudio.PlaylistMasterVolume = 0;
			musicMuteToggle.isOn = true;
		}
		else
		{
			musicMute = false;
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

	IEnumerator MasterVolumeChange ()
	{
		if (OnMasterVolumeChange != null)
			OnMasterVolumeChange ();

		float volume = masterVolume;

		yield return new WaitUntil (() => volume != masterVolume);

		StartCoroutine (MasterVolumeChange ());
	}
}
