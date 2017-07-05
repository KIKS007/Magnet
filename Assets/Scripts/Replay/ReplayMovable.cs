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

	public class ReplayMovable : MonoBehaviour
	{
		public TimelinedMovableColor colors = new TimelinedMovableColor ();

		[HideInInspector]
		public Material cubeMaterial;

		protected MovableScript cubeScript;

		protected virtual void Start ()
		{
			ReplayManager.Instance.OnReplayTimeChange += Replay;
			ReplayManager.Instance.OnReplayStart += OnReplayStart;
			ReplayManager.Instance.OnReplayStop += OnReplayStop;

			cubeScript = GetComponent<MovableScript> ();

			cubeMaterial = transform.GetChild (1).GetComponent<Renderer> ().material;
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
			if(colors.blue.keys.Length > 0)
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
