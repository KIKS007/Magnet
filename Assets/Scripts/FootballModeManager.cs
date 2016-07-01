using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XboxCtrlrInput;
using UnityEngine.SceneManagement;


public class FootballModeManager : MonoBehaviour 
{
	public int maxGoalsToWin;

	public Transform playerRightSide;
	public Transform playerRightSide2;
	public Transform playerLeftSide;
	public Transform playerLeftSide2;
	
	public float timeBetweenGoals;
	
	public GameObject ball;

	public Text goalScoreRightSide;
	public Text goalScoreLeftSide;

	public Transform startBallRightSide;
	public Transform startBallLeftSide;

	public GameObject winMenu;
	public Text playerWin1;
	public Text playerWin2;
	public Text winText;
	public Text andText;
	
	private int goalsNumberRightSide;
	private int goalsNumberLeftSide;
	
	private Vector3 startPositionPlayerRightSide;
	private Vector3 startPositionPlayerRightSide2;
	private Vector3 startPositionPlayerLeftSide;
	private Vector3 startPositionPlayerLeftSide2;

	private bool gameEnded = false;
	
	// Use this for initialization
	void Start () 
	{
		goalScoreRightSide.text = goalsNumberRightSide.ToString();
		goalScoreLeftSide.text = goalsNumberLeftSide.ToString();
		
		startPositionPlayerRightSide = playerRightSide.position;

		if(playerRightSide2)
			startPositionPlayerRightSide2 = playerRightSide2.position;

		startPositionPlayerLeftSide = playerLeftSide.position;

		if(playerLeftSide2)
			startPositionPlayerLeftSide2 = playerLeftSide2.position;
	}

	void Update ()
	{
		if(gameEnded == true && XCI.GetButtonDown(XboxButton.A, 1) || gameEnded == true && XCI.GetButtonDown(XboxButton.A, 2) || gameEnded == true && XCI.GetButtonDown(XboxButton.A, 3) || gameEnded == true && XCI.GetButtonDown(XboxButton.A, 4) || gameEnded == true && Input.GetKeyDown(KeyCode.Return))
		{
			SceneManager.LoadScene("Football");
			Time.timeScale = 1f;
		}
	}

	public IEnumerator RightSideGoal ()
	{
		goalsNumberRightSide += 1;
		
		goalScoreRightSide.text = goalsNumberRightSide.ToString();

		yield return new WaitForSeconds(timeBetweenGoals);


		if(goalsNumberRightSide < maxGoalsToWin)
		{

			playerRightSide.position = startPositionPlayerRightSide;
			if(playerRightSide2)
				playerRightSide2.position = startPositionPlayerRightSide2;
			
			playerLeftSide.position = startPositionPlayerLeftSide;
			if(playerLeftSide2)
				playerLeftSide2.position = startPositionPlayerLeftSide2;
			
			ball.transform.position = startBallLeftSide.position;
			ball.transform.eulerAngles = new Quaternion(0, 0, 0, 0).eulerAngles;
			ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
			ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			ball.SetActive(true);
		}

		else 
		{
			gameEnded = true;
			Time.timeScale = 0f;
			winMenu.SetActive(true);

			if(!playerRightSide2)
			{
				winText.rectTransform.anchoredPosition = new Vector3(0, -100, 0);
				winText.text = "wins !";
				andText.text = null;

				playerWin1.rectTransform.anchoredPosition = new Vector3(0, 100, 0);

				playerWin1.text = playerRightSide.name;
				playerWin1.color = playerRightSide.GetComponent<Renderer>().material.color;
				playerWin2.text = null;
			}

			else
			{
				winText.rectTransform.anchoredPosition = new Vector3(0, -200, 0);
				winText.text = "win !";
				andText.text = "and";
				
				playerWin1.rectTransform.anchoredPosition = new Vector3(0, 400, 0);
				playerWin2.rectTransform.anchoredPosition = new Vector3(0, 0, 0);
				
				playerWin1.text = playerRightSide.name;
				playerWin1.color = playerRightSide.GetComponent<Renderer>().material.color;

				playerWin2.text = playerRightSide2.name;
				playerWin2.color = playerRightSide2.GetComponent<Renderer>().material.color;
			}

		}
	}

	public IEnumerator LeftSideGoal ()
	{
		goalsNumberLeftSide += 1;
		
		goalScoreLeftSide.text = goalsNumberLeftSide.ToString();
		
		yield return new WaitForSeconds(timeBetweenGoals);

		if(goalsNumberLeftSide < maxGoalsToWin)
		{
			playerRightSide.position = startPositionPlayerRightSide;
			if(playerRightSide2)
				playerRightSide2.position = startPositionPlayerRightSide2;
			
			playerLeftSide.position = startPositionPlayerLeftSide;
			if(playerLeftSide2)
				playerLeftSide2.position = startPositionPlayerLeftSide2;
			
			ball.transform.position = startBallRightSide.position;
			ball.transform.eulerAngles = new Quaternion(0, 0, 0, 0).eulerAngles;
			ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
			ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			ball.SetActive(true);
		}

		else 
		{
			gameEnded = true;
			Time.timeScale = 0f;
			winMenu.SetActive(true);
			
			if(!playerLeftSide2)
			{
				winText.rectTransform.anchoredPosition = new Vector3(0, -100, 0);
				winText.text = "wins !";
				andText.text = null;

				playerWin1.rectTransform.anchoredPosition = new Vector3(0, 100, 0);
				
				playerWin1.text = playerLeftSide.name;
				playerWin1.color = playerLeftSide.GetComponent<Renderer>().material.color;
				playerWin2.text = null;
			}

			else
			{
				winText.rectTransform.anchoredPosition = new Vector3(0, -200, 0);
				winText.text = "win !";
				andText.text = "and";
				
				playerWin1.rectTransform.anchoredPosition = new Vector3(0, 400, 0);
				playerWin2.rectTransform.anchoredPosition = new Vector3(0, 0, 0);
				
				playerWin1.text = playerLeftSide.name;
				playerWin1.color = playerLeftSide.GetComponent<Renderer>().material.color;
				
				playerWin2.text = playerLeftSide2.name;
				playerWin2.color = playerLeftSide2.GetComponent<Renderer>().material.color;
			}
		}
	}
}
