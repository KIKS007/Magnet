using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Replay
{
	public class ReplayColor : MonoBehaviour 
	{
		public bool recordEmission = true;

		public TimelinedColor color = new TimelinedColor ();
		public TimelinedColor emissionColor = new TimelinedColor ();

		protected Material material;

		protected virtual void Start ()
		{
			ReplayManager.Instance.OnReplayTimeChange += Replay;
			ReplayManager.Instance.OnReplayStart += OnReplayStart;
			ReplayManager.Instance.OnReplayStop += OnReplayStop;

			material = GetComponent<Renderer> ().material;
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
			color.Add (
				material.color.r, 
				material.color.g, 
				material.color.b, 
				material.color.a);

			if(recordEmission)
				emissionColor.Add (
					material.GetColor ("_EmissionColor").r, 
					material.GetColor ("_EmissionColor").g, 
					material.GetColor ("_EmissionColor").b, 
					material.GetColor ("_EmissionColor").a);
		}

		public void OnReplayStart ()
		{

		}

		public void OnReplayStop ()
		{

		}

		public void Replay (float t)
		{
			if(color.a.keys.Length > 0)
				color.Set (t, material);

			if(emissionColor.a.keys.Length > 0)
				emissionColor.Set (t, material, true);
		}

		public void OnDestroy ()
		{
			ReplayManager.Instance.OnReplayTimeChange -= Replay;
			ReplayManager.Instance.OnReplayStart -= OnReplayStart;
			ReplayManager.Instance.OnReplayStop -= OnReplayStop;
		}

		[Serializable]
		public class TimelinedColor
		{
			public AnimationCurve r;
			public AnimationCurve g;
			public AnimationCurve b;
			public AnimationCurve a;

			public void Add (float r, float g, float b, float a)
			{
				float time = ReplayManager.Instance.GetCurrentTime ();
				this.r.AddKey (time, r);
				this.g.AddKey (time, g);
				this.b.AddKey (time, b);
				this.a.AddKey (time, a);
			}

			public void Set (float _time, Material _material, bool emissionColor = false)
			{
				Color color = new Color (r.Evaluate (_time), g.Evaluate (_time), b.Evaluate (_time), a.Evaluate (_time));

				if (!emissionColor)
					_material.color = color;
				else
					_material.SetColor ("_EmissionColor", color);
			}
		}
	}
}

