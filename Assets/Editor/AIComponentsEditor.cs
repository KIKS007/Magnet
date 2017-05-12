#pragma warning disable 0618

using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(AIEditor))]
public class AIComponentsEditor : Editor 
{
	public bool isCompiling = false;

	private List<string> names = new List<string>();

	// Update is called once per frame
	void OnEnable ()
	{
		#if UNITY_EDITOR
		names.Clear ();
		MonoScript[] scripts = (MonoScript[])Object.FindObjectsOfTypeIncludingAssets( typeof( MonoScript ) );

		foreach( MonoScript m in scripts )
		{
			if(m != null)
			{
				if(m.GetClass () != null)
				{
					if (m.GetClass ().IsSubclassOf (typeof(AIComponent)))
						names.Add (m.name);
				}

			}
		}

		File.WriteAllText ("Assets/SCRIPTS/AI/Other/AIComponentsEnum.cs", "public enum AIComponents{"+string.Join(",", names.ToArray ())+"};");
		#endif
	}
}
