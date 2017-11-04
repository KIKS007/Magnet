using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MusicFeedback : MonoBehaviour
{
    [Header("Settings")]
    public AudioSpectrum.LevelsType levelsType = AudioSpectrum.LevelsType.Mean;
    public int minFrequency;
    public int maxFrequency;

    [Header("Transform Feedback")]
    public bool position;
    [ShowIf("position")]
    public Vector3 positionFeedback;
    public bool rotation;
    [ShowIf("rotation")]
    public Vector3 rotationFeedback;
    public bool scale;
    [ShowIf("scale")]
    public Vector3 scaleFeedback;

    [Header("Rectransform Feedback")]
    public bool anchoredPosition;
    [ShowIf("anchoredPosition")]
    public Vector3 anchoredPositionFeedback;
    public bool anchoredRotation;
    [ShowIf("anchoredRotation")]
    public Vector3 anchoredRotationFeedback;


    private Vector3 initialPosition;
    private Vector3 initialRotation;
    private Vector3 initialAnchoredPosition;
    private Vector3 initialAnchoredRotation;
    private Vector3 initialScale;
    private float percentage;
    private RectTransform rectTransform;

    // Use this for initialization
    void Start()
    {
        initialRotation = transform.localEulerAngles;
        initialPosition = transform.localPosition;
        initialScale = transform.localScale;

        rectTransform = GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            initialAnchoredPosition = rectTransform.anchoredPosition;
            initialAnchoredRotation = rectTransform.localEulerAngles;
        }

        if (maxFrequency == minFrequency)
            maxFrequency++;
    }
	
    // Update is called once per frame
    void Update()
    {
        GetPercentage();

        if (position)
            PositionFeedback();

        if (rotation)
            RotationFeedback();

        if (scale)
            ScaleFeedback();

        if (anchoredPosition)
            AnchoredPositionFeedback();

        if (anchoredRotation)
            AnchoredRotationFeedback();
    }

    void GetPercentage()
    {
        float p = 0;
        int frequencyCount = 0;

        switch (levelsType)
        {

            case AudioSpectrum.LevelsType.Basic:

                for (int i = minFrequency; i < maxFrequency; i++)
                {
                    p += AudioSpectrum.Instance.LevelsNormalized[i];
                    frequencyCount++;
                }

                percentage = p / frequencyCount;

                break;

            case AudioSpectrum.LevelsType.Peak:

                for (int i = minFrequency; i < maxFrequency; i++)
                {
                    p += AudioSpectrum.Instance.PeakLevelsNormalized[i];
                    frequencyCount++;
                }

                percentage = p / frequencyCount;

                break;

            case AudioSpectrum.LevelsType.Mean:

                for (int i = minFrequency; i < maxFrequency; i++)
                {
                    p += AudioSpectrum.Instance.MeanLevelsNormalized[i];
                    frequencyCount++;
                }

                percentage = p / frequencyCount;

                break;
        }

    }

    void PositionFeedback()
    {
        Vector3 position = transform.position;

        position = new Vector3(initialPosition.x + percentage * positionFeedback.x, 
            initialPosition.y + percentage * positionFeedback.y, 
            initialPosition.z + percentage * positionFeedback.z);

        transform.localPosition = position;
    }

    void RotationFeedback()
    {
        Vector3 rotation = transform.localEulerAngles;

        rotation = new Vector3(initialRotation.x + percentage * rotationFeedback.x, 
            initialRotation.y + percentage * rotationFeedback.y, 
            initialRotation.z + percentage * rotationFeedback.z);

        transform.rotation = Quaternion.Euler(rotation);
    }

    void ScaleFeedback()
    {
        Vector3 scale = transform.localScale;

        scale = new Vector3(initialScale.x + percentage * scaleFeedback.x, 
            initialScale.y + percentage * scaleFeedback.y, 
            initialScale.z + percentage * scaleFeedback.z);

        transform.localScale = scale;
    }

    void AnchoredPositionFeedback()
    {
        Vector3 position = rectTransform.position;

        position = new Vector3(initialAnchoredPosition.x + percentage * anchoredPositionFeedback.x, 
            initialAnchoredPosition.y + percentage * anchoredPositionFeedback.y, 
            initialAnchoredPosition.z + percentage * anchoredPositionFeedback.z);

        rectTransform.anchoredPosition = position;
    }

    void AnchoredRotationFeedback()
    {
        Vector3 rotation = rectTransform.localEulerAngles;

        rotation = new Vector3(initialAnchoredRotation.x + percentage * anchoredRotationFeedback.x, 
            initialAnchoredRotation.y + percentage * anchoredRotationFeedback.y, 
            initialAnchoredRotation.z + percentage * anchoredRotationFeedback.z);

        rectTransform.localRotation = Quaternion.Euler(rotation);
    }
}
