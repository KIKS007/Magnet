using UnityEngine;
using System.Collections;

public class SlowMotionDetectionScript : MonoBehaviour 
{

	public float rayLength;
	public float delayBeforeSlowMotion;
	public bool detectingSlowMotion = false;
	
	private Transform player;
	
	private RaycastHit objectHit;

	[HideInInspector]
	public Transform holdMovableTransform;
	
	
	// Use this for initialization
	void Start () 
	{
		player = gameObject.transform.parent;
	}

	void Update ()
	{
	
	}
	
	void OnTriggerStay (Collider other)
	{
		if(other.tag == "Player")
		{
			//Debug.Log(other.name);
			
			if(detectingSlowMotion)
			{
				detectingSlowMotion = false;
				
				if(other.GetComponent<PlayersGameplay>().holdingMovable == false && other.GetComponent<PlayersGameplay>().bumped == false)
				{

					//Vector3 playerPosition = new Vector3(player.transform.position.x, player.transform.position.y - 0.95f, player.transform.position.z);
					//Vector3 otherPlayerPosition = new Vector3(other.transform.position.x, other.transform.position.y - 0.95f, other.transform.position.z);

					if(holdMovableTransform != null && Physics.Raycast(holdMovableTransform.transform.position, holdMovableTransform.transform.forward, out objectHit, rayLength))
					{
						Debug.DrawRay(holdMovableTransform.transform.position, holdMovableTransform.transform.forward * 40, Color.blue);
						
						if(objectHit.collider.tag != "Wall")
						{
							if(player.GetComponent<PlayersGameplay>().holdMovableTransform.transform.tag == "ThrownMovable")
								player.GetComponent<PlayersGameplay>().holdMovableTransform.transform.GetChild(0).GetComponent<SlowMotionTriggerScript>().triggerEnabled = true;
							//Debug.Log("triggerEnabled");
						}
					}
				}
			}
			
			
		}	

	}

	public IEnumerator DelaySlowMotion ()
	{
		//Debug.Log("player touched with ray");
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().timeTween = 0.1f;
		
		yield return new WaitForSeconds(delayBeforeSlowMotion);

		//Debug.Log("slomo activated");
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SlowMotionCamera>().slowMotion = true;
	}

}
