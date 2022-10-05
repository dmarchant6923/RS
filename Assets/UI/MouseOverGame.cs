using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverGame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private void Start()
    {
        MouseManager.isOverGame = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseManager.isOverGame = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseManager.isOverGame = false;
    }
}
