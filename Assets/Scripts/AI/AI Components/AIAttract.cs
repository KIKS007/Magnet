using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAttract : AIComponent
{
    protected override void Enable()
    {
        if (!AIScript.shootLayerEnabled)
            return;

        if (!CanPlay())
            return;

        Debug.Log("Attract Enable");

        base.Enable();
		
        AIScript.isAttracting = true;
    }

    protected override void Update()
    {
        base.Update();

        if (!AIScript.shootLayerEnabled)
            return;

        if (!AIScript.isAttracting)
            AIScript.isAttracting = true;
    }

    protected override void OnDisable()
    {
        if (!AIScript.shootLayerEnabled)
            return;
		
        base.OnDisable();
		
        AIScript.isAttracting = false;
    }
}
