using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class ArenaDeadzones : MonoBehaviour 
{
	[Header ("Settings")]
	public Ease ease = Ease.OutQuad;
	public int currentSettings = 0;
	public List<ArenaDeadzonesSettings> deadzonesSettings = new List<ArenaDeadzonesSettings> ();

	[Header ("Deadly State")]
	public Color deadlyColor;
	public float zScale = 3f;
	public float transitionDuration;

	[Header ("Columns")]
	public Transform[] frontColumns = new Transform[27];
	public Transform[] backColumns = new Transform[27];
	public Transform[] rightColumns = new Transform[17];
	public Transform[] leftColumns = new Transform[17];

	// Use this for initialization
	void Start () 
	{
		StartCoroutine (Deadzones (deadzonesSettings [currentSettings].frontDelay, deadzonesSettings [currentSettings].duration, deadzonesSettings [currentSettings].frontIndex, frontColumns));
		StartCoroutine (Deadzones (deadzonesSettings [currentSettings].backDelay, deadzonesSettings [currentSettings].duration, deadzonesSettings [currentSettings].backIndex, backColumns));
		StartCoroutine (Deadzones (deadzonesSettings [currentSettings].rightDelay, deadzonesSettings [currentSettings].duration, deadzonesSettings [currentSettings].rightIndex, rightColumns));
		StartCoroutine (Deadzones (deadzonesSettings [currentSettings].leftDelay, deadzonesSettings [currentSettings].duration, deadzonesSettings [currentSettings].leftIndex, leftColumns));
	}

	IEnumerator Deadzones (float delay, float duration, int[] indexes, Transform[] columns)
	{
		yield return new WaitForSeconds (delay);

		float waitTime = duration / columns.Length;

		for(int i = 0; i < columns.Length; i++)
		{
			yield return new WaitUntil (() => GlobalVariables.Instance.GameState == GameStateEnum.Playing);

			yield return new WaitForSeconds (waitTime);

			for(int j = 0; j  < indexes.Length; j++)
			{
				if (indexes [j] == i)
					SetDeadly (columns [j]);
			}
		}
	}

	void SetDeadly (Transform column)
	{
		Transform columnChild = column.GetChild (0);

		columnChild.tag = "DeadZone";
		columnChild.GetComponent<Collider> ().enabled = true;

		columnChild.GetComponent<Renderer> ().material.DOColor (deadlyColor, "_EmissionColor", transitionDuration);
		columnChild.GetComponent<Renderer> ().material.DOColor (deadlyColor, transitionDuration);

		columnChild.DOScaleZ (zScale, transitionDuration);
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
