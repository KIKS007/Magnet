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

        [PropertyOrder(-4)]
        [Button("Clear Recording")]
        public void Clear()
        {
            StopRecording();
            StopReplay();

            isRecording = false;
            isReplaying = false;
            isPaused = false;

            /*OnReplayStart = null;
			OnReplayStop = null;
			OnReplayTimeChange = null;*/

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

        /*[ButtonGroupAttribute ("Play", -1)]
		[Button ("Play")]
		public void PlayButton ()
		{
			Play ();
		}

		[ButtonGroupAttribute ("Play", -1)]
		[Button ("Pause")]
		public void PauseButton ()
		{
			Pause ();
		}

		[ButtonGroupAttribute ("Play", -1)]
		[Button ("Replay")]
		public void ReplayButton ()
		{
			ReplayReplay ();
		}*/

        #endregion

        public bool replayEnabled = false;

        [Header("Record Rate")]
        public int recordRate = 120;
        public int particlesRecordRate = 60;

        [Header("Replay")]
        public List<ReplaySpeed> replaySpeed = new List<ReplaySpeed>();
        public int currentReplaySpeed = 0;

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

        [Header("ArenaDeadzones")]
        public List<ArenaDeadzoneColumn> arenaDeadzoneColumns = new List<ArenaDeadzoneColumn>();

        public Action<float> OnReplayTimeChange;
        public Action OnReplayStart;
        public Action OnReplayStop;
        public Action OnRecordingStart;
        public Action OnRecordingStop;
        public Action OnClear;

        private bool wasPlaying = true;

        private bool replayReplayAvailable = false;

        #region UI

        [Header("Replay UI")]
        public Slider _slide;
        public Image _play;
        public Image _replay;
        public Image _pause;
        public Text _timestamp;
        public GameObject _replayCanvas;

        #endregion

        #region Time

        private float _startTime;
        private float _endTime;
        private ArenaDeadzones _arenaDeadzones;

        #endregion

        // Use this for initialization
        void Start()
        {
            if (!gameObject.activeSelf)
                return;

            _slide = _replayCanvas.GetComponentInChildren<Slider>();

            _play.GetComponent<Button>().onClick.AddListener(() => Play());
            _pause.GetComponent<Button>().onClick.AddListener(() => Pause());
            _replay.GetComponent<Button>().onClick.AddListener(() => ReplayReplay());
            _slide.GetComponent<Slider>().onValueChanged.AddListener((Single v) => SetCursor(v));

            _arenaDeadzones = FindObjectOfType<ArenaDeadzones>();

            GlobalVariables.Instance.OnStartMode += ResetReplay;
            GlobalVariables.Instance.OnRestartMode += ResetReplay;
            GlobalVariables.Instance.OnEndMode += () =>
            {
                DOVirtual.DelayedCall(GlobalVariables.Instance.lastManManager.endGameDelay, () =>
                    {
                        StopRecording();

                    }).SetUpdate(true);
            };

            SetUIEvents();
        }

        [ButtonGroupAttribute("Repplay", -1)]
        public void NextReplaySpeed()
        {
            if (currentReplaySpeed + 1 >= replaySpeed.Count)
                currentReplaySpeed = 0;
            else
                currentReplaySpeed++;
        }

        [ButtonGroupAttribute("Repplay", -1)]
        public void PreviousReplaySpeed()
        {
            if (currentReplaySpeed - 1 < 0)
                currentReplaySpeed = replaySpeed.Count - 1;
            else
                currentReplaySpeed--;
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
                        OnReplayTimeChange(_slide.value);

                    Replay(ReplayManager.Instance.GetReplayTime());
                }
            }
        }

        public float GetCurrentTime()
        {
            return Time.time - _startTime;
        }

        public float GetReplayTime()
        {
            return _slide.value;
        }

        public void StartRecording()
        {
            _startTime = Time.time;
            isRecording = true;
            isReplaying = false;

            if (OnRecordingStart != null)
                OnRecordingStart();
        }

        public void StopRecording()
        {
            _endTime = Time.time;
            isRecording = false;
            isReplaying = false;

            if (OnRecordingStop != null)
                OnRecordingStop();
        }

        public void StartReplay()
        {
            if (OnRecordingStart == null)
            {
                Debug.LogWarning("No Replay Entity!"); 
                return;
            }

            _replayCanvas.GetComponent<CanvasGroup>().DOFade(1, MenuManager.Instance.animationDuration);
            _slide.maxValue = _endTime - _startTime;

            foreach (Transform p in GlobalVariables.Instance.ParticulesClonesParent)
                if (p.GetComponent<ParticleSystem>() != null)
                    p.GetComponent<ParticleSystem>().Clear();

            for (int i = 0; i < particlesReplay.Count; i++)
                particlesReplay[i].replayed = false;

            for (int i = 0; i < arenaDeadzoneColumns.Count; i++)
                arenaDeadzoneColumns[i].replayed = false;

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

            _slide.value = 0;

            if (OnReplayTimeChange != null)
                OnReplayTimeChange(_startTime);
        }

        void StopReplay()
        {
            if (OnReplayTimeChange != null)
                OnReplayTimeChange(_endTime);

            if (OnReplayStop != null)
                OnReplayStop();

            isReplaying = false;
            isPaused = false;
            _replayCanvas.GetComponent<CanvasGroup>().DOFade(0, MenuManager.Instance.animationDuration);

            /*OnReplayStart = null;
			OnReplayStop = null;
			OnReplayTimeChange = null;*/
        }

        void ResetReplay()
        {
            Clear();

            particlesReplay.Clear();
            arenaDeadzoneColumns.Clear();

            StartRecording();

            //_arenaDeadzones.Reset();
        }

        void Replay(float time)
        {
            foreach (var p in particlesReplay)
            {
                if (time >= p.time && !p.replayed)
                {
                    p.replayed = true;

                    if (!p.particles.gameObject.activeSelf)
                        p.particles.gameObject.SetActive(true);

                    p.particles.Play();
                    break;
                }
            }

            foreach (var c in arenaDeadzoneColumns)
            {
                if (time >= c.time && !c.replayed)
                {
                    c.replayed = true;
                    StartCoroutine(_arenaDeadzones.SetDeadly(c.columnParent, true));
                    break;
                }
            }
        }


        #region UI Methods

        void SetUIEvents()
        {
            EventTrigger trigger = _slide.GetComponent<EventTrigger>();
            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerDown;
                entry.callback.AddListener((eventData) =>
                    {
                        wasPlaying = !isPaused;

                        isPaused = true;

                        Pause();
                    });
                trigger.triggers.Add(entry);
            }
            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerUp;
                entry.callback.AddListener((eventData) =>
                    {
                        if (wasPlaying)
                            Play();
                    });
                trigger.triggers.Add(entry);
            }

            trigger = _slide.transform.parent.GetComponent<EventTrigger>();
            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerExit;
                entry.callback.AddListener((eventData) =>
                    {
                        _slide.handleRect.transform.localScale = Vector3.zero;
                    });
                trigger.triggers.Add(entry);
            }
            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener((eventData) =>
                    {
                        _slide.handleRect.transform.localScale = Vector3.one;
                    });
                trigger.triggers.Add(entry);
            }
        }

        public void Play()
        {
            _slide.Select();

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
            _slide.Select();

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
            replayReplayAvailable = false;
            isPaused = false;

            StartReplay();

            Swap(_replay.gameObject);
            Play();
        }

        public void SetCursor(Single value)
        {
            RefreshTimer();

            if (replayReplayAvailable)
            {
                replayReplayAvailable = false;
                Swap(_replay.gameObject, _play.gameObject);
            }

            if (_slide.value == _endTime - _startTime)
            {
                Pause();

                replayReplayAvailable = true;
                Swap(_play.gameObject, _replay.gameObject, .2f);
            }

            /*if (OnReplayTimeChange != null && isPaused)
            {
                OnReplayTimeChange(value + _startTime);
                Debug.Log(value + _startTime);
            }*/
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

        public void SetCurveConstant(AnimationCurve curve)
        {
            #if UNITY_EDITOR
            for (int i = 0; i < curve.keys.Length; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Constant);
                AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Constant);
            }
            #endif
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