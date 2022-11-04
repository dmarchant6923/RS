using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuEntryClick : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [System.NonSerialized] public int actionNumber = -1;
    [System.NonSerialized] public int stringNumber = -1;
    public Action action;
    public MenuScript menuScript;
    Text text;

    //public delegate void ClickMethod();
    //public ClickMethod clickMethod;

    private void Start()
    {
        text = GetComponentInChildren<Text>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            StartCoroutine(OptionClicked());
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

    IEnumerator OptionClicked()
    {
        yield return null;
        menuScript.OptionClicked(actionNumber, stringNumber);
        if (action != null)
        {
            action.PickAction(stringNumber);
        }
    }
}
