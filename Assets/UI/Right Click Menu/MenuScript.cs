using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuScript : MonoBehaviour, IPointerExitHandler
{
    [HideInInspector] public RightClickMenu parentScript;

    public GameObject text;
    GameObject newText;
    float textHeight;
    RectTransform hoverBoxRT;
    RectTransform menuRT;

    public List<string> menuStrings = new List<string>();

    private void Start()
    {
        textHeight = text.GetComponent<RectTransform>().rect.height;
        hoverBoxRT = transform.GetChild(0).GetComponent<RectTransform>();
        menuRT = GetComponent<RectTransform>();
        for (int i = 0; i < menuStrings.Count; i++)
        {
            hoverBoxRT.sizeDelta += Vector2.up * textHeight;
            menuRT.sizeDelta += Vector2.up * textHeight;
            newText = Instantiate(text, transform);
            newText.GetComponent<RectTransform>().position = text.GetComponent<RectTransform>().position + Vector3.up * textHeight * (i + 1) * 2;
            newText.GetComponentInChildren<Text>().text = menuStrings[i];
            newText.GetComponent<MenuEntryClick>().menuScript = this;
            newText.GetComponent<MenuEntryClick>().optionNumber = i + 1;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(gameObject);
    }

    public void OptionClicked(int optionNumber)
    {
        Debug.Log("option number " + optionNumber);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        parentScript.menuOpen = false;
    }
}
