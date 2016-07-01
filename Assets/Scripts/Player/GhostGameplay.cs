using UnityEngine;
using System.Collections;
using XboxCtrlrInput;
using DG.Tweening;

public class GhostGameplay : MonoBehaviour 
{
	public float rotationDuration;

	private Vector3 movement;
	private GameObject rollGameobject;
	
	public bool rolling;
	public bool rollingRight;
	public bool rollingLeft;
	public bool rollingForward;
	public bool rollingBackward;
	public bool rollingForwardRight;
	public bool rollingForwardLeft;
	public bool rollingBackwardRight;
	public bool rollingBackwardLeft;
	
	[HideInInspector]
	public int controllerNumber;
	[HideInInspector]
	public bool mouseControl = false;
	
	public GameObject deadPlayer;
	public Color deadPlayerColor;
	public string deadPlayerName;
	
	// Use this for initialization
	void Start () 
	{
		GetComponent<Renderer>().material.color = deadPlayer.GetComponent<Renderer>().sharedMaterial.color;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Controller ();

		if(mouseControl == true)
		{
			movement = new Vector3(Input.GetAxisRaw("HorizontalPlayer"), 0f, Input.GetAxisRaw("VerticalPlayer"));
			movement = movement.normalized;
		}
		else
		{
			movement = new Vector3(XCI.GetAxisRaw(XboxAxis.LeftStickX, controllerNumber), 0f, XCI.GetAxisRaw(XboxAxis.LeftStickY, controllerNumber));
			movement = movement.normalized;
		}
		
		if(0.39f <= transform.position.y && transform.position.y <= 0.41f)
		{
			if(0.65f < movement.x && movement.x < 1.35f)
			{
				if(movement.z == 0 &&  rolling == false)
					RollRight ();
			}
			
			if(-0.65f > movement.x && movement.x > -1.35f)
			{
				if(movement.z == 0 &&  rolling == false)
					RollLeft ();
			}
			
			if(0.65f < movement.z && movement.z < 1.35f)
			{
				if(movement.x == 0 &&  rolling == false)
					RollForward ();
			}
			
			if(-0.65f > movement.z && movement.z > -1.35f)
			{
				if(movement.x == 0 &&  rolling == false)
					RollBackward ();
			}
			
			
			if(movement.x > 0 && movement.z > 0)
			{
				if(rollingForwardRight == false)
				{
					StartCoroutine("RollForwardRight");
				}
			}
			
			if(movement.x < 0 && movement.z > 0)
			{
				if(rollingForwardLeft == false)
				{
					StartCoroutine("RollForwardLeft"); 
				}
			}
			
			if(movement.x > 0 && movement.z < 0)
			{
				if(rollingBackwardRight == false)
				{
					StartCoroutine("RollBackwardRight"); 
				}
			}
			
			if(movement.x < 0 && movement.z < 0)
			{
				if(rollingBackwardLeft == false)
				{
					StartCoroutine("RollBackwardLeft"); 
				}
			}
			
		}
	}
	
	void Controller ()
	{
		if(controllerNumber == -1)
		{
			gameObject.SetActive(false);
		}
		
		if(controllerNumber == 0)
		{
			mouseControl = true;
		}
	}
	
	void OnCollisionStay (Collision collision)
	{
		if(collision.gameObject.tag == "Floor" && rolling == false && transform.position.y > 0.4f)
		{
			Vector3 v3 = new Vector3(transform.position.x, 0.4f, transform.position.z);
			transform.position = Vector3.Lerp(transform.position, v3, 0.8f);
		}
	}
	
	
	void RollRight ()
	{
		rolling = true;
		rollingRight = true;
		
		if(rollGameobject)
		{
			transform.parent = null;
			Destroy(rollGameobject);
		}
		
		
		rollGameobject = new GameObject();
		rollGameobject.name = "RollGhost";
		Vector3 v3 = new Vector3(transform.position.x + 0.4f, transform.position.y - 0.4f, transform.position.z);
		rollGameobject.transform.position = v3;
		transform.parent = rollGameobject.transform;
		
		rollGameobject.transform.DORotate(new Vector3(0, 0, -90), rotationDuration).OnComplete(RollingFalse).SetEase(Ease.OutCirc);
	}
	
	void RollLeft ()
	{
		rolling = true;
		rollingLeft = true;
		
		if(rollGameobject)
		{
			transform.parent = null;
			Destroy(rollGameobject);
		}
		
		rollGameobject = new GameObject();
		rollGameobject.name = "RollGhost";
		Vector3 v3 = new Vector3(transform.position.x - 0.4f, transform.position.y - 0.4f, transform.position.z);
		rollGameobject.transform.position = v3;
		transform.parent = rollGameobject.transform;
		
		rollGameobject.transform.DORotate(new Vector3(0, 0, 90), rotationDuration).OnComplete(RollingFalse).SetEase(Ease.OutCirc);
	}
	
	void RollForward ()
	{
		rolling = true;
		rollingForward = true;
		
		if(rollGameobject)
		{
			transform.parent = null;
			Destroy(rollGameobject);
		}
		
		
		rollGameobject = new GameObject();
		rollGameobject.name = "RollGhost";
		Vector3 v3 = new Vector3(transform.position.x, transform.position.y - 0.4f, transform.position.z + 0.4f);
		rollGameobject.transform.position = v3;
		transform.parent = rollGameobject.transform;
		
		rollGameobject.transform.DORotate(new Vector3(90, 0, 0), rotationDuration).OnComplete(RollingFalse).SetEase(Ease.OutCirc);
	}
	
	void RollBackward ()
	{
		rolling = true;
		rollingBackward = true;
		
		if(rollGameobject)
		{
			transform.parent = null;
			Destroy(rollGameobject);
		}
		
		rollGameobject = new GameObject();
		rollGameobject.name = "RollGhost";
		Vector3 v3 = new Vector3(transform.position.x, transform.position.y - 0.4f, transform.position.z - 0.4f);
		rollGameobject.transform.position = v3;
		transform.parent = rollGameobject.transform;
		
		rollGameobject.transform.DORotate(new Vector3(-90, 0, 0), rotationDuration).OnComplete(RollingFalse).SetEase(Ease.OutCirc);
	}
	
	IEnumerator RollForwardRight ()
	{
		rollingForwardRight = true;
		
		RollForward ();
		
		yield return new WaitForSeconds(rotationDuration);
		
		RollRight ();
		
		yield return new WaitForSeconds(rotationDuration);
		
		rollingForwardRight = false;	
		
	}
	
	IEnumerator RollForwardLeft ()
	{
		rollingForwardLeft = true;
		
		RollForward ();
		
		yield return new WaitForSeconds(rotationDuration);
		
		RollLeft ();
		
		yield return new WaitForSeconds(rotationDuration);
		
		rollingForwardLeft = false;	
		
	}
	
	IEnumerator RollBackwardRight ()
	{
		rollingBackwardRight = true;
		
		RollBackward ();
		
		yield return new WaitForSeconds(rotationDuration);
		
		RollRight ();
		
		yield return new WaitForSeconds(rotationDuration);
		
		rollingBackwardRight = false;	
		
	}
	
	IEnumerator RollBackwardLeft ()
	{
		rollingBackwardLeft = true;
		
		RollBackward ();
		
		yield return new WaitForSeconds(rotationDuration);
		
		RollLeft ();
		
		yield return new WaitForSeconds(rotationDuration);
		
		rollingBackwardLeft = false;	
		
	}
	
	void RollingFalse ()
	{
		rolling = false;
		rollingRight = false;
		rollingLeft = false;
		rollingForward = false;
		rollingBackward = false;
	}
}


