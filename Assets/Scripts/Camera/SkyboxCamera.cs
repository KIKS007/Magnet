using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class SkyboxCamera : MonoBehaviour 
{
	public Vector3 direction;
	[MinMaxSliderAttribute (0, 10)]
	public Vector2 speed;

	// Use this for initialization
	void Start () 
	{
		Vector3 d = new Vector3 (Mathf.Sign (Random.Range (-1f, 1f)) * direction.x, Mathf.Sign (Random.Range (-1f, 1f)) * direction.y, Mathf.Sign (Random.Range (-1f, 1f)) * direction.z);

		transform.DOLocalRotate (d, Random.Range (speed.x, speed.y)).SetSpeedBased ().SetLoops (-1, LoopType.Incremental).SetRelative ();
	}
}
