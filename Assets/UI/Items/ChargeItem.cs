using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeItem : MonoBehaviour
{
    public int charges;
    public int maxCharges;

    public GameObject chargeItem1;
    public GameObject chargeItem2;
    public GameObject chargeItem3;
    public GameObject chargeItem4;
    List<GameObject> chargeItems = new List<GameObject>();

    public int chargeQuantity1;
    public int chargeQuantity2;
    public int chargeQuantity3;
    public int chargeQuantity4;
    List<int> chargeQuantities = new List<int>();

    Action itemAction;
    Inventory inventory;
    Item itemScript;

    bool chargeDialogueActive = false;
    bool chargeInputRecieved = false;
    int inputCharges = 0;
    string initialName;

    bool isEquipment;
    Equipment equipScript;

    private IEnumerator Start()
    {
        itemScript = GetComponent<Item>();
        inventory = itemScript.inventory;
        itemAction = GetComponent<Action>();

        if (chargeItem1 != null)
        {
            chargeItems.Add(chargeItem1);
            chargeQuantities.Add(chargeQuantity1);
        }
        if (chargeItem2 != null)
        {
            chargeItems.Add(chargeItem2);
            chargeQuantities.Add(chargeQuantity2);
        }
        if (chargeItem3 != null)
        {
            chargeItems.Add(chargeItem3);
            chargeQuantities.Add(chargeQuantity3);
        }
        if (chargeItem4 != null)
        {
            chargeItems.Add(chargeItem4);
            chargeQuantities.Add(chargeQuantity4);
        }

        TickManager.beforeTick += BeforeTick;
        TickManager.afterTick += AfterTick;

        yield return null;

        itemScript.menuTexts[1] = "Check ";
        itemAction.serverAction1 += Check;
        itemAction.cancelLevels[1] = 1;
        itemAction.orderLevels[1] = 1;

        itemAction.serverAction3 += Uncharge;
        itemAction.cancelLevels[3] = 1;
        itemAction.orderLevels[3] = -1;

        itemScript.menuTexts[3] = "Uncharge ";
        itemScript.UpdateActions(gameObject.name);

        itemAction.serverActionUse += ObjectUsedOnThis;
        itemAction.cancelLevels[9] = 1;
        itemAction.orderLevels[9] = -1;

        initialName = gameObject.name;
        if (charges <= 0)
        {
            charges = 0;
            ChargeToUncharged();
        }
        if (GetComponent<Equipment>() != null)
        {
            isEquipment = true;
            equipScript = GetComponent<Equipment>();
        }
    }

    public void UseCharge()
    {
        if (charges <= 0)
        {
            Debug.Log("ERROR: took a charge when it had no charges");
            return;
        }

        charges--;
        if (charges == 0 && ((isEquipment && equipScript.isEquipped == false) || isEquipment == false))
        {
            if (isEquipment)
            {
                Debug.Log("Your " + gameObject.name + " has run out of charges.");
            }
            ChargeToUncharged();
        }
    }

    void AfterTick()
    {
        if (charges == 0 && isEquipment && equipScript.isEquipped == false && string.IsNullOrEmpty(itemScript.menuTexts[0]) == false)
        {
            ChargeToUncharged();
        }
    }

    void Uncharge()
    {
        if (charges <= 0)
        {
            return;
        }

        int numSpaces = chargeItems.Count;
        foreach (GameObject item in chargeItems)
        {
            if (inventory.ScanForItem(item.name) != null)
            {
                numSpaces--;
            }
        }

        inventory.CountSlots();
        if (Inventory.slotsTaken > 28 - numSpaces)
        {
            Debug.Log("You don't have enough inventory space.");
            return;
        }

        for (int i = 0; i < chargeItems.Count; i++)
        {
            GameObject newItem = inventory.ScanForItem(chargeItems[i].name);
            if (newItem != null)
            {
                newItem.GetComponent<StackableItem>().AddToQuantity(chargeQuantities[i] * charges);
            }
            else
            {
                newItem = Instantiate(chargeItems[i]);
                newItem.name = chargeItems[i].name;
                newItem.GetComponent<StackableItem>().quantity = chargeQuantities[i] * charges;
                newItem.GetComponent<Item>().inventory = itemScript.inventory;
                newItem.GetComponent<Item>().groundItemsParent = itemScript.groundItemsParent;
                newItem.GetComponent<Item>().groundPrefab = itemScript.groundPrefab;
                inventory.PlaceInInventory(newItem);
            }
        }
        ChargeToUncharged();
    }


    void ChargeToUncharged()
    {
        charges = 0;
        itemScript.menuTexts[3] = "";
        itemScript.menuTexts[1] = "";
        if (isEquipment)
        {
            equipScript.RemoveEquipAction();
        }
        itemScript.itemImage.color = new Color(0.7f, 0.7f, 0.7f, 1);
        itemScript.UpdateActions(initialName + " (uncharged)");
        gameObject.name = initialName + " (uncharged)";

    }
    void UnchargedToCharged()
    {
        itemScript.menuTexts[1] = "Check ";
        itemScript.menuTexts[3] = "Uncharge ";
        if (isEquipment)
        {
            equipScript.AddEquipAction();
        }
        itemScript.itemImage.color = new Color(1, 1, 1, 1);
        itemScript.UpdateActions(initialName);
        gameObject.name = initialName;
    }
    public void RemoveUnchargeAction()
    {
        itemScript.menuTexts[3] = "";
        itemScript.UpdateActions(initialName);
    }
    public void AddUnchargeAction()
    {
        itemScript.menuTexts[3] = "Uncharge ";
        itemScript.UpdateActions(initialName);
    }


    void ObjectUsedOnThis()
    {
        bool itemFound = false;
        for (int i = 0; i < chargeItems.Count; i++)
        {
            if (itemAction.actionUsedOnThis.gameObject.name == chargeItems[i].name)
            {
                itemFound = true;
                break;
            }
        }

        if (itemFound == false)
        {
            itemAction.DefaultUseAction();
            return;
        }

        bool itemsInInventory = true;
        for (int i = 0; i < chargeItems.Count; i++)
        {
            GameObject item = inventory.ScanForItem(chargeItems[i].name);
            if (item == null || item.GetComponent<StackableItem>().quantity < chargeQuantities[i])
            {
                itemsInInventory = false;
                break;
            }
        }

        if (itemsInInventory == false)
        {
            string quantityString = "";

            if (chargeItems.Count == 1)
            {
                quantityString = chargeQuantities[0] + "x " + chargeItems[0].name;
            }
            else if (chargeItems.Count == 2)
            {
                quantityString = chargeQuantities[0] + "x " + chargeItems[0].name + " and " + chargeQuantities[1] + "x " + chargeItems[2].name;
            }
            else
            {
                for (int i = 0; i < chargeItems.Count; i++)
                {
                    if (i > 0)
                    {
                        quantityString += ", ";
                    }
                    if (i == chargeItems.Count - 1 && chargeItems.Count > 0)
                    {
                        quantityString += "and ";
                    }
                    quantityString += chargeQuantities[i] + "x " + chargeItems[i].name;
                }
            }

            Debug.Log("You need " + quantityString + " to charge the " + gameObject.name + ".");
            return;
        }

        int allowedCharges = maxCharges - charges;
        for (int i = 0; i < chargeItems.Count; i++)
        {
            GameObject item = inventory.ScanForItem(chargeItems[i].name);
            int num = (int)Mathf.Floor(item.GetComponent<StackableItem>().quantity / chargeQuantities[i]);
            if (num < allowedCharges)
            {
                allowedCharges = num;
            }
        }

        DialogueBox.PlayerInput("How many charges do you wish to add? (0 - " + allowedCharges + ")");

        chargeDialogueActive = true;
        DialogueBox.InputSubmitted += ReceiveInput;
    }
    void ReceiveInput()
    {
        DialogueBox.InputSubmitted -= ReceiveInput;
        Invoke("DelayReceiveInput", TickManager.simLatency);
        if (DialogueBox.InputString != "")
        {
            inputCharges = int.Parse(DialogueBox.InputString);
        }
    }
    void DelayReceiveInput()
    {
        chargeInputRecieved = true;
    }
    void BeforeTick()
    {
        if (chargeDialogueActive && chargeInputRecieved)
        {
            for (int i = 0; i < chargeItems.Count; i++)
            {
                GameObject item = inventory.ScanForItem(chargeItems[i].name);
                int num = (int)Mathf.Floor(item.GetComponent<StackableItem>().quantity / chargeQuantities[i]);
                if (num < inputCharges)
                {
                    inputCharges = num;
                }
            }
            if (charges == 0 && inputCharges > 0)
            {
                itemScript.UpdateActions(gameObject.name);
            }

            int initialCharges = charges;
            charges += inputCharges;
            for (int i = 0; i < chargeItems.Count; i++)
            {
                GameObject item = inventory.ScanForItem(chargeItems[i].name);
                item.GetComponent<StackableItem>().AddToQuantity(-inputCharges * chargeQuantities[i]);
            }

            if (initialCharges == 0)
            {
                UnchargedToCharged();
            }


            inputCharges = 0;
            chargeDialogueActive = false;
            chargeInputRecieved = false;
        }
    }


    void Check()
    {
        string end = "s.";
        if (charges == 1)
        {
            end = ".";
        }
        Debug.Log("Your " + gameObject.name + " has " + charges + " charge" + end);
    }
}
