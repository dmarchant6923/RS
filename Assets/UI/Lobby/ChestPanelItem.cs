using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestPanelItem : MonoBehaviour
{
    Action itemAction;
    ChestPanel chestPanelScript;

    public string examineText = "asdfas";

    private void Start()
    {
        chestPanelScript = transform.parent.parent.GetComponent<ChestPanel>();
        itemAction = GetComponent<Action>();

        itemAction.menuTexts[0] = "Take ";
        itemAction.objectName = "<color=orange>" + gameObject.name + "</color>";
        itemAction.serverAction0 += TakeItem;
        itemAction.examineText = examineText;
        itemAction.UpdateName();
    }

    void TakeItem()
    {
        Inventory.instance.CountSlots();
        if (Inventory.slotsTaken == 28)
        {
            GameLog.Log("You don't have enough space.");
        }

        if (gameObject.activeSelf)
        {
            StartCoroutine(TakeItemCR());
        }
    }

    IEnumerator TakeItemCR()
    {
        GameObject newItem = Tools.LoadFromResource(gameObject.name);
        newItem.name = gameObject.name;
        if (newItem.GetComponent<StackableItem>() != null)
        {
            newItem.GetComponent<StackableItem>().quantity = 2000;
        }
        if (newItem.GetComponent<ChargeItem>() != null)
        {
            newItem.GetComponent<ChargeItem>().charges = 2000;
        }
        if (newItem.GetComponent<BlowPipe>() != null)
        {
            newItem.GetComponent<BlowPipe>().ammoLoaded = Tools.LoadFromResource("Dragon dart");
            newItem.GetComponent<BlowPipe>().numberLoaded = 2000;
        }
        Inventory.instance.PlaceInInventory(newItem);

        yield return null;

        Item itemScript = newItem.GetComponent<Item>();
        itemScript.menuTexts[7] = "Deposit ";
        itemScript.itemAction.serverAction7 += itemScript.DestroyItem;
        itemScript.itemAction.menuPriorities[7] = 1;
        itemScript.UpdateActions();
    }
}
