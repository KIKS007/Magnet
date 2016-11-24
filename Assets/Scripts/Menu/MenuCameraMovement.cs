using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MenuCameraMovement : MonoBehaviour 
{
	[Header ("Start Screen")]
	public RectTransform menuLogo;
	public RectTransform startScreenText;
	public Vector2 logoNewPos = new Vector2 (0, 365);
	public Vector2 logoHiddenPos = new Vector2 (0, 500);
	public float logoNewScale = 0.4f;
	public float startScreenDuration = 0.5f;
	public float textOffScreenY = -700;

	[Header ("Camera Movements")]
	public Vector3 startPosition = new Vector3 (-150, 104, 18);
	public Vector3 pausePosition = new Vector3 (-48, 104, 18);
	public Vector3 playPosition = new Vector3 (0, 60, 0);
	public Vector3 modeEndPosition = new Vector3 (48, 104, 18);
	public float cameraMovementDuration = 0.8f;
	public Ease cameraEaseMovement = Ease.OutQuad;

	private Vector3 positionOnPause = Vector3.zero;

	// Use this for initialization
	void Start () 
	{
		transform.position = startPosition;

		menuLogo.transform.parent.gameObject.SetActive (true);
		startScreenText.transform.parent.gameObject.SetActive (true);

		menuLogo.gameObject.SetActive (true);
		startScreenText.gameObject.SetActive (true);

		GlobalVariables.Instance.OnGameOver += () => positionOnPause = Vector3.zero;
	}

	public void StartScreen ()
	{
		StartCoroutine (StartScreenCoroutine ());
	}

	IEnumerator StartScreenCoroutine ()
	{
		startScreenText.DOAnchorPosY (textOffScreenY, startScreenDuration).SetEase (cameraEaseMovement);
		menuLogo.DOAnchorPos (logoNewPos, startScreenDuration).SetEase (cameraEaseMovement);
		menuLogo.DOScale (logoNewScale, startScreenDuration).SetEase (cameraEaseMovement);

		yield return new WaitForSecondsRealtime (startScreenDuration);

		transform.DOMove (startPosition, cameraMovementDuration).SetEase (cameraEaseMovement);
	}

	public void PausePosition ()
	{
		positionOnPause = transform.position;
		transform.DOMove (pausePosition, cameraMovementDuration).SetEase (cameraEaseMovement);
	}

	public void PlayPosition ()
	{
		if(positionOnPause != Vector3.zero)
			transform.DOMove (positionOnPause, cameraMovementDuration).SetEase (cameraEaseMovement);
		else
			transform.DOMove (playPosition, cameraMovementDuration).SetEase (cameraEaseMovement);
	}

	public void ModeEndPosition ()
	{
		transform.DOMove (modeEndPosition, cameraMovementDuration).SetEase (cameraEaseMovement);
	}

	public void HideLogo ()
	{
		menuLogo.DOAnchorPos (logoHiddenPos, MenuManager.Instance.durationToHide).SetEase (cameraEaseMovement);
	}

	public void ShowLogo ()
	{
		menuLogo.DOAnchorPos (logoNewPos, MenuManager.Instance.durationToShow).SetEase (cameraEaseMovement);
	}
}
