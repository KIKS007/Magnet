﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStuck : AIMovement_Towards 
{
	protected override void OnEnable ()
	{
		AIScript.holdTarget = null;
		AIScript.shootTarget = null;
	}
}