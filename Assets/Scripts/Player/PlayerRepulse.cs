using UnityEngine;
using System.Collections;

public class PlayerRepulse : PlayersGameplay 
{
	[Header ("Player Zone")]
	public RepulseTriggerZones playerZone = RepulseTriggerZones.None;
}
