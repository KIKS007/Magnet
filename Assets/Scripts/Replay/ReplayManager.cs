/* IN-GAME REPLAY - @madebyfeno - <feno@ironequal.com>
 * You can use it in commercial projects (and non-commercial project of course), modify it and share it.
 * Do not resell the resources of this project as-is or even modified. 
 * TL;DR: Do what the fuck you want but don't re-sell it
 * 
 * ironequal.com
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Sirenix.OdinInspector;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Replay
{
    public class ReplayManager : Singleton<ReplayManager>
    {
        #region Buttons

        public void ClearAll()
        {
            if (isRecording)
            {
                isRecording = false;
                StopRecording();
            }
            if (isReplaying)
            {
                isReplaying = false;
                StopReplay();
            }

            isPaused = false;

            if (OnClear != null)
                OnClear();

            LoadModeManager.Instance.DestroyParticules();
        }

        [ButtonGroupAttribute("Record", -3)]
        [Button("Start Recording")]
        public void StartRecordingButton()
        {
            StartRecording();
        }

        [ButtonGroupAttribute("Record", -3)]
        [Button("Stop Recording")]
        public void StopRecordingButton()
        {
            StopRecording();
        }

        [ButtonGroupAttribute("Replay", -2)]
        [Button("Start Replay")]
        public void StartReplayButton()
        {
            StartReplay();
        }

        [ButtonGroupAttribute("Replay", -2)]
        [Button("Stop Replay")]
        public void StopReplayButton()
        {
            StopRecording();
            StopReplay();
        }

        #endregion

        public bool replayEnabled = false;

        [Header("Record Rate")]
        public int recordRate = 120;
        public int particlesRecordRate = 60;
        public float recordLength = 20;

        [Header("Replay Speed")]
        public List<ReplaySpeed> replaySpeed = new List<ReplaySpeed>();
        public int currentReplaySpeed = 0;
        public Text speedText;

        [Header("States")]
        public bool isRecording = false;
        public bool isReplaying = false;
        public bool isPaused = false;

        [Header("No Record States")]
        public List<GameStateEnum> noRecordStates = new List<GameStateEnum>();

        [Header("Replay Entities")]
        public float listRecordEpsilon = 0.008f;

        [Header("Particles")]
        public List<Particles> particlesReplay = new List<Particles>();

        private List<Particles> _particlesReplay = new List<Particles>();
        [HideInInspector]
        public List<ReplayParticles> _attractionParticles = new List<ReplayParticles>();

        [Header("ArenaDeadzones")]
        public List<ArenaDeadzoneColumn> arenaDeadzoneColumns = new List<ArenaDeadzoneColumn>();

        private List<ArenaDeadzoneColumn> _arenaDeadzoneColumns = new List<ArenaDeadzoneColumn>();

        public Action<float> OnReplayTimeChange;
        public Action OnReplayStart;
        public Action OnReplayStop;
        public Action OnRecordingStart;
        public Action OnRecordingStop;
        public Action OnClear;

        [Header("Replay UI")]
        public Button restartButton;
        public Button playPauseButton;
        public Button speedButton;
        public Button quitButton;
        public Slider _slide;
        public Image _play;
        public Image _pause;
        public Text _timestamp;
        public GameObject _replayCanvas;

        private float _startTime;
        private float _endTime;
        private ArenaDeadzones _arenaDeadzones;
       
        private float endTimeDiference = 0;

        // Use this for initialization
        void Start()
        {
            if (!gameObject.activeSelf)
                return;
            
            transform.GetChild(0).gameObject.SetActive(true);

            _slide = _replayCanvas.GetComponentInChildren<Slider>();
            _slide.GetComponent<Slider>().onValueChanged.AddListener((Single v) => SetCursor(v));

            _arenaDeadzones = FindObjectOfType<ArenaDeadzones>();

            GlobalVariables.Instance.OnStartMode += () =>
            {
                StopReplay(true);
                ResetReplay();
            };
            
            GlobalVariables.Instance.OnRestartMode += () =>
            {
                StopReplay(true);
                ResetReplay();
            };
            
            GlobalVariables.Instance.OnEndMode += () =>
            {
                DOVirtual.DelayedCall(GlobalVariables.Instance.lastManManager.endGameDelay, () =>
                    {
                        StopRecording();

                    }).SetUpdate(true);
            };

            speedText.text = replaySpeed[currentReplaySpeed].speedName;

            transform.GetChild(0).gameObject.SetActive(false);
        }

        [ButtonGroupAttribute("Repplay", -1)]
        public void NextReplaySpeed()
        {
            if (currentReplaySpeed + 1 >= replaySpeed.Count)
                currentReplaySpeed = 0;
            else
                currentReplaySpeed++;

            speedText.text = replaySpeed[currentReplaySpeed].speedName;
        }

        [ButtonGroupAttribute("Repplay", -1)]
        public void PreviousReplaySpeed()
        {
            if (currentReplaySpeed - 1 < 0)
                currentReplaySpeed = replaySpeed.Count - 1;
            else
                currentReplaySpeed--;

            speedText.text = replaySpeed[currentReplaySpeed].speedName;
        }

        // Update is called once per frame
        void Update()
        {
            if (isReplaying)
            {
                if (!isPaused)
                {
                    _slide.value += Time.unscaledDeltaTime * replaySpeed[currentReplaySpeed].speed;

                    if (OnReplayTimeChange != null)
                        OnReplayTimeChange(_slide.value + endTimeDiference);

                    Replay(ReplayManager.Instance.GetReplayTime());
                }

                ReplayInputs();
            }
        }

        void ReplayInputs()
        {
            for (int i = 0; i < GlobalVariables.Instance.rewiredPlayers.Length; i++)
//            for (int i = 0; i < 2; i++)
            {
                if (i != 0 && i != GlobalVariables.Instance.menuGamepadNumber)
                    continue;

                if (GlobalVariables.Instance.rewiredPlayers[i].GetButtonDown("PlayPause"))
                {
                    playPauseButton.onClick.Invoke();
                    playPauseButton.GetComponent<MenuButtonAnimationsAndSounds>().ShaderHighlightClick();
                }

                if (GlobalVariables.Instance.rewiredPlayers[i].GetButtonDown("Restart"))
                {
                    restartButton.onClick.Invoke();
                    restartButton.GetComponent<MenuButtonAnimationsAndSounds>().ShaderHighlightClick();
                }

                if (GlobalVariables.Instance.rewiredPlayers[i].GetButtonDown("NextSpeed"))
                {
                    speedButton.onClick.Invoke();
                    speedButton.GetComponent<MenuButtonAnimationsAndSounds>().ShaderHighlightClick();
                }

                if (GlobalVariables.Instance.rewiredPlayers[i].GetButtonDown("QuitReplay"))
                {
                    quitButton.onClick.Invoke();
                    quitButton.GetComponent<MenuButtonAnimationsAndSounds>().ShaderHighlightClick();
                }
            }
        }

        public void SelectNone()
        {
            MenuManager.Instance.eventSyst.SetSelectedGameObject(null);
        }

        public float GetCurrentTime()
        {
            return Time.time - _startTime;
        }

        public float GetReplayTime()
        {
            return _slide.value + endTimeDiference;
        }

        public void StartRecording()
        {
            if (OnClear != null)
                OnClear();

            _startTime = Time.time;
            isRecording = true;
            isReplaying = false;

            if (OnRecordingStart != null)
                OnRecordingStart();

            StartCoroutine(CleanParticlesInGame());
        }

        public void StopRecording()
        {
            _endTime = Time.time;
            isRecording = false;
            isReplaying = false;

            if (OnRecordingStop != null)
                OnRecordingStop();
        }

        public void SetupReplay()
        {
            endTimeDiference = 0;

            transform.GetChild(0).gameObject.SetActive(true);

            StartCoroutine(CleanParticles());

            if ((_endTime - _startTime) > recordLength)
            {
                endTimeDiference = _endTime - recordLength - _startTime;
                _startTime = _endTime - recordLength;
            }
        }

        public void StartReplay()
        {
            Time.timeScale = 1;

            if (OnRecordingStart == null)
            {
                Debug.LogWarning("No Replay Entity!"); 
                return;
            }

            _slide.maxValue = _endTime - _startTime;

            foreach (Transform p in GlobalVariables.Instance.particlesParent)
            {
                if (p.GetComponent<ParticleSystem>() != null)
                    p.GetComponent<ParticleSystem>().Clear();
            }

            foreach (var p in particlesReplay)
                p.particles.gameObject.SetActive(false);

            for (int i = 0; i < particlesReplay.Count; i++)
                particlesReplay[i].replayed = false;

            for (int i = 0; i < arenaDeadzoneColumns.Count; i++)
                arenaDeadzoneColumns[i].replayed = false;

            _particlesReplay = new List<Particles>(particlesReplay);
            _arenaDeadzoneColumns = new List<ArenaDeadzoneColumn>(arenaDeadzoneColumns);

            RefreshTimer();

            if (OnReplayStart != null)
            {
                // You can remove this log if you don't care
                #if UNITY_EDITOR
                Debug.Log("There's " + OnReplayStart.GetInvocationList().Length + " objects affected by the replay.");
                #endif

                OnReplayStart();
            }

            _arenaDeadzones.Reset();

            isReplaying = true;
            isPaused = false;

            Swap(_play.gameObject, _pause.gameObject);

            _slide.value = 0;

            if (OnReplayTimeChange != null)
                OnReplayTimeChange(_startTime);

        }

        IEnumerator CleanParticles()
        {
            float thresholdTime = (_endTime - _startTime - recordLength);

            foreach (var p in particlesReplay)
                if (p.time < thresholdTime)
                    Destroy(p.particles.gameObject);

            yield return new WaitForEndOfFrame();

            particlesReplay.RemoveAll(item => item.particles == null);

            isPaused = true;
        }

        IEnumerator CleanParticlesInGame()
        {
            yield return new WaitForSeconds(recordLength);

            while (isRecording && !isReplaying)
            {
                yield return new WaitForSeconds(2f);

                CleanParticlesEditor();

                yield return new WaitForEndOfFrame();

                particlesReplay.RemoveAll(item => item.particles == null);
                _attractionParticles.RemoveAll(item => item == null);

                //Resources.UnloadUnusedAssets();
            }
        }

        void CleanParticlesEditor()
        {
            float thresholdTime = (Time.time - _startTime - recordLength);

            foreach (var p in particlesReplay)
                if (p.time < thresholdTime && p.particles != null)
                    Destroy(p.particles.gameObject);

            foreach (var p in _attractionParticles)
            {
                if (p != null && p.particles.Count > 0 && p.particles[p.particles.Count - 1].time < thresholdTime)
                    Destroy(p.gameObject);
            }
        }

        public void StopReplay(bool reset = false)
        {
            isReplaying = false;
            isPaused = false;
            transform.GetChild(0).gameObject.SetActive(false);

            if (!reset)
            {
                if (OnReplayTimeChange != null)
                    OnReplayTimeChange(_endTime);
                
                if (OnReplayStop != null)
                    OnReplayStop();
            }
        }

        void ResetReplay()
        {
            ClearAll();

            foreach (var p in particlesReplay)
                if (p.particles != null)
                    Destroy(p.particles.gameObject);

            foreach (var p in _attractionParticles)
                if (p != null)
                    Destroy(p.gameObject);

            _attractionParticles.Clear();
            particlesReplay.Clear();
            arenaDeadzoneColumns.Clear();

            StartRecording();

            //_arenaDeadzones.Reset();
        }

        void Replay(float time)
        {
            Particles particles = null;

            foreach (var p in _particlesReplay)
            {
                if (p.particles == null)
                    continue;

                if (time >= p.time && !p.replayed)
                {
                    particles = p;

                    p.replayed = true;

                    if (!p.particles.gameObject.activeSelf)
                        p.particles.gameObject.SetActive(true);

                    p.particles.Play();

                    if (p.particles.GetComponent<ParticlesAutoDestroy>())
                        p.particles.GetComponent<ParticlesAutoDestroy>().Start();
                    break;
                }
            }

            if (particles != null)
                _particlesReplay.Remove(particles);

            ArenaDeadzoneColumn arenaColumn = null;

            foreach (var c in _arenaDeadzoneColumns)
            {
                if (time >= c.time && !c.replayed)
                {
                    arenaColumn = c;

                    c.replayed = true;
                    StartCoroutine(_arenaDeadzones.SetDeadly(c.columnParent, true));
                    break;
                }
            }

            if (arenaColumn != null)
                _arenaDeadzoneColumns.Remove(arenaColumn);
            
        }


        #region UI Methods

        public void PlayPause()
        {
            if (isPaused)
            {
                Time.timeScale = 1;

                if (_slide.value == _endTime - _startTime)
                {
                    StartReplay();
                    return;
                }

                isReplaying = true;
                isPaused = false;

                Swap(_play.gameObject, _pause.gameObject);
            }
            else
            {
                Time.timeScale = 0;

                isPaused = true;

                Swap(_pause.gameObject, _play.gameObject);
            }
        }

        public void Play()
        {
            if (_slide.value == _endTime - _startTime)
                return;

            if (isPaused || !isReplaying)
            {
                isReplaying = true;
                isPaused = false;

                Swap(_play.gameObject, _pause.gameObject);

                if (_play.transform.GetSiblingIndex() > _pause.transform.GetSiblingIndex())
                {
                    _play.transform.SetSiblingIndex(_pause.transform.GetSiblingIndex());
                }
            }
        }

        void Swap(GameObject _out, GameObject _in = null, float delay = 0f)
        {
            if (_in != null)
            {
                _in.SetActive(true);
            }

            _out.SetActive(false);
        }

        public void Pause()
        {
            if (!isPaused)
            {
                isPaused = true;

                Swap(_pause.gameObject, _play.gameObject);

                if (_pause.transform.GetSiblingIndex() > _play.transform.GetSiblingIndex())
                {
                    _pause.transform.SetSiblingIndex(_play.transform.GetSiblingIndex());
                }
            }
        }

        public void ReplayReplay()
        {
            _slide.value = 0;
            isPaused = false;

            StartReplay();

            Play();
        }

        public void SetCursor(Single value)
        {
            RefreshTimer();

            if (_slide.value == _endTime - _startTime)
            {
                Pause();
            }
        }

        void RefreshTimer()
        {
            float current = _slide.value;
            float total = (_endTime - _startTime);

            string currentMinutes = Mathf.Floor(current / 60).ToString("00");
            string currentSeconds = (current % 60).ToString("00");

            string totalMinutes = Mathf.Floor(total / 60).ToString("00");
            string totalSeconds = (total % 60).ToString("00");

            _timestamp.text = currentMinutes + ":" + currentSeconds + " / " + totalMinutes + ":" + totalSeconds;
        }

        #endregion

        [System.Serializable]
        public class Particles
        {
            public float time;
            public bool replayed = false;
            public ParticleSystem particles;

            public Particles(ParticleSystem p)
            {
                time = ReplayManager.Instance.GetCurrentTime();
                particles = p;
            }
        }

        [System.Serializable]
        public class ArenaDeadzoneColumn
        {
            public float time;
            public bool replayed = false;
            public Transform columnParent;

            public ArenaDeadzoneColumn(Transform t)
            {
                time = ReplayManager.Instance.GetCurrentTime();
                columnParent = t;
            }
        }

        [System.Serializable]
        public class ReplaySpeed
        {
            public string speedName;
            public float speed = 1;
        }
    }
}