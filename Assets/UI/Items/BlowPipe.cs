using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowPipe : MonoBehaviour
{
    public GameObject ammoLoaded;
    public string ammoLoadedName;
    public int numberLoaded;

    Item itemScript;
    Action itemAction;
    Equipment equipScript;

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
            AddDartStats();
            ammoLoaded = Tools.LoadFromResource(ammoLoaded.name);
            DontDestroyOnLoad(ammoLoaded);
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
        Destroy(ammoLoaded);
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
        itemScript.chargeScript.Check();
    }

    void ObjectUsedOnThis()
    {
        if (itemAction.actionUsedOnThis.gameObject.name.ToLower().Contains("dart") == false)
        {
            return;
        }

        int currentNumberLoaded = numberLoaded;
        StackableItem selectedAmmoStackScript = itemAction.actionUsedOnThis.GetComponent<StackableItem>();
        //case 1: BP has ammo and the SAME ammo is being used on it
        if (currentNumberLoaded > 0 && selectedAmmoStackScript.gameObject.name == ammoLoaded.name)
        {
            AddToQuantity(selectedAmmoStackScript.quantity);
            Destroy(selectedAmmoStackScript.gameObject);
        }
        //case 2: BP has ammo and DIFFERENT ammo is being used on it
        else if (currentNumberLoaded > 0)
        {
            equipScript.rangedStrength -= ammoLoaded.GetComponent<Equipment>().rangedStrength;

            string currentAmmoName = ammoLoaded.name;
            int currentAmmoQuantity = numberLoaded;
            Destroy(ammoLoaded);
            ammoLoaded = Tools.LoadFromResource(itemAction.actionUsedOnThis.name);
            numberLoaded = itemAction.actionUsedOnThis.GetComponent<StackableItem>().quantity;
            itemAction.actionUsedOnThis.transform.SetParent(null);
            Destroy(itemAction.actionUsedOnThis.gameObject);

            GameObject newAmmo = Tools.LoadFromResource(currentAmmoName);
            newAmmo.GetComponent<StackableItem>().quantity = currentAmmoQuantity;
            Inventory.instance.PlaceInInventory(newAmmo);

            //GameObject currentAmmoObject = Inventory.instance.ScanForItem(ammoLoaded.name);

            //GameObject newAmmoObject = itemAction.actionUsedOnThis.gameObject;
            //ammoLoaded = Tools.LoadFromResource(itemAction.actionUsedOnThis.gameObject.name);
            //AddToQuantity(selectedAmmoStackScript.quantity);
            //Destroy(itemAction.actionUsedOnThis.gameObject);

            //Inventory.instance.CountSlots();
            //if (Inventory.instance.ScanForItem(newAmmoObject.name) != null)
            //{
            //    currentAmmoObject.GetComponent<StackableItem>().AddToQuantity(numberLoaded);
            //}
            //else
            //{
            //    currentAmmoObject = Instantiate(newAmmoObject);
            //    currentAmmoObject.name = ammoLoaded.name;
            //    currentAmmoObject.GetComponent<StackableItem>().quantity = numberLoaded;
            //    currentAmmoObject.GetComponent<Item>().inventory = itemScript.inventory;
            //    currentAmmoObject.GetComponent<Item>().groundItemsParent = itemScript.groundItemsParent;
            //    currentAmmoObject.GetComponent<Item>().groundPrefab = itemScript.groundPrefab;
            //}

            AddDartStats();
            AddUnloadAction();
        }
        //case 3: BP has no ammo currently loaded.
        else
        {
            ammoLoaded = Tools.LoadFromResource(itemAction.actionUsedOnThis.gameObject.name);
            AddToQuantity(itemAction.actionUsedOnThis.GetComponent<StackableItem>().quantity);
            Destroy(selectedAmmoStackScript.gameObject);

            AddDartStats();
            AddUnloadAction();
        }

        DontDestroyOnLoad(ammoLoaded);
        itemAction.foundUseActionMethod = true;
        return;
    }

    void AddDartStats()
    {
        equipScript.rangedStrength += ammoLoaded.GetComponent<Equipment>().rangedStrength;
        equipScript.overrideProjectileColor = ammoLoaded.GetComponent<StackableItem>().projectileColor;
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
