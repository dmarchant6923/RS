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

    public GameObject panel;
    public RectTransform statPanel;
    float panelOpenPos;

    private void Start()
    {
        raycaster = FindObjectOfType<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();

        panelOpenPos = statPanel.position.x;
    }

    void Update()
    {
        Vector2 screenPoint = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        RaycastHit2D[] mouseCast = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)), Vector2.zero, 100);

        eventData = new PointerEventData(eventSystem);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);

        UIAction = null;
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
                UIAction = item.GetComponent<Action>();
                break;
            }
        }

        RightClickMenu.UIAction = UIAction;




        if (panel.activeSelf && statPanel.position.x != panelOpenPos)
        {
            statPanel.position = new Vector2(panelOpenPos, statPanel.position.y);
        }
        if (panel.activeSelf == false && statPanel.position.x == panelOpenPos)
        {
            statPanel.position = new Vector2(1920, statPanel.position.y);
        }
    }
}
