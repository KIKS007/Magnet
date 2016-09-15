using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DeathRayMotion : MonoBehaviour 
{
	[Header ("Capsules")]
	public GameObject[] rightCapsules = new GameObject[3];
	public GameObject[] leftCapsules = new GameObject[3];

	[Header ("Positions")]
	public float rightCapsulesNewX;
	public float rightCapsulesNewZ;
	public float leftCapsulesNewX;
	public float leftCapsulesNewZ;

	[Header ("Tween Settings")]
	public float tweenDuration;
	public Ease tweenEase;

	private float distanceBetweenCapsules;

	private BoxCollider boxCollider;

	// Use this for initialization
	void Start () 
	{
		boxCollider = GetComponent<BoxCollider> ();

		StartCoroutine (WaitTillGameBegins ());

		GlobalVariables.Instance.OnPause += () => DOTween.Pause("DeathRay");
		GlobalVariables.Instance.OnPlaying += () => DOTween.Play("DeathRay");
	}

	IEnumerator WaitTillGameBegins ()
	{
		yield return new WaitWhile (() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

		for(int i = 0; i < rightCapsules.Length; i++)
		{
			rightCapsules [i].transform.localPosition = new Vector3 (0, rightCapsules [i].transform.localPosition.y, 0);
			leftCapsules [i].transform.localPosition = new Vector3 (0, leftCapsules [i].transform.localPosition.y, 0);
		}

		for(int i = 0; i < rightCapsules.Length; i++)
		{
			rightCapsules [i].transform.DOLocalMoveX (rightCapsulesNewX, tweenDuration).SetEase (tweenEase).SetId("DeathRay");
			rightCapsules [i].transform.DOLocalMoveZ (rightCapsulesNewZ, tweenDuration).SetEase (tweenEase).SetId("DeathRay");
		}

		for(int i = 0; i < leftCapsules.Length; i++)
		{
			leftCapsules [i].transform.DOLocalMoveX (leftCapsulesNewX, tweenDuration).SetEase (tweenEase).SetId("DeathRay");
			leftCapsules [i].transform.DOLocalMoveZ (leftCapsulesNewZ, tweenDuration).SetEase (tweenEase).SetId("DeathRay");
		}
	}

	void Update ()
	{
		if(GlobalVariables.Instance.GameState == GameStateEnum.Playing)
		{
			distanceBetweenCapsules = Vector3.Distance (rightCapsules [0].transform.position, leftCapsules [0].transform.position);

			Vector3 temp = new Vector3(distanceBetweenCapsules * 0.5f + 0.5f, boxCollider.size.y, boxCollider.size.z);
			boxCollider.size = temp;
		}
	}
}
