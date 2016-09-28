﻿using UnityEngine;
using System.Collections;

public class ParticlesMaterial : MonoBehaviour 
{
	private Renderer rend;

	// Use this for initialization
	void Start () 
	{
		rend = GetComponent<ParticleSystemRenderer> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		rend.material.SetColor("_EmmissionColor", rend.material.color);
	}
}
