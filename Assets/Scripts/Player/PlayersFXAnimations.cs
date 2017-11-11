#pragma warning disable 0618

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DarkTonic.MasterAudio;
using Replay;

public class PlayersFXAnimations : MonoBehaviour
{
    public System.Action OnStunFXON;
    public System.Action OnStunFXOFF;

    [Header("Trail FX Settings")]
    public float trailTweenDuration = 0.5f;
    public float lowSpeedtime = 0.1f;
    public float lowSpeedstartWidth = 1;
    public float highSpeedtime = 0.1f;
    public float highSpeedstartWidth = 1;
    public float dashingTime = 0.5f;
    public float dashingStartWidth = 2;

    [Header("Shoot FX Settings")]
    public Vector3 shootPosOffset;

    [Header("Stun FX Settings")]
    public Material playerColorMaterial;
    public float[] stunFXDurations = new float[6];

    protected List<Renderer> playerMaterials = new List<Renderer>();

    [Header("Dash FX")]
    public ParticleSystem dashFX;

    [Header("Dash Available FX")]
    public ParticleSystem dashAvailableFX;

    [Header("Dash Ongle FX")]
    public Transform dashOngleFX;
    
    [Header("Stun Mesh")]
    public Transform stunMesh;

    protected float stunMeshGrowthDuration = 0.25f;
    protected float stunMeshGrowth = 1.3f;

    [Header("Player Mesh")]
    public Transform playerMesh;
    public float leanSpeed;
    public float leanLerp = 0.1f;
    public float leanMaxAngle;

    [Header("Safe FX")]
    public float safeDurationBetween = 0.5f;

    [Header("Attraction Settings")]
    public float aFactor = 0.03f;
    public float bFactor = -0.03f;

    [Header("Repulsion Settings")]
    public float aFactor2 = 0.03f;
    public float bFactor2 = -0.03f;

    protected PlayersGameplay playerScript;
    protected PlayersSounds playerSoundsScript;
    protected PlayerName playerName;
    protected int playerNumber = -1;
    protected float spawnDuration = 0.2f;
    protected Vector3 initialScale;
    protected Color initialEmission;
    protected float stunEmissionValue = 0.2f;
    protected Color playerColor;
    protected Material dashOngleMaterial;
    protected float dashOngleAlpha;
    [HideInInspector]
    public float distance;
    [HideInInspector]
    public List<GameObject> attractionRepulsionFX = new List<GameObject>();
    protected Vector3 initialStunMeshScale = Vector3.one;
    [HideInInspector]
    public bool stunned = false;

    protected virtual void Awake()
    {
        initialScale = transform.localScale;
    }

    // Use this for initialization
    protected virtual void Start()
    {
        if (dashOngleFX != null)
        {
            dashOngleMaterial = dashOngleFX.GetChild(0).GetComponent<Renderer>().material;
            dashOngleAlpha = dashOngleMaterial.GetColor("_TintColor").a;
        }
        
        playerScript = GetComponent<PlayersGameplay>();
        playerSoundsScript = GetComponent<PlayersSounds>();

        playerScript.OnShoot += ShootFX;
        playerScript.OnDashAvailable += DashAvailableFX;
        playerScript.OnDash += StopDashAvailable;
        playerScript.OnDashEnd += DisableDashFX;
        playerScript.OnStun += () => StartCoroutine(StunFX());
        playerScript.OnDash += EnableDashFX;
        playerScript.OnDeath += RemoveAttractionRepulsionFX;
        playerScript.OnSafe += () => StartCoroutine(SafeFX());
        playerScript.OnSafeEnd += OnSafeEnd;

        DisableDashFX();

        SetupMaterials();

        Setup();
    }

    protected virtual void Setup()
    {
        playerMesh.gameObject.SetActive(true);
        if (stunMesh != null)
            stunMesh.gameObject.SetActive(false);

        playerColor = GlobalVariables.Instance.playersColors[(int)playerScript.playerName];
        stunned = false;

        SetupMaterials();

        playerName = playerScript.playerName;
        playerNumber = (int)playerScript.playerName;
    }

    protected virtual void SetupMaterials()
    {
        var renderers = playerMesh.GetComponentsInChildren<Renderer>();

        foreach (var r in renderers)
            if (r.material.HasProperty("_Color") && r.material.color == playerColorMaterial.color)
                playerMaterials.Add(r);

        initialEmission = playerMaterials[0].material.GetColor("_EmissionColor");
    }

    protected virtual void OnEnable()
    {
        if (GlobalVariables.Instance.GameState == GameStateEnum.Playing)
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(initialScale, spawnDuration).SetEase(Ease.InBack).SetUpdate(false);
        }

        for (int i = 0; i < playerMaterials.Count; i++)
            if (playerMaterials[i] != null)
                playerMaterials[i].material.SetColor("_EmissionColor", initialEmission);

        if (stunMesh != null && gameObject.layer == LayerMask.NameToLayer("Safe"))
        {
            stunMesh.gameObject.SetActive(true);
            playerMesh.gameObject.SetActive(false);
        }

        stunned = false;
    }
	
    // Update is called once per frame
    protected virtual void Update()
    {
        if (GlobalVariables.Instance.GameState != GameStateEnum.Playing || playerScript.playerState == PlayerState.Startup)
            return;

        if (ReplayManager.Instance.isReplaying)
            return;

        if (dashAvailableFX != null && dashAvailableFX.isPlaying)
        {
            ParticleSystem.Particle[] particlesList = new ParticleSystem.Particle[dashAvailableFX.particleCount];
            dashAvailableFX.GetParticles(particlesList);

            for (int i = 0; i < particlesList.Length; i++)
                particlesList[i].rotation = transform.rotation.eulerAngles.y;

            dashAvailableFX.SetParticles(particlesList, particlesList.Length);
            dashAvailableFX.startRotation = transform.rotation.eulerAngles.y;
        }

        LeanMesh();
    }

    protected virtual void LeanMesh()
    {
        Vector3 movementDirection = transform.InverseTransformDirection(playerScript.movement);
        Vector3 newRotation = new Vector3();

        if (movementDirection.z > 0.5f || movementDirection.z < -0.5f)
            newRotation.x = movementDirection.z * leanSpeed;
        else
            newRotation.x = 0;

        if (movementDirection.x > 0.5f || movementDirection.x < -0.5f)
            newRotation.z = -movementDirection.x * leanSpeed;
        else
            newRotation.z = 0;

        if (playerScript.holdState == HoldState.Holding)
            newRotation = Vector3.zero;

        playerMesh.localRotation = Quaternion.Lerp(playerMesh.localRotation, Quaternion.Euler(newRotation), leanLerp);

        playerMesh.localEulerAngles = new Vector3(ClampAngle(playerMesh.localEulerAngles.x, -leanMaxAngle, leanMaxAngle), playerMesh.localEulerAngles.y, ClampAngle(playerMesh.localEulerAngles.z, -leanMaxAngle, leanMaxAngle));
    }

    protected virtual float ClampAngle(float angle, float min, float max)
    {
        if (angle < 90 || angle > 270)
        {
            if (angle > 180)
                angle -= 360;

            if (max > 180)
                max -= 360;

            if (min > 180)
                min -= 360;
        }

        angle = Mathf.Clamp(angle, min, max);

        if (angle < 0)
            angle += 360;

        return angle;
    }

    protected virtual void EnableDashFX()
    {
        StartCoroutine(DashOngleFx());

        if (dashOngleFX != null)
        {
            dashOngleFX.gameObject.SetActive(true);
            dashOngleMaterial.DOFade(dashOngleAlpha, "_TintColor", 0.1f);
        } 

        if (dashFX != null)
            dashFX.Play();
    }

    protected virtual IEnumerator DashOngleFx()
    {
        if (dashOngleFX == null)
            yield break;

        while (playerScript.dashState == DashState.Dashing)
        {
            dashOngleFX.LookAt(transform.position + playerScript.playerRigidbody.velocity);
            dashOngleFX.localRotation = dashOngleFX.localRotation;

            yield return new WaitForEndOfFrame();
        }
    }

    protected virtual void DisableDashFX()
    {
        if (dashFX != null)
            dashFX.Stop();

        if (dashOngleFX != null)
            dashOngleMaterial.DOFade(0, "_TintColor", 0.1f).OnComplete(() =>
                {
                    dashOngleFX.gameObject.SetActive(false);
                });
    }

    protected virtual void ShootFX()
    {
        Instantiate(GlobalVariables.Instance.shootFX[playerNumber], transform.position + shootPosOffset, transform.rotation, GlobalVariables.Instance.lastManManager.transform);
    }

    protected virtual IEnumerator StunFX()
    {
        float[] stunFXDurationsTemp = stunFXDurations;
        stunFXDurationsTemp[0] = stunFXDurations[0] + Random.Range(-0.005f, 0.005f);
        stunFXDurationsTemp[1] = stunFXDurationsTemp[0];

        stunFXDurationsTemp[2] = stunFXDurations[2] + Random.Range(-0.005f, 0.005f);
        stunFXDurationsTemp[3] = stunFXDurationsTemp[2];

        stunFXDurationsTemp[4] = stunFXDurations[4] + Random.Range(-0.005f, 0.005f);
        stunFXDurationsTemp[5] = stunFXDurations[5] + Random.Range(-0.005f, 0.005f);

        StunOFF();

        yield return new WaitForSeconds(stunFXDurationsTemp[0]);

        StunON();

        yield return new WaitForSeconds(stunFXDurationsTemp[1]);

        StunOFF();

        yield return new WaitForSeconds(stunFXDurationsTemp[2]);

        StunON();
       
        yield return new WaitForSeconds(stunFXDurationsTemp[3]);

        StunOFF();

        yield return new WaitForSeconds(stunFXDurationsTemp[4]);

        StunON();
       
        yield return new WaitForSeconds(stunFXDurationsTemp[5]);

        StunOFF();
       
        yield return new WaitUntil(() => playerScript.playerState != PlayerState.Stunned);

        for (int i = 0; i < playerMaterials.Count; i++)
            playerMaterials[i].material.SetColor("_EmissionColor", initialEmission);

        if (!ReplayManager.Instance.isReplaying)
            playerSoundsScript.StunEND();

        if (OnStunFXON != null)
            OnStunFXON();
    }

    public virtual void StunON()
    {
        if (OnStunFXON != null)
            OnStunFXON();

        for (int i = 0; i < playerMaterials.Count; i++)
            playerMaterials[i].material.SetColor("_EmissionColor", initialEmission);

        stunned = true;

        if (!ReplayManager.Instance.isReplaying)
            playerSoundsScript.StunON(); 
    }

    public virtual void StunOFF()
    {
        if (OnStunFXOFF != null)
            OnStunFXOFF();

        for (int i = 0; i < playerMaterials.Count; i++)
            playerMaterials[i].material.SetColor("_EmissionColor", initialEmission * stunEmissionValue);

        stunned = false;

        if (!ReplayManager.Instance.isReplaying)
            playerSoundsScript.StunOFF();
    }

    protected virtual void DashAvailableFX()
    {
        dashAvailableFX.Play();
    }

    protected virtual void StopDashAvailable()
    {
        if (dashAvailableFX.isPlaying)
            dashAvailableFX.Stop();
    }

    protected virtual IEnumerator SafeFX()
    {
        if (stunMesh != null)
        {
            stunMesh.localScale = initialStunMeshScale;

            stunMesh.gameObject.SetActive(true);
            playerMesh.gameObject.SetActive(false);

            stunMesh.DOScale(initialStunMeshScale * stunMeshGrowth, stunMeshGrowthDuration).SetLoops(-1, LoopType.Yoyo);
        }

        while (gameObject.layer == LayerMask.NameToLayer("Safe"))
        {
            for (int i = 0; i < playerMaterials.Count; i++)
                playerMaterials[i].material.SetColor("_EmissionColor", initialEmission * stunEmissionValue);

            playerSoundsScript.StunOFF();

            if (gameObject.layer != LayerMask.NameToLayer("Safe"))
                break;
			
            yield return new WaitForSeconds(safeDurationBetween);

            for (int i = 0; i < playerMaterials.Count; i++)
                playerMaterials[i].material.SetColor("_EmissionColor", initialEmission);

            playerSoundsScript.StunON();

            yield return new WaitForSeconds(safeDurationBetween);
        }
    }

    protected virtual void OnSafeEnd()
    {
        if (stunMesh != null)
        {
            stunMesh.gameObject.SetActive(false);
            playerMesh.gameObject.SetActive(true);
            DOTween.Kill(stunMesh);
        }

        for (int i = 0; i < playerMaterials.Count; i++)
            playerMaterials[i].material.SetColor("_EmissionColor", initialEmission);

        playerSoundsScript.StunON();
    }

    public virtual IEnumerator AttractionFX(GameObject whichCube)
    {
        GameObject fx = Instantiate(GlobalVariables.Instance.attractFX[playerNumber], whichCube.transform.position, transform.rotation) as GameObject;
        attractionRepulsionFX.Add(fx);

        ReplayManager.Instance._attractionParticles.Add(fx.GetComponent<ReplayParticles>());

        ParticleSystem ps = fx.GetComponent<ParticleSystem>();
        fx.transform.SetParent(GlobalVariables.Instance.lastManManager.transform);

        ps.startSize = 3 + whichCube.transform.lossyScale.x;

        StartCoroutine(SetAttractionParticles(whichCube, fx, ps));

        MovableScript script = whichCube.GetComponent<MovableScript>();

        yield return new WaitWhile(() => playerScript.cubesAttracted.Contains(script));

        ps.Stop();

        yield return new WaitWhile(() => ps.IsAlive());

        attractionRepulsionFX.Remove(fx);

        if (fx.GetComponent<ReplayParticles>() == null)
            Destroy(fx);
        else
            fx.gameObject.SetActive(false);
    }

    protected virtual IEnumerator SetAttractionParticles(GameObject whichCube, GameObject fx, ParticleSystem ps)
    {
        while (ps != null && ps.IsAlive())
        {
            fx.transform.position = whichCube.transform.position;

            Vector3 lookPos = new Vector3(transform.position.x, fx.transform.position.y, transform.position.z);
            fx.transform.LookAt(lookPos);

            float dist = Vector3.Distance(transform.position, whichCube.transform.position);
            float lifeTime = aFactor * dist + bFactor;

            ps.startLifetime = lifeTime;

            ParticleSystem.Particle[] particlesList = new ParticleSystem.Particle[ps.particleCount];
            ps.GetParticles(particlesList);

            distance = Vector3.Distance(transform.position, whichCube.transform.position);

            for (int i = 0; i < ps.particleCount; i++)
            {
                dist = Vector3.Distance(whichCube.transform.position, ps.transform.TransformPoint(particlesList[i].position));
                lifeTime = aFactor * dist + bFactor;

                if (dist >= distance)
                    particlesList[i].startLifetime = 0;
                /* else
                    particlesList[i].startLifetime = lifeTime;*/
            }

            ps.SetParticles(particlesList, particlesList.Length);

            yield return new WaitForFixedUpdate();
        }
    }

    public virtual IEnumerator RepulsionFX(GameObject whichCube)
    {
        GameObject fx = Instantiate(GlobalVariables.Instance.repulseFX[playerNumber], whichCube.transform.position, transform.rotation) as GameObject;
        attractionRepulsionFX.Add(fx);

        ReplayManager.Instance._attractionParticles.Add(fx.GetComponent<ReplayParticles>());

        ParticleSystem ps = fx.GetComponent<ParticleSystem>();
        fx.transform.SetParent(GlobalVariables.Instance.lastManManager.transform);

        ps.startSize = 3 + whichCube.transform.lossyScale.x;

        StartCoroutine(SetRepulsionParticles(whichCube, fx, ps));

        MovableScript script = whichCube.GetComponent<MovableScript>();

        yield return new WaitWhile(() => playerScript.cubesRepulsed.Contains(script));

        ps.Stop();

        yield return new WaitWhile(() => ps.IsAlive());

        attractionRepulsionFX.Remove(fx);

        if (fx.GetComponent<ReplayParticles>() == null)
            Destroy(fx);
        else
            fx.gameObject.SetActive(false);
    }

    protected virtual IEnumerator SetRepulsionParticles(GameObject whichCube, GameObject fx, ParticleSystem ps)
    {
        while (ps.IsAlive())
        {
            fx.transform.position = transform.position;

            Vector3 lookPos = new Vector3(whichCube.transform.position.x, fx.transform.position.y, whichCube.transform.position.z);
            fx.transform.LookAt(lookPos);

            float dist = Vector3.Distance(transform.position, whichCube.transform.position);
            float lifeTime = aFactor2 * dist + bFactor2;

            ps.startLifetime = lifeTime;

            ParticleSystem.Particle[] particlesList = new ParticleSystem.Particle[ps.particleCount];
            ps.GetParticles(particlesList);

            distance = Vector3.Distance(transform.position, whichCube.transform.position);

            for (int i = 0; i < ps.particleCount; i++)
            {
                dist = Vector3.Distance(transform.position, ps.transform.TransformPoint(particlesList[i].position));

                if (dist >= distance)
                    particlesList[i].startLifetime = 0;
                else
                    particlesList[i].startLifetime = lifeTime;
            }

            ps.SetParticles(particlesList, particlesList.Length);

            yield return new WaitForFixedUpdate();
        }
    }

    protected virtual void RemoveAttractionRepulsionFX()
    {
        for (int i = 0; i < attractionRepulsionFX.Count; i++)
        {
            Destroy(attractionRepulsionFX[i]);
        }
    }

    public virtual void DeathExplosionFX(Vector3 position)
    {
        int playerNumber = (int)playerName;

        GameObject instance = Instantiate(GlobalVariables.Instance.explosionFX[playerNumber], position, GlobalVariables.Instance.explosionFX[playerNumber].transform.rotation) as GameObject;
        instance.transform.parent = GlobalVariables.Instance.lastManManager.transform;

        MasterAudio.PlaySound3DAtTransformAndForget(SoundsManager.Instance.explosionSound, transform);
    }

    public virtual void DeathExplosionFX()
    {
        int playerNumber = (int)playerName;

        GameObject instance = Instantiate(GlobalVariables.Instance.explosionFX[playerNumber], transform.position, GlobalVariables.Instance.explosionFX[playerNumber].transform.rotation) as GameObject;
        instance.transform.parent = GlobalVariables.Instance.lastManManager.transform;

        MasterAudio.PlaySound3DAtTransformAndForget(SoundsManager.Instance.explosionSound, transform);
    }

    public virtual void WaveFX(bool singleWave = false)
    {
        int loopsCount = !singleWave ? (int)playerName + 1 : 1;

        for (int i = 0; i < loopsCount; i++)
        {
            DOVirtual.DelayedCall(GlobalVariables.Instance.delayBetweenWavesFX * i, () =>
                {
                    SpawnWave();                   
                });
        }
    }

    void SpawnWave()
    {
        Quaternion rotation = Quaternion.Euler(new Vector3(90, 0, 0));

        Instantiate(GlobalVariables.Instance.waveFX[playerNumber], transform.position, rotation, transform);

        if (GetComponent<PlayersVibration>() != null)
            GetComponent<PlayersVibration>().Wave();
    }


    protected virtual void OnDestroy()
    {
        if (dashOngleMaterial)
            Destroy(dashOngleMaterial);
    }

    public virtual GameObject DeathParticles(Vector3 position)
    {
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, Vector3.up);
        GameObject instantiatedParticles = Instantiate(GlobalVariables.Instance.DeadParticles, position, rot) as GameObject;

        instantiatedParticles.transform.SetParent(GlobalVariables.Instance.lastManManager.transform);
        instantiatedParticles.GetComponent<ParticleSystemRenderer>().material.color = playerColor;

        return instantiatedParticles;
    }
}
