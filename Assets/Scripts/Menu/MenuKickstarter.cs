using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;

public class MenuKickstarter : MonoBehaviour 
{
	[Header ("Bounds")]
	public Vector2 xBounds = new Vector2 ();
	public Vector2 yBounds = new Vector2 ();

	[Header ("Spawn")]
	public float spawnDuration = 0.2f;
	public float spawnDelay = 0.01f;
	public Ease spawnEase;

	[Header ("Backers")]
	public Transform backersParent;
	public List<RectTransform> allBackers = new List<RectTransform> ();

	private List<RectTransform> backersSpawned = new List<RectTransform> ();

	// Use this for initialization
	void Awake () 
	{
		var backersText = backersParent.GetComponentsInChildren<Text> ();

		foreach (var b in backersText)
			allBackers.Add (b.GetComponent<RectTransform> ());
	}

	void OnEnable ()
	{
		StartCoroutine (SpawnBackers ());
	}

	IEnumerator SpawnBackers ()
	{
		yield return new WaitUntil (() => allBackers.Count == 0);

		foreach (var b in allBackers)
			b.gameObject.SetActive (false);

		backersSpawned.Clear ();

		foreach(var b in allBackers)
		{
			bool validPosition = true;

			do
			{
				validPosition = true;
				b.anchoredPosition = new Vector2 (Random.Range (xBounds.x, xBounds.y), Random.Range (yBounds.x, yBounds.y));

				foreach(var s in backersSpawned)
				{
					if(b.rect.Overlaps (s.rect))
					{
						validPosition = false;
						break;
					}
				}
			}
			while (!validPosition);

			backersSpawned.Add (b);
			Spawn (b);

			yield return new WaitForSecondsRealtime (spawnDelay);
		}

		yield return 0;
	}

	void Spawn (RectTransform backer)
	{
		Vector3 scale = backer.localScale;
		backer.localScale = Vector3.zero;
		backer.gameObject.SetActive (true);

		backer.DOScale (scale, spawnDuration).SetEase (spawnEase);
	}
}
