using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Klak.Motion;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using DarkTonic.MasterAudio;

public class MenuCameraMovement : MonoBehaviour 
{
	public Ease cameraEaseMovement = Ease.OutQuad;
	public bool tweening = false;

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
	public List<Vector3> newStartPositions = new List<Vector3> ();
	public List<Vector3> newStartRotations = new List<Vector3> ();

	[Header ("Movements")]
	public Vector3 newMenuPosition;
	public Vector3 newPlayPosition;
	public float newMovementDuration = 0.8f;

	[Header ("Rotations")]
	public Vector3 newMenuRotation;
	public Vector3 newPlayRotation = new Vector3 (90.0f, 0.0f, 0.0f);

	private Vector3 positionOnPause = Vector3.zero;
	private SlowMotionCamera slowMo;
	private BrownianMotion browianMotion;
	private float browianInitialFrequency;

	[HideInInspector]
	public bool farPosition = true;

	// Use this for initialization
	void Awake () 
	{
		slowMo = GetComponent<SlowMotionCamera> ();
		browianMotion = GetComponent<BrownianMotion> ();
		browianInitialFrequency = browianMotion.positionFrequency;

		if(SceneManager.GetActiveScene ().name != "Scene Testing")
		{
			transform.position = newStartPositions [(int)GlobalVariables.Instance.environementChroma];
			transform.rotation = Quaternion.Euler (newStartRotations [(int)GlobalVariables.Instance.environementChroma]);
			EnableBrowianMotion (false);
		}
		else
		{
			transform.position = newPlayPosition;
			transform.rotation = Quaternion.Euler (newPlayRotation);
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

	void Update ()
	{
		tweening = DOTween.IsTweening ("MenuCamera");
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

	public void ToggleFarPosition ()
	{
		if (farPosition)
			StartCoroutine (StartPosition ());
		else
			StartCoroutine (StartFarPosition ());

		MasterAudio.PlaySound (SoundsManager.Instance.gameStartSound);
	}

	public IEnumerator StartFarPosition ()
	{
		StopPreviousMovement ();

		farPosition = true;

		transform.DOMove (newStartPositions [(int)GlobalVariables.Instance.environementChroma], newMovementDuration * 0.9f).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		transform.DORotate (newStartRotations [(int)GlobalVariables.Instance.environementChroma], newMovementDuration, RotateMode.Fast).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		yield return new WaitForSecondsRealtime (newMovementDuration);

		EnableBrowianMotion (false);
	}

	public IEnumerator StartPosition ()
	{
		StopPreviousMovement ();

		farPosition = false;

		if(menuLogo != null)
		{
			startScreenText.DOAnchorPosY (textOffScreenY, startScreenDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera").OnComplete (()=> Destroy (startScreenText.gameObject));
			menuLogo.DOSizeDelta (menuLogo.sizeDelta * logoNewScale, startScreenDuration).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		}

		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing || GlobalVariables.Instance.GameState == GameStateEnum.Paused)
			positionOnPause = transform.position;

		transform.DOMove (newMenuPosition, newMovementDuration * 0.9f).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		transform.DORotate (newMenuRotation, newMovementDuration, RotateMode.Fast).SetEase (cameraEaseMovement).SetId ("MenuCamera");

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

		transform.rotation = Quaternion.Euler (newPlayRotation);

		transform.DOMove (newMenuPosition, newMovementDuration * 0.9f).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		transform.DORotate (newMenuRotation, newMovementDuration, RotateMode.FastBeyond360).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		yield return new WaitForSecondsRealtime (newMovementDuration);

		MasterAudio.PlaySound(SoundsManager.Instance.winSound);

		EnableBrowianMotion ();
	}

	public IEnumerator NewPlayPosition ()
	{
		DisableBrowianMotion ();
		StopPreviousMovement ();

		StartCoroutine (HideLogo ());

		Vector3 position = positionOnPause != Vector3.zero ? positionOnPause : newPlayPosition;

		transform.DOMove (position, newMovementDuration * 0.9f).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		transform.DORotate (newPlayRotation, newMovementDuration, RotateMode.Fast).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		yield return new WaitForSecondsRealtime (newMovementDuration);
	}

	public IEnumerator NewRestartRotation ()
	{
		StopPreviousMovement ();

		MasterAudio.PlaySound(SoundsManager.Instance.winSound);

		transform.DOMove (newPlayPosition, newMovementDuration * 0.5f).SetEase (cameraEaseMovement).SetId ("MenuCamera");
		transform.DORotate (new Vector3(-360f, 0f, 0f), newMovementDuration, RotateMode.LocalAxisAdd).SetEase (cameraEaseMovement).SetId ("MenuCamera");

		yield return new WaitForSecondsRealtime (newMovementDuration);

		transform.DORotate (newPlayRotation, 0.5f, RotateMode.Fast).SetEase (cameraEaseMovement).SetId ("MenuCamera");
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

	void EnableBrowianMotion (bool lookatMenu = true)
	{
		browianMotion._initialPosition = transform.position;
		//browianMotion.enabled = true;
		browianMotion.enablePositionNoise = true;

		if(lookatMenu)
			StartCoroutine (LookAtTarget ());

		DOTween.To (()=> browianMotion.positionFrequency, x=> browianMotion.positionFrequency =x, browianInitialFrequency, newMovementDuration);
	}

	void DisableBrowianMotion ()
	{
		browianMotion.enablePositionNoise = false;
		DOTween.To (()=> browianMotion.positionFrequency, x=> browianMotion.positionFrequency =x, 0, newMovementDuration);
	}

	[ButtonGroupAttribute ("a", -1)]
	public void MenuPosition ()
	{
		StartCoroutine (NewMenuPosition ());
	}

	[ButtonGroupAttribute ("a", -1)]
	public void PlayPosition ()
	{
		StartCoroutine (NewPlayPosition ());
	}


	[ButtonGroupAttribute ("b", -1)]
	public void EditorMenuPosition ()
	{
		transform.position = newMenuPosition;
		transform.rotation = Quaternion.Euler (newMenuRotation);
	}

	[ButtonGroupAttribute ("b", -1)]
	public void EditorPlayPosition ()
	{
		transform.position = newPlayPosition;
		transform.rotation = Quaternion.Euler (newPlayRotation);
	}

	[ButtonGroupAttribute ("c", -1)]
	public void EditorFarPosition ()
	{
		if(transform.position == newStartPositions [0])
		{
			transform.position = newStartPositions [1];
			transform.rotation = Quaternion.Euler (newStartRotations [1]);
		}
		else if(transform.position == newStartPositions [1])
		{
			transform.position = newStartPositions [2];
			transform.rotation = Quaternion.Euler (newStartRotations [2]);
		}
		else if(transform.position == newStartPositions [2])
		{
			transform.position = newStartPositions [3];
			transform.rotation = Quaternion.Euler (newStartRotations [3]);
		}
		else
		{
			transform.position = newStartPositions [0];
			transform.rotation = Quaternion.Euler (newStartRotations [0]);
		}
	}
}
