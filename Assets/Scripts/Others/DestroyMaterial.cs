using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMaterial : MonoBehaviour
{
    void OnDestroy()
    {
        DestroyImmediate(GetComponent<Renderer>().material);
    }
}
