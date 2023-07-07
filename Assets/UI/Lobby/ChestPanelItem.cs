using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestPanelItem : MonoBehaviour
{
    Action itemAction;
    ChestPanel chestPanelScript;

    public string examineText = "asdfas";
    public bool stackable = false;

    private void Start()
    {
        chestPanelScript = transform.parent.parent.GetComponent<ChestPanel>();
        itemAction = GetComponent<Action>();

        itemAction.objectName = "<color=orange>" + gameObject.name + "</color>";
        itemAction.menuTexts[0] = "Take ";
        itemAction.serverAction0 += TakeOne;
        if (stackable)
        {
            itemAction.menuTexts[1] = "Take-10 ";
            itemAction.serverAction1 += TakeTen;
            itemAction.menuTexts[2] = "Take-1000 ";
            itemAction.serverAction2 += TakeThousand;
        }
        itemAction.examineText = examineText;
        itemAction.serverActionExamine += itemAction.ReturnGEPrice;
        itemAction.UpdateName();
    }

    void TakeOne()
    {
        TakeItem(1);
    }
    void TakeTen()
    {
        TakeItem(10);
    }
    void TakeThousand()
    {
        TakeItem(1000);
    }

    void TakeItem(int num)
    {
        Inventory.instance.CountSlots();
        if (Inventory.slotsTaken == 28)
        {
            GameLog.Log("You don't have enough space.");
            return;
        }

        if (gameObject.activeSelf)
        {
            StartCoroutine(TakeItemCR(num));
        }
    }

    IEnumerator TakeItemCR(int num)
    {
        if (stackable)
        {
            GameObject stackableItem = Inventory.instance.ScanForItem(gameObject.name);
            if (stackableItem != null)
            {
                stackableItem.GetComponent<StackableItem>().AddToQuantity(num);
                //chestPanelScript.UpdatePrice();
                yield break;
            }
        }
        string name = gameObject.name;
        if (name.EndsWith("(4)"))
        {
            name = name.Remove(name.Length - 3);
        }
        GameObject newItem = Tools.LoadFromResource(name);
        newItem.name = name;
        if (newItem.GetComponent<StackableItem>() != null)
        {
            newItem.GetComponent<StackableItem>().quantity = num;
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

        PlayerAudio.PlayClip(PlayerAudio.instance.pickUpSound);

        yield return null;

        Item itemScript = newItem.GetComponent<Item>();
        itemScript.menuTexts[7] = "Deposit ";
        itemScript.itemAction.serverAction7 += itemScript.DestroyItem;
        itemScript.itemAction.menuPriorities[7] = 1;
        itemScript.UpdateActions();
        //chestPanelScript.UpdatePrice();
    }
}
