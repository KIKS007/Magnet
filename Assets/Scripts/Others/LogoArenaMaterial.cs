using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class LogoArenaMaterial : MonoBehaviour
{
    public Color color = new Color();

    [ButtonAttribute]
    void SetAllMaterial()
    {
        foreach (var m in FindObjectsOfType<LogoArenaMaterial>())
            m.SetMaterial();
    }

    public void SetMaterial()
    {
        Material m = new Material(GetComponent<Image>().material);

        Texture t = GetComponent<Image>().mainTexture;
        m.SetTexture("_Texture", t);
        m.SetColor("_Color", color);
        GetComponent<Image>().material = m;
    }
}
