using UnityEngine;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;

public class DemoCamera : MonoBehaviour
{
    public bool demoEnabled = false;

    [Header("Settings")]
    public float rotationSpeed = 1;
    public bool followTarget = false;
    public bool lookAt = false;

    [Header("Rotate Around Point")]
    public Vector3 rotationPoint;
    public Transform lookAtTarget = null;

    [Header("Rotate Around Player")]
    public PlayerName playerTarget;
    public Vector3 startPosition;

    [Header("Follow Player")]
    public float positionLerp = 0.1f;

    private Transform target;
    private GameObject parent;

    private bool sloMo = false;
    private Vector3 startPositionTemp;

    // Use this for initialization
    void Start()
    {
		
    }

    void StartDemo()
    {
        
    }
	
    // Update is called once per frame
    void Update()
    {
        if (demoEnabled && startPosition != startPositionTemp && target)
        {
            startPositionTemp = startPosition;
            transform.position = target.position + startPosition;
        }
    }


    [ButtonAttribute]
    void EnableDemo()
    {
        if (parent != null)
        {
            transform.SetParent(null);

            Destroy(parent);
        }

        if (!demoEnabled)
        {
            GlobalVariables.Instance.demoEnabled = true;
            demoEnabled = true;
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
            demoEnabled = false;
            GlobalVariables.Instance.demoEnabled = false;
        }
    }

    [ButtonAttribute]
    void FollowPlayer()
    {
        if (parent != null)
        {
            transform.SetParent(null);

            Destroy(parent);
        }

        if (GlobalVariables.Instance.Players[(int)playerTarget] != null)
            target = GlobalVariables.Instance.Players[(int)playerTarget].GetComponent<Transform>();
        else
            target = GlobalVariables.Instance.Players[0].GetComponent<Transform>();

        parent = new GameObject();
        parent.transform.position = target.position;

        transform.SetParent(parent.transform);
        transform.position = target.position + startPosition;

        startPositionTemp = startPosition;
    }

    [ButtonAttribute]
    void RotateAroundPoint()
    {
        if (parent != null)
        {
            transform.SetParent(null);

            Destroy(parent);
        }

        parent = new GameObject();
        parent.transform.position = rotationPoint;

        transform.SetParent(parent.transform);

        startPositionTemp = startPosition;

        target = lookAtTarget;
    }

    [ButtonAttribute]
    void SlowMo()
    {
        if (!sloMo)
        {
            sloMo = true;
            GetComponent<SlowMotionCamera>().StartSlowMotion();
            GetComponent<SlowMotionCamera>().slowMoNumber = 5;

        }
        else
        {
            sloMo = false;
            GetComponent<SlowMotionCamera>().StopSlowMotion();
            GetComponent<SlowMotionCamera>().slowMoNumber = 0;
        }
    }

    void FixedUpdate()
    {
        if (demoEnabled)
        {
            if (target && followTarget)
                parent.transform.position = Vector3.Lerp(parent.transform.position, target.position, positionLerp);

            if (target && lookAt)
                transform.LookAt(target);

            if (target == null && lookAt)
                transform.LookAt(rotationPoint);

            if (parent)
                parent.transform.Rotate(Vector3.up * rotationSpeed);
        }
    }

    [ButtonAttribute("Spawn Cubes")]
    void SpawnCubesVoid()
    {
        StartCoroutine(SpawnCubes());
    }

    IEnumerator SpawnCubes()
    {
        yield return StartCoroutine(GlobalMethods.Instance.RandomPositionMovables());

        yield return new WaitForSeconds(0.85f);

        StartCoroutine(SpawnCubes());
    }

    public IEnumerator RandomPositionMovables(float durationBetweenSpawn = 0.1f)
    {
        GameObject[] allMovables = GameObject.FindGameObjectsWithTag("Movable");
        Vector3[] allScales = new Vector3[allMovables.Length];
        LayerMask layer = (1 << 9) | (1 << 12) | (1 << 13) | (1 << 14);

        Tween tween = null;

        for (int i = 0; i < allMovables.Length; i++)
        {
            allScales[i] = allMovables[i].transform.lossyScale;
            allMovables[i].transform.localScale = new Vector3(0, 0, 0);
        }

        for (int i = 0; i < allMovables.Length; i++)
        {
            Vector3 newPos = new Vector3();

            do
            {
                newPos = new Vector3(Random.Range(-20f, 20f), 3, Random.Range(-10f, 10f));
            }
            while(Physics.CheckSphere(newPos, 5, layer));

            yield return new WaitForSeconds(durationBetweenSpawn);

            allMovables[i].gameObject.SetActive(true);
			
            tween = allMovables[i].transform.DOScale(allScales[i], 0.8f).SetEase(Ease.OutElastic);
			
            allMovables[i].transform.position = newPos;
            allMovables[i].transform.rotation = Quaternion.Euler(Vector3.zero);
            allMovables[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            allMovables[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }

        yield return tween.WaitForCompletion();
    }


}
