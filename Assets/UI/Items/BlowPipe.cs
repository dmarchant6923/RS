using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowPipe : MonoBehaviour
{
    public GameObject ammoLoaded;
    public int numberLoaded;

    Item itemScript;
    Action itemAction;
    Equipment equipScript;
    public Color projectileColor;

    private IEnumerator Start()
    {
        itemScript = GetComponent<Item>();
        itemAction = GetComponent<Action>();
        equipScript = GetComponent<Equipment>();

        yield return null;


        if (ammoLoaded == null || numberLoaded == 0)
        {
            ammoLoaded = null;
            numberLoaded = 0;
        }
        else
        {
            itemScript.menuTexts[5] = "Unload ";
            projectileColor = ammoLoaded.GetComponent<StackableItem>().projectileColor;
            AddDartStats();
        }

        itemAction.cancelLevels[5] = 1;
        itemAction.orderLevels[5] = -1;
        itemAction.clientAction5 += Click;
        itemAction.serverAction5 += Unload;

        itemAction.serverActionUse += ObjectUsedOnThis;
        itemAction.cancelLevels[9] = 1;
        itemAction.orderLevels[9] = -1;

        yield return null;
        itemScript.UpdateActions();
    }

    void Click()
    {
        itemScript.clickHeld = true;
    }

    void Unload()
    {
        Inventory.instance.CountSlots();
        if (Inventory.slotsTaken == 28)
        {
            Debug.Log("You don't have enough inventory space.");
            return;
        }

        GameObject ammo = Inventory.instance.ScanForItem(ammoLoaded.name);
        if (Inventory.instance.ScanForItem(ammoLoaded.name) != null)
        {
            ammo.GetComponent<StackableItem>().AddToQuantity(numberLoaded);
        }
        else
        {
            ammo = Instantiate(ammoLoaded);
            ammo.name = ammoLoaded.name;
            ammo.GetComponent<StackableItem>().quantity = numberLoaded;
            ammo.GetComponent<Item>().inventory = itemScript.inventory;
            ammo.GetComponent<Item>().groundItemsParent = itemScript.groundItemsParent;
            ammo.GetComponent<Item>().groundPrefab = itemScript.groundPrefab;
            Inventory.instance.PlaceInInventory(ammo);
        }

        equipScript.rangedStrength -= ammoLoaded.GetComponent<Equipment>().rangedStrength;
        ammoLoaded = null;
        RemoveUnloadAction();
    }

    public void AddToQuantity(int num)
    {
        numberLoaded += num;
        if (numberLoaded <= 0)
        {
            numberLoaded = 0;
            RemoveUnloadAction();
        }
    }

    void RemoveUnloadAction()
    {
        numberLoaded = 0;
        itemScript.menuTexts[5] = "";
        itemScript.UpdateActions();
    }

    void AddUnloadAction()
    {
        itemScript.menuTexts[5] = "Unload ";
        itemScript.UpdateActions();
    }

    void ObjectUsedOnThis()
    {
        if (itemAction.actionUsedOnThis.gameObject.name.ToLower().Contains("dart") == false)
        {
            itemAction.DefaultUseAction();
            return;
        }

        int currentLoaded = numberLoaded;
        StackableItem stackScript = itemAction.actionUsedOnThis.GetComponent<StackableItem>();
        if (currentLoaded > 0 && stackScript.gameObject.name == ammoLoaded.name)
        {
            AddToQuantity(stackScript.quantity);
            Destroy(stackScript.gameObject);
        }
        else if (currentLoaded > 0)
        {
            equipScript.rangedStrength -= ammoLoaded.GetComponent<Equipment>().rangedStrength;

            GameObject ammo = Inventory.instance.ScanForItem(ammoLoaded.name);

            GameObject currentAmmo = itemAction.gameObject;
            ammoLoaded = Tools.LoadFromResource(itemAction.actionUsedOnThis.gameObject.name);
            AddToQuantity(stackScript.quantity);
            Destroy(itemAction.gameObject);

            Inventory.instance.CountSlots();
            if (Inventory.instance.ScanForItem(currentAmmo.name) != null)
            {
                ammo.GetComponent<StackableItem>().AddToQuantity(numberLoaded);
            }
            else
            {
                ammo = Instantiate(currentAmmo);
                ammo.name = ammoLoaded.name;
                ammo.GetComponent<StackableItem>().quantity = numberLoaded;
                ammo.GetComponent<Item>().inventory = itemScript.inventory;
                ammo.GetComponent<Item>().groundItemsParent = itemScript.groundItemsParent;
                ammo.GetComponent<Item>().groundPrefab = itemScript.groundPrefab;
            }

            AddDartStats();
            AddUnloadAction();
        }
        else
        {
            ammoLoaded = Tools.LoadFromResource(itemAction.actionUsedOnThis.gameObject.name);
            AddToQuantity(itemAction.actionUsedOnThis.GetComponent<StackableItem>().quantity);
            Destroy(stackScript.gameObject);

            AddDartStats();
            AddUnloadAction();
        }
    }

    void AddDartStats()
    {
        equipScript.rangedStrength += ammoLoaded.GetComponent<Equipment>().rangedStrength;
    }

    public void UseRangedAmmo(Vector2 targetTile, int delay)
    {
        float rand = Random.Range(0f, 1f);
        if (WornEquipment.assembler)
        {
            if (rand < 0.2f)
            {
                AddToQuantity(-1);
            }
        }
        else if (WornEquipment.accumulator)
        {
            if (rand < 0.2f)
            {
                AddToQuantity(-1);
            }
            else if (rand < 0.28f)
            {
                AddToQuantity(-1);
                //spawn logic
            }
        }
        else
        {
            AddToQuantity(-1);
            if (rand > 0.07f)
            {
                //spawn logic
            }
        }
    }
}
