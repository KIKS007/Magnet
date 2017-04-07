using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ArenaVisualizer : MonoBehaviour 
{
	[Header ("Columns")]
	public Transform[] frontColumns = new Transform[27];
	public Transform[] backColumns = new Transform[27];
	public Transform[] rightColumns = new Transform[17];
	public Transform[] leftColumns = new Transform[17];

	[Header ("Settings")]
	public float factor = 100;
	public float minimumHeight = 0.2f;
	public int currentSettings = 0;
	public List<ArenaSettings> allSettings = new List<ArenaSettings> ();

	private bool wrongSettings = false;

	void Start ()
	{
		foreach(ArenaSettings arena in allSettings)
		{
			if(arena.frontIndex.Length != 27 || 
				arena.backIndex.Length != 27 ||
				arena.rightIndex.Length != 17 ||
				arena.leftIndex.Length != 17 ||
				frontColumns.Length != 27 ||
				backColumns.Length != 27 ||
				rightColumns.Length != 17 ||
				leftColumns.Length != 17)
			{
				wrongSettings = true;
				Debug.LogWarning ("Wrong Arena Settings !");
			}
		}


	}

	[ContextMenu ("Rename Columns")]
	public void Rename ()
	{
		for (int i = 0; i < frontColumns.Length; i++)
			frontColumns [i].name = "Cube Pivot " + i;

		for (int i = 0; i < backColumns.Length; i++)
			backColumns [i].name = "Cube Pivot " + i;

		for (int i = 0; i < rightColumns.Length; i++)
			rightColumns [i].name = "Cube Pivot " + i;

		for (int i = 0; i < leftColumns.Length; i++)
			leftColumns [i].name = "Cube Pivot " + i;
	}

	// Update is called once per frame
	void Update () 
	{
		if (AudioSpectrum.Instance == null || wrongSettings)
			return;

		if (currentSettings >= allSettings.Count)
			return;

		for(int i = 0; i < frontColumns.Length; i++)
		{
			int index = allSettings [currentSettings].frontIndex [i];

			Vector3 scale = frontColumns[i].localScale;
			scale.y = minimumHeight + AudioSpectrum.Instance.MeanLevels[index] * factor;
			frontColumns[i].localScale = scale;
		}

		for(int i = 0; i < backColumns.Length; i++)
		{
			int index = allSettings [currentSettings].backIndex [i];

			Vector3 scale = backColumns[i].localScale;
			scale.y = minimumHeight + AudioSpectrum.Instance.MeanLevels[index] * factor;
			backColumns[i].localScale = scale;
		}

		for(int i = 0; i < rightColumns.Length; i++)
		{
			int index = allSettings [currentSettings].rightIndex [i];

			Vector3 scale = rightColumns[i].localScale;
			scale.y = minimumHeight + AudioSpectrum.Instance.MeanLevels[index] * factor;
			rightColumns[i].localScale = scale;
		}

		for(int i = 0; i < leftColumns.Length; i++)
		{
			int index = allSettings [currentSettings].leftIndex [i];

			Vector3 scale = leftColumns[i].localScale;
			scale.y = minimumHeight + AudioSpectrum.Instance.MeanLevels[index] * factor;
			leftColumns[i].localScale = scale;
		}
	}
}

[Serializable]
public class ArenaSettings
{
	public int[] frontIndex = new int[27];
	public int[] backIndex = new int[27];
	public int[] rightIndex = new int[17];
	public int[] leftIndex = new int[17];
}
