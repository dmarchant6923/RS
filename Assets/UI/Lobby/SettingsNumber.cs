using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsNumber : MonoBehaviour
{
    Action textAction;
    public string actionString;
    public float max;
    public float min;

    bool dialogueActive = false;

    public GameObject highlight;

    public Text text;

    public float value;

    private void Start()
    {
        textAction = GetComponent<Action>();
        textAction.menuTexts[0] = "Set " + actionString;
        textAction.serverAction0 += OpenDialogue;
    }

    void OpenDialogue()
    {
        DialogueBox.PlayerInput("Set " + actionString + " (" + min + " - " + max + ")");

        dialogueActive = true;
        highlight.SetActive(true);

        DialogueBox.InputSubmitted += SetValue;
    }

    void SetValue()
    {
        DialogueBox.InputSubmitted -= SetValue;
        if (DialogueBox.InputString != "")
        {
            SetValue(int.Parse(DialogueBox.InputString));
        }
        else
        {
            SetValue(value);
        }
    }

    public void SetValue(float number)
    {
        value = number;
        value = Mathf.Floor(Mathf.Clamp(value, min, max));
        text.text = value.ToString();
        highlight.gameObject.SetActive(false);
        dialogueActive = false;
    }
}
