using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public RectTransform panel;
    public GameObject unsortedItems;
    float panelScale;

    Item[] items;
    RectTransform[] itemRTs;

    public GameObject inventoryParent;
    public GameObject inventorySlot;
    GameObject newInventorySlot;
    [HideInInspector] public static GameObject[] inventorySlots = new GameObject[28];

    public static int slotsTaken;

    [HideInInspector] public List<Equipment> equipQueue = new List<Equipment>();

    public delegate void EquipAction();
    public static event EquipAction UpdateEquippedItems;
    public static event EquipAction ReadEquippedItems;

    private void Start()
    {
        items = new Item[28];
        itemRTs = new RectTransform[28];

        foreach (Item item in unsortedItems.GetComponentsInChildren<Item>())
        {
            for (int i = 0; i < 28; i++)
            {
                if (items[i] == null)
                {
                    items[i] = item;
                    itemRTs[i] = item.GetComponent<RectTransform>();
                    break;
                }
            }
        }

        panelScale = panel.transform.localScale.x;

        Vector2 panelAnchor = panel.position;
        float panelWidth = panel.rect.width * panelScale;
        float panelHeight = panel.rect.height * panelScale;
        Vector2 center = panelAnchor + new Vector2(-panelWidth / 2, panelHeight / 2);
        float widthIncrement = panelWidth * 0.9f / 4;
        float heightIncrement = panelHeight * 0.9f / 7;
        Vector2 br = center += new Vector2(widthIncrement * 1.5f, -heightIncrement * 3);

        for (int j = 0; j < 7; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                int index = (j * 4) + (i % 4);
                float height = br.y + (6 - j) * heightIncrement;
                float width = br.x - (3 - i) * widthIncrement;
                Vector2 position = new Vector2(width, height);
                newInventorySlot = Instantiate(inventorySlot, inventoryParent.transform);
                newInventorySlot.GetComponent<RectTransform>().position = position;
                inventorySlots[27 - index] = newInventorySlot;
            }
        }

        TickManager.onTick += CountSlots;
        //TickManager.beforeTick += EquipQueue;
        SortInventory();
    }

    public void SortInventory()
    {
        foreach (Item item in unsortedItems.GetComponentsInChildren<Item>())
        {
            for (int i = 0; i < 28; i++)
            {
                if (inventorySlots[i].GetComponentInChildren<Item>() == null)
                {
                    item.transform.SetParent(inventorySlots[i].transform);
                    item.transform.position = item.transform.parent.position;
                }
            }
        }

        foreach (GameObject slot in inventorySlots)
        {
            if (slot.GetComponentInChildren<Item>() != null)
            {
                slot.GetComponentInChildren<Item>().GetComponent<RectTransform>().position = slot.GetComponent<RectTransform>().position;
            }
        }
        CountSlots();
    }
    public void CountSlots()
    {
        slotsTaken = 0;
        foreach (GameObject slot in inventorySlots)
        {
            if (slot.GetComponentInChildren<Item>() != null)
            {
                slotsTaken++;
            }
        }
    }

    public void PlaceInInventory(GameObject item)
    {
        if (slotsTaken == 28)
        {
            Destroy(item);
            Debug.Log("ERROR item destroyed");
            return;
        }

        if (item.GetComponent<StackableItem>() != null)
        {
            GameObject existingItem = ScanForItem(item.name);
            if (existingItem != null)
            {
                existingItem.GetComponent<StackableItem>().AddToQuantity(item.GetComponent<StackableItem>().quantity);
                Destroy(item);
                return;
            }
        }

        item.transform.SetParent(unsortedItems.transform);
        SortInventory();
        CountSlots();
    }

    public void UpdateStats()
    {
        if (UpdateEquippedItems != null)
        {
            UpdateEquippedItems();
        }
        if (ReadEquippedItems != null)
        {
            ReadEquippedItems();
        }
    }

    public GameObject ScanForItem(string itemName)
    {
        GameObject item = null;
        foreach (GameObject slot in inventorySlots)
        {
            if (slot.transform.childCount > 0 && slot.transform.GetChild(0).name == itemName)
            {
                item = slot.transform.GetChild(0).gameObject;
                break;
            }
        }

        return item;
    }
}
