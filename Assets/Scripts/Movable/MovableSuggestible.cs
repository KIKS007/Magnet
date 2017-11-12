using UnityEngine;
using System.Collections;
using DG.Tweening;
using DarkTonic.MasterAudio;

public class MovableSuggestible : MovableScript
{
    public static MovableSuggestible testingMovable = null;

    public override void Start()
    {
        if (testingMovable == null)
        {
            testingMovable = this;

            if (!SteamAchievements.Instance.Achieved(AchievementID.ACH_FLOW))
                StartCoroutine(CheckFlowAchievement());
        }


        gameObject.tag = "Suggestible";
        slowMoTrigger = transform.GetComponentInChildren<SlowMotionTriggerScript>();
        ToDeadlyColor();
    }

    protected override void HitPlayer(Collision other)
    {
        if (other.collider.tag == "Player")
        {
            PlayersGameplay playerScript = other.collider.GetComponent<PlayersGameplay>();

            if (playerScript.playerState == PlayerState.Dead)
                return;

            playerScript.Death(DeathFX.All, other.contacts[0].point);

            PlayerKilled();

            foreach (GameObject g in attracedBy)
                StatsManager.Instance.PlayerKills(g.GetComponent<PlayersGameplay>());

            foreach (GameObject g in repulsedBy)
                StatsManager.Instance.PlayerKills(g.GetComponent<PlayersGameplay>());

            InstantiateParticles(other.contacts[0], GlobalVariables.Instance.HitParticles, GlobalVariables.Instance.playersColors[(int)playerScript.playerName]);

            GlobalMethods.Instance.Explosion(transform.position);
        }
    }

    IEnumerator CheckFlowAchievement()
    {
        while (true)
        {
            if (GlobalVariables.Instance.GameState == GameStateEnum.EndMode)
                yield break;

            if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
                yield return new WaitUntil(() => GlobalVariables.Instance.GameState == GameStateEnum.Playing);

            int right = 0;
            int left = 0;

            foreach (var m in GlobalVariables.Instance.AllMovables)
                if (m.transform.position.x > 0)
                    right++;
                else
                    left++;

            if (right == GlobalVariables.Instance.AllMovables.Count || left == GlobalVariables.Instance.AllMovables.Count)
            {
                SteamAchievements.Instance.UnlockAchievement(AchievementID.ACH_FLOW);
                yield break;
            }

            yield return new WaitForSecondsRealtime(1);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        StopAllCoroutines();

        if (testingMovable && testingMovable == this)
            testingMovable = null;
    }
}
