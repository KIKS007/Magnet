using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HideOnFar : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        GlobalVariables.Instance.menuCameraMovement.OnFarPosition += OnFar;
        GlobalVariables.Instance.menuCameraMovement.OnClosePosition += OnClose;

        if (GlobalVariables.Instance.menuCameraMovement.farPosition)
            OnFar();
        else
            OnClose();
    }

    void OnFar()
    {
        gameObject.SetActive(false);
    }

    void OnClose()
    {
        DOVirtual.DelayedCall(GlobalVariables.Instance.menuCameraMovement.newMovementDuration, () =>
            {
                gameObject.SetActive(true);
            });
    }

    void OnDestroy()
    {
        if (!GlobalVariables.applicationIsQuitting)
        {
            GlobalVariables.Instance.menuCameraMovement.OnFarPosition -= OnFar;
            GlobalVariables.Instance.menuCameraMovement.OnClosePosition -= OnClose;
        }
    }
}
