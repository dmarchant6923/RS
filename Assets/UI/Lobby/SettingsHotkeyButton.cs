using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsHotkeyButton : MonoBehaviour
{
    public int buttonNumber;

    ButtonScript button;
    public SettingsPanel settings;

    [HideInInspector] public int currentHotkey = -1;
    string buttonText;

    bool listening = false;
    float listenWindow = 3;
    float listenTimer = 0;

    private void Start()
    {
        button = GetComponent<ButtonScript>();

        button.buttonClicked += Listen;

        currentHotkey = settings.settings.hotkeys[buttonNumber];
        SetHotkey(currentHotkey);
    }

    void Listen()
    {
        listening = true;
        listenTimer = listenWindow;
        button.ActivateButton(false);
        button.buttonText.text = "Listening...";
        button.buttonText.fontSize = 30;
    }

    void CancelListen()
    {
        listening = false;
        listenTimer = 0;
        button.ActivateButton(true);
        SetButtonText();
    }

    public void SetHotkey(int key)
    {
        currentHotkey = key;
        SetButtonText();
        CancelListen();
        foreach (SettingsHotkeyButton button in settings.hotkeyButtons)
        {
            if (key != -1 && button != this && button.currentHotkey == key)
            {
                button.SetHotkey(-1);
            }
        }
    }

    void SetButtonText()
    {
        if (currentHotkey == -1)
        {
            buttonText = "Listen";
            button.buttonText.fontSize = 35;
        }
        else
        {
            buttonText = currentHotkey.ToString();
            button.buttonText.fontSize = 55;
        }
        button.buttonText.text = buttonText;
    }

    private void Update()
    {
        if (listening)
        {
            if (Input.anyKeyDown && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)) == false)
            {
                bool validKey = false;
                for (int i = 0; i < 10; i++)
                {
                    if (Input.GetKeyDown(i.ToString()))
                    {
                        validKey = true;
                        SetHotkey(i);
                    }
                }

                if (validKey == false)
                {
                    SetHotkey(-1);
                }
            }


            if (listenTimer > 0)
            {
                listenTimer -= Time.deltaTime;
            }
            else
            {
                CancelListen();
            }
        }
    }

    private void OnDisable()
    {
        currentHotkey = settings.settings.hotkeys[buttonNumber];
        CancelListen();
    }
}
