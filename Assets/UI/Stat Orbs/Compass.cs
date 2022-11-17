using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Compass : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Button compassButton;
    public Camera cam;
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

        compassMiddle = transform.GetChild(0);

        compassAction = GetComponent<Action>();

        compassAction.menuTexts[0] = "Look North";
        compassAction.menuTexts[1] = "Look East";
        compassAction.menuTexts[2] = "Look South";
        compassAction.menuTexts[3] = "Look West";

        compassAction.clientAction0 += OnClickNorth;
        compassAction.clientAction1 += OnClickEast;
        compassAction.clientAction2 += OnClickSouth;
        compassAction.clientAction3 += OnClickWest;
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
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }
}
