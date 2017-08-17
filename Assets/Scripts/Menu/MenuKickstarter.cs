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
	public Vector2 offPosition;
	public float widthBoundsFactor = 1.1f;
	public float heightBoundsFactor = 1.1f;

	[Header ("Spawn")]
	public float spawnDuration = 0.2f;
	public float spawnDelay = 0.01f;
	public Ease spawnEase;
	public int spawnTries = 20;
	public int spawnFails = 0;

	[Header ("Backers")]
	public Transform backersParent;
	public List<RectTransform> allBackers = new List<RectTransform> ();

	public RectTransform rec1;
	public RectTransform rec2;
	public bool isOverlap;

	private List<RectTransform> backersSpawned = new List<RectTransform> ();

	// Use this for initialization
	void Awake () 
	{
		var backersText = backersParent.GetComponentsInChildren<Text> ();

		foreach (var b in backersText)
			allBackers.Add (b.GetComponent<RectTransform> ());
	}

	void Update ()
	{
		if(rec1 && rec2)
		{
			Rect rect1 = new Rect (rec1.anchoredPosition.x, rec1.anchoredPosition.y, rec1.rect.width * widthBoundsFactor, rec1.rect.height * heightBoundsFactor);
			Rect rect2 = new Rect (rec2.anchoredPosition.x, rec2.anchoredPosition.y, rec2.rect.width * widthBoundsFactor, rec2.rect.height * heightBoundsFactor);
			
			isOverlap = rect1.Overlaps (rect2);
		}
	}

	void OnEnable ()
	{
		StartCoroutine (SpawnBackers ());
	}

	IEnumerator SpawnBackers ()
	{
		yield return new WaitUntil (() => allBackers.Count != 0);

		foreach (var b in allBackers)
			b.anchoredPosition = offPosition;

		backersSpawned.Clear ();
		spawnFails = 0;

		yield return new WaitUntil (() => !MenuManager.Instance.isTweening);

		foreach(var b in allBackers)
		{
			bool validPosition = true;
			Rect rect1 = new Rect ();
			Rect rect2 = new Rect ();

			int tries = 0;

			do
			{
				if(MenuManager.Instance.isTweening)
					yield break;

				tries++;

				if(tries >= spawnTries)
				{
					spawnFails++;
					break;
				}

				validPosition = true;

				b.anchoredPosition = new Vector2 (Random.Range (xBounds.x, xBounds.y), Random.Range (yBounds.x, yBounds.y));
				rect1 = new Rect (b.anchoredPosition.x, b.anchoredPosition.y, b.rect.width * widthBoundsFactor, b.rect.height * heightBoundsFactor);

				foreach(var s in backersSpawned)
				{
					rect2 = new Rect (s.anchoredPosition.x, s.anchoredPosition.y, s.rect.width * widthBoundsFactor, s.rect.height * heightBoundsFactor);

					if(rect1.Overlaps (rect2))
					{
						validPosition = false;
						break;
					}
				}
			}
			while (!validPosition);

			if (validPosition) {
				backersSpawned.Add (b);
				Spawn (b);
			} else
				b.gameObject.SetActive (false);

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
