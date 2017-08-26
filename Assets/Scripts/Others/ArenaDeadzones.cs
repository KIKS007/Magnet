using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Replay;

public class ArenaDeadzones : MonoBehaviour 
{
	public delegate void ColumnDeadly (Transform column);

	public enum RandomType { Single, AllSettings, CurrentSettings };

	[Header ("Random")]
	public RandomType randomType = RandomType.Single;

	[Header ("Settings")]
	public Ease ease = Ease.OutQuad;
	public int currentSettings = 0;
	public List<ArenaDeadzonesSettings> deadzonesSettings = new List<ArenaDeadzonesSettings> ();

	[Header ("Single")]
	public float singleDelay = 0;
	public float currentInterval;
	public Vector2 intervalLimits = new Vector2 (10, 1);
	public float reducedTime = 1f;

	[Header ("Deadly State")]
	public Color deadlyColor;
	public float zScale = 3f;
	public float xScale = 1.8f;
	public float transitionDuration;

	[Header ("Columns")]
	public Transform[] frontColumns = new Transform[27];
	public Transform[] backColumns = new Transform[27];
	public Transform[] rightColumns = new Transform[17];
	public Transform[] leftColumns = new Transform[17];

	[HideInInspector]
	public List<GameObject> deadlyColumns = new List<GameObject> ();

	// Use this for initialization
	void Start () 
	{
		Setup ();
	}

	public void Setup ()
	{
		int settingsIndex = currentSettings;

		switch (randomType)
		{
		case RandomType.Single:
			StartCoroutine (SingleDeadzone ());
			break;
		case RandomType.AllSettings:
			settingsIndex = Random.Range (0, deadzonesSettings.Count);
			break;
		}

		if(randomType != RandomType.Single)
		{
			StartCoroutine (Deadzones (deadzonesSettings [settingsIndex].frontDelay, deadzonesSettings [settingsIndex].duration, deadzonesSettings [settingsIndex].frontIndex, frontColumns));
			StartCoroutine (Deadzones (deadzonesSettings [settingsIndex].backDelay, deadzonesSettings [settingsIndex].duration, deadzonesSettings [settingsIndex].backIndex, backColumns));
			StartCoroutine (Deadzones (deadzonesSettings [settingsIndex].rightDelay, deadzonesSettings [settingsIndex].duration, deadzonesSettings [settingsIndex].rightIndex, rightColumns));
			StartCoroutine (Deadzones (deadzonesSettings [settingsIndex].leftDelay, deadzonesSettings [settingsIndex].duration, deadzonesSettings [settingsIndex].leftIndex, leftColumns));
		}
	}

	IEnumerator Deadzones (float delay, float duration, int[] indexes, Transform[] columns)
	{
		yield return new WaitForSeconds (delay);

		float waitTime = duration / columns.Length;

		for(int i = 0; i < columns.Length; i++)
		{
			yield return new WaitUntil (() => GlobalVariables.Instance.GameState == GameStateEnum.Playing);

			yield return new WaitWhile (() => ReplayManager.Instance.isReplaying);

			for(int j = 0; j  < indexes.Length; j++)
			{
				if (indexes [j] == i)
					SetDeadly (columns [j]);
			}

			yield return new WaitForSeconds (waitTime);
		}

		if (GlobalVariables.Instance.CurrentModeLoaded == WhichMode.Crush)
			SteamAchievements.Instance.UnlockAchievement (AchievementID.ACH_CRUSH);
	}

	IEnumerator SingleDeadzone ()
	{
		currentInterval = intervalLimits.x;

		List<Transform> allColumns = new List<Transform> (frontColumns);
		allColumns.AddRange (backColumns);
		allColumns.AddRange (rightColumns);
		allColumns.AddRange (leftColumns);

		yield return new WaitForSeconds (singleDelay);

		int columnsCount = frontColumns.Length + backColumns.Length + rightColumns.Length + leftColumns.Length;

		for(int i = 0; i < columnsCount; i++)
		{
			yield return new WaitUntil (() => GlobalVariables.Instance.GameState == GameStateEnum.Playing);

			yield return new WaitWhile (() => ReplayManager.Instance.isReplaying);

			Transform column = null;

			do
			{
				column = allColumns [Random.Range (0, allColumns.Count)];
			}
			while(column.GetChild (0).tag == "DeadZone");

			SetDeadly (column);

			yield return new WaitForSeconds (currentInterval);

			currentInterval -= reducedTime;
			if (currentInterval < intervalLimits.y)
				currentInterval = intervalLimits.y;
		}
	}

	void SetDeadly (Transform column)
	{
		for(int i = 0; i < column.childCount; i++)
		{
			column.GetChild (i);

			column.GetChild (i).tag = "DeadZone";
			column.GetChild (i).GetComponent<Collider> ().enabled = true;

			column.GetChild (i).GetComponent<Renderer> ().material.DOColor (deadlyColor, "_EmissionColor", transitionDuration).SetUpdate (false);
			column.GetChild (i).GetComponent<Renderer> ().material.DOColor (deadlyColor, transitionDuration).SetUpdate (false);

			column.GetChild (i).DOScaleZ (zScale, transitionDuration).SetUpdate (false);

			if(i == 0)
				column.GetChild (i).DOScaleX (xScale, transitionDuration).SetUpdate (false);
			else
				column.GetChild (i).DOScaleX (1.3f, transitionDuration).SetUpdate (false);

			deadlyColumns.Add (column.GetChild (i).gameObject);
		}
	}
}

[System.Serializable]
public class ArenaDeadzonesSettings
{
	[Header ("Duration")]
	public float duration;

	[Header ("Delays")]
	public float frontDelay;
	public float backDelay;
	public float rightDelay;
	public float leftDelay;

	[Header ("Indexes")]
	public int[] frontIndex = new int[27];
	public int[] backIndex = new int[27];
	public int[] rightIndex = new int[17];
	public int[] leftIndex = new int[17];
}
