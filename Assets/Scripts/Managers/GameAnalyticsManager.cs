using UnityEngine;
using System.Collections;
using GameAnalyticsSDK;

public class GameAnalyticsManager : Singleton<GameAnalyticsManager> 
{
	private int gamesCount = 0;

	void Start ()
	{
		MenuManager.Instance.OnStartModeClick += GamesCount;
		MenuManager.Instance.OnStartModeClick += PlayersControllers;

		GlobalVariables.Instance.OnStartMode += PlayersControllers;

		GlobalVariables.Instance.OnStartMode += OnModeStart;
		GlobalVariables.Instance.OnRestartMode += OnModeStart;

		GlobalVariables.Instance.OnRestartMode += ModeRestart;

		GlobalVariables.Instance.OnEndMode += OnEndMode;
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

		GameAnalytics.NewDesignEvent ("Game:" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":GamepadsCount", gamepads);

		GameAnalytics.NewDesignEvent ("Game:" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":Mouse", mouseKeyboard);

		GameAnalytics.NewDesignEvent ("Game:" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":PlayersCount", GlobalVariables.Instance.NumberOfPlayers);

		GameAnalytics.NewDesignEvent ("Game:" + GlobalVariables.Instance.CurrentModeLoaded.ToString() + ":BotsCount", GlobalVariables.Instance.NumberOfBots);

		if(GlobalVariables.Instance.NumberOfBots > 0)
		{
			foreach(var p in GlobalVariables.Instance.AlivePlayersList)
			{
				if (p == null)
					continue;

				AIGameplay script = p.GetComponent<AIGameplay> ();

				if (script == null)
					continue;
							
				GameAnalytics.NewDesignEvent ("Game:" + GlobalVariables.Instance.CurrentModeLoaded.ToString () + ":BotsLevel", (int) script.aiLevel);
			}
		}
	}

	void ModeRestart ()
	{
		GameAnalytics.NewDesignEvent ("Game:" + GlobalVariables.Instance.ModeSequenceType.ToString () + ":" + GlobalVariables.Instance.CurrentModeLoaded.ToString () + ":GamesInARow");
	}

	void OnModeStart ()
	{
		GamesCount ();
	}

	void GamesCount ()
	{
		gamesCount++;
	}

	void OnEndMode ()
	{
		
	}

	void OnApplicationQuit ()
	{
		GameAnalytics.NewDesignEvent ("Game:GamesCount", gamesCount);
	}
}
