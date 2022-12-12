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
    [HideInInspector] public float rtScale;

    private void Start()
    {
        RightClickMenu.menuOpen = true;
        canvas = FindObjectOfType<Canvas>();
        textHeight = text.GetComponent<RectTransform>().rect.height;
        hoverBoxRT = transform.GetChild(0).GetComponent<RectTransform>();
        menuRT = GetComponent<RectTransform>();
        boxWidth = menuRT.rect.width;
        rtScale = transform.localScale.x * canvas.scaleFactor;
        transform.localScale = Vector3.one * rtScale;

        if (RightClickMenu.isUsingItem == false && RightClickMenu.isCastingSpell == false)
        {
            for (int priority = -1; priority < 2; priority++)
            {
                for (int i = 0; i < RightClickMenu.openActions.Count; i++)
                //for (int i = RightClickMenu.openActions.Count - 1; i >= 0; i--)
                {
                    for (int j = RightClickMenu.openActions[i].menuTexts.Length - 1; j >= 0; j--)
                    {
                        if (string.IsNullOrEmpty(RightClickMenu.openActions[i].menuTexts[j]) || RightClickMenu.openActions[i].menuPriorities[j] != priority)
                        {
                            continue;
                        }
                        if (RightClickMenu.isUsingItem == false && RightClickMenu.isCastingSpell == false)
                        {
                            hoverBoxRT.sizeDelta += Vector2.up * textHeight;
                            menuRT.sizeDelta += Vector2.up * textHeight;
                            newText = Instantiate(text, transform);
                            newText.GetComponent<RectTransform>().position = text.GetComponent<RectTransform>().position + Vector3.up * textHeight * (entries + 1) * rtScale * canvas.scaleFactor;
                            newText.GetComponentInChildren<Text>().text = RightClickMenu.openActions[i].menuTexts[j];
                            newText.GetComponent<MenuEntryClick>().menuScript = this;
                            newText.GetComponent<MenuEntryClick>().actionNumber = i;
                            newText.GetComponent<MenuEntryClick>().stringNumber = j;
                            newText.GetComponent<MenuEntryClick>().action = RightClickMenu.openActions[i];
                            entries++;
                        }

                        if (newText.GetComponentInChildren<Text>().preferredWidth + 10 > boxWidth)
                        {
                            menuRT.sizeDelta += Vector2.right * (newText.GetComponentInChildren<Text>().preferredWidth + 10 - boxWidth);
                            hoverBoxRT.sizeDelta += Vector2.right * (newText.GetComponentInChildren<Text>().preferredWidth + 10 - boxWidth);
                            boxWidth = newText.GetComponentInChildren<Text>().preferredWidth + 10;
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = RightClickMenu.openActions.Count - 1; i >= 0; i--)
            {
                if (RightClickMenu.openActions[i].ignoreUse == false)
                {
                    if (RightClickMenu.isUsingItem && RightClickMenu.openActions[i] == RightClickMenu.itemBeingUsed.GetComponent<Action>())
                    {
                        continue;
                    }
                    if (RightClickMenu.isCastingSpell && RightClickMenu.openActions[i].gameObject.GetComponent<NPC>() == null)
                    {
                        continue;
                    }
                    hoverBoxRT.sizeDelta += Vector2.up * textHeight;
                    menuRT.sizeDelta += Vector2.up * textHeight;
                    newText = Instantiate(text, transform);
                    newText.GetComponent<RectTransform>().position = text.GetComponent<RectTransform>().position + Vector3.up * textHeight * (entries + 1) * rtScale;
                    newText.GetComponentInChildren<Text>().text = RightClickMenu.usingItemString + RightClickMenu.openActions[i].gameObject.name;
                    if (RightClickMenu.isCastingSpell)
                    {
                        newText.GetComponentInChildren<Text>().text = RightClickMenu.castingSpellString + RightClickMenu.openActions[i].gameObject.name;
                    }
                    newText.GetComponent<MenuEntryClick>().menuScript = this;
                    newText.GetComponent<MenuEntryClick>().actionNumber = i;
                    newText.GetComponent<MenuEntryClick>().stringNumber = 9;
                    newText.GetComponent<MenuEntryClick>().action = RightClickMenu.openActions[i];
                    entries++;
                }

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
        Vector2 maxCorner = RTPosition + Vector2.right * canvas.scaleFactor * menuRT.rect.width / 2;
        Vector2 minCorner = RTPosition - new Vector2(menuRT.rect.width / 2, menuRT.rect.height) * canvas.scaleFactor;

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
        //Debug.Log("action number: " + actionNumber + ". string number: " + stringNumber);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        RightClickMenu.menuOpen = false;
    }
}
