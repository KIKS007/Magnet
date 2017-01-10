using UnityEngine;
using System.Collections;
using GameAnalyticsSDK;

public class GameAnalyticsManager : Singleton<GameAnalyticsManager> 
{
	private int gamesCount = 0;

	void Start ()
	{
		GlobalVariables.Instance.OnStartMode += StartModeTimer;
		GlobalVariables.Instance.OnRestartMode += StartModeTimer;
		GlobalVariables.Instance.OnEndMode += StopModeTimer;
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

		if (GlobalVariables.Instance.PlayersControllerNumber[0] == 0 || GlobalVariables.Instance.PlayersControllerNumber[1] == 0 || GlobalVariables.Instance.PlayersControllerNumber[2] == 0 || GlobalVariables.Instance.PlayersControllerNumber[3] == 0)
			mouseKeyboard = 1;

		if (GlobalVariables.Instance.PlayersControllerNumber[0] > 0)
			gamepads += 1;

		if (GlobalVariables.Instance.PlayersControllerNumber[1] > 0)
			gamepads += 1;

		if (GlobalVariables.Instance.PlayersControllerNumber[2] > 0)
			gamepads += 1;

		if (GlobalVariables.Instance.PlayersControllerNumber[3] > 0)
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
