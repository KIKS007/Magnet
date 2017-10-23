using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class MenuEnviroColor : MonoBehaviour
{
    private Color[] colors = new Color[4];

    private Text text;
    private Image image;
    private GlobalVariables gv;

    // Use this for initialization
    void Start()
    {
        GlobalVariables.Instance.OnEnvironementChromaChange += UpdateColor;

        Setup();
    }

    [ButtonAttribute()]
    void UpdateAll()
    {
        foreach (var s in Resources.FindObjectsOfTypeAll<MenuEnviroColor> ())
            Setup();
    }

    public void Setup()
    {
        gv = FindObjectOfType<GlobalVariables>();

        colors[0] = gv.uiMaterial.GetColor("_PURPLECHROMAIdle");
        colors[1] = gv.uiMaterial.GetColor("_BLUECHROMAIdle");
        colors[2] = gv.uiMaterial.GetColor("_GREENCHROMAIdle");
        colors[3] = gv.uiMaterial.GetColor("_ORANGECHROMAIdle");

        text = GetComponent<Text>();

        image = GetComponent<Image>();

        UpdateColor();
    }

    void UpdateColor()
    {
        if (text != null)
            text.color = colors[(int)gv.environementChroma];

        if (image != null)
            image.color = colors[(int)gv.environementChroma];
    }
}
