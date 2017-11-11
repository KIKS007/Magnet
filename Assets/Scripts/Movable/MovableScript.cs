#pragma warning disable 0618

using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using Replay;
using System.Reflection;

public enum CubeColor
{
    Neutral,
    Blue,
    Pink,
    Green,
    Yellow,
    Deadly}
;

public enum SpeedState
{
    Acceleration,
    Deceleration,
    Same,
    None}
;

public class MovableScript : MonoBehaviour
{
    public event EventHandler OnHoldEvent;
    public event EventHandler OnReleaseEvent;

    public System.Action OnDeadly;
    public System.Action OnNeutral;

    #region Variables

    [Header("Color")]
    public CubeColor cubeColor;

    [Header("Speed")]
    public float higherVelocity;
    public float currentVelocity;
    public float limitVelocity = 80f;
    public SpeedState speedState = SpeedState.Same;

    [Header("Cube States")]
    public bool hold;
    public List<GameObject> attracedBy = new List<GameObject>();
    public List<GameObject> repulsedBy = new List<GameObject>();

    [Header("Players")]
    public GameObject playerThatThrew;

    protected bool canPlaySound = true;

    protected const float toColorDuration = 0.25f;
    protected const float toNeutralDuration = 0.25f;
    protected const float toDeadlyDuration = 0.25f;

    protected float massRb;
    protected float drag;
    protected CollisionDetectionMode collisionDetectionModeRb;

    protected Renderer movableRenderer;

    protected GameObject mainCamera;

    [HideInInspector]
    public ParticleSystem deadlyParticle;
    [HideInInspector]
    public ParticleSystem deadlyParticle2;
    [HideInInspector]
    public Rigidbody rigidbodyMovable;
    [HideInInspector]
    public SlowMotionTriggerScript slowMoTrigger;
    [HideInInspector]
    public Transform player;
    [HideInInspector]
    public MeshFilter cubeMeshFilter;
    [HideInInspector]
    public Material cubeMaterial;
    [HideInInspector]
    public Vector3 initialScale;

    #endregion

    #region Setup

    public virtual void Awake()
    {
        initialScale = transform.localScale;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    public virtual void Start()
    {
        //ToNeutralColor ();
    }

    public virtual void OnEnable()
    {
        hold = false;

        rigidbodyMovable = GetComponent<Rigidbody>();
        movableRenderer = GetComponent<Renderer>();
        cubeMeshFilter = transform.GetChild(2).GetComponent<MeshFilter>();
        cubeMaterial = transform.GetChild(1).GetComponent<Renderer>().material;
        deadlyParticle = transform.GetChild(3).GetComponent<ParticleSystem>();
        deadlyParticle2 = transform.GetChild(4).GetComponent<ParticleSystem>();

        slowMoTrigger = transform.GetComponentInChildren<SlowMotionTriggerScript>();

        deadlyParticle.Stop();
        deadlyParticle2.Stop();
        cubeMeshFilter.mesh = GlobalVariables.Instance.cubesStripes[Random.Range(0, GlobalVariables.Instance.cubesStripes.Length)];
        attracedBy.Clear();
        repulsedBy.Clear();
    }

    #endregion

    #region Update / FixedUpdate

    protected virtual void Update()
    {
        if (ReplayManager.Instance.isReplaying)
            return;

        SetSpeedState();

        CurrentVelocity();

        CheckPlayerThatThrew();

        LowVelocity();

        HighVelocity();
    }

    protected virtual void SetSpeedState()
    {
        if (!hold && rigidbodyMovable != null)
        {
            if (rigidbodyMovable.velocity.magnitude < 1)
                speedState = SpeedState.None;
            else if (currentVelocity == rigidbodyMovable.velocity.magnitude)
                speedState = SpeedState.Same;
            else if (currentVelocity > rigidbodyMovable.velocity.magnitude)
                speedState = SpeedState.Deceleration;
            else if (currentVelocity < rigidbodyMovable.velocity.magnitude)
                speedState = SpeedState.Acceleration;
        }
    }

    protected virtual void CurrentVelocity()
    {
        if (hold == false && rigidbodyMovable != null)
            currentVelocity = rigidbodyMovable.velocity.magnitude;

        if (currentVelocity > higherVelocity)
            higherVelocity = currentVelocity;
    }

    protected virtual void CheckPlayerThatThrew()
    {
        if (currentVelocity < limitVelocity && playerThatThrew != null && gameObject.tag != "ThrownMovable")
        {
            if (speedState != SpeedState.Acceleration)
                playerThatThrew = null;
        }
    }

    protected virtual void HighVelocity()
    {
        if (hold == false && currentVelocity > 0)
        {
            if (currentVelocity > limitVelocity)
            {
                if (slowMoTrigger == null)
                    slowMoTrigger = transform.GetComponentInChildren<SlowMotionTriggerScript>();

                if (tag == "ThrownMovable" || tag == "DeadCube")
                    slowMoTrigger.SetEnabled(true);
            }
        }
    }

    protected virtual void LowVelocity()
    {
        if (hold == false && currentVelocity > 0)
        {
            if (currentVelocity < limitVelocity && gameObject.tag == "ThrownMovable")
            {
                if (slowMoTrigger == null)
                    slowMoTrigger = transform.GetComponentInChildren<SlowMotionTriggerScript>();
				
                slowMoTrigger.SetEnabled(false);
				
                gameObject.tag = "Movable";
            }
        }
    }

    #endregion

    #region Color

    public virtual void ToColor(GameObject otherPlayer = null, float overrideDuration = toColorDuration)
    {
        int whichPlayer = otherPlayer != null ? (int)otherPlayer.GetComponent<PlayersGameplay>().playerName : (int)player.GetComponent<PlayersGameplay>().playerName;

        CubeColor whichColor = (CubeColor)whichPlayer + 1;

        if (DOTween.IsTweening("CubeColorTween" + gameObject.GetInstanceID()))
            DOTween.Kill("CubeColorTween" + gameObject.GetInstanceID());
		
        DisableAllColor(overrideDuration);

        if (deadlyParticle == null)
            deadlyParticle = transform.GetChild(3).GetComponent<ParticleSystem>();
		
        if (deadlyParticle2 == null)
            deadlyParticle2 = transform.GetChild(4).GetComponent<ParticleSystem>();

        deadlyParticle.Stop();
        deadlyParticle2.Stop();

        switch (whichColor)
        {
            case CubeColor.Blue:
                cubeMaterial.DOFloat(1f, "_LerpBLUE", overrideDuration).SetId("CubeColorTween" + gameObject.GetInstanceID()).SetUpdate(false);
                break;
            case CubeColor.Pink:
                cubeMaterial.DOFloat(1f, "_LerpPINK", overrideDuration).SetId("CubeColorTween" + gameObject.GetInstanceID()).SetUpdate(false);
                break;
            case CubeColor.Green:
                cubeMaterial.DOFloat(1f, "_LerpGREEN", overrideDuration).SetId("CubeColorTween" + gameObject.GetInstanceID()).SetUpdate(false);
                break;
            case CubeColor.Yellow:
                cubeMaterial.DOFloat(1f, "_LerpYELLOW", overrideDuration).SetId("CubeColorTween" + gameObject.GetInstanceID()).SetUpdate(false);
                break;
        }

        StartCoroutine(WaitToChangeColorEnum(whichColor, overrideDuration));
    }

    public virtual void ToNeutralColor(float overrideDuration = toNeutralDuration)
    {
        if (!hold)
        {
            if (deadlyParticle == null)
                deadlyParticle = transform.GetChild(3).GetComponent<ParticleSystem>();

            if (deadlyParticle2 == null)
                deadlyParticle2 = transform.GetChild(4).GetComponent<ParticleSystem>();

            deadlyParticle.Stop();
            deadlyParticle2.Stop();

            //cubeColor = CubeColor.Neutral;

            if (OnNeutral != null)
                OnNeutral();

            DisableAllColor(overrideDuration);

            StartCoroutine(WaitToChangeColorEnum(CubeColor.Neutral, overrideDuration));
        }
    }

    public virtual void ToDeadlyColor(float overrideDuration = toColorDuration)
    {
        DisableAllColor(overrideDuration);

        if (deadlyParticle == null)
            deadlyParticle = transform.GetChild(3).GetComponent<ParticleSystem>();

        if (deadlyParticle2 == null)
            deadlyParticle2 = transform.GetChild(4).GetComponent<ParticleSystem>();

        deadlyParticle.Play();
        deadlyParticle2.Play();

        if (OnDeadly != null)
            OnDeadly();

        cubeMaterial.DOFloat(1f, "_LerpRED", overrideDuration).SetId("CubeColorTween" + gameObject.GetInstanceID()).SetUpdate(false);

        cubeColor = CubeColor.Deadly;
    }

    public virtual void DisableAllColor(float duration)
    {
        if (DOTween.IsTweening("CubeColorTween" + gameObject.GetInstanceID()))
            DOTween.Kill("CubeColorTween" + gameObject.GetInstanceID());
		
        cubeMaterial.DOFloat(0f, "_LerpBLUE", duration).SetId("CubeColorTween" + gameObject.GetInstanceID()).SetUpdate(false);
		
        cubeMaterial.DOFloat(0f, "_LerpPINK", duration).SetId("CubeColorTween" + gameObject.GetInstanceID()).SetUpdate(false);
		
        cubeMaterial.DOFloat(0f, "_LerpGREEN", duration).SetId("CubeColorTween" + gameObject.GetInstanceID()).SetUpdate(false);
		
        cubeMaterial.DOFloat(0f, "_LerpYELLOW", duration).SetId("CubeColorTween" + gameObject.GetInstanceID()).SetUpdate(false);
		
        cubeMaterial.DOFloat(0f, "_LerpRED", duration).SetId("CubeColorTween" + gameObject.GetInstanceID()).SetUpdate(false);
    }

    protected virtual IEnumerator WaitToChangeColorEnum(CubeColor whichColor, float waitTime)
    {
        yield return new WaitForSeconds(waitTime * 0.5f);		

        if (!hold)
            cubeColor = whichColor;
		
    }

    #endregion

    #region Collisions

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (ReplayManager.Instance.isReplaying)
            return;

        if (GlobalVariables.Instance.GameState == GameStateEnum.Playing)
        {
            if (other.collider.tag != "HoldMovable" && other.collider.tag == "Player")
                HitPlayer(other);			
			
            if (!hold && other.gameObject.layer == LayerMask.NameToLayer("Movables"))
                HitOtherMovable(other);	

            if (!hold && other.collider.tag == "HoldMovable" && playerThatThrew)
                other.collider.GetComponent<MovableScript>().player.GetComponent<PlayersGameplay>().playerThatHit = playerThatThrew.GetComponent<PlayersGameplay>();

            if (other.gameObject.layer == LayerMask.NameToLayer("Walls"))
                HitWall(other);			
        }
    }

    protected virtual void HitPlayer(Collision other)
    {
        if (other.collider.tag == "Player" && gameObject.tag == "ThrownMovable")
        {
            PlayersGameplay playerScript = other.collider.GetComponent<PlayersGameplay>();

            if (playerScript.playerState == PlayerState.Stunned)
                return;

            if (playerThatThrew == null || other.gameObject.name != playerThatThrew.name)
            {
                playerScript.StunVoid(true);
				
                InstantiateParticles(other.contacts[0], GlobalVariables.Instance.HitParticles, GlobalVariables.Instance.playersColors[(int)playerScript.playerName]);	

                if (playerThatThrew != null)
                    StatsManager.Instance.PlayersHits(playerThatThrew, other.gameObject);
            }
        }
    }

    protected virtual void HitOtherMovable(Collision other)
    {
        float numberOfParticlesFloat = (0.2f * rigidbodyMovable.velocity.magnitude);
        int numberOfParticles = (int)numberOfParticlesFloat;

        GameObject instantiatedParticles = InstantiateParticles(other.contacts[0], GlobalVariables.Instance.WallHitParticles, gameObject.GetComponent<Renderer>().material.color);

        instantiatedParticles.GetComponent<ParticleSystem>().startSize += (gameObject.transform.lossyScale.x * 0.1f);
        instantiatedParticles.GetComponent<ParticleSystem>().emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, (short)numberOfParticles, (short)numberOfParticles) });
        // instantiatedParticles.GetComponent<ParticleSystem>().Emit(numberOfParticles);

        if (playerThatThrew != null && other.collider.tag != "HoldMovable")
            other.gameObject.GetComponent<MovableScript>().playerThatThrew = playerThatThrew;

        if (canPlaySound && GlobalVariables.Instance.GameState == GameStateEnum.Playing)
            StartCoroutine(HitSound());
    }

    protected virtual void HitWall(Collision other)
    {
        if (canPlaySound)
            StartCoroutine(HitSound());
    }

    protected IEnumerator HitSound()
    {
        canPlaySound = false;

        if (currentVelocity >= limitVelocity)
            MasterAudio.PlaySound3DFollowTransformAndForget(SoundsManager.Instance.wallHitSound, transform);
        else
        {
            float soundVolume = (currentVelocity * 100) / limitVelocity / 100;
            MasterAudio.PlaySound3DFollowTransformAndForget(SoundsManager.Instance.wallHitSound, transform, soundVolume);	
        }

        yield return new WaitForSecondsRealtime(0.05f);

        canPlaySound = true;
    }

    protected virtual void PlayerKilled()
    {
        if (gameObject.GetComponent<PlayersDeadCube>() != null)
            SteamAchievements.Instance.UnlockAchievement(AchievementID.ACH_KILL_REINCARNATED_PLAYER);
    }

    #endregion

    #region Particles / FX

    public virtual GameObject InstantiateImpactFX(ContactPoint contact)
    {
        GameObject prefab = GlobalVariables.Instance.wallImpactFX[(int)cubeColor];

        Vector3 pos = contact.point;
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
        GameObject instance = Instantiate(prefab, pos, rot) as GameObject;

        instance.transform.SetParent(GlobalVariables.Instance.lastManManager.transform);

        return instance;
    }

    public virtual GameObject InstantiateParticles(ContactPoint contact, GameObject prefab, Color color)
    {
        Vector3 pos = contact.point;
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
        GameObject instance = Instantiate(prefab, pos, rot) as GameObject;

        instance.transform.SetParent(GlobalVariables.Instance.lastManManager.transform);
        instance.GetComponent<ParticleSystemRenderer>().material.color = color;

        return instance;
    }

    #endregion

    #region Hold / Release

    public virtual void DestroyRigibody()
    {
        if (rigidbodyMovable == null)
            return;

        massRb = rigidbodyMovable.mass;
        collisionDetectionModeRb = rigidbodyMovable.collisionDetectionMode;
        drag = rigidbodyMovable.drag;

        Destroy(rigidbodyMovable);
    }

    public virtual void AddRigidbody()
    {
        if (rigidbodyMovable == null)
            rigidbodyMovable = gameObject.AddComponent<Rigidbody>();
		
        rigidbodyMovable.mass = massRb;
        rigidbodyMovable.collisionDetectionMode = collisionDetectionModeRb;
        rigidbodyMovable.drag = drag;
        player.GetComponent<PlayersGameplay>().holdMovableRB = rigidbodyMovable;
    }

    public virtual void OnHold()
    {
        hold = true;

        attracedBy.Clear();
        repulsedBy.Clear();

        ToColor();

        OnHoldEventVoid();
    }

    public virtual void OnRelease()
    {
        hold = false;

        ToNeutralColor();

        OnReleaseEventVoid();
    }

    public virtual void OnHoldEventVoid()
    {
        if (OnHoldEvent != null)
            OnHoldEvent();
    }

    public virtual void OnReleaseEventVoid()
    {
        if (OnReleaseEvent != null)
            OnReleaseEvent();
    }

    protected virtual void OnDestroy()
    {
        if (cubeMaterial)
            Destroy(cubeMaterial);
    }

    #endregion
}
