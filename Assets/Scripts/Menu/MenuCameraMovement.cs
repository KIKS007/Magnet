using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MenuCameraMovement : MonoBehaviour 
{
	public Ease cameraEaseMovement = Ease.OutQuad;

	[Header ("Start Screen")]
	public RectTransform startScreenText;
	public float startScreenDuration = 0.5f;
	public float textOffScreenY = -700;
	public float logoNewScale = 0.4f;

	[Header ("Camera Movements")]
	public Vector3 startPosition = new Vector3 (-150, 104, 18);
	public Vector3 pausePosition = new Vector3 (-48, 104, 18);
	public Vector3 playPosition = new Vector3 (0, 60, 0);
	public Vector3 endModePosition = new Vector3 (48, 104, 18);
	public float movementDuration = 0.8f;

	[Header ("Camera Loading Movements")]
	public Vector3 loadingPosition;
	public Vector3 restartPosition;
	public float loadingMovementDuration = 0.25f;

	[Header ("Logo Movements")]
	public RectTransform menuLogo;
	public float logoMovementDuration = 1f;
	public Vector2 logoNewPos = new Vector2 (0, 365);
	public Vector2 logoHiddenPos = new Vector2 (0, 500);

	private Vector3 positionOnPause = Vector3.zero;
	private Vector3 positionOnLoad = Vector3.zero;

	// Use this for initialization
	void Awake () 
	{
		transform.position = startPosition;

		menuLogo.transform.parent.gameObject.SetActive (true);
		startScreenText.transform.parent.gameObject.SetActive (true);

		menuLogo.gameObject.SetActive (true);
		startScreenText.gameObject.SetActive (true);

		GlobalVariables.Instance.OnEndMode += () => positionOnPause = Vector3.zero;
		GlobalVariables.Instance.OnMenu += () => positionOnPause = Vector3.zero;
	}

	public IEnumerator StartScreen ()
	{
		StopPreviousMovement ();

		startScreenText.DOAnchorPosY (textOffScreenY, startScreenDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera").OnComplete (()=> Destroy (startScreenText.gameObject));
		menuLogo.DOAnchorPos (logoNewPos, startScreenDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		menuLogo.DOScale (logoNewScale, startScreenDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		transform.DOMove (ModeRelativePosition(pausePosition), movementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		yield return new WaitForSecondsRealtime (startScreenDuration);
	}

	public IEnumerator MainMenuPosition ()
	{
		StopPreviousMovement ();

		transform.DOMove (ModeRelativePosition(pausePosition), movementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		StartCoroutine (ShowLogo ());

		yield return new WaitForSecondsRealtime (movementDuration);
	}

	public IEnumerator PausePosition ()
	{
		StopPreviousMovement ();

		positionOnPause = transform.position;
		transform.DOMove (ModeRelativePosition(pausePosition), movementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		StartCoroutine (ShowLogo ());

		yield return new WaitForSecondsRealtime (movementDuration);
	}

	public IEnumerator PlayPosition ()
	{
		StopPreviousMovement ();

		if(GlobalVariables.Instance.GameState == GameStateEnum.Paused)
			transform.DOMove (positionOnPause, movementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		else
			transform.DOMove (ModeRelativePosition(playPosition), movementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		StartCoroutine (HideLogo ());

		yield return new WaitForSecondsRealtime (movementDuration);
	}

	public IEnumerator EndModePosition ()
	{
		StopPreviousMovement ();

		transform.DOMove (ModeRelativePosition(endModePosition), movementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		yield return new WaitForSecondsRealtime (movementDuration);
	}

	public IEnumerator LoadingPosition ()
	{
		StopPreviousMovement ();

		if(transform.position != ModeRelativePosition(loadingPosition))
		{
			positionOnLoad = transform.position;
			transform.DOMove (ModeRelativePosition(loadingPosition), loadingMovementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		}

		else
			transform.DOMove (positionOnLoad, loadingMovementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		yield return new WaitForSecondsRealtime (loadingMovementDuration);
	}

	public IEnumerator RestartPosition ()
	{
		StopPreviousMovement ();

		if(transform.position != ModeRelativePosition(restartPosition))
			transform.DOMove (ModeRelativePosition(restartPosition), loadingMovementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		else
			transform.DOMove (ModeRelativePosition(new Vector3 (GlobalVariables.Instance.currentModePosition.x, ModeRelativePosition(restartPosition).y, ModeRelativePosition(restartPosition).z)), loadingMovementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		
		yield return new WaitForSecondsRealtime (loadingMovementDuration);
	}

	public IEnumerator HideLogo ()
	{
		menuLogo.DOAnchorPos (logoHiddenPos, logoMovementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		yield return new WaitForSecondsRealtime (logoMovementDuration);
	}

	public IEnumerator ShowLogo ()
	{
		menuLogo.DOAnchorPos (logoNewPos, logoMovementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		yield return new WaitForSecondsRealtime (logoMovementDuration);
	}

	void StopPreviousMovement ()
	{
		if (DOTween.IsTweening ("MenuCamera"))
			DOTween.Kill ("MenuCamera");
	}

	Vector3 ModeRelativePosition (Vector3 position)
	{
		return position + GlobalVariables.Instance.currentModePosition;
	}
}
