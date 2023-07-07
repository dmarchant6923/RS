using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogoutButton : MonoBehaviour
{
    OpenCloseButton button;

    public ButtonScript redButton;
    Image redButtonImage;
    Text redButtonText;

    public static LogoutButton instance;

    bool active = false;


    private void Start()
    {
        instance = this;
        button = GetComponent<OpenCloseButton>();
        button.buttonClicked += OpenLogoutPanel;
        redButton.buttonClicked += Quit;
        redButtonImage = redButton.GetComponent<Image>();
        redButtonText = redButton.GetComponentInChildren<Text>();
    }

    public void Toggle(bool _active)
    {
        active = _active;
        Color color = Color.gray;
        if (active) { color = Color.white; }
        redButtonImage.enabled = active;
        redButtonText.color = color;
    }

    void OpenLogoutPanel()
    {
        PanelButtons.instance.ForceOpen("logout");
    }

    void Quit()
    {
        if (active == false)
        {
            return;
        }
        InfernoManager.instance.ReturnToLobby();
        //UnityEditor.EditorApplication.isPlaying = false;
    }
}
