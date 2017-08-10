using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DarkTonic.MasterAudio;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Demos;
using Replay;

public class ArenaVisualizer : MonoBehaviour
{
	public enum Bounce { Bounce, Wait, Reset }

	[Space (20)]

	public AudioSpectrum.LevelsType levelsType = AudioSpectrum.LevelsType.Mean;
	public bool normalizedValues = true;

	[Header ("Settings")]
	public float factor = 100;
	public float minimumHeight = 0.2f;

	[Header ("Normalized Settings")]
	public float normalizedFactor = 100;
	public float normalizedMinimumHeight = 0.2f;

	[Header ("Emission")]
	public bool emissionChange = false;
	public float minEmission = 0.5f;
	public float maxEmission = 3f;

	[Header ("Columns")]
	public Transform[] frontColumns = new Transform[27];
	public Transform[] backColumns = new Transform[27];
	public Transform[] rightColumns = new Transform[17];
	public Transform[] leftColumns = new Transform[17];

	[Header ("Arena Settings")]
	public int currentSettings = 0;
	public List<ArenaSettings> allSettings = new List<ArenaSettings> ();

	[Header ("Color Bounce")]
	public List<BounceSettings> bounceSettings;

	[Header ("Single Color Bounce")]
	public float singleColorBounceDuration = 0.2f;
	public float singleColorBounceResetDuration = 0.1f;

	private Renderer[] columnsRenderers = new Renderer[0];
	private bool wrongSettings = false;
	private Color columnInitialColor;
	private Color columnDeadlyColor;

	private List<ColumnMaterial> frontMaterials = new List<ColumnMaterial> ();
	private List<ColumnMaterial> backMaterials = new List<ColumnMaterial> ();
	private List<ColumnMaterial> rightMaterials = new List<ColumnMaterial> ();
	private List<ColumnMaterial> leftMaterials = new List<ColumnMaterial> ();

	void Start ()
	{
		columnInitialColor = frontColumns [0].GetChild (0).GetComponent<Renderer> ().material.color;
		columnDeadlyColor = GetComponent<ArenaDeadzones> ().deadlyColor;

		columnsRenderers = transform.GetComponentsInChildren<Renderer> ();

		GlobalVariables.Instance.OnEndMode += DoColorBounce;

		foreach(ArenaSettings arena in allSettings)
		{
			if(arena.frontIndex.Length != frontColumns.Length || 
				arena.backIndex.Length != backColumns.Length ||
				arena.rightIndex.Length != rightColumns.Length ||
				arena.leftIndex.Length != leftColumns.Length ||
				frontColumns.Length != 27 ||
				backColumns.Length != 27 ||
				rightColumns.Length != 15 ||
				leftColumns.Length != 15)
			{
				wrongSettings = true;
				Debug.LogWarning ("Wrong Arena Settings !");
			}
		}

		float currentMinimumHeight = normalizedValues ? normalizedMinimumHeight : minimumHeight;
	
		for(int i = 0; i < frontColumns.Length; i++)
		{
			Vector3 scale = frontColumns[i].localScale;
			scale.y = currentMinimumHeight;
			frontColumns[i].localScale = scale;
		}

		for(int i = 0; i < backColumns.Length; i++)
		{
			Vector3 scale = backColumns[i].localScale;
			scale.y = currentMinimumHeight;
			backColumns[i].localScale = scale;
		}

		for(int i = 0; i < rightColumns.Length; i++)
		{
			Vector3 scale = rightColumns[i].localScale;
			scale.y = currentMinimumHeight;
			rightColumns[i].localScale = scale;
		}

		for(int i = 0; i < leftColumns.Length; i++)
		{
			Vector3 scale = leftColumns[i].localScale;
			scale.y = currentMinimumHeight;
			leftColumns[i].localScale = scale;
		}

		GetMaterials ();
	}

	void OnDestroy ()
	{
		if(GlobalVariables.Instance)
			GlobalVariables.Instance.OnEndMode -= DoColorBounce;
	}

	void GetMaterials ()
	{
		foreach(Transform pivot in frontColumns)
		{
			foreach (Transform c in pivot)
			{
				frontMaterials.Add (new ColumnMaterial ());
				frontMaterials [frontMaterials.Count - 1].materials.Add (c.GetComponent<Renderer> ().material);
			}
		}

		foreach(Transform pivot in backColumns)
		{
			foreach (Transform c in pivot)
			{
				backMaterials.Add (new ColumnMaterial ());
				backMaterials [backMaterials.Count - 1].materials.Add (c.GetComponent<Renderer> ().material);
			}
		}

		foreach(Transform pivot in rightColumns)
		{
			foreach (Transform c in pivot)
			{
				rightMaterials.Add (new ColumnMaterial ());
				rightMaterials [rightMaterials.Count - 1].materials.Add (c.GetComponent<Renderer> ().material);
			}
		}

		foreach(Transform pivot in leftColumns)
		{
			foreach (Transform c in pivot)
			{
				leftMaterials.Add (new ColumnMaterial ());
				leftMaterials [leftMaterials.Count - 1].materials.Add (c.GetComponent<Renderer> ().material);
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

		if (ReplayManager.Instance.isReplaying)
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

			if(emissionChange)
				ColumnEmission (frontColumns[i], frontMaterials [i], index);
		}

		for(int i = 0; i < backColumns.Length; i++)
		{
			int index = allSettings [currentSettings].backIndex [i];

			Vector3 scale = backColumns[i].localScale;
			scale.y = NewHeight (currentMinimumHeight, index, currentFactor);
			backColumns[i].localScale = scale;

			if(emissionChange)
				ColumnEmission (backColumns[i], backMaterials [i], index);
		}

		for(int i = 0; i < rightColumns.Length; i++)
		{
			int index = allSettings [currentSettings].rightIndex [i];

			Vector3 scale = rightColumns[i].localScale;
			scale.y = NewHeight (currentMinimumHeight, index, currentFactor);
			rightColumns[i].localScale = scale;

			if(emissionChange)
				ColumnEmission (rightColumns[i], rightMaterials [i], index);
		}

		for(int i = 0; i < leftColumns.Length; i++)
		{
			int index = allSettings [currentSettings].leftIndex [i];

			Vector3 scale = leftColumns[i].localScale;
			scale.y = NewHeight (currentMinimumHeight, index, currentFactor);
			leftColumns[i].localScale = scale;

			if(emissionChange)
				ColumnEmission (leftColumns[i], leftMaterials [i], index);
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
		case AudioSpectrum.LevelsType.Peak:
			if(!normalizedValues)
				return currentMinimumHeight + AudioSpectrum.Instance.PeakLevels[index] * currentFactor;
			else
				return currentMinimumHeight + AudioSpectrum.Instance.PeakLevelsNormalized[index] * currentFactor;
		case AudioSpectrum.LevelsType.Mean:
			if(!normalizedValues)
				return currentMinimumHeight + AudioSpectrum.Instance.MeanLevels[index] * currentFactor;
			else
				return currentMinimumHeight + AudioSpectrum.Instance.MeanLevelsNormalized[index] * currentFactor;
		default:
			if(!normalizedValues)
				return currentMinimumHeight + AudioSpectrum.Instance.Levels[index] * currentFactor;
			else
				return currentMinimumHeight + AudioSpectrum.Instance.LevelsNormalized[index] * currentFactor;
		}
	}

	void ColumnEmission (Transform pivot, ColumnMaterial columnMaterial, int index)
	{
		float emissionIntensity = 0;

		switch(levelsType)
		{
		case AudioSpectrum.LevelsType.Basic:
			emissionIntensity = (maxEmission - minEmission) * AudioSpectrum.Instance.LevelsNormalized[index] + minEmission;
			break;
		case AudioSpectrum.LevelsType.Peak:
			emissionIntensity = (maxEmission - minEmission) * AudioSpectrum.Instance.PeakLevelsNormalized[index] + minEmission;
			break;
		case AudioSpectrum.LevelsType.Mean:
			emissionIntensity = (maxEmission - minEmission) * AudioSpectrum.Instance.MeanLevelsNormalized [index] + minEmission;
			break;
		}

		foreach(Material m in columnMaterial.materials)
		{
			if(pivot.GetChild (0).tag != "DeadZone")
				m.SetColor ("_EmissionColor", columnInitialColor * emissionIntensity);
			else
				m.SetColor ("_EmissionColor", columnDeadlyColor * emissionIntensity);
		}

//		foreach(Transform c in column)
//		{
//			if(c.tag != "DeadZone")
//				c.GetComponent<Renderer> ().material.SetColor ("_EmissionColor", columnInitialColor * emissionIntensity);
//			else
//				c.GetComponent<Renderer> ().material.SetColor ("_EmissionColor", columnDeadlyColor * emissionIntensity);
//		}
	}

	[PropertyOrder (-1)]
	[Button]
	public void DoColorBounce ()
	{
		StartCoroutine (ColorBounce ());
	}

	IEnumerator ColorBounce ()
	{
		if (StatsManager.Instance.winnerName == WhichPlayer.Draw || StatsManager.Instance.winnerName == WhichPlayer.None)
			yield break;

		if (ReplayManager.Instance.isReplaying)
			yield break;

		foreach(var b in bounceSettings)
		{
			switch (b.bounceType)
			{
			case Bounce.Bounce:
				foreach(Renderer r in columnsRenderers)
				{
					r.material.DOColor (GlobalVariables.Instance.playersColors [(int)StatsManager.Instance.winnerName], b.duration).SetEase (Ease.OutQuad);
					r.material.DOColor (GlobalVariables.Instance.playersColors [(int)StatsManager.Instance.winnerName], "_EmissionColor", b.duration).SetEase (Ease.OutQuad);
				}
				if(b.wait)
					yield return new WaitForSecondsRealtime (b.duration);
				break;
			case Bounce.Wait:
				yield return new WaitForSecondsRealtime (b.duration);
				break;
			case Bounce.Reset:
				foreach(Renderer r in columnsRenderers)
				{
					r.material.DOColor (columnInitialColor, b.duration).SetEase (Ease.OutQuad);
					r.material.DOColor (columnInitialColor, "_EmissionColor", b.duration).SetEase (Ease.OutQuad);
				}
				if(b.wait)
					yield return new WaitForSecondsRealtime (b.duration);
				break;
			}
		}

		yield return 0;
	}

	[PropertyOrder (-1)]
	[Button ("Do Single Color Bounce")]
	public void SingleColorBounce ()
	{
		StartCoroutine (SingleColorBounceCoroutine ());
	}

	public IEnumerator SingleColorBounceCoroutine (PlayerName player = PlayerName.Player3)
	{
		foreach(Renderer r in columnsRenderers)
		{
			if(r.material.color == columnInitialColor)
			{
				r.material.DOColor (GlobalVariables.Instance.playersColors [(int)player], singleColorBounceDuration).SetEase (Ease.OutQuad);
				r.material.DOColor (GlobalVariables.Instance.playersColors [(int)player], "_EmissionColor", singleColorBounceDuration).SetEase (Ease.OutQuad);
			}
		}

		yield return new WaitForSecondsRealtime (singleColorBounceDuration);

		foreach(Renderer r in columnsRenderers)
		{
			if(r.material.color == GlobalVariables.Instance.playersColors [(int)player])
			{
				r.material.DOColor (columnInitialColor, singleColorBounceResetDuration).SetEase (Ease.OutQuad);
				r.material.DOColor (columnInitialColor, "_EmissionColor", singleColorBounceResetDuration).SetEase (Ease.OutQuad);
			}
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

[Serializable]
public class ColumnMaterial 
{
	public List<Material> materials = new List<Material> ();
}

[Serializable]
public class BounceSettings 
{
	public ArenaVisualizer.Bounce bounceType;
	public float duration;
	public bool wait = true;
}
