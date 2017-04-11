using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ArenaDeadzones : MonoBehaviour 
{
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
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	IEnumerator Deadzones (float delay, int[] indexes, Transform[] columns)
	{
		yield return new WaitForSeconds (1);
	}

	void SetDeadly (Transform column)
	{
		column.tag = "DeadZone";
		column.GetComponent<Collider> ().enabled = true;

		column.GetComponent<Renderer> ().material.DOColor (deadlyColor, "_EMISSION", transitionDuration);
		column.GetComponent<Renderer> ().material.DOColor (deadlyColor, transitionDuration);

		column.DOScaleZ (zScale, transitionDuration);
	}
}

[System.Serializable]
public class ArenaDeadzonesSettings
{
	public float frontDelay;
	public float backDelay;
	public float rightDelay;
	public float leftDelay;

	public int[] frontIndex = new int[27];
	public int[] backIndex = new int[27];
	public int[] rightIndex = new int[17];
	public int[] leftIndex = new int[17];
}
