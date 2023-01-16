using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OpenCloseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Texture buttonOff;
    public Texture buttonOn;
    [HideInInspector] public RawImage image;

    public string actionText;

    [HideInInspector] public Action buttonAction;

    public delegate void closeButton();
    public event closeButton buttonClicked;

    public GameObject objectToClose;

    private void Start()
    {
        image = GetComponent<RawImage>();
        buttonAction = GetComponent<Action>();
        buttonAction.menuTexts[0] = actionText;
        buttonAction.clientAction0 += ClickButton;
    }

    void ClickButton()
    {
        //targetObject.SetActive(!targetObject.activeSelf);
        buttonClicked?.Invoke();
        if (objectToClose != null)
        {
            objectToClose.SetActive(false);
        }
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
        if (image != null)
        {
            image.texture = buttonOff;
        }
    }
}
