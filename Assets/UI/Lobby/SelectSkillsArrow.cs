using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectSkillsArrow : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public RawImage skillImage;

    public bool up;

    Action arrowAction;
    public SelectSkillsPanel panelScript;

    public Texture2D upArrowOn;
    public Texture2D upArrowOff;
    public Texture2D downArrowOn;
    public Texture2D downArrowOff;
    RawImage arrowImage;

    private void Start()
    {
        arrowAction = GetComponent<Action>();
        string skillText = transform.parent.name;
        arrowAction.menuTexts[0] = "Increase " + skillText + " level";
        if (up == false)
        {
            arrowAction.menuTexts[0] = "Decrease " + skillText + " level";
        }
        arrowAction.clientAction0 += Click;
        arrowImage = GetComponent<RawImage>();
    }

    void Click()
    {
        panelScript.IncrementSkill(skillImage, up);
        PlayerAudio.PlayClip(PlayerAudio.instance.menuClickSound);
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (up)
        {
            arrowImage.texture = upArrowOn;
        }
        else
        {
            arrowImage.texture = downArrowOn;
        }
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        ArrowOff();
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        ArrowOff();
    }
    void ArrowOff()
    {
        if (up)
        {
            arrowImage.texture = upArrowOff;
        }
        else
        {
            arrowImage.texture = downArrowOff;
        }
    }
}
