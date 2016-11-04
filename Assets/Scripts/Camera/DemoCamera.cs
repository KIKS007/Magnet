using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DemoCamera : MonoBehaviour 
{
	public KeyCode keyToActivate;

	public float rotationSpeed = 1;
	public Vector3 startPosition;
	public KeyCode keySloMo;
	public KeyCode keyCubesSpawn;

	private Transform target;
	private GameObject parent;

	private bool sloMo = false;
	private bool demoEnabled = false;

	// Use this for initialization
	void Start () 
	{
		
	}

	void StartDemo ()
	{
		target = GlobalVariables.Instance.Players[0].GetComponent<Transform> ();

		parent = new GameObject ();
		parent.transform.position = target.position;
		transform.SetParent (parent.transform);
		transform.position = startPosition;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!sloMo && Input.GetKeyDown (keySloMo))
		{
			sloMo = true;
			GetComponent<SlowMotionCamera> ().StartSlowMotion ();
			GetComponent<SlowMotionCamera> ().slowMoNumber = 5;
			
		}
		
		else if (sloMo && Input.GetKeyDown (keySloMo))
		{
			sloMo = false;
			GetComponent<SlowMotionCamera> ().StopSlowMotion ();
			GetComponent<SlowMotionCamera> ().slowMoNumber = 0;
		}


		if(Input.GetKeyDown (keyToActivate) && !demoEnabled)
		{
			GetComponent<DynamicCamera> ().enabled = false;
			StartDemo ();
			demoEnabled = true;
		}

		else if(Input.GetKeyDown (keyToActivate) && demoEnabled)
		{
			transform.rotation = Quaternion.Euler(new Vector3 (90, 0, 0));
			GetComponent<DynamicCamera> ().enabled = true;
			demoEnabled = false;
		}

		if (Input.GetKeyDown (keyCubesSpawn))
			StartCoroutine (SpawnCubes ());
	}

	IEnumerator SpawnCubes ()
	{
		yield return GlobalMethods.Instance.StartCoroutine("RandomPositionMovables", 0.05f);

		yield return new WaitForSeconds (0.85f);

		StartCoroutine (SpawnCubes ());
	}

	public IEnumerator RandomPositionMovables (float durationBetweenSpawn = 0.1f)
	{
		GameObject[] allMovables = GameObject.FindGameObjectsWithTag ("Movable");
		Vector3[] allScales = new Vector3[allMovables.Length];
		LayerMask layer = (1 << 9) | (1 << 12) | (1 << 13) | (1 << 14);

		Tween tween = null;

		for(int i = 0; i < allMovables.Length; i++)
		{
			allScales [i] = allMovables [i].transform.lossyScale;
			allMovables [i].transform.localScale = new Vector3 (0, 0, 0);
		}

		for(int i = 0; i < allMovables.Length; i++)
		{
			Vector3 newPos = new Vector3 ();

			do
			{
				newPos = new Vector3(Random.Range(-20f, 20f), 3, Random.Range(-10f, 10f));
			}
			while(Physics.CheckSphere(newPos, 5, layer));

			yield return new WaitForSeconds (durationBetweenSpawn);

			allMovables [i].gameObject.SetActive (true);
			
			tween = allMovables [i].transform.DOScale (allScales [i], 0.8f).SetEase (Ease.OutElastic);
			
			allMovables [i].transform.position = newPos;
			allMovables [i].transform.rotation = Quaternion.Euler (Vector3.zero);
			allMovables [i].GetComponent<Rigidbody> ().velocity = Vector3.zero;
			allMovables [i].GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
		}

		yield return tween.WaitForCompletion ();
	}

	void FixedUpdate ()
	{
		if(demoEnabled)
		{
			parent.transform.position = target.position;
			
			transform.LookAt (target);
			parent.transform.Rotate(Vector3.up * rotationSpeed);
		}
	}
}
