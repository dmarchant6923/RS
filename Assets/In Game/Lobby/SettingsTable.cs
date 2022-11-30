using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsTable : MonoBehaviour
{
    Action settingsAction;
    public GameObject settingsPanel;

    Vector2 swTile;
    int sizeX = 2;
    int sizeY = 4;

    bool willOpenSettings = false;

    private void Start()
    {
        settingsAction = GetComponent<Action>();
        settingsAction.menuTexts[0] = "Change-settings ";
        settingsAction.objectName = "<color=cyan>" + gameObject.name + "</color>";
        settingsAction.serverAction0 += OpenSettings;
        settingsAction.orderLevels[0] = -1;
        settingsAction.menuPriorities[0] = 1;
        settingsAction.cancelLevels[0] = 1;
        settingsAction.staticPlayerActions[0] = true;
        settingsAction.inGame = true;
        settingsAction.UpdateName();

        Action.cancel1 += Cancel;

        Vector2 position = transform.position;
        swTile = TileManager.FindTile(position - new Vector2(0.5f, 1.5f));
        position = swTile + new Vector2(0.5f, 1.5f);
        transform.position = position;

        TickManager.beforeTick += BeforeTick;
    }

    void OpenSettings()
    {
        willOpenSettings = true;
        if (Tools.PlayerIsAdjacentToLargeObject(swTile, sizeX, sizeY, false) == false)
        {
            Player.player.trueTileScript.ExternalMovement(Tools.NearestTileToPlayer(swTile, sizeX, sizeY));
            return;
        }
        BeforeTick();
    }
    void BeforeTick()
    {
        if (willOpenSettings && Tools.PlayerIsAdjacentToLargeObject(swTile, sizeX, sizeY, false))
        {
            settingsPanel.GetComponent<SettingsPanel>().OpenPanel();
            willOpenSettings = false;
        }
    }
    void Cancel()
    {
        willOpenSettings = false;
    }

    private void OnDestroy()
    {
        Action.cancel1 -= Cancel;
    }
}
