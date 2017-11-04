using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;

[Flags]
public enum TutorialState
{
    Movement = 1,
    Dash = 2,
    DashHit = 4,
    Aim = 8,
    AttractRepel = 16,
    Shoot = 32,
    DeadlyWall = 64,
    ReincarnatedCube = 128,

    DashStep = 
		Movement | Dash,
    DashHitStep = 
		Movement | Dash | DashHit,
    AimStep = 
		Movement | Dash | DashHit | Aim,
    AttractRepelStep = 
		Movement | Dash | DashHit | Aim | AttractRepel,
    ShootStep = 
		Movement | Dash | DashHit | Aim | AttractRepel | Shoot,
    DeadlyWallStep = 
		Movement | Dash | DashHit | Aim | AttractRepel | Shoot | DeadlyWall,
    ReincarnatedCubeStep = 
		Movement | Dash | DashHit | Aim | AttractRepel | Shoot | DeadlyWall | ReincarnatedCube
}

public class TutorialManager : MonoBehaviour
{
    public TutorialState tutorialState = TutorialState.DeadlyWall;

    [Header("Infos")]
    public float tweenDuration = 0.5f;
    public float delayDuration = 0.2f;
    public List<TutorialStep> tutorialSteps = new List<TutorialStep>();

    [Header("Intro")]
    public float introWaitDuration;

    [Header("MOVEMENT")]
    public float movingTime = 6;

    [Header("DASH")]
    public int dashCount = 3;
	
    [Header("DASH Hit")]
    public int dashHitCount = 3;

    [Header("ATTRACT / REPEL")]
    public float attractTime = 4;
    public float repelTime = 3;

    [Header("SHOOTS")]
    public int shootsCount = 5;
    public int hitsCount = 4;

    [Header("DEADLY WALLS")]
    public ArenaDeadzones arena;
    public int livesCount = 4;

    [Header("DEAD CUBES")]
    public float timeBeforePlayerRespawn = 2;
    public bool oneDeadCube = false;
    public MovableScript movableExampleScript;

    [Header("Next Previous")]
    public float transitionDuration = 0.5f;
    public Collider nextCollider;
    public Collider previousCollider;

    private List<PlayersTutorial> playersScript = new List<PlayersTutorial>();
    private List<PlayersFXAnimations> playersFX = new List<PlayersFXAnimations>();

    public int tutorialInfosIndex = -1;
    private Transform previousPanel = null;
    private Vector3 colliderInitialScale;

    // Use this for initialization
    void Start()
    {
        colliderInitialScale = nextCollider.transform.GetChild(0).localScale;

        arena = FindObjectOfType<ArenaDeadzones>();

        foreach (var t in tutorialSteps)
        {
            t.panel.localScale = Vector3.zero;
            t.panel.gameObject.SetActive(true);
        }

        StartCoroutine(WaitPlaying());
    }

    IEnumerator ShowInfos()
    {
        previousPanel = tutorialSteps[tutorialInfosIndex].panel;

        tutorialSteps[tutorialInfosIndex].panel.localScale = Vector3.zero;
        tutorialSteps[tutorialInfosIndex].panel.DOScale(1, tweenDuration).SetEase(MenuManager.Instance.easeMenu).SetUpdate(false);

        yield return new WaitForSeconds(tweenDuration);
    }

    IEnumerator WaitPlaying()
    {
        nextCollider.GetComponent<TutorialColliderEventTrigger>().eventEnabled = false;
        previousCollider.GetComponent<TutorialColliderEventTrigger>().eventEnabled = false;

        //nextCollider.enabled = false;
        //previousCollider.enabled = false;

        nextCollider.transform.GetChild(0).DOScale(0, 0).SetEase(Ease.OutQuad);
        previousCollider.transform.GetChild(0).DOScale(0, 0).SetEase(Ease.OutQuad);

        yield return new WaitUntil(() => GlobalVariables.Instance.GameState == GameStateEnum.Playing);

        DOVirtual.DelayedCall(0.01f, () =>
            {
                arena.Reset();
            });

        playersScript.Clear();

        foreach (GameObject g in GlobalVariables.Instance.EnabledPlayersList)
        {
            playersScript.Add(g.GetComponent<PlayersTutorial>());
            playersFX.Add(g.GetComponent<PlayersFXAnimations>());
        }

        foreach (PlayersTutorial p in playersScript)
            p.livesCount = livesCount;

        tutorialInfosIndex = -1;

        Next();

        /*yield return StartCoroutine (ShowInfos ());

		yield return new WaitForSeconds (introWaitDuration);

		StartCoroutine (MovementStep ());*/
    }

    void Waves()
    {
        foreach (PlayersFXAnimations f in playersFX)
            f.WaveFX(true);
    }

    public virtual void PlayerDeath(PlayerName playerName, GameObject player)
    {
        PlayersGameplay playerScript = player.GetComponent<PlayersGameplay>();

        playerScript.livesCount--;

        GlobalVariables.Instance.ListPlayers();

        //Spawn Play if has lives left
        if (playerScript.livesCount != 0)
        {
            GlobalMethods.Instance.SpawnDeathText(playerName, player, playerScript.livesCount);
            GlobalMethods.Instance.SpawnExistingPlayerRandomVoid(player, timeBeforePlayerRespawn, true);
        }
        else
        {
            GlobalMethods.Instance.SpawnPlayerDeadCubeVoid(playerScript.playerName, playerScript.controllerNumber, movableExampleScript);

            if (!oneDeadCube)
            {
                oneDeadCube = true;
            }
        }

    }

    void OnDestroy()
    {
        if (!GlobalVariables.applicationIsQuitting && arena)
            arena.Setup();
    }

    [ButtonGroup]
    public void Previous()
    {
        if (tutorialInfosIndex == 0)
            return;

        tutorialInfosIndex--;

        nextCollider.GetComponent<TutorialColliderEventTrigger>().eventEnabled = false;
        previousCollider.GetComponent<TutorialColliderEventTrigger>().eventEnabled = false;

        DOVirtual.DelayedCall(0.2f, () =>
            {
			
                nextCollider.transform.GetChild(0).DOScale(0, transitionDuration).SetEase(Ease.OutQuad);
                previousCollider.transform.GetChild(0).DOScale(0, transitionDuration).SetEase(Ease.OutQuad);
            });


        if ((int)tutorialSteps[tutorialInfosIndex].state != 0)
            tutorialState = tutorialSteps[tutorialInfosIndex].state;

        switch (tutorialSteps[tutorialInfosIndex].state)
        {
            case TutorialState.AimStep:

                tutorialState = TutorialState.AimStep;

                if (GlobalVariables.Instance.AllMovables.Count > 0 && GlobalVariables.Instance.AllMovables[0].activeSelf)
                    foreach (var m in GlobalVariables.Instance.AllMovables)
                    {
                        float scale = m.transform.localScale.x;

                        m.transform.DOScale(0, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
                            {
                                m.gameObject.SetActive(false);
                                m.transform.localScale = new Vector3(scale, scale, scale);
                            });
                    }

                break;
            case TutorialState.ShootStep:

                tutorialState = TutorialState.ShootStep;

                arena.Reset();

                break;
        }

        StartCoroutine(WaitTutorial());
    }

    [ButtonGroup]
    public void Next()
    {
        if (tutorialInfosIndex == tutorialSteps.Count - 1)
            return;

        tutorialInfosIndex++;

        nextCollider.GetComponent<TutorialColliderEventTrigger>().eventEnabled = false;
        previousCollider.GetComponent<TutorialColliderEventTrigger>().eventEnabled = false;

        DOVirtual.DelayedCall(0.2f, () =>
            {

                nextCollider.transform.GetChild(0).DOScale(0, transitionDuration).SetEase(Ease.OutQuad);
                previousCollider.transform.GetChild(0).DOScale(0, transitionDuration).SetEase(Ease.OutQuad);

            });

        if ((int)tutorialSteps[tutorialInfosIndex].state != 0)
            tutorialState = tutorialSteps[tutorialInfosIndex].state;

        switch (tutorialSteps[tutorialInfosIndex].state)
        {
            case TutorialState.AttractRepelStep:

                if (GlobalVariables.Instance.AllMovables.Count > 0 && !GlobalVariables.Instance.AllMovables[0].activeSelf)
                    GlobalMethods.Instance.RandomPositionMovablesVoid(GlobalVariables.Instance.AllMovables.ToArray());

                break;

            case TutorialState.DeadlyWallStep:

                tutorialState = TutorialState.DeadlyWallStep;

                arena.Setup();

                break;
        }

        StartCoroutine(WaitTutorial());
    }

    IEnumerator WaitTutorial()
    {
        if (previousPanel != null && previousPanel.localScale == Vector3.one)
        {
            previousPanel.DOScale(0, tweenDuration).SetEase(MenuManager.Instance.easeMenu).SetUpdate(false);
            yield return new WaitForSeconds(tweenDuration);
        }

        yield return StartCoroutine(ShowInfos());

        if (tutorialInfosIndex != tutorialSteps.Count - 1)
            nextCollider.transform.GetChild(0).DOScale(colliderInitialScale, transitionDuration).SetEase(Ease.OutQuad);

        if (tutorialInfosIndex > 0)
            previousCollider.transform.GetChild(0).DOScale(colliderInitialScale, transitionDuration).SetEase(Ease.OutQuad);

        yield return new WaitForSeconds(transitionDuration * 0.1f);

        nextCollider.GetComponent<TutorialColliderEventTrigger>().eventEnabled = true;
        previousCollider.GetComponent<TutorialColliderEventTrigger>().eventEnabled = true;

        //nextCollider.enabled = true;
        //previousCollider.enabled = true;
    }

    [System.Serializable]
    public class TutorialStep
    {
        public TutorialState state;
        public Transform panel;
    }
}
