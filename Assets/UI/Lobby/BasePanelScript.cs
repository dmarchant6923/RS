using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanelScript : MonoBehaviour
{
    public OpenCloseButton closeButton;
    public GameObject objectToClose;

    public delegate void Panel();
    public event Panel panelClosed;

    void Start()
    {
        closeButton.buttonClicked += ClosePanel;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
        }
    }

    public void ClosePanel()
    {
        panelClosed?.Invoke();
        objectToClose.SetActive(false);
        if (DialogueBox.dialogueActive)
        {
            DialogueBox.CloseDialogue();
        }
    }
    private void OnDisable()
    {
        Action.cancel1 -= ClosePanel;
    }

    private void OnEnable()
    {
        Action.cancel1 += ClosePanel;
    }

    private void OnDestroy()
    {
        Action.cancel1 -= ClosePanel;
    }
}
