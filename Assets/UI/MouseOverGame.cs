using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverGame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static bool isOverGame;

    private void Start()
    {
        isOverGame = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseManager.actions.Add("Walk here");
        isOverGame = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseManager.actions.Remove("Walk here");
        isOverGame = false;
    }
}
