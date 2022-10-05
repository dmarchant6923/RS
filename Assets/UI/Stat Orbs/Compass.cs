using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Compass : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Button compassButton;
    Camera cam;
    Transform compassMiddle;

    Action compassAction;
    MenuEntryClick[] menuEntries = new MenuEntryClick[4];

    private void Start()
    {
        compassButton = GetComponent<Button>();
        compassButton.onClick.AddListener(delegate { OnClickNorth(); });
        //transform.GetChild(0).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
        foreach (Image image in GetComponentsInChildren<Image>())
        {
            image.alphaHitTestMinimumThreshold = 0.5f;
        }

        cam = FindObjectOfType<Camera>();
        compassMiddle = transform.GetChild(0);

        compassAction = GetComponent<Action>();
    }

    public void OnClickNorth()
    {
        cam.transform.eulerAngles = Vector3.zero;
    }
    public void OnClickEast()
    {
        cam.transform.eulerAngles = new Vector3(0, 0, 270);
    }
    public void OnClickSouth()
    {
        cam.transform.eulerAngles = new Vector3(0, 0, 180);
    }
    public void OnClickWest()
    {
        cam.transform.eulerAngles = new Vector3(0, 0, 90);
    }

    private void Update()
    {
        compassMiddle.eulerAngles = new Vector3(0, 0, -cam.transform.eulerAngles.z);

        if (RightClickMenu.menuOpen && RightClickMenu.openActions.Contains(compassAction))
        {
            if (menuEntries[0] == null)
            {
                int i = 0;
                foreach (MenuEntryClick entry in RightClickMenu.newMenu.GetComponentsInChildren<MenuEntryClick>())
                {
                    if (entry.action == compassAction)
                    {
                        menuEntries[i] = entry;
                        i++;
                    }
                }
            }
            if (menuEntries[0] != null)
            {
                foreach (MenuEntryClick entry in menuEntries)
                {
                    if (entry.stringNumber == 0)
                    {
                        entry.clickMethod = OnClickNorth;
                    }
                    if (entry.stringNumber == 1)
                    {
                        entry.clickMethod = OnClickEast;
                    }
                    if (entry.stringNumber == 2)
                    {
                        entry.clickMethod = OnClickSouth;
                    }
                    if (entry.stringNumber == 3)
                    {
                        entry.clickMethod = OnClickWest;
                    }
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }
}
