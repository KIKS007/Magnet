using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DeathRayScript : MonoBehaviour 
{
	public Transform point1;
	public Transform point2;

	private LineRenderer line;

	// Use this for initialization
	void Start () 
	{
		line = GetComponent<LineRenderer> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		line.SetPosition (0, point1.position);
		line.SetPosition (1, point2.position);
	}
}
