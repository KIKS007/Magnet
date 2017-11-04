using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFXAnimations : PlayersFXAnimations
{
    [Header("AI")]
    public Transform trails;
    public Transform meshes;
    public Transform stunMeshes;
    public Transform dashDispo;
    public Transform dash;

    protected AIGameplay aiScript;

    protected override void Start()
    {
        aiScript = (AIGameplay)playerScript;

        base.Start();
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
        stunMesh = SetupPlayer(stunMeshes).transform;
        dashAvailableFX = SetupPlayer(dashDispo).GetComponent<ParticleSystem>();
        dashFX = SetupPlayer(dash).GetComponent<ParticleSystem>();

        playerColorMaterial = GlobalVariables.Instance.playersMaterials[(int)aiScript.playerName];

        stunMesh.gameObject.SetActive(false);

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
