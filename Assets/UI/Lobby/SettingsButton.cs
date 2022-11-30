using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public SettingsPanel panelScript;

    Image buttonImage;
    Text buttonText;

    public Sprite onSprite;
    public Sprite offSprite;

    public bool apply = false;

    Action buttonAction;

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        buttonAction = GetComponent<Action>();
        buttonText = GetComponentInChildren<Text>();
    }
    private void Start()
    {
        buttonAction.menuTexts[0] = apply ? "Apply settings" : "Reset to default settings";
        buttonAction.clientAction0 += ButtonClick;
    }

    void ButtonClick()
    {
        if (apply)
        {
            panelScript.ApplySettings();
        }
        else
        {
            panelScript.InitializeSettings(true);
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

    private void OnDisable()
    {
        buttonText.color = Color.yellow;
        ButtonOff();
    }
}
