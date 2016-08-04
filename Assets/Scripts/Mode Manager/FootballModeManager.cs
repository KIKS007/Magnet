using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XboxCtrlrInput;
using UnityEngine.SceneManagement;


public class FootballModeManager : MonoBehaviour 
{
	[Header ("Football Settings")]
	public int goalsToWin = 5;
	public float timeBetweenGoals = 1;

	[Header ("Teams and Balls Positions")]
	public Transform[] team1Positions = new Transform[3];
	public Transform[] team2Positions = new Transform[3];
	public Transform[] ballsPositions = new Transform[3];

	[Header ("Balls")]
	public GameObject[] balls;

	[Header ("Goals Text")]
	public Text goalScoreRightSide;
	public Text goalScoreLeftSide;

	public float timeBeforeEndGame = 2;

	public LayerMask sphereLayer;


	private int goalsNumberRightSide;
	private int goalsNumberLeftSide;


	// Use this for initialization
	void Start () 
	{
		balls = GameObject.FindGameObjectsWithTag ("Movable");

		goalScoreRightSide.text = goalsNumberRightSide.ToString();
		goalScoreLeftSide.text = goalsNumberLeftSide.ToString();

		for(int i = 0; i < balls.Length; i++)
		{
			balls[i].transform.position = ballsPositions[i].position;

			balls[i].transform.eulerAngles = new Quaternion(0, 0, 0, 0).eulerAngles;
			balls[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
			balls[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			balls[i].SetActive(true);
		}
	}

	void OnEnable ()
	{
		ChooseTeamScript.OnTeamChange += FindPlayersPosition;
	}

	void OnDisable ()
	{
		ChooseTeamScript.OnTeamChange -= FindPlayersPosition;
	}

	void Update ()
	{
		
	}

	void FindPlayersPosition ()
	{
		GlobalMethods.Instance.SetPlayersPositions2Team (team1Positions, team2Positions);
	}

	public void GoalScoreVoid (int whichGoal, GameObject ball)
	{
		StartCoroutine (GoalScore (whichGoal, ball));
	}

	IEnumerator GoalScore (int whichGoal, GameObject ball)
	{
		if(whichGoal == 1)
		{
			goalsNumberLeftSide += 1;
			goalScoreLeftSide.text = goalsNumberLeftSide.ToString();
		}

		if( whichGoal == 2)
		{
			goalsNumberRightSide += 1;
			goalScoreRightSide.text = goalsNumberRightSide.ToString();
		}


		if(goalsNumberRightSide < goalsToWin)
		{
			yield return new WaitForSeconds(timeBetweenGoals);

			ResetBall (ball);
		}

		else 
		{
			StartCoroutine (GameEnded ());
		}

		yield return null;
	}

	void ResetBall (GameObject ball)
	{
		Vector3 newPos = new Vector3 ();

		do
		{
			newPos = ballsPositions [Random.Range (0, ballsPositions.Length)].position;
		}
		while(Physics.CheckSphere(newPos, 3, sphereLayer));


		ball.transform.position = newPos;
		ball.transform.eulerAngles = new Quaternion(0, 0, 0, 0).eulerAngles;
		ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
		ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

		ball.SetActive(true);
		Instantiate (GlobalVariables.Instance.MovableExplosion, newPos, GlobalVariables.Instance.MovableExplosion.transform.rotation);
	}


	IEnumerator GameEnded ()
	{
		GlobalVariables.Instance.GameOver = true;
		GlobalVariables.Instance.GamePaused = true;

		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().StartPauseSlowMotion();
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScreenShake>().CameraShaking();

		yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(timeBeforeEndGame));

		GameObject.FindGameObjectWithTag("MainMenuManager").GetComponent<MainMenuManagerScript>().GameOverMenuVoid ();
	}
}
