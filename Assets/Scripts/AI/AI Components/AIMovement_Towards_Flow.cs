using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIMovement_Towards_Flow : AIMovement_Towards
{
    private int randomCubes = 4;

    protected override void OnEnable()
    {
        AIScript.holdTarget = null;

        base.OnEnable();

        if (!CanPlay())
            return;
    }

    protected override void Enable()
    {
        if (!AIScript.movementLayerEnabled)
            return;

        if (!CanPlay())
            return;

        base.Enable();

        ChooseTarget();
    }

    void ChooseTarget()
    {
        List<GameObject> targetsTemp = new List<GameObject>(GlobalVariables.Instance.AllMovables);

        List<GameObject> players = new List<GameObject>(GlobalVariables.Instance.AlivePlayersList);
        players.Remove(gameObject);

        GameObject player = players[Random.Range(0, players.Count)];

        player = gameObject;

        targetsTemp = targetsTemp.OrderBy(x => Vector3.Distance(player.transform.position, x.transform.position)).ToList();

        int loopCount = 0;

        if (targetsTemp.Count >= randomCubes)
        {
            do
            {
                if (loopCount > 300)
                    return;

                loopCount++;

                AIScript.shootTarget = target = targetsTemp[Random.Range(0, randomCubes)].transform;
            }
            while(!AIScript.shootTarget.gameObject.activeSelf);
        }
        else
        {
            if (targetsTemp[0].activeSelf)
                AIScript.shootTarget = target = targetsTemp[0].transform;
        }
    }

    protected override void Update()
    {
        if (!AIScript.movementLayerEnabled)
            return;

        if (!CanPlay())
            return;

        base.Update();

        if (AIScript.shootTarget != null && !AIScript.shootTarget.gameObject.activeSelf)
        {
            AIScript.shootTarget = null;
            target = null;
        }

        if (AIScript.closerPlayers.Count == 0)
            return;

        if (target == null)
            ChooseTarget();

        if (AIScript.shootTarget)
            AIScript.aiAnimator.SetFloat("cubeDistanceFromCenter", Vector3.Distance(AIScript.shootTarget.position, Vector3.zero));
        else
            AIScript.aiAnimator.SetFloat("cubeDistanceFromCenter", 666);
    }

    protected override void OnDisable()
    {
        //AIScript.playerTarget = null;
        target = null;

        base.OnDisable();
    }
}
