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
	public class TimelinedColor
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

		public float GetBlue (float _time)
		{
			return blue.Evaluate (_time);
		}

		public float GetPink (float _time)
		{
			return pink.Evaluate (_time);
		}

		public float GetGreen (float _time)
		{
			return green.Evaluate (_time);
		}

		public float GetYellow (float _time)
		{
			return yellow.Evaluate (_time);
		}

		public float GetRed (float _time)
		{
			return red.Evaluate (_time);
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


	public class ReplayMovable : MonoBehaviour
	{
		public TimelinedColor colors = new TimelinedColor ();

		[HideInInspector]
		public Material cubeMaterial;

		protected ParticleSystem deadlyParticle;
		protected ParticleSystem deadlyParticle2;

		protected virtual void Start ()
		{
			StartCoroutine (Recording ());

			ReplayManager.Instance.OnReplayTimeChange += Replay;
			ReplayManager.Instance.OnReplayStart += OnReplayStart;
			ReplayManager.Instance.OnReplayStop += OnReplayStop;

			cubeMaterial = transform.GetChild (1).GetComponent<Renderer> ().material;
			deadlyParticle = transform.GetChild (3).GetComponent<ParticleSystem> ();
			deadlyParticle2 = transform.GetChild (4).GetComponent<ParticleSystem> ();

			deadlyParticle.Stop ();
			deadlyParticle2.Stop ();
		}

		IEnumerator Recording ()
		{
			while (true) 
			{
				yield return new WaitForSeconds (1 / ReplayManager.Instance.recordRate);

				if (ReplayManager.Instance.isRecording && !ReplayManager.Instance.noRecordStates.Contains (GlobalVariables.Instance.GameState)) 
				{
					RecordColors ();
				}
			}
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
			

		public void OnReplayStart ()
		{
			
		}

		public void OnReplayStop ()
		{
			
		}

		public void Replay (float t)
		{
			colors.Set (t, cubeMaterial);
		}

		public void OnDestroy ()
		{
			ReplayManager.Instance.OnReplayTimeChange -= Replay;
			ReplayManager.Instance.OnReplayStart -= OnReplayStart;
			ReplayManager.Instance.OnReplayStop -= OnReplayStop;
		}
	}
}
