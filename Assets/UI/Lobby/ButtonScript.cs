using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    Image buttonImage;
    [HideInInspector] public Text buttonText;

    public Sprite onSprite;
    public Sprite offSprite;
    public string actionText;
    public bool invertTextColor = false;

    Color mouseOffColor;
    Color mouseOnColor;

    [HideInInspector] public Action buttonAction;

    public bool serverClick = false;

    public delegate void Button();
    public event Button buttonClicked;

    bool active = true;

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        buttonAction = GetComponent<Action>();
        buttonText = GetComponentInChildren<Text>();

        if (invertTextColor == false)
        {
            mouseOnColor = Color.white;
            mouseOffColor = Color.yellow;
        }
        else
        {
            mouseOnColor = Color.yellow;
            mouseOffColor = Color.white;
        }
    }

    private void Start()
    {
        if (serverClick == false)
        {
            buttonAction.clientAction0 += ButtonClick;
        }
        else
        {
            buttonAction.serverAction0 += ButtonClick;
        }
        if (string.IsNullOrEmpty(actionText) == false)
        {
            buttonAction.menuTexts[0] = actionText;
        }
        buttonAction.UpdateName();
    }

    void ButtonClick()
    {
        buttonClicked?.Invoke();
    }

    public void ActivateButton(bool newActive)
    {
        active = newActive;
        if (newActive)
        {
            GetComponent<Image>().enabled = true;
            GetComponentInChildren<Text>().color = mouseOffColor;
            ButtonOff();
        }
        else
        {
            GetComponent<Image>().enabled = false;
            GetComponentInChildren<Text>().color = Color.gray;
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (active == false || buttonText == null)
        {
            return;
        }
        buttonText.color = mouseOnColor;
    }
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (active == false)
        {
            return;
        }
        buttonImage.sprite = onSprite;
    }
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        if (active == false)
        {
            return;
        }
        ButtonOff();
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (active == false)
        {
            return;
        }
        ButtonOff();
        if (buttonText == null)
        {
            return;
        }
        buttonText.color = mouseOffColor;
    }
    void ButtonOff()
    {
        if (active == false)
        {
            return;
        }
        buttonImage.sprite = offSprite;
    }

    private void OnDisable()
    {
        if (active == false)
        {
            return;
        }
        ButtonOff();
        if (buttonText == null)
        {
            return;
        }
        buttonText.color = mouseOffColor;
    }
}
