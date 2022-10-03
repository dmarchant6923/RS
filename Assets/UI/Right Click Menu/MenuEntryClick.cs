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

    private void Start()
    {
        text = GetComponentInChildren<Text>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            menuScript.OptionClicked(optionNumber);
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
