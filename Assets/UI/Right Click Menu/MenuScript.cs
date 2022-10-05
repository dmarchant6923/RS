using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuScript : MonoBehaviour, IPointerExitHandler, IPointerClickHandler
{
    public GameObject text;
    GameObject newText;
    float textHeight;
    RectTransform hoverBoxRT;
    RectTransform menuRT;
    int entries = 0;

    Canvas canvas;
    float boxWidth;

    private void Start()
    {
        RightClickMenu.menuOpen = true;
        textHeight = text.GetComponent<RectTransform>().rect.height;
        hoverBoxRT = transform.GetChild(0).GetComponent<RectTransform>();
        menuRT = GetComponent<RectTransform>();
        boxWidth = menuRT.rect.width;

        for (int i = RightClickMenu.openActions.Count - 1; i >= 0; i--)
        {
            for (int j = RightClickMenu.openActions[i].menuTexts.Count - 1; j >= 0; j--)
            {
                hoverBoxRT.sizeDelta += Vector2.up * textHeight;
                menuRT.sizeDelta += Vector2.up * textHeight;
                newText = Instantiate(text, transform);
                newText.GetComponent<RectTransform>().position = text.GetComponent<RectTransform>().position + Vector3.up * textHeight * (entries + 1) * 2;
                newText.GetComponentInChildren<Text>().text = RightClickMenu.openActions[i].menuTexts[j];
                newText.GetComponent<MenuEntryClick>().menuScript = this;
                newText.GetComponent<MenuEntryClick>().actionNumber = i;
                newText.GetComponent<MenuEntryClick>().stringNumber = j;
                newText.GetComponent<MenuEntryClick>().action = RightClickMenu.openActions[i];
                entries++;

                if (newText.GetComponentInChildren<Text>().preferredWidth + 10 > boxWidth)
                {
                    menuRT.sizeDelta += Vector2.right * (newText.GetComponentInChildren<Text>().preferredWidth + 10 - boxWidth);
                    hoverBoxRT.sizeDelta += Vector2.right * (newText.GetComponentInChildren<Text>().preferredWidth + 10 - boxWidth);
                    boxWidth = newText.GetComponentInChildren<Text>().preferredWidth + 10;
                }
            }
        }

        Vector2 cameraSize = new Vector2(Screen.width, Screen.height) - Vector2.one;
        Vector2 RTPosition = menuRT.position;
        Vector2 maxCorner = RTPosition + Vector2.right * menuRT.rect.width;
        Vector2 minCorner = RTPosition - new Vector2(menuRT.rect.width, menuRT.rect.height * 2);

        if (maxCorner.x > cameraSize.x)
        {
            menuRT.position += Vector3.left * (maxCorner.x - cameraSize.x);
        }
        if (maxCorner.y > cameraSize.y)
        {
            menuRT.position += Vector3.down * (maxCorner.y - cameraSize.y);
        }
        if (minCorner.x < 1)
        {
            menuRT.position += Vector3.right * (1 - minCorner.x);
        }
        if (minCorner.y < 1)
        {
            menuRT.position += Vector3.up * (1 - minCorner.y);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(gameObject);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Destroy(gameObject);
    }

    public void OptionClicked(int actionNumber, int stringNumber)
    {
        Debug.Log("action number: " + actionNumber + ". string number: " + stringNumber);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        RightClickMenu.menuOpen = false;
    }
}
