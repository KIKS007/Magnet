using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StatsScript : MonoBehaviour 
{
	public GameObject player1;
	public GameObject player2;
	public GameObject player3;
	public GameObject player4;

	public int Player_1frags;
	public int Player_2frags;
	public int Player_3frags;
	public int Player_4frags;

	public int Player_1HitByPlayer_2;
	public int Player_1HitByPlayer_3;
	public int Player_1HitByPlayer_4;
	public int Player_1TotalHit;

	public int Player_2HitByPlayer_1;
	public int Player_2HitByPlayer_3;
	public int Player_2HitByPlayer_4;
	public int Player_2TotalHit;

	public int Player_3HitByPlayer_1;
	public int Player_3HitByPlayer_2;
	public int Player_3HitByPlayer_4;
	public int Player_3TotalHit;

	public int Player_4HitByPlayer_1;
	public int Player_4HitByPlayer_2;
	public int Player_4HitByPlayer_3;
	public int Player_4TotalHit;

	public float Player_1Ratio;
	public float Player_2Ratio;
	public float Player_3Ratio;
	public float Player_4Ratio;

	public Text player_1Stats;
	public Text player_2Stats;
	public Text player_3Stats;
	public Text player_4Stats;

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	public void StatsUpdate ()
	{
		Player_1frags = Player_2HitByPlayer_1 + Player_3HitByPlayer_1 + Player_4HitByPlayer_1;
		Player_2frags = Player_1HitByPlayer_2 + Player_3HitByPlayer_2 + Player_4HitByPlayer_2;
		Player_3frags = Player_1HitByPlayer_3 + Player_2HitByPlayer_3 + Player_4HitByPlayer_3;
		Player_4frags = Player_1HitByPlayer_4 + Player_2HitByPlayer_4 + Player_3HitByPlayer_4;
		
		Player_1TotalHit = Player_1HitByPlayer_2 + Player_1HitByPlayer_3 + Player_1HitByPlayer_4;
		Player_2TotalHit = Player_2HitByPlayer_1 + Player_2HitByPlayer_3 + Player_2HitByPlayer_4;
		Player_3TotalHit = Player_3HitByPlayer_1 + Player_3HitByPlayer_2 + Player_3HitByPlayer_4;
		Player_4TotalHit = Player_4HitByPlayer_1 + Player_4HitByPlayer_2 + Player_4HitByPlayer_3;

	}
}
