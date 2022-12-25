using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadPlayerAttributes : MonoBehaviour
{
    public static LoadPlayerAttributes instance;

    public static string[] _equipment = new string[11];
    public static string _equipBlowpipeAmmo;
    public static string[] _items = new string[28];
    public static string _inventoryBlowpipeAmmo;

    private void Start()
    {
        instance = this;

        if (string.IsNullOrEmpty(_equipment[0]))
        {
            return;
        }

        //StartCoroutine(LoadPresetEnum(_equipment, _equipBlowpipeAmmo, _items, _inventoryBlowpipeAmmo));
    }

    public static IEnumerator LoadPresetEnum(GameManager.ItemAndQuantity[] equipment, string equipBlowpipeAmmo, GameManager.ItemAndQuantity[] items, string inventoryBlowpipeAmmo)
    //public static IEnumerator LoadPresetEnum(string[] equipment, string equipBlowpipeAmmo, string[] items, string inventoryBlowpipeAmmo)
    {
        yield return new WaitForSeconds(0.05f);
        foreach (GameObject slot in Inventory.inventorySlots)
        {
            if (slot.GetComponentInChildren<Item>() != null)
            {
                Destroy(slot.GetComponentInChildren<Item>().gameObject);
            }
        }

        yield return null;

        foreach (Transform slot in WornEquipment.slots)
        {
            if (slot.GetComponentInChildren<Equipment>() != null)
            {
                slot.GetComponentInChildren<Equipment>().DestroyEquippedItem();
            }
        }

        List<GameObject> equipments = new List<GameObject>();
        for (int i = 0; i < 11; i++)
        {
            if (string.IsNullOrEmpty(equipment[i].name) == false)
            {
                GameObject newEquipment = Tools.LoadFromResource(equipment[i].name);
                if (newEquipment != null)
                {
                    equipments.Add(newEquipment);
                    if (newEquipment.GetComponent<StackableItem>() != null)
                    {
                        newEquipment.GetComponent<StackableItem>().quantity = equipment[i].quantity;
                    }
                    if (newEquipment.GetComponent<ChargeItem>() != null)
                    {
                        newEquipment.GetComponent<ChargeItem>().charges = 2000;
                    }
                    if (newEquipment.GetComponent<BlowPipe>() != null && string.IsNullOrEmpty(equipBlowpipeAmmo) == false)
                    {
                        newEquipment.GetComponent<BlowPipe>().ammoLoaded = Tools.LoadFromResource(equipBlowpipeAmmo);
                        newEquipment.GetComponent<BlowPipe>().numberLoaded = 2000;
                    }
                }
            }
        }

        List<GameObject> newItems = new List<GameObject>();
        for (int i = 0; i < 28; i++)
        {
            if (string.IsNullOrEmpty(items[i].name) == false)
            {
                GameObject newItem = Tools.LoadFromResource(items[i].name);
                if (newItem != null)
                {
                    newItems.Add(newItem);
                    if (newItem.GetComponent<StackableItem>() != null)
                    {
                        newItem.GetComponent<StackableItem>().quantity = items[i].quantity;
                    }
                    if (newItem.GetComponent<ChargeItem>() != null)
                    {
                        newItem.GetComponent<ChargeItem>().charges = 2000;
                    }
                    if (newItem.GetComponent<BlowPipe>() != null && string.IsNullOrEmpty(inventoryBlowpipeAmmo) == false)
                    {
                        newItem.GetComponent<BlowPipe>().ammoLoaded = Tools.LoadFromResource(inventoryBlowpipeAmmo);
                        newItem.GetComponent<BlowPipe>().numberLoaded = 2000;
                    }

                    Inventory.instance.PlaceInInventory(newItem, i);
                }
            }
        }

        yield return new WaitForSeconds(0.05f);

        foreach (GameObject equipItem in equipments)
        {
            equipItem.GetComponent<Equipment>().Equip();
        }
        newItems.Reverse();
        //foreach (GameObject item in newItems)
        //{
        //    Inventory.instance.PlaceInInventory(item);
        //}
    }
}
