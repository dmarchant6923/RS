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
        isOverGame = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOverGame = false;
    }
}
