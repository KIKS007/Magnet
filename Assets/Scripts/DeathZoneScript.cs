using UnityEngine;
using System.Collections;

public class DeathZoneScript : MonoBehaviour 
{
	void OnTriggerEnter (Collider other)
	{

		for(int i = 0; i < other.transform.childCount; i++)
		{
			if(other.transform.GetChild(i).tag == "Movable" || other.transform.GetChild(i).tag == "HoldMovable")
			{
				other.transform.GetChild(i).transform.SetParent(null);
			}
		}


		Destroy (other.gameObject);
	}



	void OnCollisionEnter (Collision other)
	{
		for(int i = 0; i < other.transform.childCount; i++)
		{
			if(other.transform.GetChild(i).tag == "Movable" || other.transform.GetChild(i).tag == "HoldMovable")
			{
				other.transform.GetChild(i).transform.SetParent(null);
			}
		}

		Destroy (other.gameObject);
	}
}
