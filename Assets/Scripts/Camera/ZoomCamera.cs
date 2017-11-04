using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class ZoomCamera : MonoBehaviour
{
    public bool zoomEnabled = true;

    public List<ZoomSettings> zoomList = new List<ZoomSettings>();

    [Header("Ease")]
    public Ease zoomEase = Ease.OutQuad;

    [Header("Test")]
    public FeedbackType whichZoomTest;
    public bool test = false;

    private Camera cam;
    private float initialFOV;

    // Use this for initialization
    void Awake()
    {
        cam = GetComponent<Camera>();
        initialFOV = cam.fieldOfView;

        GlobalVariables.Instance.OnStartupDone += () => Zoom(FeedbackType.Startup);
        GlobalVariables.Instance.OnEndMode += () => Zoom(FeedbackType.ModeEnd);
    }
	
    // Update is called once per frame
    void Update()
    {
        if (test)
        {
            test = false;
            Zoom(whichZoomTest);
        }
    }

    public void Zoom(FeedbackType whichZoom)
    {
        if (!zoomEnabled)
            return;
        
        StopAllCoroutines();
        StartCoroutine(ZoomCoroutine(whichZoom));
    }

    IEnumerator ZoomCoroutine(FeedbackType whichZoom)
    {
        if (DOTween.IsTweening("ZoomCamera"))
            DOTween.Kill("ZoomCamera");

        float newFOV = 60;
        float zoomDuration = 0;
        float resetDuration = 0;
        float delay = 0;

        bool validZoom = false;

        for (int i = 0; i < zoomList.Count; i++)
        {
            if (zoomList[i].whichZoom == whichZoom)
            {
                newFOV = zoomList[i].newFOV;
                zoomDuration = zoomList[i].zoomDuration;
                resetDuration = zoomList[i].resetDuration;
                delay = zoomList[i].delay;

                validZoom = true;
                break;
            }
        }

        if (!validZoom)
            yield break;

        if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
        {
            if (DOTween.IsTweening("ZoomCamera"))
                DOTween.Kill("ZoomCamera");
			
            yield break;
        }

        Tween tween = cam.DOFieldOfView(newFOV, zoomDuration).SetEase(zoomEase).SetId("ZoomCamera");

        yield return tween.WaitForCompletion();

        yield return new WaitForSecondsRealtime(delay);

        tween = cam.DOFieldOfView(initialFOV, resetDuration).SetEase(zoomEase).SetId("ZoomCamera");
    }

}

[Serializable]
public class ZoomSettings
{
    public FeedbackType whichZoom = FeedbackType.Default;

    public float newFOV;
    public float zoomDuration;
    public float resetDuration;
    public float delay = 0;
}
