using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TV : MonoBehaviour
{
    Action tvAction;
    public GameObject statsPanel;
    InteractableObject objectScript;

    private void Start()
    {
        tvAction = GetComponent<Action>();
        objectScript = GetComponent<InteractableObject>();

        tvAction.menuTexts[0] = "Hiscores ";
        objectScript.interaction += OpenSettings;
    }

    void OpenSettings()
    {
        statsPanel.SetActive(true);
    }
}
