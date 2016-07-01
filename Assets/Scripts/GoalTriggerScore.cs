using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GoalTriggerScore : MonoBehaviour 
{
	public GameObject footballScoreManager;
	

	// Use this for initialization
	void Start () 
	{

	}


	public void ScoreUpdate ()
	{
		if(gameObject.name == "LeftGoalTrigger")
		{
			footballScoreManager.GetComponent<FootballModeManager>().StartCoroutine("RightSideGoal");
		}

		else if(gameObject.name == "RightGoalTrigger")
		{
			footballScoreManager.GetComponent<FootballModeManager>().StartCoroutine("LeftSideGoal");
		}
	}
}
