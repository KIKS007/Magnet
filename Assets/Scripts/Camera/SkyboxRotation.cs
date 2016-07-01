using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SkyboxRotation : MonoBehaviour 
{

	public static float xRotStatic = 0;
	public static float yRotStatic = 0;
	public static float zRotStatic = 0;

	public static Vector3 rotationStatic;

	public float xRot;
	public float yRot;
	public float zRot;

	void Start () 
	{
		if(rotationStatic == Vector3.zero)
		{
			transform.rotation = Random.rotation;
		}
		else
		{
			transform.rotation = Quaternion.Euler(rotationStatic);
		}


		if(xRotStatic == 0 && yRotStatic == 0 && zRotStatic == 0)
		{
			if(xRot + yRot + zRot < 0.015f)
			{
				xRot = Random.Range(-0.015f, 0.015f);
				yRot = Random.Range(-0.015f, 0.015f);
				zRot = Random.Range(-0.015f, 0.015f);
			}

			xRotStatic = xRot;
			yRotStatic = yRot;
			zRotStatic = zRot;
		}
		else
		{
			xRot = xRotStatic;
			yRot = yRotStatic;
			zRot = zRotStatic;
		}

	}

	void Update ()
	{
		transform.Rotate(new Vector3(xRot, yRot, zRot));

		rotationStatic = transform.rotation.eulerAngles;
	}
}
