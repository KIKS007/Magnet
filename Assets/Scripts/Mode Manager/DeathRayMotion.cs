using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DeathRayMotion : MonoBehaviour 
{
	[Header ("Capsules")]
	public GameObject[] rightCapsules = new GameObject[3];
	public GameObject[] leftCapsules = new GameObject[3];

	[Header ("Collider")]
	public CapsuleCollider[] colliders = new CapsuleCollider[3];

	[Header ("Positions")]
	public float rightCapsulesNewX;
	//public float rightCapsulesNewZ;
	public float leftCapsulesNewX;
	//public float leftCapsulesNewZ;

	[Header ("Tween Settings")]
	public float tweenDuration;
	public Ease tweenEase;
	public float tweenDelay = 0;

	private float distanceBetweenCapsules;

	public float x;

	// Use this for initialization
	void Start () 
	{
		StartCoroutine (WaitTillGameBegins ());

		GlobalVariables.Instance.OnPause += () => DOTween.Pause("DeathRay");
		GlobalVariables.Instance.OnResume += () => DOTween.Play("DeathRay");

		SetupColliders ();
	}

	void SetupColliders ()
	{
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders [i] = gameObject.AddComponent<CapsuleCollider> ();

			//colliders [i].radius = 1;
			Vector3 center = Vector3.zero;
			colliders [i].direction = 0;

			switch(i)
			{
			case 0:
				center.y = -1.8f;
				break;
			case 1:
				center.y = 0;
				break;
			case 2:
				center.y = 1.8f;
				break;
			}

			colliders [i].center = center;
		}
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
			rightCapsules [i].transform.DOLocalMoveX (rightCapsulesNewX, tweenDuration).SetEase (tweenEase).SetId("DeathRay").SetDelay (tweenDelay);
			//rightCapsules [i].transform.DOLocalMoveZ (rightCapsulesNewZ, tweenDuration).SetEase (tweenEase).SetId("DeathRay");
		}

		for(int i = 0; i < leftCapsules.Length; i++)
		{
			leftCapsules [i].transform.DOLocalMoveX (leftCapsulesNewX, tweenDuration).SetEase (tweenEase).SetId("DeathRay").SetDelay (tweenDelay);
			//leftCapsules [i].transform.DOLocalMoveZ (leftCapsulesNewZ, tweenDuration).SetEase (tweenEase).SetId("DeathRay");
		}
	}

	void Update ()
	{
		if(DOTween.IsTweening ("DeathRay"))
		{
			float height = Vector3.Distance (rightCapsules [0].transform.position, leftCapsules [0].transform.position) + 1;
			float xCenter = (rightCapsules [0].transform.localPosition + leftCapsules [0].transform.localPosition).x ;
			
			x = xCenter;
			
			for (int i = 0; i < colliders.Length; i++)
			{
				colliders [i].height = height;
				
				Vector3 center = colliders [i].center;
				center.x = xCenter;
				
				colliders [i].center = center;
			}			
		}
	}
}
