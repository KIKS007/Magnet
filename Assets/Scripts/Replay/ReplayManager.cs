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
using UnityEditor;

namespace Replay
{
	public class ReplayManager : MonoBehaviour
	{
		#region Buttons

		[ButtonGroupAttribute ("Record", -3)]
		[Button ("Start Recording")]
		public void StartRecordingButton ()
		{
			StartRecording ();
		}

		[ButtonGroupAttribute ("Record", -3)]
		[Button ("Stop Recording")]
		public void StopRecordingButton ()
		{
			StopRecording ();
		}

		[ButtonGroupAttribute ("Replay", -2)]
		[Button ("Start Replay")]
		public void StartReplayButton ()
		{
			StartReplay ();
		}

		[ButtonGroupAttribute ("Replay", -2)]
		[Button ("Stop Replay")]
		public void StopReplayButton ()
		{
			StopRecording ();
			StopReplay ();
		}

		[ButtonGroupAttribute ("Play", -1)]
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
		}

		#endregion

		public static ReplayManager Instance;

		[Header ("Record Rate")]
		public int recordRate = 120;

		[Header ("States")]
		public bool isRecording = false;
		public bool isReplaying = false;
		public bool isPaused = false;

		[Header ("No Record States")]
		public List<GameStateEnum> noRecordStates = new List<GameStateEnum> ();

		public Action<float> OnReplayTimeChange;
		public Action OnReplayStart;
		public Action OnReplayStop;
		public Action OnRecordingStart;
		public Action OnRecordingStop;

		private bool wasPlaying = true;

		private bool replayReplayAvailable = false;

		#region UI

		[Header ("Replay UI")]
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

		#endregion

		void Awake ()
		{
			if (ReplayManager.Instance == null) 
				ReplayManager.Instance = this;
			else 
				Destroy (gameObject);
		}

		public float GetCurrentTime ()
		{
			return Time.time - _startTime;
		}

		public void StartRecording ()
		{
			_startTime = Time.time;
			isRecording = true;
			isReplaying = false;

			if (OnRecordingStart != null)
				OnRecordingStart ();
		}

		public void StopRecording ()
		{
			_endTime = Time.time;
			isRecording = false;
			isReplaying = false;

			if (OnRecordingStop != null)
				OnRecordingStop ();
		}

		void StartReplay ()
		{
			if (OnReplayTimeChange == null)
				return;

			_replayCanvas.SetActive (true);
			isReplaying = false;
			isPaused = false;
			_replayCanvas.GetComponent<CanvasGroup> ().alpha = 1;
			_slide.maxValue = _endTime - _startTime;

			if(OnReplayTimeChange != null)
				OnReplayTimeChange (0);
			
			RefreshTimer ();

			if (OnReplayStart != null) 
			{
				// You can remove this log if you don't care
				#if UNITY_EDITOR
				Debug.Log ("There's " + OnReplayStart.GetInvocationList ().Length + " objects affected by the replay.");
				#endif

				OnReplayStart ();
			}
		}

		void StopReplay ()
		{
			_replayCanvas.SetActive (false);
			isReplaying = false;
			isPaused = false;
			_replayCanvas.GetComponent<CanvasGroup> ().alpha = 0;

			if(OnReplayTimeChange != null)
				OnReplayTimeChange (_endTime);

			if (OnReplayStop != null) 
				OnReplayStop ();

			OnReplayStart = null;
			OnReplayStop = null;
			OnReplayTimeChange = null;
		}

		// Use this for initialization
		void Start ()
		{
			_slide = _replayCanvas.GetComponentInChildren<Slider> ();


			_play.GetComponent<Button> ().onClick.AddListener (() => Play ());
			_pause.GetComponent<Button> ().onClick.AddListener (() => Pause ());
			_replay.GetComponent<Button> ().onClick.AddListener (() => ReplayReplay ());
			_slide.GetComponent<Slider> ().onValueChanged.AddListener ((Single v) => SetCursor (v));

			SetUIEvents ();
		}

		void SetUIEvents ()
		{
			EventTrigger trigger = _slide.GetComponent<EventTrigger> ();
			{
				EventTrigger.Entry entry = new EventTrigger.Entry ();
				entry.eventID = EventTriggerType.PointerDown;
				entry.callback.AddListener ((eventData) => {
					wasPlaying = isReplaying;
					Pause ();
				});
				trigger.triggers.Add (entry);
			}
			{
				EventTrigger.Entry entry = new EventTrigger.Entry ();
				entry.eventID = EventTriggerType.PointerUp;
				entry.callback.AddListener ((eventData) => {
					if (wasPlaying)
						Play ();
				});
				trigger.triggers.Add (entry);
			}

			trigger = _slide.transform.parent.GetComponent<EventTrigger> ();
			{
				EventTrigger.Entry entry = new EventTrigger.Entry ();
				entry.eventID = EventTriggerType.PointerExit;
				entry.callback.AddListener ((eventData) => {
					_slide.handleRect.transform.localScale = Vector3.zero;
				});
				trigger.triggers.Add (entry);
			}
			{
				EventTrigger.Entry entry = new EventTrigger.Entry ();
				entry.eventID = EventTriggerType.PointerEnter;
				entry.callback.AddListener ((eventData) => {
					_slide.handleRect.transform.localScale = Vector3.one;
				});
				trigger.triggers.Add (entry);
			}
		}

	
		// Update is called once per frame
		void Update ()
		{
			if (isReplaying) 
			{
				if (!isPaused) 
					_slide.value += Time.deltaTime * Time.timeScale;

				if(OnReplayTimeChange != null)
					OnReplayTimeChange (_slide.value);
			}
		}

		public void Play ()
		{
			_slide.Select ();

			if (_slide.value == _endTime - _startTime)
				return;
			
			if (isPaused || !isReplaying) 
			{
				isReplaying = true;
				isPaused = false;

				Swap (_play.gameObject, _pause.gameObject);

				if (_play.transform.GetSiblingIndex () > _pause.transform.GetSiblingIndex ()) {
					_play.transform.SetSiblingIndex (_pause.transform.GetSiblingIndex ());
				}
			}
		}

		void Swap (GameObject _out, GameObject _in = null, float delay = 0f)
		{
			if (_in != null) 
			{
				_in.SetActive (true);
			}

			_out.SetActive (false);
		}

		public void Pause ()
		{
			_slide.Select ();

			if (!isPaused) 
			{
				isPaused = true;

				Swap (_pause.gameObject, _play.gameObject);

				if (_pause.transform.GetSiblingIndex () > _play.transform.GetSiblingIndex ()) {
					_pause.transform.SetSiblingIndex (_play.transform.GetSiblingIndex ());
				}
			}
		}

		public void ReplayReplay ()
		{
			_slide.value = 0;
			replayReplayAvailable = false;
			isPaused = false;

			Swap (_replay.gameObject);
			Play ();
		}

		public void SetCursor (Single value)
		{
			RefreshTimer ();

			if (replayReplayAvailable) 
			{
				replayReplayAvailable = false;
				Swap (_replay.gameObject, _play.gameObject);
			}

			if (_slide.value == _endTime - _startTime) 
			{
				Pause ();

				replayReplayAvailable = true;
				Swap (_play.gameObject, _replay.gameObject, .2f);
			}

			if (OnReplayTimeChange != null) {
				OnReplayTimeChange (value + _startTime);
			}
		}

		void RefreshTimer ()
		{
			float current = _slide.value;
			float total = (_endTime - _startTime);

			string currentMinutes = Mathf.Floor (current / 60).ToString ("00");
			string currentSeconds = (current % 60).ToString ("00");

			string totalMinutes = Mathf.Floor (total / 60).ToString ("00");
			string totalSeconds = (total % 60).ToString ("00");

			_timestamp.text = currentMinutes + ":" + currentSeconds + " / " + totalMinutes + ":" + totalSeconds;
		}

		public void SetCurveConstant (AnimationCurve curve)
		{
			for(int i = 0; i < curve.keys.Length; i++)
			{
				AnimationUtility.SetKeyLeftTangentMode (curve, i, AnimationUtility.TangentMode.Constant);
				AnimationUtility.SetKeyRightTangentMode (curve, i, AnimationUtility.TangentMode.Constant);
			}
		}

		#if UNITY_EDITOR
		void OnDestroy ()
		{
//			Debug.LogWarning (gameObject.name + " destroyed.");
		}
		#endif
	}
}