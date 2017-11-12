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

    void OnDestroy()
    {
        var p = GetComponent<ParticleSystemRenderer>();

        foreach (var m in p.materials)
            DestroyImmediate(m);

        DestroyImmediate(p.material);
    }

    /*	
	// Update is called once per frame
	void Update () 
	{
		rend.material.SetColor("_EmissionColor", rend.material.color);
	}*/
}
