using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresetsPanelButton : MonoBehaviour
{
    public bool save;
    public bool load;
    public bool clear;

    public int presetNumber;

    public PresetsPanel panelScript;
    Action buttonAction;

    bool active;

    bool doThisOne = false;

    private void Start()
    {
        buttonAction = GetComponent<Action>();

        if (save)
        {
            buttonAction.menuTexts[0] = "Save Preset " + presetNumber;
        }
        else if (load)
        {
            buttonAction.menuTexts[0] = "Load Preset " + presetNumber;
        }
        else if (clear)
        {
            buttonAction.menuTexts[0] = "Clear Preset " + presetNumber;
        }

        buttonAction.clientAction0 += ButtonClick;
        buttonAction.serverAction0 += PresetAction;
        buttonAction.menuPriorities[0] = 1;
    }
    
    void ButtonClick()
    {
        if (panelScript.IgnoreActions == false)
        {
            doThisOne = true;
        }
        panelScript.IgnoreActions = true;
    }

    void PresetAction()
    {
        if (panelScript.IgnoreActions && doThisOne == false)
        {
            return;
        }

        if (save)
        {
            panelScript.SavePreset(presetNumber - 1);
        }
        if (load && active)
        {
            panelScript.LoadPreset(presetNumber - 1);
        }
        if (clear && active)
        {
            panelScript.ClearPreset(presetNumber - 1);
        }

        doThisOne = false;
    }

    public void SetActive(bool newActive)
    {
        if (newActive)
        {
            active = true;
            GetComponent<Image>().enabled = true;
            GetComponentInChildren<Text>().color = new Color(1, 0.5f, 0);
        }
        else
        {
            active = false;
            GetComponent<Image>().enabled = false;
            GetComponentInChildren<Text>().color = Color.gray;
        }
    }
}
