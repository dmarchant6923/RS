using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Button button;

    public delegate void MouseAction();
    public static event MouseAction InGameMouseDown;

    Camera cam;
    public static bool mouseOnScreen;

    public class Action
    {

    }
    [HideInInspector] public static List<string> actions = new List<string>();

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(delegate { GameClick(); });

        cam = FindObjectOfType<Camera>();
        mouseOnScreen = false;
    }

    void GameClick()
    {
        if (InGameMouseDown != null)
        {
            InGameMouseDown();
            if (Input.GetMouseButtonDown(2))
            {
                Debug.Log("you are here");
            }
        }
    }

    private void Update()
    {
        Vector2 screenPoint = cam.ScreenToViewportPoint(Input.mousePosition);
        if (screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1)
        {
            mouseOnScreen = false;
            return;
        }
        mouseOnScreen = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        actions.Add("Walk here");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        actions.Remove("Walk here");
    }
}
