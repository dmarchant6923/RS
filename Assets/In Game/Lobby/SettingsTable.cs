using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsTable : MonoBehaviour
{
    Action settingsAction;
    public SettingsPanel settingsPanel;
    InteractableObject objectScript;

    private void Start()
    {
        settingsAction = GetComponent<Action>();
        objectScript = GetComponent<InteractableObject>();

        settingsAction.menuTexts[0] = "Change-settings ";
        objectScript.interaction += OpenSettings;
    }

    void OpenSettings()
    {
        settingsPanel.OpenPanel();
        SettingsPanel.panelOpen = true;
    }
}
