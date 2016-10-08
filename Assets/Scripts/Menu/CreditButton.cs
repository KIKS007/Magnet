using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
public class CreditButton : MonoBehaviour 
{
	public string url;

	[Header ("Role Text")]
	public float onX;
	public float offX;
	public float roleTweenDuration;

	private RectTransform roleText;

	// Use this for initialization
	void Start () 
	{
		roleText = transform.GetChild (1).GetComponent<RectTransform> ();
		roleText.GetComponent<Text> ().DOFade (0, 0);
		roleText.anchoredPosition = new Vector2 (offX, roleText.anchoredPosition.y);
	}

	void OnEnable ()
	{
		roleText = transform.GetChild (1).GetComponent<RectTransform> ();

		if(roleText.anchoredPosition.x == onX)
		{
			roleText.GetComponent<Text> ().DOFade (0, 0);
			roleText.anchoredPosition = new Vector2 (offX, roleText.anchoredPosition.y);
		}
	}
	
	public void FadeIn ()
	{
		roleText = transform.GetChild (1).GetComponent<RectTransform> ();
		roleText.GetComponent<Text> ().DOFade (1, roleTweenDuration);
		roleText.DOAnchorPos(new Vector2(onX, roleText.anchoredPosition.y), roleTweenDuration);
	}

	public void FadeOut ()
	{
		roleText = transform.GetChild (1).GetComponent<RectTransform> ();
		roleText.GetComponent<Text> ().DOFade (0, roleTweenDuration);
		roleText.DOAnchorPos(new Vector2(offX, roleText.anchoredPosition.y), roleTweenDuration);
	}

	public void GetToURL ()
	{
		if(url != "")
			Application.OpenURL (url);
	}
}
