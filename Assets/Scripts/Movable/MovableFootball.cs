using UnityEngine;
using System.Collections;

public class MovableFootball : MovableScript 
{
	protected override void OnCollisionEnter (Collision other)
	{
		if(other.collider.tag != "HoldMovable")
		{
			HitPlayer (other);			
		}			


		if(other.gameObject.tag == "Movable")
		{
			HitOtherMovable (other);
		}

		//Touched Wall
		if(other.gameObject.layer == 16)
		{
			HitWall (other);
		}

		if(other.gameObject.name == "LeftGoalTrigger")
		{
			GameObject.Find ("Football Mode Manager").GetComponent<FootballModeManager> ().GoalScoreVoid (1, gameObject);
			InstantiateParticles (other.contacts [0], GlobalVariables.Instance.MovableExplosion, gameObject.GetComponent<Renderer> ().material.color);

			gameObject.SetActive (false);
		}

		if(other.gameObject.name == "RightGoalTrigger")
		{
			GameObject.Find ("Football Mode Manager").GetComponent<FootballModeManager> ().GoalScoreVoid (2, gameObject);
			InstantiateParticles (other.contacts [0], GlobalVariables.Instance.MovableExplosion, gameObject.GetComponent<Renderer> ().material.color);

			gameObject.SetActive (false);
		}
	}
}
