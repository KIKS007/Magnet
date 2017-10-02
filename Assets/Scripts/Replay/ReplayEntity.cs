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
        public List<EntityData> entityData = new List<EntityData>();

        private Rigidbody rigidBody;
        private NavMeshAgent agent;
        private Animator animator;

        protected override void Start()
        {
            base.Start();

            rigidBody = GetComponent<Rigidbody>();
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (recordEnable)
                data.AddEnable(true);
        }

        void OnDisable()
        {
            if (gameObject && recordEnable && !ReplayManager.applicationIsQuitting)
                data.AddEnable(false);
        }

        public override void OnClear()
        {
            base.OnClear();
            data = new RecordData();
        }

        protected override void Recording()
        {
            base.Recording();

            if (ReplayManager.Instance.useAnimationCurves)
            {
                if (recordPosition)
                    data.position.Add(transform.position);
				
                if (recordRotation)
                    data.rotation.Add(transform.rotation);
				
                if (recordScale)
                    data.scale.Add(transform.localScale);
            }
            else
                entityData.Add(new EntityData(transform, this));
        }

        public override void OnReplayStart()
        {
            base.OnReplayStart();

            if (ReplayManager.Instance.useAnimationCurves)
            {
                data.AddEnable(true);
				
                ReplayManager.Instance.SetCurveConstant(data.enabled);
            }
            else
                entityData.Add(new EntityData(transform, this));


            rigidBody = GetComponent<Rigidbody>();

            if (rigidBody != null)
                rigidBody.isKinematic = true;

            if (agent)
                agent.enabled = false;

            if (animator)
                animator.enabled = false;	
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

            if (ReplayManager.Instance.useAnimationCurves)
            {
                ReplayManager.Instance.SetCurveConstant(data.enabled);
				
                if (recordPosition && data.position.x.keys.Length > 0)
                    transform.position = data.position.Get(t);
				
                if (recordRotation && data.rotation.x.keys.Length > 0)
                    transform.rotation = data.rotation.Get(t);
				
                if (recordScale && data.scale.x.keys.Length > 0)
                    transform.localScale = data.scale.Get(t);
				
                if (recordEnable && data.enabled.keys.Length > 0)
                    data.SetEnable(t, transform.gameObject);
            }
            else
            {
                foreach (var d in entityData)
                {
                    if (Mathf.Abs(d.time - t) < ReplayManager.Instance.listRecordEpsilon)
                    {
                        if (recordPosition)
                            transform.position = d.position;
						
                        if (recordRotation)
                            transform.rotation = d.rotation;
						
                        if (recordScale)
                            transform.localScale = d.scale;
						
                        if (recordEnable)
                            gameObject.SetActive(d.enabled);
						
                        break;
                    }
                }
            }
        }
    }

    [Serializable]
    public class EntityData
    {
        public float time;

        public Vector3 position = new Vector3();
        public Quaternion rotation = new Quaternion();
        public Vector3 scale = new Vector3();
        public bool enabled;

        public EntityData(Transform t, ReplayEntity entity)
        {
            time = ReplayManager.Instance.GetCurrentTime();

            if (entity.recordPosition)
                position = t.position;
			
            if (entity.recordRotation)
                rotation = t.rotation;
			
            if (entity.recordScale)
                scale = t.localScale;
			
            if (entity.recordEnable)
                enabled = t.gameObject.activeSelf;
        }
    }

    [Serializable]
    public class TimelinedVector3
    {
        public AnimationCurve x;
        public AnimationCurve y;
        public AnimationCurve z;

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
        public AnimationCurve x;
        public AnimationCurve y;
        public AnimationCurve z;
        public AnimationCurve w;

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
        public TimelinedVector3 position;
        public TimelinedQuaternion rotation;
        public TimelinedVector3 scale;
        public AnimationCurve enabled;

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

            SetEnable(_time, _transform.gameObject);
        }

        public void AddEnable(bool enable)
        {
            if (enabled != null)
                enabled.AddKey(ReplayManager.Instance.GetCurrentTime(), enable ? 1f : 0f);
        }

        public void AddEnable(bool enable, float _time)
        {
            if (enabled != null)
                enabled.AddKey(_time, enable ? 1f : 0f);
        }

        public void SetEnable(float _time, GameObject _gameobject)
        {
            if (enabled.Evaluate(_time) > 0.5f && !_gameobject.activeSelf)
                _gameobject.SetActive(true);

            if (enabled.Evaluate(_time) < 0.5f && _gameobject.activeSelf)
                _gameobject.SetActive(false);
        }
    }
}
