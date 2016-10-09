using UnityEngine;
using System.Collections;
using GameAnalyticsSDK;

public class GameAnalyticsManager : Singleton<GameAnalyticsManager> 
{
	private int gamesCount = 0;

	void Start ()
	{
		GlobalVariables.Instance.OnModeStarted += StartModeTimer;
		GlobalVariables.Instance.OnGameOver += StopModeTimer;
	}

	public void GamesCount ()
	{
		gamesCount++;
	}

	void OnApplicationQuit ()
	{
		GameAnalytics.NewDesignEvent ("Game:GamesCount", gamesCount);
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
		

		GameAnalytics.NewDesignEvent ("Game:" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + "Mouse", mouseKeyboard);

		if(GamepadsManager.Instance.numberOfGamepads == 0 && gamepads == 1)
			GameAnalytics.NewDesignEvent ("Game:" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":GamepadsPlugged:" + GamepadsManager.Instance.numberOfGamepads, -1f);
		else
			GameAnalytics.NewDesignEvent ("Game:" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":GamepadsPlugged:" + GamepadsManager.Instance.numberOfGamepads, gamepads);
	}

	private float startModeTime;

	void StartModeTimer ()
	{
		startModeTime = Time.unscaledTime;
	}

	void StopModeTimer ()
	{
		int modeDuration = (int)(Time.unscaledTime - startModeTime);

		GameAnalytics.NewDesignEvent ("Game:" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":PlayerCount:" + GlobalVariables.Instance.NumberOfPlayers, modeDuration);
	}
}
