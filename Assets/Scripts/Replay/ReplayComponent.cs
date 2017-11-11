using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace Replay
{
    public class ReplayComponent : MonoBehaviour
    {
        protected virtual void Start()
        {
            SetEvents();
        }

        public virtual void OnClear()
        {
			
        }

        public virtual void SetEvents()
        {
            ReplayManager.Instance.OnReplayTimeChange += Replay;

            ReplayManager.Instance.OnRecordingStart += OnRecordingStart;
            ReplayManager.Instance.OnRecordingStop += OnRecordingStop;

            ReplayManager.Instance.OnReplayStart += OnReplayStart;
            ReplayManager.Instance.OnReplayStop += OnReplayStop;

            ReplayManager.Instance.OnClear += OnClear;
        }

        protected virtual void OnEnable()
        {
            
        }

        public virtual void OnRecordingStart()
        {
            
        }

        protected virtual void Update()
        {
            if (ReplayManager.Instance.isRecording)
            {
                if (ReplayManager.Instance.isRecording && !ReplayManager.Instance.noRecordStates.Contains(GlobalVariables.Instance.GameState))
                    Recording();
            }

            if (ReplayManager.Instance.isReplaying && !ReplayManager.Instance.isPaused)
                Replay(ReplayManager.Instance.GetReplayTime());
        }

        protected virtual void Recording()
        {
			
        }

        public virtual void OnRecordingStop()
        {

        }

        public virtual void OnReplayStart()
        {
            //gameObject.SetActive(true);
        }

        public virtual void Replay(float t)
        {
            //Debug.Log (t + " " + name);
        }

        public virtual void OnReplayStop()
        {
        }

        protected virtual void OnDestroy()
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

