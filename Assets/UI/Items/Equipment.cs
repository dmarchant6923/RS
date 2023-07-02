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

    [HideInInspector] public bool isEquipped = false;
    [HideInInspector] public bool equippedWhenClicked = false;
    public string equipSlotName;
    [HideInInspector] public Transform equipSlot;
    [HideInInspector] public Item itemScript;
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

    public int attackSpeed;
    public int attackDistance = 1;

    bool chargeItem;
    ChargeItem chargeScript;

    public bool addAmmoRangedStats = true;
    public Color overrideProjectileColor;
    public bool canFireDragonAmmo = true;

    [HideInInspector] public SpecialAttack spec;

    public AudioClip overrideAttackSound;
    AudioClip equipSound;

    private IEnumerator Start()
    {
        itemAction = GetComponent<Action>();

        yield return null; 

        for (int i = 0; i < WornEquipment.instance.transform.childCount; i++)
        {
            if (equipSlotName.ToLower() == WornEquipment.instance.transform.GetChild(i).name.ToLower())
            {
                equipSlot = WornEquipment.instance.transform.GetChild(i);
                break;
            }
        }

        if (equipSlot != WornEquipment.ammoSlot && equipSlot != WornEquipment.weaponSlot)
        {
            stackable = false;
        }

        itemScript = GetComponent<Item>();
        spec = GetComponent<SpecialAttack>();

        yield return null;

        equipString = "Wear ";
        if ((weaponCategory != null && weaponCategory != "") || equipSlot == WornEquipment.ammoSlot)
        {
            equipString = "Wield ";
        }
        itemScript.menuTexts[0] = equipString;
        itemAction.cancelLevels[0] = 1;
        itemScript.UpdateActions();
        itemAction.clientAction0 += Click;
        itemAction.serverAction0 += AddToEquipQueue;
        itemAction.orderLevels[0] = -1;

        if (GetComponent<ChargeItem>() != null)
        {
            chargeItem = true;
            chargeScript = GetComponent<ChargeItem>();
        }


    }

    void Click()
    {
        itemScript.clickHeld = true;
    }

    void AddToEquipQueue()
    {
        if (equippedWhenClicked == isEquipped)
        {
            Equip();
        }
    }

    public void Equip()
    {
        itemScript.clickHeld = false;
        if (isEquipped == false)
        {
            if (twoHanded && Inventory.slotsTaken == 28 && WornEquipment.shield != null)
            {
                GameLog.Log("You don't have enough free space to do that.");
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

            foreach (Equipment equipment in equipSlot.GetComponentsInChildren<Equipment>())
            {
                if (equipment != this && equipment.name == gameObject.name && stackable == true)
                {
                    equipment.GetComponent<StackableItem>().AddToQuantity(GetComponent<StackableItem>().quantity);
                    Destroy(gameObject);
                    break;
                }

                if (equipment != this)
                {
                    equipment.Equip();
                    equipment.transform.SetParent(inventorySlot);
                    equipment.transform.position = inventorySlot.position;
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

            WornEquipment.PlayEquipNoise();
        }
        else
        {
            if (stackable)
            {
                GameObject existingItem = itemScript.inventory.ScanForItem(gameObject.name);
                if (existingItem != null)
                {
                    existingItem.GetComponent<StackableItem>().AddToQuantity(GetComponent<StackableItem>().quantity);
                    equipSlot.GetComponent<RawImage>().enabled = true;
                    Destroy(gameObject);
                    return;
                }
            }

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
                return;
            }
        }

        SpecBar.instance.CheckSpec();
        itemScript.inventory.UpdateStats();

        if (equipSlot.GetComponentInChildren<Equipment>() != null)
        {
            equipSlot.GetComponent<RawImage>().enabled = false;
        }
        else
        {
            equipSlot.GetComponent<RawImage>().enabled = true;
        }
    }

    public void DestroyEquippedItem()
    {
        equipSlot.GetComponent<RawImage>().enabled = true;
        itemScript.inventory.UpdateStats();
        Destroy(gameObject);
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
