﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Replay;
using DG.Tweening;

public class ArenaColumn : MonoBehaviour
{
    private Renderer rend;
    private float emission = 3f;
    private float duration = 0.5f;
    private Color color;
    private float scaleModifier = 1.5f;
    private Vector3 scale;

    void Start()
    {
        rend = GetComponent<Renderer>();
        scale = transform.localScale;

        GlobalVariables.Instance.OnStartMode += () => color = rend.material.GetColor("_EmissionColor");
        GlobalVariables.Instance.OnRestartMode += () => color = rend.material.GetColor("_EmissionColor");
    }

    void OnTriggerEnter(Collider other)
    {
        if (ReplayManager.Instance.isReplaying)
            return;

        if (GlobalVariables.Instance.GameState == GameStateEnum.Playing)
        {
            if (other.gameObject.tag != "HoldMovable" && other.gameObject.tag == "Player" && tag == "DeadZones")
            {
                var playerScript = other.gameObject.GetComponent<PlayersGameplay>();

                if (other.gameObject.layer != LayerMask.NameToLayer("Safe") && playerScript.playerState != PlayerState.Dead)
                    playerScript.Death(DeathFX.All, other.transform.position);
            } 

            if (tag != "DeadZones")
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("Movables") || other.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    if (DOTween.IsTweening("ColumnRend" + GetInstanceID()))
                        DOTween.Kill("ColumnRend" + GetInstanceID());

                    transform.DOScale(scale * scaleModifier, duration * 0.5f);
                    rend.material.DOColor(color * emission, "_EmissionColor", duration * 0.5f).SetId("ColumnRend" + GetInstanceID());
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (tag != "DeadZones")
        if (other.gameObject.layer == LayerMask.NameToLayer("Movables") || other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            StartCoroutine(Exit());
        }
    }

    IEnumerator Exit()
    {
        yield return new WaitWhile(() => DOTween.IsTweening("ColumnRend" + GetInstanceID()));

        if (tag == "DeadZones")
            yield break;
        
        transform.DOScale(scale, duration);
        rend.material.DOColor(color, "_EmissionColor", duration).SetId("ColumnRend" + GetInstanceID());
    }
}
