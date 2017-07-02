using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Sirenix.OdinInspector;
using System.IO;
using System;

public class SteamAchievements : Singleton<SteamAchievements>
{
	[PropertyOrder(-2)]
	[ButtonAttribute]
	public void UpdateAchievementsEnum ()
	{
		File.WriteAllText ("Assets/SCRIPTS/Steamworks.NET/SteamAchievementsEnum.cs", "public enum AchievementID  \n {  \n\t" + string.Join(", \n \t", AchievementsIDs.ToArray ()) +" \n };" );
	}

	[HideInEditorMode]
	[ValueDropdown ("AchievementsIDs")]
	public string achievementToUnlock;
	[PropertyOrder(-1)]
	[ButtonGroup("1", -1)]
	[HideInEditorMode]
	public void UnlockAchievementTest ()
	{
		UnlockAchievement(achievementToUnlock);
	}

	[HideInEditorMode]
	[PropertyOrder(-1)]
	[ButtonGroup("1", -1)]
	public void ResetAchievements ()
	{
		SteamUserStats.ResetAllStats (true);
		SteamUserStats.RequestCurrentStats ();
	}

	[Header ("Achievements IDs")]
	public List<string> AchievementsIDs = new List<string> ();

	[Header ("Achievements List")]
	[HideInEditorMode]
	public List<Achievement> Achievements = new List<Achievement> ();

	// Our GameID
	private CGameID m_GameID;

	private bool achievementsEventSetup = false;

	protected Callback<UserStatsReceived_t> m_UserStatsReceived;
	protected Callback<UserStatsStored_t> m_UserStatsStored;
	protected Callback<UserAchievementStored_t> m_UserAchievementStored;

	// Use this for initialization
	void Start ()
	{
		if (!SteamManager.Initialized)
			return;

		UpdateAchievementsEnum ();

		string name = SteamFriends.GetPersonaName ();
		Debug.Log (name);

		Achievements.Clear ();

		foreach(string s in AchievementsIDs)
			Achievements.Add (new Achievement (s));
	}

	void OnEnable() 
	{
		if (!SteamManager.Initialized)
			return;

		// Cache the GameID for use in the Callbacks
		m_GameID = new CGameID(SteamUtils.GetAppID());

		m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
		m_UserStatsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
		m_UserAchievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);

		StartCoroutine (GetSteamData ());
	}

	IEnumerator GetSteamData ()
	{
		bool bSuccess = false;

		do
		{
			bSuccess = SteamUserStats.RequestCurrentStats();
			yield return new WaitForEndOfFrame ();
		}
		while (!bSuccess);
	}

	IEnumerator StoreSteamData ()
	{
		bool bSuccess = false;

		do
		{
			bSuccess = SteamUserStats.StoreStats();
			yield return new WaitForEndOfFrame ();
		}
		while (!bSuccess);
	}

	void SetupAchievementsEvents ()
	{
		achievementsEventSetup = true;

		if (!Achieved (AchievementID.ACH_WIN_ONE_GAME))
			GlobalVariables.Instance.OnStartMode += () => UnlockAchievement (AchievementID.ACH_WIN_ONE_GAME);
	}

	bool Achieved (AchievementID id)
	{
		foreach(var a in Achievements)
		{
			if(a.achievementID == id)
			{
				if(a.achieved)
					return true;
				else
					return false;
			}
		}
		return false;
	}

	public void UnlockAchievement(Achievement achievement) 
	{
		if (!SteamManager.Initialized)
			return;
		
		if (achievement.achieved)
			return;

		achievement.achieved = true;

		// mark it down
		SteamUserStats.SetAchievement(achievement.achievementID.ToString ());

		StartCoroutine (StoreSteamData ());
	}

	public void UnlockAchievement(string achievementID) 
	{
		if (!SteamManager.Initialized)
			return;
		
		foreach (var a in Achievements)
			if (a.achievementID.ToString () == achievementID)
			{
				if (a.achieved)
					return;

				a.achieved = true;
				break;
			}

		// mark it down
		SteamUserStats.SetAchievement(achievementID);

		StartCoroutine (StoreSteamData ());
	}

	public void UnlockAchievement(AchievementID achievementID) 
	{
		if (!SteamManager.Initialized)
			return;
		
		foreach (var a in Achievements)
			if (a.achievementID == achievementID)
			{
				if (a.achieved)
					return;

				a.achieved = true;
				break;
			}

		// mark it down
		SteamUserStats.SetAchievement(achievementID.ToString ());

		StartCoroutine (StoreSteamData ());
	}

	//-----------------------------------------------------------------------------
	// Purpose: We have stats data from Steam. It is authoritative, so update
	//			our data with those results now.
	//-----------------------------------------------------------------------------
	void OnUserStatsReceived(UserStatsReceived_t pCallback) 
	{
		if (!SteamManager.Initialized)
			return;

		// we may get callbacks for other games' stats arriving, ignore them
		if ((ulong)m_GameID == pCallback.m_nGameID) 
		{
			if (EResult.k_EResultOK == pCallback.m_eResult) 
			{
				Debug.Log("Received stats and achievements from Steam\n");

				// load achievements
				foreach (Achievement ach in Achievements) 
				{
					bool ret = SteamUserStats.GetAchievement(ach.achievementID.ToString (), out ach.achieved);

					if (ret) 
					{
						ach.name = SteamUserStats.GetAchievementDisplayAttribute(ach.achievementID.ToString (), "name");
						ach.description = SteamUserStats.GetAchievementDisplayAttribute(ach.achievementID.ToString (), "desc");
						SteamUserStats.GetAchievement(ach.achievementID.ToString (), out ach.achieved);
					}
					else 
					{
						Debug.LogWarning("SteamUserStats.GetAchievement failed for Achievement " + ach.achievementID.ToString () + "\nIs it registered in the Steam Partner site?");
					}
				}

				StartCoroutine (StoreSteamData ());
				
				if (!achievementsEventSetup)
					SetupAchievementsEvents ();
			}
			else 
			{
				Debug.Log("RequestStats - failed, " + pCallback.m_eResult);
			}
		}
	}

	//-----------------------------------------------------------------------------
	// Purpose: Our stats data was stored!
	//-----------------------------------------------------------------------------
	void OnUserStatsStored(UserStatsStored_t pCallback) 
	{
		// we may get callbacks for other games' stats arriving, ignore them
		if ((ulong)m_GameID == pCallback.m_nGameID) 
		{
			if (EResult.k_EResultOK == pCallback.m_eResult) 
			{
				Debug.Log("StoreStats - success");
			}
			else if (EResult.k_EResultInvalidParam == pCallback.m_eResult) 
			{
				// One or more stats we set broke a constraint. They've been reverted,
				// and we should re-iterate the values now to keep in sync.
				Debug.Log("StoreStats - some failed to validate");
				// Fake up a callback here so that we re-load the values.
				UserStatsReceived_t callback = new UserStatsReceived_t();
				callback.m_eResult = EResult.k_EResultOK;
				callback.m_nGameID = (ulong)m_GameID;
				OnUserStatsReceived(callback);
			}
			else 
			{
				Debug.Log("StoreStats - failed, " + pCallback.m_eResult);
			}
		}
	}

	//-----------------------------------------------------------------------------
	// Purpose: An achievement was stored
	//-----------------------------------------------------------------------------
	void OnAchievementStored(UserAchievementStored_t pCallback) 
	{
		// We may get callbacks for other games' stats arriving, ignore them
		if ((ulong)m_GameID == pCallback.m_nGameID) 
		{
			if (0 == pCallback.m_nMaxProgress) 
			{
				Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' unlocked!");
			}
			else 
			{
				Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' progress callback, (" + pCallback.m_nCurProgress + "," + pCallback.m_nMaxProgress + ")");
			}
		}
	}

	[System.Serializable]
	public class Achievement 
	{
		public AchievementID achievementID;
		public string name;
		public string description;
		public bool achieved;

		public Achievement(string achievementID, string name = "", string desc = "") 
		{
			this.achievementID = (AchievementID) Enum.Parse (typeof(AchievementID), achievementID);
			this.name = name;
			description = desc;
			achieved = false;
		}
	}
}
