using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuEntryClick : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [System.NonSerialized] public int actionNumber = -1;
    [System.NonSerialized] public int stringNumber = -1;
    public Action action;
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
            menuScript.OptionClicked(actionNumber, stringNumber);
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
