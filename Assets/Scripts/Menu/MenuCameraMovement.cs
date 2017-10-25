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
    public System.Action OnFarPosition;
    public System.Action OnClosePosition;

    public Ease cameraEaseMovement = Ease.OutQuad;
    public bool tweening = false;

    [Header("Bobbing")]
    public Vector3 bobbingLookTarget;

    [Header("Start")]
    public List<Vector3> newStartPositions = new List<Vector3>();
    public List<Vector3> newStartRotations = new List<Vector3>();

    [Header("Movements")]
    public Vector3 newMenuPosition;
    public Vector3 newPlayPosition;
    public float newMovementDuration = 0.8f;

    [Header("Rotations")]
    public Vector3 newMenuRotation;
    public Vector3 newPlayRotation = new Vector3(90.0f, 0.0f, 0.0f);

    private Vector3 positionOnPause = Vector3.zero;
    private SlowMotionCamera slowMo;
    private BrownianMotion browianMotion;
    private float browianInitialFrequency;

    public bool farPosition = true;

    // Use this for initialization
    void Awake()
    {
        slowMo = GetComponent<SlowMotionCamera>();
        browianMotion = GetComponent<BrownianMotion>();
        browianInitialFrequency = browianMotion.positionFrequency;

        GlobalVariables.Instance.OnEndMode += () => positionOnPause = Vector3.zero;
        GlobalVariables.Instance.OnMenu += () => positionOnPause = Vector3.zero;
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "Scene Testing")
        {
            transform.position = newStartPositions[(int)GlobalVariables.Instance.environementChroma];
            transform.rotation = Quaternion.Euler(newStartRotations[(int)GlobalVariables.Instance.environementChroma]);
            EnableBrowianMotion(false);
        }
        else
        {
            transform.position = newPlayPosition;
            transform.rotation = Quaternion.Euler(newPlayRotation);
            browianMotion.enabled = false;
        }
    }

    void Update()
    {
        tweening = DOTween.IsTweening("MenuCamera");
    }

    public void ToggleFarPosition()
    {
        if (GlobalVariables.Instance.demoEnabled)
            return;
        
        if (farPosition)
            StartCoroutine(StartPosition());
        else
            StartCoroutine(StartFarPosition());

        MasterAudio.PlaySound(SoundsManager.Instance.gameStartSound);
    }

    public IEnumerator StartFarPosition()
    {
        if (GlobalVariables.Instance.demoEnabled)
            yield break;
        
        if (OnFarPosition != null)
            OnFarPosition();

        MenuManager.Instance.ShowLogo();

        StopPreviousMovement();

        farPosition = true;

        transform.DOMove(newStartPositions[(int)GlobalVariables.Instance.environementChroma], newMovementDuration * 0.9f).SetEase(cameraEaseMovement).SetId("MenuCamera");
        transform.DORotate(newStartRotations[(int)GlobalVariables.Instance.environementChroma], newMovementDuration, RotateMode.Fast).SetEase(cameraEaseMovement).SetId("MenuCamera");

        yield return new WaitForSecondsRealtime(newMovementDuration);

        EnableBrowianMotion(false);
    }

    public IEnumerator StartPosition()
    {
        if (GlobalVariables.Instance.demoEnabled)
            yield break;
        
        if (OnClosePosition != null)
            OnClosePosition();
        
        StartCoroutine(MenuManager.Instance.HideLogo(MenuManager.Instance.startScreen));
	
        StopPreviousMovement();

        farPosition = false;

        if (GlobalVariables.Instance.GameState == GameStateEnum.Playing || GlobalVariables.Instance.GameState == GameStateEnum.Paused)
            positionOnPause = transform.position;

        transform.DOMove(newMenuPosition, newMovementDuration * 0.9f).SetEase(cameraEaseMovement).SetId("MenuCamera");
        transform.DORotate(newMenuRotation, newMovementDuration, RotateMode.Fast).SetEase(cameraEaseMovement).SetId("MenuCamera");

        yield return new WaitForSecondsRealtime(newMovementDuration);

        EnableBrowianMotion();
    }

    public IEnumerator NewMenuPosition()
    {
        if (GlobalVariables.Instance.demoEnabled)
            yield break;

        StopPreviousMovement();

        DOVirtual.DelayedCall(newMovementDuration * 0.5f, () => slowMo.StopEffects());

        if (GlobalVariables.Instance.GameState == GameStateEnum.Playing || GlobalVariables.Instance.GameState == GameStateEnum.Paused)
            positionOnPause = transform.position;

        transform.rotation = Quaternion.Euler(newPlayRotation);

        transform.DOMove(newMenuPosition, newMovementDuration * 0.9f).SetEase(cameraEaseMovement).SetId("MenuCamera");
        transform.DORotate(newMenuRotation, newMovementDuration, RotateMode.FastBeyond360).SetEase(cameraEaseMovement).SetId("MenuCamera");

        yield return new WaitForSecondsRealtime(newMovementDuration);

        MasterAudio.PlaySound(SoundsManager.Instance.winSound);

        EnableBrowianMotion();
    }

    public IEnumerator NewPlayPosition()
    {
        if (GlobalVariables.Instance.demoEnabled)
            yield break;
        
        DisableBrowianMotion();
        StopPreviousMovement();

        Vector3 position = positionOnPause != Vector3.zero ? positionOnPause : newPlayPosition;

        transform.DOMove(position, newMovementDuration * 0.9f).SetEase(cameraEaseMovement).SetId("MenuCamera");
        transform.DORotate(newPlayRotation, newMovementDuration, RotateMode.Fast).SetEase(cameraEaseMovement).SetId("MenuCamera");

        yield return new WaitForSecondsRealtime(newMovementDuration);
    }

    public IEnumerator NewRestartRotation()
    {
        if (GlobalVariables.Instance.demoEnabled)
            yield break;
        
        StopPreviousMovement();

        MasterAudio.PlaySound(SoundsManager.Instance.winSound);

        transform.DOMove(newPlayPosition, newMovementDuration * 0.5f).SetEase(cameraEaseMovement).SetId("MenuCamera");
        transform.DORotate(new Vector3(-360f, 0f, 0f), newMovementDuration, RotateMode.LocalAxisAdd).SetEase(cameraEaseMovement).SetId("MenuCamera");

        yield return new WaitForSecondsRealtime(newMovementDuration);

        StartCoroutine(GlobalVariables.Instance.screenShakeCamera.ResetCameraRotationCoroutine());

        //transform.DORotate(newPlayRotation, 0.5f, RotateMode.Fast).SetEase(cameraEaseMovement).SetId("MenuCamera");
    }

    void StopPreviousMovement()
    {
        DisableBrowianMotion();

        if (DOTween.IsTweening("ScreenShake"))
            DOTween.Kill("ScreenShake");
		
        if (DOTween.IsTweening("MenuCamera"))
            DOTween.Kill("MenuCamera");
    }

    IEnumerator LookAtTarget()
    {
        while (browianMotion.enablePositionNoise)
        {
            if (GlobalVariables.Instance.demoEnabled)
                yield break;
            
            transform.LookAt(bobbingLookTarget);
            yield return new WaitForEndOfFrame();
        }
    }

    void EnableBrowianMotion(bool lookatMenu = true)
    {
        if (GlobalVariables.Instance.demoEnabled)
            return;
        
        browianMotion._initialPosition = transform.position;

        if (!browianMotion.enabled)
            browianMotion.enabled = true;

        browianMotion.enablePositionNoise = true;

        if (lookatMenu)
            StartCoroutine(LookAtTarget());

        DOTween.To(() => browianMotion.positionFrequency, x => browianMotion.positionFrequency = x, browianInitialFrequency, newMovementDuration);
    }

    void DisableBrowianMotion()
    {
        if (GlobalVariables.Instance.demoEnabled)
            return;
        
        browianMotion.enablePositionNoise = false;
        DOTween.To(() => browianMotion.positionFrequency, x => browianMotion.positionFrequency = x, 0, newMovementDuration);
    }

    [ButtonGroupAttribute("a", -1)]
    void MenuPosition()
    {
        StartCoroutine(NewMenuPosition());
    }

    [ButtonGroupAttribute("a", -1)]
    void PlayPosition()
    {
        StartCoroutine(NewPlayPosition());
    }


    [ButtonGroupAttribute("b", -1)]
    public void EditorMenuPosition()
    {
        transform.position = newMenuPosition;
        transform.rotation = Quaternion.Euler(newMenuRotation);
    }

    [ButtonGroupAttribute("b", -1)]
    public void EditorPlayPosition()
    {
        transform.position = newPlayPosition;
        transform.rotation = Quaternion.Euler(newPlayRotation);
    }

    [ButtonGroupAttribute("c", -1)]
    public void EditorFarPosition()
    {
        if (transform.position == newStartPositions[0])
        {
            transform.position = newStartPositions[1];
            transform.rotation = Quaternion.Euler(newStartRotations[1]);
        }
        else if (transform.position == newStartPositions[1])
        {
            transform.position = newStartPositions[2];
            transform.rotation = Quaternion.Euler(newStartRotations[2]);
        }
        else if (transform.position == newStartPositions[2])
        {
            transform.position = newStartPositions[3];
            transform.rotation = Quaternion.Euler(newStartRotations[3]);
        }
        else
        {
            transform.position = newStartPositions[0];
            transform.rotation = Quaternion.Euler(newStartRotations[0]);
        }
    }
}
