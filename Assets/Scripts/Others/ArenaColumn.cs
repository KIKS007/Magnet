using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Replay;
using DG.Tweening;

public class ArenaColumn : MonoBehaviour
{
    private Renderer rend;
    private float[] emission = new float[] { 3, 3, 3, 3 };
    private float duration = 0.5f;
    private Color color;
    private float scaleModifier = 1.5f;
    private Vector3 scale;
    private ArenaDeadzones arenaDeadzones;

    void Start()
    {
        rend = GetComponent<Renderer>();
        scale = transform.localScale;

        GlobalVariables.Instance.OnStartMode += () => color = GlobalVariables.Instance.arenaColors[(int)GlobalVariables.Instance.environementChroma].gamma;
        GlobalVariables.Instance.OnRestartMode += () => color = GlobalVariables.Instance.arenaColors[(int)GlobalVariables.Instance.environementChroma].gamma;

        arenaDeadzones = FindObjectOfType<ArenaDeadzones>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (ReplayManager.Instance.isReplaying)
            return;

        if (GlobalVariables.Instance.GameState == GameStateEnum.Playing)
        {
            if (other.gameObject.tag != "HoldMovable" && other.gameObject.tag == "Player" && tag == "DeadZone")
            {
                var playerScript = other.gameObject.GetComponent<PlayersGameplay>();

                if (other.gameObject.layer != LayerMask.NameToLayer("Safe") && playerScript.playerState != PlayerState.Dead)
                    playerScript.Death(DeathFX.All, other.transform.position);
            } 

            if (tag != "DeadZone")
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("Movables") && other.attachedRigidbody != null || other.gameObject.layer == LayerMask.NameToLayer("Player") && other.gameObject.tag == "Player" && other.attachedRigidbody != null)
                {
                    if (DOTween.IsTweening("ColumnRend" + GetInstanceID()))
                        DOTween.Kill("ColumnRend" + GetInstanceID());

                    transform.DOScale(scale * scaleModifier, duration * 0.5f);
                    rend.material.DOColor(color * emission[(int)GlobalVariables.Instance.environementChroma], "_EmissionColor", duration * 0.5f).SetId("ColumnRend" + GetInstanceID());
                }
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (ReplayManager.Instance.isReplaying)
            return;

        if (GlobalVariables.Instance.GameState == GameStateEnum.Playing)
        {
            if (other.gameObject.tag != "HoldMovable" && other.gameObject.tag == "Player" && tag == "DeadZone")
            {
                var playerScript = other.gameObject.GetComponent<PlayersGameplay>();

                if (other.gameObject.layer != LayerMask.NameToLayer("Safe") && playerScript.playerState != PlayerState.Dead)
                    playerScript.Death(DeathFX.All, other.transform.position);
            } 

            if (tag != "DeadZone")
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("Movables") && other.attachedRigidbody != null || other.gameObject.layer == LayerMask.NameToLayer("Player") && other.gameObject.tag == "Player" && other.attachedRigidbody != null)
                {
                    if (DOTween.IsTweening("ColumnRend" + GetInstanceID()))
                        DOTween.Kill("ColumnRend" + GetInstanceID());

                    transform.DOScale(scale * scaleModifier, duration * 0.5f);
                    rend.material.DOColor(color * emission[(int)GlobalVariables.Instance.environementChroma], "_EmissionColor", duration * 0.5f).SetId("ColumnRend" + GetInstanceID());
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (ReplayManager.Instance.isReplaying)
            return;
        
        if (tag != "DeadZone")
        if (other.gameObject.layer == LayerMask.NameToLayer("Movables") && other.attachedRigidbody != null || other.gameObject.layer == LayerMask.NameToLayer("Player") && other.gameObject.tag == "Player" && other.attachedRigidbody != null)
        {
            StartCoroutine(Exit());
        }
    }

    IEnumerator Exit()
    {
        yield return new WaitWhile(() => DOTween.IsTweening("ColumnRend" + GetInstanceID()));

        if (tag == "DeadZone")
            yield break;
        
        transform.DOScale(scale, duration);
        rend.material.DOColor(color * arenaDeadzones.normalEmission, "_EmissionColor", duration).SetId("ColumnRend" + GetInstanceID());
    }
}
