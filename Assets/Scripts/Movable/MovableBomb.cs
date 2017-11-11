using UnityEngine;
using System.Collections;
using DG.Tweening;
using DarkTonic.MasterAudio;
using UnityEngine.PostProcessing;

public class MovableBomb : MovableScript
{
    [Header("BOMB")]
    public GameObject playerHolding;
    public float trackSpeed = 1.2f;
    public float trackSpeedAdded = 0.001f;

    public bool trackingPlayer = false;

    [HideInInspector]
    public bool changingPlayer = false;

    private float speedAddedCooldown = 0.5f;
    private float trackSpeedTemp;

    public override void OnEnable()
    {
        hold = false;
        trackingPlayer = false;
        playerHolding = null;

        rigidbodyMovable = GetComponent<Rigidbody>();
        movableRenderer = GetComponent<Renderer>();
        cubeMeshFilter = transform.GetChild(2).GetComponent<MeshFilter>();
        cubeMaterial = transform.GetChild(1).GetComponent<Renderer>().material;
        deadlyParticle = transform.GetChild(3).GetComponent<ParticleSystem>();
        deadlyParticle2 = transform.GetChild(4).GetComponent<ParticleSystem>();

        slowMoTrigger = transform.GetComponentInChildren<SlowMotionTriggerScript>();

        tag = "Movable";

        deadlyParticle.Stop();
        deadlyParticle2.Stop();

        changingPlayer = false;

        if (playerHolding == null)
        {
            cubeMaterial.SetFloat("_Lerp", 0);
            cubeMaterial.SetColor("_Color", GlobalVariables.Instance.playersColors[4]);
        }

        foreach (GameObject g in GlobalVariables.Instance.EnabledPlayersList)
            g.GetComponent<PlayersGameplay>().RemoveCubeAttractionRepulsion(this);

        attracedBy.Clear();
        repulsedBy.Clear();
    }

    protected override void HitPlayer(Collision other)
    {
        PlayersGameplay playerScript = null;

        if (other.gameObject.tag == "Player")
            playerScript = other.gameObject.GetComponent<PlayersGameplay>();

        if (tag == "Movable" && other.gameObject.tag == "Player"
            || tag == "ThrownMovable" && other.gameObject.tag == "Player" && !trackingPlayer)
        {
            if (playerThatThrew == null || playerThatThrew != other.gameObject)
            {
                if (!trackingPlayer && playerThatThrew != null)
                    StatsManager.Instance.PlayersHits(playerThatThrew, other.gameObject);

                BombManager manager = (BombManager)GlobalVariables.Instance.lastManManager;

                if (manager.timer < 1 && !trackingPlayer && playerThatThrew != null)
                    SteamAchievements.Instance.UnlockAchievement(AchievementID.ACH_BOMB);

                playerScript.OnHoldMovable(gameObject, true);
                playerHolding = other.gameObject;

                mainCamera.GetComponent<ScreenShakeCamera>().CameraShaking(FeedbackType.Stun);
                mainCamera.GetComponent<ZoomCamera>().Zoom(FeedbackType.Stun);

                InstantiateParticles(other.contacts[0], GlobalVariables.Instance.HitParticles, GlobalVariables.Instance.playersColors[(int)playerScript.playerName]);
            }
        }

        if (tag == "DeadCube" && other.gameObject.tag == "Player" && trackingPlayer && playerScript.playerState != PlayerState.Dead)
        {
            hold = true;
            playerHolding = other.gameObject;

            playerScript.OnDeath -= PlayerSuicide;
            playerScript.Death(DeathFX.All, other.contacts[0].point);

            mainCamera.GetComponent<ScreenShakeCamera>().CameraShaking(FeedbackType.Stun);
            mainCamera.GetComponent<ZoomCamera>().Zoom(FeedbackType.Stun);

            InstantiateParticles(other.contacts[0], GlobalVariables.Instance.HitParticles, GlobalVariables.Instance.playersColors[(int)playerScript.playerName]);
        }
    }

    public override void OnHold()
    {
        base.OnHold();

        if (playerHolding != null)
            playerHolding.GetComponent<PlayersGameplay>().OnDeath -= PlayerSuicide;

        playerHolding = player.gameObject;

        playerHolding.GetComponent<PlayersGameplay>().OnDeath += PlayerSuicide;
    }

    public override void OnRelease()
    {
        OnReleaseEventVoid();
    }

    public virtual void ResetColor()
    {
        //Debug.Log ("Neutral Color");
		
        if (deadlyParticle == null)
            deadlyParticle = transform.GetChild(3).GetComponent<ParticleSystem>();

        if (cubeMaterial == null)
            cubeMaterial = transform.GetChild(1).GetComponent<Renderer>().material;

        deadlyParticle.Stop();
		
        DisableAllColor(toColorDuration);
		
        cubeColor = CubeColor.Neutral;
    }

    public IEnumerator Explode()
    {
        ToDeadlyColor();

        if (!hold)
        {
            trackingPlayer = true;
            tag = "ThrownMovable";

            yield return StartCoroutine(GetToPlayerPosition());
        }

        mainCamera.GetComponent<SlowMotionCamera>().StartSlowMotion();

        GlobalMethods.Instance.Explosion(transform.position);

        MasterAudio.StopAllOfSound(SoundsManager.Instance.lastSecondsSound);
        MasterAudio.StopAllOfSound(SoundsManager.Instance.cubeTrackingSound);

        Vector3 explosionPos = Vector3.Lerp(playerHolding.transform.position, transform.position, 0.5f);

        playerHolding.GetComponent<PlayersGameplay>().OnDeath -= PlayerSuicide;

        playerHolding.GetComponent<PlayersGameplay>().Death(DeathFX.All, explosionPos);

        gameObject.SetActive(false);

        ToNeutralColor();

        playerHolding = null;
        trackingPlayer = false;
        hold = false;
    }

    IEnumerator GetToPlayerPosition()
    {
        transform.DORotate(Vector3.zero, 0.5f).SetUpdate(false);
        transform.DOLocalMoveY(1.5f, 0.5f).SetUpdate(false);

        rigidbodyMovable.velocity = Vector3.zero;
        rigidbodyMovable.angularVelocity = Vector3.zero;

        trackSpeedTemp = trackSpeed;

        tag = "DeadCube";

        foreach (GameObject g in GlobalVariables.Instance.EnabledPlayersList)
            g.GetComponent<PlayersGameplay>().RemoveCubeAttractionRepulsion(this);

        StartCoroutine(AddSpeed());

        while (playerHolding && Vector3.Distance(playerHolding.transform.position, transform.position) > 0.5f)
        {
            if (!hold)
            {
                Vector3 direction = (playerHolding.transform.position - transform.position);
                direction.Normalize();

                //float distance = Vector3.Distance (playerHolding.transform.position, transform.position) + distanceFactor;
                //rigidbodyMovable.MovePosition (transform.position + direction * distance * getToPlayerForce * Time.deltaTime);

                rigidbodyMovable.AddForce(direction * trackSpeedTemp, ForceMode.Impulse);

                if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
                    yield return new WaitWhile(() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

                yield return new WaitForFixedUpdate();
            }
            else
            {
                trackingPlayer = false;
                break;
            }
        }

        StopCoroutine(AddSpeed());
    }

    IEnumerator AddSpeed()
    {
        yield return new WaitForSeconds(speedAddedCooldown);

        if (GlobalVariables.Instance.GameState != GameStateEnum.Playing)
            yield return new WaitWhile(() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

        trackSpeedTemp += trackSpeedAdded;

        if (!hold)
            StartCoroutine(AddSpeed());
    }

    void PlayerSuicide()
    {
        tag = "Untagged";

        if (playerHolding)
            playerHolding.GetComponent<PlayersGameplay>().OnDeath -= PlayerSuicide;

        playerHolding = null;
        trackingPlayer = false;
        hold = false;

        Vector3 scale = transform.localScale;

        transform.DOScale(0, 0.5f).OnComplete(() =>
            {
                gameObject.SetActive(false);
                transform.localScale = scale;
                FindObjectOfType<BombManager>().timer = -1f;
            });
    }

}
