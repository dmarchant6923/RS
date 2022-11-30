using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectSkillsButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    public bool apply = false;

    Action buttonAction;
    public SelectSkillsPanel panelScript;

    public Sprite onSprite;
    public Sprite offSprite;
    Image buttonImage;

    public Text buttonText;

    private void Start()
    {
        buttonImage = GetComponent<Image>();
        buttonAction = GetComponent<Action>();
        buttonAction.menuTexts[0] = apply ? "Apply changed stats" : "Reset all stats";
        buttonAction.clientAction0 += ButtonClick;
    }

    void ButtonClick()
    {
        if (apply)
        {
            panelScript.ApplySkills();
        }
        else
        {
            panelScript.ResetSkills();
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        buttonText.color = Color.white;
    }
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        buttonImage.sprite = onSprite;
    }
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        ButtonOff();
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        ButtonOff();
        buttonText.color = Color.yellow;
    }
    void ButtonOff()
    {
        buttonImage.sprite = offSprite;
    }
}
