using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsDisableButton : MonoBehaviour
{
    public RawImage disableImage;

    [HideInInspector] public bool active = true;

    public bool sfx;
    public bool music;
    public string actionStr;

    ButtonScript button;
    Action buttonAction;

    public SettingsSlider slider;

    private void Start()
    {
        button = GetComponent<ButtonScript>();
        buttonAction = GetComponent<Action>();
        button.buttonClicked += Toggle;

        SetString();
    }

    public void Toggle()
    {
        active = !active;
        disableImage.enabled = !active;
        SetString();
        slider.Toggle(active);
        SetValue();
    }

    void SetString()
    {
        string verb = "Mute ";
        if (active == false)
        {
            verb = "Unmute ";
        }
        string noun = "Music";
        if (sfx)
        {
            noun = "SFX";
        }

        buttonAction.menuTexts[0] = verb + noun;
    }

    void SetValue()
    {
        string noun = "Music";
        if (sfx)
        {
            noun = "SFX";
        }
        PlayerPrefs.SetInt(noun + " enabled", active ? 1 : 0);
        Debug.Log(PlayerPrefs.GetInt("Music enabled", 1));
    }
}
