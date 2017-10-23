using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class MenuEnviroColor : MonoBehaviour
{
    public bool useArenaColors = true;

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

        if (!useArenaColors)
        {
            colors[0] = gv.uiMaterial.GetColor("_PURPLECHROMAIdle");
            colors[1] = gv.uiMaterial.GetColor("_BLUECHROMAIdle");
            colors[2] = gv.uiMaterial.GetColor("_GREENCHROMAIdle");
            colors[3] = gv.uiMaterial.GetColor("_ORANGECHROMAIdle");
        }

        text = GetComponent<Text>();

        image = GetComponent<Image>();

        UpdateColor();
    }

    void UpdateColor()
    {
        if (text != null)
        if (!useArenaColors)
            text.color = colors[(int)gv.environementChroma];
        else
            text.color = gv.arenaColors[(int)gv.environementChroma];

        if (image != null)
        if (!useArenaColors)
            image.color = colors[(int)gv.environementChroma];
        else
            image.color = gv.arenaColors[(int)gv.environementChroma];
    }
}
