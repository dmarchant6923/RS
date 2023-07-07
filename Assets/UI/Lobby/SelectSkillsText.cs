using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectSkillsText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Action textAction;
    public SelectSkillsPanel panelScript;
    public RawImage skillImage;
    string skillText;
    public RawImage highlight;

    bool skillDialogueActive = false;

    public static RawImage currentDialogueActive = null;

    private void Start()
    {
        textAction = GetComponent<Action>();
        skillText = transform.parent.name;

        textAction.menuTexts[0] = "Set " + skillText + " level";
        textAction.clientAction0 += ClickSound;
        textAction.serverAction0 += OpenDialogue;
    }

    void ClickSound()
    {
        PlayerAudio.PlayClip(PlayerAudio.instance.menuClickSound);
    }

    void OpenDialogue()
    {
        if (currentDialogueActive != null)
        {
            if (currentDialogueActive == skillImage)
            {
                return;
            }
            highlight.gameObject.SetActive(false);
            DialogueBox.CancelEdit();
        }
        DialogueBox.PlayerInput("Set " + skillText + " level (1 - 99)");

        skillDialogueActive = true;
        highlight.gameObject.SetActive(true);
        currentDialogueActive = skillImage;

        DialogueBox.InputSubmitted += SetSkillLevel;
    }
    
    void SetSkillLevel()
    {
        DialogueBox.InputSubmitted -= SetSkillLevel;
        if (DialogueBox.InputString != "")
        {
            panelScript.SetSkill(skillImage, int.Parse(DialogueBox.InputString));
        }
        highlight.gameObject.SetActive(false);
        currentDialogueActive = null;
        skillDialogueActive = false;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        highlight.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (skillDialogueActive == false)
        {
            highlight.gameObject.SetActive(false);
        }
    }
}
