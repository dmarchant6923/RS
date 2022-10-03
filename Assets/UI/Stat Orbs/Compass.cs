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

    private void Start()
    {
        compassButton = GetComponent<Button>();
        compassButton.onClick.AddListener(delegate { OnClick(); });
        //transform.GetChild(0).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
        foreach (Image image in GetComponentsInChildren<Image>())
        {
            image.alphaHitTestMinimumThreshold = 0.5f;
        }

        cam = FindObjectOfType<Camera>();
        compassMiddle = transform.GetChild(0);
    }

    void OnClick()
    {
        cam.transform.eulerAngles = Vector3.zero;
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
