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


namespace Replay
{
	[Serializable]
	public class TimelinedMovableColor
	{
		public AnimationCurve blue;
		public AnimationCurve pink;
		public AnimationCurve green;
		public AnimationCurve yellow;
		public AnimationCurve red;

		public void Add (float b, float p, float g, float y, float r)
		{
			float time = ReplayManager.Instance.GetCurrentTime ();
			blue.AddKey (time, b);
			pink.AddKey (time, p);
			green.AddKey (time, g);
			yellow.AddKey (time, y);
			red.AddKey (time, r);
		}

		public void Set (float _time, Material _material)
		{
			_material.SetFloat ("_LerpBLUE", blue.Evaluate (_time));
			_material.SetFloat ("_LerpPINK", pink.Evaluate (_time));
			_material.SetFloat ("_LerpGREEN", green.Evaluate (_time));
			_material.SetFloat ("_LerpYELLOW", yellow.Evaluate (_time));
			_material.SetFloat ("_LerpRED", red.Evaluate (_time));
		}
	}

	[Serializable]
	public class TimelinedDeadlyParticles
	{
		public AnimationCurve deadParticle;
		public AnimationCurve deadParticle2;

		public void Add (Vector2 particles)
		{
			float time = ReplayManager.Instance.GetCurrentTime ();
			deadParticle.AddKey (time, particles.x);
			deadParticle2.AddKey (time, particles.y);
		}

		public void Set (float _time, ParticleSystem ps1, ParticleSystem ps2)
		{
			if (deadParticle.Evaluate (_time) > 0.5f && !ps1.isEmitting)
				ps1.Play ();

			if (deadParticle2.Evaluate (_time) > 0.5f && !ps2.isEmitting)
				ps2.Play ();

			if (deadParticle.Evaluate (_time) < 0.5f && ps1.isEmitting)
				ps1.Stop ();

			if (deadParticle2.Evaluate (_time) < 0.5f && ps2.isEmitting)
				ps2.Stop ();
		}
	}

	public class ReplayMovable : MonoBehaviour
	{
		public TimelinedMovableColor colors = new TimelinedMovableColor ();
		public TimelinedDeadlyParticles deadlyParticles = new TimelinedDeadlyParticles ();

		[HideInInspector]
		public Material cubeMaterial;

		protected MovableScript cubeScript;

		protected ParticleSystem deadlyParticle;
		protected ParticleSystem deadlyParticle2;

		protected virtual void Start ()
		{
			ReplayManager.Instance.OnReplayTimeChange += Replay;
			ReplayManager.Instance.OnReplayStart += OnReplayStart;
			ReplayManager.Instance.OnReplayStop += OnReplayStop;

			cubeScript = GetComponent<MovableScript> ();

			cubeMaterial = transform.GetChild (1).GetComponent<Renderer> ().material;
			deadlyParticle = transform.GetChild (3).GetComponent<ParticleSystem> ();
			deadlyParticle2 = transform.GetChild (4).GetComponent<ParticleSystem> ();

			deadlyParticle.Stop ();
			deadlyParticle2.Stop ();
		}

		void OnEnable ()
		{
			StartCoroutine (Recording ());
		}

		IEnumerator Recording ()
		{
			while (true) 
			{
				yield return new WaitForSeconds (1 / ReplayManager.Instance.recordRate);

				if (ReplayManager.Instance.isRecording && !ReplayManager.Instance.noRecordStates.Contains (GlobalVariables.Instance.GameState)) 
					Record ();
			}
		}

		protected virtual void Record ()
		{
			RecordColors ();
			RecordParticles ();
		}

		void RecordColors ()
		{
			float b = cubeMaterial.GetFloat ("_LerpBLUE");
			float p = cubeMaterial.GetFloat ("_LerpPINK");
			float g = cubeMaterial.GetFloat ("_LerpGREEN");
			float y = cubeMaterial.GetFloat ("_LerpYELLOW");
			float r = cubeMaterial.GetFloat ("_LerpRED");

			colors.Add (b, p, g, y, r);
		}

		void RecordParticles ()
		{
			int x = deadlyParticle.isEmitting ? 1 : 0;
			int y = deadlyParticle.isEmitting ? 1 : 0;

			deadlyParticles.Add (new Vector2(x, y));
		}
			

		public void OnReplayStart ()
		{
			
		}

		public void OnReplayStop ()
		{
			
		}

		public void Replay (float t)
		{
			colors.Set (t, cubeMaterial);

			deadlyParticles.Set (t, deadlyParticle, deadlyParticle2);
		}

		public void OnDestroy ()
		{
			ReplayManager.Instance.OnReplayTimeChange -= Replay;
			ReplayManager.Instance.OnReplayStart -= OnReplayStart;
			ReplayManager.Instance.OnReplayStop -= OnReplayStop;
		}
	}
}
