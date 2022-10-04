using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuEntryClick : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int optionNumber = 0;
    public MenuScript menuScript;
    Text text;

    public delegate void ClickMethod();
    public ClickMethod clickMethod;


    private void Start()
    {
        text = GetComponentInChildren<Text>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            menuScript.OptionClicked(optionNumber);
            if (clickMethod != null)
            {
                clickMethod();
            }
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = Color.yellow;

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = Color.white;
    }
}
