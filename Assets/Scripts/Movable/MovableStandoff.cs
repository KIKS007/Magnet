using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovableStandoff : MovableScript
{
    public Vector3 shooterPosition;

    protected override void LowVelocity()
    {
        if (hold == false && currentVelocity > 0)
        {
            if (currentVelocity < limitVelocity)
            {
                if (gameObject.tag == "DeadCube")
                {
                    if (slowMoTrigger == null)
                        slowMoTrigger = transform.GetComponentInChildren<SlowMotionTriggerScript>();

                    slowMoTrigger.triggerEnabled = false;

                    gameObject.tag = "Movable";

                    ToNeutralColor();
                }
            }
        }
    }

    protected override void HitPlayer(Collision other)
    {
        if (other.collider.tag == "Player")
        {
            PlayersGameplay playerScript = other.collider.GetComponent<PlayersGameplay>();

            if (playerScript.playerState == PlayerState.Stunned)
                return;

            if (tag == "ThrownMovable")
            {
                if (playerThatThrew == null || other.gameObject.name != playerThatThrew.name)
                {
                    playerScript.StunVoid(true);

                    InstantiateParticles(other.contacts[0], GlobalVariables.Instance.HitParticles, GlobalVariables.Instance.playersColors[(int)playerScript.playerName]);	

                    if (playerThatThrew != null)
                        StatsManager.Instance.PlayersHits(playerThatThrew, other.gameObject);

                }				
            }
        }

        if (other.collider.tag == "Player")
        {
            PlayersGameplay playerScript = other.collider.GetComponent<PlayersGameplay>();

            if (playerScript.playerState == PlayerState.Dead)
                return;

            if (tag == "DeadCube")
            {
                playerScript.Death(DeathFX.All, other.contacts[0].point, playerThatThrew);

                PlayerKilled();

                if (playerThatThrew != null)
                {
                    StatsManager.Instance.PlayersHits(playerThatThrew, other.gameObject);

                    if (!SteamAchievements.Instance.Achieved(AchievementID.ACH_STANDOFF) && Vector3.Distance(other.transform.position, shooterPosition) > 30f)
                        SteamAchievements.Instance.UnlockAchievement(AchievementID.ACH_STANDOFF);
                }

                GlobalMethods.Instance.Explosion(transform.position);
            }
        }
    }

    public override void OnHold()
    {
        hold = true;

        attracedBy.Clear();
        repulsedBy.Clear();

        StartCoroutine(DeadlyTransition());

        OnHoldEventVoid();
    }

    public override void OnRelease()
    {
        if (playerThatThrew != null)
            shooterPosition = playerThatThrew.transform.position;

        OnReleaseEventVoid();

        StartCoroutine(DeadlyTransition());
    }

    IEnumerator DeadlyTransition()
    {
        ToDeadlyColor();

        yield return new WaitForSeconds(0.01f);

        tag = "DeadCube";
    }
}
