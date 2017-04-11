using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ArenaVisualizer : MonoBehaviour 
{
	public AudioSpectrum.LevelsType levelsType = AudioSpectrum.LevelsType.Mean;
	public bool normalizedValues = true;

	[Header ("Settings")]
	public float factor = 100;
	public float minimumHeight = 0.2f;

	[Header ("Normalized Settings")]
	public float normalizedFactor = 100;
	public float normalizedMinimumHeight = 0.2f;

	[Header ("Columns")]
	public Transform[] frontColumns = new Transform[27];
	public Transform[] backColumns = new Transform[27];
	public Transform[] rightColumns = new Transform[17];
	public Transform[] leftColumns = new Transform[17];

	[Header ("Arena Settings")]
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
			frontColumns [i].name = "Pivot " + i;

		for (int i = 0; i < backColumns.Length; i++)
			backColumns [i].name = "Pivot " + i;

		for (int i = 0; i < rightColumns.Length; i++)
			rightColumns [i].name = "Pivot " + i;

		for (int i = 0; i < leftColumns.Length; i++)
			leftColumns [i].name = "Pivot " + i;
	}

	// Update is called once per frame
	void Update () 
	{
		if (AudioSpectrum.Instance == null || wrongSettings)
			return;

		if (currentSettings >= allSettings.Count)
			return;

		float currentFactor = normalizedValues ? normalizedFactor : factor;
		float currentMinimumHeight = normalizedValues ? normalizedMinimumHeight : minimumHeight;

		for(int i = 0; i < frontColumns.Length; i++)
		{
			int index = allSettings [currentSettings].frontIndex [i];

			Vector3 scale = frontColumns[i].localScale;
			scale.y = NewHeight (currentMinimumHeight, index, currentFactor);
			frontColumns[i].localScale = scale;
		}

		for(int i = 0; i < backColumns.Length; i++)
		{
			int index = allSettings [currentSettings].backIndex [i];

			Vector3 scale = backColumns[i].localScale;
			scale.y = NewHeight (currentMinimumHeight, index, currentFactor);
			backColumns[i].localScale = scale;
		}

		for(int i = 0; i < rightColumns.Length; i++)
		{
			int index = allSettings [currentSettings].rightIndex [i];

			Vector3 scale = rightColumns[i].localScale;
			scale.y = NewHeight (currentMinimumHeight, index, currentFactor);
			rightColumns[i].localScale = scale;
		}

		for(int i = 0; i < leftColumns.Length; i++)
		{
			int index = allSettings [currentSettings].leftIndex [i];

			Vector3 scale = leftColumns[i].localScale;
			scale.y = NewHeight (currentMinimumHeight, index, currentFactor);
			leftColumns[i].localScale = scale;
		}
	}

	float NewHeight (float currentMinimumHeight, int index, float currentFactor)
	{
		switch(levelsType)
		{
		case AudioSpectrum.LevelsType.Basic:
			if(!normalizedValues)
				return currentMinimumHeight + AudioSpectrum.Instance.Levels[index] * currentFactor;
			else
				return currentMinimumHeight + AudioSpectrum.Instance.LevelsNormalized[index] * currentFactor;
			break;
		case AudioSpectrum.LevelsType.Peak:
			if(!normalizedValues)
				return currentMinimumHeight + AudioSpectrum.Instance.PeakLevels[index] * currentFactor;
			else
				return currentMinimumHeight + AudioSpectrum.Instance.PeakLevelsNormalized[index] * currentFactor;
			break;
		case AudioSpectrum.LevelsType.Mean:
			if(!normalizedValues)
				return currentMinimumHeight + AudioSpectrum.Instance.MeanLevels[index] * currentFactor;
			else
				return currentMinimumHeight + AudioSpectrum.Instance.MeanLevelsNormalized[index] * currentFactor;
			break;
		default:
			if(!normalizedValues)
				return currentMinimumHeight + AudioSpectrum.Instance.Levels[index] * currentFactor;
			else
				return currentMinimumHeight + AudioSpectrum.Instance.LevelsNormalized[index] * currentFactor;
			break;
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
