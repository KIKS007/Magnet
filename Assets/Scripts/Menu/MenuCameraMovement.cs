using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Klak.Motion;

public class MenuCameraMovement : MonoBehaviour 
{
	public Ease cameraEaseMovement = Ease.OutQuad;

	[Header ("Start Screen")]
	public RectTransform startScreenText;
	public float startScreenDuration = 0.5f;
	public float textOffScreenY = -700;
	public float logoNewScale = 0.4f;

	[Header ("Bobbing")]
	public Vector3 bobbingLookTarget;

	[Header ("Logo Movements")]
	public RectTransform menuLogo;
	public float logoMovementDuration = 1f;
	public Vector2 logoNewPos = new Vector2 (0, 365);
	public Vector2 logoHiddenPos = new Vector2 (0, 500);

	[Header ("Start")]
	public Vector3 startRotation;
	public Vector3 newStartPosition;

	[Header ("Movements")]
	public Vector3 newMenuPosition;
	public Vector3 newPlayPosition;
	public float newMovementDuration = 0.8f;

	private Vector3 positionOnPause = Vector3.zero;
	private SlowMotionCamera slowMo;
	private BrownianMotion browianMotion;
	private float browianInitialFrequency;

	// Use this for initialization
	void Awake () 
	{
		slowMo = GetComponent<SlowMotionCamera> ();
		browianMotion = GetComponent<BrownianMotion> ();
		browianInitialFrequency = browianMotion.positionFrequency;

		if(SceneManager.GetActiveScene ().name != "Scene Testing")
		{
			transform.position = newStartPosition;
			transform.rotation = Quaternion.Euler (startRotation);
			EnableBrowianMotion ();
		}
		else
		{
			transform.position = newPlayPosition;
			transform.rotation = Quaternion.Euler (new Vector3 (90, 0, 0));
			browianMotion.enabled = false;
		}

		//StartCoroutine (NewMenuPosition ());

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

	public IEnumerator StartPosition ()
	{
		StopPreviousMovement ();

		if(menuLogo != null)
		{
			startScreenText.DOAnchorPosY (textOffScreenY, startScreenDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera").OnComplete (()=> Destroy (startScreenText.gameObject));
			menuLogo.DOSizeDelta (menuLogo.sizeDelta * logoNewScale, startScreenDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		}

		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing || GlobalVariables.Instance.GameState == GameStateEnum.Paused)
			positionOnPause = transform.position;

		transform.DOMove (newMenuPosition, newMovementDuration * 0.9f).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		transform.DORotate (Vector3.zero, newMovementDuration, RotateMode.Fast).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		yield return new WaitForSecondsRealtime (newMovementDuration);

		EnableBrowianMotion ();
	}

	public IEnumerator NewMenuPosition ()
	{
		StopPreviousMovement ();

		DOVirtual.DelayedCall (newMovementDuration * 0.5f, ()=> StopSlowMotion ());

		StartCoroutine (ShowLogo ());

		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing || GlobalVariables.Instance.GameState == GameStateEnum.Paused)
			positionOnPause = transform.position;

		transform.rotation = Quaternion.Euler (new Vector3 (90f, 0f, 0f));

		transform.DOMove (newMenuPosition, newMovementDuration * 0.9f).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		transform.DORotate (Vector3.zero, newMovementDuration, RotateMode.FastBeyond360).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		yield return new WaitForSecondsRealtime (newMovementDuration);

		EnableBrowianMotion ();
	}

	public IEnumerator NewPlayPosition ()
	{
		DisableBrowianMotion ();
		StopPreviousMovement ();

		StartCoroutine (HideLogo ());

		Vector3 position = positionOnPause != Vector3.zero ? positionOnPause : newPlayPosition;

		transform.DOMove (position, newMovementDuration * 0.9f).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		transform.DORotate (new Vector3 (90f, 0f, 0f), newMovementDuration, RotateMode.Fast).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		yield return new WaitForSecondsRealtime (newMovementDuration);
	}

	public IEnumerator NewRestartRotation ()
	{
		StopPreviousMovement ();
		
		transform.DOMove (newPlayPosition, newMovementDuration * 0.5f).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		transform.DORotate (new Vector3(-360f, 0f, 0f), newMovementDuration, RotateMode.LocalAxisAdd).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		yield return new WaitForSecondsRealtime (newMovementDuration);

		transform.DORotate (new Vector3 (90f, 0f, 0f), 0.5f, RotateMode.Fast).SetEase (cameraEaseMovement).SetId ("MenuCamera");
	}

	void StopSlowMotion ()
	{
		slowMo.StopEffects ();
	}

	void StopPreviousMovement ()
	{
		DisableBrowianMotion ();

		if (DOTween.IsTweening ("ScreenShake"))
			DOTween.Kill ("ScreenShake");
		
		if (DOTween.IsTweening ("MenuCamera"))
			DOTween.Kill ("MenuCamera");
	}

	IEnumerator LookAtTarget ()
	{
		while(browianMotion.enablePositionNoise)
		{
			transform.LookAt (bobbingLookTarget);
			yield return new WaitForEndOfFrame ();
		}
	}

	void EnableBrowianMotion ()
	{
		browianMotion._initialPosition = transform.position;
		//browianMotion.enabled = true;
		browianMotion.enablePositionNoise = true;
		StartCoroutine (LookAtTarget ());
		DOTween.To (()=> browianMotion.positionFrequency, x=> browianMotion.positionFrequency =x, browianInitialFrequency, newMovementDuration);
	}

	void DisableBrowianMotion ()
	{
		browianMotion.enablePositionNoise = false;
		DOTween.To (()=> browianMotion.positionFrequency, x=> browianMotion.positionFrequency =x, 0, newMovementDuration);
	}

	Vector3 ModeRelativePosition (Vector3 position)
	{
		return position + GlobalVariables.Instance.currentModePosition;
	}

	[ContextMenu ("Menu Position")]
	public void EditorMenuPosition ()
	{
		transform.position = newMenuPosition;
		transform.rotation = Quaternion.Euler (Vector3.zero);
	}

	[ContextMenu ("Play Position")]
	public void EditorPlayPosition ()
	{
		transform.position = newPlayPosition;
		transform.rotation = Quaternion.Euler (new Vector3 (90, 0, 0));
	}
}
