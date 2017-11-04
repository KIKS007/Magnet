using UnityEngine;
using System.Collections;

public class ParticlesMaterial : MonoBehaviour
{
    private Renderer rend;

    // Use this for initialization
    void Start()
    {
        rend = GetComponent<ParticleSystemRenderer>();
        rend.material.SetColor("_EmissionColor", rend.material.color * 1.3f);
    }
    /*	
	// Update is called once per frame
	void Update () 
	{
		rend.material.SetColor("_EmissionColor", rend.material.color);
	}*/
}
