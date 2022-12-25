using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoutButton : MonoBehaviour
{
    OpenCloseButton button;

    public ButtonScript redButton;

    private void Start()
    {
        button = GetComponent<OpenCloseButton>();
        button.buttonClicked += OpenLogoutPanel;
        redButton.buttonClicked += Quit;
    }

    void OpenLogoutPanel()
    {
        PanelButtons.instance.ForceOpen("logout");
    }

    void Quit()
    {
        Application.Quit();
        //UnityEditor.EditorApplication.isPlaying = false;
    }
}
