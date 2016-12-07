using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MenuCameraMovement : MonoBehaviour 
{
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
	public float cameraMovementDuration = 0.8f;
	public Ease cameraEaseMovement = Ease.OutQuad;

	[Header ("Logo Movements")]
	public RectTransform menuLogo;
	public float logoMovementDuration = 1f;
	public Vector2 logoNewPos = new Vector2 (0, 365);
	public Vector2 logoHiddenPos = new Vector2 (0, 500);

	private Vector3 positionOnPause = Vector3.zero;

	// Use this for initialization
	void Start () 
	{
		transform.position = startPosition;

		menuLogo.transform.parent.gameObject.SetActive (true);
		startScreenText.transform.parent.gameObject.SetActive (true);

		menuLogo.gameObject.SetActive (true);
		startScreenText.gameObject.SetActive (true);

		GlobalVariables.Instance.OnEndMode += () => positionOnPause = Vector3.zero;
		GlobalVariables.Instance.OnMenu += () => positionOnPause = Vector3.zero;
	}

	public void StartScreen ()
	{
		StartCoroutine (StartScreenCoroutine ());
	}

	IEnumerator StartScreenCoroutine ()
	{
		startScreenText.DOAnchorPosY (textOffScreenY, startScreenDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera").OnComplete (()=> Destroy (startScreenText.gameObject));
		menuLogo.DOAnchorPos (logoNewPos, startScreenDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		menuLogo.DOScale (logoNewScale, startScreenDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		//yield return new WaitForSecondsRealtime (startScreenDuration);

		transform.DOMove (pausePosition, cameraMovementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		yield return null;
	}

	public void MainMenuPosition ()
	{
		transform.DOMove (pausePosition, cameraMovementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		ShowLogo ();
	}

	public void PausePosition ()
	{
		positionOnPause = transform.position;
		transform.DOMove (pausePosition, cameraMovementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		ShowLogo ();
	}

	public void PlayPosition ()
	{
		if(positionOnPause != Vector3.zero)
			transform.DOMove (positionOnPause, cameraMovementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		else
			transform.DOMove (playPosition, cameraMovementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		HideLogo ();
	}

	public void EndModePosition ()
	{
		transform.DOMove (endModePosition, cameraMovementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");
	}

	public void HideLogo ()
	{
		menuLogo.DOAnchorPos (logoHiddenPos, logoMovementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");
	}

	public void ShowLogo ()
	{
		menuLogo.DOAnchorPos (logoNewPos, logoMovementDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");
	}
}
