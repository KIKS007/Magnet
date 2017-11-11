using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeName : MonoBehaviour
{
    private Text text;

    void Start()
    {
        text = GetComponent<Text>();

        MenuManager.Instance.OnStartModeClick += Set;
        GlobalVariables.Instance.OnStartMode += Set;
        GlobalVariables.Instance.OnRestartMode += Set;

        Set();
    }

    void Set()
    {
        text.text = GlobalVariables.Instance.CurrentModeLoaded.ToString().ToUpper();
    }

    void OnDestroy()
    {
        if (!GlobalVariables.applicationIsQuitting)
        {
            MenuManager.Instance.OnStartModeClick -= Set;
            GlobalVariables.Instance.OnStartMode -= Set;
            GlobalVariables.Instance.OnRestartMode -= Set;
        }
    }
}
