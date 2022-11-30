using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PresetsPanelButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool save;
    public bool load;
    public bool clear;

    public int presetNumber;

    public PresetsPanel panelScript;
    Action buttonAction;

    Image buttonImage;
    public Sprite onButton;
    public Sprite offButton;
    Text buttonText;

    bool active;

    bool doThisOne = false;

    private void Start()
    {
        buttonAction = GetComponent<Action>();
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<Text>();

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
            GetComponentInChildren<Text>().color = Color.yellow;
        }
        else
        {
            active = false;
            GetComponent<Image>().enabled = false;
            GetComponentInChildren<Text>().color = Color.gray;
        }
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        buttonImage.sprite = onButton;
    }
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        ArrowOff();
    }
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        buttonText.color = Color.white;
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        ArrowOff();
        buttonText.color = Color.yellow;
    }
    void ArrowOff()
    {
        buttonImage.sprite = offButton;
    }
}
