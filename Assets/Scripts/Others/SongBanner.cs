using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DarkTonic.MasterAudio;
using DG.Tweening;

public class SongBanner : MonoBehaviour
{
	[Header("Banner")]
	public Ease bannerEase = Ease.Linear;
	public float bannerSpeed;
	public float bannerPauseDuration;

	private RectTransform maskBannerRect;
	private RectTransform songRect;
	private Text songText;

	public float centerPosition;
	public float leftPosition;
	public float rightPosition;

	private PlaylistController playlistController;

	// Use this for initialization
	void Awake()
	{
		playlistController = FindObjectOfType<PlaylistController>();

		maskBannerRect = GetComponent<RectTransform>();
		songRect = transform.GetChild(0).GetComponent<RectTransform>();
		songText = transform.GetChild(0).GetComponent<Text>();

		playlistController.SongChanged += SongChangedVoid;
	}

	void OnEnable()
	{
		playlistController = FindObjectOfType<PlaylistController>();
		songText = transform.GetChild(0).GetComponent<Text>();

		if (playlistController.CurrentPlaylistClip)
			songText.text = playlistController.CurrentPlaylistClip.name;

		StartCoroutine(WaitForPlaylist());
	}

	void OnDestroy()
	{
		playlistController.SongChanged -= SongChangedVoid;
	}

	void SongChangedVoid(string name)
	{
		if (!gameObject.activeSelf || !gameObject.activeInHierarchy)
			return;
		
		StopAllCoroutines();
		StartCoroutine(SongChanged(name));
	}

	IEnumerator WaitForPlaylist()
	{
		yield return new WaitUntil(() => playlistController.CurrentPlaylistClip != null);

		StartCoroutine(SongChanged(playlistController.CurrentPlaylistClip.name));
	}

	IEnumerator SongChanged(string newSongName)
	{
		DOTween.Kill("SongBanner");

		Tween tween = songRect.DOAnchorPosX(leftPosition, 10000f).SetSpeedBased().SetUpdate(true);
		yield return tween.WaitForCompletion();

		songText.text = newSongName;

		yield return new WaitForEndOfFrame();

		centerPosition = 0;
		leftPosition = -songRect.sizeDelta.x;
		rightPosition = maskBannerRect.sizeDelta.x;

		Show();
	}

	void Show()
	{
		songRect.anchoredPosition = new Vector2(rightPosition, songRect.anchoredPosition.y);
		
		songRect.DOAnchorPosX(centerPosition, bannerSpeed).SetSpeedBased().SetEase(bannerEase).OnComplete(() =>
			{
				DOVirtual.DelayedCall(bannerPauseDuration, () => Hide()).SetId("SongBanner").SetUpdate(true);
			}).SetId("SongBanner").SetUpdate(true);
	}

	void Hide()
	{
		songRect.DOAnchorPosX(leftPosition, bannerSpeed).SetSpeedBased().OnComplete(Show).SetId("SongBanner").SetUpdate(true);
	}
}
