using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    GraphicRaycaster raycaster;
    PointerEventData eventData;
    EventSystem eventSystem;

    Action UIAction;

    private void Start()
    {
        raycaster = FindObjectOfType<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();
    }

    void Update()
    {
        Vector2 screenPoint = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        RaycastHit2D[] mouseCast = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)), Vector2.zero, 100);

        eventData = new PointerEventData(eventSystem);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);

        bool actionFound = false;
        foreach (RaycastResult result in results)
        {
            GameObject item = result.gameObject;
            while (item.GetComponent<Action>() == null)
            {
                if (item.transform.parent == null || item.transform.parent.tag == "Action Parent")
                {
                    break;
                }
                item = item.transform.parent.gameObject;
            }
            if (item.GetComponent<Action>() != null)
            {
                actionFound = true;
                UIAction = item.GetComponent<Action>();
                if (RightClickMenu.actions.Contains(UIAction) == false)
                {
                    RightClickMenu.actions.Add(UIAction);
                    break;
                }
            }
        }
        if (actionFound == false)
        {
            if (UIAction != null && RightClickMenu.actions.Contains(UIAction))
            {
                RightClickMenu.actions.Remove(UIAction);
            }
            UIAction = null;
        }
    }
}
