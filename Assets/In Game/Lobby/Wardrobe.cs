using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wardrobe : MonoBehaviour
{
    Action wardrobeAction;
    public GameObject presetPanel;

    Vector2 swTile;
    int size = 3;

    bool willOpenWardrobe = false;

    private void Start()
    {
        wardrobeAction = GetComponent<Action>();
        wardrobeAction.menuTexts[0] = "Check-presets ";
        wardrobeAction.objectName = "<color=cyan>" + gameObject.name + "</color>";
        wardrobeAction.serverAction0 += OpenWardrobe;
        wardrobeAction.orderLevels[0] = -1;
        wardrobeAction.menuPriorities[0] = 1;
        wardrobeAction.cancelLevels[0] = 1;
        wardrobeAction.inGame = true;
        wardrobeAction.UpdateName();

        Action.cancel1 += Cancel;

        Vector2 position = transform.position;
        swTile = TileManager.FindTile(position - Vector2.one);
        position = swTile + Vector2.one;
        transform.position = position;

        TickManager.beforeTick += BeforeTick;
    }

    void OpenWardrobe()
    {
        willOpenWardrobe = true;
        if (Tools.PlayerIsAdjacentToLargeObject(swTile, size, false) == false)
        {
            Player.player.trueTileScript.ExternalMovement(Tools.NearestTileToPlayer(swTile, size));
            return;
        }
        BeforeTick();
    }

    void Cancel()
    {
        willOpenWardrobe = false;
    }

    void BeforeTick()
    {
        if (willOpenWardrobe && Tools.PlayerIsAdjacentToLargeObject(swTile, size, false))
        {
            PanelButtons.instance.ForceOpen("Inventory");
            presetPanel.SetActive(true);
            willOpenWardrobe = false;
        }
    }
}
