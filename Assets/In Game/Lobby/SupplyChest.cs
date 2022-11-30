using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyChest : MonoBehaviour
{
    Action chestAction;
    public GameObject chestPanel;

    Vector2 swTile;
    int size = 2;

    bool willOpenChest = false;

    private void Start()
    {
        chestAction = GetComponent<Action>();
        chestAction.menuTexts[0] = "Open ";
        chestAction.objectName = "<color=cyan>" + gameObject.name + "</color>";
        chestAction.serverAction0 += OpenChest;
        chestAction.orderLevels[0] = -1;
        chestAction.menuPriorities[0] = 1;
        chestAction.cancelLevels[0] = 1;
        chestAction.staticPlayerActions[0] = true;
        chestAction.inGame = true;
        chestAction.UpdateName();

        Action.cancel1 += Cancel;

        Vector2 position = transform.position;
        swTile = TileManager.FindTile(position - Vector2.one * 0.5f);
        position = swTile + Vector2.one * 0.5f + Vector2.down * 0.2f;
        transform.position = position;

        TickManager.beforeTick += BeforeTick;
    }

    void OpenChest()
    {
        willOpenChest = true;
        if (Tools.PlayerIsAdjacentToLargeObject(swTile, size, size, false) == false)
        {
            Player.player.trueTileScript.ExternalMovement(Tools.NearestTileToPlayer(swTile, size, size));
            return;
        }
        BeforeTick();
    }

    void BeforeTick()
    {
        if (willOpenChest && Tools.PlayerIsAdjacentToLargeObject(swTile, size, size, false))
        {
            chestPanel.SetActive(true);
            PanelButtons.instance.ForceOpen("Inventory");
            willOpenChest = false;
        }
    }

    void Cancel()
    {
        willOpenChest = false;
    }

    private void OnDestroy()
    {
        Action.cancel1 -= Cancel;
    }
}
