using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace Replay
{
	public class ReplayParticles : ReplayComponent 
	{
		[Header ("Data")]
		public List<TimelinedParticles> particles = new List<TimelinedParticles> ();

		protected ParticleSystem particleSys;

		protected override void Start ()
		{
			particleSys = GetComponent<ParticleSystem> ();

			base.Start ();
		}

		public override void OnClear ()
		{
			base.OnClear ();
			particles.Clear ();
		}

		public override void OnRecordingStart ()
		{
			base.OnRecordingStart ();

			particles = new List<TimelinedParticles> ();

			if (particleSys != null)
				particles.Add (new TimelinedParticles (particleSys));
			else
				Debug.LogWarning ("No ParticleSystem!"); 
		}

		protected override void Recording ()
		{
			base.Recording ();

			particles.Add (new TimelinedParticles (particleSys));
		}

		public override void OnReplayStart ()
		{
			base.OnReplayStart ();

			particleSys.Play (true);
			particleSys.Pause (true);
		}

		public override void OnReplayStop ()
		{
			base.OnReplayStop ();

		}

		public override void Replay (float t)
		{
			base.Replay (t);

			if(particles == null)
			{
				Debug.LogWarning ("No Particles!");
				return;
			}

			if (particles.Count == 0)
				return;
			
			if (t < particles [0].time)
				particleSys.Clear ();

			foreach(var p in particles)
			{
				if(Mathf.Abs (p.time - t) < ReplayManager.Instance.listRecordEpsilon)
				{
					particleSys.SetParticles (p.particles, p.particles.Length);
					break;
				}
			}
		}
	}

	[Serializable]
	public class TimelinedParticles 
	{
		public float time;
		public ParticleSystem.Particle[] particles = new ParticleSystem.Particle[0];

		public TimelinedParticles (ParticleSystem _ps)
		{
			time = ReplayManager.Instance.GetCurrentTime ();

			particles = new ParticleSystem.Particle[_ps.particleCount];
			_ps.GetParticles (particles);
		}
	}
}

