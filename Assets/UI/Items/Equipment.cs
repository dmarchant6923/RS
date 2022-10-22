using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Equipment : MonoBehaviour
{
    public bool twoHanded = false;
    public string weaponCategory;
    public string ammoCategory;

    Action itemAction;

    bool isEquipped = false;
    bool equipDelayActive = false;
    public Transform equipSlot;
    public GameObject itemsParent;
    Item itemScript;
    string equipString;

    public bool stackable;

    public int attackStab;
    public int attackSlash;
    public int attackCrush;
    public int attackMagic;
    public int attackRange;

    public int defenceStab;
    public int defenceSlash;
    public int defenceCrush;
    public int defenceMagic;
    public int defenceRange;

    public int meleeStrength;
    public int rangedStrength;
    public int magicDamage;
    public int prayer;

    public bool hasSpec = false;
    public int attackSpeed;
    public int specCost;
    public string specDescription;

    bool chargeItem;
    ChargeItem chargeScript;

    private IEnumerator Start()
    {
        itemAction = GetComponent<Action>();
        itemAction.itemAction = true;

        if (equipSlot != WornEquipment.ammoSlot)
        {
            stackable = false;
        }

        itemScript = GetComponent<Item>();

        yield return null;

        equipString = "Wear ";
        if (weaponCategory != null && weaponCategory != "")
        {
            equipString = "Wield ";
        }
        itemScript.menuTexts[0] = equipString;
        itemAction.cancelLevel[0] = 1;
        itemScript.UpdateActions();
        itemAction.action0 += DelayedEquip;

        if (GetComponent<ChargeItem>() != null)
        {
            chargeItem = true;
            chargeScript = GetComponent<ChargeItem>();
        }
    }

    public void DelayedEquip()
    {
        if (equipDelayActive == false && itemScript.inventory.equipQueue.Contains(this) == false)
        {
            itemScript.clickHeld = true;
            StartCoroutine(EquipDelay());
        }
    }
    IEnumerator EquipDelay()
    {
        equipDelayActive = true;
        yield return new WaitForSeconds(TickManager.simLatency);
        equipDelayActive = false;
        itemScript.inventory.equipQueue.Add(this);
    }

    public void Equip()
    {
        itemScript.clickHeld = false;
        if (isEquipped == false)
        {
            if (twoHanded && Inventory.slotsTaken == 28 && WornEquipment.shield != null)
            {
                Debug.Log("You don't have enough free space to do that.");
                return;
            }

            isEquipped = true;

            itemScript.menuTexts[0] = "Remove ";
            itemScript.menuTexts[2] = "";

            itemScript.menuTexts[4] = "";
            itemScript.UpdateActions();

            //itemAction.action1 -= Use;

            Transform inventorySlot = transform.parent;
            transform.SetParent(equipSlot.transform);
            transform.position = equipSlot.transform.position;

            foreach (Equipment item in equipSlot.GetComponentsInChildren<Equipment>())
            {
                if (item != this)
                {
                    item.Equip();
                    item.transform.SetParent(inventorySlot);
                    item.transform.position = inventorySlot.position;
                    break;
                }
            }

            if (twoHanded && WornEquipment.shield != null)
            {
                WornEquipment.shield.Equip();
            }
            if (equipSlot == WornEquipment.shieldSlot && WornEquipment.weapon != null && WornEquipment.weapon.twoHanded)
            {
                WornEquipment.weapon.Equip();
            }
            if (chargeItem)
            {
                chargeScript.RemoveUnchargeAction();
            }
        }
        else
        {
            if (Inventory.slotsTaken < 28)
            {
                isEquipped = false;

                itemScript.menuTexts[0] = equipString;
                itemScript.menuTexts[2] = "Use ";

                itemScript.menuTexts[4] = "Drop ";
                itemScript.UpdateActions();

                //itemAction.action1 += Use;

                itemScript.inventory.PlaceInInventory(gameObject);
                if (chargeItem && chargeScript.charges > 0)
                {
                    chargeScript.AddUnchargeAction();
                }
            }
            else
            {
                Debug.Log("You don't have enough free space to do that.");
            }
        }

        if (equipSlot.GetComponentInChildren<Equipment>() != null)
        {
            equipSlot.GetComponent<RawImage>().enabled = false;
        }
        else
        {
            equipSlot.GetComponent<RawImage>().enabled = true;
        }
    }

    public void RemoveEquipAction()
    {
        itemScript.menuTexts[0] = "";
        itemScript.UpdateActions();
    }

    public void AddEquipAction()
    {
        itemScript.menuTexts[0] = equipString;
        itemScript.UpdateActions();
    }
}
