using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class SpotlightAnimation : MonoBehaviour
{
    public Vector3 animRotation;
    public float animSpeed;
    public Ease animEase = Ease.OutQuad;

    private Vector3 initialRotationEuler;
    private Vector3 inRotation;
    private Vector3 outRotation;

    private Quaternion initialRotation;

    // Use this for initialization
    void Start()
    {
        initialRotationEuler = transform.localEulerAngles;
        inRotation = initialRotationEuler + animRotation;
        outRotation = initialRotationEuler - animRotation;

        initialRotation = transform.localRotation;

        In();
    }

    [ButtonAttribute]
    void In()
    {
        transform.localRotation = initialRotation;

        if (animRotation == Vector3.zero)
            return;

        transform.DOLocalRotate(inRotation, animSpeed).SetSpeedBased().SetEase(animEase).OnComplete(() =>
            {
                transform.DOLocalRotate(initialRotationEuler, animSpeed).SetSpeedBased().SetEase(animEase).OnComplete(Out);
            });

        /*transform.DOLocalRotate(animRotation, animSpeed).SetSpeedBased().SetEase(animEase).SetRelative().OnComplete(() =>
            {
                transform.DOLocalRotateQuaternion(initialRotation, animSpeed).SetSpeedBased().SetEase(animEase).OnComplete(Out);
                //transform.DOLocalRotate(-animRotation, animSpeed).SetSpeedBased().SetEase(animEase).SetRelative().OnComplete(Out);
            });*/
    }

    void Out()
    {
        transform.DOLocalRotate(outRotation, animSpeed).SetSpeedBased().SetEase(animEase).OnComplete(() =>
            {
                transform.DOLocalRotate(initialRotationEuler, animSpeed).SetSpeedBased().SetEase(animEase).OnComplete(In);
            });
        
        /*transform.DOLocalRotate(-animRotation, animSpeed).SetSpeedBased().SetEase(animEase).SetRelative().OnComplete(() =>
            {
                transform.DOLocalRotateQuaternion(initialRotation, animSpeed).SetSpeedBased().SetEase(animEase).OnComplete(Out);
                //transform.DOLocalRotate(animRotation, animSpeed).SetSpeedBased().SetEase(animEase).SetRelative().OnComplete(In);
            });*/
    }
}
