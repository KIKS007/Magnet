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
using UnityEngine.UI;


namespace Replay
{
    public class ReplayMovable : ReplayComponent
    {
        [Header("Data")]
        public TimelinedMovableColor colors = new TimelinedMovableColor();
        public TimelinedBombTimer timer = new TimelinedBombTimer();
        public List<DeadlyData> deadlyData = new List<DeadlyData>();

        [HideInInspector]
        public Material cubeMaterial;

        protected MovableScript cubeScript;
        protected BombManager bombManager;
        protected Text text;

        protected override void Start()
        {
            base.Start();

            cubeScript = GetComponent<MovableScript>();

            cubeMaterial = transform.GetChild(1).GetComponent<Renderer>().material;

            bombManager = FindObjectOfType<BombManager>();

            if (bombManager != null)
                text = GetComponentInChildren<Text>();

            cubeScript.OnNeutral += () => deadlyData.Add(new DeadlyData(false));
            cubeScript.OnDeadly += () => deadlyData.Add(new DeadlyData(true));
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (ReplayManager.Instance.isRecording)
                StartCoroutine(Enable());
        }

        IEnumerator Enable()
        {
            yield return new WaitUntil(() => GetComponent<MovableScript>() != null);

            if (cubeScript == null)
                cubeScript = GetComponent<MovableScript>();

            deadlyData.Add(new DeadlyData(cubeScript.cubeColor == CubeColor.Deadly));
        }

        void OnDisable()
        {
            if (GlobalVariables.applicationIsQuitting)
                return;

            if (ReplayManager.Instance.isRecording && cubeScript != null)
                deadlyData.Add(new DeadlyData(cubeScript.cubeColor == CubeColor.Deadly));
        }

        public override void OnClear()
        {
            base.OnClear();
            colors = new TimelinedMovableColor();
            timer = new TimelinedBombTimer();
            deadlyData = new List<DeadlyData>();
        }

        public override void OnRecordingStart()
        {
            base.OnRecordingStart();

            deadlyData.Add(new DeadlyData(cubeScript.cubeColor == CubeColor.Deadly));
        }

        protected override void Recording()
        {
            RecordColors();

            if (bombManager != null)
                timer.Add(Mathf.RoundToInt(bombManager.timer));
        }

        public override void OnRecordingStop()
        {
            base.OnRecordingStop();

            deadlyData.Add(new DeadlyData(cubeScript.cubeColor == CubeColor.Deadly));
        }

        void RecordColors()
        {
            float b = cubeMaterial.GetFloat("_LerpBLUE");
            float p = cubeMaterial.GetFloat("_LerpPINK");
            float g = cubeMaterial.GetFloat("_LerpGREEN");
            float y = cubeMaterial.GetFloat("_LerpYELLOW");
            float r = cubeMaterial.GetFloat("_LerpRED");

            colors.Add(b, p, g, y, r);
        }

        public override void OnReplayStart()
        {
            base.OnReplayStart();

            DeadlyEnable(false);
        }

        public override void Replay(float t)
        {
            if (colors.blue.keys.Length > 0)
                colors.Set(t, cubeMaterial);

            if (bombManager != null)
            {
                if (text == null)
                    text = GetComponentInChildren<Text>();

                text.text = timer.Get(t).ToString();
            }

            if (deadlyData.Count == 0)
                return;

            bool enable = false;

            if (deadlyData.Count > 0 && t < deadlyData[0].time)
            {
                DeadlyEnable(false);
                return;
            }

            foreach (var d in deadlyData)
            {
                if (d.time <= t)
                    enable = d.enabled;
                else
                {
                    break;
                }
            }

            DeadlyEnable(enable);
        }

        void DeadlyEnable(bool enable)
        {
            if (enable)
            {
                cubeScript.deadlyParticle.Play();
                cubeScript.deadlyParticle2.Play();
            }
            else
            {
                cubeScript.deadlyParticle.Stop();
                cubeScript.deadlyParticle2.Stop();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (cubeMaterial)
                Destroy(cubeMaterial);
        }
    }

    [Serializable]
    public class DeadlyData
    {
        public float time;
        public bool enabled;

        public DeadlyData(bool enable)
        {
            time = ReplayManager.Instance.GetCurrentTime();
            enabled = enable;
        }
    }

    [Serializable]
    public class TimelinedMovableColor
    {
        public AnimationCurve blue = new AnimationCurve();
        public AnimationCurve pink = new AnimationCurve();
        public AnimationCurve green = new AnimationCurve();
        public AnimationCurve yellow = new AnimationCurve();
        public AnimationCurve red = new AnimationCurve();

        public void Add(float b, float p, float g, float y, float r)
        {
            float time = ReplayManager.Instance.GetCurrentTime();
            blue.AddKey(time, b);
            pink.AddKey(time, p);
            green.AddKey(time, g);
            yellow.AddKey(time, y);
            red.AddKey(time, r);
        }

        public void Set(float _time, Material _material)
        {
            _material.SetFloat("_LerpBLUE", blue.Evaluate(_time));
            _material.SetFloat("_LerpPINK", pink.Evaluate(_time));
            _material.SetFloat("_LerpGREEN", green.Evaluate(_time));
            _material.SetFloat("_LerpYELLOW", yellow.Evaluate(_time));
            _material.SetFloat("_LerpRED", red.Evaluate(_time));
        }
    }

    [Serializable]
    public class TimelinedBombTimer
    {
        public AnimationCurve timer = new AnimationCurve();

        public void Add(int t)
        {
            float time = ReplayManager.Instance.GetCurrentTime();
            timer.AddKey(time, t);
        }

        public float Get(float _time)
        {
            return timer.Evaluate(_time);
        }
    }
}
