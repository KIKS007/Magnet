/* IN-GAME REPLAY - @madebyfeno - <feno@ironequal.com>
 * You can use it in commercial projects (and non-commercial project of course), modify it and share it.
 * Do not resell the resources of this project as-is or even modified. 
 * TL;DR: Do what the fuck you want but don't re-sell it
 * 
 * ironequal.com
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.AI;
using Sirenix.OdinInspector;

namespace Replay
{
    public class ReplayEntity : ReplayComponent
    {
        [Header("Settings")]
        public bool recordPosition = true;
        public bool recordRotation = true;
        public bool recordScale = true;
        public bool recordEnable = true;

        [Header("Data")]
        public RecordData data = new RecordData();
        public List<EnableData> enableData = new List<EnableData>();

        private Rigidbody rigidBody;
        private NavMeshAgent agent;
        private Animator animator;

        private LayerMask initialLayer;

        protected override void Start()
        {
            base.Start();

            initialLayer = gameObject.layer;

            rigidBody = GetComponent<Rigidbody>();
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (ReplayManager.Instance.isRecording && gameObject.layer == LayerMask.NameToLayer("Movables") && recordScale)
                data.scale.Add(Vector3.zero);

            if (ReplayManager.Instance.isRecording && recordEnable)
                enableData.Add(new EnableData(true));
        }

        void OnDisable()
        {
            if (GlobalVariables.applicationIsQuitting)
                return;
            
            if (ReplayManager.Instance.isRecording && recordEnable)
                enableData.Add(new EnableData(false));
        }

        public override void OnClear()
        {
            base.OnClear();
            data = new RecordData();
            enableData = new List<EnableData>();
        }

        public override void OnRecordingStart()
        {
            base.OnRecordingStart();

            if (recordEnable)
                enableData.Add(new EnableData(gameObject.activeSelf));
        }

        protected override void Recording()
        {
            base.Recording();

            if (recordPosition)
                data.position.Add(transform.position);
				
            if (recordRotation)
                data.rotation.Add(transform.rotation);
				
            if (recordScale)
                data.scale.Add(transform.localScale);

            //enableData.Add(new EnableData(transform, gameObject.activeSelf));
        }

        public override void OnRecordingStop()
        {
            base.OnRecordingStop();

            if (recordEnable)
                enableData.Add(new EnableData(gameObject.activeSelf));
        }

        public override void OnReplayStart()
        {
            base.OnReplayStart();

            if (recordEnable)
            {
                if (tag == "Player" && GlobalVariables.Instance.EnabledPlayersList.Contains(gameObject) || tag != "Player")
                    SetEnable(enableData[0].enabled);
            }

            rigidBody = GetComponent<Rigidbody>();

            if (rigidBody != null)
                rigidBody.isKinematic = true;

            if (agent)
                agent.enabled = false;

            if (animator)
                animator.enabled = false;	
        }

        void SetEnable(bool enable)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            if (enable)
                SetLayerRecursively(gameObject, initialLayer);
            else
                SetLayerRecursively(gameObject, LayerMask.NameToLayer("Disabled"));
        }

        void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (null == obj)
            {
                return;
            }

            obj.layer = newLayer;

            foreach (Transform child in obj.transform)
            {
                if (null == child)
                {
                    continue;
                }
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }

        public override void OnReplayStop()
        {
            base.OnReplayStop();

            rigidBody = GetComponent<Rigidbody>();

            if (rigidBody != null)
                rigidBody.isKinematic = false;

            if (agent)
                agent.enabled = true;

            if (animator)
                animator.enabled = true;	
        }

        public override void Replay(float t)
        {
            base.Replay(t);

            if (recordPosition && data.position.x.keys.Length > 0)
                transform.position = data.position.Get(t);
				
            if (recordRotation && data.rotation.x.keys.Length > 0)
                transform.rotation = data.rotation.Get(t);
				
            if (recordScale && data.scale.x.keys.Length > 0)
                transform.localScale = data.scale.Get(t);

            /* for(int i = enableData.Count - 1; i >= 0; i--)
            {
                
            }*/

            bool enable = enableData[0].enabled;

            foreach (var d in enableData)
            {
                if (d.time <= t)
                    enable = d.enabled;
                else
                {
                    break;
                }
            }

            SetEnable(enable);
        }
    }

    [Serializable]
    public class EnableData
    {
        public float time;
        public bool enabled;

        public EnableData(bool enable)
        {
            time = ReplayManager.Instance.GetCurrentTime();
            Debug.Log("Enable: " + enable + " at " + time);
            enabled = enable;
        }
    }

    [Serializable]
    public class TimelinedVector3
    {
        public AnimationCurve x = new AnimationCurve();
        public AnimationCurve y = new AnimationCurve();
        public AnimationCurve z = new AnimationCurve();

        public void Add(Vector3 v)
        {
            x.AddKey(ReplayManager.Instance.GetCurrentTime(), v.x);
            y.AddKey(ReplayManager.Instance.GetCurrentTime(), v.y);
            z.AddKey(ReplayManager.Instance.GetCurrentTime(), v.z);
        }

        public Vector3 Get(float _time)
        {
            return new Vector3(x.Evaluate(_time), y.Evaluate(_time), z.Evaluate(_time));
        }
    }

    [Serializable]
    public class TimelinedQuaternion
    {
        public AnimationCurve x = new AnimationCurve();
        public AnimationCurve y = new AnimationCurve();
        public AnimationCurve z = new AnimationCurve();
        public AnimationCurve w = new AnimationCurve();

        public void Add(Quaternion v)
        {
            x.AddKey(ReplayManager.Instance.GetCurrentTime(), v.x);
            y.AddKey(ReplayManager.Instance.GetCurrentTime(), v.y);
            z.AddKey(ReplayManager.Instance.GetCurrentTime(), v.z);
            w.AddKey(ReplayManager.Instance.GetCurrentTime(), v.w);
        }

        public Quaternion Get(float _time)
        {
            return new Quaternion(x.Evaluate(_time), y.Evaluate(_time), z.Evaluate(_time), w.Evaluate(_time));
        }
    }

    [Serializable]
    public class RecordData
    {
        public TimelinedVector3 position = new TimelinedVector3();
        public TimelinedQuaternion rotation = new TimelinedQuaternion();
        public TimelinedVector3 scale = new TimelinedVector3();

        public void Add(Transform t)
        {
            position.Add(t.position);
            rotation.Add(t.rotation);
            scale.Add(t.localScale);
        }

        public void Set(float _time, Transform _transform)
        {
            _transform.position = position.Get(_time);
            _transform.rotation = rotation.Get(_time);
            _transform.localScale = scale.Get(_time);
        }
    }
}
