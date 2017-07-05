using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Replay
{
	public class ReplayComponent : MonoBehaviour 
	{
		[Header ("Record Rate")]
		public bool overrideRecordRate = false;
		[ShowIfAttribute ("overrideRecordRate")]
		public int recordRate = 120;

		protected virtual void Start ()
		{
			ReplayManager.Instance.OnClear += OnClear;
			SetEvents ();
		}

		public virtual void OnClear ()
		{
			SetEvents ();
		}

		public virtual void SetEvents ()
		{
			ReplayManager.Instance.OnReplayTimeChange += Replay;

			ReplayManager.Instance.OnRecordingStart += OnRecordingStart;
			ReplayManager.Instance.OnRecordingStop += OnRecordingStop;

			ReplayManager.Instance.OnReplayStart += OnReplayStart;
			ReplayManager.Instance.OnReplayStop += OnReplayStop;
		}

		protected virtual void OnEnable ()
		{
			StartCoroutine (RecordingRate ());
		}

		protected virtual void Update ()
		{
			if (ReplayManager.Instance.isReplaying && !ReplayManager.Instance.isPaused)
				Replay (ReplayManager.Instance.GetReplayTime ());
		}

		public virtual void OnRecordingStart ()
		{
			
		}

		protected virtual IEnumerator RecordingRate ()
		{
			while (true) 
			{
				int recordRate = ReplayManager.Instance.recordRate;

				if (overrideRecordRate)
					recordRate = this.recordRate;

				yield return new WaitForSeconds (1 / recordRate);

				if (ReplayManager.Instance.isRecording && !ReplayManager.Instance.noRecordStates.Contains (GlobalVariables.Instance.GameState)) 
					Recording ();
			}
		}

		protected virtual void Recording ()
		{
			
		}

		public virtual void OnRecordingStop ()
		{

		}

		public virtual void OnReplayStart ()
		{
		}

		public virtual void Replay (float t)
		{
			//Debug.Log (t + " " + name);
		}

		public virtual void OnReplayStop ()
		{
		}

		protected virtual void OnDestroy ()
		{
			if (GlobalVariables.applicationIsQuitting)
				return;

			ReplayManager.Instance.OnReplayTimeChange -= Replay;

			ReplayManager.Instance.OnRecordingStart -= OnRecordingStart;
			ReplayManager.Instance.OnRecordingStop -= OnRecordingStop;

			ReplayManager.Instance.OnReplayStart -= OnReplayStart;
			ReplayManager.Instance.OnReplayStop -= OnReplayStop;
		}
	}
}

