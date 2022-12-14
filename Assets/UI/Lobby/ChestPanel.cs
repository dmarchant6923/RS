using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestPanel : MonoBehaviour
{
    [HideInInspector] public bool panelOpen = false;
    public GameObject image;
    public Transform itemParent;

    public int itemsPerRow;
    public int itemsPerColumn;

    public RectTransform nwCorner;
    public RectTransform neCorner;
    public RectTransform swCorner;
    public RectTransform seCorner;

    public float itemScale = 0.6f;

    public ButtonScript depositInventoryButton;
    public ButtonScript depositEquipmentButton;
    public GameObject warning;

    public List<GameObject> items = new List<GameObject>();
    List<GameObject> spawnedItems = new List<GameObject>();

    BasePanelScript panelScript;

    private IEnumerator Start()
    {
        TickManager.afterTick += CheckWarningTick;
        PlayerStats.reinitialize += FindPanel;
        panelScript = GetComponent<BasePanelScript>();
        panelScript.panelClosed += CloseChest;

        depositInventoryButton.buttonClicked += DepositInventory;
        depositEquipmentButton.buttonClicked += DepositEquipment;

        yield return null;

        foreach (GameObject item in items)
        {
            if (item != null)
            {
                GameObject newItem = Instantiate(item, Vector2.one * -1000, Quaternion.identity);
                newItem.name = item.name;
                spawnedItems.Add(newItem);
            }
        }

        yield return null;
        yield return null;

        int numItems = itemsPerRow * itemsPerColumn;

        float spacingX = ((float)neCorner.position.x - (float)nwCorner.position.x) / (itemsPerRow - 1);
        float spacingY = ((float)neCorner.position.y - (float)seCorner.position.y) / (itemsPerColumn - 1);

        Vector2 position = nwCorner.position;
        int columnNumber = 1;
        int rowNumber = 1;

        for (int i = 0; i < spawnedItems.Count; i++)
        {
            if (i < spawnedItems.Count && spawnedItems[i] != null)
            {
                float positionX = nwCorner.position.x + (columnNumber - 1) * spacingX;
                float positionY = nwCorner.position.y - (rowNumber - 1) * spacingY;
                position = new Vector2(positionX, positionY);

                GameObject newImage = Instantiate(image, position, Quaternion.identity);
                newImage.GetComponent<RawImage>().texture = spawnedItems[i].GetComponent<Item>().itemTexture;
                newImage.GetComponent<ChestPanelItem>().examineText = spawnedItems[i].GetComponent<Action>().examineText;
                newImage.name = items[i].name;
                newImage.transform.localScale = Vector2.one * itemScale * FindObjectOfType<Canvas>().scaleFactor;
                newImage.transform.SetParent(itemParent);

                columnNumber++;
                if (columnNumber > itemsPerRow)
                {
                    columnNumber = 1;
                    rowNumber++;
                }
            }
            else
            {
                break;
            }
        }

        foreach (GameObject item in spawnedItems)
        {
            Destroy(item);
        }
    }

    void FindPanel()
    {
        SupplyChest chest = FindObjectOfType<SupplyChest>();
        if (chest != null)
        {
            chest.chestPanel = itemParent.gameObject;
        }
    }

    public void OpenChest()
    {
        panelOpen = true;
        itemParent.gameObject.SetActive(true);
        PanelButtons.instance.ForceOpen("Inventory");
        foreach (GameObject slot in Inventory.inventorySlots)
        {
            if (slot.GetComponentInChildren<Item>() != null)
            {
                Item item = slot.GetComponentInChildren<Item>();
                item.menuTexts[7] = "Deposit ";
                item.itemAction.serverAction7 += item.DestroyItem;
                item.itemAction.menuPriorities[7] = 1;
                item.UpdateActions();
            }
        }
    }

    void CloseChest()
    {
        panelOpen = false;
        foreach (GameObject slot in Inventory.inventorySlots)
        {
            if (slot.GetComponentInChildren<Item>() != null)
            {
                Item item = slot.GetComponentInChildren<Item>();
                item.menuTexts[7] = null;
                item.itemAction.serverAction7 -= item.DestroyItem;
                item.itemAction.menuPriorities[7] = 0;
                item.UpdateActions();
            }
        }
    }

    void DepositInventory()
    {
        foreach (GameObject slot in Inventory.inventorySlots)
        {
            if (slot.GetComponentInChildren<Item>() != null)
            {
                Destroy(slot.GetComponentInChildren<Item>().gameObject);
            }
        }
    }

    void DepositEquipment()
    {
        foreach (Transform slot in WornEquipment.slots)
        {
            if (slot.GetComponentInChildren<Equipment>() != null)
            {
                slot.GetComponentInChildren<Equipment>().DestroyEquippedItem();
            }
        }
    }

    void CheckWarningTick()
    {
        if (panelOpen == false)
        {
            return;
        }

        Invoke(nameof(CheckWarning), 0.05f);
    }

    void CheckWarning()
    {
        warning.SetActive(UIManager.instance.CheckWarning());
    }
}
