using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InfernoPortal : MonoBehaviour
{
    Action portalAction;
    Vector2 swTile;
    int size = 4;

    bool willEnterPortal = false;
    void Start()
    {
        portalAction = GetComponent<Action>();
        portalAction.menuTexts[0] = "Enter ";
        portalAction.menuPriorities[0] = 1;
        portalAction.objectName = "<color=cyan>" + gameObject.name + "</color>";
        portalAction.UpdateName();
        portalAction.serverAction0 += EnterPortal;

        Vector2 position = transform.position;
        swTile = TileManager.FindTile(position - Vector2.one * 1.5f);

        TickManager.beforeTick += BeforeTick;
        Action.cancel1 += Cancel;
    }

    void EnterPortal()
    {
        willEnterPortal = true;
        if (Tools.PlayerIsAdjacentToLargeObject(swTile, size, size, false) == false)
        {
            Player.player.trueTileScript.ExternalMovement(Tools.NearestTileToPlayer(swTile, size, size));
            return;
        }
        BeforeTick();
    }

    void BeforeTick()
    {
        if (willEnterPortal && Tools.PlayerIsAdjacentToLargeObject(swTile, size, size, false))
        {
            willEnterPortal = false;

            string[] equipment = new string[11];
            equipment[0] = WornEquipment.head != null ? WornEquipment.head.name : "";
            equipment[1] = WornEquipment.cape != null ? WornEquipment.cape.name : "";
            equipment[2] = WornEquipment.neck != null ? WornEquipment.neck.name : "";
            equipment[3] = WornEquipment.ammo != null ? WornEquipment.ammo.name : "";
            equipment[4] = WornEquipment.weapon != null ? WornEquipment.weapon.name : "";
            equipment[5] = WornEquipment.body != null ? WornEquipment.body.name : "";
            equipment[6] = WornEquipment.shield != null ? WornEquipment.shield.name : "";
            equipment[7] = WornEquipment.leg != null ? WornEquipment.leg.name : "";
            equipment[8] = WornEquipment.glove != null ? WornEquipment.glove.name : "";
            equipment[9] = WornEquipment.boot != null ? WornEquipment.boot.name : "";
            equipment[10] = WornEquipment.ring != null ? WornEquipment.ring.name : "";

            string equipBlowpipeAmmo = null;
            if (equipment[4] == "Toxic blowpipe")
            {
                equipBlowpipeAmmo = WornEquipment.weapon.GetComponent<BlowPipe>().ammoLoaded.name;
            }

            string[] items = new string[28];
            string inventoryBlowpipeAmmo = null;

            for (int i = 0; i < 28; i++)
            {
                if (Inventory.inventorySlots[i].GetComponentInChildren<Item>() != null)
                {
                    items[i] = Inventory.inventorySlots[i].GetComponentInChildren<Item>().name;
                    if (Inventory.inventorySlots[i].GetComponentInChildren<BlowPipe>() != null)
                    {
                        inventoryBlowpipeAmmo = Inventory.inventorySlots[i].GetComponentInChildren<BlowPipe>().ammoLoaded.name;
                    }
                }
            }

            LoadPlayerAttributes._equipment = equipment;
            LoadPlayerAttributes._equipBlowpipeAmmo = equipBlowpipeAmmo;
            LoadPlayerAttributes._items = items;
            LoadPlayerAttributes._inventoryBlowpipeAmmo = inventoryBlowpipeAmmo;

            SceneManager.LoadScene("Zuk");
        }
    }

    void Cancel()
    {
        willEnterPortal = false;
    }

    private void OnDestroy()
    {
        Action.cancel1 -= Cancel;
    }
}
