using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OpenCloseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Texture buttonOff;
    public Texture buttonOn;
    public RawImage image;

    public string actionText;

    Action buttonAction;

    public delegate void closeButton();
    public event closeButton buttonClicked;

    private void Start()
    {
        buttonAction = GetComponent<Action>();
        buttonAction.menuTexts[0] = actionText;
        buttonAction.clientAction0 += ClickButton;
    }

    void ClickButton()
    {
        //targetObject.SetActive(!targetObject.activeSelf);
        buttonClicked?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.texture = buttonOn;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        image.texture = buttonOff;
    }

    private void OnDisable()
    {
        image.texture = buttonOff;
    }
}
