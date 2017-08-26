using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;


public class CreditButton : MonoBehaviour 
{
	public string url;

	[Header ("Role Text")]
	public float onY;
	public float offY;

	private float roleTweenDuration = 0.2f;

	private RectTransform roleText;
	private static List<CreditButton> creditButtons = new List<CreditButton> ();
	[HideInInspector]
	public bool hasClick = false;

	// Use this for initialization
	void Start () 
	{
		if (!creditButtons.Contains (this))
			creditButtons.Add (this);
		
		roleText = transform.GetChild (1).GetComponent<RectTransform> ();
		roleText.GetComponent<Text> ().DOFade (0, 0);
		roleText.anchoredPosition = new Vector2 (roleText.anchoredPosition.x, offY);
	}

	void OnEnable ()
	{
		roleText = transform.GetChild (1).GetComponent<RectTransform> ();

		if(roleText.anchoredPosition.x == onY)
		{
			roleText.GetComponent<Text> ().DOFade (0, 0);
			roleText.anchoredPosition = new Vector2 (roleText.anchoredPosition.x, offY);
		}
	}
	
	public void FadeIn ()
	{
		roleText = transform.GetChild (1).GetComponent<RectTransform> ();
		roleText.GetComponent<Text> ().DOFade (1, roleTweenDuration);
		roleText.DOAnchorPos(new Vector2(roleText.anchoredPosition.x, onY), roleTweenDuration);
	}

	public void FadeOut ()
	{
		roleText = transform.GetChild (1).GetComponent<RectTransform> ();
		roleText.GetComponent<Text> ().DOFade (0, roleTweenDuration);
		roleText.DOAnchorPos(new Vector2(roleText.anchoredPosition.x, offY), roleTweenDuration);
	}

	public void GetToURL ()
	{
		hasClick = true;

		if(url != "")
			Application.OpenURL (url);

		foreach (var c in creditButtons)
			if (!c.hasClick)
				return;

		SteamAchievements.Instance.UnlockAchievement (AchievementID.ACH_ALL_CREDITS);
	}
}
