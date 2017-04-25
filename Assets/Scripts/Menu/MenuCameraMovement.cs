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

	[Header ("Camera Loading Relative Movements")]
	public Vector3 loadingPosition;
	public Vector3 restartPosition;
	public float loadingMovementDuration = 0.25f;

	[Header ("Logo Movements")]
	public RectTransform menuLogo;
	public float logoMovementDuration = 1f;
	public Vector2 logoNewPos = new Vector2 (0, 365);
	public Vector2 logoHiddenPos = new Vector2 (0, 500);

	[Header ("New Movements")]
	public Vector3 newMenuPosition;
	public Vector3 newPlayPosition;
	public float newMovementDuration = 0.8f;

	private Vector3 positionOnPause = Vector3.zero;

	private bool loading = false;
	private bool restarting = false;

	// Use this for initialization
	void Awake () 
	{
		//transform.position = startPosition;

		if(menuLogo != null)
		{
			menuLogo.transform.parent.gameObject.SetActive (true);
			startScreenText.transform.parent.gameObject.SetActive (true);

			menuLogo.gameObject.SetActive (true);
			startScreenText.gameObject.SetActive (true);
		}
		else
			Debug.LogWarning ("No Menu Logo");


		GlobalVariables.Instance.OnEndMode += () => positionOnPause = Vector3.zero;
		GlobalVariables.Instance.OnMenu += () => positionOnPause = Vector3.zero;
	}

	public IEnumerator StartScreen ()
	{
		StopPreviousMovement ();

		if(menuLogo != null)
		{
			startScreenText.DOAnchorPosY (textOffScreenY, startScreenDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera").OnComplete (()=> Destroy (startScreenText.gameObject));
			menuLogo.DOSizeDelta (menuLogo.sizeDelta * logoNewScale, startScreenDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		}

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

		if(!loading)
		{
			loading = true;
			transform.DOMove (loadingPosition, loadingMovementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera").SetRelative ();
		}

		else
		{
			loading = false;
			transform.DOMove (-loadingPosition, loadingMovementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera").SetRelative ();
		}

		yield return new WaitForSecondsRealtime (loadingMovementDuration);
	}

	public IEnumerator RestartPosition ()
	{
		StopPreviousMovement ();

		if(!restarting)
		{
			restarting = true;
			transform.DOMove (restartPosition, loadingMovementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera").SetRelative ();
		}

		else
		{
			restarting = false;
			transform.DOMove (ModeRelativePosition(playPosition), loadingMovementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		}
		
		yield return new WaitForSecondsRealtime (loadingMovementDuration);
	}

	public IEnumerator HideLogo ()
	{
		if(menuLogo != null)
			menuLogo.DOAnchorPos (logoHiddenPos, logoMovementDuration).SetEase (cameraEaseMovement).SetId ("Logo");
		
		yield return new WaitForSecondsRealtime (logoMovementDuration);
	}

	public IEnumerator ShowLogo ()
	{
		if(menuLogo != null)
			menuLogo.DOAnchorPos (logoNewPos, logoMovementDuration).SetEase (cameraEaseMovement).SetId ("Logo");
		
		yield return new WaitForSecondsRealtime (logoMovementDuration);
	}

	public IEnumerator NewMenuPosition ()
	{
		StopPreviousMovement ();

		if(startScreenText != null)
		{
			startScreenText.DOAnchorPosY (textOffScreenY, startScreenDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera").OnComplete (()=> Destroy (startScreenText.gameObject));
			menuLogo.DOSizeDelta (menuLogo.sizeDelta * logoNewScale, startScreenDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		}

		Debug.Log ("Menu");

		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing || GlobalVariables.Instance.GameState == GameStateEnum.Paused)
			positionOnPause = transform.position;

		transform.DOMove (newMenuPosition, newMovementDuration * 0.9f).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		transform.DORotate (Vector3.zero, newMovementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		yield return new WaitForSecondsRealtime (newMovementDuration);
	}

	public IEnumerator NewPlayPosition ()
	{
		StopPreviousMovement ();

		Debug.Log ("Play");

		Vector3 position = positionOnPause != Vector3.zero ? positionOnPause : newPlayPosition;

		transform.DOMove (position, newMovementDuration * 0.9f).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		transform.DORotate (new Vector3 (90, 0, 0), newMovementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		yield return new WaitForSecondsRealtime (newMovementDuration);
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
