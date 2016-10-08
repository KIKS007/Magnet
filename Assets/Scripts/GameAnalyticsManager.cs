using UnityEngine;
using System.Collections;
using GameAnalyticsSDK;

public class GameAnalyticsManager : Singleton<GameAnalyticsManager> 
{
	public void CreditLinks (string whichLink)
	{
		GameAnalytics.NewDesignEvent (whichLink);
	}

	public void PlayersNumber ()
	{
		GameAnalytics.NewDesignEvent ("Players Number", GlobalVariables.Instance.NumberOfPlayers);
	}

	public void PlayersControllers ()
	{
		int mouseKeyboard = 0;
		int gamepads = 0;

		if (GlobalVariables.Instance.ControllerNumberPlayer1 == 0 || GlobalVariables.Instance.ControllerNumberPlayer2 == 0 || GlobalVariables.Instance.ControllerNumberPlayer3 == 0 || GlobalVariables.Instance.ControllerNumberPlayer4 == 0)
			mouseKeyboard = 1;

		if (GlobalVariables.Instance.ControllerNumberPlayer1 > 0)
			gamepads += 1;

		if (GlobalVariables.Instance.ControllerNumberPlayer2 > 0)
			gamepads += 1;

		if (GlobalVariables.Instance.ControllerNumberPlayer3 > 0)
			gamepads += 1;

		if (GlobalVariables.Instance.ControllerNumberPlayer4 > 0)
			gamepads += 1;
		
		string playersControllers = "Mouse : " + mouseKeyboard + " ; Gamepads : " + gamepads + " ; Gamepads Plugged : " + GamepadsManager.Instance.numberOfGamepads;

		GameAnalytics.NewDesignEvent ("Players Controllers = " + playersControllers);

		GameAnalytics.NewDesignEvent ("Mouse Controller", mouseKeyboard);

		GameAnalytics.NewDesignEvent ("Gamepads Controllers", gamepads);

		GameAnalytics.NewDesignEvent ("Gamepads Plugged", GamepadsManager.Instance.numberOfGamepads);
	}
}
