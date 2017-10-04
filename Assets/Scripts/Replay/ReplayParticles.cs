using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace Replay
{
    public class ReplayParticles : ReplayComponent
    {
        public bool listParticle = true;

        [Header("Data")]
        public List<TimelinedParticles> particles = new List<TimelinedParticles>();

        private List<TimelinedParticles> _particles = new List<TimelinedParticles>();

        protected ParticleSystem particleSys;
        protected float listRecordEpsilon = 0.008f;

        protected override void Start()
        {
            if (!ReplayManager.Instance.replayEnabled)
                return;

            listRecordEpsilon = ReplayManager.Instance.listRecordEpsilon;

            if (GetComponent<ParticlesAutoDestroy>())
            {
                if (!listParticle)
                    Destroy(GetComponent<ParticlesAutoDestroy>());
                else
                    GetComponent<ParticlesAutoDestroy>().replayParticles = true;
            }

            particleSys = GetComponent<ParticleSystem>();

            if (listParticle)
                ReplayManager.Instance.particlesReplay.Add(new ReplayManager.Particles(particleSys));

            base.Start();
        }

        public override void OnClear()
        {
            if (listParticle)
                return;

            base.OnClear();
            particles.Clear();
        }

        public override void OnRecordingStart()
        {
            if (listParticle)
                return;
			
            base.OnRecordingStart();

            particles = new List<TimelinedParticles>();

            if (particleSys != null)
                particles.Add(new TimelinedParticles(particleSys));
            else
                Debug.LogWarning("No ParticleSystem!"); 
        }

        protected override void Recording()
        {
            if (listParticle)
                return;
			
            base.Recording();

            particles.Add(new TimelinedParticles(particleSys));
        }

        public override void OnReplayStart()
        {
            if (listParticle)
                return;
			
            gameObject.SetActive(true);

            _particles = new List<TimelinedParticles>(particles);

            base.OnReplayStart();

            particleSys.Play(true);
            particleSys.Pause(true);
        }

        public override void OnReplayStop()
        {
            if (listParticle)
                return;
			
            base.OnReplayStop();

        }

        public override void Replay(float t)
        {
            if (listParticle)
                return;
			
            base.Replay(t);

            if (particles == null)
            {
                Debug.LogWarning("No Particles!");
                return;
            }

            if (particles.Count == 0)
                return;

            if (t < particles[0].time)
            {
                particleSys.Clear();
                return;
            }

            if (t > particles[particles.Count - 1].time)
            {
                return;
            }

            TimelinedParticles timelinedParticles = null;

            foreach (var p in _particles)
            {
                if (Mathf.Abs(p.time - t) < listRecordEpsilon)
                {
                    timelinedParticles = p;

                    particleSys.SetParticles(p.particles, p.particles.Length);
                    break;
                }
            }

            if (timelinedParticles != null)
                _particles.Remove(timelinedParticles);
        }
    }

    [Serializable]
    public class TimelinedParticles
    {
        public float time;
        public ParticleSystem.Particle[] particles = new ParticleSystem.Particle[0];

        public TimelinedParticles(ParticleSystem _ps)
        {
            time = ReplayManager.Instance.GetCurrentTime();

            particles = new ParticleSystem.Particle[_ps.particleCount];
            _ps.GetParticles(particles);
        }
    }
}

