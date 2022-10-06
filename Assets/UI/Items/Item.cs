using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public bool equipment = false;

    Action itemAction;
    MenuEntryClick menuEntry;

    public RawImage itemImage;
    public GameObject mask;
    Shadow shadow;
    Outline outline;

    bool useActive = false;

    private void Start()
    {
        itemAction = GetComponent<Action>();
        itemAction.itemAction = true;
        itemAction.itemName = gameObject.name;
        Debug.Log(itemAction.itemName);
        if (equipment)
        {
            itemAction.menuTexts = new List<string>();
            itemAction.menuTexts.Add("Equip ");
            itemAction.menuTexts.Add("Use ");
            itemAction.menuTexts.Add("Drop ");
            itemAction.menuTexts.Add("Examine ");

            for (int i = 0; i < itemAction.menuTexts.Count; i++)
            {
                itemAction.menuTexts[i] += gameObject.name;
            }
        }
        shadow = itemImage.GetComponent<Shadow>();
        outline = itemImage.GetComponent<Outline>();
    }

    private void Update()
    {
        if (RightClickMenu.menuOpen == false && menuEntry != null)
        {
            menuEntry = null;
        }
        if (RightClickMenu.menuOpen && RightClickMenu.openActions.Contains(itemAction))
        {
            if (menuEntry == null)
            {
                foreach (MenuEntryClick entry in RightClickMenu.newMenu.GetComponentsInChildren<MenuEntryClick>())
                {
                    if (entry.action == itemAction)
                    {
                        menuEntry = entry;
                        break;
                    }
                }
            }
            if (menuEntry != null)
            {
                menuEntry.clickMethod = Use;
            }
        }
    }

    void Use()
    {
        useActive = !useActive;
        if (useActive)
        {
            mask.SetActive(true);
            shadow.enabled = false;
            outline.enabled = false;
        }
        else
        {
            mask.SetActive(false);
            shadow.enabled = true;
            outline.enabled = true;
        }
    }
}
