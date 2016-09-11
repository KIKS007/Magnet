using UnityEngine;
using System.Collections;

public class PlayersRepulse : PlayersGameplay 
{
	[Header ("Player Zone")]
	public RepulseTriggerZones playerZone = RepulseTriggerZones.None;
}
