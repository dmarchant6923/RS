using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PanelButtonClick : MonoBehaviour, IPointerDownHandler
{
    PanelButtons script;

    void Start()
    {
        script = transform.parent.GetComponent<PanelButtons>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            script.OnClick(transform);
        }
    }
}
