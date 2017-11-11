using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace Replay
{
    public class ReplayColor : ReplayComponent
    {
        [Header("Data")]
        public bool recordEmission = true;

        public TimelinedColor color = new TimelinedColor();
        public TimelinedColor emissionColor = new TimelinedColor();

        protected Material material;

        protected override void Start()
        {
            base.Start();

            material = GetComponent<Renderer>().material;
        }

        public override void OnClear()
        {
            base.OnClear();
            color = new TimelinedColor();
            emissionColor = new TimelinedColor();
        }

        protected override void Recording()
        {
            base.Recording();

            color.Add(
                material.color.r, 
                material.color.g, 
                material.color.b, 
                material.color.a);

            if (recordEmission)
                emissionColor.Add(
                    material.GetColor("_EmissionColor").r, 
                    material.GetColor("_EmissionColor").g, 
                    material.GetColor("_EmissionColor").b, 
                    material.GetColor("_EmissionColor").a);
        }

        public override void OnReplayStart()
        {
            base.OnReplayStart();
        }

        public override void Replay(float t)
        {
            base.Replay(t);

            if (color.a.keys.Length > 0)
                color.Set(t, material);

            if (emissionColor.a.keys.Length > 0)
                emissionColor.Set(t, material, true);
        }

        [Serializable]
        public class TimelinedColor
        {
            public AnimationCurve r = new AnimationCurve();
            public AnimationCurve g = new AnimationCurve();
            public AnimationCurve b = new AnimationCurve();
            public AnimationCurve a = new AnimationCurve();

            public void Add(float r, float g, float b, float a)
            {
                float time = ReplayManager.Instance.GetCurrentTime();
                this.r.AddKey(time, r);
                this.g.AddKey(time, g);
                this.b.AddKey(time, b);
                this.a.AddKey(time, a);
            }

            public void Set(float _time, Material _material, bool emissionColor = false)
            {
                Color color = new Color(r.Evaluate(_time), g.Evaluate(_time), b.Evaluate(_time), a.Evaluate(_time));

                if (!emissionColor)
                    _material.color = color;
                else
                    _material.SetColor("_EmissionColor", color);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (material)
                Destroy(material);
        }
    }
     
}

