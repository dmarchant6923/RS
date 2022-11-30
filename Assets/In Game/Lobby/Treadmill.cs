using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treadmill : MonoBehaviour
{
    Action treadmillAction;
    public GameObject skillsPanel;

    Vector2 swTile;
    int sizeX = 4;
    int sizeY = 2;

    bool willUseTreadmill = false;
    void Start()
    {
        treadmillAction = GetComponent<Action>();
        treadmillAction.menuTexts[0] = "Use ";
        treadmillAction.objectName = "<color=cyan>" + gameObject.name + "</color>";
        treadmillAction.serverAction0 += UseTreadmill;
        treadmillAction.orderLevels[0] = -1;
        treadmillAction.menuPriorities[0] = 1;
        treadmillAction.cancelLevels[0] = 1;
        treadmillAction.staticPlayerActions[0] = true;
        treadmillAction.inGame = true;
        treadmillAction.UpdateName();

        Action.cancel1 += Cancel;
        TickManager.beforeTick += BeforeTick;

        Vector2 position = transform.position;
        swTile = TileManager.FindTile(position - new Vector2((float)sizeX / 2, (float)sizeY / 2) + Vector2.one * 0.5f);
        position = swTile + new Vector2(sizeX / 2, sizeY / 2) - Vector2.one * 0.5f;
        transform.position = position;
    }

    void UseTreadmill()
    {
        willUseTreadmill = true;
        if (Tools.PlayerIsAdjacentToLargeObject(swTile, sizeX, sizeY, false) == false)
        {
            Player.player.trueTileScript.ExternalMovement(Tools.NearestTileToPlayer(swTile, sizeX, sizeY));
            return;
        }
        BeforeTick();
    }

    void BeforeTick()
    {
        if (willUseTreadmill && Tools.PlayerIsAdjacentToLargeObject(swTile, sizeX, sizeY, false))
        {
            skillsPanel.SetActive(true);
            PanelButtons.instance.ForceOpen("Inventory");
            willUseTreadmill = false;
        }
    }

    void Cancel()
    {
        willUseTreadmill = false;
    }

    private void OnDestroy()
    {
        Action.cancel1 -= Cancel;
    }
}
