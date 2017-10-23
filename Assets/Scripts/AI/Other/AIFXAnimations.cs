using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFXAnimations : PlayersFXAnimations
{
    [Header("AI")]
    public Transform trails;
    public Transform meshes;
    public Transform dashDispo;
    public Transform dash;

    protected AIGameplay aiScript;

    protected override void Start()
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

        aiScript = (AIGameplay)playerScript;

        DisableDashFX();

        SetupMaterials();

        Setup();
    }

    public void AISetup()
    {
        if (aiScript == null)
        {
            playerScript = GetComponent<PlayersGameplay>();
            aiScript = (AIGameplay)playerScript;
        }

        SetupPlayer(trails);

        playerMesh = SetupPlayer(meshes).transform;
        dashAvailableFX = SetupPlayer(dashDispo).GetComponent<ParticleSystem>();
        dashFX = SetupPlayer(dash).GetComponent<ParticleSystem>();

        playerColorMaterial = GlobalVariables.Instance.playersMaterials[(int)aiScript.playerName];

        base.Setup();
    }

    GameObject SetupPlayer(Transform t)
    {
        GameObject g = null;

        for (int i = 0; i < t.childCount; i++)
        {
            if (i != (int)aiScript.playerName)
                t.GetChild(i).gameObject.SetActive(false);
            else
            {
                g = t.GetChild(i).gameObject;
                t.GetChild(i).gameObject.SetActive(true);
            }
        }

        return g;
    }
}
